using Godot;
using RTS.Physics;
using System;

namespace RTS.Gameplay
{
    /// <summary>
    /// <para>Signifies if the Ability has to have a target.</para>
    /// <para>I am currently assuming this is a binary state and there are only Targeted/NonTargeted Abilities</para>
    /// Every child specifies
    /// 1.what happens when the target is recieved (if we store the target in any way is up to it)
    /// 2.What happens when the target is reached
    /// </summary>
    public abstract partial class TargetedAbility : Ability
    {
        protected Area2D detectionZone;
        float range = 0;
        /// <summary>
        /// Returns range with the added size of the Selectable ownign it.
        /// </summary>
        public float Range
        {
            get =>
                range
                + OwningSelectable.SizeRadious//adding size of the Selectable
                + 14;//A hair extra so it works why fourteen? Cause 4 was not good enough and unit increases seemed too weak
        }
        /// <summary>
        /// Triggered upon specifiing the target (Ability.OnClick has been triggered and then a Target has been given)
        /// </summary>
        /// <param name="target"></param>
        public virtual void OnTargetRecieved(Target target)
        {
            if (OwningSelectable.Position.DistanceTo(target.Position) <= Range) OnTargetReached();//If it is within range already
            else detectionZone.Monitoring = true;
        }
        /// <summary>
        /// Triggers upon getting within abilityrange of the Target
        /// </summary>
        public virtual void OnTargetReached()
        {
            if (!Channeled) detectionZone.Monitoring = false;
        }//I chose not to disable monitoring here cause sometimes an ability might want to be channeled.
        public override void _Ready()
        {
            base._Ready();
            detectionZone = new();
            detectionZone.AddChild
                (new CollisionShape2D()
                {
                    Shape = new CircleShape2D()
                    {
                        Radius = Range
                    }
                }
                );
            AddChild(detectionZone);
            detectionZone.Monitoring = false;//we don't want to use it if it is not necessary (if we ímplement autocast then it might wanna be changed)
            detectionZone.BodyEntered += (shape) => { if (IsTarget(shape)) OnTargetReached(); };
        }
        /// <summary>
        /// Check whether an object that entered the Abilities radious is desired target
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        protected abstract bool IsTarget(Node2D shape);

    }
    public abstract partial class Ability : Node
    {
        protected Second cooldown = 0;
        /// <summary>
        /// <para>Method bound to an ability button calls the OnUse function. Is used for HumanPlayers so Abilities can have a UI part</para>
        /// <para>Has a reference to its button and through it to the rest of the scene</para>
        /// </summary>
        /// <param name="button"></param>
        public void BaseOnClickUI(AbilityButton button)
        {
            OnClickUI(button);
            BaseOnUse();
        }
        public virtual void OnClickUI(AbilityButton button) { }
        /// <summary>
        /// <para>Triggers upon pressing the button by the player (called from OnClickUI) or by various AI players</para>
        /// <para>Is what ability does right after using it</para>
        /// doesn't need to do anything
        /// </summary>
        public abstract void OnUse();
        /// <summary>
        /// This enforces cooldowns.
        /// OnUse is the overriden part
        /// </summary>
        public void BaseOnUse()
        {
            if (OnCooldown) return;//Done here again for the sake of nonUI using players
            cooldown = Cooldown;
            OnUse();
        }
        public override void _Process(double delta)
        {
            base._Process(delta);
            if (cooldown > 0) cooldown -= delta;
            else
            {
                cooldown = 0f;
            }
        }

        public Texture2D Icon;//TODO: Need to get it from somewhere also should probably set it as abstract too so it has to be set
        /// <summary>
        /// <para>Ability name and the text that displays on the Button. Its called Text so it doesn't colide with <c>Name</c> which is the name of the root <c>Node</c></para>
        /// <para>Its abstract cause its mandatory</para>
        /// <para> We generaly throw exceptions on set except in generated names but maybe because of translations we might wanna change it later </para>
        /// </summary>
        public abstract string Text { get; set; }
        [Export]
        public Key Shortcut { get; set; } = Key.None;
        /// <summary>Ability cooldown (static value) (<c>cooldown</c> is the dynamic time remaining)</summary>
        public abstract Second Cooldown { get; }
        /// <summary>
        /// True if Ability is cooling down
        /// </summary>
        public bool OnCooldown
        {
            get
            {
                if (Cooldown == 0) return false;
                else return cooldown > 0;
            }
        }
        ///<summary> Describes whether ability is Active or Passive (button is either enabled or not)</summary>
        public abstract bool Active { get; }
        public bool Channeled { get; set; } = false;//Whole different rabbithole. Gotta check for bodyexits etc.
        /// <summary>
        /// The selectable that owns this Ability
        /// </summary>
        public Selectable OwningSelectable { get; set; }
    }
}

