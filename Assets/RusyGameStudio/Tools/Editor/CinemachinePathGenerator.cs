using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;

namespace RusyGameStudio.Tools
{
    public class CinemachinePathGenerator : EditorWindow
    {
        #region Variables
        // Constant Variables ---------------------------------------------
        private Color BTNCOL_LIGHT = new Color(1.6f, 2.0f, 2.2f);
        private Color BTNCOL_DARK = new Color(0.8f, 0.85f, 1.0f);
        private Color DIVIDER_LIGHT = new Color(0.15f, 0.3f, 0.35f);
        private Color DIVIDER_DARK = new Color(0.1f, 0.15f, 0.2f);
        private Color CONNECT_LIGHT = new Color(1.6f, 2.2f, 1.3f);
        private Color CONNECT_DARK = new Color(2.0f, 1.8f, 1.2f);
        private const int TARGETMAX = 30;
        private const string COLORTAG_SG = "<color=#33BF33>　LOG : ";
        private const string COLORTAG_SY = "<color=#BFBF33>　CAUTION : ";
        private const string COLORTAG_SR = "<color=#BF3333>　ERROR : ";
        private const string COLORTAG_E = "</color>";
        // For Window Param -----------------------------------------------
        private Vector2 scrollPos = Vector2.zero;
        private string logText = default;
        private bool isConnecting = false;
        private int prevSmoothness = 0;
        private string dummyObjectPath = "Assets/_AKACore/Tools/Objects/DummySphere.prefab";
        // Main Variables -------------------------------------------------
        private CinemachineSmoothPath currentPath = default;
        private CinemachineSmoothPath.Waypoint[] sampleWaypoint = default;
        private int pathResolution = 20;
        private bool pathIsLooped = true;
        private Color pathVisColor = Color.green;
        private float pathVisWidth = 0.2f;
        private string pathObjectName = default;
        private Transform generateParent = default;
        private bool mode = false;
        // Settings for Object Mode ---------------------------------------
        private int targetCount = 5;
        private List<GameObject> targetSampler = new List<GameObject>();
        private List<GameObject> targets = new List<GameObject>();
        private Transform targetParent = default;
        private string targetPrefixName = default;
        // Settings for Spline Mode ---------------------------------------
        private SplineContainer baseSpline = default;
        private int smoothness = 10;
        private List<int> segmentSmoothness = new List<int>();
        private Transform splineParent = default;
        private string splineName = default;

        private bool separateSmoothness = true;
        // ----------------------------------------------------------------
        #endregion


        [MenuItem("Rusy Game Studio/Cinemachine Path Generator")]
        static void Open()
        {
            var window = GetWindow<CinemachinePathGenerator>();
            window.titleContent = new GUIContent("Cinemachine Path Generator");
            window.targetSampler.Clear();
            for (int i = 0; i < TARGETMAX; i++)
            {
                window.targetSampler.Add(null);
            }

            window.OnTargetsValueChanged();
        }

