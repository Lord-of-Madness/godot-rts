using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using RTS.UI;
using System.Linq;


namespace RTS.Gameplay
{
    public interface ITargetable//Interface might be better than a class ->TODO conisder
    {
        public Vector2 Position { get; }
        public string ToString();
    }


    //[StructLayout(LayoutKind.Explicit)]//Tried making it into C++esque  Variant-like object. Probably silly and completely unecessary. And it didn't even work(even making it a struct broke something somewhere)
    public class Target
    {
        public enum Type
        {
            Location,
            Selectable
        }
        public Target(Selectable selectable)
        {
            type = Type.Selectable; this.selectable = selectable;
        }
        public Target(Vector2 location)
        {
            type = Type.Location;
            this.location = location;
        }
        public Target(Target other)
        {
            type = other.type;
            selectable = other.selectable;
            location = other.location;
        }
        //[FieldOffset(0)]
        public Type type;
        //[FieldOffset(sizeof(Type))]
        public Selectable selectable;
        //[FieldOffset(sizeof(Type))]
        public Vector2 location;
        public Vector2 Position
        {
            get
            {
                if (type == Type.Location) return location;
                else return selectable.Position;
            }
        }
        public override string ToString()
        {
            if (type == Type.Location) return location.ToString();
            else return selectable.ToString();
        }
    }

    //[Tool]
    public partial class Player : Node
    {
        private Team team;
        [Export]
        public Team Team { get => team; private set => team = value; }
        public string name = "Player";

        protected SortedSet<Selectable> Selection = new();
        protected Selectable HighlightedSelectable { get => Selection.Min; }//TODO: Tabing through groups etc.
        public Map localLevel;


        //[Export] private bool EditorPause { get; set; } = true;

        private Array<GameResource> gameresources;//having a backing field made sense back when we were trying to be adding resources from the editor. For now its a relic.
        public Array<GameResource> Game_Resources
        {
            get => gameresources;
            set
            {

                //HBoxContainer rtab = GetNode<Camera2D>(nameof(Camera2D)).GetNode<CanvasLayer>(nameof(HUD)).GetNode<ColorRect>(nameof(TopBar)).GetNode<HBoxContainer>(nameof(ResourceTab));
                gameresources = value;
                /*for (int i = 0; i < gameresources.Count; i++)
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
                }*/
            }
        }



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

        public void TogglePause(bool toggleOn)//this might get moved into the player but it could also be handy to use a neutral player who could control some campaign stuff to use it too.
        {
            Input.MouseMode = toggleOn ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Confined;
            localLevel.GetTree().Paused = toggleOn;
        }

    }
}