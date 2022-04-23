/// newinfo plugin

function onNMonoEngineStart(env) {
    NJLog("Environment variables: {}", [env.toString()])
}

function onNMonoPSongChanged(idx, songName) {
    NJLog("Song changed!\nSong's index: {}\nThe Song Name: {}", [idx.toString(), songName]);
}