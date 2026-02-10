using System.Collections.Generic;
using System.Linq;
using Outposts;
using RimWorld;
using Verse;

namespace VOEFactoryAddon
{
    public class Outpost_Recycler : Outpost
    {
        private IEnumerable<Thing> RecycleThing(Thing thing, float efficiency)
        {
            List<ThingDefCountClass> costListAdj = thing.def.CostListAdjusted(thing.Stuff);
            for (int j = 0; j < costListAdj.Count; j++)
            {
                int num = GenMath.RoundRandom((float)costListAdj[j].count * efficiency);
                if (num > 0)
                {
                    Thing newThing = ThingMaker.MakeThing(costListAdj[j].thingDef);
                    newThing.stackCount = num;
                    yield return newThing;
                }
            }
            if (thing.def.smeltProducts != null)
            {
                for (int j = 0; j < thing.def.smeltProducts.Count; j++)
                {
                    ThingDefCountClass thingDefCountClass = thing.def.smeltProducts[j];
                    Thing newThing = ThingMaker.MakeThing(thingDefCountClass.thingDef);
                    newThing.stackCount = thingDefCountClass.count;
                    yield return newThing;
                }
            }
        }

        public override void Produce()
        {
            List<Thing> inventory = Things.Where(t => t.def.category == ThingCategory.Item).ToList();
            if (inventory.Count == 0) return;

            List<Thing> resourcesToReturn = new List<Thing>();
            float efficiency = 0.5f;

            foreach (var item in inventory)
            {
                resourcesToReturn.AddRange(RecycleThing(item, efficiency));

                TakeItem(item);
                item.Destroy();
            }

            if (resourcesToReturn.Count > 0) Deliver(resourcesToReturn);
        }

        public override string ProductionString() =>
            "VOEFactory_RecyclingInProgress".Translate(Things.Count(t => t.def.category == ThingCategory.Item), TimeTillProduction);
    }
}
