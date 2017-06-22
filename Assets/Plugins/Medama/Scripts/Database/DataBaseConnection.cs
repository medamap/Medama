#if MEDAMA_USE_MYSQL
using System.Collections.Generic;
using System.Data.Common;
using System;

namespace Medama.Database
{
    public class DataBaseConnection : SingletonMonoBehaviour<DataBaseConnection>
    {

        public Dictionary<string, DbConnection> cons = new Dictionary<string, DbConnection>();

        /// <summary>
        /// コネクション登録
        /// </summary>
        /// <param name="vsplit"></param>
        /// <returns></returns>
        public DbConnection RegistConnection<T>(string label, string connection_string) where T : DbConnection, new() {
            if (!cons.ContainsKey(label)) {
                cons[label] = (DbConnection)Activator.CreateInstance(typeof(T), new object[] { connection_string });
            }
            return cons[label];
        }

        /// <summary>
        /// コネクション取得
        /// </summary>
        /// <param name="vsplit"></param>
        /// <returns></returns>
        public DbConnection GetConnection<T>(string label) where T : DbConnection, new() {
            return cons.ContainsKey(label) ? cons[label] : null;
        }

    }
}
#endif
