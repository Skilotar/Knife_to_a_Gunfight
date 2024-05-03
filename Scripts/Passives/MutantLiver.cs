using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Gungeon;



namespace Knives
{
    class MutantLiver : PassiveItem
    {
        //Call this method from the Start() method of your ETGModule extension
        public static void Register()
        {
            //The name of the item
            string itemName = "Mutant Liver";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Mutant_Liver";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<MutantLiver>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Failing Forward";
            string longDesc = "Taking poison damage has a 50% chance to heal you.\n\n" +
                "Its the hardest working liver in the galaxy. " +
                "This liver belonged to a crippling alcholoic lizard. " +
                "Instead of failing this particular liver mutated to be able to handle stronger and stronger poisons until the owners blood became saturated with venom." +
            "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");


            item.quality = PickupObject.ItemQuality.D;
            
            itemID = item.PickupObjectId;

        }
        public static int itemID;
        public override void Pickup(PlayerController player)
        {
            player.healthHaver.ModifyDamage += this.ModDam;

            base.Pickup(player);
        }

        private void ModDam(HealthHaver arg1, HealthHaver.ModifyDamageEventArgs arg2)
        {
            if (block)
            {
                arg2.ModifiedDamage = 0;
                
                bool FullHealth = false;
                if (arg1.GetMaxHealth() == arg1.GetCurrentHealth()) FullHealth = true;
                if (FullHealth)
                {
                    arg1.Armor++;
                    AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", gameObject);

                }
                else
                {
                    arg1.ApplyHealing(.5f);
                    AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", gameObject);

                }
                arg1.TriggerInvulnerabilityPeriod();
                block = false;
            }
        }
        public bool block;
        public override void Update()
        {
            if( this.Owner != null)
            {
                if (this.Owner.CurrentPoisonMeterValue >= 1)
                {
                    if (UnityEngine.Random.Range(0f, 2f) <= 1)
                    {
                        block = true;
                    }
                }

            }

            base.Update();
        }


        public override DebrisObject Drop(PlayerController player)
        {

            
            return base.Drop(player);
        }

    }
}
