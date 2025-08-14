# コードスタイルと規約

## 命名規則
- **名前空間**: `Approvers.King` がルート名前空間
- **クラス名**: PascalCase (例: `GachaCommandPresenter`, `DiscordManager`)
- **メソッド名**: PascalCase、非同期メソッドは `Async` サフィックス付き
- **プロパティ**: PascalCase
- **プライベートフィールド**: camelCase または _camelCase

## C# 言語機能
- **Target Framework**: .NET 8
- **Nullable Reference Types**: 有効 (`<Nullable>enable</Nullable>`)
- **Implicit Usings**: 有効 (`<ImplicitUsings>enable</ImplicitUsings>`)
- **Top-level statements**: 使用していない（Programクラスを明示的に定義）

## アーキテクチャパターン
- **Presenter パターン**: 各機能は `PresenterBase` を継承
- **Manager パターン**: 共通機能は Manager クラスで管理
- **Singleton パターン**: `Singleton<T>` クラスを使用
- **非同期処理**: Discord.Net のイベントは全て非同期（`Task` ベース）

## ディレクトリ構成規約
- **Common/**: 共通機能・ユーティリティ
- **Events/**: 機能別のイベントハンドラ（Presenter）
- **Models/**: Entity Framework のデータモデル
- **Migrations/**: EF Core のマイグレーションファイル

## ファイル命名
- クラス名と同じファイル名
- 1ファイル1クラスが基本
- Presenter クラスは `~Presenter.cs` の命名

## 非同期処理
- Discord.Net のイベントハンドラは全て非同期
- メソッド名に `Async` サフィックスを付ける
- `Task` または `Task<T>` を返す

## Entity Framework
- Code First アプローチ
- マイグレーションで DB スキーマ管理
- DbContext は `AppService` クラス

## 依存性注入
- Singleton パターンで管理
- 各 Manager クラスは Singleton として実装

## エラーハンドリング
- try-catch で適切に例外処理
- ログ出力は LogManager を使用
</content>
</invoke>