using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;

namespace Knives
{

    class PeaceStandard : PassiveItem
    {
        public static void Register()
        {

            string itemName = "Peace Standard";


            string resourceName = "Knives/Resources/PeaceStandard";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<PeaceStandard>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Give Peace A Chance";
            string longDesc = "Reminder to spread Love an Peace as long as the person agrees with you and isnt Oppressing your rights Duuuuuuuuuuuuuuuuude. " +
                "_______________________________________________________________________________\n" +
                "Deals more damage the longer the player does not deal damage for in combat.\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item




            item.quality = PickupObject.ItemQuality.C;
            
            ID = item.PickupObjectId;
        }

        public static int ID;
        public float counter = 0;
        public float synergyCounter = 0;
        public bool Maxed = false;
        
        public int token = 0;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            player.OnDealtDamage += this.dealtDamage;
            counter = 0;
            Maxed = false;

        }

        private void dealtDamage(PlayerController arg1, float arg2)
        {
            counter = 0;
            Maxed = false;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnDealtDamage -= this.dealtDamage;

            return base.Drop(player);
        }
        public override void  Update()
        {
            if(this.Owner != null)
            {
                if (this.Owner.IsInCombat && !manager.IsPaused)
                {

                    if (Time.timeScale <= 0f)
                    {

                    }
                    else
                    {

                        if (counter <= 4) // accrew Stat over 4 seconds
                        {
                            Maxed = false;
                            counter += Time.deltaTime / 4;
                            float buff = Mathf.Lerp(0, 1, counter / 4);
                            RemoveStat(PlayerStats.StatType.Damage);
                            AddStat(PlayerStats.StatType.Damage, buff);
                            this.Owner.stats.RecalculateStats(Owner, true);
                        }
                        if(counter >= 4)
                        {
                            if (Maxed == false)
                            {
                                AkSoundEngine.PostEvent("Play_OBJ_power_up_01", base.gameObject);
                                Maxed = true;
                            }
                        }

                        if(this.Owner.PlayerHasActiveSynergy("Double Standards"))
                        {
                            if (synergyCounter <= .5f)
                            {
                                synergyCounter += Time.deltaTime;
                            }
                            else
                            {
                                synergyCounter = 0;
                                foreach (AIActor friend in this.Owner.companions)
                                {
                                    if (friend.healthHaver.IsAlive && friend.healthHaver.GetCurrentHealth() != friend.healthHaver.GetMaxHealth())
                                    {
                                        friend.healthHaver.ApplyHealing(5f);
                                        AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", base.gameObject);
                                        synergyCounter = 0;
                                    }
                                }

                            }
                        }
                        
                    }


                }
                else
                {
                    
                    RemoveStat(PlayerStats.StatType.Damage);
                    this.Owner.stats.RecalculateStats(Owner, true);
                    counter = 0;
                    
                }

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



        public static GameManager manager = new GameManager();
    }

}

