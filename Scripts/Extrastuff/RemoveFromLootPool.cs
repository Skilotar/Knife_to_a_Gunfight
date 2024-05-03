using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Knives
{
    public static class Remove_from_lootpool
    {
        public static PickupObject RemovePickupFromLootTables(this PickupObject po)
        {
            WeightedGameObject go1 = GameManager.Instance.RewardManager.GunsLootTable.defaultItemDrops.FindWeightedGameObjectInCollection(po);
            if (go1 != null)
            {
                GameManager.Instance.RewardManager.GunsLootTable.defaultItemDrops.elements.Remove(go1);
            }
            WeightedGameObject go2 = GameManager.Instance.RewardManager.ItemsLootTable.defaultItemDrops.FindWeightedGameObjectInCollection(po);
            if (go2 != null)
            {
                GameManager.Instance.RewardManager.ItemsLootTable.defaultItemDrops.elements.Remove(go2);
            }
            return po;
        }

        public static WeightedGameObject FindWeightedGameObjectInCollection(this WeightedGameObjectCollection collection, PickupObject po)
        {
            WeightedGameObject go = collection.FindWeightedGameObjectInCollection(po.PickupObjectId);
            if (go == null)
            {
                go = collection.FindWeightedGameObjectInCollection(po.gameObject);
            }
            return go;
        }

        public static WeightedGameObject FindWeightedGameObjectInCollection(this WeightedGameObjectCollection collection, int id)
        {
            foreach (WeightedGameObject go in collection.elements)
            {
                if (go.pickupId == id)
                {
                    return go;
                }
            }
            return null;
        }

        public static WeightedGameObject FindWeightedGameObjectInCollection(this WeightedGameObjectCollection collection, GameObject obj)
        {
            foreach (WeightedGameObject go in collection.elements)
            {
                if (go.gameObject == obj)
                {
                    return go;
                }
            }
            return null;
        }
    }
}
