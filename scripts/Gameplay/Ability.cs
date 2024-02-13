using Godot;
using RTS.Physics;
using System;

namespace RTS.Gameplay
{
    /// <summary>
    /// <para>Signifies if the Ability has to have a target.</para>
    /// <para>I am currently assuming this is a binary state and there are only Targeted/NonTargeted Abilities</para>
    /// </summary>
    [GlobalClass]
    public abstract partial class TargetedAbility : Ability
    {

        public abstract void OnTargetRecieved(Target target);
    }
    [GlobalClass]
    public abstract partial class Ability : Node
    {
        private Second cooldown = 0;
        /// <summary>
        /// <para>Triggers upon pressing the button (later also on Shortcut)</para>
        /// <para>Has a reference to its button and through it to the rest of the scene</para>
        /// <para><c>base.OnClick(button)</c> should always be called even when overriding as it will handle Cooldowns</para>
        /// </summary>
        /// <param name="button"></param>
        public virtual void OnClick(AbilityButton button)
        {
            if (OnCooldown) return;
            if (Cooldown != 0)
            {
                cooldown = Cooldown;
            }

        }
        public override void _Process(double delta)
        {
            base._Process(delta);
            if(cooldown>0) cooldown -= delta;
            else
            {
                cooldown = 0f;
            }
        }

        public Texture2D Icon;//TODO: Need to get it from somewhere also should probably set it as abstract too so it has to be set
        /// <summary>
        /// <para>Ability name and the text that displays on the Button. Its called Text so it doesn't colide with <c>Name</c> which is the name of the root <c>Node</c></para>
        /// <para>Its abstract cause its mandatory</para>
        /// </summary>
        public abstract string Text { get; set; }
        public Key Shortcut { get; set; } = Key.None;
        /// <summary>Ability cooldown (static value) (<c>cooldown</c> is the dynamic time remaining)</summary>
        public abstract Second Cooldown { get; }
        public bool OnCooldown { get
            {
                if (Cooldown == 0) return false;
                else return cooldown <= 0;
            } }
        ///<summary> Describes whether ability is Active or Passive (button is either enabled or not)</summary>
        public abstract bool Active { get; }
        public Selectable OwningSelectable { get; set; }
    }
}

