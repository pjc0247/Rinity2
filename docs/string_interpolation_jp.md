String Interpolation
====

__StringInterpolation__ enables you to write more clear code when you needs to concatenate serveral values above string literals.<br>
(Likes a string interpolation feature in C# 6.0, but it provides less features and error-prone.) 

```cs
int playerHp = 100;

Debug.Log("Player HP : {{playerHp}}");
```

String interpolation does not perform a `Find & Replace`. __Rinity2__ will make additional codes for interpolation in compile time.

```cs
var str = "Player HP : {{playerHp}}";

// X
str = str.Replace("{{playerHp}}", playerHp.ToString());

// O
str = "Player HP : " + playerHp.ToString();
```

Accepted Values
----
* Local variables
* Method's parameters
* Properties
* Fields

Limitations (Non-Accepted Values)
----
* Expressions
    * such as `{{1 + 2}}` or `{{Sum(1, 2)}}`
* Captured variables in lambda expr body (partial)