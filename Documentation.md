# Documentation to RTS zápočťák (name in progress)

## Basic concept
This project is a RTS base. It attempts to create the foundation for a Warcraft/Starcraft-like RTS.
RTS games in general have converged on some design principles which I tried to capture in this project.
### Technology used
This project is built using Godot Engine v4.2.1
C# is used as a primary scripting language with C++ modules planned for the future.

## Project structure
### Godot side:
Menus and Gamelevels are scenes that are switched between. There is no managing node.
#### Menu
Menu allows to:
- "start campaign" which starts the first (and only level)
- Select a level: looks in the Levels directory for level files. Will not load files with wrong extensions and if a .tscn file is not a valid level it will recognise it upon selecting said level and will mark it as invalid.
- Settings does nothing
- Exit exits the game

#### Gameplay
- Player scene is the ancestor scene for HumanPlayer and future AI players.
  - It contains only basic nodes for tracking ownership of ```Selectable```(more on them in C# section)
  - Contains a FogofWar node that currently does nothing but is meant to handle the vision of the player
  - HumanPlayer
    - Contains UI elements
- Unit contains the skeleton for any unit -> collision, nodes that are to be filled with required textures, UI elements etc.
  - Specific units inherit from this scene and add textures and attacks
- Buildings work the same way as units but have no movement. Otherwise fairly similar in structure
- Attack -> Units and Buildings work as platforms that are equipped with attacks.
- Abilities -> Abilities are added to the Unit node via the inspector.
  - Unit contains ```Exported Abilities``` - an array of ```AbilityPair``` which is just a wrapper class effectively meaning ```<ushort,AbilityRes>``` where the ```ushort``` signifies the position in the UI and the ```AbilityRes``` is the ```Godot.Resource``` that initializes ```Ability``` itself. (more on that in C#)
- Map
  - contains all Players present in the level and the Tilemap of the level
  - Contains configuration for Gamespeed


### C# by namespaces:
- ```RTS.mainspace```
  - Contains intial menu
  - Extension classes
    - Meant to contain Extensions for ```Godot.Collections``` and such. Currently contains ```ToGodotArray``` a function that turn ```IEnumerable``` to ```Godot.Array```
  - skeleton for Saving and Loading
    - Does nothing
- ```RTS.Gameplay```
  - Contains gameplay code
  - ```Selectable```
    - A abstract parent class for anything that can be interacted with in the game.
    - Is to be connected to basic graphics including UI elements
    - Has mechanism for highlighting upon hover, being clicked and displays Abilities.
    - Makes comparisons between Selectables and the child types of Selectable. f.e. Units always have priority above buildings and those above other Selectables.
    - Contains CommandQueue so that commands can be queued ("command" is the use of an Ability)
    - Contains the VisionRange for the selectable -> a circular Area2D used for Agroing and in the future for Player vision.
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
- ```RTS.Graphics```
  - Contains scripts for ```Selectable``` graphics
- ```RTS.UI```
  - Contains scripts for UI elements
- ```RTS.Physics```
  - Contains the physics structs