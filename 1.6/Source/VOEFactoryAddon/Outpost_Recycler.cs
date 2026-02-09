using System.Collections.Generic;
using System.Linq;
using Outposts;
using RimWorld;
using Verse;

namespace VOEFactoryAddon
{
    public class Outpost_Recycler : Outpost
    {
        private IEnumerable<Thing> SmeltProducts(Thing thing, float efficiency)
        {
            List<ThingDefCountClass> costListAdj = thing.def.CostListAdjusted(thing.Stuff);
            for (int j = 0; j < costListAdj.Count; j++)
            {
                if (!costListAdj[j].thingDef.intricate && costListAdj[j].thingDef.smeltable)
                {
                    int num = GenMath.RoundRandom((float)costListAdj[j].count * efficiency);
                    if (num > 0)
                    {
                        Thing newThing = ThingMaker.MakeThing(costListAdj[j].thingDef);
                        newThing.stackCount = num;
                        yield return newThing;
                    }
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
                resourcesToReturn.AddRange(SmeltProducts(item, efficiency));

                TakeItem(item);
                item.Destroy();
            }

            List<Thing> mergedResources = new List<Thing>();
            foreach (var group in resourcesToReturn.GroupBy(t => t.def))
            {
                int total = group.Sum(t => t.stackCount);
                mergedResources.AddRange(group.Key.Make(total));
            }

            if (mergedResources.Count > 0) Deliver(mergedResources);
        }

        public override string ProductionString() =>
            "VOEFactory_RecyclingInProgress".Translate(Things.Count(t => t.def.category == ThingCategory.Item), TimeTillProduction);
    }
}
