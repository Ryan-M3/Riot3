# Riot3

This repo contains the files for a Unity video game I made to learn C# and demonstrate my programming skills. It requires Unity to compile and I can't guarantee that Github recognizes every little thing required to make it compile without a little tweaking on your computer (you can ask for a compiled binary if you'd like, though). It's a turn-based strategy game about putting down a riot. I'll add more detail about navigating this repo soon.

# Folders

## Audio

Nothing important here.

## Board

As a turn-based strategy game, the playing field is envisioned as a board with distinct grid tiles. Path-finding is probably the most optimized thing I've coded in the entire project. Efficiency was very important to me because the shortest path from a player to a grid tile follows the mouse during the player's turn. That is, the shortest path theoretically has to be calculated 60 times per second.

First, the Floyd-Warshall Algorithm is used to precompute the distance between every two grid positions taking into account all static objects in the scene. This way, we can effectively use all the information we know about the scene at compile-time by pre-computing (baking) path-finding information. However, there may still be dynamic objects in the scene during run-time. A* Search is then used to path-find around dynamically placed objects should they be placed in the path of what would otherwise be the shortest path. Finally, because each unit can only move so many tiles according their action points and a player may be expected to mouse-over a variety of tiles in their viscinity repeatedly, contemplating their next action, I've also memoized the result of each path-finding query; the cache is reset each time the player ends his or her turn.

#### A_Star

Implementation of A* Search algorithm. This is used in conjunction with the Floyd Warshall algorithm as the basis for the "heuristic distance" used in A*.

#### Board

The Board class acts as a facade design pattern. It provides a single point of access to querying where objects are on the board, for placing or moving objects on the board, and for storing general information about the board. Although there is only in fact one playable level, I wrote the code to accommodate multiple levels; each level would have it's own board which may differ in size and object placement.

#### Distances

Deprecated. Included only because it contains some code on implementing the Gift-Wrapping algorithm for finding the convex hull of a set of points. It was depricated in favor of drawing the actual path to where the mouse is pointing rather than an outline around the player.

#### FlowObj

In researching how rioters behave, I came across several papers that attempted to describe crowd or "egress" situations as a fluid flow. As such, the board contains a hidden (to the player) vector field, the direction of which can be set during the level design phase. A flow object is an object that is traversible (a rioter or police officer can walk on it) that has an associated direction of crowd flow. For instance, protestors will tend to walk down roads, but around "repulsive" objects like tear gas canisters and particularly threatening police officers.

#### FlowdWarshallBaker

On large maps, Floyd-Warshall became annoyingly long to calculate. To get around this, I implemented a multi-threaded version which greatly sped up calculation times. Although I tried to remember to cite any papers I followed in my code to clearly delineate what's my code and what's from someone else, it looks like I forgot to put a citation here. I believe this is the sourrce I used to implement this: https://gkaracha.github.io/papers/floyd-warshall.pdf

#### GridObj

A grid object is part of the scene or level. It automatically registers itself with the relevant Board object and tells it whether it represents a tile that is traversible, where it is, and other basic information.


#### Point

Unity doesn't throw a type error when a Vector2 is passed where a Vector3 is expected. The result is that refactoring code or accidentally writing Vector2 instead of Vector3 results in code that appears to work just fine, until several weeks later when you're not getting the expected behavior you want, but without throwing any errors or indications that something went wrong. So I rewrote the Vector2 class as Point, which I generally used whenever I was refering to grid coordinates.

## Data Libs

I originally created Data Libs to solve a problem encountered when allowing the player to equip their character with different clothing and weapon options. I can write to a text file that the player has selected weapon number 4, but that doesn't tell me which mesh to load to represent that weapon, or that the weapon does 5 damage. A Data Lib is a game asset that holds references to other games assets which can be accessed during run-time by providing the correct ID for it, as well as providing general data about that particular item.

#### Armor

