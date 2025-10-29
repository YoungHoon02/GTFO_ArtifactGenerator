using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;

namespace GTFO_ArtifactGenerator
{
    [BepInPlugin("YoungHoon02.GTFO_ArtifactGenerator", "GTFO Artifact Generator", "1.0.0")]
    public class Plugin : BasePlugin
    {
        private Harmony _harmony = null!;
        internal static ArtifactSelectorUI? UI { get; set; }

        public override void Load()
        {
            // Register custom MonoBehaviour with IL2CPP
            try
            {
                ClassInjector.RegisterTypeInIl2Cpp<ArtifactSelectorUI>();
                Log.LogInfo("Registered ArtifactSelectorUI with IL2CPP");
            }
            catch (System.Exception ex)
            {
                Log.LogWarning($"Could not register ArtifactSelectorUI (may already be registered): {ex.Message}");
            }

            // Initialize Harmony for patching
            _harmony = new Harmony("YoungHoon02.GTFO_ArtifactGenerator");
            _harmony.PatchAll();

            Log.LogInfo("GTFO Artifact Generator is loaded!");
            Log.LogInfo("The UI will be available in-game. Press F6 to toggle.");
        }
    }
}

