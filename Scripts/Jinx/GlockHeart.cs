using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Gungeon;



namespace Knives
{
    class MiniGlockodileHeart : PassiveItem
    {
        //Call this method from the Start() method of your ETGModule extension
        public static void Register()
        {
            //The name of the item
            string itemName = "Mini Glockodile Heart";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/miniGlockodileHeart";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<MiniGlockodileHeart>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Glass Half Empty";
            string longDesc = "A heart torn from a mighty repilian beast. Halves users current health, but gain max health for every 50 kills." +
            "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item



            //Set the rarity of the item
            item.CanBeDropped = false;
            item.PersistsOnDeath = true;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            item.quality = PickupObject.ItemQuality.A;
            itemID = item.PickupObjectId;

            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Halves your current health, Gain empty heartcontainers with every 30 kills";
        }
        public static int itemID;
        public int killsSincePickedUp;
        public override void Pickup(PlayerController player)
        {
            float health = player.healthHaver.GetMaxHealth();
            float HalvedHealth = health/2;
            HalvedHealth = (float)Math.Truncate(HalvedHealth);
           
            
            player.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.Health, StatModifier.ModifyMethod.ADDITIVE, -HalvedHealth));
            base.Pickup(player);

            player.OnKilledEnemy += Player_OnKilledEnemy;
        }
        public int Token;
        private void Player_OnKilledEnemy(PlayerController obj)
        {
            killsSincePickedUp++;
            if(killsSincePickedUp % 30 == 0) //every 30 kills
            {
                Token++;
                RemoveStat(PlayerStats.StatType.Health);
                AddStat(PlayerStats.StatType.Health, Token);
                this.Owner.stats.RecalculateStats(Owner, true);


            }
        }

        public override DebrisObject Drop(PlayerController player)
        {

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