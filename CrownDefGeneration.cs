
using RoR2;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using HarmonyLib;

namespace Crown_Mod;

public class CrownItemDisplayRule
{
    [HarmonyPatch(typeof(ItemDisplayRuleSet), nameof(ItemDisplayRuleSet.Init))]
    private class AddItemDisplayRules
    {
        private static void PatchCharacterModel(CharacterModel model)
        {
            if (!model.itemDisplayRuleSet)
            {
                model.itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            }

            model.itemDisplayRuleSet.SetDisplayRuleGroup(CrownItemDef.GetCrownDef(), new DisplayRuleGroup
            {
                rules = new ItemDisplayRule[1] { CharacterSpecific.GetCrownDisplayRuleForCharacter(model) }
            });
        }

        [HarmonyPrefix]
        private static void AddCustomDispRules()
        {
            foreach (GameObject character in BodyCatalog.allBodyPrefabs)
            {
                CharacterModel model = character.GetComponentInChildren<CharacterModel>();

                if (model)
                {
                    PatchCharacterModel(model);
                }
            }
        }
    }

    private static ItemDisplayRule crownDisplayRule;
    private static bool setCrownDisplayRule = false;

    public static MeshRenderer crownMeshRenderer
    {
        get
        {
            if (!CrownItemDef.crownGameobj)
            {
                return null;
            }

            return CrownItemDef.crownGameobj.GetComponentInChildren<MeshRenderer>();
        }
    }

    private static void SetParentedPrefabSettings()
    {
        crownDisplayRule.ruleType = ItemDisplayRuleType.ParentedPrefab;
        crownDisplayRule.limbMask = LimbFlags.None;

        crownDisplayRule.followerPrefab = CrownItemDef.crownGameobj;
        crownDisplayRule.followerPrefabAddress = new AssetReferenceGameObject("");

        crownDisplayRule.childName = "Head";
        crownDisplayRule.localScale = Vector3.one * 0.3f;
        crownDisplayRule.localPos = Vector3.up * 0.55f;

        crownDisplayRule.followerPrefab.AddComponent<SpinCrown>();
    }

    private static void SetRenderingInfo()
    {
        ItemDisplay itemDisplay = crownDisplayRule.followerPrefab.AddComponent<ItemDisplay>();

        itemDisplay.visibilityLevel = VisibilityLevel.Visible;
        MeshRenderer meshRenderer = CrownItemDisplayRule.crownMeshRenderer;

        itemDisplay.rendererInfos = new CharacterModel.RendererInfo[1] {
            new CharacterModel.RendererInfo {
                renderer = meshRenderer,
                defaultMaterial = meshRenderer.material,
                defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                ignoreOverlays = false,
                hideOnDeath = true
            }
        };
    }

    public static ItemDisplayRule GetCrownDisplayRule()
    {
        if (setCrownDisplayRule)
        {
            return crownDisplayRule;
        }

        crownDisplayRule = new ItemDisplayRule();

        SetParentedPrefabSettings();

        SetRenderingInfo();

        setCrownDisplayRule = true;

        UnitTest.TestShaderConfig();

        return crownDisplayRule;
    }
}

public class CrownItemDef
{
    public const string CROWN_ITEM_NAME = "MVP Crown";
    public const ItemTier CROWN_ITEM_TIER = ItemTier.NoTier;

    private static ItemDef crownItemDef = null;

    public static GameObject crownGameobj
    {
        get
        {
            if (!crownItemDef)
            {
                return null;
            }

            return crownItemDef.pickupModelPrefab;
        }
    }

    private static string ResourcesFolder
    {
        get
        {
            return System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }

    private static void SetCrownDescriptions()
    {
        crownItemDef.name = CROWN_ITEM_NAME;
        crownItemDef.nameToken = CROWN_ITEM_NAME;

        crownItemDef.descriptionToken = "Congrats Your The Best Player In The Lobby!";
        crownItemDef.loreToken = "Wow, i didn't even know that this would ever be visible :)";
        crownItemDef.pickupToken = "Congrats Your The Best Player In The Lobby!";
    }

    private static void SetItemDefDisplayProperties()
    {
        crownItemDef.pickupIconSprite = Helpers.LoadSpriteFromName("CrownItemInventoryImage.png");

        AssetBundle prefabsBundle = AssetBundle.LoadFromFile(ResourcesFolder + "\\crownbundle");
        crownItemDef.pickupModelPrefab = prefabsBundle.LoadAsset<GameObject>("Crown");

        crownItemDef.hidden = false;
    }

    private static void SetRequiredMisc()
    {
        crownItemDef.unlockableDef = null;
        crownItemDef.tier = CROWN_ITEM_TIER;
        crownItemDef.canRemove = false;

        crownItemDef.tags = new ItemTag[] { ItemTag.CannotDuplicate, ItemTag.CannotCopy, ItemTag.CannotSteal};
    }

    public static ItemDef GetCrownDef()
    {
        if (crownItemDef)
        {
            return crownItemDef;
        }

        crownItemDef = ScriptableObject.CreateInstance<ItemDef>();

        SetCrownDescriptions();
        SetItemDefDisplayProperties();
        SetRequiredMisc();

        return crownItemDef;
    }
}