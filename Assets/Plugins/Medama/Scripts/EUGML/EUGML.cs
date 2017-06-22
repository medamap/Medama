using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
#if MEDAMA_USE_MYSQL
using System.Data;
#endif
using System.Linq;
using System.Xml.Linq;
using System.Reflection;

namespace Medama.EUGML {

    public enum SizeFilter {
        NoUse,
        None,
        Vertical,
        Horizontal,
        Both
    }

    /// <summary>
    /// # EUGML独自のレイアウト指定
    /// # EUGML's own layout specification
    /// - RectTransform の anchorMin, anchorMax, anchoredPosition, sizeDelta, pivot の設定方法を指定する
    /// - Specify how to set anchorMin, anchorMax, anchoredPosition, sizeDelta, pivot of RectTransform.
    /// </summary>
    public enum LayoutType {
        /// <summary>
        /// - 何も設定しない
        /// - Not specify anything.
        /// </summary>
        NoUse,
        /// <summary>
        /// - 未使用
        /// - No use.
        /// </summary>
        ScreenFit,
        /// <summary>
        /// - 未使用
        /// - No use.
        /// </summary>
        ScreenFitWithScrollBarBoth,
        /// <summary>
        /// - 未使用
        /// - No use.
        /// </summary>
        ParentFit,
        /// <summary>
        /// - 未使用
        /// - No use.
        /// </summary>
        ParentFitWithScrollBarBoth,
        /// <summary>
        /// - アンカーとピボットを左上に設定し、縦横のサイズを指定可能に設定
        /// - Set anchor and pivot to Top Left. Enable Width and Height.
        /// </summary>
        TopLeft,
        /// <summary>
        /// - アンカーを左上と右上に設定し、ストレッチを左右に設定
        /// - Set the anchor from Top Left to Top Right. Set stretch Left and Right.
        /// </summary>
        TopStretch,
        /// <summary>
        /// - アンカーとピボットを右上に設定し、縦横のサイズを指定可能に設定
        /// - Set anchor and pivot to Top Right. Enable Width and Height.
        /// </summary>
        TopRight,
        /// <summary>
        /// - アンカーを左下から左上に設定し、ストレッチを上下に設定
        /// - Set the anchor from Bottom Left to Top Left. Set stretch Bottom and Top.
        /// </summary>
        StretchLeft,
        /// <summary>
        /// - ストレッチを上下左右に設定<br />
        /// - Set stretch Bottom, Top, Left and Right.
        /// </summary>
        StretchStretch,
        /// <summary>
        /// - アンカーを右下から右上に設定し、ストレッチを上下に設定
        /// - Set the anchor from Bottom Right to Top Right. Set stretch Bottom and Top.
        /// </summary>
        StretchRight,
        /// <summary>
        /// - アンカーとピボットを左下に設定し、縦横のサイズを指定可能に設定
        /// - Set anchor and pivot to Bottom Left. Enable Width and Height.
        /// </summary>
        BottomLeft,
        /// <summary>
        /// - アンカーを左下と右下に設定し、ストレッチを左右に設定
        /// - Set the anchor from Bottom Left to Bottom Right. Set stretch Left and Right.
        /// </summary>
        BottomStretch,
        /// <summary>
        /// - アンカーとピボットを右下に設定し、縦横のサイズを指定可能に設定
        /// - Set anchor and pivot to Bottom Right. Enable Width and Height.
        /// </summary>
        BottomRight,
        /// <summary>
        /// - アンカーとピボットを上段の中央に設定し、縦横のサイズを指定可能に設定
        /// - Set anchor and pivot to Top Center. Enable Width and Height.
        /// </summary>
        TopCenter,
        /// <summary>
        /// - アンカーとピボットを下段の中央に設定し、縦横のサイズを指定可能に設定
        /// - Set anchor and pivot to Bottom Center. Enable Width and Height.
        /// </summary>
        BottomCenter,
        /// <summary>
        /// - アンカーとピボットを中央に設定し、縦横のサイズを指定可能に設定
        /// - Set anchor and pivot to Center Center. Enable Width and Height.
        /// </summary>
        CenterCenter,
        /// <summary>
        /// - アンカーとピボットを中央の左に設定し、縦横のサイズを指定可能に設定
        /// - Set anchor and pivot to Center Left. Enable Width and Height.
        /// </summary>
        CenterLeft,
        /// <summary>
        /// - アンカーとピボットを中央の右に設定し、縦横のサイズを指定可能に設定
        /// - Set anchor and pivot to Center Right. Enable Width and Height.
        /// </summary>
        CenterRight
    }

    /// <summary>
    /// # GameObjectの拡張メソッドを定義する
    /// # Define extension method of GameObject.
    /// </summary>
    public static partial class EUGML {

