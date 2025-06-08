using UnityEngine;
using UnityEditor;

namespace RusyGameStudio.Tools
{
    public class ObjectRenameAssistant : EditorWindow
    {
        private string prefixName = default;
        private int startFrom = 1;
        private bool fixedDigits = false;
        private int fixedDigitCount = 1;
        
        [MenuItem("RusyEditorToolKit/Object Rename Assistant")]
        [MenuItem("GameObject/Rusy Editor Tool Kit/Object Rename Assistant", false, 0)]
        static void Open()
        {
            var window = GetWindow<ObjectRenameAssistant>();
            window.titleContent = new GUIContent("Object Rename Assistant");
        }

        private void OnGUI()
        {
            CustomGUILayout.Title("Object Rename Assistant", this);


            EditorGUILayout.LabelField("Settings", CustomGUIStyles.mainLabel);
            EditorGUILayout.Space(4);
            prefixName = EditorGUILayout.TextField("Prefix Name", prefixName);
            startFrom = EditorGUILayout.IntField("Start From", startFrom);
            fixedDigits = EditorGUILayout.Toggle("Fixed Digits", fixedDigits);
            if (fixedDigits) fixedDigitCount = EditorGUILayout.IntSlider("Fixed Digit Count", fixedDigitCount, 1, 8);

            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.LabelField(
                    "Turning Fixed Digits Settings ON, fixed the number of digits to an arbitrary value." +
                    "When OFF, it matches the number of objects in the selection.",
                    CustomGUIStyles.explanation);
            }
            EditorGUILayout.EndVertical();

            CustomGUILayout.Divider(this);

            EditorGUILayout.LabelField("Expected Name", CustomGUIStyles.mainLabel);
            EditorGUILayout.Space(4);
            if (fixedDigits) EditorGUILayout.LabelField(prefixName + startFrom.ToString($"D{fixedDigitCount}"));
            else             EditorGUILayout.LabelField(prefixName + startFrom.ToString($"D{Digit(Selection.gameObjects.Length)}"));

            CustomGUILayout.Divider(this);

            if (CustomGUILayout.ButtonBlueLarge("Rename")) Rename();

        }

        private void Rename()
        {
            if (Selection.gameObjects.Length == 0)
            {
                Debug.LogError("Please select objects");
                return;
            }

            int number = startFrom;
            int digits = Digit(Selection.gameObjects.Length);

            if (fixedDigits)
            {
                if (digits > fixedDigitCount)
                {
                    Debug.LogError("Please set the Fixed Digit Count value to more than " +
                        "the number of digits of the number of objects in the selection.");
                    return;
                }

                foreach (var item in Selection.gameObjects)
                {
                    item.name = prefixName + number.ToString($"D{fixedDigitCount}");
                    number++;
                }
            }
            else
            {
                foreach(var item in Selection.gameObjects)
                {
                    item.name = prefixName + number.ToString($"D{digits}");
                    number++;
                }
            }
        }

        public int Digit(int num) => (num == 0) ? 1 : ((int)Mathf.Log10(num) + 1);
    }
}