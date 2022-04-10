```cs
public string[] GetSpec()
```

Returns the current spec,

For example, in NFy Mono 7, the spec is [wav, ogg, mp3], Mono 6, it would be [wav, ogg], which is used by OpenCorrect() for checking which format exists.