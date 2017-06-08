using UnityEngine;

namespace Medama
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static volatile T instance;
        private static object syncObj = new object();

        public static T Instance {
            get {
                if (applicationIsQuitting) {
                    return null;
                }
                if (instance == null) {
                    instance = FindObjectOfType<T>() as T;

                    if (FindObjectsOfType<T>().Length > 1) {
                        return instance;
                    }

                    if (instance == null) {
                        lock (syncObj) {
                            GameObject singleton = new GameObject();
                            singleton.name = typeof(T).ToString() + " (singleton)";
                            instance = singleton.AddComponent<T>();
                            DontDestroyOnLoad(singleton);
                        }
                    }

                }
                return instance;
            }
            private set {
                instance = value;
            }
        }

        static bool applicationIsQuitting = false;

        void OnApplicationQuit() {
            applicationIsQuitting = true;
        }

        void OnDestroy() {
            Instance = null;
        }

        protected SingletonMonoBehaviour() { }
    }
}