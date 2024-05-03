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
            string longDesc = "Reminder to spread 'Love' an 'Peace' as long as the person agrees with you and isnt 'Oppressing' your rights Duuuuuuuuuuuuuuuuude. " +
                "_______________________________________________________________________________\n" +
                "Deals more damage the longer the player does not kill for in combat.\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item




            item.quality = PickupObject.ItemQuality.C;
            item.CanBeDropped = false;
            ID = item.PickupObjectId;
        }

        public static int ID;
        public int counter = 0;
        public int synergyCounter = 0;
        public int token = 0;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            player.OnKilledEnemy += this.OnKilledEnemy;


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
                        counter++;
                        if (this.Owner.HasPickupID(529))
                        {
                            synergyCounter++;
                        }
                    }


                }


                if (counter == 450)
                {
                    if(token <= 2)
                    {
                        token++;
                    }
                    RemoveStat(PlayerStats.StatType.Damage);
                    AddStat(PlayerStats.StatType.Damage, token / 2);
                    this.Owner.stats.RecalculateStats(Owner, true);
                    counter = 0;
                }

                base.Update();

                if (synergyCounter == 2000)
                {
                    foreach (AIActor friend in this.Owner.companions)
                    {
                        if (friend.healthHaver.GetCurrentHealth() > 0f)
                        {
                            friend.healthHaver.ApplyHealing(10f);

                           
                            StartVFX(friend);
                            synergyCounter = 0;
                        }
                    }
                    AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", base.gameObject);
                }
                if (synergyCounter == 50)
                {
                    foreach (AIActor friend in this.Owner.companions)
                    {
                        if (friend.healthHaver.GetCurrentHealth() > 0f)
                        {

                            StopVFX(friend);

                        }
                    }
                }

            }

        }

        private void StartVFX(AIActor user)
        {
            Material outline = SpriteOutlineManager.GetOutlineMaterial(user.sprite);
            outline.SetColor("_OverrideColor", new Color(63f, 236f, 165f));
        }


        private void StopVFX(AIActor user)
        {
            Material outline = SpriteOutlineManager.GetOutlineMaterial(user.sprite);
            outline.SetColor("_OverrideColor", new Color(0f, 0f, 0f));
        }

        public void OnKilledEnemy(PlayerController user)
        {
            if(token > 0)
            {
                token --;
                token --;
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



        public static GameManager manager = new GameManager();
    }

}

