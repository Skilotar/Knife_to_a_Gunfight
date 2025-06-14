using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class WormholeAmmo : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Wormhole ammo";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/wormhole_clips";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<WormholeAmmo>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Multiverse. ACP";
            string longDesc = "Reload with no ammo to gain ammo and temporarily debuff weapons. Debuff clears when picking up normal ammo.\n\n" +

                "A box of ammo swirling with energy. It contains ammo from a different version of this reality where the ammo kinda sucks...\n" +
                "Use it to extend your firefight, but probably not for too long." +
                "\n\n\n - Knife_to_a_Gunfight" +
                "\n Sprite by Lynceus";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            item.quality = PickupObject.ItemQuality.C;
            ID = item.PickupObjectId;
        }

        public static int ID;
        public override void Pickup(PlayerController player)
        {
            player.CurrentGun.OnReloadPressed += this.onreload;
            player.inventory.OnGunChanged += Inventory_OnGunChanged;
            base.Pickup(player);
        }

        private void onreload(PlayerController player, Gun gun, bool arg3)
        {
            if(gun.ammo == 0)
            {
                int ammo_to_give = player.CurrentGun.ClipCapacity;
                if (ammo_to_give >= 25)
                {
                    player.CurrentGun.GainAmmo(25);
                    
                }
                else
                {
                    player.CurrentGun.GainAmmo(ammo_to_give);
                    
                }
                player.CurrentGun.Reload();
                AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_Stat_Down", base.gameObject);
                WormholeDebuffGun worm = player.CurrentGun.gameObject.GetOrAddComponent<WormholeDebuffGun>();
                worm.player = player;
                worm.multiplier++;
                
            }
        }

        private void Inventory_OnGunChanged(Gun previous, Gun current, Gun previousSecondary, Gun currentSecondary, bool newGun)
        {
            this.Owner.CurrentGun.OnReloadPressed -= this.onreload;
            this.Owner.CurrentGun.OnReloadPressed += this.onreload;
        }

       

        public override DebrisObject Drop(PlayerController player)
        {
            player.CurrentGun.OnReloadPressed -= this.onreload;
            player.inventory.OnGunChanged -= Inventory_OnGunChanged;
            
            return base.Drop(player);
        }

        public override void Update()
        {

            base.Update();
        }

 
    }
}

