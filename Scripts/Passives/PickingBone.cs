using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;

namespace Knives
{
    class Picking_Bone : PassiveItem
    {
        public static void Register()
        {

            string itemName = "Picking Bone";


            string resourceName = "Knives/Resources/Picking_bone";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<Picking_Bone>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Bad To The Bone";
            string longDesc = "An ancient pick that oozes tar in an unnatural and unsettling fashion. Deal Large damage back to an enemy that hits you with a projectile. \n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1);




            item.quality = PickupObject.ItemQuality.C;
            ID = item.PickupObjectId;

        }
        public static int ID;

        public override void Pickup(PlayerController player)
        {

            
            
            base.Pickup(player);
        }

 

        public override DebrisObject Drop(PlayerController player)
        {
            

            return base.Drop(player);
        }

        public bool Doingit { get; private set; }

        public override void Update()
        {
            base.Update();
            if (this.Owner)
            {
                Hitcalc(this.Owner);
            }

        }

        public void Hitcalc(PlayerController player)
        {
            
            foreach (var projectile in GetBullets())
            {

                if (projectile.gameObject.GetComponent<EnemyProjOnHitPlayer>() == null)
                {
                    projectile.gameObject.GetOrAddComponent<EnemyProjOnHitPlayer>();
                }
                else
                {
                    EnemyProjOnHitPlayer pick = projectile.gameObject.GetOrAddComponent<EnemyProjOnHitPlayer>();
                    if(pick.hit == true)
                    {
                        if(pick.owner != null)
                        {
                            if (pick.owner.healthHaver.IsAlive)
                            {
                                //ETGModConsole.Log(1);
                                pick.owner.healthHaver.ApplyDamage(75, Vector2.zero, "BONED", CoreDamageTypes.Void, DamageCategory.Unstoppable, true, null,false);
                            }
                        }
                       
                    }
                }

            }
        }
        private static List<Projectile> GetBullets()
        {
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                if (projectile && projectile.sprite && !projectile.ImmuneToBlanks && !projectile.ImmuneToSustainedBlanks)
                {
                    if (projectile.Owner != null)
                    {
                        if (projectile.collidesWithEnemies = false || projectile.isFakeBullet || projectile.Owner is AIActor || (projectile.Shooter != null && projectile.Shooter.aiActor != null) || projectile.ForcePlayerBlankable || projectile.IsBulletScript)
                        {
                            list.Add(projectile);
                        }
                    }
                }
            }
            return list;
        }
    }
}