        private void OnGUI()
        {
            minSize = new Vector2(500, 684);
            #region GUI Styles
            GUIStyle ConnectBtnA = new GUIStyle(GUI.skin.button);
            {
                ConnectBtnA.normal.textColor = new Color(0.2f, 0.3f, 0.2f);
                ConnectBtnA.hover.textColor = new Color(0.4f, 0.35f, 0.1f);
                ConnectBtnA.fontSize = 18;
                ConnectBtnA.fontStyle = FontStyle.Bold;
                ConnectBtnA.wordWrap = false;
                ConnectBtnA.alignment = TextAnchor.MiddleCenter;
            }
            GUIStyle ConnectBtnB = new GUIStyle(GUI.skin.button);
            {
                ConnectBtnB.normal.textColor = new Color(0.3f, 0.2f, 0.1f);
                ConnectBtnB.hover.textColor = new Color(0.4f, 0.35f, 0.1f);
                ConnectBtnB.fontSize = 18;
                ConnectBtnB.fontStyle = FontStyle.Bold;
                ConnectBtnB.wordWrap = false;
                ConnectBtnB.alignment = TextAnchor.MiddleCenter;
            }
            GUIStyle LogStyle = new GUIStyle();
            {
                LogStyle.fontStyle = FontStyle.Bold;
            }
            #endregion


            CustomGUILayout.Title("Cinemachine Path Generator", this);
            CustomGUILayout.Divider(this);


            // == // Mode Select // ========================================================================================== //
            {
                EditorGUILayout.BeginVertical("Box");
                
                // == -- == -- == // Title Section // == -- == -- == //
                {
                    EditorGUILayout.Space(2);
                    EditorGUILayout.LabelField("Mode Select", CustomGUIStyles.mainLabel);
                    EditorGUILayout.Space(2);
                }
                // == -- == -- == // Title Section // == -- == -- == //

                // == -- ===== -- == // Buttons // == -- ===== -- == //
                {
                    EditorGUILayout.BeginHorizontal();
                    if (CustomGUILayout.ToggleBlueLarge("Object Mode",  mode)) mode = true;
                    if (CustomGUILayout.ToggleBlueLarge("Spline Mode", !mode)) mode = false;
                    EditorGUILayout.EndHorizontal();
                }
                // == -- ===== -- == // Buttons // == -- ===== -- == //

                // == // Settings Field // =================================================================================== //
                {
                    EditorGUILayout.BeginVertical("Box");

                    // == -- == -- == // Field Section // == -- == -- == //
                    {
                        if (mode)
                        {
                            // == // Object Mode // == //
                            targetCount = EditorGUILayout.IntSlider("Object Amount", targetCount, 2, TARGETMAX);


                            EditorGUI.BeginChangeCheck();///////////////////////////////////////////////////////////////////////////////////// -- Check values -- //
                            {                                                                                                                                     //
                                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);                                                                           //
                                for (int i = 0; i < targetCount; i++)                                                                                             //
                                {                                                                                                                                 //
                                    targetSampler[i] = (GameObject)EditorGUILayout.ObjectField($"Target Object {i}", targetSampler[i], typeof(GameObject), true); //
                                }                                                                                                                                 //
                                EditorGUILayout.EndScrollView();                                                                                                  //
                            }                                                                                                                                     //
                            if (EditorGUI.EndChangeCheck()) OnTargetsValueChanged();////////////////////////////////////////////////////////////////////////////////


                            EditorGUILayout.Space(4);

                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.Space(6);
                                targetParent = (Transform)EditorGUILayout.ObjectField("Parent Object", targetParent, typeof(Transform), true);
                                EditorGUILayout.EndVertical();
                                if (CustomGUILayout.ButtonBlueSmall("Generate Targets", 200f)) GenerateTargets();
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space(2);

                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.Space(6);
                                targetPrefixName = EditorGUILayout.TextField("Target Prefix Name", targetPrefixName);
                                EditorGUILayout.EndVertical();
                                if (CustomGUILayout.ButtonBlueSmall("Find Targets", 200f)) FindTargets();
                            }
                            EditorGUILayout.EndHorizontal();
                            // == // Object Mode // == //
                        }
                        else
                        {
                            // == // Spline Mode // == //
                            baseSpline = (SplineContainer)EditorGUILayout.ObjectField("Spline", baseSpline, typeof(SplineContainer), true);

                            EditorGUILayout.Space(2);
                            
                            EditorGUI.BeginChangeCheck();////////////////////////////////////////////////////////////////////////////////////////////////////////////// -- Check values -- //
                            {                                                                                                                                                              //
                                EditorGUILayout.BeginHorizontal();                                                                                                                         //
                                if (CustomGUILayout.ToggleBlueSmall(  "One smoothness",   !separateSmoothness, position.width / 2 - 12)) separateSmoothness = false;
                                if (CustomGUILayout.ToggleBlueSmall("Separate smoothness", separateSmoothness, position.width / 2 - 12)) separateSmoothness = true;
                                EditorGUILayout.EndHorizontal();                                                                                                                           //
                            }                                                                                                                                                              //
                            if (EditorGUI.EndChangeCheck()) OnSplineValueChanged();//////////////////////////////////////////////////////////////////////////////////////////////////////////

                            if (!separateSmoothness)
                            {
                                if (baseSpline == null) EditorGUILayout.LabelField("Spline container does not exist.");
                                else
                                {
                                    if (baseSpline.Splines.Count == 0) EditorGUILayout.LabelField("Spline does not exist.");
                                    else if (baseSpline.Splines.Count > 1) EditorGUILayout.LabelField("Two or more splines exist.");
                                    else
                                    {
                                        if (baseSpline.Spline.Count == 0) EditorGUILayout.LabelField("Spline has not knots");
                                        else
                                        {
                                            EditorGUILayout.Space(0);
                                            int segment = baseSpline.Spline.Count;

                                            if (baseSpline.Spline.Closed == true)
                                                EditorGUILayout.LabelField("<Color=#AAAAAA> Spline is</Color> <Color=#22FF88>Closed</Color>", LogStyle);
                                            else
                                                EditorGUILayout.LabelField("<Color=#AAAAAA> Spline is</Color> <Color=#FFFF22>NOT Closed</Color>", LogStyle);
                                        }
                                    }
                                }

                                EditorGUI.BeginChangeCheck();/////////////////////////////////////////////////////// -- Check values -- //
                                {                                                                                                       //
                                    smoothness = EditorGUILayout.IntSlider("Path Smoothness", smoothness, 2, 50);                       //
                                }                                                                                                       //
                                if (EditorGUI.EndChangeCheck()) OnSplineValueChanged();///////////////////////////////////////////////////

                                GUILayout.FlexibleSpace();
                            }
                            else
                            {
                                if (baseSpline == null) EditorGUILayout.LabelField("Spline container does not exist.");
                                else
                                {
                                    if (baseSpline.Splines.Count == 0) EditorGUILayout.LabelField("Spline does not exist.");
                                    else if (baseSpline.Splines.Count > 1) EditorGUILayout.LabelField("Two or more splines exist.");
                                    else
                                    {
                                        if (baseSpline.Spline.Count == 0) EditorGUILayout.LabelField("Spline has not knots");
                                        else
                                        {
                                            EditorGUILayout.Space(0);
                                            int segment = baseSpline.Spline.Count;
                                            prevSmoothness = smoothness;

                                            if (baseSpline.Spline.Closed)
                                                EditorGUILayout.LabelField("<Color=#AAAAAA> Spline is</Color> <Color=#22FF88>Closed</Color>", LogStyle);
                                            else
                                                EditorGUILayout.LabelField("<Color=#AAAAAA> Spline is</Color> <Color=#FFFF22>NOT Closed</Color>", LogStyle);

                                            if (segmentSmoothness.Count != segment)
                                            {
                                                segmentSmoothness.Clear();
                                                for (int i = 0; i < segment; i++) segmentSmoothness.Add(0);
                                            }

                                            EditorGUI.BeginChangeCheck();///////////////////////////////////////////////////////////////////////////////////// -- Check values -- //
                                            {                                                                                                                                     //
                                                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);                                                                           //
                                                                                                                                                                                  //
                                                smoothness = EditorGUILayout.IntSlider("Batch change", smoothness, 0, 10);                                                        //
                                                                                                                                                                                  //
                                                for (int i = 0; i < segment; i++)                                                                                                 //
                                                {                                                                                                                                 //
                                                    if (!baseSpline.Spline.Closed) if (i == segment - 1) break;                                                                   //
                                                    segmentSmoothness[i] = EditorGUILayout.IntSlider($"Knot {i} - Knot {i + 1}", segmentSmoothness[i], 0, 10);                    //
                                                }                                                                                                                                 //
                                                EditorGUILayout.EndScrollView();                                                                                                  //
                                            }                                                                                                                                     //
                                            if (EditorGUI.EndChangeCheck()) OnSplineValueChanged();/////////////////////////////////////////////////////////////////////////////////
                                        }
                                    }
                                }
                            }

                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.Space(6);
                                splineParent = (Transform)EditorGUILayout.ObjectField("Parent Object", splineParent, typeof(Transform), true);
                                EditorGUILayout.EndVertical();
                                if (CustomGUILayout.ButtonBlueSmall("Generate Spline", 200f)) GenerateSpline();
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space(2);

                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.Space(6);
                                splineName = EditorGUILayout.TextField("Spline Object Name", splineName);
                                EditorGUILayout.EndVertical();
                                if (CustomGUILayout.ButtonBlueSmall("Find Spline", 200f)) FindSpline();
                            }
                            EditorGUILayout.EndHorizontal();
                            // == // Spline Mode // == //
                        }
                    }
                    // == -- == -- == // Field Section // == -- == -- == //

