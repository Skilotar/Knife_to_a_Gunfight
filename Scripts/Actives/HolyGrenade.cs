using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;

namespace Knives
{
    class HolyGrenade :PlayerItem
    {
        public static Projectile proj;
        public static void Register()
        {
            //The name of the item
            string itemName = "Purifier";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Purifier";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<HolyGrenade>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Armaments 2:9-21";
            string longDesc =
                "Oh Kaliber Bless this thy hand grenade, That with it thous may blowest thine enemies to tiny bits... \n\n In thy mercy." +
                "\n\n-----------------------------" +
                "try to hit a direct to deal massive damage to jammed enemies" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 250f);


           

            item.consumable = false;
            item.quality = PickupObject.ItemQuality.A;

           // Projectile projectile = ((Gun)ETGMod.Databases.Items[81]).DefaultModule.projectiles[0];
            //projectile.SetProjectileSpriteRight("Purifier", 16, 16, false, tk2dBaseSprite.Anchor.MiddleCenter, 16, 16);
           // HolyNadeController HOLY = projectile.gameObject.AddComponent<HolyNadeController>();
           // proj = projectile;


        }
        
        
        public override void  DoEffect(PlayerController user)
        {

            RadialSlowInterface test = new RadialSlowInterface();
            test.RadialSlowHoldTime = .5f;
            test.RadialSlowOutTime = .25f;
            test.RadialSlowTimeModifier = .20f;
            test.DoesSepia = false;
            test.UpdatesForNewEnemies = true;
            test.RadialSlowInTime = .25f;
            test.DoRadialSlow(user.CenterPosition, user.CurrentRoom);

            AkSoundEngine.PostEvent("Play_hallelujah", base.gameObject);
           
            GameObject gameObject = SpawnManager.SpawnProjectile(proj.gameObject, this.LastOwner.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.LastOwner.CurrentGun == null) ? 0f : this.LastOwner.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag = component != null;
            if (flag)
            {
                component.Owner = this.LastOwner;
                component.Shooter = this.LastOwner.specRigidbody;
                component.baseData.damage = 1f;
                component.baseData.speed = 8f;
                component.baseData.range = 15f;
                component.angularVelocity = 500;
                
                component.OnHitEnemy += this.OnHitEnemy;
               
            }

            
        }
        
        public void OnHitEnemy(Projectile bullet, SpeculativeRigidbody Enemy, bool probably)
        {
            HolyBoom(bullet);
            bullet.OnHitEnemy -= this.OnHitEnemy;
        }
      
        public void HolyBoom(Projectile projectile)
        {
            DoSafeExplosion(projectile.LastPosition);
            
            RoomHandler currentRoom = this.LastOwner.CurrentRoom;
            foreach (AIActor aiactor in currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
            {
                
                if (aiactor.healthHaver != null && aiactor.CompanionOwner != this.LastOwner && aiactor.IsBlackPhantom)
                {
                    
                    if (aiactor.healthHaver.IsBoss)
                    {
                        
                        aiactor.healthHaver.ApplyDamage(300, Vector2.zero, "Kaliber's mercy");
                    }
                    else
                    {
                       
                        aiactor.healthHaver.ApplyDamage(200, Vector2.zero, "Kaliber's mercy");
                    }
                }

            }
            AkSoundEngine.PostEvent("Play_holy_boom", base.gameObject);
        }
        public void DoSafeExplosion(Vector3 position)
        {

            ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
            this.smallPlayerSafeExplosion.effect = defaultSmallExplosionData2.effect;
            this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
            this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
            Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);

        }
        private ExplosionData smallPlayerSafeExplosion = new ExplosionData
        {
            damageRadius = 2.5f,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 20f,
            doDestroyProjectiles = true,
            doForce = false,
            debrisForce = 10f,
            preventPlayerForce = true,
            explosionDelay = 0.1f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = false,
            breakSecretWalls = true,
            
            
        };

    }
}
