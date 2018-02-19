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

Implemented syntaxes
----
__Variables__<br>
일반적인 대입 연산을 이용하여 변수를 선언합니다. 변수는 대입과 동시에 생성됩니다.
```
a = 10
```

__Expressions__<br>
일반적인 수식 계산이 가능합니다. 수식에는 변수를 사용할 수 있습니다. 
```
4 + 4 * 4

a + a * a - b
```

__Values and functions__<br>
You can bind C# values and functions to MiniScript context.
```
// Value
script.Bind("a", 10);

// Function
script.Bind("sum", (a, b) => a + b);
```

__Function call__<br>
```
b = sum(1, 10)
```

Unimplemented syxtaxes
----
* Comment
* Const variable
* If statement
* For, While statement
* 스크립트를 통한 함수 정의
