using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class Survivor : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Survivor";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/I_will_survive";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Survivor>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "I Will Survive";
            string longDesc = "You've been through bullet-hell and survived. At this point as long as you keep trying it feels like nothing can kill you.\n\n" +
                "Increases enemy bullet speed and gives chance to ignore damage." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.EnemyProjectileSpeedMultiplier, .15f, StatModifier.ModifyMethod.ADDITIVE);
            //Set the rarity of the item

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
        public override void  Update()
        {
            base.Update();
          
        }
       
        
        private void ModifyIncomingDamage(HealthHaver source, HealthHaver.ModifyDamageEventArgs args)
        {
            
            PlayerController player = (PlayerController)this.Owner;

            if (args == EventArgs.Empty)
            {
                return;
            }
                
            System.Random rng = new System.Random();
            if(rng.Next(1,3) == 1)
            {
                this.Owner.BloopItemAboveHead(base.sprite, "Knives/Resources/I_will_survive");
                args.ModifiedDamage = 0;

            }
           
        }

        
        
    }
}
