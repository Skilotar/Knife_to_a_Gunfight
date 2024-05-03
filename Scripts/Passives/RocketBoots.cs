using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class Rocket_boots :PassiveItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "Rocket Boots";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/rocket_boots";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Rocket_boots>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Poorly Excecuted";
            string longDesc =

                "Known for her impatience, Cadence grew tired of walking places. So, as she did with many of lifes problems she strapped a rocket too it.\n\n" +
                "While this solution would have worked in theory she was also too impatient to secure the rockets properly to the boots." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;


            
            //Set the rarity of the item
            
            
            item.quality = PickupObject.ItemQuality.C;

            Item_ID = item.PickupObjectId;
        }

        public static int Item_ID;
        public override void Pickup(PlayerController player)
        {
            player.OnPreDodgeRoll += this.OnPreDodgeRoll;
            base.Pickup(player);
        }

        private bool hasSynergy = false;
        public int ID = 16;
        private void OnPreDodgeRoll(PlayerController player)
        {
            
            Projectile projectile = ((Gun)ETGMod.Databases.Items[ID]).DefaultModule.projectiles[0];
            projectile.baseData.damage = .001f;
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, this.Owner.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.Owner.CurrentGun == null) ? 0f : this.Owner.CurrentGun.CurrentAngle + 170f), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            if (ID == 16)
            {
                GameObject gameObject2 = SpawnManager.SpawnProjectile(projectile.gameObject, this.Owner.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.Owner.CurrentGun == null) ? 0f : this.Owner.CurrentGun.CurrentAngle - 170f), true);
                Projectile component2 = gameObject2.GetComponent<Projectile>();
                bool flag2 = component2 != null;
                if (flag2)
                {
                    component2.Owner = this.Owner;
                    component2.Shooter = this.Owner.specRigidbody;
                }
            }
            bool flag = component != null;
           
            if (flag)
            {
                component.Owner = this.Owner;
                component.Shooter = this.Owner.specRigidbody;
            }

            
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnPreDodgeRoll -= this.OnPreDodgeRoll;
            return base.Drop(player);
        }

        public override void  Update()
        {
            if(this.Owner != null)
            {
                bool flag = this.Owner;
                if (flag)
                {
                    bool flag2 = this.Owner.HasPickupID(92) || this.Owner.HasPickupID(14) || this.Owner.HasPickupID(630) || this.Owner.HasPickupID(138);
                    if (flag2)
                    {
                        this.hasSynergy = true;
                    }
                    else
                    {
                        this.hasSynergy = false;
                    }
                }
                if (this.hasSynergy)
                {
                    ID = 92;
                }
                else
                {
                    ID = 16;
                }

            }

            base.Update();
        }
        
    }
}
