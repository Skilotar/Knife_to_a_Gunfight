using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using Alexandria.BreakableAPI;
using Alexandria.ItemAPI;
using UnityEngine;
using System.Reflection;
using GungeonAPI;
using MonoMod.RuntimeDetour;


namespace Knives
{
    public class MovieMagic : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Explosive Props";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Explosive_Props";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<MovieMagic>();

            //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Special Effects";
            string longDesc = "A satisfying mixture of cardboard and movie magic that causes small objects to burst into fake explosions."
                 +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            item.CanBeDropped = true;
            item.quality = PickupObject.ItemQuality.C;
            //Set the rarity of the item
            ID = item.PickupObjectId;
           
        }

        public static GameObject Boom;
        public static int ID;
        
        public static void MovieMagicHookMethod(Action<MinorBreakable,Vector2> orig, MinorBreakable self,Vector2 vect)
        {
            if(self != null)
            {
                if(self.specRigidbody != null)
                {
                   
                    if (mainOwner.HasPickupID(ID))
                    {

                        if (!self.IsBroken && mainOwner.IsInCombat && mainOwner.CenterPosition.GetAbsoluteRoom() == self.CenterPoint.GetAbsoluteRoom() && self.explodesOnBreak != true)
                        {
                            self.explodesOnBreak = true;
                            self.explosionData = MovieMagic.smallPlayerSafeExplosion;
                        }
                    }
                }
            }
            
            orig(self,vect);
           
        }
        public override void Pickup(PlayerController player)
        {
            mainOwner = player;

            Hook MinorbreakHook = new Hook(
            typeof(MinorBreakable).GetMethod("Break", new Type[] { typeof(Vector2) }),
            typeof(MovieMagic).GetMethod("MovieMagicHookMethod", BindingFlags.Public | BindingFlags.Static));

           
            base.Pickup(player);
        }
        public static PlayerController mainOwner;

        public static ExplosionData smallPlayerSafeExplosion = new ExplosionData
        {
            damageRadius = 3f,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 12f,
            doDestroyProjectiles = true,
            doForce = true,
            debrisForce = .5f,
            preventPlayerForce = true,
            explosionDelay = 0.0f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = true,
            breakSecretWalls = false,
            secretWallsRadius = 3,
            force = 2,
            forceUseThisRadius = true,
            effect = EasyVFXDatabase.RedFireBlastVFX, 
            ignoreList = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData.ignoreList,
            ss = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData.ss

        };

        public override DebrisObject Drop(PlayerController player)
        {

            return base.Drop(player);
        }
        public override void OnDestroy()
        {

            base.OnDestroy();
        }


    }
}
