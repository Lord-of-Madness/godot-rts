using Godot;

//using MonoCustomResourceRegistry;
namespace RTS.Gameplay
{
    [GlobalClass]
    public partial class BuildingBlueprint : Resource
    {
        PackedScene packed;
        Building building;
        public Building Building
        {
            get { 
                building ??= PackedBuilding.Instantiate<Building>();//this Typechecks at runtime.
                return building;
            }
        }
        [Export]
        PackedScene PackedBuilding { get=>packed; set
            //The following makes sense only in Toolmode. But it also needs Building to exist in Toolmode otherwise Building class doesn't exist yet and Instantiate wouldn't instantiate into Building but CharacterBody2D
            //Note: Just making Building into a Toolscript isn't enough. Perhaps C++?
            //Its purpose is to Typecheck the PackedScene for PackedScenes are untyped
            {
                //building = value.Instantiate<Building>();
                packed = value;
            }
        }
    }
}