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
    class Superstar : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("SuperStar", "SuperStar");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:superstar", "ski:superstar");
            gun.gameObject.AddComponent<Superstar>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Shine Bright");
            gun.SetLongDescription("Reloads one shell at a time. Can fire loaded bullets to interupt reload. \n" +
                "Racking a new shell launches spent shells as weak projectiles. That's 66% more bullet per bullet\n\n" +
                "" +
                "A single load pump action shotgun used by the chidren of the sun during their duels over territory. " +


                "\n\n\n - Knife_to_a_Gunfight");
            gun.SetupSprite(null, "SuperStar_idle_001", 1);
            gun.SetAnimationFPS(gun.shootAnimation, 9);
            gun.SetAnimationFPS(gun.reloadAnimation, 5);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;

            for (int i = 0; i < 5; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(157) as Gun, true, false);


            }
            foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
            {
                projectileModule.ammoCost = 1;
                projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                projectileModule.cooldownTime = .8f;
                projectileModule.angleVariance = 6f;
                projectileModule.numberOfShotsInClip = 5;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                projectileModule.projectiles[0] = projectile;
                projectile.baseData.damage = 6f;
                projectile.AdditionalScaleMultiplier = .5f;
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                
                gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.BLUE_SHOTGUN;
                gun.DefaultModule.projectiles[0] = projectile;
                bool flag = projectileModule != gun.DefaultModule;
                if (flag)
                {
                    projectileModule.ammoCost = 0;
                }

            }
            gun.barrelOffset.transform.localPosition = new Vector3(1.5f, .5f, 0f);
            gun.reloadTime = 3f;
            gun.SetBaseMaxAmmo(300);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(157) as Gun).muzzleFlashEffects;
            gun.quality = PickupObject.ItemQuality.B;
            gun.gunClass = GunClass.SHOTGUN;

            
            Projectile shell = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            shell.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(shell.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(shell);
            shell.baseData.damage = 18f;
            shell.baseData.speed *= .8f;
            shell.baseData.range = 20f;
            gun.DefaultModule.finalProjectile = shell;
            shell.SetProjectileSpriteRight("Shell", 10, 4, false, tk2dBaseSprite.Anchor.MiddleCenter, 10, 4);
            Shell_g = shell;

            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;
        }

        public static int ID;


        public static PlayerController Loneranger;
        public static Projectile Shell_g;
        public override void  OnPickedUpByPlayer(PlayerController player)
        {
            Loneranger = player;
            base.OnPickedUpByPlayer(player);
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            StartCoroutine(ShellCasingLaunch());
            AkSoundEngine.PostEvent("Play_half_gauge_fire", base.gameObject);
            base.OnPostFired(player, gun);
        }

       
        private IEnumerator ShellCasingLaunch()
        {

            yield return new WaitForSeconds(.25f);
            GameObject gameObject = SpawnManager.SpawnProjectile(Shell_g.gameObject, this.gun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, ((this.Owner as PlayerController).CurrentGun == null) ? 0f : (this.Owner as PlayerController).CurrentGun.CurrentAngle));
            Projectile component = gameObject.GetComponent<Projectile>();
            component.Owner = (this.Owner as PlayerController);
            component.angularVelocity = 700;
            
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
           

            base.PostProcessProjectile(projectile);
        }


        
        public int LoneCount = 5;
        public float perload = .5f;

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

                
                if (this.gun.IsReloading && Key(GungeonActions.GungeonActionType.Shoot, this.gun.CurrentOwner as PlayerController) && this.gun.ClipShotsRemaining > 0)
                {
                    LoneCount = gun.ClipShotsRemaining;
                    this.gun.ForceImmediateReload(false);
                    gun.ClipShotsRemaining = LoneCount;
                }
                
                if(gun.ClipShotsRemaining <=0 && Key(GungeonActions.GungeonActionType.Shoot, this.gun.CurrentOwner as PlayerController))
                {
                    PlayerController player = this.gun.CurrentOwner as PlayerController;
                    this.gun.ClipShotsRemaining = 0;
                    gun.reloadTime = ((this.gun.ClipCapacity - this.gun.ClipShotsRemaining) * perload) * player.stats.GetStatValue(PlayerStats.StatType.ReloadSpeed);
                    gun.reloadTime = gun.reloadTime + (1 * player.stats.GetStatValue(PlayerStats.StatType.ReloadSpeed));
                    gun.Reload();
                    if (gun.IsReloading && this.HasReloaded)
                    {
                        AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                        HasReloaded = false;
                        
                        StartCoroutine(gradualReload());
                        

                    }

                }


                if (!this.gun.IsReloading)
                {
                    PlayerController player = (PlayerController)this.gun.CurrentOwner;


                    gun.reloadTime = ((this.gun.ClipCapacity - this.gun.ClipShotsRemaining) * perload) * player.stats.GetStatValue(PlayerStats.StatType.ReloadSpeed);

                    if (gun.ClipShotsRemaining <= 0)
                    {
                        gun.reloadTime = gun.reloadTime + (1 * player.stats.GetStatValue(PlayerStats.StatType.ReloadSpeed));
                    }


                }

               


            }

            base.Update();
        }
       
        public override void OnReloadPressed(PlayerController player, Gun gun, bool manualReload)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                HasReloaded = false;
                base.OnReload(player, gun);
                
                StartCoroutine(gradualReload());
                
                
            }
            
        }

        public static MethodInfo info = typeof(Gun).GetMethod("FinishReload", BindingFlags.Instance | BindingFlags.NonPublic);
        public bool Key(GungeonActions.GungeonActionType action, PlayerController user)
        {
            return BraveInput.GetInstanceForPlayer(user.PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
        }

        bool isdoing = false;
        public IEnumerator gradualReload()
        {
            isdoing = true;
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
            isdoing = false;
        }
      
    }
}