        private static readonly Vector2 top_left = new Vector2(0f, 1f);
        private static readonly Vector2 center_left = new Vector2(0f, 0.5f);
        private static readonly Vector2 bottom_left = new Vector2(0f, 0f);
        private static readonly Vector2 top_center = new Vector2(0.5f, 1f);
        private static readonly Vector2 center_center = new Vector2(0.5f, 0.5f);
        private static readonly Vector2 bottom_center = new Vector2(0.5f, 0f);
        private static readonly Vector2 top_right = new Vector2(1f, 1f);
        private static readonly Vector2 center_right = new Vector2(1f, 0.5f);
        private static readonly Vector2 bottom_right = new Vector2(1f, 0f);
        private static readonly string _toplevel_resource = "___TLR___"; // Top Level Resource

        private static Dictionary<string, MethodInfo> dcMethodInfo = new Dictionary<string, MethodInfo>();

        /// <summary>
        /// Get extension methods
        /// </summary>
        private static IEnumerable<MethodInfo> GetExtensionMethods(this Type type, Assembly extensionsAssembly) {
            var query = from t in extensionsAssembly.GetTypes()
                        where !t.IsGenericType && !t.IsNested
                        from m in t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        where m.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
                        where m.GetParameters()[0].ParameterType == type
                        select m;

            return query;
        }

        /// <summary>
        /// Get extension method
        /// </summary>
        private static MethodInfo GetExtensionMethod(this Type type, string name) {
            if (dcMethodInfo.ContainsKey(name)) {
                return dcMethodInfo[name];
            }
            dcMethodInfo[name] = type.GetExtensionMethods(Assembly.GetExecutingAssembly()).FirstOrDefault(m => m.Name == name);
            return dcMethodInfo[name];
        }

        /// <summary>
        /// Get extension method
        /// </summary>
        private static MethodInfo GetExtensionMethod(this Type type, Assembly extensionsAssembly, string name, Type[] types) {
            var methods = (from m in type.GetExtensionMethods(extensionsAssembly)
                           where m.Name == name
                           && m.GetParameters().Count() == types.Length + 1 // + 1 because extension method parameter (this)
                           select m).ToList();

            if (!methods.Any()) {
                return default(MethodInfo);
            }

            if (methods.Count() == 1) {
                return methods.First();
            }

            foreach (var methodInfo in methods) {
                var parameters = methodInfo.GetParameters();

                bool found = true;
                for (byte b = 0; b < types.Length; b++) {
                    found = true;
                    if (parameters[b].GetType() != types[b]) {
                        found = false;
                    }
                }

                if (found) {
                    return methodInfo;
                }
            }

            return default(MethodInfo);
        }

        /// <summary>
        /// ## XMLを解釈して uGUI オブジェクトツリーを生成する
        /// ## Create uGUI objects tree from XML
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static Dictionary<int, GameObject> MedamaUIParseXml(string xml) {
            var canvas = MedamaUIGetCanvas();
            MedamaUIGetEventSystem();

            return canvas.MedamaUIParseXml(xml);
        }

        /// <summary>
        /// ## XMLを解釈して uGUI オブジェクトツリーを生成する
        /// ## Create uGUI objects tree from XML
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static Dictionary<int, GameObject> MedamaUIParseXml(this GameObject gameObject, string xml)
        {
#pragma warning disable 219, 168
            var canvas = gameObject.MedamaUIGetCanvas();
#pragma warning restore 219, 168
            gameObject.MedamaUIGetEventSystem();

            var doc = XElement.Parse(xml);
            var dcGO = new Dictionary<int, GameObject>();

            return gameObject.MedamaUIParseXml(doc, dcGO);
        }

        /// <summary>
        /// ## XMLを解釈して uGUI オブジェクトツリーを生成する
        /// ## Create uGUI objects tree from XML
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static Dictionary<int, GameObject> MedamaUIParseXml(this GameObject parent, XElement doc, Dictionary<int, GameObject> dcGO) {
            foreach (var xml_node in doc.Elements()) {
                try {
                    var node = parent.MedamaUIInvokeMethod(xml_node, dcGO);
                    dcGO[node.GetInstanceID()] = node;
                    if (doc.HasElements) {
                        node.MedamaUIParseXml(xml_node, dcGO);
                    }
                } catch (Exception ex) {
                    Debug.Log(ex.Message + "\n" + ex.StackTrace + "\nDetail : " + xml_node.Name.ToString());
                }
            }
            return dcGO;
        }

