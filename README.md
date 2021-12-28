# WitchHutSearch

This is a C# implementation of witch hut searching for Minecraft 1.18 in a given seed.
The tool should search any provided seed for double, triple, or quad witch huts within a specified range from origin (0,0).

This includes a partial port of [cubiomes](https://github.com/Cubitect/cubiomes), for the parts relevant for figuring out 
the biome at a given position and the location of a witch hut structure in a region. The port is constrained to the 
WitchHutSearch.Generator C# project, located in the 
[src/WitchHutSearch.Generator](https://github.com/totorewa/WitchHutSearch.NET/tree/main/src/WitchHutSearch.Generator) 
directory.