using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Brave;
using Brave.BulletScript;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using UnityEngine;

namespace Knives
{
    class HonorGaurd : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Honor Guard", "HonorGaurd");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:honor_guard", "ski:honor_guard");
            gun.gameObject.AddComponent<HonorGaurd>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Live With Honor...");
            gun.SetLongDescription("Die with Glory!\n\n" +
                "A relic of a guild Ghastly Knights. Lost wandering in the space between life and the afterlife. \n\n" +
                "Tap for a quick thrust, \n" +
                "Charge for a heavy Swing, \n" +
                "and reload to fire. " +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "HonorGaurd_idle_001", 8);
            gun.SetAnimationFPS(gun.chargeAnimation, 8);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 16);
            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(59) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 4;


            
            gun.DefaultModule.ammoCost = 0;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = .9f;
            gun.DefaultModule.numberOfShotsInClip = 2;
            gun.DefaultModule.cooldownTime = .5f;

            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.B;
            gun.muzzleFlashEffects = null;
            gun.carryPixelOffset = new IntVector2(8, 0);
            gun.barrelOffset.transform.localPosition = new Vector3(56/16f, 34/16f, 0f);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "Musketball";

            //stab
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 8f;
            projectile.baseData.speed = 3f;
            projectile.baseData.range = 3f;
            projectile.baseData.force = 5;
            projectile.AppliesKnockbackToPlayer = true;
            projectile.PlayerKnockbackForce = -26;
            
            projectileStates state = projectile.gameObject.GetOrAddComponent<projectileStates>();
            state.HonorStab = true;
            ProjectileSlashingBehaviour stabby = projectile.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            stabby.SlashDimensions = 30;
            stabby.SlashDamageUsesBaseProjectileDamage = false;
            stabby.SlashDamage = 18;
            stabby.SlashRange = 5.7f;
            stabby.slashKnockback = 7f;
            stabby.SlashVFX = null;
            stabby.InteractMode = SlashDoer.ProjInteractMode.IGNORE;
            stabby.NoCost = true;
            //stabby.soundToPlay = "";
           
            //slash
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            projectile2.baseData.damage = 9f;
            projectile2.baseData.speed = 3f;
            projectile2.baseData.range = 3f;
            projectile2.baseData.force = 5;
            projectileStates state2 = projectile2.gameObject.GetOrAddComponent<projectileStates>();
            state2.HonorSwipe = true;
            ProjectileSlashingBehaviour Slashy = projectile2.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            Slashy.SlashDimensions = 80;
            Slashy.SlashDamageUsesBaseProjectileDamage = false;
            Slashy.SlashDamage = 40;
            Slashy.SlashRange = 5.5f;
            Slashy.slashKnockback = 15f;
            Slashy.playerKnockback = 0;
            Slashy.SlashVFX = null;
            Slashy.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
            Slashy.soundToPlay = "Play_ENM_gunnut_swing_01";
            Slashy.NoCost = true;

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f,
                AmmoCost = 0,


            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile2,
                ChargeTime = .5f,
                AmmoCost = 0,
                

            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2,
            };
            MiscToolMethods.TrimAllGunSprites(gun);
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }


        public static int ID;
        public override void OnPickedUpByPlayer(PlayerController player)
        {

            base.OnPickedUpByPlayer(player);
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            
            gun.PreventNormalFireAudio = true;
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            
            if (projectile.gameObject.GetOrAddComponent<projectileStates>().HonorSwipe == true)
            {
                gun.spriteAnimator.Play("HonorGaurd_critical_fire");
            }
            base.PostProcessProjectile(projectile);
        }

        
        private bool HasReloaded;

        public override void Update()
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

            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            base.OnReloadPressed(player, gun, bSOMETHING);
            if (!gun.IsReloading)
            {
                player.StartCoroutine(ReloadShoot(player));
            }
        }
            

        private IEnumerator ReloadShoot(PlayerController player)
        {
            AkSoundEngine.PostEvent("Play_WPN_duelingpistol_shot_01", base.gameObject);
            gun.LoseAmmo(1); 
            gun.ClipShotsRemaining--;
            Projectile flint = ((Gun)PickupObjectDatabase.GetById(9)).DefaultModule.projectiles[0].projectile;
            Vector3 pointInSpace = this.gun.barrelOffset.transform.position;
            gun.Reload();

            MiscToolMethods.SpawnProjAtPosi(flint, pointInSpace, player, gun);
            //SpawnManager.SpawnVFX(, this.gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, ((gun.CurrentOwner as PlayerController).CurrentGun == null) ? 0f : (gun.CurrentOwner as PlayerController).CurrentGun.CurrentAngle));

            yield return new WaitForSeconds(.25f);
            
            AkSoundEngine.PostEvent("Play_WPN_duelingpistol_reload_01", base.gameObject);
        }
    }
}