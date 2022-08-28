using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace ImmortalCult
{

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static List<FollowerInfo> _immortalFollowers = new List<FollowerInfo>();

        private void Awake()
        {
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

            // Patch save method to disable immortality
            harmony.Patch(
                AccessTools.Method(typeof(SaveAndLoad), "Saving"),
                new HarmonyMethod(AccessTools.Method(typeof(Plugin), "DisableImmortality")),
                new HarmonyMethod(AccessTools.Method(typeof(Plugin), "EnableImmortality"))
                );

            // Patch load method to enable immortality
            harmony.Patch(
                AccessTools.Method(typeof(SaveAndLoad), "Load"),
                null,
                new HarmonyMethod(AccessTools.Method(typeof(Plugin), "EnableImmortality"))
                );

            // Patch new recruit method to enable immortality
            harmony.Patch(
                AccessTools.Method(typeof(FollowerInfo), "NewCharacter"),
                null,
                new HarmonyMethod(AccessTools.Method(typeof(Plugin), "MakeNewCharacterImmortal"))
                );

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }


        private static void MakeImmortal(FollowerInfo follower)
        {
            Console.Write("Follower " + follower.Name);
            if (follower.Traits.Contains(FollowerTrait.TraitType.Immortal))
            {
                if (!_immortalFollowers.Contains(follower))
                {
                    Console.WriteLine(" is immortal");
                    _immortalFollowers.Add(follower);
                }
            }
            else
            {
                Console.WriteLine(" is mortal");
                follower.Traits.Add(FollowerTrait.TraitType.Immortal);
            }
        }

        private static void MakeAllImmortal(List<FollowerInfo> followers)
        {
            foreach (FollowerInfo follower in followers)
            {
                MakeImmortal(follower);
            }
        }

        private static void MakeNoneImmortal(List<FollowerInfo> followers)
        {
            Console.WriteLine("IMMORTALS : " + _immortalFollowers.Count);
            foreach (FollowerInfo follower in followers)
            {
                if (follower.Traits.Contains(FollowerTrait.TraitType.Immortal)
                    && !_immortalFollowers.Contains(follower))
                {
                    follower.Traits.Remove(FollowerTrait.TraitType.Immortal);
                }
            }
        }

        [HarmonyPostfix]
        public static void EnableImmortality()
        {
            Console.WriteLine("Enabling immortality");

            MakeAllImmortal(DataManager.Instance.Followers);
            MakeAllImmortal(DataManager.Instance.Followers_Recruit);
        }

        [HarmonyPostfix]
        public static void MakeNewCharacterImmortal(FollowerInfo __instance, FollowerInfo __result)
        {
            Console.WriteLine("New character immortal");

            MakeImmortal(__result);
        }

        public static void DisableImmortality()
        {
            Console.WriteLine("Disabling immortality");

            MakeNoneImmortal(DataManager.Instance.Followers);
            MakeNoneImmortal(DataManager.Instance.Followers_Recruit);
        }

    }
}
