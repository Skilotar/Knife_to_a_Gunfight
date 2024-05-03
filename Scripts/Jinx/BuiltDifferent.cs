using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Gungeon;



namespace Knives
{
    class BuiltDifferent : PassiveItem
    {
        //Call this method from the Start() method of your ETGModule extension
        public static void Register()
        {
            //The name of the item
            string itemName = "Built Different";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Built_Different";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<BuiltDifferent>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "No Pain, No Gain!";
            string longDesc = "Increase power for each missing health, But gain size.\n\n" +
            "A patchwork mess of multiple stuffed animal parts. The tag on the back reads \'return to X\'." +
            "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, 1);

            //Set the rarity of the item
            item.CanBeDropped = false;
            item.quality = PickupObject.ItemQuality.C;
            item.PersistsOnDeath = true;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            itemID = item.PickupObjectId;

            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Gain health and scaling damage, But gain size.";
        }

        public static int itemID;
        public float gainsXmain = 0;
        public float gainsYmain = 0;
        public static float Scaler = 1.25f;
        public override void Pickup(PlayerController player)
        {
            player.healthHaver.OnHealthChanged += HealthHaver_OnHealthChanged;
            if (gainsXmain == 0 && gainsYmain == 0)
            {
                float gX = (1 * Scaler);
                float gY = (1 * Scaler);
                gainsXmain = gX;
                gainsYmain = gY;
            }

            player.transform.localScale = new Vector3(gainsYmain, gainsYmain, player.transform.localScale.z);
            player.specRigidbody.UpdateCollidersOnScale = true;
            player.specRigidbody.UpdateColliderPositions();
           
            base.Pickup(player);
        }

        private void HealthHaver_OnHealthChanged(float resultValue, float maxValue)
        {
            RemoveStat(PlayerStats.StatType.Damage);
            AddStat(PlayerStats.StatType.Damage, .15f * (maxValue - resultValue)); // .075 damage per missing heart
            this.Owner.stats.RecalculateStats(Owner, true);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.OnHealthChanged -= HealthHaver_OnHealthChanged;
            player.transform.localScale = new Vector3(1, 1, player.transform.localScale.z);
            player.specRigidbody.UpdateColliderPositions();

            return base.Drop(player);
        }

        private void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier modifier = new StatModifier();
            modifier.amount = amount;
            modifier.statToBoost = statType;
            modifier.modifyType = method;

            foreach (var m in passiveStatModifiers)
            {
                if (m.statToBoost == statType) return; //don't add duplicates
            }

            if (this.passiveStatModifiers == null)
                this.passiveStatModifiers = new StatModifier[] { modifier };
            else
                this.passiveStatModifiers = this.passiveStatModifiers.Concat(new StatModifier[] { modifier }).ToArray();
        }


        //Removes a stat
        private void RemoveStat(PlayerStats.StatType statType)
        {
            var newModifiers = new List<StatModifier>();
            for (int i = 0; i < passiveStatModifiers.Length; i++)
            {
                if (passiveStatModifiers[i].statToBoost != statType)
                    newModifiers.Add(passiveStatModifiers[i]);
            }
            this.passiveStatModifiers = newModifiers.ToArray();
        }

    }
}
