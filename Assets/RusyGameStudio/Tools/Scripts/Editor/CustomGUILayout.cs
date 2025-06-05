using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RusyGameStudio.Tools
{
    public class CustomGUILayout
    {
        private const float SIZE_L = 40;
        private const float SIZE_S = 30;
        private static Color BTN_LB = new Color(1.90f, 2.10f, 2.20f);
        private static Color BTN_DB = new Color(0.80f, 0.85f, 1.00f);
        private static Color BTN_LY = new Color(1.6f, 2.0f, 2.2f);
        private static Color BTN_RD = new Color(1.6f, 2.0f, 2.2f);
        private static Color DIV_BL = new Color(0.25f, 0.35f, 0.40f);
        private static void TitleLine(EditorWindow window, Color color)
        {
            var splitterRect = EditorGUILayout.GetControlRect(false, GUILayout.Height(2));
            splitterRect.x = 4;
            splitterRect.width = window.position.width - 8;
            EditorGUI.DrawRect(splitterRect, color);
        }

        public static bool ButtonBlueLarge(string label, float width = 0f)
        {
            using (new BackgroundColorScope(BTN_LB))
            {
                if (width == 0f) return GUILayout.Button(label, CustomGUIStyles.buttonBlueLarge, GUILayout.Height(SIZE_L));
                else return GUILayout.Button(label, CustomGUIStyles.buttonBlueLarge, GUILayout.Width(width), GUILayout.Height(SIZE_L));
            }
        }
        public static bool ButtonBlueSmall(string label, float width = 0f)
        {
            using (new BackgroundColorScope(BTN_LB))
            {
                if (width == 0f) return GUILayout.Button(label, CustomGUIStyles.buttonBlueSmall, GUILayout.Height(SIZE_S));
                else return GUILayout.Button(label, CustomGUIStyles.buttonBlueSmall, GUILayout.Width(width), GUILayout.Height(SIZE_S));
            }
        }
        public static bool ToggleBlueLarge(string label, bool active, float width = 0f)
        {
            using (new BackgroundColorScope(active ? BTN_LB : BTN_DB))
            {
                if (width == 0f) return GUILayout.Button(label, CustomGUIStyles.buttonBlueLarge, GUILayout.Height(SIZE_L));
                else return GUILayout.Button(label, CustomGUIStyles.buttonBlueLarge, GUILayout.Width(width), GUILayout.Height(SIZE_L));
            }
        }
        public static bool ToggleBlueSmall(string label, bool active, float width = 0f)
        {
            using (new BackgroundColorScope(active ? BTN_LB : BTN_DB))
            {
                if (width == 0f) return GUILayout.Button(label, CustomGUIStyles.buttonBlueSmall, GUILayout.Height(SIZE_S));
                else return GUILayout.Button(label, CustomGUIStyles.buttonBlueSmall, GUILayout.Width(width), GUILayout.Height(SIZE_S));
            }
        }


        public static void Title(string title, EditorWindow window)
        {
            EditorGUILayout.Space(10);
            TitleLine(window, new Color(DIV_BL.r, DIV_BL.g, DIV_BL.b, 0.25f));
            TitleLine(window, new Color(DIV_BL.r, DIV_BL.g, DIV_BL.b, 0.50f));
            TitleLine(window, new Color(DIV_BL.r, DIV_BL.g, DIV_BL.b, 0.75f));
            TitleLine(window, new Color(DIV_BL.r, DIV_BL.g, DIV_BL.b, 1.00f));
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField(title, CustomGUIStyles.mainLabel);
            EditorGUILayout.Space(2);
            TitleLine(window, new Color(DIV_BL.r, DIV_BL.g, DIV_BL.b, 1.00f));
            TitleLine(window, new Color(DIV_BL.r, DIV_BL.g, DIV_BL.b, 0.75f));
            TitleLine(window, new Color(DIV_BL.r, DIV_BL.g, DIV_BL.b, 0.50f));
            TitleLine(window, new Color(DIV_BL.r, DIV_BL.g, DIV_BL.b, 0.25f));
            EditorGUILayout.Space(10);
        }
        public static void Divider(EditorWindow window)
        {
            var splitterRect = EditorGUILayout.GetControlRect(false, GUILayout.Height(2));
            splitterRect.x = 4;
            splitterRect.width = window.position.width - 8;
            EditorGUI.DrawRect(splitterRect, DIV_BL);
        }
    }
}