        /// <summary>
        /// ## XMLノード内の属性をパラメータに変換し、拡張メソッドをコールする
        /// ## Convert attributes in the XML node to parameters and call extension methods.
        /// </summary>
        private static GameObject MedamaUIInvokeMethod(this GameObject node, XElement xElement, Dictionary<int, GameObject> dcGO) {

            var method = "MedamaUI" + xElement.Name.ToString();

            // Dictionary<string, object> arguments = new Dictionary<string, object>();
            Type type = node.GetType();
            var methodinfo = type.GetExtensionMethod(method);

            if (methodinfo == null) {
                Debug.LogWarning(string.Format("method name {0} is not found in {1}", method, type.ToString()));
                return node;
            }

            var attributes = xElement.Attributes().ToDictionary(x => x.Name.ToString(), x => x.Value);

            var parameterinfos = methodinfo.GetParameters().ToDictionary(x => x.Name, x => x);
            object[] parameters = parameterinfos.Select(x => x.Value.DefaultValue).ToArray();

            var index = 0;
            foreach (var pi in parameterinfos) {

                var key = pi.Key;

                if (attributes.ContainsKey(key)) {

                    var value = attributes[key];

                    int iValue = 0;
                    long lValue = 0;
                    float fValue = 0;
                    double dValue = 0;
                    bool boolValue = false;

                    if (pi.Value.ParameterType == typeof(int) && int.TryParse(value, out iValue)) { parameters[index] = iValue; } // int
                    else if (pi.Value.ParameterType == typeof(long) && long.TryParse(value, out lValue)) { parameters[index] = lValue; } // long
                    else if (pi.Value.ParameterType == typeof(float) && float.TryParse(value, out fValue)) { parameters[index] = fValue; } // float
                    else if (pi.Value.ParameterType == typeof(double) && double.TryParse(value, out dValue)) { parameters[index] = dValue; } // double
                    else if (pi.Value.ParameterType == typeof(string)) { parameters[index] = value; } // string
                    else if (pi.Value.ParameterType == typeof(Vector2)) { parameters[index] = value.ToVector2(); } // Vector2
                    else if (pi.Value.ParameterType == typeof(Vector3)) { parameters[index] = Vector3.zero; } // Vector3
                    else if (pi.Value.ParameterType == typeof(Texture2D)) { parameters[index] = null; } // Texture2D
                    else if (pi.Value.ParameterType == typeof(Texture3D)) { parameters[index] = null; } // Texture3D
                    else if (pi.Value.ParameterType == typeof(Sprite)) { parameters[index] = MedamaUILoadSprite(value); } // Sprite
                    else if (pi.Value.ParameterType == typeof(Image.Type)) { parameters[index] = value.ToImageType(); } // Image.Type
                    else if (pi.Value.ParameterType == typeof(Color)) { parameters[index] = value.ToColor(); } // Color
                    else if (
                        pi.Value.ParameterType.IsGenericType &&
                        pi.Value.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                        Nullable.GetUnderlyingType(pi.Value.ParameterType) == typeof(Color)) {
                        parameters[index] = value.ToColor();
                    } // Nullable<Color>
                    else if (pi.Value.ParameterType == typeof(GameObject)) { parameters[index] = dcGO.Where(x => x.Value.name == value).Select(x => x.Value).FirstOrDefault(); } // GameObject
                    else if (pi.Value.ParameterType == typeof(LayoutType)) { parameters[index] = value.ToLayoutType(); } // Mera.UI.LayoutType
                    else if (pi.Value.ParameterType == typeof(TextAnchor)) { parameters[index] = value.ToTextAnchor(); } // TextAnchor
                    else if (pi.Value.ParameterType == typeof(ScrollRect.MovementType)) { parameters[index] = value.ToMovementType(); } // MovementType
                    else if (pi.Value.ParameterType == typeof(bool) && bool.TryParse(value, out boolValue)) { parameters[index] = boolValue; } // bool
                    else if (pi.Value.ParameterType == typeof(ContentSizeFitter.FitMode)) { parameters[index] = value.ToFitMode(); } // ContentSizeFitter.FitMode
                    else if (pi.Value.ParameterType == typeof(InputField.ContentType)) { parameters[index] = value.ToContentType(); } // InputField.ContentType
                    else { Debug.LogWarning(string.Format("Parameter type {0} is not implements in {1}", pi.Value.ParameterType.FullName, xElement.ToString())); }
                }
                index++;
            }
            parameters[0] = node;

            return (GameObject)methodinfo.Invoke(node, parameters);
        }

        /// <summary>
        /// ## リソースからスプライトを読み込み、キャッシュする
        /// ## Load Sprite and cache from Resources.
        /// </summary>
        public static Sprite MedamaUILoadSprite(string value) {

            // Check path type
            // var pathTypelse = (value.IndexOf("://") < 0) ? "" : value.Substring(0, value.IndexOf("://") + 1);
            value = (value.IndexOf("://") < 0) ? value : value.Substring(value.IndexOf("://") + 3);

            // Split multi sprite name # sprite name
            var spriteNamePair = value.Split('#');
            string parent = (spriteNamePair.Length == 2) ? spriteNamePair[0] : "";
            string name = (spriteNamePair.Length == 2) ? spriteNamePair[1] : (spriteNamePair.Length > 0 ? spriteNamePair[0] : "");

            Sprite returnSprite = null;

            if (string.IsNullOrEmpty(parent) && string.IsNullOrEmpty(name)) {
                return null;
            }

            if (string.IsNullOrEmpty(parent)) {

                // No parents
                if (!UIManager.Instance.sprites.ContainsKey(_toplevel_resource)) {
                    UIManager.Instance.sprites[_toplevel_resource] = new Dictionary<string, Sprite>();
                }
                if (!UIManager.Instance.sprites[_toplevel_resource].ContainsKey(name)) {
                    //UIManager.Instance.sprites[_toplevel_resource][name] = Resources.Load<Sprite>(name);
                    UIManager.Instance.sprites[_toplevel_resource][name] = Resources.Load<Sprite>(name);
                }
                returnSprite = UIManager.Instance.sprites[_toplevel_resource][name];

            } else {

                // Parent
                if (!UIManager.Instance.sprites.ContainsKey(parent)) {
                    UIManager.Instance.sprites[parent] = Resources.LoadAll<Sprite>(parent).ToDictionary(sprite => sprite.name);
                }
                UIManager.Instance.sprites[parent].TryGetValue(name, out returnSprite);

            }

            return returnSprite;
        }

