StringInterpolation
====

```cs
int value = 10;

string Foo() {
    // 10
    return StringInterpolation.Bind("{{value}}", gameObject);
}
```