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
