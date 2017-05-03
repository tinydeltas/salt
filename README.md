# salt

"traveler, taste the salt lingering in your memories; taste your sister's sand-crusted skin, your mother's dumplings, your past self, waiting to be born."

an unconventional survival game set in a procedurally-generated, post-apocalyptic world.

senior project for lining wang // advisor: holly rushmeier

## code

Located in the Assets folder.

* `_Lib/` library functions  usable in a procedural generation context
    * `_GradientNoise`: implementations of voronoi, perlin, value, and exponential noise; incomplete implementation of diamond-square noise
    * `_Textures` procedural texture generation.
    * `_Mesh` mesh utility functions, such as triangle calculation; mask implementations
    * `_Terrain` defines a generic terrain
    * `_Util` math utilities and others
* `_Opt/` async optimization module
* `_Material/` used for procedural generation of materials
* `_IslandGen/` scripts that use libraries to render islands
	* `Pipeline/` facilitates user exploration, using the `Nav` script

## usage

`git clone https://github.com/linii/salt`

## licensing

MIT License:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

## contact

* lining.wang@yale.edu
