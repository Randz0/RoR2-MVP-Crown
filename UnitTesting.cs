using RoR2;
using RoR2.Stats;
using UnityEngine;
using HarmonyLib;

namespace Crown_Mod;

internal class UnitTest
{
    internal static bool testingItemIndex = false;
    internal static bool testingCanGiveCrownItem = false;

    internal static bool testingItemShader = false;
    internal static bool testingCrownDisplayConflicts = false;


    [HarmonyPatch(typeof(ItemCatalog), nameof(ItemCatalog.Init))]
    private class TestGetItemIndex
    {

        private static void OnTestFail()
        {
            Plugin.Logger.LogInfo($"Item Name --> {CrownItemDef.GetCrownDef().name}");

            Plugin.Logger.LogInfo("Printing Entire Catalog");

            foreach (string itemName in ItemCatalog.itemNameToIndex.Keys)
            {
                Plugin.Logger.LogInfo($"\t{itemName} --> {ItemCatalog.itemNameToIndex[itemName]}");
            }
        }

        [HarmonyPostfix]
        private static void PrintCrownIndex()
        {
            if (!testingItemIndex)
            {
                return;
            }

            bool success = ItemCatalog.itemNameToIndex.ContainsKey(CrownItemDef.CROWN_ITEM_NAME);

            Plugin.Logger.LogInfo($"Test: Is Item In Catalog : {success}");

            if (success)
            {
                Plugin.Logger.LogInfo($"Item Index --> {ItemCatalog.itemNameToIndex[CrownItemDef.CROWN_ITEM_NAME]}");
                return;
            }

            OnTestFail();
        }
    }

    [HarmonyPatch(typeof(StatManager), nameof(StatManager.OnServerStageBegin))]
    private class TestCrownItemEquipable
    {
        [HarmonyPostfix]
        private static void GiveCrownItemToPlayer()
        {
            if (!testingCanGiveCrownItem)
            {
                return;
            }

            Inventory playerInventory = PlayerStatsComponent.instancesList[0].characterMaster.inventory;
            playerInventory.GiveItemString(CrownItemDef.CROWN_ITEM_NAME);

            Plugin.Logger.LogInfo($"Testing : Give Player Crown Item : Status Unknown");
        }
    }

    public static void ReadItemDispRules()
    {
        foreach (GameObject character in BodyCatalog.allBodyPrefabs)
        {
            CharacterModel model = character.GetComponentInChildren<CharacterModel>();

            if (model)
            {
                foreach (DisplayRuleGroup ruleSet in model.itemDisplayRuleSet.runtimeItemRuleGroups)
                {
                    if (ruleSet.isEmpty || ruleSet.rules[0].ruleType != ItemDisplayRuleType.ParentedPrefab)
                    {
                        continue;
                    }

                    Plugin.Logger.LogInfo(ruleSet.rules[0].childName);
                }
            }
        }
    }

    public static void TestShaderConfig()
    {
        if (!testingItemShader)
        {
            return;
        }

        MeshRenderer renderer = CrownItemDisplayRule.crownMeshRenderer;

        if (!renderer)
        {
            Plugin.Logger.LogWarning("Could Not Find The Renderer Of The Crown Model");
            return;
        }

        Material crownMat = renderer.GetMaterial();

        if (!crownMat)
        {
            Plugin.Logger.LogWarning("Could Not Find the Material Of The Crown");
            return;
        }

        if (!crownMat.shader)
        {
            Plugin.Logger.LogWarning("Could Not Find the Shader Of The Crown");
            return;
        }

        Plugin.Logger.LogInfo($"Crown Mat Shader Name == {crownMat.shader.name}");

        if (!crownMat.GetTexture("_MainTex"))
        {
            Plugin.Logger.LogInfo("Crown Mat Texture Could Not Be Found");
            return;
        }

        if (crownMat.GetTexture("_MainTex"))
        {
            Plugin.Logger.LogInfo($"Main Texture Name == {crownMat.GetTexture("_MainTex").name}");
        }
    }
}