Clothing assets (many provide zero damage resistence and probably shouldn't be called armor).

#### Equippables IconLib

Provides references to 2D images representing a given weapon.

#### Lib

The actual code implementing DataLibs.

#### MeshPresets

Game meshes as saved as .FBX files and the materials (color, shinnines, etc.) can vary for the same mesh. Doing so might also change the statts that object has. I call the actual geometry data the mesh, and everything else about it the mesh preset.

#### Units

Default police and rioter units.

#### Wave Collapse

Inspired by a "wave collapse" algorithm for procedural, the implementation should probably be called "Constraint-Based Stochastic Clothing Generation," but that's too long of a name. Basically, I wanted to create randomly-generated rioters. However, I also wanted to be able to list rules about what makes a good selection of mesh presets. The rules for generating plausible outfits are found in Wave Collapse/Constraints and have self-explanatory names ("Men Wear Men's Clothes.asset" is a rule that says if any equipped piece of clothing is marked as "male only," then no other article of clothing may be marked as "female only."). I was surprised how well just a few rules wound up creating reasonable-looking NPCs.

## Data Structures

Contains very simple, general data structures used throughout the project.

#### Edges
An edge is a struct containing two points.

#### KDTree

A k-Dimensional Tree. Also contains a "PtTree" which is just a special case of a KDTree that uses my Point class, specifically for returning and working with grid coordinates. Mostly used so I can do nearest-neighbor search to find nearby board pieces. That is, so I write code that does something like "if there is a weapon within 2 tiles of the rioter, have the rioter pick it up with a probability of 10%."

#### Object Pooling

This bit of code really isn't in this particular iteration of the project. In a prior version of the game, it was real-time and there could be many objects being spawned at once, especially sound files. Additionally, the board was loaded in pieces. But basically what it does is it instantiates objects over time, then hides them. Rather than create the object when needed, a "Swimmer" (an object which wants to use an object pool) borrows that object rather than instatiate it, then "returns" it instead of destroys it. That object must have the "Poolable" script attached to it. Additionally, the rate at which various object pools  instatiate their poolable object behind the scenes is called the "WaterPark" (because it's a collection of pools).

This was important when there were a lot of sound effects because the sound effect has a spatial location which it will occupy. So if you have 10 characters in the scene and each one has their own footstep sound, you can easily wind up having to instantiate a dozens of footstep sounds into the scene every second. With object pooling, I was able to cycle through just three or four and mostly just moving them and replaying the audio.

#### ProbabilityQueue

This is apparently called the Roulette Algorithm and apparently everyone implements it incorrectly. The idea is to draw an item from a list of items such that 30% of the time you get item A, 20% of the time you get item B, etc. Most people create an array of cumulative probabilities, generatte a random number, then iterate through that array until they find the appropriate probability, then use the index of that probability to map back to the item they should return. A naive implementation is O(n) or O(log n), whereas the implementation I used is supposedly O(1). Honestly, it probably didn't matter performance-wise and my benchmarking indicated that you needed a lot of items in your queue for the optimized version to actually noticable outperform the naive version, but it seemed easy enough to implement and the code is re-usable, so why not?

#### ReversibleDict

A dictionary that's smart enough to know that if you're mapping, say, an integer to a string then myDict[myInt] should return the associated string and myDict[myString] should return the associated integer.

#### SparseMatrix

A matrix implemented with a dictionary that assumes any values not in that dictionary have a value of zero.

#### Vector4Int

A 4-dimensional vector composed of integers.

#### VectorField

At it's core it's just a matrix of vectors. However, it's used to model crowd behavior as a fluid flow. So it also includes FieldVisualizer to display in the Unity editor/engine a bunch of arrows in the scene indicating where a crowd wants to go. It also lets you smooth a vector field, to add or subtract a vector field to it, all sort of fun stuff like that.

## Dev Tools

#### Cracks

I didn't actually finish this one, but I did prototype it in Haskell elsewhere and found it to be lacking. It was going to procedurally generate cracks.

#### Edit Mode Ctrl

Junk file. I must have missed it when I uploaded this project to github. Sorry!

#### Procedural Fire