        /// <summary>
        /// ## EventSystemを取得する。それが存在しなければ生成する
        /// ## If an EventSystem already exists, it returns its gameObject, and if not, it creates it
        /// </summary>
        public static GameObject MedamaUIGetEventSystem(this GameObject node) { return MedamaUIGetEventSystem(); }

        /// <summary>
        /// ## EventSystemを取得する。それが存在しなければ生成する
        /// ## If an EventSystem already exists, it returns its gameObject, and if not, it creates it
        /// </summary>
        public static GameObject MedamaUIGetEventSystem() {
            GameObject event_object;
            var ev = UnityEngine.Object.FindObjectOfType<EventSystem>();
            if (ev == null) {
                event_object = new GameObject("EventSystem");
                event_object.AddComponent<EventSystem>();
                event_object.AddComponent<StandaloneInputModule>();
            } else {
                event_object = ev.gameObject;
            }
            return event_object;
        }

        /// <summary>
        /// ## Canvasを取得する。それが存在しなければ生成する
        /// ## If the Canvas already exists, it returns its gameObject, and if it does not exist, it creates it
        /// </summary>
        public static GameObject MedamaUIGetCanvas(this GameObject node) { return MedamaUIGetCanvas(); }

        /// <summary>
        /// ## Canvasを取得する。それが存在しなければ生成する
        /// ## If the Canvas already exists, it returns its gameObject, and if it does not exist, it creates it
        /// </summary>
        public static GameObject MedamaUIGetCanvas() {
            GameObject canvas_object;
            var canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
            if (canvas == null) {
                canvas_object = new GameObject("Canvas");

                canvas = canvas_object.AddComponent<Canvas>();
                canvas_object.AddComponent<CanvasScaler>();
                canvas_object.AddComponent<GraphicRaycaster>();

                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            } else {
                canvas_object = canvas.gameObject;
            }
            return canvas_object;
        }

