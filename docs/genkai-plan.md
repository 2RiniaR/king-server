# 限界様（genkai）bot 実装計画書

アクティブメンバー用ロールを自動で付与・剥奪する bot を、既存サーバーアプリに 4 つ目の bot インスタンスとして追加する。

## 1. 要件の整理

| 項目 | 内容 |
| --- | --- |
| 対象ロール | マスタ `genkai_setting` の `ActiveRoleId` |
| 付与条件 | 過去 `ActiveRoleThresholdDays` 日以内に、ギルド内のいずれかのチャンネルでメッセージを投稿した（bot 以外の）メンバー |
| 剥奪条件 | 最終投稿日時から `ActiveRoleThresholdDays` 日が経過した / 投稿記録がない / bot である |
| 更新頻度 | 5 分に 1 回の定期リコンサイル（＋ 投稿時の即時付与、後述） |
| 起動時 | 各メンバーの最終投稿日時テーブルを構築し、ロール状態を一括更新する |
| 制約 | Discord API コールは最小限。全メッセージ履歴の走査はしない |

### 「投稿」とみなすもの
- 通常のユーザーメッセージ（`SocketUserMessage`）。テキストチャンネル・アナウンスチャンネル・ボイスチャンネル内チャット・スレッド・フォーラム投稿を含む
- 除外：bot のメッセージ、Webhook のメッセージ、システムメッセージ（参加通知・ピン留め通知など）
- 編集・リアクションは投稿とみなさない。削除されたメッセージも、受信済みなら投稿済みとして扱う

## 2. 全体設計

### 最終投稿日時をどうやって知るか
Discord API には「メンバーの最終投稿日時」を取得する手段がない。そこで以下の 3 つを組み合わせて、API コールを抑える。

1. **DB への永続化**：最終投稿日時（ユーザーごと）と、処理済みの最後のメッセージ ID（チャンネルごと）を SQLite に保存する。再起動のたびに一からやり直さないようにするため。
2. **ダウンタイム中の差分だけを取得**：起動時は、各チャンネルの「処理済みの最後のメッセージ ID」より後のメッセージだけを、`after` 指定でページング取得する（1 回あたり最大 100 件）。コストは停止中に投稿された件数 / 100 回程度で済む。
3. **Snowflake による範囲の絞り込み**：メッセージ ID には作成時刻が含まれる。
   - 各チャンネルの `LastMessageId` は Gateway の GUILD_CREATE で得られる（API コール不要）。これが処理済み ID 以下、または閾値日数より前ならそのチャンネルは **API コールなしでスキップ**。
   - DB にデータがない初回起動時は、開始点を `SnowflakeUtils.ToSnowflake(now - threshold_days)` にする。これで **閾値期間内のメッセージだけ** を前方向に走査でき、それより古い履歴は一切読まない。

平常時はメッセージ受信イベント（Gateway）で in-memory テーブルを更新するため、定期処理での REST 呼び出しは「ロールの付け外しが必要なメンバーの分」だけになる。

### データフロー

```
[起動]
  DB読込 ─▶ in-memoryテーブル ◀─ 差分バックフィル(REST, 必要なチャンネルのみ)
                  ▲
[稼働中]           │
  MessageReceived ─┘ (即時更新 + ロール未所持なら即時付与)

[5分ごと]
  in-memoryテーブル + メンバー/ロールキャッシュ ─▶ 差分計算 ─▶ 付与/剥奪(REST)
                                                     └▶ DBへフラッシュ
```

## 3. 変更・追加ファイル

### 3.1 マスタ
**スプレッドシートに `genkai_setting` シートを追加**（既存の `*_setting` と同じ `key` / `value` 形式）

| key | value の例 | 説明 |
| --- | --- | --- |
| `ActiveRoleThresholdDays` | `14` | 何日以内に投稿があればアクティブとするか |
| `ActiveRoleId` | `123456789012345678` | アクティブメンバー用ロールの ID |

