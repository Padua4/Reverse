using System.Configuration;

namespace Reverse.Helpers
{
    /// <summary>
    /// Helper para obter connection string do App.config
    /// </summary>
    public static class ConnectionHelper
    {
        /// <summary>
        /// Obtém a connection string do ReverseDB do App.config
        /// </summary>
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        }
    }
}