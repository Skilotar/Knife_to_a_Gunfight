using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;

namespace Knives
{
    class Semi : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "AutoSemiAuto";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Greedy_clip";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Semi>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "What";
            string longDesc =

                "You know what Screw you *automatics your semiautos" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;



            //Set the rarity of the item

            item.CanBeDropped = false;
            item.quality = PickupObject.ItemQuality.SPECIAL;
        }
       

        public override void Pickup(PlayerController player)
        {
            
            base.Pickup(player);
        }
        public bool Key(GungeonActions.GungeonActionType action, PlayerController user)
        {
            return BraveInput.GetInstanceForPlayer(user.PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
        }
        public bool KeyDown(GungeonActions.GungeonActionType action, PlayerController user)
        {
            return BraveInput.GetInstanceForPlayer(user.PlayerIDX).ActiveActions.GetActionFromType(action).WasPressed;
        }
        
        public override void  Update()
        {
            Gun thisone = this.Owner.CurrentGun;
            if (thisone.DefaultModule.shootStyle == ProjectileModule.ShootStyle.SemiAutomatic && Time.timeScale > 0f)
            {
                if(Key(GungeonActions.GungeonActionType.Shoot, this.Owner))
                {
                    thisone.Attack();
                }

            }
            base.Update();
        }


        private void PostProcessProjectile(Projectile projectile, float chance)
        {
           
        }
       
          

        

      
    }
}


