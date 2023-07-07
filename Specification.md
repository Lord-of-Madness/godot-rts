# Name: "RTS game"(WIP)

# Autor: Jiří Malý

# Overview:
 - The goal is to create a game in the RTS genre in the GODOT game engine.

# Tools:
- Godot Engine (v4.0.3. at the point of writing)
- Primary scripting language: C#
- High performance with C++
- GDScript for specific use cases
# Main Goal:
    Create playable game.

# Features:
1. Menu  
    - Campaign selection  
    - Map selection  
        - Based on the contents of the map folder (Allowing making additional maps for the game.)  
    - Settings  
       - Gamespeed: All game physics and time-related events will be multiplied by Gamespeed.  
    - Background: 
        - Will run a 4player FFA as a visual background and as a benchmarking tool for the player to test how will his PC preform in game. (Disabled by default, enabled in settings and keybound to probably some function key. Disable on crash (If I can figure out how to do it.))
2. Gameplay:  
    - Unit movement and combat - melee and ranged attacks  
    - Unit stats and abilities  
    - Buildings and unit production.  
    - Objectives (for map completion and/or events)  
    - Maps - terrain and resources  
    - Save/Load progress in maps/campaign.  
3. InGame UI:  
    - Resource counters  
    - Menu  
    - Unit actions/abilities  
    - Unit cards  
    - Minimap  
    - View of selected units  
4. Content:  
    - Campaign -> ammount of missions depending on time.  
    - Ability to make maps accesible from game menu  
5. Graphics:  
    - Free-to-use or custom pixel art.  
6. Sounds:  
    - Free-to-use or custom music and sounds  
7. Platform:  
    - Primary Windows, but it shouldn't be difficult to make it multiplatform.
    - Note that Godot 4.0.3. doesn't support consoles yet and doesn't support C# for mobile platforms. (Leaving us with PC only for now.)  

# Priorities:
1. Gameplay and overall playability
2. Level design and the actual content
3. Graphic and sound design
4. Try and code the game in such a way that adding multiplayer isn't a big problem (Make the Player part of the code as separated from gameplay code as possible)



# Game Structure:

```
Root(Probably just an empty node that will help make switching scenes easier(Perhaps will carry some information between Menus and Game sessions))
│───Menu  
│
└───GameSession(handles the map transitions in case of missions with multiple maps.
        Handles more complex scripted events in campaign scenarios. Fairly inactive in PvP)
│   │───Map1
│   │    Selectables (Units,Buildings)
│   │    Triggers
│   │───Map2
:   :       
│   │
│   │───PlayerInterface1 (facilitates Player data local to the play session)
│   │       Resources
│   │       Local statistics
│   │       Gameplay code
│   │───PlayerInterface2
:   :
│   │───AIPlayer1
│   │   Currently figuring out whether AI player and player will be of the same ancestor or with shared interface
│   └───AIPlayer2
│   
└───Player (Account related things)
    │   Saves
    │   (Replays perhaps eventually)
    │   (statistics)
    │   (campaign progress)
```
## Class hierarchy
```
Selectables (Anything on the game map that can be selected: Buildings, units, special things)
    Selectable will be highlighten upon selection and List<Selectable> should be last resort selection for PlayerInterfaces(Perhaps will not be necessary).
Units : Selectables
    GameplayStats
    Graphics,audio
    Abilities
    Gameplay code -> movement, autoattack
Buildings : Selectables
    GameplayStats
    Graphics,audio
    Abilities
    Gameplay code -> production, rally points
//Not sure how much can be unified with unit into selectable yet. Perhaps Graphics, audio and abilities.

TBA Player and AI Player
    The core PlayerInterface should just send selected units to places.
    HumanPlayer needs UI and information is gathered through peripherals
    Basic AIplayer: 
        Predetermined attack patterns at certain time intervals
            ->But it wouldn't be able to micro
    Smart AIPlayer:
        Needs to be given a representation of the map and will be outputting commands based on the state of the game.
        Needs to have limited APM in some way to not be too good.
        In perfect world would have
            exact representation of the map each frame and recalculate strategy on frame by basis which would be incredibely performance intensive.

    Therefore I will have to have a Scripted (Basic) AIplayer that has some prescripted micro.
```