        /// <summary>
        /// ## uGUIオブジェクトを作成して指定された親オブジェクトにアタッチする。親オブジェクトが省略されている場合は、Canvasにアタッチする
        /// ## Create and attach uGUI object to the specified parent object. If parent is omitted, attach to Canvas.
        /// </summary>
        public static GameObject MedamaUIAddNode(
            this GameObject parent,
            string name = "",
            LayoutType layout = LayoutType.StretchStretch,
            float width = 0,
            float height = 0,
            float top = 0,
            float bottom = 0,
            float left = 0,
            float right = 0,
            string sprite = "",
            Image.Type spritetype = Image.Type.Sliced,
            LayoutType pivot = LayoutType.NoUse,
            bool active = true
        ) {
            Vector2 anchorMin = Vector2.zero;
            Vector2 anchorMax = Vector2.zero;
            Vector2 anchoredPosition = Vector2.zero;
            Vector2 sizeDelta = Vector2.zero;
            Vector2 pivot_set = Vector2.zero;

            switch(layout) {
                case LayoutType.TopLeft: anchorMin = top_left; anchorMax = top_left; anchoredPosition = new Vector2(left, -top); sizeDelta = new Vector2(width, height); pivot_set = top_left; break;
                case LayoutType.TopStretch: anchorMin = top_left; anchorMax = top_right; anchoredPosition = new Vector2(0, -top); sizeDelta = new Vector2(-left-right, height); pivot_set = top_center; break;
                case LayoutType.TopRight: anchorMin = top_right; anchorMax = top_right; anchoredPosition = new Vector2(-right, -top); sizeDelta = new Vector2(width, height); pivot_set = top_right; break;
                case LayoutType.StretchLeft: anchorMin = bottom_left; anchorMax = top_left; anchoredPosition = new Vector2(left, bottom); sizeDelta = new Vector2(width, -top-bottom); pivot_set = center_left; break;
                case LayoutType.StretchStretch: anchorMin = bottom_left; anchorMax = top_right; anchoredPosition = new Vector2((left-right)*0.5f, (bottom-top)*0.5f); sizeDelta = new Vector2(-left-right, -bottom-top); pivot_set = center_center; break;
                case LayoutType.StretchRight: anchorMin = bottom_right; anchorMax = top_right; anchoredPosition = new Vector2(-right, bottom); sizeDelta = new Vector2(width, -top - bottom); pivot_set = center_right; break;
                case LayoutType.BottomLeft: anchorMin = bottom_left; anchorMax = bottom_left; anchoredPosition = new Vector2(left, bottom); sizeDelta = new Vector2(width, height); pivot_set = bottom_left; break;
                case LayoutType.BottomStretch: anchorMin = bottom_left; anchorMax = bottom_right; anchoredPosition = new Vector2(0, bottom); sizeDelta = new Vector2(-left - right, height); pivot_set = bottom_center; break;
                case LayoutType.BottomRight: anchorMin = bottom_right; anchorMax = bottom_right; anchoredPosition = new Vector2(-right, bottom); sizeDelta = new Vector2(width, height); pivot_set = bottom_right; break;
                case LayoutType.TopCenter: anchorMin = top_center; anchorMax = top_center; anchoredPosition = new Vector2(0, -top); sizeDelta = new Vector2(width, height); pivot_set = top_center; break;
                case LayoutType.CenterLeft: anchorMin = center_left; anchorMax = center_left; anchoredPosition = new Vector2(left, 0); sizeDelta = new Vector2(width, height); pivot_set = center_left; break;
                case LayoutType.CenterCenter: anchorMin = center_center; anchorMax = center_center; anchoredPosition = Vector2.zero; sizeDelta = new Vector2(width, height); pivot_set = center_center; break;
                case LayoutType.CenterRight: anchorMin = center_right; anchorMax = center_right; anchoredPosition = new Vector2(-right, 0); sizeDelta = new Vector2(width, height); pivot_set = center_right; break;
                case LayoutType.BottomCenter: anchorMin = bottom_center; anchorMax = bottom_center; anchoredPosition = new Vector2(0, bottom); sizeDelta = new Vector2(width, height); pivot_set = bottom_center; break;
            }

            switch(pivot) {
                case LayoutType.TopLeft: pivot_set = top_left; break;
                case LayoutType.TopCenter: pivot_set = top_center; break;
                case LayoutType.TopRight: pivot_set = top_right; break;
                case LayoutType.CenterLeft: pivot_set = center_left; break;
                case LayoutType.CenterCenter: pivot_set = center_center; break;
                case LayoutType.CenterRight: pivot_set = center_right; break;
                case LayoutType.BottomLeft: pivot_set = bottom_left; break;
                case LayoutType.BottomCenter: pivot_set = bottom_center; break;
                case LayoutType.BottomRight: pivot_set = bottom_right; break;
            }

            var newnode = string.IsNullOrEmpty(sprite)
                ? parent.MedamaUIAddNode(name, anchorMin, anchorMax, anchoredPosition, sizeDelta, pivot_set)
                : parent.MedamaUIAddNode(name, anchorMin, anchorMax, anchoredPosition, sizeDelta, pivot_set).MedamaUISetCanvasRender().MedamaUISetImage(MedamaUILoadSprite(sprite), spritetype);

            newnode.SetActive(active);

            return newnode;
        }

        /// <summary>
        /// ## uGUIオブジェクトを作成して指定された親オブジェクトにアタッチする。親オブジェクトが省略されている場合は、Canvasにアタッチする
        /// ## Create and attach uGUI object to the specified parent object. If parent is omitted, attach to Canvas.
        /// </summary>
        public static GameObject MedamaUIAddNode(this GameObject parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta, Vector2 pivot) {
            name = string.IsNullOrEmpty(name) ? Guid.NewGuid().ToString() : name;
            GameObject node = new GameObject(name);
            var rect = node.AddComponent<RectTransform>();
            node.transform.SetParent(parent.transform);
            rect.anchoredPosition = anchoredPosition;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.sizeDelta = sizeDelta;
            rect.pivot = pivot;
            return node;
        }

        /// <summary>
        /// ## Regist Image component
        /// </summary>
        public static GameObject MedamaUISetImage(
            this GameObject node,
            Sprite sprite = null,
            Image.Type type = Image.Type.Simple
        ) {
            var image = node.GetComponent<Image>() ?? node.AddComponent<Image>();
            image.sprite = sprite;
            image.type = type;
            return node;
        }

        /// <summary>
        /// ## Regist CanvasRender component
        /// </summary>
        public static GameObject MedamaUISetCanvasRender(
            this GameObject node
        ) {
            if (node.GetComponent<CanvasRenderer>() == null) node.AddComponent<CanvasRenderer>();
            return node;
        }

        /// <summary>
        /// ## Regist Mask component
        /// </summary>
        public static GameObject MedamaUISetMask(
            this GameObject node,
            bool showMaskGraphic = true
        ) {
            var mask = node.GetComponent<Mask>() ?? node.AddComponent<Mask>();
            mask.showMaskGraphic = showMaskGraphic;
            return node;
        }

        /// <summary>
        /// ## Regist ScrollRect component
        /// </summary>
        public static GameObject MedamaUISetScrollRect(
            this GameObject node,
            bool horizontal = true,
            bool vertical = true,
            ScrollRect.MovementType movementType = ScrollRect.MovementType.Elastic
        ) {
            var scroll_rect = node.GetComponent<ScrollRect>() ?? node.AddComponent<ScrollRect>();
            scroll_rect.horizontal = horizontal;
            scroll_rect.vertical = vertical;
            scroll_rect.movementType = movementType;
            return node;
        }

