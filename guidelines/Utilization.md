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

Getting the directory of NFy Mono & trying to access it's files are two different things. And should be
treated as such. Using CTEXT("") is deprecated behaviour.

```cs

GetCurrentDir() // C:\users\bla\appdata\roaming\nmono\

// Would return the same thing, but for readability,
// you use GetCurrentDir() for the current dir
// (For experimentation), and use CTEXT(path) for getting
// Files.
CTEXT("") // WRONG!

CTEXT("songs/hello"); // Example

```
