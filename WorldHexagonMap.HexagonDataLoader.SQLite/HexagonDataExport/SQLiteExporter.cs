using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using Dapper;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Utils;
using WorldHexagonMap.HexagonDataLoader.HexagonDataExporters;

namespace WorldHexagonMap.HexagonDataLoader.SQLite.HexagonDataExport
{
    public class SQLiteExporter : IHexagonDataExporter
    {
        private readonly SQLiteTransaction _transaction;
        private readonly SQLiteConnection _connection;

        public SQLiteExporter(string output, bool overwrite = false)
        {
            if(overwrite)
            {
                SQLiteConnection.CreateFile(output);
            }
            
            _connection = new SQLiteConnection($"Data Source={output};Version=3;");
            
            _connection.Open();
            
            _transaction = _connection.BeginTransaction();
            
            _connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS [HexagonLocation] (
                    [U] INTEGER NOT NULL,
                    [V] INTEGER NOT NULL,
                    [PIXELX] NUMBER NOT NULL,
                    [PIXELY] NUMBER NOT NULL,
                    [CREATED] TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (U,V)
                )");
            
            _connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS [HexagonData] (
                    [U] INTEGER NOT NULL,
                    [V] INTEGER NOT NULL,
                    [FIELD] TEXT NOT NULL,
                    [VALUE] BLOB NOT NULL,
                    [CREATED] TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (U,V,FIELD)
                )");   
            
        }

        public async Task<bool> ExportResults(IEnumerable<Hexagon> hexagons, HexagonDefinition hexagonDefinition,  MergeStrategy mergeStrategy)
        {
            foreach (Hexagon hexagon in hexagons)
            {                
                string insertCommand = @"INSERT INTO HexagonLocation (U,V,PIXELX,PIXELY) VALUES (@U,@V,@PixelX,@PixelY) ON CONFLICT(U,V) DO UPDATE SET PixelX=excluded.PixelX,PixelY=excluded.PixelY";

                PointXY hexagonCenter = HexagonUtils.GetCenterPointXYOfHexagonLocationUV(hexagon.LocationUV, hexagonDefinition);
               
                await _connection.ExecuteAsync(insertCommand,
                    new
                    {
                        U = hexagon.LocationUV.U,
                        V = hexagon.LocationUV.V,
                        PixelX = hexagonCenter.X,
                        PixelY = hexagonCenter.Y
                    });
                
                foreach (var key in hexagon.HexagonData.Data.Keys)
                {
                    object dataValue = hexagon.HexagonData.Data[key];

                    string insertDataCommand = @"INSERT INTO HexagonData (U,V,FIELD,VALUE) VALUES (@U,@V,@Field,@Value) ON CONFLICT(U,V,FIELD) DO ";

                    switch (mergeStrategy)
                    {
                        case MergeStrategy.BitMask:
                            insertDataCommand += "UPDATE SET VALUE=VALUE|excluded.Value";
                            break;
                        case MergeStrategy.Ignore:
                            insertDataCommand += "NOTHING";
                            break;
                        case MergeStrategy.Replace:
                            insertDataCommand += "UPDATE Set Value=excluded.Value";
                            break;
                        case MergeStrategy.Max:
                            insertDataCommand += "UPDATE Set Value=max(excluded.Value,Value)";
                            break;
                        case MergeStrategy.Min:
                            insertDataCommand += "UPDATE Set Value=min(excluded.Value,Value)";
                            break;
                    }
                    
                    await _connection.ExecuteAsync(insertDataCommand,
                        new
                        {
                            U = hexagon.LocationUV.U,
                            V = hexagon.LocationUV.V,
                            Field = key,
                            Value = dataValue
                        });
                }
            }
            
            return true;
        }


        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _transaction?.Commit();
                _transaction?.Dispose();
                _connection?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}