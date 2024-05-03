using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod.RuntimeDetour;

namespace Knives
{
    class rad_board : PlayerItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Rad board";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/rad_board";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<rad_board>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Sick Flips!";
            string longDesc = "A skate board that has had its fair share of use and of new coats of paint. Its usefulness in the gungeon is questionable, but you sure look cool jumping over bullets." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 250f);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.GlobalPriceMultiplier, .20f, StatModifier.ModifyMethod.ADDITIVE);
            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.A;
            
        }
        float coolpoints = 0;
        public override void  DoEffect(PlayerController user)
        {
            if(this.LastOwner != null)
            {
                coolpoints = 0;
                float dura = 20f;
                user.OnDodgedProjectile += this.OnDodgedProjectile;
                StartCoroutine(ItemBuilder.HandleDuration(this, dura, user, EndEffect));
            }
            
        }
        private void OnDodgedProjectile(Projectile projectile)
        {   
            if(this.LastOwner != null)
            {
                AkSoundEngine.PostEvent("Play_WPN_radgun_noice_01", base.gameObject);
                coolpoints = coolpoints + 1;
            }
            
        }
        
        protected void EndEffect(PlayerController user)
        {
            if(this.LastOwner != null)
            {
                if(user.stats.GetStatValue(PlayerStats.StatType.Coolness) <= 15) // max cap to gainable coolness 
                {
                    coolpoints = coolpoints / 15;
                    if (coolpoints >= 3)
                    {
                        ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.Coolness, 3, StatModifier.ModifyMethod.ADDITIVE);
                        this.LastOwner.stats.RecalculateStats(LastOwner, true);
                    }
                    else
                    {
                        ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.Coolness, coolpoints, StatModifier.ModifyMethod.ADDITIVE);
                        this.LastOwner.stats.RecalculateStats(LastOwner, true);
                    }

                }

                user.OnDodgedProjectile -= this.OnDodgedProjectile;
            }
            
        }

        public override void  OnPreDrop(PlayerController user)
        {
            user.OnDodgedProjectile -= this.OnDodgedProjectile;
            base.OnPreDrop(user);
        }
    }
   
}

  