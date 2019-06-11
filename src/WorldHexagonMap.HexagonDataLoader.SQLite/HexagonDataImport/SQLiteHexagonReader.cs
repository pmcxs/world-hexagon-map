using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using WorldHexagonMap.HexagonDataLoader.HexagonDataImporters;

namespace WorldHexagonMap.HexagonDataLoader.SQLite.HexagonDataImport
{

    public class SQLiteHexagonReader : IHexagonDataImporter
    {
        private readonly SQLiteConnection _connection;

        public SQLiteHexagonReader(string input)
        {
            _connection = new SQLiteConnection($"Data Source={input};Version=3;");
            _connection.Open();
        }

        public IEnumerable<object> Read(int minX, int maxX, int minY, int maxY, params string[] properties)
        {
            string query = "select d.U, d.V";
            query += string.Concat(properties.Select(p => $"\n,max(case when d.Field = '{p}' then d.VALUE end) {p}\n"));

            query += @"from HexagonData d
                inner join HexagonLocation l on l.U = d.U and l.V = d.V
	            where l.PixelX between @MinX and @MaxX and l.PixelY between @MinY and @MaxY
                group by d.U,d.V";

            IEnumerable<object> resultSet = _connection.Query<object>(query,
                new
                {
                    MinX = minX,
                    MaxX = maxX,
                    MinY = minY,
                    MaxY = maxY
                });

            return resultSet;
        }

        public IEnumerable<object> Read(params string[] properties)
        {
            string query = "select d.U, d.V";
            query += string.Concat(properties.Select(p => $"\n,max(case when d.Field = '{p}' then d.VALUE end) {p}\n"));

            query += @"from HexagonData d
                group by d.U,d.V";

            IEnumerable<object> resultSet = _connection.Query<object>(query);

            return resultSet;

        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
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