using System.Data;
using System.Data.SqlClient;

namespace SQLHelpers
{
    public static class SQLStandard
    {
        public static SqlConnection? Connection { get; set; }

        private static SqlCommand? SqlCommand { get; set; }

        public static void ExecuteQuery(string query, List<(string name, object value)>? paramaters = null, int timeout = 300)
        {
            InitExec(timeout);

            using var command = new SqlCommand(query, Connection)
            {
                CommandTimeout = timeout
            };

            try
            {
                if (paramaters != null)
                {
                    command.AddParameters(paramaters);
                }

                command.ExecuteNonQuery();
            }
            catch (Exception ex) 
            {
                throw new Exception($"Failed to execute query: {query} {ex}");
            }
        }

        private static void InitExec(int timeout = 300)
        {
            if (Connection == null)
            {
                throw new Exception("SQLConnection not initialized!");
            }

            if (Connection.State != System.Data.ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        public static DataTable ExecuteQueryWithResults(string query, List<(string name, object value)>? paramaters = null, int timeout = 300)
        {
            DataTable dataTable = new();

            InitExec(timeout);

            using var command = new SqlCommand(query, Connection)
            {
                CommandTimeout = timeout
            };

            try
            {
                if (paramaters != null)
                {
                    command.AddParameters(paramaters);
                }

                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to execute query: {query} {ex}");
            }

            return dataTable;
        }

        private static void AddParameters(this SqlCommand command, List<(string name, object value)> parameters)
        {
            if (parameters == null)
            {
                return;
            }

            foreach (var (name, value) in parameters)
            {
                command.Parameters.AddWithValue(name, value);
            }
        }
    }
}