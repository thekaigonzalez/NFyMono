# Api Differences (Mono VS Normal)

## Playlists VS Rotations

**Rotations** are songs in a certain order, a faster reimplementation of the **Playlists** in normal NFy.

They work like this:

```
If index 0: Play initial song


if song ends and next available using nextExists() then Move index and play song

Redo
```

Playlists work like this:

```

if 0 initial
if next index available play that song
if index after than then play

if end replay current song

```

## 