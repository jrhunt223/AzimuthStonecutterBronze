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
        public const string PluginVersion = "1.0.1";

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
        // Stonecutter itself requires Iron. Moving the Stonecutter to Bronze would make
        // Player.UpdateKnownRecipesList discover the Sharpening Stone immediately.
        //
        // Hide that one recipe from ObjectDB while the vanilla discovery pass runs for
        // players who have not discovered Iron. This prevents both the unlock itself
        // and the repeated "New crafting recipe" notification. If an earlier version
        // already added the recipe, remove it once from the player's known set.
        [HarmonyPatch(typeof(Player), "UpdateKnownRecipesList")]
        private static class PreserveSharpeningStoneIronGate
        {
            private static readonly System.Reflection.FieldInfo KnownRecipesField = AccessTools.Field(typeof(Player), "m_knownRecipes");
            private static readonly System.Reflection.FieldInfo KnownMaterialField = AccessTools.Field(typeof(Player), "m_knownMaterial");

            private sealed class GateState
            {
                public Recipe Recipe;
                public int Index;
            }

            private static void Prefix(Player __instance, out GateState __state)
            {
                __state = null;

                try
                {
                    if (__instance == null || ObjectDB.instance == null || ObjectDB.instance.m_recipes == null) return;

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

                    if (knownMaterials.Contains(ironToken)) return;

                    // Clean up characters that learned it while running v1.0.0.
                    knownRecipes.Remove(sharpeningToken);

                    for (int i = 0; i < ObjectDB.instance.m_recipes.Count; ++i)
                    {
                        Recipe recipe = ObjectDB.instance.m_recipes[i];
                        if (recipe != null && recipe.m_item != null && recipe.m_item.gameObject.name == "SharpeningStone")
                        {
                            __state = new GateState { Recipe = recipe, Index = i };
                            ObjectDB.instance.m_recipes.RemoveAt(i);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("[Azimuth Stonecutter Bronze] Iron progression gate prefix failed: " + ex);
                }
            }

            private static void Postfix(GateState __state)
            {
                try
                {
                    if (__state == null || __state.Recipe == null || ObjectDB.instance == null || ObjectDB.instance.m_recipes == null) return;

                    int index = Math.Max(0, Math.Min(__state.Index, ObjectDB.instance.m_recipes.Count));
                    ObjectDB.instance.m_recipes.Insert(index, __state.Recipe);
                }
                catch (Exception ex)
                {
                    Debug.LogError("[Azimuth Stonecutter Bronze] Iron progression gate restore failed: " + ex);
                }
            }
        }
    }
}
