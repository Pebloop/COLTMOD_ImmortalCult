using BepInEx;
using HarmonyLib;
using System.Collections.Generic;

namespace ImmortalCult
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

            // Patch save method to disable immortality
            harmony.Patch(
                AccessTools.Method(typeof(SaveAndLoad), "Saving"),
                new HarmonyMethod(AccessTools.Method(typeof(Plugin), "DisableImmortal"))
                );

            // Patch load method to enable immortality
            harmony.Patch(
                AccessTools.Method(typeof(SaveAndLoad), "Load"),
                null,
                new HarmonyMethod(AccessTools.Method(typeof(Plugin), "EnableImmortal"))
                );

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        [HarmonyPostfix]
        public static void EnableImmortal()
        {
            List<FollowerInfo> followers = DataManager.Instance.Followers;
            foreach (FollowerInfo follower in followers)
            {
                if (!follower.Traits.Contains(FollowerTrait.TraitType.Immortal))
                    follower.Traits.Add(FollowerTrait.TraitType.Immortal);
            }
        }

        public static void DisableImmortal()
        {
            List<FollowerInfo> followers = DataManager.Instance.Followers;
            foreach (FollowerInfo follower in followers)
            {
                if (follower.Traits.Contains(FollowerTrait.TraitType.Immortal))
                {
                    follower.Traits.Add(FollowerTrait.TraitType.Immortal);
                }
            }
        }
    }
}
