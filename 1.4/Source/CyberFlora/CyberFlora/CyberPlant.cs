using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;


namespace Ruyu.CyberFlora
{
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
                    stringBuilder.AppendLine("GrowthRate".Translate() + ": " + fertility.ToStringPercent());
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
}
