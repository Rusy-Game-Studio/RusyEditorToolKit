using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RusyGameStudio.Tools
{
  public class ObjectSortAssistant : EditorWindow
  {
    private bool isAscending = true;

    [MenuItem("RusyEditorToolKit/Object Sort Assistant")]
    [MenuItem("GameObject/Rusy Editor Tool Kit/Object Sort Assistant", false, 0)]
    static void Open()
    {
      var window = GetWindow<ObjectSortAssistant>();
      window.titleContent = new GUIContent("Object Sort Assistant");
    }

    private void OnGUI()
    {
      CustomGUILayout.Title("Object Sort Assistant", this);

      EditorGUILayout.BeginVertical("Box");
      {
        EditorGUILayout.LabelField(
          "名前順でオブジェクトを並び替えます。ソートしたいオブジェクト群の親オブジェクトを一つ選択し、" +
          "【Sort】を押してください。一度に大量のオブジェクトの整理が行われるので、予期せぬ並び替えを" +
          "防ぐために複数選択およびオブジェクト全てソートは非対応としています。\n\n" +
          "Sort objects by name. Select only one parent object from the group of objects you wish to sort." +
          "then press [Sort]. To prevent unexpected sorting, multiple selections and sorting all objects" +
          "are not supported.",
          CustomGUIStyles.explanation);
      }
      EditorGUILayout.EndVertical();

      EditorGUILayout.Space(10);
      EditorGUILayout.LabelField("Settings", CustomGUIStyles.mainLabel);
      EditorGUILayout.Space(4);

      isAscending = EditorGUILayout.Toggle("Is Ascending", isAscending);

      CustomGUILayout.Divider(this);

      if (CustomGUILayout.ButtonBlueLarge("Sort")) Sort();
    }

    private void Sort()
    {
      // 複数ソートは非対応
      if (Selection.gameObjects.Length = 0)
      {
        Debug.LogError("全ソートはできません。\nNot allowed sorting all objects.");
        return;
      }
      else if (Selection.gameObjects.Length > 1)
      {
        Debug.LogError("複数の親オブジェクトのソートはできません。\nNot allowed sorting multiple parent objects.");
        return;
      }

      int childCount = Selection.gameObjects[0].transform.childCount;

      // 選択中のオブジェクトに子がいなければエラー
      if (childCount == 0)
      {
        Debug.LogError("選択中のオブジェクトに子が配置されていません。\nNot placed child objects in the selection.");
        return;
      }
      else if (childCount == 1)
      {
        Debug.LogError("選択中のオブジェクトに子が一つしか配置されていません。\nPlaced a single child object in the selection.");
        return;
      }

      // 実行
      List<GameObject> children = new List<GameObject>();
      for (int i = 0; i < childCount; i++)
      {
        children.Add(Selection.gameObjects[0].transform.GetChild(i).gameObject);
      }

      if (isAscending)
      {
        children = children.OrderBy(x => x.name).ToList();
      }
      else
      {
        children = children.OrderByDescending(x => x.name).ToList();
      }

      for (int i = 0; i < children.Count; i++)
      {
        children[i].transform.SetSiblingIndex(i);
      }
    }
  }
}
