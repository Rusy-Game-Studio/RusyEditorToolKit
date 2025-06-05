using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace RusyGameStudio.Tools
{
    public class AutoObjectPositioner : EditorWindow
    {
        // Parameters on the window
        private SplineContainer baseSpline = default;
        private Transform instanceRoot = default;
        private Vector3 positionOffset = Vector3.zero;
        private Vector3 rotationOffset = Vector3.zero;
        private Vector3 scaleOffset = Vector3.zero;
        private int objectCount = 0;
        private float distance = 0f;
        private bool randomize = false;
        private bool lookForward = true;
        private List<GameObject> instanceObjects = new List<GameObject>();
        // Hidden variables
        private List<GameObject> objectList = new List<GameObject>();
        private bool isCountMode = true;
        private Vector3 lookat;
        private int prevCount;
        private float splineLength;
        private float remainder;

        private const int RANDOMIZEMAXCOUNT = 10;
        private const int WIDTH  = 450;
        private const int HEIGHT = 506;
        private Color COLORON  = new Color(0.3f, 0.8f, 1.0f);
        private Color COLOROFF = new Color(0.8f, 0.8f, 0.8f);


        [MenuItem("Rusy Game Studio/Auto Object Positioner")]
        static void Open()
        {
            var window = GetWindow<AutoObjectPositioner>();
            window.titleContent = new GUIContent("Auto Object Positioner");
        }

        private void OnGUI()
        {
            int addHeightA = baseSpline != null ? 20 : 0;
            int addHeightB = instanceObjects.Count * 20;
            int addHeightC = instanceObjects.Count > 1 ? 20 : 0;
            minSize = new Vector2(WIDTH, HEIGHT + addHeightA + addHeightB + addHeightC);
            maxSize = new Vector2(WIDTH, HEIGHT + addHeightA + addHeightB + addHeightC);

            #region GUIStyles
            GUIStyle title_A = new GUIStyle();
            title_A.normal.textColor = Color.white;
            title_A.fontSize = 20;
            title_A.fontStyle = FontStyle.Bold;
            title_A.wordWrap = false;
            title_A.alignment = TextAnchor.MiddleCenter;
            GUIStyle content_A = new GUIStyle();
            content_A.normal.textColor = Color.white;
            content_A.wordWrap = true;
            #endregion

            {// Spline -------------------------
                EditorGUILayout.BeginVertical("Box");
                {// Divider ------------------------------
                    var splitterRect = EditorGUILayout.GetControlRect(false, GUILayout.Height(1));
                    splitterRect.x = 25;
                    splitterRect.width = position.width - 50;
                    EditorGUI.DrawRect(splitterRect, Color.white);
                }// Divider ------------------------------
                EditorGUILayout.LabelField("Base References", title_A);
                {// Divider ------------------------------
                    var splitterRect = EditorGUILayout.GetControlRect(false, GUILayout.Height(1));
                    splitterRect.x = 25;
                    splitterRect.width = position.width - 50;
                    EditorGUI.DrawRect(splitterRect, Color.white);
                }// Divider ------------------------------
                EditorGUILayout.Space(4);
                baseSpline = (SplineContainer)EditorGUILayout.ObjectField("Spline", baseSpline, typeof(SplineContainer), true);
                instanceRoot = (Transform)EditorGUILayout.ObjectField("Instance Root", instanceRoot, typeof(Transform), true);
                if (baseSpline != null)
                {
                    splineLength = baseSpline.CalculateLength();
                    EditorGUILayout.LabelField(" - Spline Length", $"{splineLength}");
                }
                EditorGUILayout.EndVertical();
            }// Spline -------------------------

            EditorGUILayout.Space(4);

            {// Prefab -------------------------
                EditorGUILayout.BeginVertical("Box");
                {// Divider ------------------------------
                    var splitterRect = EditorGUILayout.GetControlRect(false, GUILayout.Height(1));
                    splitterRect.x = 25;
                    splitterRect.width = position.width - 50;
                    EditorGUI.DrawRect(splitterRect, Color.white);
                }// Divider ------------------------------
                EditorGUILayout.LabelField("Prefabs", title_A);
                {// Divider ------------------------------
                    var splitterRect = EditorGUILayout.GetControlRect(false, GUILayout.Height(1));
                    splitterRect.x = 25;
                    splitterRect.width = position.width - 50;
                    EditorGUI.DrawRect(splitterRect, Color.white);
                }// Divider ------------------------------
                EditorGUILayout.Space(4);

                EditorGUILayout.BeginVertical("Box");
                {
                    EditorGUILayout.LabelField("Offset Settings", content_A);

                    EditorGUI.BeginChangeCheck();
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(" - Position Offset");
                        positionOffset = EditorGUILayout.Vector3Field("", positionOffset);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(" - Rotation Offset");
                        rotationOffset = EditorGUILayout.Vector3Field("", rotationOffset);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(" - Scale Offset");
                        scaleOffset = EditorGUILayout.Vector3Field("", scaleOffset);
                        EditorGUILayout.EndHorizontal();
                    }
                    if (EditorGUI.EndChangeCheck()) Replace();

                    EditorGUILayout.Space(2);

                    EditorGUILayout.LabelField("Look At Settings", content_A);
                    
                    EditorGUI.BeginChangeCheck();
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(" - Look Forward");
                        if (GUILayout.Button(lookForward ? "✓" : "", GUILayout.Width(20))) lookForward = true;
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(" - Look World Up");
                        if (GUILayout.Button(lookForward ? "" : "✓", GUILayout.Width(20))) lookForward = false;
                        EditorGUILayout.EndHorizontal();
                    }
                    if (EditorGUI.EndChangeCheck()) Replace();

                    EditorGUILayout.Space(4);

                    EditorGUILayout.BeginHorizontal();
                    using (new BackgroundColorScope(isCountMode ? COLORON : COLOROFF))
                    {
                        if (GUILayout.Button("Count mode", GUILayout.Width(position.width / 2 - 10))) isCountMode = true;
                    }
                    using (new BackgroundColorScope(!isCountMode ? COLORON : COLOROFF))
                    {
                        if (GUILayout.Button("Distance mode", GUILayout.Width(position.width / 2 - 10))) isCountMode = false;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.BeginChangeCheck();
                    {
                        if (isCountMode)
                        {
                            objectCount = EditorGUILayout.IntField("Object Count", objectCount);
                            if (objectCount < 1) objectCount = 1;

                            if (objectCount < 2) distance = float.PositiveInfinity;
                            else distance = splineLength / (objectCount - 1);
                            EditorGUILayout.LabelField("Distance", $"{distance}");

                            EditorGUILayout.LabelField("Remainder", $"{remainder = 0f}");
                        }
                        else
                        {
                            objectCount = Divisors(splineLength, distance, out remainder);
                            EditorGUILayout.LabelField("Object count", $"{objectCount}");

                            distance = EditorGUILayout.FloatField("Distance", distance);
                            if (distance < 0.01f) distance = 0.01f;
                            else if (distance > splineLength) distance = splineLength;

                            EditorGUILayout.LabelField("Remainder", $"{remainder}");
                        }
                    }
                    if (EditorGUI.EndChangeCheck()) Replace();
                }
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.BeginVertical("Box");
                {
                    EditorGUILayout.LabelField("Instancing References", content_A);
                    
                    if (instanceObjects.Count > 1) randomize = EditorGUILayout.Toggle("Randomize", randomize);

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("+", GUILayout.Width(position.width / 2 - 10)))
                        if (instanceObjects.Count < RANDOMIZEMAXCOUNT) instanceObjects.Add(null);
                    if (GUILayout.Button("-", GUILayout.Width(position.width / 2 - 10))) 
                        if (instanceObjects.Count > 0) instanceObjects.RemoveAt(instanceObjects.Count - 1);
                    EditorGUILayout.EndHorizontal();
                    
                    for (int i = 0; i < instanceObjects.Count; i++)
                        instanceObjects[i] = (GameObject)EditorGUILayout.ObjectField($"Prefab {i}", instanceObjects[i], typeof(GameObject), true);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndVertical();
            }// Prefab -------------------------

            EditorGUILayout.Space(4);

            {// Generate -------------------------
                EditorGUILayout.BeginVertical("Box");
                {// Divider ------------------------------
                    var splitterRect = EditorGUILayout.GetControlRect(false, GUILayout.Height(1));
                    splitterRect.x = 25;
                    splitterRect.width = position.width - 50;
                    EditorGUI.DrawRect(splitterRect, Color.white);
                }// Divider ------------------------------
                EditorGUILayout.LabelField("Generate", title_A);
                {// Divider ------------------------------
                    var splitterRect = EditorGUILayout.GetControlRect(false, GUILayout.Height(1));
                    splitterRect.x = 25;
                    splitterRect.width = position.width - 50;
                    EditorGUI.DrawRect(splitterRect, Color.white);
                }// Divider ------------------------------
                EditorGUILayout.Space(4);
                if (GUILayout.Button("Generate Objects")) GenerateObjects();
                if (GUILayout.Button("Reflesh Objects List")) objectList.Clear();
                EditorGUILayout.EndVertical();
            }// Generate -------------------------
        }

        private void GenerateObjects()
        {
            #region Check Resources
            if (baseSpline == null)
            {
                Debug.LogError("Please set base spline");
                return;
            }
            if (instanceObjects.Count <= 0)
            {
                Debug.LogError("Please set instance objects");
                return;
            }
            foreach (GameObject item in instanceObjects) 
            {
                if (item == null)
                {
                    Debug.LogError("Please set instance objects");
                    return;
                }
            }
            #endregion


            // Destroy objects
            if (objectList.Count != 0) foreach (GameObject obj in objectList) DestroyImmediate(obj);
            objectList.Clear();

            // Set parent
            Transform root = baseSpline.transform;
            if (instanceRoot != null) root = instanceRoot;

            // Generate objects
            float interval = (1f - (remainder / splineLength)) / ((float)objectCount - 1f);
            for (int i = 0; i < objectCount; i++)
            {
                int n = i % instanceObjects.Count;
                if (randomize) n = UnityEngine.Random.Range(0, instanceObjects.Count);

                GameObject obj = PrefabUtility.InstantiatePrefab(instanceObjects[n], root) as GameObject;
                float evaluate = interval * i;

                ObjectTransformation(obj, evaluate);
                objectList.Add(obj);
            }
        }

        private void Replace()
        {
            if (objectList.Count == 0) return;

            float interval = (1f - (remainder / splineLength)) / ((float)objectCount - 1f);
            for (int i = 0; i < objectList.Count; i++)
            {
                float evaluate = interval * i;
                ObjectTransformation(objectList[i], evaluate);
            }

            if (objectCount != prevCount) GenerateObjects();
            prevCount = objectCount;
        }

        private void ObjectTransformation(GameObject obj, float position)
        {
            baseSpline.Evaluate(position, out var pos, out var tan, out var up);
            obj.transform.position = pos;

            if (lookForward)
            {
                var rotation = quaternion.LookRotationSafe(tan, up);
                obj.transform.rotation = rotation;
                obj.transform.eulerAngles += rotationOffset;
            }
            else
            {
                lookat.x = obj.transform.position.x;
                lookat.y = obj.transform.position.y + 1f;
                lookat.z = obj.transform.position.z;

                obj.transform.LookAt(lookat);
                obj.transform.eulerAngles += new Vector3(90, 0, 0);
                obj.transform.eulerAngles += rotationOffset;
            }

            obj.transform.localPosition += positionOffset;

            obj.transform.localScale = Vector3.one;
            obj.transform.localScale += scaleOffset;
        }

        private int Divisors(float length, float distance, out float remainder)
        {
            float result = length / distance;
            int count = Mathf.FloorToInt(result);
            remainder = distance * (result - count);
            return count + 1;
        }
    }
}