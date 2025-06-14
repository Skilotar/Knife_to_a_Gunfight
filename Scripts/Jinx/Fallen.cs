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
            string longDesc = "Increase damage while flying over pits, less damage to jammed enemies. \n" +

                "An armor worn by the fallen aimgels." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 2);


            item.CanBeDropped = false;
            item.PersistsOnDeath = true;
            item.quality = PickupObject.ItemQuality.C;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            item.ArmorToGainOnInitialPickup = 1;
            itemID = item.PickupObjectId;
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Increase damage while flying over pits, less damage to jammed enemies.";

        }

        public static int itemID;

        public override void Pickup(PlayerController player)
        {
            player.SetIsFlying(true, "FallenArmor");
            player.AdditionalCanDodgeRollWhileFlying.SetOverride("FallenArmor", true);
            player.PostProcessProjectile += this.PostProcessProjectile;
            base.Pickup(player);
            prefabToAttachToPlayer = ((PickupObjectDatabase.GetById(307).gameObject.GetComponent<WingsItem>().prefabToAttachToPlayer));

            this.instanceWing = player.RegisterAttachedObject(this.prefabToAttachToPlayer, "jetpack", 0f);
            this.instanceWingSprite = this.instanceWing.GetComponent<tk2dSprite>();
            instanceWingSprite.color = ExtendedColours.paleYellow;
        }

        private void PostProcessProjectile(Projectile arg1, float arg2)
        {
           arg1.BlackPhantomDamageMultiplier = .85f;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.SetIsFlying(false, "FallenArmor");
            player.AdditionalCanDodgeRollWhileFlying.RemoveOverride("FallenArmor");
            player.PostProcessProjectile -= this.PostProcessProjectile;
            UnityEngine.GameObject.Destroy(instanceWing);
            UnityEngine.GameObject.Destroy(instanceWingSprite);
            return base.Drop(player);
        }
        bool doingbuffroutine = false;
        public override void  Update()
        {
            
            if (this.Owner != null)
            {
                if (this.Owner.IsFlying)
                {
                    if (this.Owner.CurrentRoom != null)
                    {
                        CellData cell = this.Owner.CurrentRoom.GetNearestCellToPosition(this.Owner.CenterPosition);
                        if(cell.type == CellType.PIT)
                        {
                            RemoveStat(PlayerStats.StatType.Damage);
                            AddStat(PlayerStats.StatType.Damage, .75f);
                            this.Owner.stats.RecalculateStats(Owner, true);
                        }
                        else
                        {
                            RemoveStat(PlayerStats.StatType.Damage);
                            AddStat(PlayerStats.StatType.Damage, 0f);
                            this.Owner.stats.RecalculateStats(Owner, true);
                        }
                    }
                }
            }
            base.Update();
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

        private tk2dSprite instanceWingSprite;
        private GameObject instanceWing;
        public GameObject prefabToAttachToPlayer;
        
    }
}
