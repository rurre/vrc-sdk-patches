using UnityEngine;

namespace Pumkin.VrcSdkPatches
{
    internal static class PumkinPatcherLogger
    {
        const string LogPrefix = "Pumkin's SDK Patches";
        
        internal static void Log(string message) => Debug.Log($"{LogPrefix}: {message}");
        internal static void LogError(string message) => Debug.LogError($"{LogPrefix}: {message}");
        internal static void LogWarning(string message) => Debug.LogWarning($"{LogPrefix}: {message}");
        
    }
}