using UnityEngine;

namespace Core
{
    public static class KDebugger
    {
        public static void Print(string message) => Debug.Log(message);
        public static void Error(string message) => Debug.LogError(message);
    }
}