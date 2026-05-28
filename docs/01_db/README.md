# データベースセットアップ

このフォルダには、社員情報管理システムを別PCで動かすためのPostgreSQL用SQLを配置しています。

## 前提

- PostgreSQLがインストールされていること
- `postgres` ユーザーでDB作成権限があること
- アプリの接続先は `EmployeeManagement.Web/appsettings.json` の内容に合わせること

現在のアプリ設定:

```text
Host=localhost;Port=5432;Database=employee_management;Username=postgres;Password=training;
```

## セットアップ手順

リポジトリ直下で次のコマンドを実行します。

```bash
psql -h localhost -p 5432 -U postgres -f docs/01_db/00_setup_database.sql
```

パスワードを求められた場合は、PostgreSQLの `postgres` ユーザーのパスワードを入力してください。

## 作成されるもの

- データベース: `employee_management`
- テーブル:
  - `departments`
  - `employees`
  - `users`
- 初期データ:
  - 部門データ
  - 社員データ
  - 人事部社員のログイン情報

## 初期ログイン情報

以下のログインIDでログインできます。初期パスワードはいずれも同じです。

```text
ログインID: 1001 / 1004 / 1005 / 1006
パスワード: Password123!
```

ログインできるのは人事部に所属する社員のみです。

## ファイル構成

```text
docs/01_db
├─ 00_setup_database.sql      一括セットアップ用
├─ 01_create_database.sql     データベース作成用
├─ 02_create_tables.sql       テーブル作成用
├─ 03_insert_initial_data.sql 初期データ投入用
├─ er_diagram.dio             ER図
└─ table_definition.xlsx      テーブル定義書
```

## 注意

`02_create_tables.sql` は既存の `users`、`employees`、`departments` テーブルを削除してから作成し直します。
既に必要なデータが入っている環境では、実行前にバックアップを取得してください。
