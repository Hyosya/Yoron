# Yoron
## The Programing Language on .NET Framework
- FizzBuzzが書けるレベルの言語機能を持たせました。
- 構造体宣言やクラスや配列などのインスタンス生成は実装していません。


### 実装済み機能
#### 変数宣言と int, bool, string型のリテラル
    var a = 1
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
    var a = 2;
    var b = 1;
    if a > b 
    {
        Writeln(a)
    }
---
#### while
    while(true)
    {
        Writeln("無限ループ")
    }