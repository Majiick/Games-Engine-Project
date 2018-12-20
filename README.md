# Assignment Description
The basic premise of the project is to simulates the construction of roads on a certain terrain to see how they develop and hopefully will be efficient. The terrain is randomly generated using perlin noise and is split into 3 levels - land, water and mountain.

The roads are generated using a teleological approach via simulating villager agents. The villagers spawn in either one of two villages on the map which are randomly placed. The villagers collect food and water and bring it back to their village, the trees and food spawn randomly around the map. When the villagers walk on land they trample the ground beneath them. When the ground is trampled it makes it easier for other villagers to walk on, and using the A* path finding they're more likely to choose the trampled road than the raw land. Fully developed roads cost 90% less to walk on than normal land. Villagers can also swim and cross mountains, but it costs them 100 times more than walking on normal land. The A* algorithm also takes into consideration the height difference between tiles when choosing a path.

The trees are procedurally generated with also a randomly chosen tri-color scheme.

The terrain is drawn using the Terrain Unity object. It is colored by generating a texture and setting the texture on the terrain object.

# Which parts I did myself
Everything in the assignment is my work . The only part that I got from a Youtube tutorial is the base Perlin Noise terrain generation. However, it was only the very basics of Perlin noise, and I added on octaves and smoothing myself.



# What I am proud about
I am proud about optimizing the A* algorithm and proud of how the aesthetic of the terrain generation. The terrain looks nice and even better with roads on it.

 I have written the A* before many times but it was only for small grids less than 25x25 and therefore I only used a list for everything. However with this big grid it was taking a very long time to generate some paths so I looked at the code and picked out the best fitting data structures myself and replaced the list with a sorted list and a hash map and the performance is phenomenal now.

I am also proud that the roads actually do make sense, there is usually a big highway down the middle of the map and 3 perpendicular more little highways on the map depending on the map generation. You can see examples of this in the Youtube video.

# Building and Running
Works on Unity 2017.3.0f3 64-bit. Might not work on other versions of Unity. This is because the Terrain built-in asset changed.

(Please mute audio, just a lot of background noise.)
[![YouTube](https://youtu.be/PoNaX-FmW_4)](https://youtu.be/PoNaX-FmW_4)