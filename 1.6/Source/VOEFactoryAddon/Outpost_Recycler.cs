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
            Log.Message($"[Outpost_Recycler] SmeltProducts START - defName: {thing.def.defName}, stackCount: {thing.stackCount}, stuff: {thing.Stuff?.defName ?? "null"}, efficiency: {efficiency}");
            List<ThingDefCountClass> costListAdj = thing.def.CostListAdjusted(thing.Stuff);
            Log.Message($"[Outpost_Recycler] CostListAdjusted count: {costListAdj.Count}");
            for (int j = 0; j < costListAdj.Count; j++)
            {
                Log.Message($"[Outpost_Recycler] Checking costListAdj[{j}]: {costListAdj[j].thingDef.defName}, count: {costListAdj[j].count}, intricate: {costListAdj[j].thingDef.intricate}, smeltable: {costListAdj[j].thingDef.smeltable}");
                if (!costListAdj[j].thingDef.intricate && costListAdj[j].thingDef.smeltable)
                {
                    int num = GenMath.RoundRandom((float)costListAdj[j].count * efficiency);
                    Log.Message($"[Outpost_Recycler] Calculated num: {num}");
                    if (num > 0)
                    {
                        Thing newThing = ThingMaker.MakeThing(costListAdj[j].thingDef);
                        newThing.stackCount = num;
                        Log.Message($"[Outpost_Recycler] Yielding from costListAdj: {newThing.def.defName} x{newThing.stackCount}");
                        yield return newThing;
                    }
                }
            }
            if (thing.def.smeltProducts != null)
            {
                Log.Message($"[Outpost_Recycler] smeltProducts count: {thing.def.smeltProducts.Count}");
                for (int j = 0; j < thing.def.smeltProducts.Count; j++)
                {
                    ThingDefCountClass thingDefCountClass = thing.def.smeltProducts[j];
                    Thing newThing = ThingMaker.MakeThing(thingDefCountClass.thingDef);
                    newThing.stackCount = thingDefCountClass.count;
                    Log.Message($"[Outpost_Recycler] Yielding from smeltProducts: {newThing.def.defName} x{newThing.stackCount}");
                    yield return newThing;
                }
            }
            else
            {
                Log.Message("[Outpost_Recycler] smeltProducts is null");
            }
            Log.Message("[Outpost_Recycler] SmeltProducts END");
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

            if (resourcesToReturn.Count > 0) Deliver(resourcesToReturn);
        }

        public override string ProductionString() =>
            "VOEFactory_RecyclingInProgress".Translate(Things.Count(t => t.def.category == ThingCategory.Item), TimeTillProduction);
    }
}
