using HarmonyLib;
using Player;
using System.Linq;

namespace GTFO_ArtifactGenerator
{
    /// <summary>
    /// Optional Harmony patch for ArtifactPickup_Core.OnInteractionPickUp
    /// Currently configured to allow default pickup behavior while optionally opening UI
    ///
    /// To disable default artifact granting and only use custom UI:
    /// Change the Prefix return value to false
    /// </summary>
    [HarmonyPatch(typeof(ArtifactPickup_Core), nameof(ArtifactPickup_Core.OnInteractionPickUp))]
    internal static class ArtifactPickupPatch
    {
        /// <summary>
        /// Prefix patch that runs before OnInteractionPickUp
        /// </summary>
        /// <param name="player">The player who picked up the artifact</param>
        /// <returns>true to continue default behavior, false to block it</returns>
        static bool Prefix(PlayerAgent player)
        {
            // Only host can grant custom boosters
            if (!IsHost())
                return true;

            // Optional: Open UI when artifact is picked up
            // Plugin.UI?.Toggle();

            // Return true: Allow default random artifact to be granted
            // Return false: Block default artifact, use only custom UI to grant
            return true;
        }

        /// <summary>
        /// Checks if the local player is the host using reflection
        /// </summary>
        private static bool IsHost()
        {
            try
            {
                // Find SNet type dynamically
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    var snetType = assembly.GetTypes().FirstOrDefault(t => t.Name == "SNet");
                    if (snetType != null)
                    {
                        var isMasterProp = snetType.GetProperty("IsMaster", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        if (isMasterProp != null)
                        {
                            return (bool)isMasterProp.GetValue(null);
                        }
                    }
                }
            }
            catch { }

            return false;
        }
    }
}
