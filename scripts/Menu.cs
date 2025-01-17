using Godot;
using System.Collections.Generic;
using System.Linq;

namespace RTS.mainspace
{
    public partial class Menu : Control
    {
        private Control MainMenu;
        private Control ChapterMenu;
        private readonly List<string> Levels = new();

        bool toggleOn = false;
        double cooldown = 0;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            Input.MouseMode = Input.MouseModeEnum.Confined;
            MainMenu = GetNode<Control>(nameof(MainMenu));
            ChapterMenu = GetNode<Control>(nameof(ChapterMenu));
            MainMenu.GetNode<Button>("ButtonCampaign").Pressed += Campaign;
            MainMenu.GetNode<Button>("ButtonChapters").Pressed += Chapters;
            MainMenu.GetNode<Button>("ButtonExit").Pressed += ExitGame;

            DirAccess directory = DirAccess.Open("res://scenes/Levels");
            //TODO could add error checking and file validity ahead (currently we are doing it upon trying to click said button.)
            //Also we could do it more file-systemy eventually (support for directories etc.)
            foreach (var file in directory.GetFiles())
            {
                if (file.Split('.').Last() == "tscn")
                    Levels.Add(file);
            }
            directory.ListDirEnd();
            ChapterMenu.GetNode<Button>("BACK").Pressed += Back;

            foreach (string lvl in Levels)
            {
                string name = lvl.Split('.')[0];
                Button button = new()
                {
                    Name = name,
                    Text = name,
                    CustomMinimumSize = new(0, 40)
                };
                ChapterMenu.GetNode<VBoxContainer>("ScrollContainer/Chapters").AddChild(button);
                button.Pressed += () => LevelSelector(lvl);
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
            if (!(Error.Ok == GetTree().ChangeSceneToFile($"res://scenes/Levels/{lvl}")))
            {
                //If the file is not a scene:
                Button button = ChapterMenu.GetNode<VBoxContainer>("ScrollContainer/Chapters").GetNode<Button>(lvl.Split('.')[0]);
                button.Pressed -= () => LevelSelector(lvl);//Unbind the func
                button.Disabled = true;
                button.Text = "INVALID";
            }
        }
        public override void _Process(double delta)
        {
            base._Process(delta);
            if (cooldown > 0) cooldown -= delta;
        }
        public override void _UnhandledKeyInput(InputEvent @event)
        {
            base._UnhandledKeyInput(@event);
            if (@event is InputEventKey key)
            {
                if (cooldown <= 0 && key.Keycode == Key.Escape)
                {
                    toggleOn = !toggleOn;
                    cooldown = 1d;
                    Input.MouseMode = toggleOn ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Confined;

                }
            }
        }
        private void Back()
        {
            MainMenu.Visible = true;
            ChapterMenu.Visible = false;
        }
    }
}