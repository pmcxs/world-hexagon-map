## Description
This command converts hexagon data (typically obtained through the command 'geo2hexagon') to usable map data: geojson, vector/raster tiles

## Parameters

- Verbose (`--verbose` or `-v`).
Will produce aditional output, typically useful for debugging purposes
- Tile (`--tile`)
Tile in the format z:x:y. Ex: `--tile 10:343:343`. Supplying this parameter overrides the boundary parameters (n,s,e,w), thus making sure that only a single tile is generated
- HexagonSize (`--hexagonsize` or `-h`). Default value: `10`
The size of the hexagon side on the reference zoom. Total size = 256 << referencezoom
- HexagonReferenceZoom (`--referencezoom`). Default value: `10`

## Examples

Example 1. Generating an unbounded map (as in, picks up whatever exists on the sqlite input) and generates geojson data

On this example, the input file "hexagons.sqlite" is processed so that a corresponding geojson file is screated. The generated data will include the mandatory u,v properties, but also including the speficied "country" property

```hexagon2map --input hexagons.sqlite --hexagonsize 500 --dataproperties country --outputtype Geojson --output hexagons.geojson"```

