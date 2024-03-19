# Documentation to RTS zápočťák (name in progress)

## Basic concept
This project is a toolkit for creation of Real time strategy games (RTS).
RTS games in general have converged on some design principles which I tried to capture in this project.
### Technology used
This project is built using Godot Engine v4.2.1
C# is used as a primary scripting language with C++ modules planned for the future.

## Features
#### Godot scenes paired with C# scripts for:
1. Menus
2. Maps
3. Units, buildings, interactable objects
4. Abilities

## Project structure
### Godot side
Menus and Gamelevels are scenes that are switched between. There is no managing node.
#### Menu
Menu allows to:
- "start campaign" which starts the first (and only level)
- Select a level: looks in the Levels directory for level files. Will not load files with wrong extensions and if a .tscn file is not a valid level it will recognise it upon selecting said level and will mark it as invalid.
- Settings does nothing
- Exit exits the game

#### Gameplay
- Player scene is the ancestor scene for HumanPlayer and AI players.
  - It contains only basic nodes for tracking ownership of ```Selectable```(more on them in C# section)
  - Contains a FogofWar node that currently does nothing but is meant to handle the vision of the player
  - HumanPlayer inherits from Player
    - Contains UI elements whose purpose should be obvious
    - SelectRect - the Rectangle that is shown when selecting units
  - BuildOrderPlayer
    - Currently indistinguishable from Player except for script. Therefore placement of this player can be done either by placing player and then changing its script or directly adding BuildOrderPlayer.tscn
- Unit&Building - Characterbody2D set up for topdown game
On C# side they inherit from the same parent so they are very similar even in Godot
(Godot scene inheritance is dangerous and unpredictable and therefore there is no such complex inheritance on the Godot side)
"Abstract" scene for Units/Buildings to inherit from (there is nothing enforcing the abstractness)
  - SceneTree:
    - CollisionShape2D for collision
    - UnitPortrait - meant to be displayed on the HUD when Selectable is selected (the naming is confusing W.I.P.)
    - Graphics (separateScene)
    - VisionArea - used for agro range and planned to be used for Player vision
    - Attacks - node meant to hold Attack (separate scene)
    - Abilities - node meant to hold Abilities (added through the inspector)
  - Inspector: (Since Godot doesn't allow editor description for C# Exported values there will be a brief decription here)
    - Damagable
      - Max Hp - initial HP value
    - Selectable
      - SName - unique name for any unit or selectable (Root node name is unreliable and subject to changes at the whims of the engine)
      - Export Abilities - Array of pairs of abilities (Godot.Dictionary doesn't allow for type safety and thus is simulated as an array of pairs (Abilities with the same position will produce errors))
        - Each element is of the ```AbilityPair``` type.
          - **Pos** is the position of the ability on the UI grid (enumerated by rows (like western text)).
          - **Ability** is the ```AbilityRes``` that will initialize the ```Ability``` in game
- Unit - equiped with movement
  - SceneTree:
    - NavAgent - used for navigation
  - Inspector: 
    - Speed - movement speed
- Buildings
  - SceneTree:
  - Inspector:

- SelecableGraphic a mandatory component for any ```Selectable```, contains UI elements
  - Selected - indicator whether a ```Selectable``` is hovered over or selected
  - Pathline - graphic indicator of a path (units use it to show their path, buildings use it for Rally points)
  - AnimationPlayer for animations
  - Sprite2D for the visuals in general
  - Cross - used in DeathAnimations (instead of DeathAnimations units display a flashing cross over them when they die)
- DamagableGrpahic inherits from SelectableGrahpic
  - Adds Healthbar
- Attack -> Units and Buildings work as platforms that are equipped with attacks.
  - Graphic - Units and buildings don't have attack animations. Instead each attack has its own (usually a sprite of sword appearing to slash and disapearing)
  - AttackRange - Area which Attack monitors for its target

- Abilities - pair of ```<ushort,AbilityRes>``` (explained above)

- Map
  - New maps are added under res://scenes/Levels/
  - contains all Players present in the level and the Tilemap of the level
  - Inspector:
    - Gamespeed - controls the speed the game runs on.
      - Affects:
        -  ability cooldowns
        -  movement speed
      - Does not affect
        - Animation speed
- UI/resource_ui
  - Added to HumanPlayer (under Camera2D/HUD/TopBar/ResourceTab) when adding new resource.
  - HumanPlayer currently has two.


### C# by namespaces:
Unless specified otherwise each class resides in a file of the same name.
- ```RTS.mainspace```
  - ```Menu```  Main menu (script that controls the initial scene (buttons etc))
  - ```GodotExtensions``` - Meant to contain Extensions for ```Godot.Collections```,```Godot.Node``` etc.
  - ```SavingLoading``` - skeleton for Saving and Loading
    - Does nothing
- ```RTS.Gameplay``` - Contains gameplay code
  - ```Selectable```
    - A abstract parent class for anything that can be interacted with in the game.
    - Is to be connected to basic graphics including UI elements
    - Has mechanism for highlighting upon hover, being clicked and displays Abilities.
    - Makes comparisons between Selectables and the child types of Selectable. f.e. Units always have priority above buildings and those above other Selectables.
    - Contains CommandQueue so that commands can be queued ("command" is the use of an Ability)
    - Uses VisionRange for the selectable -> a circular Area2D used for Agroing and in the future for Player vision.
  - ```Damagable```
    - Inherits from ```Selectable```
    - Handles all things with Health
    - Contains graphics for Healthbar (There in no separate DamagableGraphics class)
    - Contains signals for Damaged, Health changed and Dead
  - ```Unit```&```Building```
    - Inherits from ```Damagable```
    - Compares to other Units/Buildings currently based on unit health, age in scene and other factors
  - ```Unit```
    - Has a NavigationAgent and other functions facilitating movement.
    - Commands implemented are:
      - Move command - moves towards specified location or unit ignoring anything else
      - Attack command
        - If given location will move there attacking anything on the way
        - If given unit will Move to attack it ignoring anything on the way
        - ```Unit``` will use the ```RetargetAttacks``` function to give a target to its attacks
      - If a Unit is specified with either Move or Attack command the unit will follow it until given different command.
  - ```Building```
    - Has a rally point for unit production and potentially some abilities may use it.
    - If a unit is specified as the target of the rally point it will move with the unit.
  - ```Ability``` & ```AbilityRes```
    - ```Ability``` is a child of ```Node``` and is actually running in the game. It is an abstract class and is used as a base for all Abilities. It contains cooldowns and the OnClick function that gets called upon pressing the Ability button in the UI.
    - ```AbilityRes``` is a child of ```Resource``` and is used in the editor to assign ability to a Selectable and is initializing it.
    - ```TargetedAbility``` is a child of ```Ability``` and adds abstract function that recieves target from the ```Player``` that is using the ```Selectable```. It is used for Abilities that need a target.
    - Location - a struct that is used as a way to give Vector2 the ITargetable interface
    - ```ITargetable``` an interface used by Location and Selectable. Used for targeting of movement and Abilities
  - ```Attack```
    - Script for the Attack Node.
    - Enables the functionality described above in the Godot section about the Attack node
- ```RTS.Graphics``` - Contains scripts for ```Selectable``` graphics
- ```RTS.UI``` - Contains scripts for UI elements
  - Often small scripts that often just bridge the gap between the Node structure and code
- ```RTS.Physics``` - Contains the physics structs (all located in **Physics.cs**)
  - PhysicsConsts - a way to pass constants into the physics structs
  - PhysicsValues - a way to pass parametrized values into the physics structs
  - TilesPerSecond - a unit of speed
  - Persec - a unit of frequency
  - Tilemeter - a unit of distance (in tiles)
  - PhysicsExtensions - Extension class used primarly to convert standart types to Physics.