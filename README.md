# Yoron
## The Programing Language on .NET Framework
- FizzBuzzが書けるレベルの言語機能を持たせました。
- クラスや構造体の宣言、メソッドや関数の定義、クラスや配列などのインスタンス生成は実装していません。
- .NET Framework 4.8が必要です。
- バックエンドはExpression Treeです。
- テスト駆動開発で作りました。
---

### Hello World
1. お好きなテキストエディタで`Writeln("Hello World")`と書いてhello.yrとして保存
2. コマンドプロンプトなどで`Yoron.exe hello.yr`
3. hello.exeが出力されるので実行してください。
---

### 実装済み機能
#### 変数宣言と int, bool, string型のリテラル
##### 行末のセミコロンはオプションです。
    var a = 1;
    var b = true
    var c = "文字列"

---
#### intの演算
    var x = 4
    var y = 2
    var z = x + y
    z = x - y
    z = x * y
    z = x / y
    z = x % y
---
#### 比較演算
    var b = x == y
    v = x != y
    b = x > y
    b = x < y
    b = x <= y
    b = x >= y
    var t = true
    var f = false
    c = t == f
    c = t != f
    c = t && f
    c = t || f
---
#### 文字列結合
    var x = "文字" + "列"

#### 標準入出力
    var input = Readln()
    Writeln(input)
---
#### if
##### ifの後の条件の括弧はオプションです
    var a = 2;
    var b = 1;
    if a > b 
    {
        Writeln(a)
    }
---
#### while
##### 条件の括弧はオプションです
    while(true)
    {
        Writeln("無限ループ")
    }