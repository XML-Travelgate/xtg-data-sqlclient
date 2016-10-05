using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace XTG.Data.SqlClient
{
    public class DBLibrary
    {
        #region "internal functions"

        internal async Task<int> ExecuteNonQueryAsync(string connectionString, string query, List<SqlParameter> list, CommandType cmdType)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                PrepareCommand(cmd, conn, null, cmdType, query, list.ToArray());
                int val = await cmd.ExecuteNonQueryAsync();
                cmd.Parameters.Clear();
                return val;
            }
        }

        internal async Task<IEnumerable<Dictionary<string, T>>> ExecuteDataTableAsync<T>(string connectionString, string query, List<SqlParameter> list, CommandType cmdType)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                PrepareCommand(cmd, conn, null, cmdType, query, list.ToArray());
                SqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return Serialize<T>(reader);
            }
        }

        #endregion

        #region "Auxiliars"
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] commandParameters)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = cmdType;
            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(cmd, commandParameters);
            }
        }

        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            foreach (SqlParameter p in commandParameters)
            {
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }

                command.Parameters.Add(p);
            }
        }

        private IEnumerable<Dictionary<string, T>> Serialize<T>(SqlDataReader reader)
        {
            var results = new List<Dictionary<string, T>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
                results.Add(SerializeRow<T>(cols, reader));

            return results;
        }

        private Dictionary<string, T> SerializeRow<T>(IEnumerable<string> cols, SqlDataReader reader)
        {
            var result = new Dictionary<string, T>();
            foreach (var col in cols)
                result.Add(col, (T)reader[col]);
            return result;
        }
        #endregion
    }
}
