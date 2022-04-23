function onNMonoEngineStart(_e) {
    NJLog("Hello from {}!", "JavaScript")
}

// function onNMonoTick() - loud function

function onNMonoPSongChanged(_n, sn) {
    NJPrint(sn + " is the name of the new song")
}

function onNMonoPlaylistEnded(_idx) {
    NJPrint("Aww no more playlist!")
}