                    EditorGUILayout.EndVertical();
                }
                // == // Settings Field // =================================================================================== //

                EditorGUILayout.EndVertical();
            }
            // == // Mode Select // ========================================================================================== //

            CustomGUILayout.Divider(this);

            // == // Generate Settings // ==================================================================================== //
            {
                EditorGUILayout.BeginVertical("Box");

                // == -- == -- == // Title Section // == -- == -- == //
                {
                    EditorGUILayout.Space(2);
                    EditorGUILayout.LabelField("Generate Settings", CustomGUIStyles.mainLabel);
                    EditorGUILayout.Space(2);
                }
                // == -- == -- == // Title Section // == -- == -- == //

                // == -- == -- == // Field Section // == -- == -- == //
                {
                    currentPath = (CinemachineSmoothPath)EditorGUILayout.ObjectField("Path", currentPath, typeof(CinemachineSmoothPath), true);
                    
                    EditorGUILayout.Space(2);
                    
                    pathObjectName = EditorGUILayout.TextField("Object Name", pathObjectName);
                    generateParent = (Transform)EditorGUILayout.ObjectField("Parent Object", generateParent, typeof(Transform), true);
                }
                // == -- == -- == // Field Section // == -- == -- == //

                EditorGUILayout.Space(2);

                // == -- ===== -- == // Buttons // == -- ===== -- == //
                {
                    using (new BackgroundColorScope(BTNCOL_LIGHT))
                    {
                        if (GUILayout.Button("Generate New Cinemachine Smooth Path", CustomGUIStyles.buttonBlueLarge, GUILayout.Height(40))) GenerateCinemachinePath(); 
                        if (GUILayout.Button("Find Cinemachine Smooth Path by Name", CustomGUIStyles.buttonBlueLarge, GUILayout.Height(40))) FindCinemachinePath(); 
                    }
                }
                // == -- ===== -- == // Buttons // == -- ===== -- == //
                
                EditorGUILayout.EndVertical();
            }
            // == // Generate Settings // ==================================================================================== //

            CustomGUILayout.Divider(this);

            // == // Connect Path // ========================================================================================= //
            {
                EditorGUILayout.BeginVertical("Box");

                // == -- == -- == // Title Section // == -- == -- == //
                {
                    EditorGUILayout.Space(2);
                    EditorGUILayout.LabelField("Connect Path", CustomGUIStyles.mainLabel);
                    EditorGUILayout.Space(2);
                }
                // == -- == -- == // Title Section // == -- == -- == //

                // == -- == -- == // Field Section // == -- == -- == //
                {
                    if (currentPath != null)
                    {
                        EditorGUI.BeginChangeCheck();////////////////////////////////////////////////////// -- Check values -- //
                        {                                                                                                      //
                            pathResolution = EditorGUILayout.IntSlider("Resolution", pathResolution, 1, 100);                  //
                            pathIsLooped = EditorGUILayout.Toggle("Looped", pathIsLooped);                                     //
                            pathVisColor = EditorGUILayout.ColorField("Visualization Color", pathVisColor);                    //
                            pathVisWidth = EditorGUILayout.Slider("Visualization Width", pathVisWidth, 0f, 10f);               //
                        }                                                                                                      //
                        if (EditorGUI.EndChangeCheck()) OnPathValueChanged();////////////////////////////////////////////////////
                    }
                }
                // == -- == -- == // Field Section // == -- == -- == //

                EditorGUILayout.Space(2);

                // == -- ===== -- == // Buttons // == -- ===== -- == //
                {
                    string buttonLabel = "";
                    if (isConnecting) buttonLabel = "Disconnect the Path";
                    else buttonLabel = mode ? "Connect Objects to Cinemachine Smooth Path" : "Connect Spline to Cinemachine Smooth Path";
                    using (new BackgroundColorScope(isConnecting ? CONNECT_DARK : CONNECT_LIGHT))
                    { if (GUILayout.Button(buttonLabel, isConnecting ? ConnectBtnB : ConnectBtnA, GUILayout.Height(40))) ConnectPath();}
                }
                // == -- ===== -- == // Buttons // == -- ===== -- == //

                EditorGUILayout.EndVertical();
            }
            // == // Connect Path // ========================================================================================= //


            CustomGUILayout.Divider(this);


            // == // Log // ================================================================================================== //
            {
                EditorGUILayout.BeginVertical("Box");

                // == -- == -- == // Title Section // == -- == -- == //
                {
                    EditorGUILayout.Space(2);
                    EditorGUILayout.LabelField("Log", CustomGUIStyles.mainLabel);
                    EditorGUILayout.Space(2);
                }
                // == -- == -- == // Title Section // == -- == -- == //


                // == -- == -- == // Field Section // == -- == -- == //
                {
                    EditorGUILayout.Space(2);

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.Space(5);
                        EditorGUILayout.LabelField(logText, LogStyle);
                        EditorGUILayout.EndVertical();

                        using (new BackgroundColorScope(BTNCOL_LIGHT))
                        { if (GUILayout.Button("Clear Log", CustomGUIStyles.buttonBlueSmall, GUILayout.Width(200), GUILayout.Height(30))) logText = ""; }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(2);
                }
                // == -- == -- == // Field Section // == -- == -- == //
                
                EditorGUILayout.EndVertical();
            }
            // == // Log // ================================================================================================== //


            CustomGUILayout.Divider(this);
        }


        //=====================================================================//
        // Runs once when each button clicked.                                 //
        //=====================================================================//
        #region Button Actions

        private void ConnectPath()
        {
            if (currentPath == null)
            {
                logText = $"{COLORTAG_SR}Please set Cinemachine smooth path.{COLORTAG_E}";
                return;
            }
            if (!mode && baseSpline == null)
            {
                logText = $"{COLORTAG_SR}Please set Spline.{COLORTAG_E}";
                return;
            }

            isConnecting = !isConnecting;
        }

        private void GenerateCinemachinePath()
        {
            GameObject obj = new GameObject();
            obj.name = string.IsNullOrEmpty(pathObjectName) ? "CinemachineSmoothPath" : pathObjectName;
            if (generateParent != null) obj.transform.parent = generateParent;
            CinemachineSmoothPath path = obj.AddComponent<CinemachineSmoothPath>();
            currentPath = path;
            path.m_Resolution = pathResolution;
            path.m_Looped = pathIsLooped;
            path.m_Appearance.pathColor = pathVisColor;
            path.m_Appearance.width = pathVisWidth;
            Selection.activeGameObject = obj;
            
            logText = $"{COLORTAG_SG}Cinemachine Smooth Path Generated.{COLORTAG_E}";
        }

        private void FindCinemachinePath()
        {
            if (string.IsNullOrEmpty(pathObjectName))
            {
                logText = $"{COLORTAG_SR}Please set name at ObjectName.{COLORTAG_E}";
                return;
            }
            else
            {
                GameObject[] objects = FindObjectsOfType<GameObject>();
                if (objects.Length == 0)
                {
                    logText = $"{COLORTAG_SR}There are no objects.{COLORTAG_E}";
                    return;
                }
                foreach (GameObject obj in objects)
                {
                    if (obj.name == pathObjectName)
                    {
                        currentPath = obj.GetComponent<CinemachineSmoothPath>();
                        generateParent = null;
                        Selection.activeGameObject = obj;
                        logText = $"{COLORTAG_SY}An object with the same name may exist.{COLORTAG_E}\n" +
                                  $"{COLORTAG_SG}Object found.{COLORTAG_E}";
                        return;
                    }
                }

                logText = $"{COLORTAG_SR}{pathObjectName} could not find.{COLORTAG_E}";
            }
        }

        private void GenerateTargets()
        {
            for (int i = 0; i < targetCount; i++)
            {
                GameObject obj = new GameObject();
                obj.name = string.IsNullOrEmpty(targetPrefixName) ? $"TargetObject_{i}" : targetPrefixName + $"_{i}";

                MeshFilter meshfilter = obj.AddComponent<MeshFilter>();
                meshfilter.mesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/_AKACore/Tools/Objects/Sphere.mesh");

                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.name = "Lit";
                mat.SetColor("_BaseColor", Color.HSVToRGB(1f / (float)targetCount * (float)i, 1, 1));
                MeshRenderer render = obj.AddComponent<MeshRenderer>();
                render.material = mat;

                if (targetParent != null) obj.transform.parent = targetParent;

                Vector3 childPostion = Vector3.zero;
                float angle = ( 90f - (360f / (float)targetCount * (float)i) ) * Mathf.Deg2Rad;
                childPostion.x += Mathf.Cos(angle);
                childPostion.z += Mathf.Sin(angle);
                obj.transform.position = childPostion;

                targetSampler[i] = obj;
            }

            OnTargetsValueChanged();

            logText = $"{COLORTAG_SG}{targetCount} targets Generated.{COLORTAG_E}";
        }

        private void FindTargets()
        {
            if (string.IsNullOrEmpty(targetPrefixName))
            {
                logText = $"{COLORTAG_SR}Please set name at Target Prefix Name.{COLORTAG_E}";
                return;
            }
            else
            {
                GameObject[] objects = FindObjectsOfType<GameObject>();
                if (objects.Length == 0)
                {
                    logText = $"{COLORTAG_SR}There are no objects.{COLORTAG_E}";
                    return;
                }

                bool success = false;
                int index = -1;
                foreach (GameObject obj in objects)
                {
                    string objName = obj.name.Substring(0, targetPrefixName.Length);
                    if (objName == targetPrefixName)
                    {
                        success = true;
                        index++;
                        if (index < targetCount) targetSampler[index] = obj;
                    }
                }

                OnTargetsValueChanged();

                targetParent = null;
                logText = $"{COLORTAG_SY}A spline with the same name may exist.{COLORTAG_E}\n" +
                          $"{COLORTAG_SG}Spline found.{COLORTAG_E}";
                
                if (!success)
                    logText = $"{COLORTAG_SR}No objects with beginning with {targetPrefixName} were found.{COLORTAG_E}";
            }
        }
        
        private void GenerateSpline()
        {
            GameObject obj = new GameObject();
            obj.name = string.IsNullOrEmpty(splineName) ? "OriginSpline" : splineName;
            if (splineParent != null) obj.transform.parent = splineParent;
            SplineContainer path = obj.AddComponent<SplineContainer>();
            baseSpline = path;

            logText = $"{COLORTAG_SG}Spline Generated{COLORTAG_E}";
        }
        
        private void FindSpline()
        {
            if (string.IsNullOrEmpty(splineName))
            {
                logText = $"{COLORTAG_SR}Please set name at Spline Object Name.{COLORTAG_E}";
                return;
            }
            else
            {
                GameObject[] objects = FindObjectsOfType<GameObject>();
                if (objects.Length == 0)
                {
                    logText = $"{COLORTAG_SR}There are no objects.{COLORTAG_E}";
                    return;
                }
                foreach (GameObject obj in objects)
                {
                    if (obj.name == splineName)
                    {
                        currentPath = obj.GetComponent<CinemachineSmoothPath>();
                        splineParent = null;
                        logText = $"{COLORTAG_SY}A spline with the same name may exist.{COLORTAG_E}\n" +
                                  $"{COLORTAG_SG}Spline found.{COLORTAG_E}";
                        return;
                    }
                }

                logText = $"{COLORTAG_SR}{splineName} could not find.{COLORTAG_E}";
            }
        }

        #endregion

        //=====================================================================//
        // Runs once when the value changed                                    //
        // to lighten the processing executed every frame as much as possible. //
        //=====================================================================//
        #region On Value Changed
        private void OnTargetsValueChanged()
        {
            targets.Clear();

            for (int i = 0; i < targetCount; i++)
            {
                if (targetSampler[i] == null) continue;
                targets.Add(targetSampler[i]);
            }

            sampleWaypoint = new CinemachineSmoothPath.Waypoint[targets.Count];
        }
        private void OnSplineValueChanged()
        {
            if (separateSmoothness)
            {
                if (smoothness > 10) smoothness = 10;
                if (prevSmoothness != smoothness) for (int i = 0; i < segmentSmoothness.Count; i++) segmentSmoothness[i] = smoothness;

                int wayPointCount = 0;
                foreach (int num in segmentSmoothness) wayPointCount += num;
                wayPointCount += segmentSmoothness.Count;

                sampleWaypoint = new CinemachineSmoothPath.Waypoint[wayPointCount];
            }
            else
            {
                if (smoothness < 2) smoothness = 2;
                sampleWaypoint = new CinemachineSmoothPath.Waypoint[smoothness];
            }

            Repaint();
        }
        private void OnPathValueChanged()
        {
            currentPath.m_Resolution = pathResolution;
            currentPath.m_Looped = pathIsLooped;
            currentPath.m_Appearance.pathColor = pathVisColor;
            currentPath.m_Appearance.width = pathVisWidth;
        }
        #endregion



        private void OnEnable() => EditorApplication.update += OnUpdate;
        private void OnDisable() => EditorApplication.update -= OnUpdate;

        private void OnUpdate()
        {
            #region Runable check
            if (!isConnecting) return;
            if (currentPath == null)
            {
                logText = $"{COLORTAG_SR}Please set Cinemachine smooth path.{COLORTAG_E}";
                isConnecting = false;
                return;
            }
            if (!mode && baseSpline == null)
            {
                logText = $"{COLORTAG_SR}Please set Spline.{COLORTAG_E}";
                isConnecting = false;
                return;
            }
            #endregion

            if (mode) ConnectingWithObjectMode();
            else      ConnectingWithSplineMode();
        }

        private void ConnectingWithObjectMode()
        {
            for (int i = 0; i < targets.Count; i++)
            {
                sampleWaypoint[i].position = targets[i].transform.position;
                sampleWaypoint[i].roll = 0f;
            }

            currentPath.m_Waypoints = sampleWaypoint;
            currentPath.InvalidateDistanceCache();
        }

        private void ConnectingWithSplineMode()
        {
            float wholeLength = baseSpline.Spline.GetLength();

            if (separateSmoothness)
            {
                float prevLength = 0f;
                int sampleCount = 0;
                for (int i = 0; i < segmentSmoothness.Count; i++)
                {
                    float length = baseSpline.Spline.GetCurveLength(i) / wholeLength;
                    
                    for (int j = 0; j < segmentSmoothness[i] + 1; j++)
                    {
                        float epoint = prevLength + (length  / ((float)segmentSmoothness[i] + 1f) * (float)j);
                        sampleWaypoint[sampleCount].position = baseSpline.Spline.EvaluatePosition(epoint);
                        sampleWaypoint[sampleCount].roll = 0f;
                        sampleCount++;
                    }

                    prevLength += length;
                }
            }
            else
            {
                for (int i = 0; i < smoothness; i++)
                {
                    float epoint = 1f / (float)smoothness * (float)i;
                    sampleWaypoint[i].position = baseSpline.Spline.EvaluatePosition(epoint);
                    sampleWaypoint[i].roll = 0f;
                }
            }

            currentPath.m_Waypoints = sampleWaypoint;
            currentPath.InvalidateDistanceCache();
        }
    }
}