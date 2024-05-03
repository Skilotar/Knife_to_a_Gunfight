using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using System.Reflection;
using HutongGames.PlayMaker.Actions;

namespace Knives
{
    class Long_roll_boots : PassiveItem
    {

        public static void Register()
        {
            string itemName = "Long Roll Boots";

            string resourceName = "Knives/Resources/long_roll_boots";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Long_roll_boots>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "*beewwww*";
            string longDesc = "An apeture science prototype for the long fall boots. This early model attempted to negate falling injuries by slowing the timestream of the person jumping. \n\n Turns out slowing time doesnt slow momentum. The subjects still splatted... just really slowly.\n\n" +
                "Oh well all in the name of science I guess. This specific version of the boots is equiped with temporally repositioning airbreaks which let you control your movement mid roll. Good luck and happy rolling." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
           
            
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.DodgeRollSpeedMultiplier, -.3f, StatModifier.ModifyMethod.ADDITIVE);
            



            item.quality = PickupObject.ItemQuality.C;
            item.RespawnsIfPitfall = true;
            item.IgnoredByRat = true;
            ID = item.PickupObjectId;
        }

        public static int ID;
        bool toggleknockbackeffect = false;
        public override void Pickup(PlayerController player)
        {
          
            player.OnPreDodgeRoll += this.OnPreDodgeRoll;
            base.Pickup(player);
        }

        private void OnPreDodgeRoll(PlayerController player)
        {
            toggleknockbackeffect = true;
            
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnPreDodgeRoll -= this.OnPreDodgeRoll;
            return base.Drop(player);
        }
        
        public override void  Update()
        {
            if (this.Owner != null)
            {
                if (Time.timeScale <= 0f)
                {

                }
                else
                {

                    PlayerController player = (PlayerController)this.Owner;
                    if (toggleknockbackeffect && player.IsDodgeRolling)
                    {

                        player.knockbackDoer.ApplyKnockback(player.NonZeroLastCommandedDirection, .6f * (player.stats.MovementSpeed/7));
                    }
                    else
                    {
                        if (toggleknockbackeffect == true)
                        {
                            player.knockbackDoer.ApplyKnockback(player.NonZeroLastCommandedDirection * -1, .35f);
                            toggleknockbackeffect = false;
                        }

                    }
                }

            }
               
         


            base.Update();
        }

    }
}