I was unsatisfied with standard implementations of fire in games that used particle effects. Instead I actually wrote a script that manipulated the points on the mesh (the tris) itself. Then I added some flickering and layerinig. I thought it came out pretty well all things considered and fit with the art style.

## Editor

This is a special file used in Unity that modifies the behavior of the Unity Editor. These are basically workflow enhancements and honestly aren't very important or interesting.

## Global State Adaptors

This folder is kind of a mish-mash of design patterns.

First, let's talk about the concept of a Modal Object. This evolved out of a frustration using C#s event system. Generally the code didn't look very pretty, it was spread across a bunch of different functions, and I had to explicitly think about what other objects are doing, while making it annoying to modify and refactor. "Global state" is a four-letter word in programming, but that's the true, underlying reality of the game. There is indeed a state and objects and code need to react to that state. So I took inspiration from my favorite text editor, Vim, and decided that the game needed to have "modes" and the player would switch between modes. I then implemented this in a way that would be familiar to anyone who codes with Unity: just like how a MonoBehaviour has it's own Update function, FixedUpdate, and late Update which do the same thing, but have different properties, I would have various Modal Updates. These are updates that are only active when the game is in that mode.

Any object which might behave differently in different game modes (which happen to be Deploy Troops, Player Turn, Enemy Turn, Paused, and Level Over), inherits from the ModalObject class which allows that object to override methods that are called each update in each game mode, the first time you enter that game mode, and when you exit that game mode.

I found this so grreatly simplified and cleaned up my code, I created a "ViewModalObject" class which is an extension of game state into UI state. So a player might select a unit to move and then click "move" which triggers an event setting the ViewMode to "moving." Various UI behaviors, such as path-finding and highlighting then start updating. For instance, a line must be drawn from the player to the mouse. The code for drawing this line is contained in the "MoveUpdate" method and it is automatically turned on and off as needed.

This allows me to write lots of behaviors in a decentralized way. If I need a script to control changing the lights on a traffic light, all I have to do is inherit from ViewModalObject and put the relevant code under the correct Update function.

This folder also contains the basics of a Model-view-Control architecture. I found it useful to segregate code representing the underyling state of the game with how it's presented to the user. What's notable is that I used the Command design pattern for the Control division of the code. This allows players to undo actions. But when combined with MVC it meant that I could do things like make all the rioters do their turn at the same time, and let the code for animating and physically moving them on the board do it's own thing independently. Not only does this mean the player doesn't have to sit and watch each rioter go through their own motions (which can be an issue if you want 100 rioters), but by moving at once it looks much more like a crowd.

## Materials

Materials are Unity assets that encapsulate the textures, shaders, colors, and other settings used for an object. I followed a somewhat strange naming convention. I made a script that prepended each material with three numbers between 0 and 8 representing the hue, saturation, and luminance of the color of each material. This way, if you sort by name in the editor colors that are similar to each other are actually near each other and you can just scroll down to the navy blue materials and pick one.

## Misc

Things that don't fit anywhere else and don't even seem to be very important or interesting.

## Not My Code

Things I didn't write and wanted to make sure I didn't claim I wrote. Sometimes you just don't feel like implementing everything, even in a project meant to demonstrate that you know how to program.

#### Catenary

I started out trying to make this on my own, but then when people started talking about transcendental numbers and writing custom meshes (which I later learned how to do to write procedural fire), I just grabbed a library from github.

#### Gaussian Number

All I did was translated some C++ code on wikipedia into some C# code. I don't really understand why it works, I just want a normally distributed number sometimes.

#### PriorityQueues

Grabbed off of github.

#### Shuffle

Shuffles an array. It's short and simple, but I did steal it from a stackoverflow answer.

#### Singleton

The implementation is slightly different when you're using Unity's MonoBehaviours rather than just straight C# code.


## Not My Art Assets

Art assets I grabbed elsewhere.

## Object Abstractions

Certain character stats like action point and health points needed their own logic, so I broke them out into components (Unity uses an Entity-Component system rather than a deeply nested hierarchy more typical in OOP). For instance, action points need to refresh every turn.

## Plugins

