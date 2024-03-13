using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using RTS.UI;
using RTS.mainspace;


namespace RTS.Gameplay
{
    public interface ITargetable//Interface might be better than a class ->TODO conisder
    {
        public Vector2 Position { get; }
        public string ToString();
    }
    public struct Location : ITargetable
    {
        private Vector2 data;
        public readonly Vector2 Position => data;
    }


    /// <summary>
    /// Is either <c>Selectable</c> or <c>Location</c> (this is stored in the <c>type</c> property)
    /// <para><c>Position</c> returns appropriate position regardless of type</para>
    /// </summary>
    /// <remarks>
    /// <para>struct-like object (cannot be struct for some Godot related reasons)</para>
    /// TODO: figure out why exactly
    /// <para>Should never add empty constructor - causes errors down the line. If needed use <c>Vector.Zero</c> as parameter
    /// (cannot add as default parameter either cause it is not a compile time constant for  some reason).
    /// If needed empty constructor oughta just call the Vector2 constructor with the <c>Vector.Zero</c> argument.
    /// Not adding it right now because I want to read this and think about it if I am adding it</para>
    /// </remarks>
    //[StructLayout(LayoutKind.Explicit)]
    //Tried making it into C++esque  Variant-like object. Probably silly and completely unecessary.
    //And it didn't even work(even making it a struct broke something somewhere (probably bacause of Godot))
    public class Target
    {
        public enum Type
        {
            Location,
            Selectable
        }
        public Target(Selectable selectable)
        {
            type = Type.Selectable;
            this.selectable = selectable;
        }
        public Target(Vector2 location)//Cannot add default Vector.Zero cause it is not compile time constant
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
        /// <returns>Position regardless of type</returns>
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
        /// Currently selected <c>Selectable</c>s
        /// </summary>
        protected SortedSet<Selectable> Selection = new(Comparer<Selectable>.Create(
            (a, b) => //a.CompareTo(b)));//this doesn't make em equal when they should be
            {
                int res = a.SName.CompareTo(b.SName);
                if (res == 0) return a.Name.CompareTo(b.Name);
                else return res;
            }));
        //NOTE: If we need to reasign it elsewhere it would make sense to make it a custom type so we don't accidentaly use wrong comparers.
        //Also - I kinda guessed the Comparer maybe its incorrect I don't know .
        // It didn't work with default comparer. Not certain why.
        /// <summary>
        /// The current highlighted segment of the <c>Selection</c>
        /// </summary>
        protected Selectable HighlightedSelectable { get => Selection.Min; }//TODO: Tabing through groups etc., highlighting
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