**`Common/Master/GenkaiSettingMaster.cs`（新規）**
- `MasterTable<string, Setting>` を継承する。`GetString` / `GetInt` / `GetULong` は既存の `EyesSettingMaster` などと同じパターンで実装する。
- プロパティ
  - `int ActiveRoleThresholdDays => GetInt(nameof(ActiveRoleThresholdDays))`
  - `ulong ActiveRoleId => GetULong(nameof(ActiveRoleId))`
- キーは既存マスタと同じく `nameof(...)` によるアッパーキャメルケース（PascalCase）とする。

**`Common/Master/MasterManager.cs`**
- `[field: MasterTable("genkai_setting")] private GenkaiSettingMaster _genkaiSettingMaster;` と、その static アクセサを追加する。

### 3.2 環境変数
**`Common/Environment/EnvironmentManager.cs`**
- `DiscordSecretGenkai` を追加する。
- 本番・開発の VPS 側の環境設定（appsettings / 環境変数）にもトークンの追加が必要（リポジトリ外の作業）。

### 3.3 bot インスタンス
**`Common/Discord/Instances/GenkaiBotInstance.cs`（新規）**
- `DisplayName => "Genkai"`、トークンは `EnvironmentManager.DiscordSecretGenkai`
- `GatewayIntents`：`Guilds | GuildMembers | GuildMessages`
  - `GuildMembers`（特権インテント）：全メンバーの一覧とロール保持状態をキャッシュするために必要
  - `MessageContent` は **不要**（投稿者と時刻だけ見れば足りる）
- `DiscordSocketConfig.AlwaysDownloadUsers = true`：起動時に Gateway 経由で全メンバーをキャッシュする。REST の `GET members` を叩かずに済む。
- `RegisterEvents()`
  - `MessageReceived`：ターゲットギルドの、bot / Webhook 以外の `SocketUserMessage` なら `GenkaiActivityTracker.OnMessage(...)` を呼ぶ
  - `UserLeft`：テーブルからは消さない（再参加に備える）。ロール操作の対象はキャッシュ上の現メンバーだけなので、特別な処理はしない。

**`Common/Discord/DiscordManager.cs`**
- `_genkaiBot` の生成、`InitializeAsync` の `Task.WhenAll` への追加、`GenkaiBot` アクセサを追加する。

**`Program.cs`**
- `RegisterEvents()` に `DiscordManager.GenkaiBot.RegisterEvents()` を追加する。
- 起動時バックフィルは `InitializeStatesAsync` 内ではなく、**バックグラウンドタスク**として開始する（他の bot の起動を止めないため）。完了するまで定期リコンサイルはスキップする。

### 3.4 DB
**`Common/Models/GenkaiMemberActivity.cs`（新規）**
```csharp
public class GenkaiMemberActivity
{
    [Key] public ulong DiscordId { get; set; }
    public DateTime LastMessageAt { get; set; }   // UTC
}
```

**`Common/Models/GenkaiChannelCursor.cs`（新規）**
```csharp
public class GenkaiChannelCursor
{
    [Key] public ulong ChannelId { get; set; }
    public ulong LastMessageId { get; set; }      // 処理済みの最新メッセージID
}
```

**`Common/Models/AppService.cs`**
- `DbSet<GenkaiMemberActivity>` と `DbSet<GenkaiChannelCursor>` を追加する。

**マイグレーション**
- `make migrate name=AddGenkaiActivity` で `Migrations/` 以下を生成する。デプロイ時は既存どおり efbundle で適用される。

### 3.5 アクティビティ管理（コアロジック）
**`Common/Genkai/GenkaiActivityTracker.cs`（新規, `Singleton`）**

状態（in-memory）
- `ConcurrentDictionary<ulong, DateTime> _lastMessageAt`：ユーザー ID → 最終投稿日時（UTC）
- `ConcurrentDictionary<ulong, ulong> _channelCursor`：チャンネル ID → 処理済みの最新メッセージ ID
- `bool IsReady`：起動時バックフィルが終わったか
- `SemaphoreSlim _reconcileLock`：リコンサイル処理の多重実行を防ぐ
- `HashSet<ulong> _dirtyUsers`, `_dirtyChannels`：DB へのフラッシュ対象

