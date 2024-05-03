using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections.Generic;


namespace Knives
{
    class Trinket_of_Kaliber : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Trinket of Kaliber";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Trinket";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Trinket_of_Kaliber>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Kaliber's Covenant";
            string longDesc =

                "A small pendant signifying a promise of Kaliber to protect her followers from her wrath. " +
                "As long as the trinket is worn, the bite of her wrath will pass over you. Black phantom bullets will deal regular damage to you." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            item.quality = PickupObject.ItemQuality.B;
            itemID = item.PickupObjectId;
            
        }
        public static int itemID;

        public override void Pickup(PlayerController player)
        {
            
            player.healthHaver.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Combine(player.healthHaver.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage));
            
            base.Pickup(player);
        }


        public void ModifyIncomingDamage(HealthHaver arg1, HealthHaver.ModifyDamageEventArgs arg2)
        {
            if(arg2.InitialDamage >= .5f)
            {
                arg2.ModifiedDamage = .5f;
            }

        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Remove(player.healthHaver.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage));
            
            return base.Drop(player);
        }

        public override void Update()
        {
            try
            {
                if (this.Owner != null)
                {
                   
                }

            }
            catch
            {

            }

            base.Update();
        }

    }
}

