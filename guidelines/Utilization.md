# Utilization

When you're trying to get a path for the current directory, use the `CTEXT` function to wrap the binary's path around the given path.

```cs

listDir("hello") // WRONG!

listDir(CTEXT("hello")) // CORRECT!

```

When you're trying to print to the console, you should use the `print()` function to print to NFy Mono's console (**specials**), and the default `stdout`.

```cs

Console.WriteLine("Hello!") // WRONG

print("Hello!") // CORRECT!

```
