namespace XTG.DataBase.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public class DBLibrary
    {
        #region "internal functions"

        internal async Task<int> ExecuteNonQueryAsync(string connectionString, string query, List<SqlParameter> list, CommandType cmdType)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                PrepareCommand(cmd, conn, null, cmdType, query, list.ToArray());
                int val = await cmd.ExecuteNonQueryAsync();
                return val;
            }
        }

        internal async Task<DataSet<T>> ExecuteDataTableAsync<T>(string connectionString, string query, List<SqlParameter> list, CommandType cmdType)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                PrepareCommand(cmd, conn, null, cmdType, query, list.ToArray());
                DataSet<T> dataSet = new DataSet<T>();
                var cols = new List<string>();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    dataSet = new DataSet<T>(reader);
                }
                return dataSet;
            }
        }

        #endregion

        #region "Auxiliares"
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] commandParameters)
        {
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                foreach (SqlParameter p in commandParameters)
                {
                    if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                    {
                        p.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(p);
                }
            }
        }
        #endregion
    }
}
