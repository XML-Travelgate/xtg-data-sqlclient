namespace XTG.DataBase.SqlClient
{
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public class DataSet<T>
    {
        private Dictionary<string, int> _columns;
        private Dictionary<int, T[]> _rows;

        public DataSet()
        {
            _columns = new Dictionary<string, int>();
            _rows = new Dictionary<int, T[]>();
        }

        public DataSet(SqlDataReader reader)
        {
            _columns = new Dictionary<string, int>();
            _rows = new Dictionary<int, T[]>();

            SetDataSet(reader);
        }

        public void SetDataSet(SqlDataReader reader)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                _columns.Add(reader.GetName(i), i);
            }

            int numberRow = 0;
            while (reader.Read())
            {
                T[] row = new T[_columns.Count];
                foreach (var column in _columns)
                {
                    row[column.Value] = (T)reader[column.Key];
                }
                _rows.Add(numberRow, row);
                numberRow++;
            }
        }

        public bool HasRows()
        {
            return _rows.Count > 0;
        }

        public int CountRows()
        {
            return _rows.Count;
        }

        public T GetField(string column, int row)
        {
            return _rows[row][_columns[column]];
        }

        public Dictionary<string, T> GetRow(int row)
        {
            Dictionary<string, T> retrow = new Dictionary<string, T>();
            foreach (var columnname in _columns)
            {
                retrow.Add(columnname.Key, _rows[row][columnname.Value]);
            }
            return retrow;
        }

        public Dictionary<string, T> GetFirstRow()
        {
            if (HasRows()) { return GetRow(0); }
            return null;

        }

    }
}
