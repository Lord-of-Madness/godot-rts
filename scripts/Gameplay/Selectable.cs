﻿using Godot;
using RTS.Graphics;
using RTS.Physics;
using Godot.Collections;
using System;
using System.Collections.Generic;

namespace RTS.Gameplay
{
    public static class TeamExtension
    {
        public static bool IsHostile(this Team team, Team otherteam)
        {
            return team != otherteam;
        }
    }
    public enum Team
    {
        Team1,
        Team2,
        Team3,
        Team4
    }
    [GlobalClass]
    public abstract partial class Selectable : CharacterBody2D, IComparable<Selectable>, ITargetable
    {
        [Signal] public delegate void SignalDisablingSelectionEventHandler(Selectable selectable);//when dead, loss of control etc.
        public enum SelectableAction
        {
            Move,
            Attack,
            Idle,
            Stay,
            Patrol,
            Dying,
            UsingAbility
        }
        private Queue<SelectableAction> CommandQueue = new();
        /*
         * TODO: Implement Queuing of Commands. (UnitAction might not be the thing)
         * I am considering making some kind of Command Class cause we need to store a bunch of data about different Commands.
         * Well technicaly we need to store what is the UnitAction for the duration of this here Command
         * And in case of Abilities we need to ensure we know what ability does the thing.         
         */

        public Godot.Collections.Array<Attack> Attacks;
        //this was supposed to be done from the inspector but the Attacks weren't unique (It kept interacting with just the last attack on screen so now its a special node in the scene tree under which the attacks are.)


        //This will be action queue later now it shall be just one command.
        private SelectableAction ca;
        public SelectableAction CurrentAction
        {
            get { return ca; }
            set
            {
                /*if (ca == UnitAction.Attack && value != UnitAction.Attack)
                {
                    GD.Print("Undoing");
                    foreach (Attack attack in Attacks)
                    {
                        attack.AttackRange.BodyEntered -= AttackTargetInRange;
                    }

                }*/
                if (CurrentAction == SelectableAction.Dying) return;
                ca = value;
            }
        }

        public SelectableGraphics Graphics;
        public Team team;
        public HumanPlayer Beholder;//This should not be exported and it should be Beholder and not Owner
        public Player OwnerPlayer;

        public float SizeRadious { get => ((CircleShape2D)GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape).Radius; }

        [Export]
        public string SName { get; set; }//Name of the Selectable. Must be unique

        [Export]
        public Array<AbilityPair> ExportAbilities = new();

        public Node AbilityNode;
        public Godot.Collections.Dictionary<ushort, Ability> Abilities = new();

        public Area2D VisionArea;
        [Export(PropertyHint.Range, "0,10,1,or_greater")]
        public float visionRange;//in Tilemeters
        public Tilemeter VisionRange { get => (Tilemeter)visionRange; set { visionRange = (float)value; } }
        /// <summary>
		/// Graphicaly shows the selected status.
		/// </summary>
		public void Select()
        {
            Graphics.Select();
        }
        /// <summary>
		/// Graphicaly deselects.
		/// </summary>
        public void Deselect()
        {
            Graphics.Deselect();
        }
#pragma warning disable IDE1006 // Naming Styles (Godot uses GDScripts naming conventions and VisualStudio doesn't like it at all)
        public void _on_mouse_entered()
#pragma warning restore IDE1006 // Naming Styles
        {
            Beholder?.JustHovered(this);
        }
#pragma warning disable IDE1006 // Naming Styles
        public void _on_mouse_exited()
#pragma warning restore IDE1006 // Naming Styles
        {
            Beholder?.DeHovered(this);
        }
        public abstract void Command(Player.ClickMode clickMode, ITargetable target, Ability ability = null);
        public virtual void CleanCommandQueue()
        {
            CommandQueue = new();
        }
        public override void _Ready()
        {
            Graphics = GetNode<SelectableGraphics>(nameof(Graphics));
            VisionArea = GetNode<Area2D>(nameof(VisionArea));
            AbilityNode = GetNode<Node>(nameof(Abilities));
            Attacks = GetNode<AttacksNode>(nameof(Attacks)).Attacks;
            ((CircleShape2D)VisionArea.GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape).Radius = VisionRange.Pixels;
            if(GetTree().CurrentScene.HasNode(nameof(HumanPlayer)))//Used for the Menu thingie
                Beholder = GetTree().CurrentScene.GetNode<HumanPlayer>(nameof(HumanPlayer));
            foreach (var abilityPair in ExportAbilities)//Dictionaries don't work in Export so gotta hack it in like this to get a proper Dict
            {
                var ab = abilityPair.ability.Instantiate(this);
                Abilities.Add(abilityPair.pos, ab);
                AbilityNode.AddChild(ab);
            }
            OwnerPlayer = GetParent().GetParent<Player>();
            team = OwnerPlayer.Team;
            if (Attacks.Count > 0)
            {
                VisionArea.BodyEntered += TryAgro;
            }

        }
        /// <summary>
        /// Checks if the node can be attacked (is <c>Damagable</c> and hostile)
        /// </summary>
        /// <param name="node"></param>
        public virtual void TryAgro(Node2D node)
        {
            if (node is Damageable damagable && damagable.team.IsHostile(team))
            {
                RetargetAttacks(damagable);
            }
        }
        /// <summary>
        /// Calls retarget on all available attacks
        /// </summary>
        /// <param name="target"></param>
        protected void RetargetAttacks(ITargetable target)
        {
            if (Attacks is not null)
            {
                foreach (Attack attack in Attacks)
                {
                    /**/
                    attack.Retarget(target);
                    /*
                    attack.AttackRange.BodyEntered += (body) =>
                    {
                        AttackTargetInRange(body, attack);
                    };*/
                    //attack.AttackRange.BodyEntered += (Node2D smthn) => { GD.Print("Whatever"); };
                    //attack.AttackRange.AreaEntered += (Area2D smthn) => { GD.Print(smthn); };
                    //GD.Print(((CircleShape2D)attack.AttackRange.GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape).Radius);

                }

            }
        }
        protected void DetargetAttacks()
        {
            if (Attacks is not null)
            {
                foreach (Attack attack in Attacks)
                {
                    attack.Detarget();
                }

            }
        }
        /// <summary>
		/// Tries to compare it if its the same type (<c>Unit</c>/<c>Building</c>) Units are above Buildings. Otherwise sorted by Age
		/// </summary>
        public int CompareTo(Selectable other)
        {
            if (other == this) return 0;
            if (other is Unit otherunit)
            {
                if (this is Building) {
                    return 1; }
                else if (this is Unit unit) return unit.CompareTo(otherunit);//Not certain if these can even be called. Wouldn't the tighter Comparer be used anyway?
            }
            else if (other is Building otherbuilding)
            {
                if (this is Unit) return -1;
                else if (this is Building building) return building.CompareTo(otherbuilding);
            }

            //Ordering by age in SceneTree-> only happens if we haven't implemented better ordering
            return GetIndex().CompareTo(other.GetIndex());

        }
        public override string ToString()
        {
            return base.ToString() + " " + Name;
        }
        public override void _ExitTree()
        {
            base._ExitTree();
            //TODO signal this so people can detarget?
        }
    }
}
