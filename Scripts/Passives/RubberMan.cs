using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using HutongGames.PlayMaker.Actions;

namespace Knives
{
    class rubber_man : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Rubberband Man";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/rubberbandman";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<rubber_man>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Prepare Yourself";
            string longDesc = "Boiyoiyoiyoiyoing!\n\n\n - Knife_to_a_Gunfight"
                ;

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            
            //Set the rarity of the item

            item.quality = PickupObject.ItemQuality.B;
        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        public override void  Update()
        {
            base.Update();
            if(this.Owner != null)
            {
                healthchecker();
            }
           
        }

        public float lastknownhealth;
        public void healthchecker()
        {
            float health = this.Owner.healthHaver.GetCurrentHealth();
            
            if(health < lastknownhealth)
            {

                reflect();

            }
            lastknownhealth = health;
        }
        
        public void reflect()
        {
            
            foreach (var projectile in GetBullets())
            {
                PassiveReflectItem.ReflectBullet(projectile, true, this.Owner, 15f, 1f, 1.5f, 0f);
                
            }
        }

        private static List<Projectile> GetBullets()
        {
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                if (projectile && projectile.sprite && !projectile.ImmuneToBlanks && !projectile.ImmuneToSustainedBlanks)
                {
                    if (projectile.Owner != null)
                    {
                        if (projectile.isFakeBullet || projectile.Owner is AIActor || (projectile.Shooter != null && projectile.Shooter.aiActor != null) || projectile.ForcePlayerBlankable)
                        {
                            list.Add(projectile);
                        }
                    }
                }
            }
            return list;
        }

    }
}
