using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using Object = UnityEngine.Object;


namespace Medama.EUGML {

    public static class UIUtil {

        public static readonly Vector2 top_left = new Vector2(0f, 1f);
        public static readonly Vector2 middle_left = new Vector2(0f, 0.5f);
        public static readonly Vector2 bottom_left = new Vector2(0f, 0f);
        public static readonly Vector2 top_center = new Vector2(0.5f, 1f);
        public static readonly Vector2 middle_center = new Vector2(0.5f, 0.5f);
        public static readonly Vector2 bottom_center = new Vector2(0.5f, 0f);
        public static readonly Vector2 top_right = new Vector2(1f, 1f);
        public static readonly Vector2 middle_right = new Vector2(1f, 0.5f);
        public static readonly Vector2 bottom_right = new Vector2(1f, 0f);
        private static readonly string _toplevel_resource = "___TLR___"; // Top Level Resource

        /// <summary>
        /// Built in resource Arial font
        /// </summary>
        public static Font arial = null;

        /// <summary>
        /// プロパティ設定
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <param name="node"></param>
        public static void SetProperty<T>(T component, XElement node) {
            foreach (var attribute in node.Attributes()) {
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
        /// イベントシステム取得<br />
        /// すでにイベントシステムが存在する場合はそれの gameObject を返し、存在しない場合は生成する
        /// </summary>
        /// <returns>イベントシステム gameObject</returns>
        public static GameObject GetEventSystem() {

            GameObject event_object;
            var ev = Object.FindObjectOfType<EventSystem>();
            if (ev == null) {
                event_object = new GameObject("EventSystem");
                event_object.AddComponent<EventSystem>();
                event_object.AddComponent<StandaloneInputModule>();
                event_object.AddComponent<StandaloneInputModule>();
            } else {
                event_object = ev.gameObject;
            }
            return event_object;
        }

        /// <summary>
        /// キャンバス取得<br />
        /// すでにキャンバスが存在する場合はそれの gameObject を返し、存在しない場合は生成する
        /// </summary>
        /// <returns>キャンバス gameObject</returns>
        public static GameObject GetCanvas() {
            GameObject canvas_object;
            var canvas = Object.FindObjectOfType<Canvas>();
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
        public static GameObject AddNode(
            GameObject parent = null,
            string name = "",
            Nullable<Vector2> anchorMin = null,
            Nullable<Vector2> anchorMax = null,
            Nullable<Vector2> anchoredPosition = null,
            Nullable<Vector2> sizeDelta = null,
            Nullable<Vector2> pivot = null
        ) {
            parent = parent ?? GetCanvas();
            if (string.IsNullOrEmpty(name)) {
                name = System.Guid.NewGuid().ToString();
            }
            GameObject node = new GameObject(name);
            node.AddComponent<RectTransform>();
            node.transform.SetParent(parent.transform);

            var rect = node.GetComponent<RectTransform>();
            rect.anchoredPosition = anchoredPosition ?? Vector2.zero;
            rect.anchorMin = anchorMin ?? middle_center;
            rect.anchorMax = anchorMax ?? middle_center;
            rect.sizeDelta = sizeDelta ?? Vector2.zero;
            rect.pivot = pivot ?? middle_center;

            return node;
        }

        /// <summary>
        /// Imageコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <param name="sprite">スプライトオブジェクト</param>
        /// <param name="type">イメージ表示タイプ</param>
        public static GameObject SetImage(
            GameObject node = null,
            Sprite sprite = null,
            Image.Type type = Image.Type.Simple
        ) {
            node = node ?? AddNode();

            var image = node.GetComponent<Image>();
            if (image == null) {
                node.AddComponent<Image>();
                image = node.GetComponent<Image>();
            }
            image.sprite = sprite;
            image.type = type;

            return node;
        }

        /// <summary>
        /// CanvasRenderコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        public static GameObject SetCanvasRender(
            GameObject node = null
        ) {
            node = node ?? AddNode();

            var canvas_render = node.GetComponent<CanvasRenderer>();
            if (canvas_render == null) {
                node.AddComponent<CanvasRenderer>();
                canvas_render = node.GetComponent<CanvasRenderer>();
            }

            return node;
        }

        /// <summary>
        /// Maskコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <param name="showMaskGraphic">マスク画像表示</param>
        public static GameObject SetMask(
            GameObject node = null,
            bool showMaskGraphic = true
        ) {
            node = node ?? AddNode();

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
        public static GameObject SetScrollRect(
            GameObject node = null,
            bool horizontal = true,
            bool vertical = true,
            ScrollRect.MovementType movementType = ScrollRect.MovementType.Elastic
        ) {
            node = node ?? AddNode();

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
        /// VerticalLayoutGroupコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <param name="childAlignment"></param>
        /// <param name="childForceExpandWidth"></param>
        /// <param name="childForceExpandHeight"></param>
        /// <returns>適用後 gameObject</returns>
        public static GameObject SetVerticalLayoutGroup(
            GameObject node = null,
            UnityEngine.TextAnchor childAlignment = UnityEngine.TextAnchor.MiddleCenter,
            float spacing = 0,
            bool childForceExpandWidth = false,
            bool childForceExpandHeight = false
        ) {
            node = node ?? AddNode();

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
        /// HorizontalLayoutGroupコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <param name="childAlignment"></param>
        /// <param name="childForceExpandWidth"></param>
        /// <param name="childForceExpandHeight"></param>
        /// <returns>適用後 gameObject</returns>
        public static GameObject SetHorizontalLayoutGroup(
            GameObject node = null,
            UnityEngine.TextAnchor childAlignment = UnityEngine.TextAnchor.MiddleCenter,
            float spacing = 0,
            bool childForceExpandWidth = false,
            bool childForceExpandHeight = false
        ) {
            node = node ?? AddNode();

            var layout = node.GetComponent<HorizontalLayoutGroup>();
            if (layout == null) {
                node.AddComponent<HorizontalLayoutGroup>();
                layout = node.GetComponent<HorizontalLayoutGroup>();
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
        public static GameObject SetContentSizeFitter(
            GameObject node = null,
            ContentSizeFitter.FitMode horizontalFit = ContentSizeFitter.FitMode.Unconstrained,
            ContentSizeFitter.FitMode verticalFit = ContentSizeFitter.FitMode.Unconstrained
        ) {
            node = node ?? AddNode();

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
        /// LayoutElementコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <returns>適用後 gameObject</returns>
        public static GameObject SetLayoutElement(
            GameObject node = null,
            float minWidth = 16,
            float minHeight = 16
        ) {
            node = node ?? AddNode();

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
        public static GameObject SetText(
            GameObject node = null,
            string Text = null,
            System.Nullable<Color> color = null,
            Font font = null,
            int fontsize = 14,
            UnityEngine.FontStyle fontStyle = UnityEngine.FontStyle.Normal,
            bool resizeTextForBestFit = false,
            int resizeTextMinSize = 10,
            int resizeTextMaxSize = 40,
            UnityEngine.TextAnchor alignment = UnityEngine.TextAnchor.MiddleCenter,
            bool supportRichText = true,
            UnityEngine.HorizontalWrapMode horizontalOverflow = UnityEngine.HorizontalWrapMode.Wrap,
            UnityEngine.VerticalWrapMode verticalOverflow = UnityEngine.VerticalWrapMode.Truncate,
            int lineSpacing = 1
        ) {
            node = node ?? AddNode();

            if (arial == null) {
                arial = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            var text = node.GetComponent<Text>();
            if (text == null) {
                node.AddComponent<Text>();
                text = node.GetComponent<Text>();
            }

            text.text = Text ?? "";
            text.color = (color != null) ? (Color)color : Color.black;
            text.font = font ?? arial;
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
        /// Scrollbarコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <returns>適用後 gameObject</returns>
        public static GameObject SetScrollbar(
            GameObject node = null,
            GameObject scrollview = null,
            bool Horizontal = false,
            bool Vertical = false
        ) {
            node = node ?? AddNode();

            var scrollbar = node.GetComponent<Scrollbar>();
            if (scrollbar == null) {
                node.AddComponent<Scrollbar>();
                scrollbar = node.GetComponent<Scrollbar>();
            }
            if (scrollview != null) {
                var scroll_rect = scrollview.GetComponent<ScrollRect>();
                if (scroll_rect != null) {
                    if (Horizontal) {
                        scroll_rect.horizontalScrollbar = scrollbar;
                        scrollbar.direction = Scrollbar.Direction.LeftToRight;
                    }
                    if (Vertical) {
                        scroll_rect.verticalScrollbar = scrollbar;
                        scrollbar.direction = Scrollbar.Direction.BottomToTop;
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
        public static GameObject SetHandleToScrollBar(
            GameObject node = null
        ) {
            if (node == null || node.transform.parent == null || node.transform.parent.parent == null) {
                return null;
            }

            var handle_rect = node.GetComponent<RectTransform>();
            if (handle_rect == null) {
                return null;
            }

            var parent = node.transform.parent.parent.gameObject;
            if (parent == null) {
                return null;
            }
            
            var scrollbar = parent.GetComponent<Scrollbar>();
            if (scrollbar != null) {
                scrollbar.handleRect = handle_rect;
            }

            return node;
        }

        /// <summary>
        /// Buttonコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <returns>適用後 gameObject</returns>
        public static GameObject SetButton(
            GameObject node = null
        ) {
            node = node ?? AddNode();

            var button = node.GetComponent<Button>();
            if (button == null) {
                node.AddComponent<Button>();
                button = node.GetComponent<Button>();
            }

            var image = node.GetComponent<Image>();
            if (image != null) {
                button.targetGraphic = image;
            }

            return node;
        }

        /// <summary>
        /// EventTriggerコンポーネント登録
        /// </summary>
        /// <param name="node">登録先 gameObject</param>
        /// <returns>適用後 gameObject</returns>
        public static GameObject SetEventTrigger(
            GameObject node = null,
            List<EventSet> lsEventSet = null
        ) {
            node = node ?? AddNode();

            var trigger = node.GetComponent<EventTrigger>();
            if (trigger == null) {
                node.AddComponent<EventTrigger>();
                trigger = node.GetComponent<EventTrigger>();
            }
//            if (trigger.delegates == null) {
//                trigger.delegates = new List<EventTrigger.Entry>();
//            }

            if (trigger != null && lsEventSet != null) {
                foreach (var oEventSet in lsEventSet) {
                    if (oEventSet.action1 != null) {
                        AddEventTrigger(trigger, oEventSet.action1, oEventSet.triggerType);
                    } else if (oEventSet.action2 != null) {
                        AddEventTrigger(trigger, oEventSet.action2, oEventSet.triggerType);
                    }
                }
            }
            return node;
        }

        /// <summary>
        /// イベントトリガー追加
        /// </summary>
        /// <param name="eventTrigger"></param>
        /// <param name="action"></param>
        /// <param name="triggerType"></param>
        public static void AddEventTrigger(EventTrigger eventTrigger, UnityAction action, EventTriggerType triggerType) {
            var trigger = new EventTrigger.TriggerEvent();
            trigger.AddListener((eventData) => action());
            EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };
//            eventTrigger.delegates.Add(entry);
        }

        /// <summary>
        /// イベントトリガー追加
        /// </summary>
        /// <param name="eventTrigger"></param>
        /// <param name="action"></param>
        /// <param name="triggerType"></param>
        public static void AddEventTrigger(EventTrigger eventTrigger, UnityAction<BaseEventData> action, EventTriggerType triggerType) {
            var trigger = new EventTrigger.TriggerEvent();
            trigger.AddListener((eventData) => action(eventData));
            EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };
//            eventTrigger.delegates.Add(entry);
        }

    }
}

