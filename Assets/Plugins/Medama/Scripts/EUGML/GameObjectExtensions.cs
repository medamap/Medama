using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using UniRx;

namespace Medama.EUGML {

    public enum SizeFilter {
        NoUse,
        None,
        Vertical,
        Horizontal,
        Both
    }

    public enum LayoutType {
        NoUse,
        ScreenFit,
        ScreenFitWithScrollBarBoth,
        ParentFit,
        ParentFitWithScrollBarBoth,
        TopLeft,
        TopStretch,
        TopRight,
        StretchLeft,
        StretchStretch,
        StretchRight,
        BottomLeft,
        BottomStretch,
        BottomRight,
        TopCenter,
        BottomCenter,
        CenterCenter,
        CenterLeft,
        CenterRight
    }

    public static partial class GameObjectExtensions {

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

        private static Type tGameObject = typeof(GameObject);
        private static Dictionary<string, MethodInfo> dcMethodInfo = new Dictionary<string, MethodInfo>();


        /// <summary>
        /// 複数の拡張メソッド取得
        /// </summary>
        /// <param name="type"></param>
        /// <param name="extensionsAssembly"></param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetExtensionMethods(this Type type, Assembly extensionsAssembly) {
            var query = from t in extensionsAssembly.GetTypes()
                        where !t.IsGenericType && !t.IsNested
                        from m in t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        where m.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
                        where m.GetParameters()[0].ParameterType == type
                        select m;

            return query;
        }

