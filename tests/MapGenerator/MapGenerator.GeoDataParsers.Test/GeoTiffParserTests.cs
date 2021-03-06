﻿using MapGenerator.Core.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace MapGenerator.GeoDataParsers.Test
{
    public class GeoTiffLoaderTests
    {
        private readonly List<GeoData> _geoData;

        public GeoTiffLoaderTests()
        {
            var loader = new GeoTiffParser();

            _geoData = loader.ParseGeodataFromSource(
                    new LayersConfiguration { Interpolate = true, Source = Path.Combine("Resources", "sample.tif") })
                .ToList();
        }

        /// <summary>
        ///     Validating that a sample file of 660x660
        ///     is parsed successfully.
        ///     The values below were obtained manually using a Graphics SW
        ///     (RGB value)
        /// </summary>
        [Theory]
        [InlineData(0, 36)] //First pixel of first line
        [InlineData(659, 12)] //Last pixel of first line
        [InlineData(660, 36)] //Second pixel of second line
        [InlineData(435599, 19)] //Last pixel of last line
        public void ParseGeoTiff_Success(int offset, byte value)
        {
            Assert.Equal(value, _geoData[offset].Values["custom_1"]);
        }
    }
}