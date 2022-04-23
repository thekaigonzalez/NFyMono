# NJMono's JavaScript API

> *So many features, but there's never enough. . .*

NFy Mono contains a plugin API to extend your NFy Mono Experience, even further.

The JS API contains:

- Events
- A Standard Library
- Documentation

When you load NFy Mono, it will try to load a plugins/ directory,
which contains all of the .js files that contain plugin code.

A list of basic events is as shown:

```js
function onNMonoEngineStart(_e) {
    NJLog("Hello from {}!", ["JavaScript"])
}

function onNMonoTick() {
    NJPrint("Every frame!");
}

function onNMonoPSongChanged(_n, sn) {
    NJPrint(sn + " is the name of the new song")
}

function onNMonoPlaylistEnded(_idx) {
    NJPrint("Aww no more playlist!")
}
```

There's also functions to automate NFy Mono's basic UI Tasks, such as

- Playing a song
- Stopping a song
- Setting volume
- Setting up NFy's directories (Using SetupAPI)

And special functions (usually only found on the CLI)

- Disabling VSign
- Creating Directories
- Printing to console