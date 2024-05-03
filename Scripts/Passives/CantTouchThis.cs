using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class Cant_touch_ths : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Can't touch this";

            string resourceName = "Knives/Resources/cant_touch_this";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Cant_touch_ths>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Hammer Time";
            string longDesc = "You randomly become so cool nothing wants to hurt you." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            

           


            item.quality = PickupObject.ItemQuality.C;
            

        }
        
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            
        }
        public int counter = 0;
        public bool currentlyEtherial = false;
        public override void  Update()
        {
            base.Update();
            if (this.Owner)
            {
                groovy(this.Owner);
            }
           
        }
        private System.Random rng = new System.Random();
        // groovy uses counter as a basic frame/update based timer 20 updates = 1 second
        protected void groovy(PlayerController player)
        {   //Now this is Groovy
            // This calls for a celibration 
            // Here take this...
            // https://www.youtube.com/watch?v=UWLIgjB9gGw
            // alright you enjoy that
            // see you around
            // - Skilotar_

            int etherial = rng.Next(1, 3000);
            if (etherial == 1 && !this.currentlyEtherial)
            {
                player.IsEthereal = true;
                AkSoundEngine.PostEvent("Play_WPN_radgun_cool_01", base.gameObject);
                currentlyEtherial = true;
                
                
            }
            if (currentlyEtherial && counter <= 60)
            {
                counter++;
            }
            if (currentlyEtherial && counter >= 60)
            {
                this.Owner.IsEthereal = false;
                currentlyEtherial = false;
                counter = 0;
              
            }
        }
       
    }
}

