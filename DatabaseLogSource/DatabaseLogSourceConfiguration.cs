using LogAnalyzer.API.LogSource;

namespace DatabaseLogSource.Editor
{
    public class DatabaseLogSourceConfiguration : ILogSourceConfiguration
    {
        public DatabaseLogSourceConfiguration(string connectionString, string query)
        {
            ConnectionString = connectionString;
            Query = query;
        }

        public string ConnectionString { get; }
        public string Query { get; }
    }
}