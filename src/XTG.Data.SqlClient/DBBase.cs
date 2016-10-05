using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace XTG.Data.SqlClient
{
    public class DBBase : DBLibrary
    {
        private string connectionString;
        public DBBase(string bdConString)
        {
            connectionString = bdConString;
        }

        public async Task<int> ExecAsync(string query)
        {
            return await base.ExecuteNonQueryAsync(connectionString, query, new List<SqlParameter>(), CommandType.Text);
        }

        public async Task<IEnumerable<Dictionary<string, T>>> ExecQueryWithoutParamsAsync<T>(string query)
        {
            return await ExecQueryWithParamsAsync<T>(query, new List<SqlParameter>());
        }

        public async Task<IEnumerable<Dictionary<string, T>>> ExecQueryWithParamsAsync<T>(string query, List<SqlParameter> listParams)
        {
            return await base.ExecuteDataTableAsync<T>(connectionString, query, listParams, CommandType.Text);
        }

        public async Task<IEnumerable<Dictionary<string, T>>> ExecProcedureDataTableWithParamsAsync<T>(string ProcedureName, List<SqlParameter> listParams)
        {
            return await base.ExecuteDataTableAsync<T>(connectionString, ProcedureName, listParams, CommandType.StoredProcedure);
        }

    }
}
