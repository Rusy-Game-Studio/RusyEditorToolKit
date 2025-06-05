using UnityEngine;

namespace RusyGameStudio.Tools
{
    public class CustomGUIStyles
    {
        //==== Text Label Styles ==================================================================
        public static GUIStyle mainLabel = new GUIStyle()
        {
            fontSize        = 20,
            fontStyle       = FontStyle.Bold,
            alignment       = TextAnchor.MiddleCenter,
            wordWrap        = false,
            normal          = new GUIStyleState() { textColor = new Color(0.90f, 1.00f, 1.00f) },
        };
        public static GUIStyle explanation = new GUIStyle()
        {
            fontSize        = 12,
            fontStyle       = FontStyle.Normal,
            alignment       = TextAnchor.UpperLeft,
            wordWrap        = true,
            normal          = new GUIStyleState() { textColor = new Color(0.50f, 0.50f, 0.55f) },
        };

        //==== Button Label Styles ================================================================
        public static GUIStyle buttonBlueLarge = new GUIStyle(GUI.skin.button)
        {
            fontSize        = 18,
            fontStyle       = FontStyle.Bold,
            alignment       = TextAnchor.MiddleCenter,
            wordWrap        = false,
            normal          = new GUIStyleState() { textColor = new Color(0.35f, 0.35f, 0.40f) },
            active          = new GUIStyleState() { textColor = new Color(0.95f, 0.95f, 1.00f) },
            hover           = new GUIStyleState() { textColor = new Color(0.50f, 0.55f, 0.55f) },
        };
        public static GUIStyle buttonBlueSmall = new GUIStyle(GUI.skin.button)
        {
            fontSize        = 14,
            fontStyle       = FontStyle.Normal,
            alignment       = TextAnchor.MiddleCenter,
            wordWrap        = false,
            normal          = new GUIStyleState() { textColor = new Color(0.35f, 0.35f, 0.40f) },
            active          = new GUIStyleState() { textColor = new Color(0.95f, 0.95f, 1.00f) },
            hover           = new GUIStyleState() { textColor = new Color(0.50f, 0.55f, 0.55f) },
        };
        public static GUIStyle ConnectBtnA = new GUIStyle(GUI.skin.button)
        {
            fontSize        = 18,
            fontStyle       = FontStyle.Bold,
            alignment       = TextAnchor.MiddleCenter,
            wordWrap        = false,
            normal          = new GUIStyleState() { textColor = new Color(0.20f, 0.30f, 0.20f) },
            active          = new GUIStyleState() { textColor = new Color(0.95f, 0.95f, 1.00f) },
            hover           = new GUIStyleState() { textColor = new Color(0.40f, 0.35f, 0.10f) },
        };
        public static GUIStyle ConnectBtnB = new GUIStyle(GUI.skin.button)
        {
            fontSize        = 18,
            fontStyle       = FontStyle.Bold,
            alignment       = TextAnchor.MiddleCenter,
            wordWrap        = false,
            normal          = new GUIStyleState() { textColor = new Color(0.30f, 0.20f, 0.10f) },
            active          = new GUIStyleState() { textColor = new Color(0.95f, 0.95f, 1.00f) },
            hover           = new GUIStyleState() { textColor = new Color(0.40f, 0.35f, 0.10f) },
        };
    }
}