#if MEDAMA_USE_MYSQL
using System.Collections.Generic;
using System.Data;

namespace Medama.Database
{
    public class DataBaseDataTableEditor : SingletonEditor<DataBaseDataTableEditor>
    {

        public Dictionary<string, DataTable> dts = new Dictionary<string, DataTable>();

        public bool HasTable(string label) {
            return dts.ContainsKey(label);
        }

        public DataTable GetDataTable(string label) {
            if (!dts.ContainsKey(label)) {
                dts[label] = new DataTable();
            }

            return dts[label];
        }
    }
}
#endif
