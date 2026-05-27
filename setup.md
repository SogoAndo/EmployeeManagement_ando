# 総合演習 初期セットアップ手順

## 1. 親ディレクトリ作成

1. VS Codeを起動し、WSLにリモート接続する。
2. メニューから、[ファイル(F)]-[フォルダを開く Ctrl+K Ctrl+O]を選択し、表示されたダイアログボックスで、CS_Training/WebAppを選択後[OK]ボタンをクリックする。
3. ターミナルを開き、以下のコマンドを実行して、EmployeeManagement_yournameディレクトリを作成する。
```bash
mkdir EmployeeManagement_yourname
```

---

## 2. ソリューション作成

1. メニューから、[ファイル(F)]-[フォルダを開く Ctrl+K Ctrl+O]を選択し、表示されたダイアログボックスで、EmployeeManagement_yournameを選択後[OK]ボタンをクリックする。
2. ターミナルを開き、以下のコマンドを実行して、プロジェクトのソリューションファイルを作成する。
```bash
dotnet new sln -n EmployeeManagement
```

※ .NET10 環境では `.slnx` が生成される。

---

## 3. docs フォルダ作成
これ以降は、手順に従って作業を進める。
```bash
mkdir docs
mkdir docs/01_db
mkdir docs/02_design
mkdir docs/03_test
mkdir docs/04_presentation
mkdir docs/05_etc
```

設計書・ER図・テストケースを格納します。

---

## 4. Git 初期化

```bash
git init
cp ../WebApp_Excercise/.gitignore ./
```

開発開始時点から変更履歴を管理します。

---

## 5. Webプロジェクト作成

```bash
dotnet new mvc -n EmployeeManagement.Web
```

---

## 6. Testプロジェクト作成

```bash
dotnet new mstest -n EmployeeManagement.Test
```

---

## 7. ソリューションへ追加

```bash
dotnet sln add EmployeeManagement.Web
dotnet sln add EmployeeManagement.Test
```

---

## 8. Testプロジェクトから参照設定

```bash
dotnet add EmployeeManagement.Test reference EmployeeManagement.Web
```

---

## 9. DDD 3層用ディレクトリ作成

### Webプロジェクト

```bash
mkdir EmployeeManagement.Web/ViewModels
mkdir EmployeeManagement.Web/Applications
mkdir EmployeeManagement.Web/Applications/Services
mkdir EmployeeManagement.Web/Infrastructures
mkdir EmployeeManagement.Web/Infrastructures/Repositories
mkdir EmployeeManagement.Web/Infrastructures/Entitys
```

---

### Testプロジェクト

```bash
mkdir EmployeeManagement.Test/Applications
mkdir EmployeeManagement.Test/Infrastructures
mkdir EmployeeManagement.Test/Controllers
```

---

## 10. 初回ビルド&run

```bash
dotnet build
dotnet run --project EmployeeManagement.Web
```
ASP.NET Core の初期画面がブラウザに表示されたことを確認したら、ターミナルでCtrl＋Cで停止する。

---

## 11. Git 初回コミット
1. Githubにログインし、publicなリポジトリを作成する。

    ※リポジトリ名：親ディレクトリ名
    （例：EmployeeManagement_yourname）
```bash
git add .
git commit -m "プロジェクト作成"
```

---

## 12. GitHubへPush

```bash
git branch -M main
git remote add origin <GitHub URL>
git push -u origin main
```
作成したリポジトリに、講師を招待する

---

# 推奨ディレクトリ構成

```text
EmployeeManagement_Kudo
├─ docs
├─ EmployeeManagement.Web
│   ├─ Controllers
│   ├─ Views
│   ├─ ViewModels
│   ├─ Applications
│   │   └─ Services
│   └─ Infrastructures
│       ├─ Repositories
│       └─ Entitys
│
└─ EmployeeManagement.Tests
    ├─ Applications
    ├─ Infrastructures
    └─ Controllers
```

---

# docs フォルダ例

```text
docs
├─ setup.md
├─ table_definition.xlsx
├─ test_case.xlsx
├─ er_diagram.drawio
├─ class_diagram.drawio
└─ sequence_diagram.drawio
```
