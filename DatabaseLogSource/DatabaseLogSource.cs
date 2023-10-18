using DatabaseLogSource.Editor;
using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLogSource
{
    internal class DatabaseLogSource : ILogSource
    {
        private DatabaseLogSourceConfiguration configuration;
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader reader;
        private List<string> columns;

        private string Escape(string value)
        {
            if (value == null)
                return null;

            return value.Replace("\"", "\\\"");
        }

        public DatabaseLogSource(ILogSourceConfiguration configuration)
        {
            this.configuration = (DatabaseLogSourceConfiguration)configuration;

            connection = new SqlConnection(this.configuration.ConnectionString);
            connection.Open();
            command = new SqlCommand(this.configuration.Query, connection);
            reader = command.ExecuteReader();

            columns = new List<string>();

            for (int i = 0; i < reader.FieldCount; i++)
                columns.Add(reader.GetName(i));
        }

        public void Dispose()
        {
            reader.Close();
            command.Dispose();
            connection.Close();
            connection.Dispose();
        }

        public string GetLine()
        {
            if (!reader.Read())
                return null;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (i > 0)
                    sb.Append(",");

                if (columns != null && i < columns.Count)
                    sb.Append(columns[i]);
                else
                    sb.Append($"Field{i}");

                sb.Append(":");

                string value = reader[i].ToString();

                sb.Append($"\"{Escape(value)}\"");
            }

            return sb.ToString();
        }

        public string GetTitle()
        {
            return "Database";
        }
    }
}
