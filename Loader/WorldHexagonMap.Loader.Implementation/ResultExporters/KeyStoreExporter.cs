﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WorldHexagonMap.Core.Contracts;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Loader.Contracts;
using WorldHexagonMap.Loader.Domain.Configuration;

namespace WorldHexagonMap.Loader.Implementation.ResultExporters
{
    public class KeyStoreExporter : IResultExporter
    {
        private readonly IDataStoreService _storeService;

        public KeyStoreExporter(IDataStoreService storeService)
        {
            _storeService = storeService;
        }

        public async Task<bool> ExportResults(IEnumerable<Hexagon> hexagons)
        {
            Console.WriteLine(DateTime.Now + ": Pushing to Key Store");

            foreach (Hexagon hexagon in hexagons)
            {
                var key = "hex:data:" + hexagon.LocationUV.U + ":" + hexagon.LocationUV.V;
                var properties = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("u", hexagon.LocationUV.U.ToString(CultureInfo.InvariantCulture)),
                    new KeyValuePair<string, string>("v", hexagon.LocationUV.V.ToString(CultureInfo.InvariantCulture)),
                };

                properties.AddRange(
                    hexagon.HexagonData.Data.Keys.Select(
                        dataField => new KeyValuePair<string, string>(dataField, hexagon.HexagonData.Data[dataField].ToString())));

                await _storeService.SetPropertiesAsync(key, properties.ToArray());
            }

            return true;
        }

        public bool MergeSupported
        {
            get { return false; }
        }
    }
}