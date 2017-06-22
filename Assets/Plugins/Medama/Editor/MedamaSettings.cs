using System.Linq;
using UnityEditor;

namespace Medama
{
    public class MedamaSettings : EditorWindow {
        [MenuItem("Window/Medama Settings")]
        public static void ShowWindow() {
            var window = GetWindow<MedamaSettings>("MEDAMA");
            window.maxSize = new UnityEngine.Vector2(300, 300);
        }

        private void OnGUI() {
            EditorGUILayout.BeginVertical();
            {
                var symbols = GetDefineSymbols();
                var unirx = symbols.Any(x => x == "MEDAMA_USE_UNIRX");
                var observable_ssh = symbols.Any(x => x == "MEDAMA_USE_OBSERVABLE_SSH");
                var arbor2 = symbols.Any(x => x == "MEDAMA_USE_ARBOR2");
                var mysql = symbols.Any(x => x == "MEDAMA_USE_MYSQL");

                var new_unirx = EditorGUILayout.Toggle("Use UniRx", unirx);
                var new_observable_ssh = EditorGUILayout.Toggle("Use ObservableSSH", observable_ssh);
                var new_arbor2 = EditorGUILayout.Toggle("Use Arbor2", arbor2);
                var new_mysql = EditorGUILayout.Toggle("Use MySQL", mysql);

                if (unirx != new_unirx) { if (new_unirx) SetDefineSymbol("MEDAMA_USE_UNIRX"); else DeleteDefineSymbol("MEDAMA_USE_UNIRX"); }
                if (observable_ssh != new_observable_ssh) { if (new_observable_ssh) SetDefineSymbol("MEDAMA_USE_OBSERVABLE_SSH"); else DeleteDefineSymbol("MEDAMA_USE_OBSERVABLE_SSH"); }
                if (arbor2 != new_arbor2) { if (new_arbor2) SetDefineSymbol("MEDAMA_USE_ARBOR2"); else DeleteDefineSymbol("MEDAMA_USE_ARBOR2"); }
                if (mysql != new_mysql) { if (new_mysql) SetDefineSymbol("MEDAMA_USE_MYSQL"); else DeleteDefineSymbol("MEDAMA_USE_MYSQL"); }
            }
            EditorGUILayout.EndVertical();
        }

        static void SetDefineSymbol(string define) {
            var symbols = GetDefineSymbols();
            ArrayUtility.Add(ref symbols, define);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(GetBuildTargetGroup(), string.Join(";", symbols));
        }

        static void DeleteDefineSymbol(string define) {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(GetBuildTargetGroup(), string.Join(";", GetDefineSymbols().Where(symbol => symbol != define).ToArray()));
        }

        static string[] GetDefineSymbols() {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(GetBuildTargetGroup()).Split(';');
        }

        static BuildTargetGroup GetBuildTargetGroup() {
#if UNITY_STANDALONE
            return BuildTargetGroup.Standalone;
#endif
#if UNITY_WII
        return BuildTargetGroup.WiiU;
#endif
#if UNITY_IOS
        return BuildTargetGroup.iOS;
#endif
#if UNITY_ANDROID
        return BuildTargetGroup.Android;
#endif
#if UNITY_PS3
        return BuildTargetGroup.PS3;
#endif
#if UNITY_PS4
        return BuildTargetGroup.PS4;
#endif
#if UNITY_SAMSUNGTV
            return BuildTargetGroup.SamsungTV;
#endif
#if UNITY_XBOX360
            return BuildTargetGroup.XBOX360;
#endif
#if UNITY_XBOXONE
            return BuildTargetGroup.XboxOne;
#endif
#if UNITY_TIZEN
            return BuildTargetGroup.Tizen;
#endif
#if UNITY_TVOS
            return BuildTargetGroup.tvOS;
#endif
#if UNITY_WP_8_1
            return BuildTargetGroup.WP8;
#endif
#if UNITY_WSA
            return BuildTargetGroup.WSA;
#endif
#if UNITY_WEBGL
            return BuildTargetGroup.WebGL;
#endif
        }
    }


}