This is a special folder in Unity for using DLLs. I used SQLiter which allowed me to use a SQL database to create the save file. I did not code SQLiter myself, but I also can't put it in the "Not My Code" folder (it only works if you put it in Plugins).

## Post-Processing

This is a folder Unity uses to do post-processing effects. I did not write this either.

## Powerups

Contains logic and assets for having items protestors can pick up and use. It only actually has a brick powerup, but in prior stuff I've coded these can get rather baroque and complicated.

## Save File

The logic requireed to save and load data from the save file.

## Scene Models and Objects

These are the files for the 3d meshses used in the game, as well as Unity prefabs (stock objects you can create and drop into the game). I made it all in Blender, which I've become quite fond of, but none of it's really got anything to do with programming.

## Scenes

These are what Unity loads to let you play the game: the splash screen, the headquarters for upgrading units, the actual level with rioters, and some testing and debug scenes.

## Streaming Assets

This is a file used by Unity save and load data and only contains baked distance data (see: Board). It is incomplete as some files were too big for github.

## UI Stuff

Since I roughyly used a model-view-controller architecture, this whole folder can be considered the View part of that.

#### Button Maker

It's a pain to set-up the UI every single time you make a change so I decided to generate the UI somewhat dynamically during run-time. 

#### Camera

Logic regarding how the main camera is controlled and operates.

###### DressupCamera

This is the camera that is focused on a character when you're editing their armor and stats. It mostly just rotates around the character.

###### Move Camera

Provides the basic functionality of moving the camera: taking keyboard inputs and performing rotation, translation, and zoom.

###### Toggle Layers

This script controls what "layers" are actually rendered by the camera. The grid overlay of the board is on its own layer and you can toggle it on and off to your liking.

#### Cursor

This moves the in-game cursor.

#### Dancing Ants

This is an effect that was strangely a real pain to implement that draws a dotted line around one or more tiles to show that it is in some way highlighted or selected.

#### Draggable Icons

Allows you to drag an icon from one place to another. The code is mostly from the official Unity documentation on dragging events.

#### Draggable Unit Icons

I wanted to keep the UI as uncluttered as possible and I took inspiration from a talk I watched where the UI designer said he wanted all of the UI elements to be in-game, where the player was looking rather than in the HUD or as an icon in the top right corner or whatever it was. So my idea was that you would have an icon representing your character and you would drag that icon to the appropriate spot on the screen to do stuff.

