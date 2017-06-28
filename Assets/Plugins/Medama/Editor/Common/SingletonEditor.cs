namespace Medama {

    public class SingletonEditor<T> where T : class, new() {

        private static T instance;
        private static object syncObj = new object();

        public static T Instance {
            get {
                if (instance == null) {
                    lock (syncObj) {
                        instance = new T();
                    }
                }
                return instance;
            }
            private set {
                instance = value;
            }
        }

        protected SingletonEditor() { }
    }

}
