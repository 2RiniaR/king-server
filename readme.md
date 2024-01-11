# king-server

## デプロイ方法のメモ

1. dockerイメージのビルド

```shell
# 開発環境
$ docker build . -t [docker_user_id]/king-server
```

2. dockerイメージのpush

```shell
# 開発環境
$ docker push [docker_user_id]/king-server
```

3. 実行環境のリフレッシュ

```shell
# デプロイ先にログインして、以下を実行
# 初回の場合は、適当なディレクトリを用意して Environment/ 以下をSCP等でコピーしてください（書き換え必須の部分があるため、適宜書き換えてください）
$ docker pull [docker_user_id]/king-server
$ docker compose down
$ docker compose up -d
```