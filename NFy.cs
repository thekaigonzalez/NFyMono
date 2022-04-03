using Godot;
using System;
public class NFy : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public float sp = 0;
    public float sl = 0;
    /*
    Official Naming convention for nodes
    */
    
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
        if (OS.GetCmdlineArgs()[1] == "--specials") { return true; } else { return false; }
    }
    public void PrintToConsole(string text) {
            GetNode<TextEdit>("CON").Text += text;
    }

    public string GetCurrentSongIfAny() {
        OptionButton sf = getNFySongList();
        string song = "";
        song = sf.GetItemText(sf.GetSelectedId());
        return song;
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
            sl = a.GetLength();
            audSound.Stream = a;   

        }

        else if(path.Extension() == "ogg")
        {
            AudioStreamOGGVorbis d = new AudioStreamOGGVorbis();
            d.Data = b;
            sl = d.GetLength();
            audSound.Stream = d;   
        }

        audSound.Play();
    }

    public void OpenCorrect(string name) {
        Console.WriteLine(CTEXT("songs/" + name + "."));
        OpenSong(CTEXT("songs/" + name + ".ogg")); // TODO Implement multiple feature
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
                if (!item.EndsWith(".import")) { // godot generated 
                    string item_parsed = ParseSongEntry(item);
                    Console.WriteLine(item_parsed);
                    getNFySongList().AddItem(item_parsed);
                }
            }
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SetupAPI.SetupNFy();
        SongPreload();
    }
    
    public void _on_Button_pressed() {
        if (getNFyStream().Stream == null)
        {
            OpenCorrect(GetCurrentSongIfAny());
            getNFyBar().MaxValue = sl;
        } else {
            getNFyStream().Play(sp);
        }
        
    }

    public void _onSongChange() {
        OpenCorrect(GetCurrentSongIfAny()); // you should just override this right?
    }

    public void _onStopRequested() {
        if (getNFyStream().Stream != null && getNFyStream().Playing) {
            // try to save position
            sp = getNFyStream().GetPlaybackPosition();
            getNFyStream().Stop();
           
            
        }
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (getNFyStream().Playing) {
            getNFyBar().Value = getNFyStream().GetPlaybackPosition();
        }
    }
}
