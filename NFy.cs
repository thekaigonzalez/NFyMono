/*! \mainpage NFy Mono
 *
 * \section intro_sec NFy Mono Source code
 *
 * This is the NFy Mono source code browser. This contains the functions for the Nfy base class, etc ..
 */
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

    public bool ANIM_ONCE = false;

    public bool ed = false;

    public bool IsInNFyAES = false;

    public string SONG_DIR = "songs/";
    public bool InNFySD = false;

    public bool Theme_Overriden = false;

    public string integral_latest = "";

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

    /// <summary>
    /// It returns a dictionary of all the command line arguments that start with "--"
    /// </summary>
    /// <returns>
    /// A dictionary of strings and booleans.
    /// </returns>
    public Dictionary<string, bool> getOptsFlags()
    {
        Dictionary<string, bool> f = new Dictionary<string, bool>();

        foreach (string arg in OS.GetCmdlineArgs())
        {
            if (arg.StartsWith("--")) { f[arg.Substring(2)] = true; }
        }

        return f;
    }

    /// <summary>
    /// It takes the command line arguments and returns a dictionary of the arguments that contain an
    /// equal sign (K=V)
    /// </summary>
    /// <returns>
    /// A dictionary of strings.
    /// </returns>
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

    /// <summary>
    /// This function returns the SongList node that is a child of the NFyScreen node.
    /// </summary>
    /// <returns>
    /// The SongList node that is a child of the NFyScreen node.
    /// </returns>
    public ItemList getNFySongList()
    {
        return getNFyScreen().GetNode<ItemList>("NSL");
    }

    /// <summary>
    /// > If the opts dictionary contains the key "specials", return true. Otherwise, return false
    /// </summary>
    /// <returns>
    /// A boolean value.
    /// </returns>
    public bool SpecialsEnabled()
    {
        if (getOptsFlags().ContainsKey("specials"))
        {
            return true; // probably not
        }
        else return false;
    }

    /// <summary>
    /// > If the opts dictionary contains the key "Unofficial", return true. Otherwise, return false
    /// </summary>
    /// <returns>
    /// A boolean value.
    /// </returns>
    public bool AutoSet()
    {
        if (getOptsFlags().ContainsKey("Unofficial"))
        {
            return true; // probably not
        }
        else return false;
    }
    /// <summary>
    /// It takes a string as an argument, and adds it to the end of the text in the TextEdit node called
    /// "CON"
    /// </summary>
    /// <param name="text">The text to print to the console.</param>
    public void PrintToConsole(string text)
    {
        GetNode<TextEdit>("CON").Text += text + "\n";
    }

    /// <summary>
    /// It gets the current song from the listbox and returns it as a string
    /// </summary>
    /// <returns>
    /// The name of the song that is currently selected.
    /// </returns>
    public string GetCurrentSongIfAny()
    {
        var sf = getNFySongList();
        string song = "";
        song = sf.GetItemText(sel);
        return song;
    }

    /// <summary>
    /// It checks if a file or directory exists, and if it does, it returns the full path to the file or
    /// directory (with the extension)
    /// </summary>
    /// <param name="s">The string to check</param>
    /// <param name="sp">The array of strings to check for (as extensions).</param>
    /// <returns>
    /// The first file or directory that matches the extension in the array.
    /// </returns>
    public string wCheck(string s, string[] sp)
    {
        foreach (string str in sp)
        {
            if (DirExists(s + "." + str)) return s + "." + str;
            if (System.IO.File.Exists(s + "." + str)) return s + "." + str;
        }
        return "";
    }



    /// <summary>
    /// It opens a file, reads it into a byte array, and then plays it
    /// </summary>
    /// <param name="path">The path to the song you want to play.</param>
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
    /// <summary>
    /// It returns the path of the current song, if any.
    /// </summary>
    /// <returns>
    /// The path to the current song.
    /// </returns>
    [Obsolete("This song is no longer in use")]
    public string CurrentSongPath()
    {
        return CTEXT(wCheck(SONG_DIR + GetCurrentSongIfAny(), GetSpec()));
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

    /// <summary>
    /// This function returns an array of strings that contains the file extensions that the executable can
    /// handle.
    /// </summary>
    /// <returns>
    /// The string array spec
    /// </returns>
    public string[] GetSpec()
    {
        string[] spec = { "wav", "ogg", "mp3" };
        return spec;
    }

    /// <summary>
    /// It opens a song (correctly, with proper checks.)
    /// </summary>
    /// <param name="name">The name of the song to open.</param>
    public void OpenCorrect(string name)
    {
        if (System.IO.File.Exists(CTEXT("images/" + GetCurrentSongIfAny() + ".jpg")))
        {
            var img = new Image();

            img.LoadJpgFromBuffer(System.IO.File.ReadAllBytes(CTEXT("images/" + GetCurrentSongIfAny() + ".jpg")));

            var imgt = new ImageTexture();
            imgt.CreateFromImage(img);

            GetNode<TextureRect>("NFYSCREEN/Ga").Texture = imgt;
        }
        else
        {
            if (name == "Potion Seller")
            {
                /*
                Hello potion seller, I’m going into battle and I want your strongest potions
My potions are too strong for you, traveller
Potion seller, I tell you I’m going into battle and I want only your strongest potions
You can’t handle my potions, they are too strong for you
Potion seller, listen to me, I want only your strongest potions
My potions would kill you traveller, you cannot handle my potions
Potion seller, enough of these games, I’m going to battle and I need your strongest potions
My strongest potions would kill you traveller, you can’t handle my strongest potions.You better go to a seller that sells weaker potions
Potion seller, I’m telling you right now, I’m going into battle and I need only your strongest potions
You don’t know what you asked traveller, my strongest potions would kill a dragon, let alone a man. You need a seller that sells weaker potions, because my potions are too strong
Potion seller, I’m telling you I need your strongest potions, I’m going into a battle, I’m going to battle, and I need your strongest potions
You can’t handle my strongest potions. No one can, my strongest potions are fit for a beast, let alone a man
Potion seller what do I have to tell you to get your potions? Why won’t you trust me with your strongest potions, potion seller? I need them if I’m to be successful in the battle
I can’t give you my strongest positions, because my strongest potions are only for the strongest beings and you are of the weakest
Well then, that’s it potion seller, I’ll go elsewhere, I’ll go elsewhere for my potions
That’s what you better do
I’ll go elsewhere for my potions and I’ll never come back
Good, you are not welcome here. My potions are only for the strongest and you are clearly not of the strongest, you are clearly of the weakest
You have had your say, potion seller, but I’ll have mine. You’re a rascal; you’re a rascal with no respect for the knights, no respect for anything except your potions
Why respect the knights? When my potions can do anything that you can?
*/
                var imga = new Image();

                imga.Load("res://Potion Seller.jpg");

                var imgtt = new ImageTexture();
                imgtt.CreateFromImage(imga);

                GetNode<TextureRect>("NFYSCREEN/Ga").Texture = imgtt;

            }
            else
            {
                var img = new Image();

                img.LoadJpgFromBuffer(System.IO.File.ReadAllBytes(CTEXT("images/default.jpg")));

                var imgt = new ImageTexture();
                imgt.CreateFromImage(img);

                GetNode<TextureRect>("NFYSCREEN/Ga").Texture = imgt;
            }
        }
        if (name != "")
            OpenSong(CTEXT(wCheck(SONG_DIR + name, GetSpec())));
    }

    /// Returns the absolute path
    /// <summary>
    /// It returns the path to the executable file, then gets the base directory of that path, then adds
    /// the base directory to the path of the file you want to open
    /// </summary>
    /// <param name="basel">The base path of the file.</param>
    /// <returns>
    /// The path to the executable file.
    /// </returns>
    public string CTEXT(string basel)
    {
        return OS.GetExecutablePath().GetBaseDir() + "/" + basel;
    }

    /// <summary>
    /// It returns the current directory of the executable
    /// </summary>
    /// <returns>
    /// The current directory of the executable.
    /// </returns>
    public string GetCurrentDir()
    {
        return OS.GetExecutablePath().GetBaseDir() + "/";
    }

    /// <summary>
    /// > Returns true if the directory exists, false if it doesn't
    /// </summary>
    /// <param name="d">The directory to check.</param>
    /// <returns>
    /// A boolean value.
    /// </returns>
    public bool DirExists(string d)
    {
        if (System.IO.Directory.Exists(CTEXT(d))) { return true; }
        else { return false; }
    }

    /// <summary>
    /// ListDir returns a list of files in a directory.
    /// </summary>
    /// <param name="dname">The directory to list.</param>
    /// <returns>
    /// A list of files in the directory
    /// </returns>
    public string[] listDir(string dname)
    {
        return System.IO.Directory.GetFiles(dname);
    }

    /// <summary>
    /// It takes a string, replaces all backslashes with forward slashes, and returns the filename
    /// without the extension
    /// </summary>
    /// <param name="B">The song entry to parse</param>
    /// <returns>
    /// The name of the song without the extension.
    /// </returns>
    public string ParseSongEntry(string B)
    {
        B = B.Replace("\\", "/"); // Convert to cross platform format
        return System.IO.Path.GetFileNameWithoutExtension(B);
    }

    public void TranslationPreload()
    {
        if (DirExists("lang"))
        {
            foreach (string item in listDir("lang"))
            {
                if (item.EndsWith(".json"))
                {
                    var js1 = JSON.Parse(System.IO.File.ReadAllText(item)).Result;
                    var js = (Godot.Collections.Dictionary)js1;


                    GetNode<OptionButton>("NFYSCREEN/Translations").AddItem(js["nj.name"] as string);

                    // var tarns = js["translation"] as Godot.Collections.Dictionary;
                    // GetNode<Button>("NFYSCREEN/LoopPL").Text = tarns["nj.loopPL"] as string;
                    // GetNode<Button>("NFYSCREEN/Loop").Text = tarns["nj.loop"] as string;
                    // GetNode<Button>("NFYSCREEN/UP").Text = tarns["nj.up"] as string;
                    // GetNode<Button>("NFYSCREEN/OpenSongs").Text = tarns["nj.songDir"] as string;

                }
            }
        }

    }

    /// <summary>
    /// It checks if the directory "songs" exists, and if it does, it checks each file in the directory
    /// to see if it's a valid audio file, and if it is, it adds it to the list of songs
    /// </summary>
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

    /// <summary>
    /// It takes a string array, and adds each item in the array to the list
    /// </summary>
    /// <param name="each">The array of strings that you want to add to the list.</param>
    public void LoadEach(string[] each)
    {
        foreach (string item in each)
        {
            getNFySongList().AddItem(item);
        }
    }

    /// <summary>
    /// It takes a string array, loops through each item in the array, and adds the item to the list
    /// </summary>
    /// <param name="each">The array of strings to be parsed.</param>
    public void LoadAndStripEach(string[] each)
    {
        foreach (string item in each)
        {
            if (!item.EndsWith(".import") && CheckEach(item, GetSpec()))
            {
                getNFySongList().AddItem(ParseSongEntry(item));
            }
        }
    }

    /// <summary>
    /// It checks if the "playlists" directory exists, if it does, it loops through all the files in the
    /// directory, and if the file ends with ".json", it adds the file name to the "Playlists" option
    /// button
    /// </summary>
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
    /// <summary>
    /// The function `print` takes a string as an argument and prints it to the console(s)
    /// </summary>
    /// <param name="text">The text to print to the console.</param>
    public void print(string text)
    {
        Console.WriteLine(text);
        PrintToConsole(text);
    }

    public string getPlaylistName()
    {
        return GetNode<OptionButton>("NFYSCREEN/Playlists").GetItemText(
            GetNode<OptionButton>("NFYSCREEN/Playlists").GetSelectedId()
        );
    }
    /**
	
	GET VERSION

	This gets the latest release and compares it to the current version sign.

    For developers: 
    
        Versions in this are formatted version.NUMBER, and should be used as such,
        With recent updates, versions can now be `UNOFFICIAL` (all lowercase though),
        Which VSign ignores, as seen at line 341, if you need version checking, but 
        use a different format, update the ver1 & ver2 variables found at lines 222 and 223.
	*/
    public void OnVersionRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        string s = System.Text.Encoding.UTF8.GetString(body);
        if (response_code == 200)
        {
            var js1 = JSON.Parse(s).Result;
            var js = (Godot.Collections.Dictionary)js1;

            string v = js["tag_name"] as string; // get the latest tag name from JSON
            integral_latest = v;
            print(integral_latest + " is lat");
            /* if version sign is found, use version */
            if (System.IO.File.Exists(".vsign"))
            {

                print("Loading VSign");
                print("Loading Version sign - Mono 7");
                string ver = System.IO.File.ReadAllText(".vsign");

                if (ver != v && ver != "unofficial")
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

    [Obsolete("this function has been replaced by NJLog and will be deprecated in NJ 12.")]
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
    /// <summary>
    /// It takes a string and an array of strings, and replaces all instances of {} with the first
    /// element of the array, {} #2 with the second element, and so on
    /// </summary>
    /// <param name="text">The text you want to print.</param>
    /// <param name="form">The array of strings that will be used to replace the placeholders in the
    /// text.</param>
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

    [Obsolete("This function is no longer in use and will be deprecated in NJ 12.")]
    /// <summary>
    /// It returns the string "true" if the input is true, and the string "false" if the input is false
    /// </summary>
    /// <param name="s">The string to convert to a bool.</param>
    /// <returns>
    /// A string
    /// </returns>
    string ToStringBool(bool s)
    {
        if (s == true) return "true";
        else return "false";
    }

    public void loadPlugins(bool callTick = false, bool getMethod = false, string methodName = "", params object[] cf)
    {

        /// <summary>
        /// It sets the volume of the stream to the value of the slider
        /// </summary>
        /// <param name="s">The volume level in decibels.</param>
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


        /// <summary>
        /// It sets the background color of the screen to the color specified in the string.
        /// </summary>
        /// <param name="clr">The HTML color code to set the background to.</param>
        void SetBackground(string clr)
        {
            print(clr);

            print("Setting background color to HTML color " + clr);

            var bg = new StyleBoxFlat();
            bg.BgColor = new Color(clr);
            getNFyScreen().AddStyleboxOverride("panel", bg);

            Update();

            Theme_Overriden = true;
        }
        /// <summary>
        /// Sets the font color.
        /// </summary>
        /// <param name="clr"></param>
        void setFontColor(string clr)
        {
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
        void UpdateButtons(string color, string outline, int margins, int marginsOutline)
        {
            print(color + ", " + outline + ".");

            print("Setting button color to " + color + ", margins to " + margins.ToString() + ", and the border width to " + marginsOutline.ToString());

            var bg = new StyleBoxFlat();

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

        string FileExists(string p)
        {
            if (System.IO.File.Exists(p) == true)
            {
                return "yes";
            }
            else
            {
                return "no";
            }
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
            // NJ_PATH_DIR + SONG_DIR + name
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
            .SetValue("NJReadFile", new Func<Jint.Native.JsValue, string>((path) => System.IO.File.ReadAllText(path.AsString())))
            .SetValue("NJExists", new Func<Jint.Native.JsValue, string>((p) => FileExists(p.AsString())))


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

    /// <summary>
    /// 
    /// The first parameter is the name of the function you want to call. The second parameter is an
    /// array of parameters you want to pass to the function
    /// </summary>
    /// <param name="fname">The name of the function to call.</param>
    public void LoadPluginFunc(string fname, params object[] parameters)
    {
        loadPlugins(false, true, fname, parameters);
    }

    /// <summary>
    /// It reads the contents of the .vsign file, and if it's "unofficial", it returns "unofficial", otherwise
    /// it returns the last part of the version number
    /// </summary>
    /// <returns>
    /// The version number of the file.
    /// </returns>
    public string getVersionNumber()
    {

        string fs = System.IO.File.ReadAllText(".vsign")
            .Trim();
        if (fs == "unofficial") return "unofficial";
        return fs.Substring(fs.LastIndexOf(".") + 1);
    }

    /// [------------------ THE INITIAL FUNCTION ------------------]
    public override void _Ready()
    {
        bool ign_v = false;
        var flag = getOptsFlags();
        var vars = getOptsEq();

        GetNode<Panel>("Opc/").Visible = true;

        GetNode<AnimationPlayer>("Opc/AnimationPlayer").Play("Fade");

        // (GetNode<Sprite>("NFYSCREEN/BLUR").Material as ShaderMaterial).SetShaderParam("blurSize", 0);

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

        print("Checking for specials");
        if (SpecialsEnabled()) GetNode<Button>("NFYSCREEN/EnableConsole").Visible = true;

        if (SpecialsEnabled())
        {
            print("Version string is true");
            GetNode<Label>("NFYSCREEN/VER").Text = "NFy Mono version " + getVersionNumber() + " DEBUG mode";
        }

        if (AutoSet())
        {
            print("Setting unofficial");
            System.IO.File.WriteAllText(".vsign", "unofficial");
        }

        if (!ign_v && Vsign)
        {
            // Setup HTTP (Versioning Sign)
            GetNode("MonoHTTPV").Connect("request_completed", this, "OnVersionRequestCompleted");
            HTTPRequest httpRequest = GetNode<HTTPRequest>("MonoHTTPV");
            httpRequest.Request(vsignUrl); // request latest release
        }
        print("Loading setup daemon");
        SetupAPI.SetupNFy();
        print("Preloading songs into list");
        SongPreload();
        TranslationPreload();
        print("Setting up the Discord presence from GDScript API");
        ChangeActivity("No Song Loaded", "On NFy MONO");

        PlaylistPreload();
        m = new NFyRotation();
    }

    public string[] Randomize(string[] input)
    {
        List<string> inputList = new List<string>(input);
        string[] output = new string[input.Length];
        Random randomizer = new Random();
        int i = 0;

        while (inputList.Count > 0)
        {
            int index = randomizer.Next(inputList.Count);
            output[i++] = inputList[index];
            inputList.RemoveAt(index);
        }

        return (output);
    }

    public void _on_Translations_item_selected(int index)
    {

        foreach (string item in listDir(CTEXT("lang")))
        {
            var js1 = JSON.Parse(System.IO.File.ReadAllText(item)).Result;
            var js = (Godot.Collections.Dictionary)js1;

            if (js["nj.name"] as string == GetNode<OptionButton>("NFYSCREEN/Translations").GetItemText(index)) {

                var tarns = js["translation"] as Godot.Collections.Dictionary;
                GetNode<Button>("NFYSCREEN/LoopPL").Text = tarns["nj.loopPL"] as string;
                GetNode<Button>("NFYSCREEN/Loop").Text = tarns["nj.loop"] as string;
                GetNode<Button>("NFYSCREEN/UP").Text = tarns["nj.up"] as string;
                GetNode<Button>("NFYSCREEN/OpenSongs").Text = tarns["nj.songDir"] as string;
            }
        }

    }


    public void _on_UP_toggled(bool t)
    {

        if (t)
        {
            if (System.IO.File.Exists("playlists/" + getPlaylistName() + ".json"))
            {
                var pl2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(System.IO.File.ReadAllText(CTEXT("playlists/" + getPlaylistName() + ".json")));


                string[] pl = { };
                bool FailSafe = false;

                if (pl2.ContainsKey("songs"))
                {
                    pl = pl2["songs"];
                    FailSafe = true;
                }
                else if (pl2.ContainsKey("playlist_songs"))
                {
                    pl = pl2["playlist_songs"];
                    FailSafe = true;
                }

                bool shuf = false;
                /* TODO: Add more settings */
                if (pl2.ContainsKey("settings"))
                {
                    foreach (string set in pl2["settings"])
                    {
                        if (set == "playlists:shuffle")
                        {
                            shuf = true;
                        }
                    }
                }

                if (FailSafe)
                {
                    // If shuffling setting enabled
                    if (shuf)
                    {
                        // randomize the string array
                        pl = Randomize(pl);
                    }
                    m = new NFyRotation(pl);

                    PLAYING_ARRAY = true;

                    OpenCorrect(m.getCurrentSong());

                    getNFySongList().Clear();

                    LoadEach(pl);

                    getNFySongList().Select(0);
                }
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

    public void _on_open_songs()
    {
        if (System.IO.Directory.Exists(CTEXT(SONG_DIR)))
            OS.ShellOpen(CTEXT(SONG_DIR));
        else
            print("Error: Directory not found");
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
    public void AnimHandler()
    {
        if (ANIM_ONCE == false)
        {
            if (GetNode<AnimationPlayer>("Opc/AnimationPlayer").IsPlaying() == false && ANIM_ONCE == false)
            {
                GetNode<Panel>("Opc").QueueFree();
                ANIM_ONCE = true;
            }
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

    [Obsolete("GetTimeSignature will be deprecated (and possibly commented out) in NJ 12, please refrain from using this function.")]
    public string GetTimeSignature()
    {
        return GetTimeFormat(getNFyStream().GetPlaybackPosition()) + " - " + GetTimeFormat(SongLength);
    }


    public override void _Process(float delta)
    {
        /*
        var path = get_cwd_file("playlists/" + PFile + ".png")
			var ogg_file = File.new()
			ogg_file.open(path, File.READ)
			var bytes = ogg_file.get_buffer(ogg_file.get_len())
			var stream = Image.new()
			stream.load_png_from_buffer(bytes)
			var itex = ImageTexture.new()
			itex.create_from_image(stream)
        */
        AnimHandler();

        //GetTimeFormat(getNFyStream().GetPlaybackPosition()) + " - " + GetTimeFormat(SongLength);
        GetNode<Label>("NFYSCREEN/Time1").Text = GetTimeFormat(getNFyStream().GetPlaybackPosition());
        GetNode<Label>("NFYSCREEN/Time2").Text = GetTimeFormat(SongLength);

        if (Input.IsKeyPressed((int)KeyList.Control) && Input.IsKeyPressed((int)KeyList.R))
        {
            var pl = listDir(CTEXT(SONG_DIR));

            m = new NFyRotation(pl);

            getNFySongList().Clear();

            LoadAndStripEach(pl);

            getNFySongList().Select(0);

            OpenCorrect(GetCurrentSongIfAny()); // Fixes CTRL+R
        }
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
            if (!Theme_Overriden)
            {
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
            GetNode<Button>("NFYSCREEN/Play").Text = ">";
        }
        else
        {
            GetNode<Button>("NFYSCREEN/Play").Text = "||";
        }

        getNFyStream().VolumeDb = ((float)GetNode<VSlider>("NFYSCREEN/Volume").Value);

        if (m.currentIndex() > m.getSize() && !m.Dull())
        {
            print("!!!!! ABOVE");
            PLAYING_ARRAY = false;
            m = new NFyRotation();
            OpenCorrect(GetCurrentSongIfAny());
        }

        if (!m.Dull() && PLAYING_ARRAY)
        {
            GetNode<Label>("NFYSCREEN/PLabel").Visible = true;
            GetNode<Label>("NFYSCREEN/PLabel").Text = "Currently in rotation;\n" + (m.currentIndex() + 1).ToString() + " of " + m.getSize();

            GetNode<Label>("NFYSCREEN/CS").Visible = true;
            GetNode<Label>("NFYSCREEN/CS").Text = GetCurrentSongIfAny();
        }
        else
        {
            GetNode<Label>("NFYSCREEN/PLabel").Visible = false;
            // GetNode<Label>("NFYSCREEN/CS").Visible = false;
        }

        if (GetCurrentSongIfAny() != "" && m.Dull())
        {
            GetNode<Label>("NFYSCREEN/CS").Visible = true;
            GetNode<Label>("NFYSCREEN/CS").Text = GetCurrentSongIfAny();
        }
        else
        {

            if (m.Dull()) GetNode<Label>("NFYSCREEN/CS").Text = "No Song Selected...";
            GetNode<Label>("NFYSCREEN/CS").Text = GetCurrentSongIfAny();

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
