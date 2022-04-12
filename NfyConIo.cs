using Godot;
using System;

public class NfyConIo : TextEdit
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddKeywordColor("Preloading", Colors.Blue);
        AddKeywordColor("Error:", Colors.Red);
        AddKeywordColor("list", Colors.GreenYellow);
        AddKeywordColor("Loading", Colors.AntiqueWhite);


    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
