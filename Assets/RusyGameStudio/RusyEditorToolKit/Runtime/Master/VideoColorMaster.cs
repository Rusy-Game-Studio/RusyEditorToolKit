using System.Collections.Generic;
using UnityEngine;

namespace RusyGameStudio
{
    public class VideoColorMaster
    {
        static private List<Color> VideoColors = new List<Color>();

        static public void ResetColorList(int count)
        {
            VideoColors.Clear();
            for (int i = 0; i < count; i++) VideoColors.Add(Color.white);
        }

        static public void SetColor(int key, Color value) => VideoColors[key] = value;
        static public void SetColors(List<Color> values) => VideoColors = values;
        static public Color GetColor(int key) => VideoColors[key];
        static public List<Color> GetColors() => VideoColors;
        static public int ColorCount => VideoColors.Count;
    }
}