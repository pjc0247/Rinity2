MiniScript
====

미니 스크립트는 __Rinity2__에 기본적으로 내장되어있는 간단한 표현식 인터프리터입니다.

```cs
using Rinity.MiniScript2;

var script = new Interpreter();

script.Exec("a = 12"); // 12
script.Exec("a + a"); // 24

script.Bind("b", 100);
script.Exec("a + b"); // 112
```
