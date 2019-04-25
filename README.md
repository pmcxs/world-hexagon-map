# world-hexagon-map
This project aims to provide a representation of the whole world into small hexagons.

The idea is to include various features (like land, water, desert, snow, mountains, forests, roads, ralways, etc), including all the pipeline to generate it, from the parsing of the Geographical data to its consequent representation on a map


## HexagonDataLoader
Loads data from geofiles (shapefiles, geojson, geotiff) and generates hexagon data

## MapTile Generator
Reads hexagon data info and exportes the corresponding map tiles

