using System.Data.SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.ResultExporters
{
    public class SQLiteExporter : IResultExporter
    {
        public async Task<bool> ExportResults(IEnumerable<Hexagon> hexagons)
        {
            SQLiteConnection.CreateFile("./hexagondata.sqlite");
            
            using (var sqLiteConnection = new SQLiteConnection("Data Source=./hexagondata.sqlite;Version=3;"))
            {
                sqLiteConnection.Open();
                
                var sqLiteTransaction = sqLiteConnection.BeginTransaction();

                await sqLiteConnection.ExecuteAsync(@"
                
                    CREATE TABLE IF NOT EXISTS [HexagonData] (
                        [U] INTEGER NOT NULL,
                        [V] INTEGER NOT NULL,
                        [LAND] INTEGER NULL,
                        [FOREST] INTEGER NULL,
                        [WATER] INTEGER NULL,
                        [URBAN] INTEGER NULL,
                        [ALTITUDE] INTEGER NULL,
                        [CREATED] TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        PRIMARY KEY (U, V)
                )");                
                
                foreach (var hexagon in hexagons)
                {
                    await sqLiteConnection.ExecuteAsync(@"INSERT INTO HexagonData (U,V,LAND,FOREST,WATER,URBAN,ALTITUDE) VALUES (@U,@V,@LAND,@FOREST,@WATER,@URBAN,@ALTITUDE)",
                        new
                        {
                            U = hexagon.LocationUV.U,
                            V = hexagon.LocationUV.V,
                            Land = hexagon.HexagonData.Land,
                            Forest = hexagon.HexagonData.Forest,
                            Water = hexagon.HexagonData.Water,
                            Urban = hexagon.HexagonData.Urban,
                            Altitude = hexagon.HexagonData.Altitude
                        });
                }
                
                sqLiteTransaction.Commit();
              
            }

            return true;
        }


    }
}