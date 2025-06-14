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
    class Slide_Bomb : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Slide Tech Bomb";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/slide_boom";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Slide_Bomb>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Sparking Splinters";
            string longDesc = "Tables explode after being slid over.\n\n" +
                "One of the Cult of the slide's more controversial texts. It was created as a way to give tables an honorable death instead of remaining half broken for centuries." +
                "\n\n\n - Knife_to_a_Gunfight"
                ;

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            //Set the rarity of the item
            ID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.B;
        }

        public static int ID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

        }
        bool doingreloadroutine = false;
        FlippableCover CurrentTable;
        public bool onepertable = false;
        public bool cleanup = false;
        public override void Update()
        {
            base.Update();
            if (this.Owner != null)
            {
                if (this.Owner.IsSlidingOverSurface)
                {

                    if (onepertable == false)
                    {
                        if (this.Owner.CurrentRoom != null)
                        {
                            IPlayerInteractable nearestInteractable = this.Owner.CurrentRoom.GetNearestInteractable(this.Owner.specRigidbody.UnitCenter, .5f, this.Owner);
                            if (nearestInteractable != null)
                            {
                                if (nearestInteractable is FlippableCover)
                                {
                                        
                                    FlippableCover Table = nearestInteractable as FlippableCover;
                                    CurrentTable = Table;
                                        
                                }
                            }
                        }
                    }
                        
                }
                else
                {
                    onepertable = false;
                    if(CurrentTable != null)
                    {
                        DoSafeExplosion(CurrentTable.specRigidbody.UnitCenter);
                        CurrentTable.DestroyCover();
                        CurrentTable = null;
                    }
                       
                }
                

            }
        }
        
        public void DoSafeExplosion(Vector3 position)
        {

            ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;
            this.tableExplosion.effect = defaultSmallExplosionData2.effect;
            this.tableExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
            this.tableExplosion.ss = defaultSmallExplosionData2.ss;
            Exploder.Explode(position, this.tableExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);

        }

        private ExplosionData tableExplosion = new ExplosionData
        {
            damageRadius = 5f,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 20f,
            doDestroyProjectiles = true,
            doForce = true,
            debrisForce = 0f,
            preventPlayerForce = true,
            explosionDelay = 0f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = true,
            breakSecretWalls = false,
            secretWallsRadius = 0,
            force = 2,
            forceUseThisRadius = true,


        };
    }
}