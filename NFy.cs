using Godot;
using System;
using System.Collections.Generic;
using Jint;
public class NFy : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public float sp = 0;
    public float SongLength = 0;

    public bool ed = false;

    public bool IsInNFyAES = false;
    public bool InNFySD = false;

    public bool Theme_Overriden = false;

    public bool Vsign = true;

    public int sel = 0;

    public string vsignUrl = "https://api.github.com/repos/thekaigonzalez/NFyMono/releases/latest";

    public Dictionary<string, string> env = new Dictionary<string, string>();

    public bool Following = false;
    public Vector2 DraggingStartPosition = new Vector2();

    public Jint.Engine myeng;

    public bool PLAYING_ARRAY = false;

    NFyRotation m;

    /*
	Official Naming convention for nodes
	*/

    /// Discord GDScript API
    public void ChangeActivity(string state, string desc)
    {
        Script gdclass = ResourceLoader.Load("res://Extensions/discordapi.gd") as Script;
        gdclass.Call("change_activity", state, desc, IsInNFyAES, InNFySD);

    }

    public Dictionary<string, bool> getOptsFlags()
    {
        Dictionary<string, bool> f = new Dictionary<string, bool>();

        foreach (string arg in OS.GetCmdlineArgs())
        {
            if (arg.StartsWith("--")) { f[arg.Substring(2)] = true; }
        }

        return f;
    }

    public Dictionary<string, string> getOptsEq()
    {
        Dictionary<string, string> f = new Dictionary<string, string>();

        foreach (string arg in OS.GetCmdlineArgs())
        {
            if (arg.Contains("="))
            {
                f[arg.Split("=")[0]] = arg.Split("=")[1];
            }
        }

        return f;
    }

    public string GetTimeFormat(float sec)
    {
        Script gdclass = ResourceLoader.Load("res://Extensions/discordapi.gd") as Script;
        return (string)gdclass.Call("parse_timer", sec);
    }

    /// CheckEach() returns true if the path is found with the extensions in spec,
    /// it returns the first instance, therefore all other members of "spec" are discarded.
    public bool CheckEach(string p, string[] spec)
    {
        foreach (string s in spec)
        {
            if (p.Extension() == s) return true;
            else continue;
        }
        return false;
    }

    public Panel getNFyScreen()
    {
        return GetNode<Panel>("NFYSCREEN");
    }

    public AudioStreamPlayer getNFyStream()
    {
        return GetNode<AudioStreamPlayer>("NfyStream");
    }
    public ProgressBar getNFyBar()
    {
        return getNFyScreen().GetNode<ProgressBar>("NFyBar");
    }

    public ItemList getNFySongList()
    {
        return getNFyScreen().GetNode<ItemList>("NSL");
    }

    public bool SpecialsEnabled()
    {
        if (getOptsFlags().ContainsKey("specials"))
        {
            return true; // probably not
        }
        else return false;
    }
    public void PrintToConsole(string text)
    {
        GetNode<TextEdit>("CON").Text += text + "\n";
    }

    public string GetCurrentSongIfAny()
    {
        var sf = getNFySongList();
        string song = "";
        song = sf.GetItemText(sel);
        return song;
    }

    /// @desc Check the spec
    public string wCheck(string s, string[] sp)
    {
        foreach (string str in sp)
        {
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


        if (path.Extension() == "wav")
        {
            AudioStreamSample a = new AudioStreamSample();
            a.Format = AudioStreamSample.FormatEnum.Format16Bits;
            a.Stereo = true;
            a.Data = b;
            SongLength = a.GetLength();
            audSound.Stream = a;

        }

        else if (path.Extension() == "ogg")
        {
            AudioStreamOGGVorbis d = new AudioStreamOGGVorbis();
            d.Data = b;
            SongLength = d.GetLength();
            audSound.Stream = d;
        }

        else if (path.Extension() == "mp3")
        {
            AudioStreamMP3 d = new AudioStreamMP3();
            d.Data = b;
            SongLength = d.GetLength();
            audSound.Stream = d;
        }



        audSound.Play();
    }
    public string CurrentSongPath()
    {
        return CTEXT(wCheck("songs/" + GetCurrentSongIfAny(), GetSpec()));
    }


    public void _on_ChangeTheme_pressed()
    {
        if (ed == false)
        {
            getNFyScreen().Theme = GD.Load<Theme>("res://Themes/NFyDarker/DarkerNFy.tres");
            ed = true;

        }
        else
        {
            getNFyScreen().Theme = GD.Load<Theme>("res://Themes/res://Themes/ClassicNFy/NFyClassic.tres");
            ed = false;
        }
    }
    public string[] GetSpec()
    {
        string[] spec = { "wav", "ogg", "mp3" };
        return spec;
    }
    public void OpenCorrect(string name)
    {
        if (name != "")
            OpenSong(CTEXT(wCheck("songs/" + name, GetSpec())));
    }

    /// Returns the absolute path
    public string CTEXT(string basel)
    {
        return OS.GetExecutablePath().GetBaseDir() + "/" + basel;
    }

    public string GetCurrentDir()
    {
        return OS.GetExecutablePath().GetBaseDir() + "/";
    }

    public bool DirExists(string d)
    {
        if (System.IO.Directory.Exists(CTEXT(d))) { return true; }
        else { return false; }
    }

    public string[] listDir(string dname)
    {
        return System.IO.Directory.GetFiles(dname);
    }

    public string ParseSongEntry(string B)
    {
        B = B.Replace("\\", "/"); // Convert to cross platform format
        return System.IO.Path.GetFileNameWithoutExtension(B);
    }

    public void SongPreload()
    {

        if (DirExists("songs"))
        { // Song preload
            foreach (string item in listDir("songs"))
            {
                if (!item.EndsWith(".import") && CheckEach(item, GetSpec()))
                { // possibly fix non-audio files showing up
                    string item_parsed = ParseSongEntry(item);
                    print(item_parsed);
                    getNFySongList().AddItem(item_parsed);
                }
            }
        }
    }

    public void LoadEach(string[] each)
    {
        foreach (string item in each)
        {
            getNFySongList().AddItem(item);
        }
    }
    public void PlaylistPreload()
    {

        if (DirExists("playlists"))
        {
            foreach (string item in listDir("playlists"))
            {
                if (item.EndsWith(".json"))
                {
                    string item_parsed = ParseSongEntry(item);
                    print(item_parsed);
                    GetNode<OptionButton>("NFYSCREEN/Playlists").AddItem(item_parsed);
                }
            }
        }
    }
    /* print to console and the bumblebee console */
    public void print(string text)
    {
        Console.WriteLine(text);
        PrintToConsole(text);
    }

    public string getPlaylistName()
    {
        return GetNode<OptionButton>("NFYSCREEN/Playlists").GetItemText(GetNode<OptionButton>("NFYSCREEN/Playlists").GetSelectedId());
    }
    /**
	
	GET VERSION

	This gets the latest release and compares it to the current version sign.

	For developers: Versions in this are formatted version.NUMBER, and should be used as such,
	to change the format, update the ver1 & ver2 variables found at lines 222 and 223.
	*/
    public void OnVersionRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        string s = System.Text.Encoding.UTF8.GetString(body);
        if (response_code == 200)
        {
            var js1 = JSON.Parse(s).Result;
            var js = (Godot.Collections.Dictionary)js1;

            string v = js["tag_name"] as string; // get the latest tag name from JSON

            /* if version sign is found, use version */
            if (System.IO.File.Exists(".vsign"))
            {

                print("Loading VSign");
                print("Loading Version sign - Mono 7");
                string ver = System.IO.File.ReadAllText(".vsign");

                if (ver != v)
                {
                    string ver1 = ver.Substring(ver.IndexOf(".") + 1); // (version.)NUMBER
                    string ver2 = v.Substring(v.IndexOf(".") + 1); // (version.)NUMBER
                    /* compare integer version numbers (one greater or less) */
                    if (ver1.ToInt() < ver2.ToInt())
                    {
                        System.IO.File.WriteAllText("BAD_VERSION.txt", "Wait!\nYou are not up to date!\n\nGo to https://github.com/thekaigonzalez/NFyMono.git and update!\nIf there's an available setup for the specified versions, choose it!\nIf you are seeing this message and you've modified the .vsign file, ignore it!\nHappy listening :)");
                        OS.ShellOpen("BAD_VERSION.txt");
                    }
                    else
                    {
                        System.IO.File.WriteAllText("ABOVE_VERSION.txt", "Wait! you currently are above the recommended stable version.\nPlease report any bugs to https://github.com/thekaigonzalez/NFyMono/issues!");
                        OS.ShellOpen("ABOVE_VERSION.txt");
                    }
                }
            }
        }
    }

    public string printFMT(string text, string[] form)
    {
        string new1 = "";
        bool InFM = false;
        int index = 0;
        foreach (char s in text)
        {
            if (s == '{')
            {
                InFM = true;
            }
            else if (s == '}' && InFM == true)
            {
                InFM = false;
                new1 += form[index];
            }
            else
            {
                new1 += s;
            }
        }
        return new1;
    }
    public void NJLog(string text, string[] form)
    {
        string new1 = "";
        bool InFM = false;
        int index = 0;
        foreach (char s in text)
        {
            if (s == '{')
            {
                InFM = true;
            }
            else if (s == '}' && InFM == true)
            {
                InFM = false;
                new1 += form[index];
            }
            else
            {
                new1 += s;
            }
        }
        print(new1);

    }

    string ToStringBool(bool s)
    {
        if (s == true) return "true";
        else return "false";
    }

    public void loadPlugins(bool callTick = false, bool getMethod = false, string methodName = "", params object[] cf)
    {
        /// <summary>
        /// Set the volume.
        /// </summary>
        /// <param name="s"></param>
        void setVol(float s)
        {
            getNFyStream().VolumeDb = s;
            GetNode<VSlider>("NFYSCREEN/Volume").Value = s;
        }
        /// <summary>
        /// Set VSign URL (Or disable vsign)
        /// </summary>
        /// <param name="even_Use"></param>
        /// <param name="url"></param>
        void setVSignURL(bool even_Use = true, string url = "")
        {
            if (even_Use)
            {
                vsignUrl = url;
            }
            else
            {
                Vsign = even_Use;
            }
        }
        /// <summary>
        /// Runs another file from the engine.
        /// </summary>
        /// <param name="fn"></param>
        void Include(string fn)
        {
            myeng.Execute(System.IO.File.ReadAllText(fn));
        }
        /// sets the BG Color, turns on OVERRIDEN if 
        void SetBackground(string clr)
        {
            print(clr);

            print("Setting background color to HTML color " + clr);

            var bg =  new StyleBoxFlat();
            bg.BgColor = new Color(clr);
            getNFyScreen().AddStyleboxOverride("panel", bg);

            Update();

            Theme_Overriden = true;
        }
        /// <summary>
        /// Sets the font color.
        /// </summary>
        /// <param name="clr"></param>
        void setFontColor(string clr) {
            print(clr);

            Theme_Overriden = true;

            print("Setting font color to HTML color " + clr);

            getNFyScreen().AddColorOverride("font_color", new Color(clr));
            
            GetNode<Label>("NFYSCREEN/CS").AddColorOverride("font_color", new Color(clr));
            
            Update();
        }
        /// <summary>
        /// Updates the buttons.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="outline"></param>
        /// <param name="hovercolor"></param>
        /// <param name="margins"></param>
        /// <param name="marginsOutline"></param>
        void UpdateButtons(string color, string outline, int margins, int marginsOutline) {
            print(color + ", " + outline + ".");

            print("Setting button color to " + color + ", margins to " + margins.ToString() + ", and the border width to " + marginsOutline.ToString());
            
            var bg =  new StyleBoxFlat();
            
            bg.BgColor = new Color(color);

            bg.BorderColor = new Color(outline);

            bg.SetBorderWidthAll(margins);

            bg.SetCornerRadiusAll(marginsOutline);

            /* Button changing code */
            
            GetNode<Button>("NFYSCREEN/Play").AddStyleboxOverride("normal", bg);
            GetNode<Button>("NFYSCREEN/Play").AddStyleboxOverride("pressed", bg);
            GetNode<Button>("NFYSCREEN/Play").AddStyleboxOverride("disabled", bg);
            GetNode<Button>("NFYSCREEN/Play").AddStyleboxOverride("focus", bg);
            GetNode<Button>("NFYSCREEN/Play").AddStyleboxOverride("hover", bg);


            GetNode<Button>("NFYSCREEN/Loop").AddStyleboxOverride("normal", bg);
            GetNode<Button>("NFYSCREEN/Loop").AddStyleboxOverride("pressed", bg);
            GetNode<Button>("NFYSCREEN/Loop").AddStyleboxOverride("disabled", bg);
            GetNode<Button>("NFYSCREEN/Loop").AddStyleboxOverride("focus", bg);
            GetNode<Button>("NFYSCREEN/Loop").AddStyleboxOverride("hover", bg);

            GetNode<Button>("NFYSCREEN/Playlists").AddStyleboxOverride("normal", bg);
            GetNode<Button>("NFYSCREEN/Playlists").AddStyleboxOverride("pressed", bg);
            GetNode<Button>("NFYSCREEN/Playlists").AddStyleboxOverride("disabled", bg);
            GetNode<Button>("NFYSCREEN/Playlists").AddStyleboxOverride("focus", bg);
            GetNode<Button>("NFYSCREEN/Playlists").AddStyleboxOverride("hover", bg);

            GetNode<Button>("NFYSCREEN/UP").AddStyleboxOverride("normal", bg);
            GetNode<Button>("NFYSCREEN/UP").AddStyleboxOverride("pressed", bg);
            GetNode<Button>("NFYSCREEN/UP").AddStyleboxOverride("disabled", bg);
            GetNode<Button>("NFYSCREEN/UP").AddStyleboxOverride("focus", bg);
            GetNode<Button>("NFYSCREEN/UP").AddStyleboxOverride("hover", bg);

            GetNode<Button>("NFYSCREEN/LoopPL").AddStyleboxOverride("normal", bg);
            GetNode<Button>("NFYSCREEN/LoopPL").AddStyleboxOverride("pressed", bg);
            GetNode<Button>("NFYSCREEN/LoopPL").AddStyleboxOverride("disabled", bg);
            GetNode<Button>("NFYSCREEN/LoopPL").AddStyleboxOverride("focus", bg);
            GetNode<Button>("NFYSCREEN/LoopPL").AddStyleboxOverride("hover", bg);

            /* End button changing Code */

        }
        
        myeng = new Jint.Engine()

            // Basic functions (The base API)
            .SetValue("NJPrint", (Action<string>)print)
            .SetValue("NJLog", (Action<string, string[]>)NJLog)
            .SetValue("NJPlaySongByName", (Action<string>)OpenCorrect)
            .SetValue("NJBackgroundColor", (Action<string>)SetBackground)
            .SetValue("NJFontColor", (Action<string>)setFontColor)
            .SetValue("NJButtonUpdate", (Action<string, string, int, int>)UpdateButtons)

            // LOW LEVEL FUNCTIONS - 
            // Only use these if you know what you're doing!
            // NOTE: _NJplay (OpenSong) needs the absolute path to a song! You can get this using the 
            // NJ_PATH_DIR + "songs/" + name
            .SetValue("_NJplay", (Action<string>)OpenSong)

            // Variables
            .SetValue("NJ_PATH_DIR", GetCurrentDir())

            // Class abstraction
            .SetValue("SetupNJMonoDirs", (Action)SetupAPI.SetupNFy)

            // The JavaScript-Intended helper API
            .SetValue("NJCreateDir", (Action<string>)JAPI.JCreateDir)
            .SetValue("NJWriteFile", (Action<string, string>)JAPI.JWriteFile)

            // Etc functions - Clearing Output, Pausing, and more.
            .SetValue("NJClearOutput", (Action)Console.Clear)
            .SetValue("NJPauseStream", (Action)getNFyStream().Stop)

            // These disable/enable , or edit certian features.
            .SetValue("NJVSignUrl", (Action<bool, string>)setVSignURL)

            // ENGINE CUSTOMS
            .SetValue("Include", (Action<string>)Include)


            .SetValue("NJSetVol", (Action<float>)setVol);


        if (System.IO.Directory.Exists("plugins"))
        {
            foreach (string f in listDir("plugins"))
            {
                myeng.Execute(System.IO.File.ReadAllText(f));

                var initName = "onNMonoEngineStart";
                var tickName = "onNMonoTick";

                if (!callTick && !getMethod)
                {
                    if (myeng.GetValue(initName) != Jint.Native.JsValue.Undefined)
                    {
                        myeng.Invoke(initName, getOptsEq());
                    }
                }
                else if (getMethod)
                {
                    if (myeng.GetValue(methodName) != Jint.Native.JsValue.Undefined)
                    {
                        myeng.Invoke(methodName, cf);
                    }
                }
                else
                {
                    if (myeng.GetValue(tickName) != Jint.Native.JsValue.Undefined)
                    {
                        myeng.Invoke(tickName, PLAYING_ARRAY);
                    }
                }
            }
        }
    }

    public void LoadPluginFunc(string fname, params object[] parameters)
    {
        loadPlugins(false, true, fname, parameters);
    }

    /// [------------------ THE INITIAL FUNCTION ------------------]
    public override void _Ready()
    {
        bool ign_v = false;
        var flag = getOptsFlags();
        var vars = getOptsEq();


        if (flag.ContainsKey("noVSign"))
        {
            ign_v = true;
        }

        if (flag.ContainsKey("startWithSong"))
        {
            Console.WriteLine("Playing song on init -> NMono Binary");
            if (vars.ContainsKey("songName"))
            {
                Console.WriteLine("Loading song " + vars["songName"]);
                OpenCorrect(vars["songName"]);
            }
        }

        loadPlugins();
        if (!ign_v && Vsign)
        {
            // Setup HTTP (Versioning Sign)
            GetNode("MonoHTTPV").Connect("request_completed", this, "OnVersionRequestCompleted");
            HTTPRequest httpRequest = GetNode<HTTPRequest>("MonoHTTPV");
            httpRequest.Request(vsignUrl); // request latest release
        }
        print("Checking for specials");
        if (SpecialsEnabled()) GetNode<Button>("NFYSCREEN/EnableConsole").Visible = true;
        print("Loading setup daemon");
        SetupAPI.SetupNFy();
        print("Preloading songs into list");
        SongPreload();
        print("Setting up the Discord presence from GDScript API");
        ChangeActivity("No Song Loaded", "On NFy MONO");

        PlaylistPreload();
        m = new NFyRotation();
    }

    public void _on_UP_toggled(bool t)
    {

        if (t)
        {
            if (System.IO.File.Exists("playlists/" + getPlaylistName() + ".json"))
            {
                string[] pl = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(System.IO.File.ReadAllText(CTEXT("playlists/" + getPlaylistName() + ".json")))["songs"];

                m = new NFyRotation(pl);

                PLAYING_ARRAY = true;

                OpenCorrect(m.getCurrentSong());

                getNFySongList().Clear();

                LoadEach(pl);

                getNFySongList().Select(0);
            }
            else
            {
                print("error: No play found."); // error message
            }
        }
        else
        {
            PLAYING_ARRAY = false;
            m = new NFyRotation();
            SongPreload();
            OpenCorrect(GetCurrentSongIfAny());
        }
    }

    public void _on_ItemList_item_selected(int indx)
    {

        sel = indx;

        if (!PLAYING_ARRAY)
        {
            OpenCorrect(GetCurrentSongIfAny());
        }
        else
        {
            OpenCorrect(GetCurrentSongIfAny());
            m.setIndex(sel);
            print("Switching");
        }
    }
    public void _on_PFUK_pressed()
    {
        OS.ShellOpen("https://www.stchadshandforth.org.uk/pdf/Prayers%20for%20Ukraine.pdf");
    }


    public override void _Input(InputEvent inputEvent)
    {


        if (Input.IsKeyPressed(((int)KeyList.L)))
        {
            getNFyStream().Seek(getNFyStream().GetPlaybackPosition() + 10);
        }
        else if (Input.IsKeyPressed(((int)KeyList.J)))
        {
            float newpos = getNFyStream().GetPlaybackPosition() - 10;
            if (newpos <= 0)
            {
                newpos = 0;
            }
            getNFyStream().Seek(newpos);
        }

        else if (Input.IsKeyPressed(((int)KeyList.K)))
        {
            if (getNFyStream().Playing)
            {
                getNFyStream().Stop();

            }
            else
            {
                getNFyStream().Play();
            }
        }
    }

    public void _on_EnableConsole_pressed()
    {
        if (GetNode<TextEdit>("CON").Visible)
            GetNode<TextEdit>("CON").Visible = false;
        else
            GetNode<TextEdit>("CON").Visible = true;

    }

    public void _on_Button_pressed()
    {


        if (getNFyStream().Stream == null)
        {
            if (!getNFyStream().Playing)
            {
                OpenCorrect(GetCurrentSongIfAny());
                getNFyBar().MaxValue = SongLength;
            }
            else
            {
                getNFyStream().Stop();
            }


        }
        else
        {
            if (!getNFyStream().Playing)
                getNFyStream().Play(sp);
            else
            {
                getNFyStream().Stop();
            }
        }

    }

    public void _song_change(int index)
    {
        OpenCorrect(GetCurrentSongIfAny()); // you should just override this right?
    }

    public void _onStopRequested()
    {
        if (getNFyStream().Stream != null && getNFyStream().Playing)
        {
            // try to save position
            sp = getNFyStream().GetPlaybackPosition();
            getNFyStream().Stop();
        }
    }
    // Contains code for a custom loop feature
    public void LoopHandler()
    {

        if (getNFyStream().GetPlaybackPosition() >= SongLength && GetNode<CheckButton>("NFYSCREEN/Loop").Pressed && m.Dull())
        {
            print("play");
            OpenCorrect(GetCurrentSongIfAny()); // replay (resets every variable)
        }
        else if (getNFyStream().GetPlaybackPosition() >= SongLength && !m.nextExists() && !m.Dull() && !GetNode<CheckButton>("NFYSCREEN/LoopPL").Pressed)
        {
            print("END OF PLAYLIST STOPPING!");
            getNFyBar().Value = 0;
            m = new NFyRotation();

            OpenCorrect(GetCurrentSongIfAny());

            loadPlugins(false, true, "onNMonoPlaylistEnded", m.currentIndex());

            PLAYING_ARRAY = false;
        }
        else if (getNFyStream().GetPlaybackPosition() >= SongLength && m.nextExists() && !m.Dull())
        { // if it's done
            print("Init next");
            m.moveIndex();
            OpenCorrect(m.getCurrentSong()); // replay (resets every variable)
            loadPlugins(false, true, "onNMonoPSongChanged", m.currentIndex(), m.getCurrentSong());

        }
        else if (getNFyStream().GetPlaybackPosition() >= SongLength && !m.nextExists() && !m.Dull() && GetNode<CheckButton>("NFYSCREEN/LoopPL").Pressed)
        {
            print("END OF PLAYLIST RELOOP!!");
            getNFyBar().Value = 0;
            m.resetIndex();
            OpenCorrect(m.getCurrentSong());
            loadPlugins(false, true, "onNMonoPSongChanged", m.currentIndex(), m.getCurrentSong());

        }


    }

    public string GetTimeSignature()
    {
        return GetTimeFormat(getNFyStream().GetPlaybackPosition()) + " - " + GetTimeFormat(SongLength);
    }


    public override void _Process(float delta)
    {

        loadPlugins(true); // call the tick frame functions for any plugins
        // Console.WriteLine(GetCurrentSongIfAny());

        if (GetCurrentSongIfAny().Trim() == "MACINTOSH PLUS 420")
        {
            getNFyScreen().Theme = GD.Load<Theme>("res://Win95.tres");
            getNFyScreen().AddStyleboxOverride("panel", GD.Load<StyleBoxFlat>("res://Win95s.tres"));
            IsInNFyAES = true;
        }
        else
        {
            if (!Theme_Overriden) {
                getNFyScreen().Theme = GD.Load<Theme>("res://Themes/NFyCorded/NFyCord.tres");

                getNFyScreen().AddStyleboxOverride("panel", GD.Load<StyleBoxFlat>("res://Themes/NFyCorded/PanelTheme.tres"));
            }



            IsInNFyAES = false;

        }

        if (GetCurrentSongIfAny().Trim() == "Some Nights")
        {
            InNFySD = true;

            GetNode<Label>("NFYSCREEN/CS").Text = "Some Nights - With NFy Mono :)";
        }
        else
        {
            InNFySD = false;
        }

        // if (getNFyStream().Playing)
        // {
        //     sp = getNFyStream().GetPlaybackPosition();
        // }
        sp = getNFyStream().GetPlaybackPosition();

        if (!getNFyStream().Playing)
        {
            GetNode<Button>("NFYSCREEN/Play").Text = "Play";
        }
        else
        {
            GetNode<Button>("NFYSCREEN/Play").Text = "Stop";
        }

        getNFyStream().VolumeDb = ((float)GetNode<VSlider>("NFYSCREEN/Volume").Value);

        if (m.currentIndex() > m.getSize())
        {
            print("!!!!! ABOVE");
            PLAYING_ARRAY = false;
            m = new NFyRotation();
            OpenCorrect(GetCurrentSongIfAny());
        }

        if (!m.Dull())
        {
            GetNode<Label>("NFYSCREEN/PLabel").Visible = true;
            GetNode<Label>("NFYSCREEN/PLabel").Text = "Currently in rotation;\n" + (m.currentIndex() + 1).ToString() + " of " + m.getSize();
            GetNode<Label>("NFYSCREEN/CS").Visible = true;
            GetNode<Label>("NFYSCREEN/CS").Text = "Rotation - " + m.getCurrentSong();
        }
        else
        {
            GetNode<Label>("NFYSCREEN/PLabel").Visible = false;
            GetNode<Label>("NFYSCREEN/CS").Visible = false;
        }

        if (GetCurrentSongIfAny() != "" && m.Dull())
        {
            GetNode<Label>("NFYSCREEN/CS").Visible = true;
            GetNode<Label>("NFYSCREEN/CS").Text = GetCurrentSongIfAny();
        }
        else
        {
            if (m.Dull())
            {
                GetNode<Label>("NFYSCREEN/CS").Visible = false;
            }
        }

        LoopHandler();

        getNFyBar().MaxValue = SongLength;

        getNFyBar().Value = getNFyStream().GetPlaybackPosition();

        if (getNFyStream().Playing && !PLAYING_ARRAY)
        {

            ChangeActivity("", "Listening to " + GetCurrentSongIfAny());
        }
        else
        {
            if (!getNFyStream().Playing)
            {
                if (!m.Dull())
                    ChangeActivity("Paused In Rotation", m.currentIndex().ToString() + " out of " + m.getSize().ToString());
                else
                    ChangeActivity("Paused", "On song " + GetCurrentSongIfAny());

            }
            else
            {
                ChangeActivity("", "Listening to " + m.getCurrentSong() + " (Rotation)");
                getNFyBar().Value = getNFyStream().GetPlaybackPosition();
            }
        }
    }
}
