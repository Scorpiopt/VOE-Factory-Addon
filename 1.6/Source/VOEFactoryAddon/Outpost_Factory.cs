using System;
using System.Collections.Generic;
using System.Linq;
using Outposts;
using RimWorld;
using UnityEngine;
using Verse;

namespace VOEFactoryAddon
{
    public enum FactoryType { Melee, Ranged, Drugs, Textiles, Prosthetics, Apparel }

    public class Outpost_Factory : Outpost_ChooseResult
    {
        private OutpostExtension_Factory ExtFactory => Ext as OutpostExtension_Factory;
        private List<ThingDef> candidates;
        public override IEnumerable<ResultOption> GetExtraOptions()
        {
            if (candidates == null)
            {
                candidates = DefDatabase<ThingDef>.AllDefs.Where(t =>
                                !t.IsBlueprint && !t.IsFrame && !t.IsCorpse && t.PlayerAcquirable &&
                                (ExtFactory.allowedTechLevels.NullOrEmpty() || ExtFactory.allowedTechLevels.Contains(t.techLevel)) &&
                                MatchesFactoryType(t)
                            ).ToList();
            }

            foreach (var thing in candidates)
            {
                float value = thing.BaseMarketValue;
                if (value <= 0) continue;

                int count = Mathf.Max(1, Mathf.RoundToInt(ExtFactory.targetMarketValuePerPawn / value));

                if (thing.IsWeapon || thing.IsApparel)
                    count = Mathf.Clamp(count, 1, 5);

                yield return new ResultOption
                {
                    Thing = thing,
                    AmountPerPawn = count,
                    MinSkills = ExtFactory.baseSkillRequirement != null ? new List<AmountBySkill> { ExtFactory.baseSkillRequirement } : null
                };
            }
        }

        private bool MatchesFactoryType(ThingDef t)
        {
            switch (ExtFactory.factoryType)
            {
                case FactoryType.Melee:
                    return t.IsMeleeWeapon;

                case FactoryType.Ranged:
                    if (t.IsRangedWeapon) return true;
                    if (t.tradeTags != null && (t.tradeTags.Contains("CE_Ammo") || t.tradeTags.Contains("Ammunition"))) return true;
                    return false;

                case FactoryType.Drugs:
                    return t.IsDrug && t.ingestible?.drugCategory != DrugCategory.Medical;

                case FactoryType.Textiles:
                    if (t.IsLeather || t.IsWool) return true;
                    if (t.IsStuff && t.stuffProps.categories.Contains(StuffCategoryDefOf.Fabric)) return true;
                    return false;

                case FactoryType.Prosthetics:
                    return t.isTechHediff;

                case FactoryType.Apparel:
                    return t.IsApparel;

                default:
                    return false;
            }
        }
    }
}
