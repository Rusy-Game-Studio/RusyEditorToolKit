using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace RusyGameStudio.Tools
{
    public class AutoTimelineBinder : EditorWindow
    {
        #region Messages
        private const string msg_A = msg_A1 + msg_A2 + msg_A3;
        private const string msg_A1 = "UNDER CONSTRUCTION";
        private const string msg_A2 = "\nsample2";
        private const string msg_A3 = "\nsample3";
        private const string msg_B = msg_B1 + msg_B2 + msg_B3 + msg_B4;
        private const string msg_B1 = "Please check the following items.";
        private const string msg_B2 = "\n - Do not put any other files in the target timeline folder";
        private const string msg_B3 = "\n - All target timelines should have the same track structure.";
        private const string msg_B4 = "\n - SOMETHING";
        #endregion

        private TimelineAsset tempTL;
        private List<PlayableBinding> binds = new List<PlayableBinding>();
        private List<Object> objects = new List<Object>();
        private bool checkType = true;
        [SerializeField] private PlayableDirector[] directors;
        private GameObject searchTarget;

        [MenuItem("Rusy Game Studio/Auto Timeline Binder")]
        static void Open()
        {
            var window = GetWindow<AutoTimelineBinder>();
            window.titleContent = new GUIContent("Auto Timeline Binder");
        }

        private void OnGUI()
        {
            #region GUIStyles
            GUIStyle title_A = new GUIStyle();
            title_A.normal.textColor = Color.white;
            title_A.wordWrap = false;
            title_A.alignment = TextAnchor.MiddleCenter;
            GUIStyle content_A = new GUIStyle();
            content_A.normal.textColor = Color.white;
            content_A.wordWrap = true;
            #endregion

            {// Instructions -------------------------
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Instruction", title_A);
                EditorGUILayout.LabelField(msg_A, content_A);
                EditorGUILayout.EndVertical();
            }// Instructions -------------------------

            EditorGUILayout.Space();

            {// Help Box -----------------------------
                if (tempTL == null)
                {
                    EditorGUILayout.HelpBox(msg_B, MessageType.Info, true);
                    EditorGUILayout.Space();
                }
            }// Help Box -----------------------------

            {// Divider ------------------------------
                var splitterRect = EditorGUILayout.GetControlRect(false, GUILayout.Height(1));
                splitterRect.x = 0;
                splitterRect.width = position.width;
                EditorGUI.DrawRect(splitterRect, Color.Lerp(Color.gray, Color.black, 0.7f));
            }// Divider ------------------------------

            EditorGUILayout.Space();

            {// Template -----------------------------
                EditorGUI.BeginChangeCheck();
                tempTL = (TimelineAsset)EditorGUILayout.ObjectField("Template", tempTL, typeof(TimelineAsset), true);
                if (EditorGUI.EndChangeCheck()) SetBindings();
                if (tempTL == null) return;
                EditorGUILayout.Space();
            }// Template -----------------------------

            {// Search object ------------------------
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Search among the children of the target GameObject or saerch from list", content_A);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("GameObject", GUILayout.Width(position.width / 2 - 4))) checkType = true;
                if (GUILayout.Button("List", GUILayout.Width(position.width / 2 - 4))) checkType = false;
                EditorGUILayout.EndHorizontal();
                if (checkType)
                {
                    searchTarget = (GameObject)EditorGUILayout.ObjectField("Search target", searchTarget, typeof(GameObject), true);
                }
                else
                {
                    SerializedObject so = new SerializedObject(this);
                    SerializedProperty stringsProperty = so.FindProperty("directors");
                    EditorGUILayout.PropertyField(stringsProperty, true);
                    so.ApplyModifiedProperties();
                }
                EditorGUILayout.EndVertical();
            }// Search object ------------------------

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Bindings", title_A);
            for (int i = 0; i < binds.Count; i++)
            {
                // bindsが多すぎるときのスクロール等の処理を入れる
                if (binds[i].sourceObject.GetType() == typeof(AnimationTrack))
                    objects[i] = EditorGUILayout.ObjectField(binds[i].streamName, objects[i], typeof(Animator), true);
                else if (binds[i].sourceObject.GetType() == typeof(ActivationTrack))
                    objects[i] = EditorGUILayout.ObjectField(binds[i].streamName, objects[i], typeof(GameObject), true);
            }
            EditorGUILayout.EndVertical();
            if (GUILayout.Button("Bind them to target timeline")) Bind();
        }

        private void SetBindings()
        {
            PlayableBinding[] bindings = tempTL.outputs.ToArray();
            binds.Clear();
            objects.Clear();
            foreach (var bind in bindings)
            {
                if (bind.sourceObject.GetType() == typeof(AnimationTrack))
                {
                    Animator obj = null;
                    objects.Add(obj);
                }
                else if (bind.sourceObject.GetType() == typeof(ActivationTrack))
                {
                    GameObject obj = null;
                    objects.Add(obj);
                }
                binds.Add(bind);
            }
        }

        private void Bind()
        {
            if (checkType)
            {
                if (searchTarget.transform.childCount == 0)
                {
                    Debug.LogError($"There are no objects in {searchTarget.name}");
                    return;
                }

                directors = new PlayableDirector[searchTarget.transform.childCount];
                for (int i = 0; i < directors.Length; i++)
                {
                    directors[i] = searchTarget.transform.GetChild(i).GetComponent<PlayableDirector>();
                }
            }

            foreach (PlayableDirector director in directors)
            {
                TimelineAsset timeline = director.playableAsset as TimelineAsset;
                PlayableBinding[] bindings = timeline.outputs.ToArray();

                for (int i = 0; i < bindings.Length; i++)
                {
                    //順番がずれる可能性もあり得るので名前で判定したい
                    director.SetGenericBinding(bindings[i].sourceObject, objects[i]);
                }

                EditorUtility.SetDirty(timeline);
                AssetDatabase.SaveAssets();
            }
        }
    }


}