For instance, before each mission (there's only one because this is for demonstration purposes) icons representing units are loaded up on the left side of the screen. If you drag that icon to the middle of the screen, it transforms into the actual character mesh and you can edit your unit by upgrading armor and equipping weapons. Then, you can drag that character to the right of the screen labeled as "on mission" to actually send him on the mission. In this way, you're physically moving him to the mission.

Additionally, during the mission when it's time to position units on the ground, you drag the icon onto the scene, where the unit transforms from an icon into a diegetic character model.

I called this behavior a "chimera" because the object can transform between an icon representation and a physical character model representation depending on where you've dragged the unit to.

#### Equippable Icons

This controls thee behavior of icons representing weapons and armor that you can have your units equip. For instance, it looks up and calculates if you have enough of this item on-hand and overlays a lock on the icon.

#### Fonts

Just some fonts.

#### Global UI Trackers

I remember reading about the psychology of grouping items together and that one thing the brain pays attention to is if two different things change at the same time. So my idea was that any non-diegetic UI elements would slowly drift in hue over time, but because all UI elements are similarly drifting in color, that tells you it's a UI element. I like the way it looked, so all manner of objects now track a global UI color (whatever it is at that precise moment). This includes text, lights, materials, you name it.

#### Grid Overlay

The game is grid-based, so to implement a grid overlay I wound up creating a transparent plane with a grid on it. Then, the plane followed the camera. Additionally, I had to make the plane larger whenever the camera zoomed out. That also meant dynamically chaning the underlying texture so that the grid didn't get stretched out.

#### Home Base UI

The only programming thing that's actually in here is logic for displaying and using cash to buy equipment and upgrades.

#### Level Complete Screen

A not very complicated script for calling level over. However, my game doesn't actually do anything interesting when the level is complete so that's why it's not very interesting. You can imagine that displaying a score with a count-down and some statistics, plus adding achievements or whatever could make this complicated. But I just didn't happen to do anything interesting with that.

#### Misc

Things that don't fit anywhere else. Contains logic for creating a new unit in the pre-mission screen, and a script for dynamically creating the buttons for the splash screen (see: UI Stuff/Button Maker).

#### MsgQ

Responsible for logging messages to the player on screen.

#### Other Artwork

Nothing to do with coding here.

#### Path Drawer

Drawing a path from the player to the mouse is actually done in two parts. The first draws the actual line. The second shows an icon with some text in the center of it that gives additional information. Player actions are actually modal (see: Global State Adaptors) so when you press "move" or "attack" or something like that, you actually enter "move mode" or "attack mode" and the options displayed to you are different. However, I tried to make these options as diegetic as possible (see: Draggable Unit Icons). So the idea is to have the mouse or whatever's on screen actually change rather than some button in a corner.

For movement, the polygon at the end of the path is a circle and the number inside the circle indicates how many action points it will cost. For attack mode, you have a pentagon and the damage is displayed. If your weapon doesn't have enough range to reach where you're pointing, it simply doesn't draw anything there.

#### Turn Visualizer

During "turn mode" there's an arrow pointing and rotating either clockwise or counter-clockwise around your unit depending on which direction you want to turn your unit. See: path drawer.

#### UI Object Manipulation

A couple of utility scripts for manipulating UI.

###### ActiveDuringViewMode

Turns an UI element or script on or off depending on what viewmode is selected. See: Global State Adaptors

###### Close Group

Turns on/off a bunch of different UI elements at oncee.

###### GluiUItoWorldObj

Hovers a UI element "over" a diegetic scene object. This basically just means translating between scene space and world space.

###### InactiveIfNoChildren

This is used when moving unit icons across the screen. See Draggable Unit Icons.

###### RenderBehind

Renders one UI element behind another.

###### TrackCamYLookAtCursor

I honestly couldn't come up with a better name for what this script does. Suppose I have a plane or object that I always want to face the camera, but I also want it rotated towards the mouse, like an arrow that always points to the mouse. That's what this script does.

## Units

#### Animations

The only script here just slects an animation at random out of a list of animations to play. Animations in this context mean the movement of characters on the screen, rather than a cartoon.

#### Basic Scripts

###### CanAttackCar

Rioters can band together to flip over a car. This script prrovides that functionality.

###### CharSheet

I envisioned each unit as having a sort of D&D character sheet. This makes saving and loading a unit from a save file easy to deal with as you're basically just saving the character sheet.

###### EnemyCtrl

While each individual unit is controlled independently boids-style, there's still a need to control all units in the same way that the player controls each of his. As an example, no individual rioter can end the enemy turn, only the EnemyCtrl can, but what each enemy does for its turn is controlled by that RioterAI, not the EnemyCtrl. EnemyCtrl is the imagined enemy player in the seat and RioterAI is the unit on the board.

###### GettingArrestedStatus

I implemented the process of being arrested as a status effect. I decided that it made the most sense to arrest a rioter on the condition that they don't have enough action points to resist arrest. Further, I decided that the non-lethal force used by riot police should primarily deplete a rioter's ability to resist arrest. Actually depleting their health points and killing them is something the player should try to avoid.

###### MeshSetter

Componenet that enables you to change the mesh (clothing) on the rig of you character model.

###### MeshSetterWaveFn

Junk file. The actual code is now located in Data Libs.

###### Party

This is the concept of your "party" in a traditional RPG setting. The rioters are on party and the riot police are the other party.

###### RioterAI

Controls the behavior or the rioters.

###### SceneLoader

###### Unit

#### Chimera, HQ Unit

#### Chimera, Mission Units

#### Command Queue

#### FBX - Unit Classes

#### Misc

#### Names 

#### Skin Tones

#### StatusFx

#### Unit Class Prefabs
