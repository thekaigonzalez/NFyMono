```cs
public bool Dull() 
```
So, NFyRotation class has two overloads to prevent NullReferenceException,

```cs
()

&

(string[])

```

the first one is in place to give the class a value, which serves no purpose.

This function allows you to check if the class is a dummy class `() overload`, or an actually running rotation. `(string[]) overload`.