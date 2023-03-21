using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public class Menu : Control
{
    private Button buttonChapters;
    private Button buttonExit;
    private Control MainMenu;
    private Control ChapterMenu;
    private List<string> levels = new List<string>();

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

        var directory = new Directory();
        directory.Open("res://scenes/Levels");
        directory.ListDirBegin(skipNavigational: true);
        string file = directory.GetNext();
        while (file != "")
        {
            if (!directory.CurrentIsDir())
                levels.Add(file);
            file = directory.GetNext();
        }

        ChapterMenu.GetNode<Button>("BACK").Connect("pressed", this, "Back");
        foreach (string lvl in levels)
        {
            var button = new Button();
            string name = lvl.Split('.')[0];
            button.Name = name;
            button.Text = name;
            button.RectMinSize=new Vector2(0,40);
            ChapterMenu.GetNode<VBoxContainer>("ScrollContainer/Chapters").AddChild(button);
            button.Connect("pressed", this, "LevelSelector", new Godot.Collections.Array { lvl });
        }


    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    /*public override void _Process(float delta)
	{

	}*/
    private void Campaign()
    {
        GetTree().ChangeScene("res://scenes/Levels/Level1.tscn");
    }
    private void Chapters()
    {
        MainMenu.Visible = false;
        ChapterMenu.Visible = true;
    }

    private void ExitGame()
    {
        GetTree().Quit();
    }
    private void LevelSelector(string lvl)
    {
        GetTree().ChangeScene($"res://scenes/Levels/{lvl}");
    }
    private void Back()
    {
        MainMenu.Visible = true;
        ChapterMenu.Visible = false;
    }
}
