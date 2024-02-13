using Godot;

//using MonoCustomResourceRegistry;
namespace RTS.Gameplay
{
    [GlobalClass]
    public partial class BuildingBlueprint : Resource
    {
        PackedScene packed;
        Building building;
        [Export]
        PackedScene PackedBuilding { get; set;
            /*{
                var node = value.Instantiate<CharacterBody2D>();
                //GD.Print(((CSharpScript)node.GetScript()).SourceCode);
                GD.Print(node.GetChildCount());

                GD.Print(GD.Load<PackedScene>("res://assets/Buildings/Barack/Building_Barrack.tscn").Instantiate<Building>().Name);


                building = value.Instantiate<Building>();
                GD.Print(building.Name);
                packed = value;
            }*/
        }
        public Building GetBuilding()
        {
            return PackedBuilding.Instantiate<Building>();
        }
    }
}