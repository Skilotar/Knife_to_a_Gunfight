using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Brave.BulletScript;
using System.Collections;

namespace Knives
{
    class Danger_dance :PassiveItem
    {
        private bool boostOn = false;

        public static void Register()
        {
            string itemName = "Risky Headband";

            string resourceName = "Knives/Resources/Danger_dance";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Danger_dance>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Flirting With Death";
            string longDesc = "You've never shyed away from a good challenge and you're not gonna start now. Narrowly avoiding bullets gives a temporary damage boost." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item




            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.EnemyProjectileSpeedMultiplier, -.3f, StatModifier.ModifyMethod.ADDITIVE);


            item.quality = PickupObject.ItemQuality.D;


        }
        public override void Pickup(PlayerController player)
        {
           
            base.Pickup(player);
        }
        public bool hasBuff = false;
        public override void  Update()
        {
            base.Update();
            if (this.Owner != null)
            {
                if (boostOn == true && hasBuff == false)
                {
                    RemoveStat(PlayerStats.StatType.Damage);
                    AddStat(PlayerStats.StatType.Damage, .25f);
                    this.Owner.stats.RecalculateStats(Owner, true);
                    hasBuff = true;
                }
                if(boostOn == false && hasBuff)
                {
                    RemoveStat(PlayerStats.StatType.Damage);
                    AddStat(PlayerStats.StatType.Damage, 0f);
                    this.Owner.stats.RecalculateStats(Owner, true);
                    hasBuff = false;
                }
                proximity(this.Owner);
                
            }
        }
       
        public void proximity(PlayerController player)
        {   


            Vector2 standing = player.CenterPosition;
            foreach (var projectile in GetBullets())
            {
               
                if (player.healthHaver.IsAlive)
                {
                    Vector2 bullet = projectile.LastPosition;

                    float radius = 1.8f;
                    if (Vector2.Distance(bullet, standing) < radius && !boostOn)
                    {
                        
                        this.Owner.BloopItemAboveHead(base.sprite, "Knives/Resources/Danger_dance");
                        boostOn = true;
                        StartCoroutine(boosttimer());
                      
                    }
                   
                    
                } 
            }

        }
      
        private IEnumerator boosttimer()
        {
            yield return new WaitForSeconds(4);
            boostOn = false;
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

        public override DebrisObject Drop(PlayerController player)
        {
            
            return base.Drop(player);
        }
      
    }


}
