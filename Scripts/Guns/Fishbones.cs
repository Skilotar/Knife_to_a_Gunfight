using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Dungeonator;
using UnityEngine;
using ItemAPI;
using Gungeon;
using System.Collections;
using HutongGames.PlayMaker.Actions;

namespace Knives
{
    class FishBones : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Fishbones", "Fishbone");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:fishbones", "ski:fishbones");
            gun.gameObject.AddComponent<FishBones>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("We Need Some Panic!");
            gun.SetLongDescription("A rocket launcher that is as thirsty for carnage as its creator. When getting a kill this launcher may [Get Excited] dragging its owner towards more fun." +
                "\n\n\n - Knife_to_a_Gunfight");
            gun.SetupSprite(null, "Fishbones_idle_001", 1);
            gun.SetAnimationFPS(gun.shootAnimation, 9);
            gun.SetAnimationFPS(gun.reloadAnimation, 7);


            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(39) as Gun, true, false);
            gun.PreventNormalFireAudio = true;

            gun.gunHandedness = GunHandedness.OneHanded;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
           
            gun.gunClass = GunClass.EXPLOSIVE;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.cooldownTime = .7f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.SetBaseMaxAmmo(60);
            gun.quality = PickupObject.ItemQuality.A;

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Shark_rocket", "Knives/Resources/Shark_clipfull", "Knives/Resources/Shark_clipempty");
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            gun.barrelOffset.transform.localPosition = new Vector3(2f, .5f, 0f);
            projectile.SetProjectileSpriteRight("Fishbonesrocket", 31, 11, false, tk2dBaseSprite.Anchor.MiddleCenter, 32, 13);
            projectile.AdditionalScaleMultiplier = .75f;

            projectile.baseData.damage = 1f;
            projectile.baseData.speed *= .5f;
            projectile.baseData.range = 60f;





            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }

        public static int ID;
        public override void  OnPickedUpByPlayer(PlayerController player)
        {
            
            base.OnPickedUpByPlayer(player);
        }


        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_comm4nd0_shot_01", base.gameObject);
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            
            if((this.gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy("Jump The Shark"))
            {
                projectile.OnDestruction += this.ondestruction;
            }
            StartCoroutine(Delayhome(projectile));
            projectile.OnHitEnemy += this.OnHitEnemy;
            base.PostProcessProjectile(projectile);
        }

        private void ondestruction(Projectile obj)
        {
            StartCoroutine(delayspawn(obj));
        }

        public IEnumerator delayspawn(Projectile obj)
        {
            yield return new WaitForSeconds(.7f);
            if ((this.gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy("Jump The Shark"))
            {
                PlayerController owner = this.gun.CurrentOwner as PlayerController;
                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("72d2f44431da43b8a3bae7d8a114a46d");
                IntVector2 intVector = Vector2Extensions.ToIntVector2(owner.unadjustedAimPoint, (VectorConversions)2);
                RoomHandler absoluteRoomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector);
                bool flag = absoluteRoomFromPosition != null && absoluteRoomFromPosition == owner.CurrentRoom && owner.IsInCombat;
                if (flag)
                {
                    AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, obj.LastPosition, absoluteRoomFromPosition, true, (AIActor.AwakenAnimationType)2, true);
                    PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
                    aiactor.CanTargetEnemies = true;
                    aiactor.CanTargetPlayers = false;
                    aiactor.IsHarmlessEnemy = true;
                    aiactor.CanDropCurrency = false;

                    aiactor.IgnoreForRoomClear = true;

                    aiactor.CompanionOwner = owner;

                    aiactor.isPassable = true;
                    aiactor.gameObject.AddComponent<KillOnRoomClear>();
                    aiactor.reinforceType = (AIActor.ReinforceType)2;
                    aiactor.HandleReinforcementFallIntoRoom(0.1f);

                    aiactor.specRigidbody.AddCollisionLayerIgnoreOverride(
                        CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.PlayerHitBox,
                        CollisionLayer.Projectile, CollisionLayer.PlayerCollider, CollisionLayer.PlayerBlocker, CollisionLayer.BeamBlocker)
                        );
                    aiactor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable, CollisionLayer.Trap));

                    if (aiactor.bulletBank != null)
                    {
                        AIBulletBank bulletBank = aiactor.bulletBank;
                        bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.OnPostProcesssharkProjectile));
                    }
                    if (aiactor.aiShooter != null)
                    {
                        AIShooter aiShooter = aiactor.aiShooter;
                        aiShooter.PostProcessProjectile = (Action<Projectile>)Delegate.Combine(aiShooter.PostProcessProjectile, new Action<Projectile>(this.OnPostProcesssharkProjectile));
                    }

                }

                throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }

        public void OnPostProcesssharkProjectile(Projectile proj)
        {
            try
            {
                if (proj.Owner is AIActor && !(proj.Owner as AIActor).CompanionOwner)
                {
                    return; //to prevent the OnPostProcessProjectile from affecting enemies projectiles
                }
                //proj.AdjustPlayerProjectileTint(Color.grey, 0);

                proj.Owner = this.gun.CurrentOwner; //to allow the projectile damage modif, otherwise it stays at 10 for some reasons
                
                
                proj.baseData.damage = 1;
                if (this.gun.CurrentOwner is PlayerController)
                {
                    proj.baseData.damage *= (this.gun.CurrentOwner as PlayerController).stats.GetStatValue(PlayerStats.StatType.Damage) + .5f;
                }
                if (proj.IsBlackBullet)
                {
                    proj.baseData.damage *= 2.5f;
                }
                proj.collidesWithPlayer = false;
                proj.collidesWithEnemies = true;
                proj.TreatedAsNonProjectileForChallenge = true;
                proj.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(proj.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));
            }
            catch (Exception e)
            {
                Tools.Print("fish OnPostProcessProjectile", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }
        private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            bool flag = otherRigidbody && otherRigidbody.healthHaver && otherRigidbody.aiActor && otherRigidbody.aiActor.CompanionOwner;
            if (flag)
            {
                float damage = myRigidbody.projectile.baseData.damage;
                myRigidbody.projectile.baseData.damage = 0f;
                GameManager.Instance.StartCoroutine(NewNewCopperChariot.ChangeProjectileDamage(myRigidbody.projectile, damage));
            }
        }
        public static IEnumerator ChangeProjectileDamage(Projectile bullet, float oldDamage)
        {
            yield return new WaitForSeconds(0.1f);
            bool flag = bullet != null;
            if (flag)
            {
                bullet.baseData.damage = oldDamage;
            }
            yield break;
        }


        public IEnumerator Delayhome(Projectile proj)
        {
            yield return new WaitForSeconds(.5f);
            if(proj != null)
            {
                proj.CurseSparks = true;
                AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", base.gameObject);
                HomingModifier home = proj.gameObject.AddComponent<HomingModifier>();
                home.AngularVelocity = 400;
                home.AssignProjectile(proj);
                home.HomingRadius = 30f;
                proj.baseData.damage = 15;
            }
            yield return new WaitForSeconds(.1f);
            AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", base.gameObject);
            yield return new WaitForSeconds(.1f);
            AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", base.gameObject);
        }
        public System.Random rng = new System.Random();
        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool fatal)
        {
            if(arg2.aiActor != null)
            {
                if (fatal)
                {

                    int frenzy = rng.Next(1, 7);
                    if (frenzy == 1 && Currentlyfrenzy == 0)
                    {
                        Currentlyfrenzy = 10;
                    }
                }
            }
           
        }
        public float Currentlyfrenzy = 0;
        
       

        private bool HasReloaded;
        public bool frenzyup = false;
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
                if(Currentlyfrenzy > 0)
                {
                    if(frenzyup != true)
                    {
                        gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, 2.5f, StatModifier.ModifyMethod.ADDITIVE);
                        gun.AddCurrentGunStatModifier(PlayerStats.StatType.ReloadSpeed, -.25f, StatModifier.ModifyMethod.ADDITIVE);
                        (gun.CurrentOwner as PlayerController).stats.RecalculateStats((gun.CurrentOwner as PlayerController), true);
                        AkSoundEngine.PostEvent("Play_Speed_Up", base.gameObject);
                        frenzyup = true;
                    }
                    Currentlyfrenzy -= Time.deltaTime;
                }
                else
                {
                    if (frenzyup == true)
                    {
                        AkSoundEngine.PostEvent("Play_Speed_Down", base.gameObject);
                        gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, -2.5f, StatModifier.ModifyMethod.ADDITIVE);
                        gun.AddCurrentGunStatModifier(PlayerStats.StatType.ReloadSpeed, .25f, StatModifier.ModifyMethod.ADDITIVE);
                        (gun.CurrentOwner as PlayerController).stats.RecalculateStats((gun.CurrentOwner as PlayerController), true);
                        frenzyup = false;
                        Currentlyfrenzy = 0;
                    }
                }

            }
        }
            
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                HasReloaded = false;
                
                AkSoundEngine.PostEvent("Play_WPN_yarirocketlauncher_reload_01", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
               
            }
        }
       
    }
}