主なメソッド
- `OnMessage(SocketUserMessage m)`
  - `_lastMessageAt[author] = max(既存, m.Timestamp)`、`_channelCursor[channel] = max(既存, m.Id)` を更新して dirty にする
  - `IsReady` かつ投稿者がロールを持っていなければ、**その場で付与**する（キャッシュで判定するので、付与不要な場合は API コール 0）。すでに付与処理中のユーザーは `_granting` セットで弾く。
- `InitializeAsync()`：起動時バックフィル（4 章）
- `ReconcileAsync()`：差分計算とロール更新（5 章）
- `FlushAsync()`：dirty なレコードを DB に upsert する

### 3.6 定期ジョブ
**`Events/Genkai/GenkaiActiveRoleRefreshPresenter.cs`（新規, `SchedulerJobPresenterBase`）**
- `MainAsync()` → `GenkaiActivityTracker.Instance.ReconcileAsync()` → `FlushAsync()`

**`Common/Scheduler/SchedulerManager.cs`**
- `_timer.RegisterOn<GenkaiActiveRoleRefreshPresenter>(x => x.Minute % 5 == 0);`
  - 既存の `SlotConditionRefreshPresenter` と同じ立ち上がり検出方式なので、5 分ごとに 1 回だけ実行される。

## 4. 起動時バックフィルのアルゴリズム

```
1. DB から GenkaiMemberActivity / GenkaiChannelCursor を in-memory に読み込む
2. guild.DownloadUsersAsync() の完了を待つ（AlwaysDownloadUsers で起動時に取得済みならスキップ）
3. cutoffId = SnowflakeUtils.ToSnowflake(now - threshold_days)
4. 対象チャンネルを列挙する:
     - guild.TextChannels（テキスト / アナウンス / ボイス内チャットを含む）
     - guild.ThreadChannels（アクティブスレッド。GUILD_CREATE に含まれるため API 不要）
     - bot に ViewChannel + ReadMessageHistory がないチャンネルは除外
5. 各チャンネルについて:
     startId = max(cursor[ch] ?? 0, cutoffId)
     if ch.LastMessageId is null or ch.LastMessageId <= startId:
         continue   // API コールなしでスキップ
     loop:
         msgs = GetMessagesAsync(startId, Direction.After, 100)   // REST 1回
         各メッセージの著者(bot/webhook/system以外)について lastMessageAt を max 更新
         startId = msgs の最大 ID
         if msgs.Count < 100: break
     cursor[ch] = startId
6. IsReady = true → すぐに ReconcileAsync() を 1 回実行 → FlushAsync()
```

**API コール数の見積もり**
- 通常の再起動（数分〜数時間の停止）：「停止中に投稿があったチャンネル数」回程度。ほとんどのチャンネルは `LastMessageId` の比較だけでスキップされる。
- 初回起動（DB が空）：閾値期間内の総メッセージ数 / 100 回。例えば 14 日で 1 万件なら約 100 回。Discord.Net がレート制限を自動で待つため、数十秒〜数分で終わる見込み。
- チャンネルの走査は **逐次** で行う（並列にすると同じレート制限バケットで待たされるだけなので）。

**補足・既知の制約**
- アーカイブ済みスレッドは走査しない。閾値期間内に投稿があってアーカイブされたスレッドだけは取りこぼす可能性がある。ただし、アーカイブ済みスレッドの一覧取得はチャンネル数ぶんの追加コールになり、スレッドだけで発言するメンバーはまれなので、許容する。初回起動時だけの問題で、稼働中はイベントで拾える。必要なら初回起動時だけ `GetPublicArchivedThreadsAsync(before: cutoff)` を有効にするオプションを用意する。
- `ActiveRoleThresholdDays` を後から **増やした** 場合、DB に記録のないメンバー（初回走査の期間より前にしか投稿していない人）はアクティブと判定されない。次に投稿した時点で正しくなる。
- 走査中に届いた `MessageReceived` も同じ dictionary を max 更新するだけなので、競合しても結果は正しい。

