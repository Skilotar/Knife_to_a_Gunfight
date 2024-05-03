using System;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;

using MonoMod;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class OakenShield : AdvancedGunBehaviour
    {

        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Oaken Shield","Oak");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:oaken_shield", "ski:oaken_shield");
            gun.gameObject.AddComponent<OakenShield>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Unphased");
            gun.SetLongDescription("A door from the lead lords keep that has for some reason been affixed with handles for holding like a shield. One shudders to imagine the horror of a creature that would use such a large object for defence." +
                "\n\n\n - Knife_to_a_Gunfight");
      
            gun.SetupSprite(null, "Oak_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 1);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.reloadTime = 0f;
            gun.DefaultModule.cooldownTime = .15f;
            //gun.UsesRechargeLikeActiveItem = true;
            //gun.ActiveItemStyleRechargeAmount = 500;
            
            gun.DefaultModule.numberOfShotsInClip = 4000;
            gun.SetBaseMaxAmmo(4000);
            gun.CurrentAmmo = 4000;
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.C;
            
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            
            projectile.baseData.damage = 0f;
            projectile.baseData.range = 0f;
            projectile.SuppressHitEffects = true;
            projectile.AdditionalScaleMultiplier = .0001f;
            projectile.baseData.speed = 0f;
            projectile.transform.parent = gun.barrelOffset;

           

            tk2dSpriteAnimationClip fireClip = gun.sprite.spriteAnimator.GetClipByName("Oak_fire");
            float[] offsetsX = new float[] { 1f, 1f};
            float[] offsetsY = new float[] { 0f, 0.0f};
            for (int i = 0; i < offsetsX.Length && i < offsetsY.Length && i < fireClip.frames.Length; i++)
            {
                int id = fireClip.frames[i].spriteId;
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY[i];
            }

            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {

            gun.sprite.specRigidbody.PixelColliders.Add(PixelCollider.CreateRectangle(CollisionLayer.BulletBlocker, gun.sprite.specRigidbody.Position.X, gun.sprite.specRigidbody.Position.Y, 4, 45));
            gun.sprite.specRigidbody.PixelColliders.Add(PixelCollider.CreateRectangle(CollisionLayer.EnemyBulletBlocker, gun.sprite.specRigidbody.Position.X, gun.sprite.specRigidbody.Position.Y, 4, 45));
            gun.sprite.specRigidbody.PixelColliders.Add(PixelCollider.CreateRectangle(CollisionLayer.BeamBlocker, gun.sprite.specRigidbody.Position.X, gun.sprite.specRigidbody.Position.Y, 4, 45));

            base.OnPostFired(player, gun);
        }
      
        private IEnumerator HandleSwing(PlayerController user, Vector2 aimVec, float rayDamage, float rayLength)
        {
            float elapsed = 0f;
            while (elapsed < 1)
            {
                elapsed += BraveTime.DeltaTime;
                SpeculativeRigidbody hitRigidbody = this.IterativeRaycast(user.CenterPosition, aimVec, rayLength, int.MaxValue, user.specRigidbody);
                
                SpeculativeRigidbody hitRigidbody2 = this.IterativeRaycast(user.CenterPosition, aimVec, rayLength, int.MaxValue, user.specRigidbody);
                if (hitRigidbody2 && hitRigidbody2.projectile && hitRigidbody2.projectile.Owner != this.gun.CurrentOwner)
                {
                    
                    hitRigidbody2.projectile.DieInAir();
                }


                yield return null;
            }
            yield break;
        }

        protected SpeculativeRigidbody IterativeRaycast(Vector2 rayOrigin, Vector2 rayDirection, float rayDistance, int collisionMask, SpeculativeRigidbody ignoreRigidbody)
        {
            int num = 0;
            RaycastResult raycastResult;
            while (PhysicsEngine.Instance.Raycast(rayOrigin, rayDirection, rayDistance, out raycastResult, true, true, collisionMask, new CollisionLayer?(CollisionLayer.BulletBlocker), false, null, ignoreRigidbody))
            {
                num++;
                SpeculativeRigidbody speculativeRigidbody = raycastResult.SpeculativeRigidbody;
                if (num < 3 && speculativeRigidbody != null)
                {
                    MinorBreakable component = speculativeRigidbody.GetComponent<MinorBreakable>();
                    if (component != null)
                    {
                        component.Break(rayDirection.normalized * 3f);
                        RaycastResult.Pool.Free(ref raycastResult);
                        continue;
                    }
                }
                RaycastResult.Pool.Free(ref raycastResult);
                return speculativeRigidbody;
            }
            return null;
        }


        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
        public override void  Update()
        {
            if (gun.CurrentOwner)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;

                }

                

            }

        }
     
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                   

            }

               

        }
          
        
    }
}
