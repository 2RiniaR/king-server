# King Server プロジェクト概要

## プロジェクトの目的
Discord bot アプリケーション。ガチャ（くじ引き）とスロットゲームシステムを管理する。

## 技術スタック
- **言語**: C# (.NET 8)
- **フレームワーク**: 
  - Discord.Net (3.12.0) - Discord bot機能
  - Entity Framework Core (8.0.10) - ORM
  - SQLite - データベース
- **外部サービス**:
  - Google Sheets API - マスタデータ管理
  - OpenAI API (Betalgo.OpenAI) - AI機能
- **コンテナ**: Docker (Linux/AMD64)
- **ビルドツール**: dotnet CLI, Make

## プロジェクト構造
```
king-server/
├── Program.cs              # エントリポイント
├── Common/                 # 共通コンポーネント
│   ├── Models/            # データモデル (User, Gacha, Slot, GachaItem)
│   ├── Discord/           # Discord関連
│   ├── Master/            # マスタデータ管理
│   ├── Scheduler/         # スケジュール管理
│   ├── Time/              # 時間管理
│   └── Utility/           # ユーティリティ
├── Events/                # イベントハンドラ
│   ├── Gacha/            # ガチャ機能
│   ├── Slot/             # スロット機能
│   ├── Admin/            # 管理コマンド
│   ├── Marugame/         # 特殊トリガー
│   ├── DailyReset/       # 日次リセット
│   └── MonthlyReset/     # 月次リセット
├── Migrations/            # EF Core マイグレーション
└── Environment/           # 環境設定ファイル

```

## データベース
- SQLite使用 (king-server-dev.db)
- Entity Framework Core でマイグレーション管理
- AppService が DbContext として機能

## メッセージ処理フロー
1. Discord メッセージ受信
2. bot メンション or トリガーフレーズチェック
3. 内容に応じた Presenter へルーティング
4. ビジネスロジック実行
5. Discord 経由で応答

## タイムゾーン
Asia/Tokyo (JST)
</content>
</invoke>