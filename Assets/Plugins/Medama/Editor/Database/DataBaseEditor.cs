#if MEDAMA_USE_MYSQL
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace Medama.Database
{
    public class DataBaseEditor : SingletonEditor<DataBaseEditor>
    {

        public void Query<T>(T da, DataTable dt) where T : DbDataAdapter {
            da.Fill(dt);
        }


        public IDisposable QueryCoroutineAnotherThread<T>(T da, DataTable dt) where T : DbDataAdapter {
            dt.Clear();
            da.Fill(dt);
            return null;
        }

        public IEnumerable<T> ExecuteQuery<T>(string label, string query, params object[] parameters) where T : class, new() {
            if (parameters == null) {
                parameters = new object[0];
            }
            IEnumerable<T> result = null;
            using (var con = DataBaseConnectionEditor.Instance.GetConnection<MySqlConnection>(label))
            using (var db = new DataContext(con)) {
                result = db.ExecuteQuery<T>(query, parameters);
            }
            return result;
        }
    }
}
#endif
