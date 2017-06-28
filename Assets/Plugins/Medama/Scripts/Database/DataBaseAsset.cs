#if MEDAMA_USE_MYSQL
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Medama.Database
{

    [CreateAssetMenu(menuName = "Database/Database Connection Info")]
    public class DataBaseAsset : ScriptableObject
    {
        public enum DatabaseType
        {
            MySQL,
            PostgreSQL,
            SQLite
        }
        public DatabaseType eDatabaseType = DatabaseType.MySQL;
        public string host = "";
        public int port = 3306;
        public string user = "";
        public string password = "";
        public string passphrase = "";
        public string keyfile = "";
        public bool pooling = false;

        public string GetConnectionString(string database = "{0}") {
            return string.Join(";",
                new List<string[]> {
                    new string[] { "Server", host },
                    new string[] { "Database", database },
                    new string[] { "User ID", user },
                    new string[] { "Password", password },
                    new string[] { "Port", port.ToString() },
                    new string[] { "Pooling", pooling.ToString() }
                }
                .Where(x => x.Length == 2 && !string.IsNullOrEmpty(x[1]))
                .Select(x => string.Format("{0}={1}", x[0], x[1]))
                .ToArray());
        }
    }
}
#endif
