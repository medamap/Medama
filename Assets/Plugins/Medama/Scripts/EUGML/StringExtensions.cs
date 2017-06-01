using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Medama.EUGML {

    public static partial class StringExtensions {

        /// <summary>
        /// Convert string to Color
        /// </summary>
        /// <param name="value">black|blue|clear|cyan|gray|green|grey|magenta|red|white|yellow|(r,g,b)|(r,g,b,a)</param>
        /// <returns></returns>
        public static Color ToColor(
            this string value
        ) {
            Color retValue = Color.white;
            switch (value.ToLower()) {
                case "black": retValue = Color.black; break;
                case "blue": retValue = Color.blue; break;
                case "clear": retValue = Color.clear; break;
                case "cyan": retValue = Color.cyan; break;
                case "gray": retValue = Color.gray; break;
                case "green": retValue = Color.green; break;
                case "grey": retValue = Color.grey; break;
                case "magenta": retValue = Color.magenta; break;
                case "red": retValue = Color.red; break;
                case "white": retValue = Color.white; break;
                case "yellow": retValue = Color.yellow; break;
                default:
                    var vector = GetVector(value);
                    switch (vector.Length) {
                        case 3: retValue = new Color(vector[0], vector[1], vector[2]); break;
                        case 4: retValue = new Color(vector[0], vector[1], vector[2], vector[3]); break;
                        default: throw new System.Exception("Invalid Color source format.");
                    }
                    break;
            }
            return retValue;
        }

        /// <summary>
        /// Convert string to Image.Type
        /// </summary>
        /// <param name="value">filled|simple|sliced|tiled</param>
        /// <returns></returns>
        public static Image.Type ToImageType(
            this string value
        ) {
            Image.Type retValue = Image.Type.Simple;

            switch (value.ToLower()) {
                case "filled": retValue = Image.Type.Filled; break;
                case "simple": retValue = Image.Type.Simple; break;
                case "sliced": retValue = Image.Type.Sliced; break;
                case "tiled": retValue = Image.Type.Tiled; break;
                default: retValue = Image.Type.Simple; break;
            }
            return retValue;
        }

        /// <summary>
        /// Convert string to Vector2
        /// </summary>
        /// <param name="value">zero|up|right|one|(x,y)</param>
        /// <returns></returns>
        public static Vector2 ToVector2(
            this string value
        ) {
            Vector2 retValue = Vector2.zero;

            switch (value) {
                case "zero": retValue = Vector2.zero; break;
                case "up": retValue = Vector2.up; break;
                case "right": retValue = Vector2.right; break;
                case "one": retValue = Vector2.one; break;
                case "center": retValue = new Vector2(0.5f, 0.5f); break;
                default:
                    var vector = GetVector(value);
                    if (vector.Length != 2) {
                        throw new System.Exception("Invalid Vector2 source format.");
                    }
                    retValue = new Vector2(vector[0], vector[1]);
                    break;
            }
            return retValue;
        }

        /// <summary>
        /// Convert string to Vector3
        /// </summary>
        /// <param name="value">zero|one|forward|back|left|right|up|down|(x,y,z)</param>
        /// <returns></returns>
        public static Vector3 ToVector3(
            this string value
        ) {
            Vector3 retValue = Vector3.zero;
            // zero one forward back left right up down
            switch (value) {
                case "zero": retValue = Vector3.zero; break;
                case "one": retValue = Vector3.one; break;
                case "up": retValue = Vector3.up; break;
                case "down": retValue = Vector3.down; break;
                case "left": retValue = Vector3.left; break;
                case "right": retValue = Vector3.right; break;
                case "forward": retValue = Vector3.forward; break;
                case "back": retValue = Vector3.back; break;
                default:
                    var vector = GetVector(value);
                    if (vector.Length != 3) {
                        throw new System.Exception("Invalid Vector3 source format.");
                    }
                    retValue = new Vector3(vector[0], vector[1], vector[2]);
                    break;
            }
            return retValue;
        }

        /// <summary>
        /// Convert float list to float array
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float[] GetVector(string value) {
            return value.Trim().Replace("(", "").Replace(")","").Replace(" ","").Split(',').
                Select(x => {
                    float fValue = 0;
                    float.TryParse(x, out fValue);
                    return fValue;
                }).ToArray();
        }

        /// <summary>
        /// Convert string to LayoutType
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LayoutType ToLayoutType(
            this string value
        ) {
            switch (value) {
                case "NoUse": return LayoutType.NoUse;
                case "ScreenFit": return LayoutType.ScreenFit;
                case "ScreenFitWithScrollBarBoth": return LayoutType.ScreenFitWithScrollBarBoth;
                case "ParentFit": return LayoutType.ParentFit;
                case "ParentFitWithScrollBarBoth": return LayoutType.ParentFitWithScrollBarBoth;
                case "TopLeft": return LayoutType.TopLeft;
                case "TopStretch": return LayoutType.TopStretch;
                case "TopRight": return LayoutType.TopRight;
                case "StretchLeft": return LayoutType.StretchLeft;
                case "StretchStretch": return LayoutType.StretchStretch;
                case "StretchRight": return LayoutType.StretchRight;
                case "BottomLeft": return LayoutType.BottomStretch;
                case "BottomStretch": return LayoutType.BottomStretch;
                case "BottomRight": return LayoutType.BottomRight;
                case "TopCenter": return LayoutType.TopCenter;
                case "BottomCenter": return LayoutType.BottomCenter;
                case "CenterCenter": return LayoutType.CenterCenter;
                case "CenterLeft": return LayoutType.CenterLeft;
                case "CenterRight": return LayoutType.CenterRight;
                default: Debug.LogWarningFormat("Invalid Layout {0}", value); return LayoutType.NoUse;
            }
        }

        /// <summary>
        /// Convert string to TextAnchor
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TextAnchor ToTextAnchor(
            this string value
        ) {
            switch (value.ToLower()) {
                case "upperleft": return TextAnchor.UpperLeft;
                case "uppercenter": return TextAnchor.UpperCenter;
                case "upperright": return TextAnchor.UpperRight;
                case "middleleft": return TextAnchor.MiddleLeft;
                case "middlecenter": return TextAnchor.MiddleCenter;
                case "middleright": return TextAnchor.MiddleRight;
                case "lowerleft": return TextAnchor.LowerLeft;
                case "lowercenter": return TextAnchor.LowerCenter;
                case "lowerright": return TextAnchor.LowerRight;
                default: Debug.LogWarningFormat("Invalid TextAnchor {0}", value); return TextAnchor.MiddleCenter;
            }
        }

        /// <summary>
        /// Convert string to ScrollRect.MovementType
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ScrollRect.MovementType ToMovementType(
            this string value
        ) {
            switch (value.ToLower()) {
                case "clamped": return ScrollRect.MovementType.Clamped;
                case "elastic": return ScrollRect.MovementType.Elastic;
                case "unrestricted": return ScrollRect.MovementType.Unrestricted;
                default: Debug.LogWarningFormat("Invalid MovementType {0}", value); return ScrollRect.MovementType.Clamped;
            }
        }

        /// <summary>
        /// Convert string to ContentSizeFitter.FitMode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ContentSizeFitter.FitMode ToFitMode(
            this string value
        ) {
            switch (value.ToLower()) {
                case "minsize": return ContentSizeFitter.FitMode.MinSize;
                case "preferredsize": return ContentSizeFitter.FitMode.PreferredSize;
                case "unconstrained": return ContentSizeFitter.FitMode.Unconstrained;
                default: Debug.LogWarningFormat("Invalid FitMode {0}", value); return ContentSizeFitter.FitMode.PreferredSize;
            }
        }
    }
}