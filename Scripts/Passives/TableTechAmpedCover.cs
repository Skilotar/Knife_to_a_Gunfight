using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class TableTech_AmpedCover : TableFlipItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Table Tech Amped Cover";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/tabletech_amped_cover";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<TableTech_AmpedCover>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Playing Favorites";
            string longDesc =

                "With special, high tech and hopefully not flamable, new parts these tables have been retrofitted as Amped Covers. " +
                "Amped Covers simultaniously nullify incoming bullets and boost outgoing bullets for an extra Ommph!" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;


            //item.CanBeDropped = false;
            //Set the rarity of the item


            item.quality = PickupObject.ItemQuality.B;
        }
        
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnTableFlipped = (Action<FlippableCover>)Delegate.Combine(player.OnTableFlipped, new Action<FlippableCover>(this.AmpedCover));
           

        }

        //this code uses the Table as the point of calcualtion instead of the bullet so that the code can easily differenciate between tables the player flipped and tables the enemies flipped.
        //the TT will not trigger on any table the player does not perform the flip action on
        public override DebrisObject Drop(PlayerController player)
        {

            DebrisObject result = base.Drop(player);

            player.OnTableFlipped = (Action<FlippableCover>)Delegate.Remove(player.OnTableFlipped, new Action<FlippableCover>(this.AmpedCover));
            return result;
        }
        public void AmpedCover(FlippableCover obj)
        {
            if(obj != null)
            {
                SpeculativeRigidbody cover = obj.specRigidbody;

                obj.specRigidbody.OnPreRigidbodyCollision += this.OnPreCollison;
            }
        }
        
        private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody other, PixelCollider otherCollider)
        {
            if (myRigidbody != null && other != null)
            {
                bool flag = myRigidbody.gameObject.name != null && myRigidbody.specRigidbody != null;
                if (flag)
                {
                    bool flagged = other.gameObject.name != null && other.specRigidbody != null;
                    if (flagged)
                    {
                        bool flag2 = myRigidbody.gameObject.name == "Table_Vertical" || myRigidbody.gameObject.name == "Table_Horizontal";
                        if (flag2)
                        {
                            if (this.Owner)
                            {
                                bool flag3 = other.gameObject.GetComponent<PlayerController>() == null || other.gameObject.GetComponent<AIActor>() == null;
                                if (flag3)
                                {
                                    if (other.projectile)
                                    {
                                        bool flag4 = other.projectile.Owner == this.Owner;
                                        if (flag4)
                                        {
                                            if (other.projectile.baseData.force != 0)
                                            {
                                                other.projectile.baseData.damage *= 1.5f;
                                                other.projectile.baseData.force = 0f;
                                            }

                                            PhysicsEngine.SkipCollision = true;
                                        }

                                    }

                                }
                            }
                        }
                    }
                   
                }
            }
        }

       
    }
}
