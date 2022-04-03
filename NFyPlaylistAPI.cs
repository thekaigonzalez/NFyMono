using System;
using Godot;
using Godot.Collections;


public class NFyPlaylist {
    
    private string file = "";
    private string[] songslist = {};

    private int it = 0;
    public void Open(string f) {
        file = f;
    }

    //: Parse the current file (from path) into a JSON object -> array of items
    public void Parse() {
        File s = new File();
        s.Open(file, File.ModeFlags.Read);

        var res = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(s.GetAsText());
        songslist = res["songs"];
    }

    public string Current_Song() {
        if (songslist.Length >= it) {
            return songslist[it];
        } else {
            return "N";
        }
    }

    public int Current_SongIndex() {
        return it;
    }

    public string[] Songs() {
        return songslist;
    }

    public void SetIndex(int f) {
        it = f;
    }

    public int Playlist_Length() {
        return songslist.Length;
    }

    public string Next_Song() {
        it += 1;

        return Current_Song();
    }
}