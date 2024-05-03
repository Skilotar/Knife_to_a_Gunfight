using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using ItemAPI;
using MonoMod;
using Gungeon;


namespace Knives
{
    public class SecondWind : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Second Wind";

            string resourceName = "Knives/Resources/secondWind";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<SecondWind>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "It aint over yet folks!";
            string longDesc = "Steel yourself and draw up strenght to fight on! \n\nConsumes master rounds to cheat death." +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            item.quality = PickupObject.ItemQuality.A;


        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            HealthHaver healthHaver = player.healthHaver;
            healthHaver.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Combine(healthHaver.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage));
            base.Pickup(player);
        }
        public override DebrisObject Drop(PlayerController player)
        {
            
            HealthHaver healthHaver = player.healthHaver;
            healthHaver.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Remove(healthHaver.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage));
            return base.Drop(player);
            
        }
        public int masterRound;
        private void ModifyIncomingDamage(HealthHaver source, HealthHaver.ModifyDamageEventArgs args)
        {
            if(args.ModifiedDamage >= this.Owner.healthHaver.GetCurrentHealth()) // if is fatal
            {
                
                PlayerController player = (PlayerController)this.Owner;
               
                if (args == EventArgs.Empty)
                {
                    return;
                }
                foreach (PassiveItem item in Owner.passiveItems)
                {
                    if (item.PickupObjectId == 469) //if player has master round 1
                    {
                        masterRound = 469;
                    }
                    else if (item.PickupObjectId == 471) //if player has master round 2
                    {
                        masterRound = 471;
                    }
                    else if (item.PickupObjectId == 468) //if player has master round 3
                    {
                        masterRound = 468;
                    }
                    else if (item.PickupObjectId == 470) //if player has master round 4 
                    {
                        masterRound = 470;
                    }
                    else if (item.PickupObjectId == 467) //if player has master round 5
                    {
                        masterRound = 467;
                    }
                    else
                    {
                        masterRound = 0;
                    }
                }
                switch (masterRound)
                {
                    case 467: //master round 5

                        args.ModifiedDamage = 0;
                        player.RemovePassiveItem(467);
                        this.othereffects();
                        break;
                    case 468: //master round 3
                        args.ModifiedDamage = 0;
                        player.RemovePassiveItem(468);
                        this.othereffects();
                        break;
                    case 469: //master round 1
                        args.ModifiedDamage = 0;
                        player.RemovePassiveItem(469);
                        this.othereffects();
                        break;
                    case 470: //master round 4
                        args.ModifiedDamage = 0;
                        player.RemovePassiveItem(470);
                        this.othereffects();
                        break;
                    case 471: //master round 2
                        args.ModifiedDamage = 0;
                        player.RemovePassiveItem(471);
                        this.othereffects();
                        break;
                    case 0: //player HAD a master round but it was removed
                        break;
                    default: //no master round
                        break;
                }
            }
        }
        public void othereffects()
        {
            this.Owner.BloopItemAboveHead(base.sprite, "Knives/Resources/secondWind"); // boop
            
            PlayerController player = (PlayerController)this.Owner;
            RadialSlowInterface the_big_slow = new RadialSlowInterface();
            the_big_slow.RadialSlowHoldTime = .25f;
            the_big_slow.RadialSlowOutTime = .25f;
            the_big_slow.RadialSlowTimeModifier = 0f;
            the_big_slow.DoesSepia = false;
            the_big_slow.UpdatesForNewEnemies = true;
            the_big_slow.RadialSlowInTime = 0f;
            the_big_slow.DoRadialSlow(this.Owner.CenterPosition, player.CurrentRoom);
        }
    }

}