        /// <summary>
        /// 拡張メソッド取得
        /// </summary>
        /// <param name="type"></param>
        /// <param name="extensionsAssembly"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MethodInfo GetExtensionMethod(this Type type, string name) {
            if (dcMethodInfo.ContainsKey(name)) {
                return dcMethodInfo[name];
            }
            dcMethodInfo[name] = type.GetExtensionMethods(Assembly.GetExecutingAssembly()).FirstOrDefault(m => m.Name == name);
            return dcMethodInfo[name];
        }

        /// <summary>
        /// 拡張メソッド取得
        /// </summary>
        /// <param name="type"></param>
        /// <param name="extensionsAssembly"></param>
        /// <param name="name"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static MethodInfo GetExtensionMethod(this Type type, Assembly extensionsAssembly, string name, Type[] types) {
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
        /// XML ドキュメントを用いて、uGUI オブジェクトを生成する
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="xml"></param>
        public static Dictionary<int, GameObject> MedamaUIParseXml(this GameObject gameObject, string xml) {

            var canvas = gameObject.MedamaUIGetCanvas();
            var eventsystem = gameObject.MedamaUIGetEventSystem();

            var doc = XElement.Parse(xml);
            var dcGO = new Dictionary<int, GameObject>();

            foreach (var xml_node in doc.Descendants().Where(x => x.Name.ToString().ToLower() == "addnode")) {

                // Get parent
                var parent = xml_node.Parent;
                var parent_instanceid = (parent != null && parent.Attribute("InstanceID") != null) ? parent.Attribute("InstanceID").Value : null;
                int instance_id = 0;
                GameObject parent_obj = null;
                if (parent_instanceid != null) {
                    parent_obj = (int.TryParse(parent_instanceid, out instance_id) || dcGO.ContainsKey(instance_id)) ? dcGO[instance_id] : null;
                }

                // Create Node
                var node = (parent_obj != null)
                    ? parent_obj.MedamaUIInvokeMethod(xml_node, "MedamaUIAddNode")
                    : canvas.MedamaUIInvokeMethod(xml_node, "MedamaUIAddNode");

                // Set Dictionary
                dcGO[node.GetInstanceID()] = node;
                xml_node.SetAttributeValue("InstanceID", node.GetInstanceID());

                // Components
                foreach (var xml_component in xml_node.Elements().Where(x => x.Name.ToString().ToLower() != "addnode")) {
                    try {
                        node.MedamaUIInvokeMethod(xml_component, "MedamaUI" + xml_component.Name.ToString());
                    } catch (Exception ex) {
                        Debug.Log(ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }

            return dcGO;
        }

        /// <summary>
        /// 拡張メソッドコール用引数情報生成
        /// </summary>
        /// <param name="node"></param>
        /// <param name="xElement"></param>
        /// <returns></returns>
        public static GameObject MedamaUIInvokeMethod(this GameObject node, XElement xElement, string methodname) {

            Dictionary<string, object> arguments = new Dictionary<string, object>();
            Type type = node.GetType();
            var methodinfo = type.GetExtensionMethod(methodname);

            if (methodinfo == null) {
                Debug.LogWarningFormat("method name {0} is not found in {1}", methodname, type.ToString());
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
                    else if (pi.Value.ParameterType == typeof(Sprite)) { parameters[index] = LoadSprite(value); } // Sprite
                    else if (pi.Value.ParameterType == typeof(Image.Type)) { parameters[index] = value.ToImageType(); } // Image.Type
                    else if (pi.Value.ParameterType == typeof(Color)) { parameters[index] = value.ToColor(); } // Color
                    else if (
                        pi.Value.ParameterType.IsGenericType &&
                        pi.Value.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                        Nullable.GetUnderlyingType(pi.Value.ParameterType) == typeof(Color)) {
                        parameters[index] = value.ToColor();
                    } // Nullable<Color>
                    else if (pi.Value.ParameterType == typeof(GameObject)) { parameters[index] = null; } // GameObject
                    else if (pi.Value.ParameterType == typeof(LayoutType)) { parameters[index] = value.ToLayoutType(); } // Mera.UI.LayoutType
                    else if (pi.Value.ParameterType == typeof(TextAnchor)) { parameters[index] = value.ToTextAnchor(); } // TextAnchor
                    else if (pi.Value.ParameterType == typeof(ScrollRect.MovementType)) { parameters[index] = value.ToMovementType(); } // MovementType
                    else if (pi.Value.ParameterType == typeof(bool) && bool.TryParse(value, out boolValue)) { parameters[index] = boolValue; } // bool
                    else if (pi.Value.ParameterType == typeof(ContentSizeFitter.FitMode)) { parameters[index] = value; } // ContentSizeFitter.FitMode
                    else if (pi.Value.ParameterType == typeof(InputField.ContentType)) { parameters[index] = value.ToContentType(); } // InputField.ContentType
                    else { Debug.LogWarningFormat("Parameter type {0} is not implements in {1}", pi.Value.ParameterType.FullName, xElement.ToString()); }
                }
                index++;
            }
            parameters[0] = node;

            return (GameObject)methodinfo.Invoke(node, parameters);
        }

        /// <summary>
        /// プロパティ設定
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <param name="xElement"></param>
        public static GameObject MedamaUISetPropertyFromXml<T>(this GameObject node, T component, XElement xElement) {
            foreach (var attribute in xElement.Attributes()) {
                var key = attribute.Name.ToString();
                var value = attribute.Value.ToString();
                var property = component.GetType().GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
                if (property == null) {
                    Debug.LogWarning(string.Format("Attribute {0} is not found in {1} (value={2}", key, component.ToString(), value));
                    continue;
                }

                int iValue = 0;
                long lValue = 0;
                float fValue = 0;
                double dValue = 0;

                if (property.PropertyType == typeof(int) && int.TryParse(value, out iValue)) { property.SetValue(component, iValue, null); } // int
                else if (property.PropertyType == typeof(long) && long.TryParse(value, out lValue)) { property.SetValue(component, lValue, null); } // long
                else if (property.PropertyType == typeof(float) && float.TryParse(value, out fValue)) { property.SetValue(component, fValue, null); } // float
                else if (property.PropertyType == typeof(double) && double.TryParse(value, out dValue)) { property.SetValue(component, dValue, null); } // double
                else if (property.PropertyType == typeof(string)) { property.SetValue(component, value, null); } // string
                else if (property.PropertyType == typeof(Vector2)) { property.SetValue(component, value.ToVector2(), null); } // Vector2
                else if (property.PropertyType == typeof(Vector3)) { property.SetValue(component, Vector3.zero, null); } // Vector3
                else if (property.PropertyType == typeof(Texture2D)) { property.SetValue(component, null, null); } // Texture2D
                else if (property.PropertyType == typeof(Texture3D)) { property.SetValue(component, null, null); } // Texture3D
                else if (property.PropertyType == typeof(Sprite)) { property.SetValue(component, LoadSprite(value), null); } // Sprite
                else if (property.PropertyType == typeof(Image.Type)) { property.SetValue(component, value.ToImageType(), null); } // Image.Type
                else if (property.PropertyType == typeof(Color)) { property.SetValue(component, value.ToImageType(), null); } // Image.Type
                else if (property.PropertyType == typeof(GameObject)) { property.SetValue(component, null, null); } // GameObject
            }
            return node;
        }

        /// <summary>
        /// リソースから Sprite を読み込み、キャッシュする
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Sprite LoadSprite(string value) {

            // パスタイプ判定
            var pathTypelse = (value.IndexOf("://") < 0) ? "" : value.Substring(0, value.IndexOf("://") + 1);
            value = (value.IndexOf("://") < 0) ? value : value.Substring(value.IndexOf("://") + 3);

            // マルチスプライト名#スプライト名 の分割
            var spriteNamePair = value.Split('#');
            string parent = (spriteNamePair.Length == 2) ? spriteNamePair[0] : "";
            string name = (spriteNamePair.Length == 2) ? spriteNamePair[1] : (spriteNamePair.Length > 0 ? spriteNamePair[0] : "");

            Sprite returnSprite = null;

            if (string.IsNullOrEmpty(parent) && string.IsNullOrEmpty(name)) {
                return null;
            }

            if (string.IsNullOrEmpty(parent)) {

                // 親無し
                if (!UIManager.Instance.sprites.ContainsKey(_toplevel_resource)) {
                    UIManager.Instance.sprites[_toplevel_resource] = new Dictionary<string, Sprite>();
                }
                if (!UIManager.Instance.sprites[_toplevel_resource].ContainsKey(name)) {
                    //UIManager.Instance.sprites[_toplevel_resource][name] = Resources.Load<Sprite>(name);
                    UIManager.Instance.sprites[_toplevel_resource][name] = UnityEngine.Resources.Load<Sprite>(name);
                }
                returnSprite = UIManager.Instance.sprites[_toplevel_resource][name];

            } else {

                // 親有り
                if (!UIManager.Instance.sprites.ContainsKey(parent)) {
                    UIManager.Instance.sprites[parent] = UnityEngine.Resources.LoadAll<Sprite>(parent).ToDictionary<Sprite, string>(sprite => sprite.name);
                }
                UIManager.Instance.sprites[parent].TryGetValue(name, out returnSprite);
            }

            return returnSprite;
        }


        /// <summary>
        /// XMLドキュメントを用いて、uGuiオブジェクトを生成する
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="xml"></param>
        public static Dictionary<int, GameObject> MedamaUIAddGuiFromXml(this GameObject gameObject, string xml) {

            var doc = XElement.Parse(xml);
            var dcGO = new Dictionary<int, GameObject>();

            foreach (var node in doc.Descendants().Where(x => x.Name.ToString().ToLower() == "node")) {

                // Get parent
                var parent = node.Parent;
                var parent_instanceid = (parent != null && parent.Attribute("InstanceID") != null) ? parent.Attribute("InstanceID").Value : null;
                int instance_id = 0;
                GameObject parent_obj = null;
                if (parent_instanceid != null) {
                    parent_obj = (int.TryParse(parent_instanceid, out instance_id) || dcGO.ContainsKey(instance_id)) ? dcGO[instance_id] : null;
                }

                // Create Node
                GameObject addnode = new GameObject(node.Name.ToString());

                if (parent_obj != null) {
                    addnode.transform.SetParent(parent_obj.transform);
                } else {
                    addnode.transform.SetParent(gameObject.MedamaUIGetCanvas().transform);
                }

                // Add RectTransform
                addnode.AddComponent<RectTransform>();
                var rect = addnode.GetComponent<RectTransform>();
                UIUtil.SetProperty(rect, node);

                // Set Dictionary
                dcGO[addnode.GetInstanceID()] = addnode;
                node.SetAttributeValue("InstanceID", addnode.GetInstanceID());

                // Components
                foreach (var component in node.Elements().Where(x => x.Name.ToString().ToLower() == "component")) {
                    try {
                        System.Type oComponentType = null;
                        switch (component.Attribute("name").Value.ToLower()) {
                            case "canvasrenderer": oComponentType = typeof(CanvasRenderer); break;
                            case "image": oComponentType = typeof(Image); break;
                            case "layoutelement": oComponentType = typeof(LayoutElement); break;
                            case "text": oComponentType = typeof(Text); break;
                            case "button": oComponentType = typeof(Button); break;
                            default: throw new System.Exception("Attribute name is invalid.");
                        }
                        addnode.AddComponent(oComponentType);
                        var component_obj = addnode.GetComponent(oComponentType);
                        UIUtil.SetProperty(component_obj, component);
                    } catch (System.Exception ex) {
                        Debug.Log(ex.Message + "\n" + ex.StackTrace);
                    }
                }


            }
            Debug.Log(doc.ToString());

            return dcGO;
        }

        /// <summary>
        /// イベントシステム取得<br />
        /// すでにイベントシステムが存在する場合はそれの gameObject を返し、存在しない場合は生成する
        /// </summary>
        /// <returns>イベントシステム gameObject</returns>
        public static GameObject MedamaUIGetEventSystem(this GameObject node) {
            return UIUtil.GetEventSystem();
        }

        /// <summary>
        /// キャンバス取得<br />
        /// すでにキャンバスが存在する場合はそれの gameObject を返し、存在しない場合は生成する
        /// </summary>
        /// <returns>キャンバス gameObject</returns>
        public static GameObject MedamaUIGetCanvas(this GameObject node) {
            GameObject canvas_object;
            var canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
            if (canvas == null) {
                canvas_object = new GameObject("Canvas");

                canvas_object.AddComponent<Canvas>();
                canvas_object.AddComponent<CanvasScaler>();
                canvas_object.AddComponent<GraphicRaycaster>();

                canvas = canvas_object.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            } else {
                canvas_object = canvas.gameObject;
            }
            return canvas_object;
        }

        /// <summary>
        /// UIノード追加
        /// </summary>
        /// <param name="parent">UI親ノード（省略でCanvas）</param>
        /// <param name="name">ノード名（省略でGUID）</param>
        /// <param name="anchorMin">anchorMin（省略で 0, 0）</param>
        /// <param name="anchorMax">anchorMax（省略で 0, 0）</param>
        /// <param name="anchoredPosition">anchoredPosition（省略で 0, 0）</param>
        /// <param name="sizeDelta">sizeDelta（省略で 100, 100）</param>
        /// <param name="pivot">pivot（省略で 0, 0）</param>
        /// <returns>追加されたノード gameObject</returns>
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
            LayoutType pivot = LayoutType.NoUse
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

            return string.IsNullOrEmpty(sprite)
                ? parent.MedamaUIAddNode(name, anchorMin, anchorMax, anchoredPosition, sizeDelta, pivot_set)
                : parent.MedamaUIAddNode(name, anchorMin, anchorMax, anchoredPosition, sizeDelta, pivot_set).MedamaUISetCanvasRender().MedamaUISetImage(UIUtil.LoadSprite(sprite), spritetype);
        }

        /// <summary>
        /// UIノード追加
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="anchorMin"></param>
        /// <param name="anchorMax"></param>
        /// <param name="anchoredPosition"></param>
        /// <param name="sizeDelta"></param>
        /// <param name="pivot"></param>
        /// <returns></returns>
        public static GameObject MedamaUIAddNode(this GameObject parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta, Vector2 pivot) {
            name = string.IsNullOrEmpty(name) ? Guid.NewGuid().ToString() : name;
            GameObject node = new GameObject(name);
            node.AddComponent<RectTransform>();
            node.transform.SetParent(parent.transform);
            var rect = node.GetComponent<RectTransform>();
            rect.anchoredPosition = anchoredPosition;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.sizeDelta = sizeDelta;
            rect.pivot = pivot;
            return node;
        }


        /// <summary>
        /// UIノード追加
        /// </summary>
        /// <param name="parent">UI親ノード</param>
        /// <param name="name">ノード名（省略でGUID）</param>
        /// <param name="anchorMin">anchorMin（省略で 0, 0）</param>
        /// <param name="anchorMax">anchorMax（省略で 0, 0）</param>
        /// <param name="anchoredPosition">anchoredPosition（省略で 0, 0）</param>
        /// <param name="sizeDelta">sizeDelta（省略で 100, 100）</param>
        /// <param name="pivot">pivot（省略で 0, 0）</param>
        /// <returns>追加されたノード gameObject</returns>
        public static GameObject UIAddNode(
            this GameObject parent,
            string name = "",
            Nullable<Vector2> anchorMin = null,
            Nullable<Vector2> anchorMax = null,
            Nullable<Vector2> anchoredPosition = null,
            Nullable<Vector2> sizeDelta = null,
            Nullable<Vector2> pivot = null,
            LayoutType layoutType = LayoutType.NoUse,
            float up = 0,
            float down = 0,
            float left = 0,
            float right = 0,
            float scrollBarWidwh = 20,
            float scrollBarHeight = 20
        ) {
            switch (layoutType) {
                case LayoutType.NoUse:
                    break;
                case LayoutType.ScreenFit:
                    sizeDelta = new Vector2(Screen.width - (left + right), Screen.height - (up + down));
                    break;
                case LayoutType.ScreenFitWithScrollBarBoth:
                    sizeDelta = new Vector2(Screen.width - (left + right + scrollBarWidwh), Screen.height - (up + down + scrollBarHeight));
                    break;
                case LayoutType.ParentFit:
                    anchorMin = Vector2.zero;
                    anchorMax = Vector2.one;
                    break;
                case LayoutType.ParentFitWithScrollBarBoth:
                    anchorMin = Vector2.zero;
                    anchorMax = Vector2.one;
                    sizeDelta = new Vector2(-scrollBarWidwh * 2, -scrollBarHeight * 2);
                    break;
            }
            return UIUtil.AddNode(parent, name, anchorMin, anchorMax, anchoredPosition, sizeDelta, pivot);
        }

        /// <summary>
        /// Imageコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <param name="sprite">スプライトオブジェクト</param>
        /// <param name="type">イメージ表示タイプ</param>
        public static GameObject MedamaUISetImage(
            this GameObject node,
            Sprite sprite = null,
            Image.Type type = Image.Type.Simple
        ) {
            return UIUtil.SetImage(node, sprite, type);
        }

        /// <summary>
        /// CanvasRenderコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        public static GameObject MedamaUISetCanvasRender(
            this GameObject node
        ) {
            return UIUtil.SetCanvasRender(node);
        }

        /// <summary>
        /// Maskコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <param name="showMaskGraphic">マスク画像表示</param>
        public static GameObject MedamaUISetMask(
            this GameObject node,
            bool showMaskGraphic = true
        ) {
            var mask = node.GetComponent<Mask>();
            if (mask == null) {
                node.AddComponent<Mask>();
                mask = node.GetComponent<Mask>();
            }
            mask.showMaskGraphic = showMaskGraphic;

            return node;
        }

        /// <summary>
        /// ScrollRectコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <param name="horizontal">横スクロール</param>
        /// <param name="vertical">縦スクロール</param>
        /// <param name="movementType">スクロールタイプ</param>
        /// <returns>適用後 gameObject</returns>
        public static GameObject MedamaUISetScrollRect(
            this GameObject node,
            bool horizontal = true,
            bool vertical = true,
            ScrollRect.MovementType movementType = ScrollRect.MovementType.Elastic
        ) {
            var scroll_rect = node.GetComponent<ScrollRect>();
            if (scroll_rect == null) {
                node.AddComponent<ScrollRect>();
                scroll_rect = node.GetComponent<ScrollRect>();
            }
            scroll_rect.horizontal = horizontal;
            scroll_rect.vertical = vertical;
            scroll_rect.movementType = movementType;

            return node;
        }

        /// <summary>
        /// HorizontalLayoutGroupコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <param name="childAlignment"></param>
        /// <param name="childForceExpandWidth"></param>
        /// <param name="childForceExpandHeight"></param>
        /// <returns>適用後 gameObject</returns>
        public static GameObject UISetHorizontalLayoutGroup(
            this GameObject node,
            UnityEngine.TextAnchor childAlignment = UnityEngine.TextAnchor.MiddleCenter,
            float spacing = 0,
            bool childForceExpandWidth = false,
            bool childForceExpandHeight = false
        ) {
            return UIUtil.SetHorizontalLayoutGroup(node, childAlignment, spacing, childForceExpandWidth, childForceExpandHeight);
        }

        /// <summary>
        /// VerticalLayoutGroupコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <param name="childAlignment"></param>
        /// <param name="childForceExpandWidth"></param>
        /// <param name="childForceExpandHeight"></param>
        /// <returns>適用後 gameObject</returns>
        public static GameObject MedamaUISetVerticalLayoutGroup(
            this GameObject node,
            UnityEngine.TextAnchor childAlignment = UnityEngine.TextAnchor.MiddleCenter,
            float spacing = 0,
            bool childForceExpandWidth = false,
            bool childForceExpandHeight = false
        ) {
            var layout = node.GetComponent<VerticalLayoutGroup>();
            if (layout == null) {
                node.AddComponent<VerticalLayoutGroup>();
                layout = node.GetComponent<VerticalLayoutGroup>();
            }
            layout.childAlignment = childAlignment;
            layout.spacing = spacing;
            layout.childForceExpandWidth = childForceExpandWidth;
            layout.childForceExpandHeight = childForceExpandHeight;

            return node;
        }

        /// <summary>
        /// ContentSizeFitterコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <param name="horizontalFit"></param>
        /// <param name="verticalFit"></param>
        /// <returns>適用後 gameObject</returns>
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
            var filter = node.GetComponent<ContentSizeFitter>();
            if (filter == null) {
                node.AddComponent<ContentSizeFitter>();
                filter = node.GetComponent<ContentSizeFitter>();
            }

            filter.horizontalFit = horizontalFit;
            filter.verticalFit = verticalFit;

            return node;
        }

        /// <summary>
        /// 親ScrollRectにContentノード設定
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
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
        /// LayoutElementコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <returns>適用後 gameObject</returns>
        public static GameObject MedamaUISetLayoutElement(
            this GameObject node,
            float minWidth = 16,
            float minHeight = 16
        ) {
            var element = node.GetComponent<LayoutElement>();
            if (element == null) {
                node.AddComponent<LayoutElement>();
                element = node.GetComponent<LayoutElement>();
            }

            element.minWidth = minWidth;
            element.minHeight = minHeight;

            return node;
        }

        /// <summary>
        /// Textコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <returns>適用後 gameObject</returns>
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
                font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            var text = node.GetComponent<Text>();
            if (text == null) {
                text = node.AddComponent<Text>();
            }

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
        /// InputField コンポーネント登録
        /// </summary>
        /// <param name="node"></param>
        /// <param name="textPlaceholder"></param>
        /// <param name="fontSize"></param>
        /// <param name="textColor"></param>
        /// <param name="placeHolderColor"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
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
            var inputField = node.GetComponent<InputField>();
            if (inputField == null) {
                inputField = node.AddComponent<InputField>();
            }

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
        /// Buttonコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <returns>適用後 gameObject</returns>
        public static GameObject MedamaUISetButton(
            this GameObject node,
            string textButton = "",
            Color? colorButton = null
        ) {
            var button = node.GetComponent<Button>();
            if (button == null) {
                button = node.AddComponent<Button>();
            }
            var image = node.GetComponent<Image>();
            if (image != null) {
                button.targetGraphic = image;
            }

            var text = node.MedamaUIAddNode("Text", LayoutType.StretchStretch, 0, 0, 0, 0, 0, 0, "", Image.Type.Simple)
                .MedamaUISetText(textButton, colorButton);

            return node;
        }

        /// <summary>
        /// Scrollbarコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <returns>適用後 gameObject</returns>
        public static GameObject MedamaUISetScrollbar(
            this GameObject node,
            GameObject scrollview = null,
            bool Horizontal = false,
            bool Vertical = false
        ) {

            var scrollbar = node.GetComponent<Scrollbar>();
            if (scrollbar == null) {
                scrollbar = node.AddComponent<Scrollbar>();
            }

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
        /// ScrollbarコンポーネントにHandleのRectTransformを登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <returns>適用後 gameObject</returns>
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
        /// ScrollbarコンポーネントにHandleのRectTransformを登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <returns>適用後 gameObject</returns>
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
        /// EventTriggerコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <returns>適用後 gameObject</returns>
        public static GameObject UISetEventTrigger(
            this GameObject node,
            List<EventSet> lsEventSet = null
        ) {
            return UIUtil.SetEventTrigger(node, lsEventSet);
        }

    }
}
