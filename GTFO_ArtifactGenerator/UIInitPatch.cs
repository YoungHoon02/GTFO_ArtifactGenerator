using HarmonyLib;
using UnityEngine;

namespace GTFO_ArtifactGenerator
{
    /// <summary>
    /// Patch to initialize the UI when the game starts
    /// Type registration is done in Plugin.Load()
    /// </summary>
    [HarmonyPatch(typeof(GameStateManager), nameof(GameStateManager.ChangeState))]
    internal static class UIInitPatch
    {
        private static bool _initialized = false;

        static void Postfix()
        {
            if (_initialized) return;

            try
            {
                // Create UI GameObject (type already registered in Plugin.Load)
                var go = new GameObject("ArtifactSelectorUI");
                UnityEngine.Object.DontDestroyOnLoad(go);
                Plugin.UI = go.AddComponent<ArtifactSelectorUI>();
                _initialized = true;

                BepInEx.Logging.Logger.CreateLogSource("ArtifactGenerator").LogInfo("UI initialized successfully!");
            }
            catch (System.Exception ex)
            {
                BepInEx.Logging.Logger.CreateLogSource("ArtifactGenerator").LogError($"Failed to initialize UI: {ex}");
            }
        }
    }
}
