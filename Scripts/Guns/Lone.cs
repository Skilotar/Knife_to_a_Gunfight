using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using ItemAPI;
using Gungeon;
using System.Collections;
using HutongGames.PlayMaker.Actions;

namespace Knives
{
    class Lone : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("LoneStar", "Lone");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:lonestar", "ski:lonestar");
            gun.gameObject.AddComponent<Lone>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("When The Stars Align");
            gun.SetLongDescription("Reloads one bullet at a time. Can fire loaded bullets to interupt reload. \n" +
                "Kills create stars that will burst and amplify the bullet when shot.\n\n" +
                "" +
                "A single load lever action rifle used by the chidren of the sun to shoot stars out of the sky." +

              
               
                "\n\n\n - Knife_to_a_Gunfight");
            gun.SetupSprite(null, "Lone_idle_001", 1);
            gun.SetAnimationFPS(gun.shootAnimation, 9);
            gun.SetAnimationFPS(gun.reloadAnimation, 7);

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(25) as Gun, true, false);
            gun.PreventNormalFireAudio = true;

            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            Gun gun3 = (Gun)ETGMod.Databases.Items["blunderbuss"];
            gun.muzzleFlashEffects = gun3.muzzleFlashEffects;
            gun.gunClass = GunClass.RIFLE;
            gun.reloadTime = 7.5f;
            gun.DefaultModule.cooldownTime = .7f;
            gun.DefaultModule.numberOfShotsInClip = 10;
            gun.SetBaseMaxAmmo(300);
            gun.quality = PickupObject.ItemQuality.B;
            

            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            gun.barrelOffset.transform.localPosition = new Vector3(2f, .5f, 0f);
            projectile.baseData.damage = 18f;
            projectile.baseData.speed *= .8f;
            projectile.baseData.range = 20f;

            gun.shellsToLaunchOnFire = 1;
            Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
            gun.shellCasing = gun2.shellCasing;



            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }
        public static PlayerController Loneranger;
        public static int ID;
        public override void  OnPickedUpByPlayer(PlayerController player)
        {
            Loneranger = player;
            base.OnPickedUpByPlayer(player);
        }


        public override void OnPostFired(PlayerController player, Gun gun)
        {
           
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            projectile.OnHitEnemy += this.OnHitEnemy;
            projectileStates state = projectile.gameObject.GetOrAddComponent<projectileStates>();
            state.isloneStarLone = true;
            

            base.PostProcessProjectile(projectile);
        }

        
        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool fatal)
        {
            if(arg2.aiActor != null)
            {
                if (fatal)
                {
                    PlayerController player = (PlayerController)this.gun.CurrentOwner;
                    Projectile projectile2 = ((Gun)ETGMod.Databases.Items[52]).DefaultModule.chargeProjectiles[1].Projectile;
                    GameObject gameObject2 = SpawnManager.SpawnProjectile(projectile2.gameObject, arg2.aiActor.Position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
                    Projectile component2 = gameObject2.GetComponent<Projectile>();
                    component2.baseData.damage = 0f;
                    component2.baseData.range = 20f;
                    component2.baseData.speed = .001f;
                    StartCoroutine(armdelay(component2));
                    component2.HasDefaultTint = true;
                    component2.DefaultTintColor = UnityEngine.Color.white;
                    component2.Owner = player;
                    component2.AdditionalScaleMultiplier = .75f;

                    component2.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyBulletBlocker, CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable));
                }

            }


        }

        public IEnumerator armdelay(Projectile bullet)
        {
            yield return new WaitForSecondsRealtime(.2f);
            projectileStates state = bullet.gameObject.GetOrAddComponent<projectileStates>();
            state.isloneStarStar = true;
            bullet.HasDefaultTint = true;
            bullet.DefaultTintColor = UnityEngine.Color.white;
        }
        public int LoneCount;

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


                if (gun.IsReloading && Key(GungeonActions.GungeonActionType.Shoot, gun.CurrentOwner as PlayerController) && gun.ClipShotsRemaining > 0)
                {
                    LoneCount = gun.ClipShotsRemaining;
                    gun.ForceImmediateReload(false);
                    gun.ClipShotsRemaining = LoneCount;
                }


                if (!this.gun.IsReloading)
                {
                    PlayerController player = (PlayerController)this.gun.CurrentOwner;
                    if (player.PlayerHasActiveSynergy("Dual Loader"))
                    {
                        gun.reloadTime = ((this.gun.ClipCapacity - this.gun.ClipShotsRemaining) * .33f)/2;

                    }
                    else
                    {
                        gun.reloadTime = (this.gun.ClipCapacity - this.gun.ClipShotsRemaining) * .33f;
                    }
                    if(gun.ClipShotsRemaining <= 0)
                    {
                        gun.reloadTime = gun.reloadTime + 1;
                    }
                    
                }


                foreach (Projectile projectile in GetLoneBullets())
                {
                    foreach (Projectile bullet in GetStarBullets())
                    {

                        float radius = 1.5f;
                        if (Vector2.Distance(bullet.LastPosition, projectile.LastPosition) < radius)
                        {
                            projectile.DefaultTintColor = UnityEngine.Color.white;
                            projectile.AdditionalScaleMultiplier = 1.2f;
                            projectile.baseData.damage *= 1.5f;

                            PlayerController player = (PlayerController)this.gun.CurrentOwner;
                            Projectile projectile2 = ((Gun)ETGMod.Databases.Items[52]).DefaultModule.chargeProjectiles[0].Projectile;
                            GameObject gameObject2 = SpawnManager.SpawnProjectile(projectile2.gameObject, bullet.LastPosition, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : bullet.Direction.ToAngle()), true); 
                            Projectile component2 = gameObject2.GetComponent<Projectile>();
                            component2.baseData.damage *= 1f;
                            component2.baseData.range = 20f;
                            component2.baseData.speed *= 1f;
                            component2.Owner = player;
                            component2.AdditionalScaleMultiplier = .80f;
                            component2.HasDefaultTint = true;
                            component2.DefaultTintColor = UnityEngine.Color.white;


                            Projectile projectile3 = ((Gun)ETGMod.Databases.Items[52]).DefaultModule.chargeProjectiles[0].Projectile;
                            GameObject gameObject3 = SpawnManager.SpawnProjectile(projectile3.gameObject, bullet.LastPosition, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : bullet.Direction.ToAngle() +120), true);
                            Projectile component3 = gameObject3.GetComponent<Projectile>();
                            component3.baseData.damage *= 1f;
                            component3.baseData.range = 20f;
                            component3.baseData.speed *= 1f;
                            component3.Owner = player;
                            component3.AdditionalScaleMultiplier = .80f;
                            component3.HasDefaultTint = true;
                            component3.DefaultTintColor = UnityEngine.Color.white;

                            Projectile projectile4 = ((Gun)ETGMod.Databases.Items[52]).DefaultModule.chargeProjectiles[0].Projectile;
                            GameObject gameObject4 = SpawnManager.SpawnProjectile(projectile4.gameObject, bullet.LastPosition, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : bullet.Direction.ToAngle() -120), true);
                            Projectile component4 = gameObject4.GetComponent<Projectile>();
                            component4.baseData.damage *= 1f;
                            component4.baseData.range = 20f;
                            component4.baseData.speed *= 1f;
                            component4.Owner = player;
                            component4.AdditionalScaleMultiplier = .80f;
                            component4.HasDefaultTint = true;
                            component4.DefaultTintColor = UnityEngine.Color.white;

                            bullet.ForceDestruction();

                        }

                    }
                }
            }

            base.Update();
        }
        public static List<Projectile> GetStarBullets()
        {
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                if (projectile.Owner != null)
                {
                    if (projectile.Owner == Loneranger)
                    {
                        if (projectile.GetComponent<projectileStates>())
                        {
                            if (projectile.GetComponent<projectileStates>().isloneStarStar == true)
                            {
                                list.Add(projectile);
                            }
                        }
                    }
                }
            }
            return list;
        }
        public static List<Projectile> GetLoneBullets()
        {
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                if (projectile.Owner != null)
                {
                    if (projectile.Owner == Loneranger)
                    {
                        if (projectile.GetComponent<projectileStates>())
                        {
                            if (projectile.GetComponent<projectileStates>().isloneStarLone == true)
                            {
                                list.Add(projectile);
                            }
                        }
                    }
                }
            }
            return list;
        }

        public float perload = .5f;
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                HasReloaded = false;

                base.OnReloadPressed(player, gun, bSOMETHING);
                if(player.PlayerHasActiveSynergy("Dual Loader"))
                {
                    StartCoroutine(DuelgradualReload());
                }
                else
                {
                    StartCoroutine(gradualReload());
                }
               
            }
        }
        public static MethodInfo info = typeof(Gun).GetMethod("FinishReload", BindingFlags.Instance | BindingFlags.NonPublic);
        public bool Key(GungeonActions.GungeonActionType action, PlayerController user)
        {
            return BraveInput.GetInstanceForPlayer(user.PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
        }


        public IEnumerator gradualReload()
        {
           
            PlayerController player = (PlayerController)this.gun.CurrentOwner;
            if (this.gun.ClipShotsRemaining <= 0)
            {
                yield return new WaitForSeconds(1f * player.stats.GetStatValue(PlayerStats.StatType.ReloadSpeed));
            }

            yield return new WaitForSeconds(perload * player.stats.GetStatValue(PlayerStats.StatType.ReloadSpeed));
            if (this.gun.ClipShotsRemaining != this.gun.ClipCapacity && gun.IsReloading)
            {
                this.gun.ClipShotsRemaining++;
              
            }
            if (gun.IsReloading)
            {
                StartCoroutine(gradualReload());
            }
            
        }

        public IEnumerator DuelgradualReload()
        {

            PlayerController player = (PlayerController)this.gun.CurrentOwner;
            if (this.gun.ClipShotsRemaining <= 0)
            {
                yield return new WaitForSeconds(.66f * player.stats.GetStatValue(PlayerStats.StatType.ReloadSpeed));
            }

            yield return new WaitForSeconds(perload * player.stats.GetStatValue(PlayerStats.StatType.ReloadSpeed));
            if (this.gun.ClipShotsRemaining != this.gun.ClipCapacity && gun.IsReloading)
            {
                if(this.gun.ClipShotsRemaining == this.gun.ClipCapacity - 1 && gun.IsReloading)
                {
                    this.gun.ClipShotsRemaining++;
                }
                else
                {
                    this.gun.ClipShotsRemaining++;
                    this.gun.ClipShotsRemaining++;
                }
               

            }
           
            if (gun.IsReloading && player.PlayerHasActiveSynergy("Dual Loader"))
            {
                StartCoroutine(DuelgradualReload());
            }
            else if (gun.IsReloading)
            {
                StartCoroutine(gradualReload());
            }

        }
    }
}