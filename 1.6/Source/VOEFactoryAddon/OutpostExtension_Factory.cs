using System.Collections.Generic;
using Outposts;
using RimWorld;

namespace VOEFactoryAddon
{
    public class OutpostExtension_Factory : OutpostExtension_Choose
    {
        public FactoryType factoryType;
        public List<TechLevel> allowedTechLevels;
        public float targetMarketValuePerPawn;
        public AmountBySkill baseSkillRequirement;
    }
}
