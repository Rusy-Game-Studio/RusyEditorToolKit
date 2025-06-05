using UnityEngine;

namespace RusyGameStudio.Tools
{
    public static class Mathematics
    {
        /// <summary> 入力した数値の桁数を返します </summary>
        public static int Digit(int num) => (num == 0) ? 1 : ((int)Mathf.Log10(num) + 1);
        
        /// <summary>  </summary>
        public static int Mod(int dividend, int divisor) => (dividend % divisor + divisor) % divisor;
        
        /// <summary> minからmaxまでの繰り返しの値にクランプします </summary>
        public static int LoopClamp(int value, int min, int max) => Mod(value, max - min + 1) + min;
        
        /// <summary> ランダムなbool値を返します </summary>
        public static bool RandomBool() => Random.Range(0, 2) == 0;
    }
}