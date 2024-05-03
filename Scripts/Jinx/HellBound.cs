using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class HellBound : PassiveItem
    {
        public static void Register()
        {

            string itemName = "Hell Bound";


            string resourceName = "Knives/Resources/Hell_bound";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<HellBound>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Unrelenting";
            string longDesc = "Move slower gain trail of fire. An abnormally hot ball and chain that feels heavier than it looks. It whispers your name and drags fire into the earth. \n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            item.quality = PickupObject.ItemQuality.C;
            item.CanBeDropped = false;
            item.PersistsOnDeath = true;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, -.7f);
            itemID = item.PickupObjectId;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Move slower gain trail of fire";


        }
        public static int itemID;
        public override void Pickup(PlayerController player)
        {
            this.m_fireImmunity = new DamageTypeModifier();
            this.m_fireImmunity.damageMultiplier = 0f;
            this.m_fireImmunity.damageType = CoreDamageTypes.Fire;
            player.healthHaver.damageTypeModifiers.Add(this.m_fireImmunity);
            base.Pickup(player);
        }
        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.damageTypeModifiers.Remove(this.m_fireImmunity);
            return base.Drop(player);
        }
        public override void  Update()
        {
            base.Update();
            if (this.Owner != null)
            {
                if(this.Owner.Velocity.magnitude > 0)
                {
                    GoopDefinition fire = PickupObjectDatabase.GetById(242).GetComponent<DirectionalAttackActiveItem>().goopDefinition;
                    fire.lifespan = 10f;
                    DeadlyDeadlyGoopManager goop = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(fire);
                    goop.TimedAddGoopCircle(this.Owner.sprite.WorldBottomCenter, .5f, .05f, true);
                }
                
            }

        }
        public DamageTypeModifier m_fireImmunity;
    }
}