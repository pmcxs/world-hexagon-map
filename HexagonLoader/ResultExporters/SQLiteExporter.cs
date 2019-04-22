using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.ResultExporters
{
    public class SQLiteExporter : IResultExporter, IDisposable
    {
        private readonly SQLiteTransaction _transaction;
        private readonly SQLiteConnection _connection;

        public SQLiteExporter()
        {
            SQLiteConnection.CreateFile("./hexagondata.sqlite");
            
            _connection = new SQLiteConnection("Data Source=./hexagondata.sqlite;Version=3;");
            
            _connection.Open();
            
            _transaction = _connection.BeginTransaction();
            
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
            foreach (var hexagon in hexagons)
            {
                foreach (var key in hexagon.HexagonData.Data.Keys)
                {
                    object dataValue = hexagon.HexagonData.Data[key];

                    string insertCommand = @"INSERT INTO HexagonData (U,V,FIELD,VALUE) VALUES (@U,@V,@Field,@Value) ON CONFLICT(U,V,FIELD) DO ";

                    switch (mergeStrategy)
                    {
                        case MergeStrategy.BitMask:
                            insertCommand += $"UPDATE SET VALUE=VALUE|{dataValue}";
                            break;
                        case MergeStrategy.Ignore:
                            insertCommand += "NOTHING";
                            break;
                        case MergeStrategy.Replace:
                            insertCommand += "UPDATE Set Value=excluded.Value";
                            break;
                        case MergeStrategy.Max:
                            insertCommand += $"UPDATE Set Value=max(excluded.Value,{dataValue})";
                            break;
                        case MergeStrategy.Min:
                            insertCommand += $"UPDATE Set Value=min(excluded.Value,{dataValue})";
                            break;
                    }
                    
                    await _connection.ExecuteAsync(insertCommand,
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


        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
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