        /// <summary>
        /// ## Regist HorizontalLayoutGroup component
        /// </summary>
        public static GameObject MedamaUISetHorizontalLayoutGroup(
            this GameObject node,
            TextAnchor childAlignment = TextAnchor.MiddleCenter,
            float spacing = 0,
            bool childForceExpandWidth = false,
            bool childForceExpandHeight = false
        ) {
            var layout = node.GetComponent<HorizontalLayoutGroup>() ?? node.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = childAlignment;
            layout.spacing = spacing;
            layout.childForceExpandWidth = childForceExpandWidth;
            layout.childForceExpandHeight = childForceExpandHeight;
            return node;
        }

        /// <summary>
        /// ## Regist VerticalLayoutGroup component
        /// </summary>
        public static GameObject MedamaUISetVerticalLayoutGroup(
            this GameObject node,
            TextAnchor childAlignment = TextAnchor.MiddleCenter,
            float spacing = 0,
            bool childForceExpandWidth = false,
            bool childForceExpandHeight = false
        ) {
            var layout = node.GetComponent<VerticalLayoutGroup>() ?? node.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = childAlignment;
            layout.spacing = spacing;
            layout.childForceExpandWidth = childForceExpandWidth;
            layout.childForceExpandHeight = childForceExpandHeight;
            return node;
        }

        /// <summary>
        /// ## Regist ContentSizeFitter component
        /// </summary>
        public static GameObject MedamaUISetContentSizeFitter(
            this GameObject node,
            ContentSizeFitter.FitMode horizontalFit = ContentSizeFitter.FitMode.Unconstrained,
            ContentSizeFitter.FitMode verticalFit = ContentSizeFitter.FitMode.Unconstrained,
            SizeFilter sizeFilter = SizeFilter.NoUse
        ) {
            switch (sizeFilter) {
                case SizeFilter.NoUse: break;
                case SizeFilter.None:
                    horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                    verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                    break;
                case SizeFilter.Horizontal:
                    horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                    break;
                case SizeFilter.Vertical:
                    horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                    verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    break;
                case SizeFilter.Both:
                    horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    break;
            }
            var filter = node.GetComponent<ContentSizeFitter>() ?? node.AddComponent<ContentSizeFitter>();
            filter.horizontalFit = horizontalFit;
            filter.verticalFit = verticalFit;
            return node;
        }

        /// <summary>
        /// ## Attach content node to parent ScrollRect node
        /// </summary>
        /// <returns></returns>
        public static GameObject MedamaUIAttachContentToScrollRect(
            this GameObject node,
            GameObject parent = null
        ) {
            
            parent = parent ?? node.transform.parent.gameObject;
            if (parent != null) {
                var parent_rect = parent.GetComponent<ScrollRect>();
                var node_transform = node.GetComponent<RectTransform>();
                if (parent_rect != null && node_transform != null) {
                    parent_rect.content = node_transform;
                }
            }
            return node;
        }

        /// <summary>
        /// ## Regist LayoutElement component
        /// </summary>
        public static GameObject MedamaUISetLayoutElement(
            this GameObject node,
            float minWidth = 16,
            float minHeight = 16
        ) {
            var element = node.GetComponent<LayoutElement>() ?? node.AddComponent<LayoutElement>();
            element.minWidth = minWidth;
            element.minHeight = minHeight;
            return node;
        }

