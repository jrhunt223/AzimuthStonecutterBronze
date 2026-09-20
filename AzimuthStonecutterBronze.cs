using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;

namespace AzimuthGaming.StonecutterBronze
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    public sealed class AzimuthStonecutterBronze : BaseUnityPlugin
    {
        public const string PluginGuid = "com.azimuthgaming.stonecutterbronze";
        public const string PluginName = "Azimuth Stonecutter Bronze";
        public const string PluginVersion = "1.0.0";

        private Harmony _harmony;

        private void Awake()
        {
            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll();
            PrefabManager.OnVanillaPrefabsAvailable += ApplyStonecutterRecipe;
            Logger.LogInfo(PluginName + " " + PluginVersion + " loaded.");
        }

        private void OnDestroy()
        {
            PrefabManager.OnVanillaPrefabsAvailable -= ApplyStonecutterRecipe;
            if (_harmony != null) _harmony.UnpatchSelf();
        }

        private void ApplyStonecutterRecipe()
        {
            try
            {
                GameObject stonecutterPrefab = PrefabManager.Instance.GetPrefab("piece_stonecutter");
                GameObject bronzePrefab = PrefabManager.Instance.GetPrefab("Bronze");
                if (stonecutterPrefab == null || bronzePrefab == null)
                {
                    Logger.LogError("Could not find piece_stonecutter or Bronze prefab.");
                    return;
                }

                Piece piece = stonecutterPrefab.GetComponent<Piece>();
                ItemDrop bronze = bronzePrefab.GetComponent<ItemDrop>();
                if (piece == null || bronze == null || piece.m_resources == null)
                {
                    Logger.LogError("Stonecutter piece or Bronze ItemDrop was invalid.");
                    return;
                }

                bool changed = false;
                foreach (Piece.Requirement requirement in piece.m_resources)
                {
                    if (requirement == null || requirement.m_resItem == null) continue;
                    if (requirement.m_resItem.gameObject.name == "Iron")
                    {
                        requirement.m_resItem = bronze;
                        requirement.m_amount = 2;
                        requirement.m_amountPerLevel = 0;
                        changed = true;
                    }
                }

                if (changed)
                    Logger.LogInfo("Stonecutter requirement changed: 2 Iron -> 2 Bronze.");
                else
                    Logger.LogWarning("No Iron requirement was found on piece_stonecutter; no cost was changed.");
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to change Stonecutter recipe: " + ex);
            }
        }

        // The Sharpening Stone is normally progression-gated indirectly because the
        // Stonecutter itself requires Iron. Since this mod moves the Stonecutter to
        // Bronze, explicitly preserve that progression gate: the player must have
        // discovered Iron before Sharpening Stone can remain in known recipes.
        [HarmonyPatch(typeof(Player), "UpdateKnownRecipesList")]
        private static class PreserveSharpeningStoneIronGate
        {
            private static readonly System.Reflection.FieldInfo KnownRecipesField = AccessTools.Field(typeof(Player), "m_knownRecipes");
            private static readonly System.Reflection.FieldInfo KnownMaterialField = AccessTools.Field(typeof(Player), "m_knownMaterial");

            private static void Postfix(Player __instance)
            {
                try
                {
                    if (__instance == null || ObjectDB.instance == null) return;

                    HashSet<string> knownRecipes = KnownRecipesField.GetValue(__instance) as HashSet<string>;
                    HashSet<string> knownMaterials = KnownMaterialField.GetValue(__instance) as HashSet<string>;
                    if (knownRecipes == null || knownMaterials == null) return;

                    GameObject ironPrefab = ObjectDB.instance.GetItemPrefab("Iron");
                    GameObject sharpeningPrefab = ObjectDB.instance.GetItemPrefab("SharpeningStone");
                    if (ironPrefab == null || sharpeningPrefab == null) return;

                    ItemDrop iron = ironPrefab.GetComponent<ItemDrop>();
                    ItemDrop sharpeningStone = sharpeningPrefab.GetComponent<ItemDrop>();
                    if (iron == null || sharpeningStone == null) return;

                    string ironToken = iron.m_itemData.m_shared.m_name;
                    string sharpeningToken = sharpeningStone.m_itemData.m_shared.m_name;

                    if (!knownMaterials.Contains(ironToken))
                        knownRecipes.Remove(sharpeningToken);
                }
                catch (Exception ex)
                {
                    Debug.LogError("[Azimuth Stonecutter Bronze] Iron progression gate failed: " + ex);
                }
            }
        }
    }
}
