# WitchHutSearch

This is a C# implementation of witch hut searching for Minecraft 1.18 in a given seed.
The tool searchs any provided seed for double, triple, or quad witch huts within a specified range from origin (0,0).

_This was a project that I wanted to try out for fun and will likely not maintain so have no expectations of it being updated for any further Minecraft generation changes unless I happen to feel up to it or need it myself._

## Usage

```cmd
USAGE
  WitchHutSearch.exe <huts> --seed <value> [options]

PARAMETERS
* huts              Number of huts.

OPTIONS
* -s|--seed         Seed to search on.
  -b|--blocks       Number of blocks to search in each direction. Default: "128000".
  -t|--threads      Number of threads to search with. Default: 4.
  -o|--out          Output file for writing locations to.
```

Example: `WitchHutSearch.exe 3 -s 123456 -b 1000000 -o output.txt`  
This will search for triple witch huts within a million block radius on the seed `123456` and write the results to `output.txt`.

You can specify a CSV file as the output to have it written in CSV format. All other file types are written as text.  

Sometimes false positives may be returned because something prevented one or more of the huts from generating in the world.

## Disclaimers

This includes a partial port of [cubiomes](https://github.com/Cubitect/cubiomes), for the parts relevant for figuring out 
the biome at a given position and the location of a witch hut structure in a region. The port is constrained to the 
WitchHutSearch.Generator C# project, located in the 
[src/WitchHutSearch.Generator](https://github.com/totorewa/WitchHutSearch.NET/tree/main/src/WitchHutSearch.Generator) 
directory.
