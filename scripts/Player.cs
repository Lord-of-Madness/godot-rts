using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using RTS.UI;
using RTS.mainspace;


namespace RTS.Gameplay
{

    /// <summary>
    /// Parent class for all Player-like agents
    /// </summary>
    public partial class Player : Node
    {
        private Team team;
        [Export]
        public Team Team { get => team; private set => team = value; }
        public string name = "Player";

        /// <summary>
        /// Reference to the local Map
        /// </summary>
        public Map localLevel;


        //[Export] private bool EditorPause { get; set; } = true;

        //private Array<GameResource> gameresources;//having a backing field made sense back when we were trying to be adding resources from the editor. For now its a relic.
        public Array<GameResource> Game_Resources { get; set; }
        /*{
           get => gameresources;
           set
           {

               //HBoxContainer rtab = GetNode<Camera2D>(nameof(Camera2D)).GetNode<CanvasLayer>(nameof(HUD)).GetNode<ColorRect>(nameof(TopBar)).GetNode<HBoxContainer>(nameof(ResourceTab));
               gameresources = value;
              for (int i = 0; i < gameresources.Count; i++)
               {
                   if (gameresources[i] == null)
                   {
                       gameresources[i] = new GameResource()
                       {
                           Name = nameof(GameResource) + i.ToString()
                       };
                       rtab.AddChild(gameresources[i]);
                       gameresources[i].Owner = GetTree().EditedSceneRoot;
                   }
               }
           }
       }*/


        /// <summary>
        /// Signifies what of the basic actions the unit is going to take upon clicking.TODO: Should be replaced by Abilities altogether
        /// </summary>
        public enum ClickMode//Is this useful for nonhuman players?
        {
            Move,
            Attack,
            Patrol,
            Defend,
            UseAbility
        }

        public override void _Ready()
        {
            localLevel = GetParent<Map>();
        }
        /// <summary>
        /// Pauses and unpauses the game
        /// </summary>
        /// <param name="toggleOn"></param>
        public void TogglePause(bool toggleOn)
        {
            Input.MouseMode = toggleOn ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Confined;
            localLevel.GetTree().Paused = toggleOn;
        }

    }
}