# 推奨コマンド一覧

## 開発・ビルド・実行
- `dotnet run` - アプリケーションをローカルで実行
- `make run` - Makefile経由でアプリケーションを実行

## データベース操作
- `make migrate name=<マイグレーション名>` - 新しいEF Coreマイグレーションを作成・適用
- `dotnet ef migrations add <name>` - 新しいマイグレーションを作成
- `dotnet ef database update` - 保留中のマイグレーションを適用

## Docker デプロイメント
- `make push app_name=<docker_user_id>/king-server` - Dockerイメージをビルド＆プッシュ
- `docker build . -t <docker_user_id>/king-server` - Dockerイメージをビルド
- `docker push <docker_user_id>/king-server` - Dockerイメージをレジストリにプッシュ

## Git操作（Darwin/macOS）
- `git status` - 現在の変更状況を確認
- `git diff` - 変更内容を確認
- `git add .` - すべての変更をステージング
- `git commit -m "message"` - コミット作成
- `git push` - リモートリポジトリにプッシュ
- `git pull` - リモートの変更を取得

## ファイル操作（Darwin/macOS）
- `ls -la` - ファイル一覧（隠しファイル含む）
- `find . -name "*.cs"` - C#ファイルを検索
- `grep -r "pattern" .` - パターンを再帰的に検索
- `cat file` - ファイル内容表示
- `mkdir directory` - ディレクトリ作成

## .NET SDK コマンド
- `dotnet build` - プロジェクトをビルド
- `dotnet restore` - パッケージを復元
- `dotnet publish -c Release` - リリース用にパブリッシュ

## プロジェクト固有
- データベースファイル: `Environment/king-server-dev.db`
- 設定ファイル: `Environment/appsettings*.json`
- Google認証: `Environment/google-credential_*.json`
</content>
</invoke>