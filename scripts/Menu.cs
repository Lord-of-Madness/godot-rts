using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Menu : Control
{
	private Button buttonChapters;
	private Button buttonExit;
	private Control MainMenu;
	private Control ChapterMenu;
	private readonly List<string> Levels = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		MainMenu = GetNode<Control>("MainMenu");
		ChapterMenu = GetNode<Control>("ChapterMenu");
		MainMenu.GetNode("ButtonCampaign").Connect("pressed", new Callable(this, nameof(Campaign)));

		buttonChapters = GetNode<Button>("MainMenu/ButtonChapters");
		buttonChapters.Connect("pressed", new Callable(this, nameof(Chapters)));
		buttonExit = GetNode<Button>("MainMenu/ButtonExit");
		buttonExit.Connect("pressed", new Callable(this, nameof(ExitGame)));

		DirAccess directory = DirAccess.Open("res://scenes/Levels");
		//TODO could add error checking
        foreach (var file in directory.GetFiles())
        {
            Levels.Add(file);
        }
        directory.ListDirEnd();

        ChapterMenu.GetNode<Button>("BACK").Connect("pressed", new Callable(this, nameof(Back)));
		foreach (string lvl in Levels)
		{
			var button = new Button();
			string name = lvl.Split('.')[0];
			button.Name = name;
			button.Text = name;
			button.CustomMinimumSize=new Vector2(0,40);
			ChapterMenu.GetNode<VBoxContainer>("ScrollContainer/Chapters").AddChild(button);
            button.Pressed += ()=>LevelSelector(lvl);
		}


	}
    private void Campaign()
	{
		GetTree().ChangeSceneToFile("res://scenes/Levels/Level1.tscn");
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
		GetTree().ChangeSceneToFile($"res://scenes/Levels/{lvl}");
	}
	private void Back()
	{
		MainMenu.Visible = true;
		ChapterMenu.Visible = false;
	}
}
