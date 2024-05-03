using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{ 
   
    class punisher :PlayerItem
    {
        public static int rand_max;

        public static void Register()
        {
            System.Random rng = new System.Random();
            //The name of the item
            string itemName = "Punisher's Cup";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/punishers_cup";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<punisher>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "A test of greed";
            string longDesc = "This cup was originally designed as a prank for anyone who dare to fill the cup too full. Unfortunatly, the cult of the gundead had other more sinster plans for this harmless prank. \n\n" +
                "Fill the charges of this cup by purchasing items from shops and use all charges in one burst to heal. However if the cup is over filled it will rend the soul from your body." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;

            
            //Set the rarity of the item
            //item.AddToSubShop(ItemBuilder.ShopType.Cursula, .01f);
            item.consumable = true;
            item.CanBeDropped = false;
            item.UsesNumberOfUsesBeforeCooldown = true;
            item.quality = PickupObject.ItemQuality.C;
            item.numberOfUses = 1;
            int randomizer = rng.Next(7, 10);
            rand_max = randomizer;
        }

        PlayerController user = new PlayerController();
        //applies damage on last use
        public override void  DoEffect(PlayerController user)
        {
            int charges = this.numberOfUses;
            user.healthHaver.ApplyHealing(charges * .5f);
            this.numberOfUses = 0;
            user.OnItemPurchased -= this.OnItemPurchased;
        }

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
            {
                return;
            }
            base.Pickup(player);
            player.OnItemPurchased += this.OnItemPurchased;
        }
        private void OnItemPurchased(PlayerController player, ShopItemController obj)
        {

            this.numberOfUses ++;

        }
        
        
        public override void Update()
        {
            if(this.LastOwner != null)
            {
                if (this.numberOfUses == rand_max)
                {
                    user.healthHaver.ApplyDamage(1f, Vector2.zero, "greed kills", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, true);
                    this.numberOfUses = 0;
                }
            }

            base.Update();
            

        }

        public override void  OnPreDrop(PlayerController user)
        {
            this.numberOfUses = 0;
            
            base.OnPreDrop(user);
        }

    }
}

