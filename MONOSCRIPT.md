# MONOScript Library

## NJPrint(text)

Print to the console (and `specials` console!)

## NJCreateDir(name)

Function to create a directory from NFy.

## SetupNJMonoDirs()

Function to setup the directories (calling `SetupAPI.SetupNFy()`)

## NJPlaySongByName(songName)

Calls the main song function (with songname)

# MONOScript Events

## onNMonoEngineStart

Called on initial load: the ready function.

Usually defined like so:

```js

function onNMonoEngineStart(env) {
    // env is any environment variables, so say the binary has TEST=1 passed into it,
    // you'd be able to call env.TEST and view it's value.

    // This is good for plugin specific code, that could possibly need variables to enable them.
}


```


## onNMonoTick

Called every frame, called during the _Process function, which could
possibly lead to performance issues, if too many plugins are installed.

it's also given the "InPlaylist" variable, to see if the current song list is a 
playlist, or the general song array.

Usually defined like so:

```js
function onNMonoTick(inPlaylist) {
    if (inPlaylist) {
        NJPrint("in Playlist!");
    }
}
```