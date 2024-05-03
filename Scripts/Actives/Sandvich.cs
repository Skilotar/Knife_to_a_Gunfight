using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI; 

namespace Knives
{
    class sandvich : PlayerItem
    {
       
        public static void Register()
        {
            //The name of the item
            string itemName = "Sandvich";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/sandvich";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<sandvich>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Perfect Fuel For Killing";
            string longDesc = "Stand still and *Om nOm Nom Naugm Nau* \n\n\n - Knife_to_a_Gunfight" +
                "";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 1200f);
            
            //Set the rarity of the item
         
            

            item.quality = PickupObject.ItemQuality.A;
            item.numberOfUses = 1;

            ID = item.PickupObjectId;
        }

        public static int ID;

        //applies damage on last use
        public override void  DoEffect(PlayerController user)
        {
            float dura = 4.5f;
            
            
            StartCoroutine(ItemBuilder.HandleDuration(this, dura, user, EndEffect));
            user.MovementModifiers += NoMotionModifier;
            user.IsStationary = true;
            this.LastOwner.stats.RecalculateStats(LastOwner, true);
            user.healthHaver.ApplyHealing(.5f);

        }
        protected void EndEffect(PlayerController user)
        {
            user.MovementModifiers -= NoMotionModifier;
            user.IsStationary = false;
            user.healthHaver.ApplyHealing(.5f);
            user.healthHaver.ApplyHealing(.5f);

        }
        private void NoMotionModifier(ref Vector2 voluntaryVel, ref Vector2 involuntaryVel)
        {
            voluntaryVel = Vector2.zero;
        }

        public override void  OnPreDrop(PlayerController user)
        {
            user.MovementModifiers -= NoMotionModifier;
            user.IsStationary = false;
            base.OnPreDrop(user);
        }
    }
}