## 5. リコンサイル（5 分ごと）

```
if !IsReady: return
if !_reconcileLock.Wait(0): return          // 前回がまだ実行中なら今回はスキップ
roleId = master.ActiveRoleId; days = master.ActiveRoleThresholdDays
role = guild.GetRole(roleId)
if role is null or days <= 0: エラーログを出して終了（誤設定で全員剥奪しないため）

threshold = utcNow - days
for member in guild.Users (キャッシュ):
    shouldHave = !member.IsBot && lastMessageAt[member.Id] >= threshold
    has        = member.Roles.Contains(role)
    if shouldHave && !has: toAdd.Add(member)
    if !shouldHave && has: toRemove.Add(member)

toAdd / toRemove を逐次実行（AddRoleAsync / RemoveRoleAsync, 1人あたり REST 1回）
  - 個別の失敗はログに出して続行（次回のリコンサイルで再試行される）
  - 監査ログ用に reason = "限界様: アクティブ判定" を付ける
変更件数を LogManager.Log で出力
```

- 判定はすべて Gateway のキャッシュ（メンバー・ロール）で行い、**REST を呼ぶのは実際に付け外しが必要なときだけ**。
- 「剥奪は日数経過の時点」という要件に対して、実際には最大 5 分遅れる。許容範囲とする。
- 手動で付与されたロールも、条件を満たさなければ剥奪する（このロールは bot が専有管理する前提）。
- bot がこのロールを持っていれば剥奪する。

## 6. Discord 側の事前準備（手作業）

1. Developer Portal で「限界様」アプリを作成し、bot トークンを発行する
2. **Server Members Intent**（特権インテント）を有効化する
3. 権限 `View Channels` / `Read Message History` / `Manage Roles` で招待する
4. サーバー設定のロール一覧で、**genkai の bot ロールを `ActiveRoleId` のロールより上** に配置する（これをしないと付け外しが 403 になる）
5. 対象にしたいすべてのチャンネル（プライベートチャンネルを含む）で閲覧権限を与える
6. 本番と開発の環境変数に `DiscordSecretGenkai` を追加する。スプレッドシートに `genkai_setting` シートを追加する。

## 7. エラーハンドリング・運用

- 起動時バックフィルで特定チャンネルが失敗しても（403 など）、ログに出して他のチャンネルの処理を続ける。
- マスタリロード（`@isso reload`）後は、次回のリコンサイルから新しい閾値とロール ID が反映される。
- 例外は既存の `PresenterBase.RunAsync` の仕組みで捕捉する。`AppException` は Isso のメインチャンネルに通知される（既存の挙動）。
- 起動時バックフィルの結果（走査したチャンネル数、取得したメッセージ数、API コール数）と、各リコンサイルの付け外し件数をログに出す。

## 8. 実装ステップ

1. マスタ（`GenkaiSettingMaster`、`MasterManager` への登録）と環境変数を追加する
2. DB モデル、`AppService`、マイグレーションを追加する
3. `GenkaiBotInstance`、`DiscordManager`、`Program.cs` への登録（ログインとイベント購読まで）
4. `GenkaiActivityTracker`（メッセージ受信時の更新、DB の読込とフラッシュ）
5. 起動時バックフィル
6. リコンサイルと定期ジョブの登録
7. 動作確認（開発サーバー）
   - 閾値を 1 日などに設定し、投稿で即時付与されること、日付を過去に書き換えた DB レコードで剥奪されることを確認する
   - 再起動時に、停止中に投稿があったチャンネルだけを取得していることをログで確認する
   - bot にロールを手動で付けても剥奪されることを確認する
