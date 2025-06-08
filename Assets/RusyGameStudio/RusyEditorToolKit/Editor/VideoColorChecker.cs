using UnityEngine;
using UnityEditor;

namespace RusyGameStudio.Tools
{
    public class VideoColorChecker : EditorWindow
    {
        [MenuItem("RusyEditorToolKit/Video Color Checker")]
        static void Open()
        {
            var window = GetWindow<VideoColorChecker>();
            window.titleContent = new GUIContent("Video Color Checker");
        }
        private void Update()
        {
            if (!EditorApplication.isPlaying) return;
            Repaint();
        }

        private void OnGUI()
        {
            if (!EditorApplication.isPlaying) return;

            for (int i = 0; i < VideoColorMaster.ColorCount; i++)
            {
                EditorGUILayout.ColorField($"Color {i}", VideoColorMaster.GetColor(i));
            }
        }
    }
}