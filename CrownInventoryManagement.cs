using RoR2;
using RoR2.Stats;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.ContentManagement;

namespace Crown_Mod;

public class CrownDistribution
{
    // Logs a warning when returning null, giving a more debuggable system
    public static PlayerStatsComponent bestPlayerFoundSafer
    {
        get
        {
            if (bestPlayerFound)
            {
                return bestPlayerFound;
            }
            else if (PlayerStatsComponent.instancesList.Count > 0)
            {
                return PlayerStatsComponent.instancesList[0];
            }

            Plugin.Logger.LogWarning("Attempting To Access Best Player Stats When No Player Exist.");
            return null;
        }
    }

    private static PlayerStatsComponent lastCrownWinner = null;
    private static PlayerStatsComponent bestPlayerFound = null;

    private static ulong GetTotalDamage(StatSheet playerStatSheet)
    {
        ulong actualDamage = 0;

        foreach (StatField statField in playerStatSheet.fields)
        {
            if (statField.name == "totalDamageDealt")
            {
                actualDamage += statField.ulongValue;
            }
            else if (statField.name == "totalMinionDamageDealt")
            {
                actualDamage += statField.ulongValue;
            }
        }

        return actualDamage;
    }

    private static PlayerStatsComponent GetPlayerWithBestStats()
    {
        PlayerStatsComponent foundBestStats = bestPlayerFoundSafer;

        foreach (PlayerStatsComponent playerStats in PlayerStatsComponent.instancesList)
        {
            if (GetTotalDamage(foundBestStats.currentStats) < GetTotalDamage(playerStats.currentStats))
            {
                foundBestStats = playerStats;
            }
        }

        return foundBestStats;
    }

    public static void EvaluateBestPlayer()
    {
        if (PlayerStatsComponent.instancesList.Count <= 0)
        {
            return;
        }

        bestPlayerFound = GetPlayerWithBestStats();
    }

    public static void AwardBestPlayer()
    {
        if (lastCrownWinner)
        {
            lastCrownWinner.characterMaster.inventory.RemoveItem(ItemCatalog.FindItemIndex(CrownItemDef.CROWN_ITEM_NAME));
        }

        bestPlayerFound.characterMaster.inventory.GiveItemString(CrownItemDef.CROWN_ITEM_NAME);
        lastCrownWinner = bestPlayerFound;
    }
}

[HarmonyPatch(typeof(StatManager), nameof(StatManager.OnServerStageBegin))]
public class GiveCrownToBestPlayerOnNewStage
{
    private static int stagesEntered = 0;

    [HarmonyPostfix]
    private static void GiveCrownToBestPlayer()
    {
        stagesEntered++;

        if (stagesEntered < 2)
        {
            return;
        }

        CrownDistribution.EvaluateBestPlayer();
        CrownDistribution.AwardBestPlayer();
    }
}