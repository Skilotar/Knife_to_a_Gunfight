using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Gungeon;



namespace Knives
{
    class WickerHeart : PassiveItem
    {
        //Call this method from the Start() method of your ETGModule extension
        public static void Register()
        {
            //The name of the item
            string itemName = "Wicker Heart";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Wicker_Heart";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<WickerHeart>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Kindled Love";
            string longDesc = "Gain Extra Health and Fire Vulnerability. A Woven wooden heart that reminds you of lost love." +
            "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, 1f, StatModifier.ModifyMethod.ADDITIVE);

            //Set the rarity of the item
            item.CanBeDropped = false;
            item.quality = PickupObject.ItemQuality.D;
            item.PersistsOnDeath = true;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            itemID = item.PickupObjectId;

            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Gain Extra Health, Fire Vulnerability.";
        }
        public static int itemID;
        public float knownHealth;
        public override void Pickup(PlayerController player)
        {
            knownHealth = player.healthHaver.GetCurrentHealth();
            player.healthHaver.OnDamaged += HealthHaver_OnDamaged;
            player.healthHaver.ModifyHealing += this.ModHeal;
            this.m_fireImmunity.damageMultiplier = .5f;
            this.m_fireImmunity.damageType = CoreDamageTypes.Fire;
            player.healthHaver.damageTypeModifiers.Add(this.m_fireImmunity);
            base.Pickup(player);
        }

        private void ModHeal(HealthHaver arg1, HealthHaver.ModifyHealingEventArgs arg2)
        {
            if (UnityEngine.Random.Range(0, 2f) < 1)
            {
                arg2.ModifiedHealing += .5f;
            }
        }

        private void HealthHaver_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            if (CoreDamageTypes.Fire == damageTypes)
            {
                if (DamageCategory.Environment == damageCategory)
                {
                    this.Owner.healthHaver.ApplyDamage(.5f, Vector2.zero, "Burned Alive");
                }
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.OnDamaged -= HealthHaver_OnDamaged;
            player.healthHaver.damageTypeModifiers.Remove(this.m_fireImmunity);
            return base.Drop(player);
        }

        public DamageTypeModifier m_fireImmunity;
    }
}

