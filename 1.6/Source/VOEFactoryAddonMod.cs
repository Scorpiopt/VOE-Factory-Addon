using HarmonyLib;
using Verse;

namespace VOEFactoryAddon
{
    public class VOEFactoryAddonMod : Mod
    {
        public VOEFactoryAddonMod(ModContentPack pack) : base(pack)
        {
            new Harmony("VOEFactoryAddonMod").PatchAll();
        }
    }
}