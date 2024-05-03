using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class Knive :KnifeShieldItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Throwing Knives";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/knives";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Knive>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Technically a gun?";
            string longDesc = "It's always been said that bringing a knife to a gunfight is a poor decision.\n\n" +
                "However, because these knives are designed specifically to be thrown they fall in a grey area with the Lord Kaliber's rules.";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.None, 1f);
            

            item.quality = PickupObject.ItemQuality.B;
            
        }
        public KnifeShieldEffect Muda = new KnifeShieldEffect();

      
        public override void  DoEffect(PlayerController user)
        {
            Muda.numKnives = 1;
            Muda.knifeDamage = 3;
            Muda.radiusChangeDistance = 1f;
            Muda.rotationDegreesPerSecond = 180f;
            Muda.throwRadius = 4;
            Muda.throwRange = 15;
            Muda.throwSpeed = 0;
            Muda.circleRadius = 2;
            Muda.Initialize(user, knifePrefab);
            
        }
        public override void  DoOnCooldownEffect(PlayerController user)
        {
            
        }
        protected void EndEffect(PlayerController user)
        {
            Muda.ThrowShield();

        }
       
      
    }
}

