using System.Threading.Tasks;
using Godot;
using System;

public class NJLoad : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    bool started = false;
    bool con = false;

    bool is_ready = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<AnimationPlayer>("Contrib/SideSlide").Play("Slide");

        GetNode<AnimationPlayer>("Load/MainLabel/PopUpAnim").Play("LoadAnim");
        started = true;

    }

    public void print(string text)
    {
        GetNode<TextEdit>("Load/Console").Text += text + "\n";
    }

    public async void ConsProc()
    {
        print("Console process loaded.");
        await Task.Delay(1000);
        print("Checking for playlists...");
        await Task.Delay(2000);
        if (System.IO.Directory.Exists("playlists"))
        {
            print("`playlists' found!");
        }
        else
        {
            print("error: playlists not found, creating...");
            await Task.Delay(1000);

            System.IO.Directory.CreateDirectory("playlists");
        }
        print("Loading NFy...");

        await Task.Delay(new Random().Next(1, 1000));
        GetNode<AnimationPlayer>("Fade/AnimationPlayer").Play("FadeIn");
        await Task.Delay(2000);


        GetTree().ChangeScene("res://NFy.tscn");

    }

    public override void _Process(float delta)
    {
        if (Input.IsKeyPressed((int)KeyList.Control) && Input.IsKeyPressed((int)KeyList.S))
        {
            GetNode<AnimationPlayer>("Fade/AnimationPlayer").Play("FadeIn");

            GetTree().ChangeScene("res://NFy.tscn");
        }
        if (started && !GetNode<AnimationPlayer>("Load/MainLabel/PopUpAnim").IsPlaying())
        {
            GetNode<AnimationPlayer>("Load/Console/ConsoleUpAnim").Play("ConsoleUpAnim");
            con = true;
            started = false;
        }

        if (con && !GetNode<AnimationPlayer>("Load/Console/ConsoleUpAnim").IsPlaying())
        {
            is_ready = true;
            print("Console is ready!");
            con = false;
        }

        if (is_ready)
        {
            print("is_ready=true");
            //yield(get_tree().create_timer(1.0), "timeout")
            print("Checking for songs/");
            if (System.IO.Directory.Exists("songs"))
            {
                print("songs/ found.");
            }
            print("Starting console process..");
            Task.Run(ConsProc);

            is_ready = false;
        }


    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
