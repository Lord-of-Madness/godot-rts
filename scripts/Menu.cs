using Godot;
using Godot.Collections;
using System;

public class Menu : Control
{
    private Button buttonChapters;
    private Button buttonExit;
    private Control MainMenu;
    private Control ChapterMenu;
    private string[] levels=new string[]{"GameLevel"};

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
        MainMenu = GetNode<Control>("MainMenu");
        ChapterMenu = GetNode<Control>("ChapterMenu");
        MainMenu.GetNode("ButtonCampaign").Connect("pressed", this, "Campaign");

        buttonChapters = GetNode<Button>("MainMenu/ButtonChapters");
        buttonChapters.Connect("pressed", this, "Chapters");
        buttonExit = GetNode<Button>("MainMenu/ButtonExit");
        buttonExit.Connect("pressed", this, "ExitGame");

        

        foreach(string lvl in levels)
        {
            GetNode($"ChapterMenu/{lvl}").Connect("pressed", this, "LevelSelector",new Godot.Collections.Array{lvl});
        }


    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    /*public override void _Process(float delta)
	{

	}*/
    private void Campaign()
    {
        GetTree().ChangeScene("res://scenes/Levels/GameLevel.tscn");
    }
    private void Chapters()
    {
        GD.Print("Chapters!");
        MainMenu.Visible = false;
        ChapterMenu.Visible = true;
    }

    private void ExitGame()
    {
        GetTree().Quit();
    }
    private void LevelSelector(string lvl)
    {
        GetTree().ChangeScene($"res://scenes/Levels/{lvl}.tscn");
    }
}
