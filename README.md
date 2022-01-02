# WitchHutSearch

This is a C# implementation of witch hut searching for Minecraft 1.18 in a given seed.
The tool searchs any provided seed for double, triple, or quad witch huts within a specified range from origin (0,0).

## Usage

```cmd
USAGE
  WitchHutSearch.exe <huts> --seed <value> [options]

PARAMETERS
* huts              Number of huts.

OPTIONS
* -s|--seed         Seed to search on.
  -b|--blocks       Number of blocks to search in each direction. Default: "128000".
  -t|--threads      Number of threads to search with. Default: # of cores x2.
  -h|--help         Shows help text.
  --version         Shows version information.
```

## Disclaimers

This includes a partial port of [cubiomes](https://github.com/Cubitect/cubiomes), for the parts relevant for figuring out 
the biome at a given position and the location of a witch hut structure in a region. The port is constrained to the 
WitchHutSearch.Generator C# project, located in the 
[src/WitchHutSearch.Generator](https://github.com/totorewa/WitchHutSearch.NET/tree/main/src/WitchHutSearch.Generator) 
directory.