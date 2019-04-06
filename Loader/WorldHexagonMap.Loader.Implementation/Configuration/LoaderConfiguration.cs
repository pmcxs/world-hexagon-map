using System;
using System.Configuration;
using WorldHexagonMap.Loader.Domain.Configuration;

namespace WorldHexagonMap.Loader.Implementation.Configuration
{
    public class LoaderConfiguration : ConfigurationSection, ILoaderConfiguration
    {
        /// <summary>
        /// Defines the number of threads that can be executed in parallel during the loading process
        /// </summary>
        [ConfigurationProperty("parallelism", DefaultValue = 4, IsRequired = false)]
        public int Parallelism
        {
            get { return Convert.ToInt32(this["parallelism"]); }
            set
            {
                this["parallelism"] = value;
            }
        }


        [ConfigurationProperty("hexagonrepository", DefaultValue = "inproc", IsRequired = false)]
        public string HexagonRepository
        {
            get { return Convert.ToString(this["hexagonrepository"]); }
            set
            {
                this["hexagonrepository"] = value;
            }
        }


        public static LoaderConfiguration GetConfiguration()
        {
            return (LoaderConfiguration) ConfigurationManager.GetSection("loader");
        }
    }
}
