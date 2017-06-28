#if MEDAMA_USE_MYSQL
using System;
using System.Collections.Generic;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace Medama.Database
{
    public class DataBaseDataAdapterEditor : SingletonEditor<DataBaseDataAdapterEditor>
    {

        public Dictionary<string, DbDataAdapter> das = new Dictionary<string, DbDataAdapter>();

        public DbDataAdapter InitializeDataAdapter<T>(string conlabel, string datalabel, string query) where T : DbDataAdapter, new() {
            var con = DataBaseConnectionEditor.Instance.GetConnection<MySqlConnection>(conlabel);
            das[datalabel] = (DbDataAdapter)Activator.CreateInstance(typeof(T), new object[] { query, con });
            return das[datalabel];
        }

        public DbDataAdapter GetDataAdapter<T>(string label) {
            if (!das.ContainsKey(label)) {
                throw new Exception(string.Format("DataAdapter \"{0}\" is not found.", label));
            }
            return das[label];
        }
    }
}
#endif
