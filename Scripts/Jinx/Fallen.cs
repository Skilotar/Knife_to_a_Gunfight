using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections.Generic;

namespace Knives
{
    class Fallen_armor : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Armor of the fallen";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Fallen Armor";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Fallen_armor>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Outcast";
            string longDesc =

                "Temporary stat boost after falling in pits, less damage to jammed enemies. An armor worn by the fallen aimgels." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1);


            item.CanBeDropped = false;
            item.PersistsOnDeath = true;
            item.quality = PickupObject.ItemQuality.C;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            item.ArmorToGainOnInitialPickup = 1;
            itemID = item.PickupObjectId;
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Immunity to pits and power after falling, Less damage to jammed enemies.";

        }

        public static int itemID;

        public override void Pickup(PlayerController player)
        {
            player.ImmuneToPits.SetOverride("fallen_Armor", true, null);
            player.PostProcessProjectile += this.PostProcessProjectile;
            base.Pickup(player);
        }

        private void PostProcessProjectile(Projectile arg1, float arg2)
        {
           arg1.BlackPhantomDamageMultiplier = .85f;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.ImmuneToPits.SetOverride("fallen_Armor", false, null);
            player.PostProcessProjectile -= this.PostProcessProjectile;
            return base.Drop(player);
        }
        bool doingbuffroutine = false;
        public override void  Update()
        {
            
            if (this.Owner != null)
            {
                if (this.Owner.IsFalling && !doingbuffroutine)
                {
                    StartCoroutine(buffroutine());
                }
            }
            base.Update();
        }

        private IEnumerator buffroutine()
        {
            doingbuffroutine = true;
            this.Owner.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.RateOfFire, StatModifier.ModifyMethod.ADDITIVE, .25f));
            this.Owner.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.MovementSpeed, StatModifier.ModifyMethod.ADDITIVE, 1f));
            this.Owner.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.ADDITIVE, 1f));
            this.Owner.stats.RecalculateStats(Owner, true);
            yield return new WaitForSeconds(12);
            this.Owner.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.ADDITIVE, -1f));
            this.Owner.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.MovementSpeed, StatModifier.ModifyMethod.ADDITIVE, -1f));
            this.Owner.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.RateOfFire, StatModifier.ModifyMethod.ADDITIVE, -.25f));
            this.Owner.stats.RecalculateStats(Owner, true);
            doingbuffroutine = false;
        }

       

    }
}
