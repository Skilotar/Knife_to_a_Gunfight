using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brave.BulletScript;
using Dungeonator;
using Gungeon;
using ItemAPI;
using MultiplayerBasicExample;
using UnityEngine;
namespace Knives
{
    class Slide_Split : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Slide Tech Split";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/slide_Split";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Slide_Split>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Smoke And Mirrors";
            string longDesc = "This page was often used as a simple party trick until the Cult of the slide's downfall where every tome was used to its most vile extent." +
                "\n\n\n - Knife_to_a_Gunfight"
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
        bool doingreloadroutine = false;
        public override void  Update()
        {
            base.Update();
            if (this.Owner != null)
            {
                if (this.Owner.IsSlidingOverSurface)
                {
                    if (doingreloadroutine != true && onepertable == false)
                    {
                        StartCoroutine(Splitroutine(this.Owner));
                    }

                }
                else
                {
                    onepertable = false;
                }


            }
        }
        public bool onepertable = false;
        private IEnumerator Splitroutine(PlayerController player)
        {
            doingreloadroutine = true;
            onepertable = true;
            yield return new WaitForSeconds(.1f);
            GameObject shadowclone = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(820) as SpawnObjectPlayerItem).objectToSpawn, player.specRigidbody.UnitCenter, Quaternion.identity);
            KageBunshinController kageBunshin = shadowclone.GetOrAddComponent<KageBunshinController>();
            kageBunshin.InitializeOwner(player);
            kageBunshin.Duration = 7;
            
            kageBunshin.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox,CollisionLayer.Projectile, CollisionLayer.PlayerCollider,CollisionLayer.LowObstacle));
            
            yield return new WaitForSeconds(13f);
            doingreloadroutine = false;
        }
    }
}