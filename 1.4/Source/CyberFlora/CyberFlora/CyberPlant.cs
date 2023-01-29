using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace Ruyu.CyberFlora
{
    //Extension for xml
    public class CyberPlantMod : DefModExtension
    {

    }
    //Harmony call
    [StaticConstructorOnStartup]
    public static class CyberPlantPatch
    {
        static CyberPlantPatch()
        {
            Harmony val = new Harmony("Ruyu.CyberPlant");
            val.PatchAll();
        }
    }
    //Main cyberplant class for fertility
    public class CyberPlant : Plant
    {
        public float fertility = 1f;
        //Override of plant growthrate to allow growth in lower/higher temps
        public override float GrowthRate
        {
            get
            {
                if (Blighted)
                {
                    return 0f;
                }
                if (base.Spawned && !PlantUtility.GrowthSeasonNow(base.Position, base.Map))
                {
                    return 0f;
                }
                return fertility;
            } 
        }
        //Override inspection of plant fertility rate
        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (def.plant.showGrowthInInspectPane)
            {
                if (LifeStage == PlantLifeStage.Growing)
                {
                    stringBuilder.AppendLine("PercentGrowth".Translate(GrowthPercentString));
                    stringBuilder.AppendLine("GrowthRate".Translate() + ": "
                        + fertility.ToStringPercent());
                }
                else if (LifeStage == PlantLifeStage.Mature)
                {
                    stringBuilder.AppendLine("Mature".Translate());
                }
            }
            string text = InspectStringPartsFromComps();
            if (!text.NullOrEmpty())
            {
                stringBuilder.Append(text);
            }
            return stringBuilder.ToString().TrimEndNewlines();
        }
    }
    //Patch to change descriptions of cyber plants
    [HarmonyPatch(typeof(ThingDef), nameof(ThingDef.SpecialDisplayStats))]
    public static class Patch
    {
        public static void Postfix(ref IEnumerable<StatDrawEntry> __result, ThingDef __instance)
        {
            if (__instance.HasModExtension<CyberPlantMod>())
                __result = PassthroughMethod(__result, __instance);
        }
        public static IEnumerable<StatDrawEntry> PassthroughMethod(IEnumerable<StatDrawEntry> __result, ThingDef __instance)
        {
            float minTemp = -273f;
            float maxTemp = 1000f;
            float growMinGlow = 0f;
                foreach (StatDrawEntry entry in __result)
                {
                    if (entry.displayOrderWithinCategory == 4152)
                    {
                        yield return new StatDrawEntry(StatCategoryDefOf.Basics,
                            "MinGrowthTemperature".Translate(), minTemp.ToStringTemperature(),
                            "Stat_Thing_Plant_MinGrowthTemperature_Desc".Translate(), 4152);
                    }
                    else if (entry.displayOrderWithinCategory == 4153)
                    {
                        yield return new StatDrawEntry(StatCategoryDefOf.Basics,
                            "MaxGrowthTemperature".Translate(), maxTemp.ToStringTemperature(),
                            "Stat_Thing_Plant_MaxGrowthTemperature_Desc".Translate(), 4153);
                    }
                    else if(entry.displayOrderWithinCategory == 4154)
                {
                    yield return new StatDrawEntry(StatCategoryDefOf.Basics,
                        "LightRequirement".Translate(), growMinGlow.ToStringPercent(), 
                        "Stat_Thing_Plant_LightRequirement_Desc".Translate(), 4154);
                }
                    else
                    {
                        yield return entry;
                    }
                }
        }
    }
}