        /// <summary>
        /// ## Regist Text component
        /// </summary>
        public static GameObject MedamaUISetText(
            this GameObject node,
            string textstring = "",
            Color? color = null,
            int fontsize = 14,
            TextAnchor alignment = TextAnchor.MiddleCenter,
            FontStyle fontStyle = FontStyle.Normal,
            Font font = null,
            bool resizeTextForBestFit = false,
            int resizeTextMinSize = 10,
            int resizeTextMaxSize = 40,
            bool supportRichText = true,
            HorizontalWrapMode horizontalOverflow = HorizontalWrapMode.Wrap,
            VerticalWrapMode verticalOverflow = VerticalWrapMode.Truncate,
            int lineSpacing = 1
        ) {
            var setColor = color ?? Color.black;

            if (font == null) {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            var text = node.GetComponent<Text>() ?? node.AddComponent<Text>();

            text.text = textstring ?? "";
            text.color = setColor;
            text.font = font ?? font;
            text.fontSize = fontsize;
            text.fontStyle = fontStyle;
            text.resizeTextForBestFit = resizeTextForBestFit;
            text.resizeTextMinSize = resizeTextMinSize;
            text.resizeTextMaxSize = resizeTextMaxSize;
            text.alignment = alignment;
            text.supportRichText = supportRichText;
            text.horizontalOverflow = horizontalOverflow;
            text.verticalOverflow = verticalOverflow;
            text.lineSpacing = lineSpacing;

            return node;
        }

        /// <summary>
        /// ## Regist InputField component
        /// </summary>
        public static GameObject MedamaUISetInputField(
            this GameObject node,
            string textPlaceholder = "",
            Color? textColor = null,
            Color? placeHolderColor = null,
            int fontSize = 14,
            float top = 7,
            float bottom = 6,
            float left = 10,
            float right = 10,
            InputField.ContentType contentType = InputField.ContentType.Standard
        ) {
            var inputField = node.GetComponent<InputField>() ?? node.AddComponent<InputField>();

            var text = node.MedamaUIAddNode("Text", LayoutType.StretchStretch, 0, 0, top, bottom, left, right, "", Image.Type.Simple)
                .MedamaUISetText("", textColor, fontSize, TextAnchor.MiddleLeft)
                .GetComponent<Text>();

            var placeholder = node.MedamaUIAddNode("Placeholder", LayoutType.StretchStretch, 0, 0, top, bottom, left, right, "", Image.Type.Simple)
                .MedamaUISetText(textPlaceholder, placeHolderColor, fontSize, TextAnchor.MiddleLeft)
                .GetComponent<Text>();

            inputField.textComponent = text;
            inputField.placeholder = placeholder;
            inputField.contentType = contentType;

            return node;
        }

        /// <summary>
        /// ## Regist Button component
        /// </summary>
        public static GameObject MedamaUISetButton(
            this GameObject node,
            string textButton = "",
            Color? colorButton = null
        ) {
            if (node.GetComponent<Button>() == null) node.AddComponent<Button>();
            if (node.GetComponent<Image>() == null) node.AddComponent<Image>();
            node.MedamaUIAddNode("Text", LayoutType.StretchStretch, 0, 0, 0, 0, 0, 0, "", Image.Type.Simple)
                .MedamaUISetText(textButton, colorButton);

            return node;
        }

        /// <summary>
        /// ## Regist Scrollbar component
        /// </summary>
        public static GameObject MedamaUISetScrollbar(
            this GameObject node,
            GameObject scrollview = null,
            bool Horizontal = false,
            bool Vertical = false
        ) {

            var scrollbar = node.GetComponent<Scrollbar>() ?? node.AddComponent<Scrollbar>();

            if (scrollview != null) {
                var scroll_rect = scrollview.GetComponent<ScrollRect>();
                if (scroll_rect != null) {
                    if (Horizontal) {
                        scroll_rect.horizontalScrollbar = scrollbar;
                        scrollbar.direction = Scrollbar.Direction.LeftToRight;
                        scrollbar.value = 0.0000000000000001f;
                    }
                    if (Vertical) {
                        scroll_rect.verticalScrollbar = scrollbar;
                        scrollbar.direction = Scrollbar.Direction.BottomToTop;
                        scrollbar.value = 1f;
                    }
                }
            }

            return node;
        }

        /// <summary>
        /// ## Regist Handle's RectTransform to Scrollbar component
        /// </summary>
        public static GameObject MedamaUISetHandleToScrollBar(
            this GameObject node,
            GameObject handle
        ) {
            var scrollbar = node.GetComponent<Scrollbar>();
            var handle_rect = handle.GetComponent<RectTransform>();
            if (scrollbar != null && handle_rect != null) {
                scrollbar.handleRect = handle_rect;
            }
            return node;
        }

        /// <summary>
        /// ## Regist Handle's RectTransform to Scrollbar component
        /// </summary>
        public static GameObject MedamaUISetScrollBarValue(
            this GameObject node,
            float value
        ) {
            var scrollbar = node.GetComponent<Scrollbar>();
            if (scrollbar != null) {
                scrollbar.value = value;
            }
            return node;
        }

        /// <summary>
        /// ## データグリッドを設定する
        /// ## Regist DataGrid component
        /// </summary>
        public static void MedamaUISetDataGrid<T>(
            this GameObject node,
            string name = "DataGrid",
            List<T> list = null,
#if MEDAMA_USE_MYSQL
            DataTable datatable = null,
#endif
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance,
            LayoutType rowLayout = LayoutType.TopStretch,
            TextAnchor rowChildAlignment = TextAnchor.UpperLeft,
            LayoutType cellLayout = LayoutType.TopLeft,
            int minWidth = 120,
            int minHeight = 24,
            string datagridSprite = "resources://Medama/EUGML/UI001#Group001",
            string scrollviewSprite = "resources://Medama/EUGML/UI001#Group001",
            string headerSprite = "resources://Medama/EUGML/UI001#GridHead001",
            string cellSprite = "resources://Medama/EUGML/UI001#GridCell001",
            string vscrollbarSprite = "resources://Medama/EUGML/UI001#GridCell001",
            string vslideareaSprite = "resources://Medama/EUGML/UI001#GridHead001",
            string hscrollbarSprite = "resources://Medama/EUGML/UI001#GridCell001",
            string hslideareaSprite = "resources://Medama/EUGML/UI001#GridHead001"
        ) {
            var type = typeof(T);
            var members = type.GetFields(flags);

            var datagrid = node.MedamaUIAddNode(name: name, top: 8, bottom: 8, left: 8, right: 8, sprite: datagridSprite);

            var scrollview = datagrid.MedamaUIAddNode(name: "ScrollView", right: 20, bottom: 20, sprite: scrollviewSprite)
                .MedamaUISetScrollRect(movementType: ScrollRect.MovementType.Clamped)
                .MedamaUISetMask(showMaskGraphic: false);

            var content = scrollview.MedamaUIAddNode(name: "Content", bottom: 8, right: 8)
                .MedamaUISetVerticalLayoutGroup()
                .MedamaUISetContentSizeFitter(horizontalFit: ContentSizeFitter.FitMode.PreferredSize, verticalFit: ContentSizeFitter.FitMode.PreferredSize)
                .MedamaUIAttachContentToScrollRect();

            if (list != null) {

                var index = 1;
                foreach (var data in list) {
                    // Header
                    if (index == 1) {
                        var headerrow = content.MedamaUIAddNode(name: "HeaderRow", layout: rowLayout)
                            .MedamaUISetHorizontalLayoutGroup(childAlignment: rowChildAlignment);
                        foreach (var member in members) {
                            headerrow.MedamaUIAddNode(name: member.Name, layout: cellLayout, sprite: headerSprite)
                                .MedamaUISetLayoutElement(minWidth: minWidth, minHeight: minHeight)
                                .MedamaUIAddNode(name: "Text")
                                .MedamaUISetText(textstring: member.Name);
                        }
                    }
                    // Data
                    var datarow = content.MedamaUIAddNode(name: "DataCell" + index.ToString(), layout: cellLayout)
                        .MedamaUISetHorizontalLayoutGroup(childAlignment: rowChildAlignment);
                    foreach (var member in members) {
                        datarow.MedamaUIAddNode(name: member.Name + index.ToString(), layout: cellLayout, sprite: cellSprite)
                            .MedamaUISetLayoutElement(minWidth: minWidth, minHeight: minHeight)
                            .MedamaUIAddNode(name: "Text")
                            .MedamaUISetText(textstring: member.GetValue(data).ToString());
                    }
                    index++;
                }

            }
#if MEDAMA_USE_MYSQL
            if (datatable != null) {

                var index = 1;
                foreach (DataRow row in datatable.Rows) {
                    // Header
                    if (index == 1) {
                        var headerrow = content.MedamaUIAddNode(name: "HeaderRow", layout: rowLayout)
                            .MedamaUISetHorizontalLayoutGroup(childAlignment: rowChildAlignment);
                        foreach (DataColumn column in datatable.Columns) {
                            headerrow.MedamaUIAddNode(name: column.ColumnName, layout: cellLayout, sprite: headerSprite)
                                .MedamaUISetLayoutElement(minWidth: minWidth, minHeight: minHeight)
                                .MedamaUIAddNode(name: "Text")
                                .MedamaUISetText(textstring: column.ColumnName);
                        }
                    }
                    // Data
                    var datarow = content.MedamaUIAddNode(name: "DataCell" + index.ToString(), layout: cellLayout)
                        .MedamaUISetHorizontalLayoutGroup(childAlignment: rowChildAlignment);
                    foreach (DataColumn column in datatable.Columns) {
                        datarow.MedamaUIAddNode(name: column.ColumnName + index.ToString(), layout: cellLayout, sprite: cellSprite)
                            .MedamaUISetLayoutElement(minWidth: minWidth, minHeight: minHeight)
                            .MedamaUIAddNode(name: "Text")
                            .MedamaUISetText(textstring: row[column.ColumnName].ToString());
                    }
                    index++;
                }

            }
#endif
            var vscroll_bar = datagrid.MedamaUIAddNode(name: "VScrollBar", layout: LayoutType.StretchRight, width: 20, bottom: 20, sprite: vscrollbarSprite, pivot: LayoutType.BottomRight)
                .MedamaUISetScrollbar(scrollview: scrollview, Vertical: true);
            var vsliding_area = vscroll_bar.MedamaUIAddNode(name: "Sliding Area", layout: LayoutType.StretchStretch, left: 10, top: 10, bottom: 10, right: 10);
            var vhandle = vsliding_area.MedamaUIAddNode(name: "VHandle", layout: LayoutType.StretchStretch, left: -10, top: -10, bottom: -10, right: -10, sprite: vslideareaSprite);

            vscroll_bar.MedamaUISetHandleToScrollBar(vhandle);

            var hscroll_bar = datagrid.MedamaUIAddNode(name: "HScrollBar", layout: LayoutType.BottomStretch, pivot: LayoutType.BottomLeft, height: 20, right: 20, sprite: hscrollbarSprite)
                .MedamaUISetScrollbar(Horizontal: true, scrollview: scrollview);
            var hsliding_area = hscroll_bar.MedamaUIAddNode(name: "Sliding Area", layout: LayoutType.StretchStretch, left: 10, top: 10, bottom: 10, right: 10);
            var hhandle = hsliding_area.MedamaUIAddNode(name: "HHandle", layout: LayoutType.StretchStretch, left: -10, top: -10, bottom: -10, right: -10, sprite: hslideareaSprite);

            hscroll_bar.MedamaUISetHandleToScrollBar(hhandle);

        }
    }
}
