using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace RusyGameStudio.Tools
{
    public class FrameRateChecker
    {
        public static void CalculateSpendingTime(Action action, int attempt)
        {
            List<int> time = new List<int>();

            for (int i = 0; i < 100; i++)
            {
                DateTime start = DateTime.Now;
                for (int j = 0; j < attempt; j++) action();
                DateTime end = DateTime.Now;

                TimeSpan span = end - start;
                time.Add(span.Milliseconds);
            }

            int min = time.Min();
            time.Remove(min);
            int max = time.Max();
            time.Remove(max);
            double average = time.Average();

            Debug.Log("<Color=#002299>Time taken to calculate your method:\n" +
                $"Fastest time : {min}\n" +
                $"Longest time : {max}\n" +
                $"Average time : {average}");
        }
    }
}