using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brave.BulletScript;
using Dungeonator;
using Gungeon;
using ItemAPI;
using MultiplayerBasicExample;
using UnityEngine;
namespace Knives
{
    class Slide_tech :PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Slide Tech Slide";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/slide_slide";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Slide_tech>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Slip Sliding Away";
            string longDesc = "Weeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee!!!\n\n" +
                "*Crash*" +
                "\n\n\n - Knife_to_a_Gunfight"
                ;

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            //Set the rarity of the item

            item.quality = PickupObject.ItemQuality.D;
        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            
        }

        public override void  Update()
        {
            base.Update();
            if(this.Owner != null)
            {
                if (this.Owner.IsSlidingOverSurface)
                {
                    DoDaGoo();
                }
                else
                {
                    DoDaFreeeez();
                }
                if (limiter == 1)
                {
                    counter++;

                }
                if (speedTimer != 0)
                {
                    speedTimer--;

                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, 4);
                    this.Owner.stats.RecalculateStats(Owner, true);
                }
                else
                {
                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, 0);
                    this.Owner.stats.RecalculateStats(Owner, true);
                }
            }
            
           
        }
        public int counter;
        public int speedTimer = 0;
        public void DoDaGoo()
        {

            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
            string text = "assets/data/goops/water goop.asset";
            GoopDefinition goopDefinition;
            try
            {
                GameObject gameObject = assetBundle.LoadAsset(text) as GameObject;
                goopDefinition = gameObject.GetComponent<GoopDefinition>();
            }
            catch
            {
                goopDefinition = (assetBundle.LoadAsset(text) as GoopDefinition);
            }
            goopDefinition.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
            DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goopDefinition);
            goopManagerForGoopType.TimedAddGoopCircle(this.Owner.sprite.WorldCenter, 10f, 1f, false);

            speedTimer = 300;
            counter = 0;
            limiter = 1;
        }
        public int limiter;
        public void DoDaFreeeez()
        {

            if (limiter == 1 && counter == 30)
            {
                
                DeadlyDeadlyGoopManager.FreezeGoopsCircle(this.Owner.sprite.WorldCenter, 40f);
                
                limiter = 0;
                counter = 0;
               
            }
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
