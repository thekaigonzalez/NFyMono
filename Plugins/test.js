function onNMonoEngineStart(env) {
	NJPrint("Hi")
	NJPrint("This: " + env["TestVar"]);
	// NJPlaySong("Cherry Blossoms");
}

function onNMonoTick(inPlaylist) {
    if (inPlaylist) {
        NJPrint("in Playlist!");
    }
}

// test plugin