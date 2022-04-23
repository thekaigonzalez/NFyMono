# MONOScript Library

## NJPrint(text)

Print to the console (and `specials` console!)

## NJCreateDir(name)

Function to create a directory from NFy.

## SetupNJMonoDirs()

Function to setup the directories (calling `SetupAPI.SetupNFy()`)

## NJPlaySongByName(songName)

Calls the main song function (with songname)

## NJLog(str: string, fmt: array[])

Similar to [NJPrint](#njprinttext), provides a function for printing the text in a **formatted** method:

```js

NJLog("Hello {}!", [ "world" ]);

```

## NJSetVol(volume: Float)

Sets the volume for the main stream.

( THIS FUNCTION ALSO SETS THE BAR'S VALUE TO THE "volume" VALUE!!! )

## NJPauseStream() 

Pauses the current stream: such as when you're playing a song, this function will pause it.

## NJClearOutput()

Clears the console output.

# MONOScript Events

## onNMonoEngineStart(variables: dictionary)

Called on initial load: the ready function.

Usually defined like so:

```js

function onNMonoEngineStart(env) {
    // env is any environment variables, so say the binary has TEST=1 passed into it,
    // you'd be able to call env.TEST and view it's value.

    // This is good for plugin specific code, that could possibly need variables to enable them.
}


```


## onNMonoTick(IN_A_PLAYLIST)

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

## onNMonoPlaylistEnded(p_ind)

Called once the playlist that's running ends.

This function provies the current index (which is usally 0.)

## onNMonoPSongChanged(_index, current_song)

Called every time the playlist's song changes.

```js
function onNMonoPSongChanged(index, songName) {
    /// Print the new song

    NJPrint("Song: " + songName);
}
```
