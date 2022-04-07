using Godot;
using System;
using System.Collections.Generic;
public class NFy : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public float sp = 0;
    public float SongLength = 0;

    public bool ed = false;

    public bool PLAYING_ARRAY = false;

    NFyRotation m;
    
    /*
    Official Naming convention for nodes
    */

    /// Discord GDScript API
    public void ChangeActivity(string state, string desc) {
        Script gdclass = ResourceLoader.Load("res://Extensions/discordapi.gd") as Script;
        gdclass.Call("change_activity", state, desc);
    }

    public string GetTimeFormat(float sec) {
        Script gdclass = ResourceLoader.Load("res://Extensions/discordapi.gd") as Script;
        return (string) gdclass.Call("parse_timer", sec);
    }

    /// CheckEach() returns true if the path is found with the extensions in spec,
    /// it returns the first instance, therefore all other members of "spec" are discarded.
    public bool CheckEach(string p, string[] spec) {
        foreach (string s in spec) {
            if (p.Extension() == s) return true;
            else continue;
        }
        return false;
    }

    /// CheckEachS() returns the extension that was found on the give path.
    [Obsolete("CheckEachS() has been deprecated for CheckEach() and GetCurrentSongIfAny(). Please use those methods instead.")]
    public string CheckEachS(string p, string[] spec) {
        foreach (string s in spec) {
            if (p.Extension() == s) return s;
            else continue;
        }
        return "";
    }

    public Panel getNFyScreen() {
        return GetNode<Panel>("NFYSCREEN");    
    }
    
    public AudioStreamPlayer getNFyStream() {
        return GetNode<AudioStreamPlayer>("NfyStream");
    }
    public ProgressBar getNFyBar() {
        return getNFyScreen().GetNode<ProgressBar>("NFyBar");
    }
    
    public OptionButton getNFySongList() {
        return getNFyScreen().GetNode<OptionButton>("CurrentSongNF");
    }

    public bool SpecialsEnabled() {
        if (OS.GetCmdlineArgs().Length >= 1) {
            if (OS.GetCmdlineArgs()[0] == "--specials") { return true; } else { return false; }
        }
        return false; // probably not
    }
    public void PrintToConsole(string text) {
            GetNode<TextEdit>("CON").Text += text + "\n";
    }

    public string GetCurrentSongIfAny() {
        OptionButton sf = getNFySongList();
        string song = "";
        song = sf.GetItemText(sf.GetSelectedId());
        return song;
    }

    [Obsolete("Soon to be removed in NFyMono 4, this API function is no longer in support.")]
    public string Attach(string s1, string s2) {
        return s1 + s2;
    }
    /// @desc Check the spec
    public string wCheck(string s, string[] sp) {
        foreach(string str in sp) {
            if (DirExists(s + "." + str)) return s + "." + str;
            if (System.IO.File.Exists(s + "." + str)) return s + "." + str;
        }
        return "";
    }

    public void OpenSong(string path)
    {        

        AudioStreamPlayer audSound = getNFyStream();

        File file = new File();
        Error err = file.Open(path, File.ModeFlags.Read);
        byte[] b = file.GetBuffer((int)file.GetLen());


        if(path.Extension() == "wav")
        {
            AudioStreamSample a = new AudioStreamSample();
            a.Format = AudioStreamSample.FormatEnum.Format16Bits;
            a.Stereo = true;
            a.Data = b;
            SongLength = a.GetLength();
            audSound.Stream = a;   

        }

        else if(path.Extension() == "ogg")
        {
            AudioStreamOGGVorbis d = new AudioStreamOGGVorbis();
            d.Data = b;
            SongLength = d.GetLength();
            audSound.Stream = d;   
        }

        audSound.Play();
    }
    public string CurrentSongPath() {
        return CTEXT(wCheck("songs/" + GetCurrentSongIfAny(), GetSpec()));
    }

    
    public void _on_ChangeTheme_pressed() {
        if (ed == false) {
            getNFyScreen().Theme = GD.Load<Theme>("res://Themes/NFyDarker/DarkerNFy.tres");
            ed = true;
            
        } else {
            getNFyScreen().Theme = GD.Load<Theme>("res://Themes/res://Themes/ClassicNFy/NFyClassic.tres");
            ed = false;
        }
    }
    public string[] GetSpec() {
        string[] spec = {"wav", "ogg"};
        return spec;
    }
    public void OpenCorrect(string name) {
        
        OpenSong(CTEXT(wCheck("songs/" + name, GetSpec())));
    }

    /// Returns the absolute path
    public string CTEXT(string basel)
    {
        return OS.GetExecutablePath().GetBaseDir() + "/" + basel;
    }

    public bool DirExists(string d) {
        if (System.IO.Directory.Exists(CTEXT(d))) { return true; } 
        else { return false; }
    }

    public string[] listDir(string dname) {
        return System.IO.Directory.GetFiles(dname);
    }
    
    public string ParseSongEntry(string B) {
        B = B.Replace("\\", "/"); // Convert to cross platform format
        return System.IO.Path.GetFileNameWithoutExtension(B);
    }

    public void SongPreload() {

        if (DirExists("songs")) { // Song preload
            foreach (string item in listDir("songs")) {
                if (!item.EndsWith(".import") && CheckEach(item, GetSpec())) { // possibly fix non-audio files showing up
                    string item_parsed = ParseSongEntry(item);
                    Console.WriteLine(item_parsed);
                    getNFySongList().AddItem(item_parsed);
                }
            }
        }
    }
    public void PlaylistPreload() {

        if (DirExists("playlists")) {
            foreach (string item in listDir("playlists")) {
                if (item.EndsWith(".json")) { 
                    string item_parsed = ParseSongEntry(item);
                    Console.WriteLine(item_parsed);
                    GetNode<OptionButton>("NFYSCREEN/Playlists").AddItem(item_parsed);
                }
            }
        }
    }

    public string getPlaylistName() {
        return GetNode<OptionButton>("NFYSCREEN/Playlists").GetItemText(GetNode<OptionButton>("NFYSCREEN/Playlists").GetSelectedId());
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PrintToConsole("Checking for specials");
        if (SpecialsEnabled()) GetNode<Button>("NFYSCREEN/EnableConsole").Visible = true;
        PrintToConsole("Loading setup daemon");
        SetupAPI.SetupNFy();
        PrintToConsole("Preloading songs into list");
        SongPreload();
        PrintToConsole("Setting up the Discord presence from GDScript API");
        ChangeActivity("No Song Loaded", "On NFy MONO");
        
        PlaylistPreload();
        m = new NFyRotation();
    }

    public void _on_UP_toggled(bool t) {

        if (t) {
            string[] pl = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(System.IO.File.ReadAllText(CTEXT("playlists/" + getPlaylistName() + ".json")))["songs"];
                
            m = new NFyRotation(pl);

            PLAYING_ARRAY = true;

            OpenCorrect(m.getCurrentSong());
        } else {
            PLAYING_ARRAY = false;
            m = new NFyRotation();
            OpenCorrect(GetCurrentSongIfAny());
        }
    }

    public void _on_PFUK_pressed() {
        OS.ShellOpen("https://www.stchadshandforth.org.uk/pdf/Prayers%20for%20Ukraine.pdf");
    }
    
    
    public override void _Input(InputEvent inputEvent) {
        if (Input.IsKeyPressed(((int)KeyList.L))) {
            getNFyStream().Seek(getNFyStream().GetPlaybackPosition() + 10);
        }
        else if (Input.IsKeyPressed(((int)KeyList.J))) {
            float newpos = getNFyStream().GetPlaybackPosition() - 10;
            if (newpos <= 0) {
                newpos = 0;
            }
            getNFyStream().Seek(newpos);
        }

        else if (Input.IsKeyPressed(((int)KeyList.K))) {
            if (getNFyStream().Playing) {
                getNFyStream().Stop();
            } else {
                getNFyStream().Play();
            }
        }
    }

    public void _on_EnableConsole_pressed() {
        GetNode<TextEdit>("CON").Visible = true;
    }

    public void _on_Button_pressed() {
        
        
        if (getNFyStream().Stream == null)
        {
            if (!getNFyStream().Playing) {
                OpenCorrect(GetCurrentSongIfAny());
                getNFyBar().MaxValue = SongLength;
            } else {
                getNFyStream().Stop();
            }
            
        
        } else {
            if (!getNFyStream().Playing) 
            getNFyStream().Play(sp);
            else {
            getNFyStream().Stop();
            }
        }

    }

    public void _song_change(int index) {
        OpenCorrect(GetCurrentSongIfAny()); // you should just override this right?
    }

    public void _onStopRequested() {
        if (getNFyStream().Stream != null && getNFyStream().Playing) {
            // try to save position
            sp = getNFyStream().GetPlaybackPosition();
            getNFyStream().Stop();
        }
    }
    // Contains code for a custom loop feature
    public void LoopHandler() {
    
        if (getNFyStream().GetPlaybackPosition() >= SongLength && GetNode<CheckButton>("NFYSCREEN/Loop").Pressed && m.Dull()) {
            Console.WriteLine("play");
            OpenCorrect(GetCurrentSongIfAny()); // replay (resets every variable)
        }
        else if (getNFyStream().GetPlaybackPosition() >= SongLength && !m.nextExists() && !m.Dull() && !GetNode<CheckButton>("NFYSCREEN/LoopPL").Pressed) {
            Console.WriteLine("END OF PLAYLIST STOPPING!");
            getNFyBar().Value = 0;
            m = new NFyRotation();

            OpenCorrect(GetCurrentSongIfAny());
            PLAYING_ARRAY = false;
        }
        else if (getNFyStream().GetPlaybackPosition() >= SongLength && m.nextExists() && !m.Dull()) { // if it's done
            Console.WriteLine("Init next");
            m.moveIndex();
            OpenCorrect(m.getCurrentSong()); // replay (resets every variable)
        } 
        else if (getNFyStream().GetPlaybackPosition() >= SongLength && !m.nextExists() && !m.Dull() && GetNode<CheckButton>("NFYSCREEN/LoopPL").Pressed) {
            Console.WriteLine("END OF PLAYLIST RELOOP!!");
            getNFyBar().Value = 0;
            m.resetIndex();
            OpenCorrect(m.getCurrentSong());
        }
        
        
    }

    public string GetTimeSignature() {
        return GetTimeFormat(getNFyStream().GetPlaybackPosition()) + " - " + GetTimeFormat(SongLength);
    }

    public override void _Process(float delta)
    {
        if (getNFyStream().Playing) {
            sp = getNFyStream().GetPlaybackPosition();
        }

        if (!getNFyStream().Playing) {
            GetNode<Button>("NFYSCREEN/Play").Text = "Play";
        } else {
            GetNode<Button>("NFYSCREEN/Play").Text = "Stop";
            

        }
        getNFyStream().VolumeDb = ((float)GetNode<VSlider>("NFYSCREEN/Volume").Value);
        if (m.currentIndex() > m.getSize()) {
            Console.WriteLine("!!!!! ABOVE");
            PLAYING_ARRAY = false;
            m = new NFyRotation();
            OpenCorrect(GetCurrentSongIfAny());
        }
        
        if (!m.Dull()) {
            GetNode<Label>("NFYSCREEN/PLabel").Visible = true;
            GetNode<Label>("NFYSCREEN/PLabel").Text = "Currently in rotation;\n" + (m.currentIndex()+1).ToString() + " of " + m.getSize();
            GetNode<Label>("NFYSCREEN/CS").Visible = true;
            GetNode<Label>("NFYSCREEN/CS").Text = "Rotation - " + m.getCurrentSong();
        } else {
            GetNode<Label>("NFYSCREEN/PLabel").Visible = false;
            GetNode<Label>("NFYSCREEN/CS").Visible = false;
        }

        if (GetCurrentSongIfAny() != "" && m.Dull()) {
            GetNode<Label>("NFYSCREEN/CS").Visible = true;
            GetNode<Label>("NFYSCREEN/CS").Text = GetCurrentSongIfAny();
        } else {
            if (m.Dull()) {
                GetNode<Label>("NFYSCREEN/CS").Visible = false;
            }
        }
        
        LoopHandler();

        getNFyBar().MaxValue = SongLength;

        if (getNFyStream().Playing && !PLAYING_ARRAY) {
            getNFyBar().Value = getNFyStream().GetPlaybackPosition();
            ChangeActivity(GetCurrentSongIfAny(), GetTimeSignature());
        } else {
            if (!getNFyStream().Playing) {
                if (!m.Dull()) 
                    ChangeActivity("Paused In Rotation", m.currentIndex().ToString() + " out of " + m.getSize().ToString());
                else
                    ChangeActivity("Paused", "On song " + GetCurrentSongIfAny());

            } else {
                ChangeActivity("In Rotation - " + m.getCurrentSong(), GetTimeSignature());
                getNFyBar().Value = getNFyStream().GetPlaybackPosition();
            }
        }
    }
}
