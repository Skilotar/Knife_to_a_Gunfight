using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using HutongGames.PlayMaker.Actions;

namespace Knives
{
    class Rocker_Collection :PlayerItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Rockers Record Collection";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Rocker_record_collection";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Rocker_Collection>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "clunky and funky!";
            string longDesc =

                "A rocker's prize collection of songs. " +
                "Much of the music was created by several musical alchemist who learned to weave magic into music.\n\n" +
                "The chest itself is to heavy to carry around anywhere quickly. You will probably only be able to bring one or two records with you, so pick wisely.\n\n" +
                "Im fairly certain that rat eying you in the corner will be happy to help clean up the spare records.";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, -4f, StatModifier.ModifyMethod.ADDITIVE);

            //Set the rarity of the item

            item.consumable = true;
            item.quality = PickupObject.ItemQuality.S;
        }

        public override void  DoEffect(PlayerController user)
        {
           
            AkSoundEngine.PostEvent("Play_OBJ_chest_unlock_01", base.gameObject);
            
      
            user.GiveItem("ski:empty_record_collection");
            AkSoundEngine.PostEvent("Play_OBJ_chest_open_01", base.gameObject);
            user.GiveItem("ski:led_maiden");
            user.GiveItem("ski:99_blobuloons");
        }
    }
}
