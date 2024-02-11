using Godot;
//using MonoCustomResourceRegistry;
using RTS.Gameplay;
using System;
namespace RTS.Gameplay
{
//    [RegisteredType(nameof(BuildAbility),"",nameof(Resource))]//
    [GlobalClass]
    public partial class BuildAbility : Ability
    {
        public override void OnClick()
        {
            throw new NotImplementedException();
            //TODO: display building list somehow
        }

        public override string Text => "Build";
        
    }
}