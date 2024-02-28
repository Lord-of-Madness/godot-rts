using Godot;
using Godot.Collections;
namespace RTS.Gameplay
{
    [GlobalClass]
    public partial class MenuAbilityRes : AbilityRes
    {
        [Export]
        public override string Text { get; set; }
        [Export]
        public Array<AbilityPair> ExportAbilities = new();
        public override MenuAbility Instantiate(Selectable owner)
        {
            Dictionary<int, Ability> abilities = new();
            foreach (var pair in ExportAbilities)
            {
                abilities.Add(pair.pos, pair.ability.Instantiate(owner));
            }
            return new MenuAbility(abilities,owner,Text);
        }
    }
}