using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using RoR2;
using RoR2.ContentManagement;

namespace Crown_Mod;

public class Helpers
{
    public static Sprite LoadSpriteFromName(string name)
    {
        string imgPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\\{name}";

        byte[] imgBytes = System.IO.File.ReadAllBytes(imgPath);

        Texture2D spriteTex = new Texture2D(0, 0);
        spriteTex.LoadImage(imgBytes);

        Sprite returnVal = Sprite.Create(spriteTex, new Rect(0, 0, spriteTex.width, spriteTex.height), new Vector2(0.5f, 0.5f));

        return returnVal;
    }
}


[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void CreateCrownModContentPack()
    {
        ContentPack crownModPack = new ContentPack();
        crownModPack.itemDefs.Add(new ItemDef[1] {
            CrownItemDef.GetCrownDef()
            }); // Adding the custom item to the pack

        crownModPack.identifier = "Crown_Mod_Pack";

        ContentPackLoader contentPackLoader = new ContentPackLoader(crownModPack);
        contentPackLoader.SetupLoadPack();
    }

    private void Awake()
    {
        Logger = base.Logger;

        CreateCrownModContentPack();

        Harmony thePatcher = new Harmony(MyPluginInfo.PLUGIN_GUID);
        thePatcher.PatchAll(Assembly.GetExecutingAssembly());
    }
}