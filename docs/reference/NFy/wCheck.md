```cs

public string wCheck(string s, string[] sp)

```

Checks the spec.

as NFyMono updates, more audio files will be supported, and to check for said audio files; it'll need to filter them out.

so it's checking if `s` ends with any of the `sp` spec extensions. Commonly used with [GetSpec](./GetSpec)

It returns the give spec (if found.) Or "N".