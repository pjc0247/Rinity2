String Interpolation
====

__StringInterpolation__ 기능은 문자열 리터럴 사이에 여러개의 변수를 끼워 넣고 싶은 경우에 좀 더 깔끔한 코드를 작성할 수 있도록 해줍니다.<br>
(C# 6.0의 문자열 보간의 하위 호환 버전입니다.)<br>
<br>
문자열 리터럴 내부에 직접 `{{value_name}}`과 같이 작성하면 런타임에 값이 바인딩됩니다.<br>
Here's a basic example of StringInterpolation:

```cs
int playerHp = 100;

Debug.Log("Player HP : {{playerHp}}");
```

보간 작업은 `Find & Replace` 같은 문자열 치환으로 수행되지 않습니다. 컴파일 타임에 보간을 위한 추가 코드가 생성됩니다.

```cs
var str = "Player HP : {{playerHp}}";

// Your code will be replace like below in compile time.
str = "Player HP : " + playerHp.ToString();
```

List of values you CAN bind
----
* Local variables inside method.
* Parameters
* Properties
* Fields

List of values you CAN'T bind
----
* Expressions
    * `{{1 + 2}}` or `{{Sum(1, 2)}}`
* Captured local variables outside lambda function (Partial support)