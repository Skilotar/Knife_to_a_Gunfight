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
    class GunLance : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("GunLance", "GnLnc");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:gunlance", "ski:gunlance");
            gun.gameObject.AddComponent<GunLance>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("This Is My BOOM STICK!");
            gun.SetLongDescription("A lance made from Dragun peices that has been retrofitted with a large cannon barrel and a cylinder." +
                " As The curse of Kaliber spread through the gungeon many weapons of old were retroactively upgraded to fill her insatiable desire for firearms.\n\n" +
                "Hold charge to change 3 different fire types!\n" +
                "No charge = swipe. \n" +
                "First Charge = shelling. \n" +
                "Second Charge = wyvernstake."+
                "\n\n\n - Knife_to_a_Gunfight"); ;

            
            gun.SetupSprite(null, "GnLnc_idle_001", 8);
            gun.SetAnimationFPS(gun.chargeAnimation, 5);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 10);
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(59) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 8;
           


            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.TwoHanded;
            
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            
            gun.reloadTime = 2f;
            gun.DefaultModule.numberOfShotsInClip = 4;
            gun.DefaultModule.cooldownTime = .6f;
            gun.OverrideAngleSnap = 1f;


            gun.SetBaseMaxAmmo(150);
            gun.quality = PickupObject.ItemQuality.B;
            gun.encounterTrackable.EncounterGuid = "Here Comes the BOOOOOOM!";

            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(1.5f, .7f, 0f);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "sail";


            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 8f;
            projectile.baseData.speed = 3f;
            projectile.baseData.range = 3f;
            projectile.baseData.force = 5;
            projectile.name = "slash";
            projectile.tag = "slash";
           
            
           
            
           //shelling
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(370) as Gun).DefaultModule.chargeProjectiles[1].Projectile);

            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            projectile2.baseData.damage = 20f;
            projectile2.baseData.speed = 50f;
            projectile2.baseData.range = 40f;
            projectile2.DefaultTintColor = UnityEngine.Color.gray;
            projectile2.name = "shelling";

           


            //wyvernstake
            Projectile projectile3 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(370) as Gun).DefaultModule.chargeProjectiles[1].Projectile);

            projectile3.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile3.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile3);
            gun.DefaultModule.projectiles[0] = projectile3;
            projectile3.baseData.damage = 50f;
            projectile3.baseData.speed = 70f;
            projectile3.baseData.range = 80f;
            projectile3.AdditionalScaleMultiplier = 3f;
            projectile3.HasDefaultTint = true;
            projectile3.DefaultTintColor = UnityEngine.Color.grey;
            projectile3.AppliesKnockbackToPlayer = true;
            projectile3.PlayerKnockbackForce = 6;
            projectile3.name = "stake";
            UnchangeableRangeController ranger3 = projectile3.gameObject.AddComponent<UnchangeableRangeController>();
            ranger3.Awake();
            Gun swipeFlash = (Gun)PickupObjectDatabase.GetById(335);
            gun.muzzleFlashEffects.type = VFXPoolType.None;

            Gun shellingFlash = (Gun)PickupObjectDatabase.GetById(17);
          

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f,
                AmmoCost = 0,
                
                
            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile2,
                ChargeTime = .2f,
                AmmoCost = 1,

            };
            ProjectileModule.ChargeProjectile item3 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile3,
                ChargeTime = 1.8f,
                AmmoCost = 1,


            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2,
                item3,
            };
            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            MiscToolMethods.TrimAllGunSprites(gun);
            ID = gun.PickupObjectId;
        }


        public static int ID;
        public override void  OnPickedUpByPlayer(PlayerController player)
        {
           
            base.OnPickedUpByPlayer(player);
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {

                gun.PreventNormalFireAudio = true;
        }
        public override void PostProcessProjectile(Projectile projectile)
        {

            starttoggle = false;
            
            
            if (projectile.GetCachedBaseDamage == 8f)
            {
                //slash

                gun.spriteAnimator.StopAndResetFrameToDefault();
                gun.spriteAnimator.Play("GnLnc_critical_fire");
                Gun swipeFlash = (Gun)PickupObjectDatabase.GetById(335);
                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 45;
                slasher.SlashRange = 5f;
                slasher.playerKnockback = 0;
                slasher.SlashDamage = 15;
                slasher.soundToPlay = "Play_gln_swing_miss_001";
                slasher.SlashVFX = swipeFlash.muzzleFlashEffects;
                player.knockbackDoer.ApplyKnockback(vector, 25, true);
                slasher.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
                gun.MoveBulletsIntoClip(2);
                gun.GainAmmo(1);
                
                AkSoundEngine.PostEvent("Play_gln_swing_miss_001", base.gameObject);
                AkSoundEngine.PostEvent("Play_gln_swing_miss_001", base.gameObject);
                
                

            }
            if (projectile.GetCachedBaseDamage == 20f)
            {
                //shelling
                AkSoundEngine.PostEvent("Play_gln_fire_small_001", base.gameObject);
                AkSoundEngine.PostEvent("Play_gln_fire_small_001", base.gameObject);
                AkSoundEngine.PostEvent("Play_gln_fire_small_001", base.gameObject);


            }
            if (projectile.GetCachedBaseDamage == 50f)
            {
                //wyvernstake
                PierceProjModifier stabby = projectile.gameObject.AddComponent<PierceProjModifier>();
                stabby.penetration = 5;
                stabby.penetratesBreakables = true;
                gun.LoseAmmo(5);
                gun.ClipShotsRemaining = gun.ClipShotsRemaining - 4;
                projectile.OnHitEnemy += this.OnHitEnemy;
                AkSoundEngine.PostEvent("Play_gln_fire_big_001", base.gameObject);
                AkSoundEngine.PostEvent("Play_gln_fire_big_001", base.gameObject);
            

            }


            base.PostProcessProjectile(projectile);
        }

        public void OnHitEnemy(Projectile bullet, SpeculativeRigidbody Enemy, bool probably)
        {
            if (Enemy.aiActor != null)
            {
                Vector2 here = Enemy.gameActor.CenterPosition;
                this.DoSafeExplosion(here);
                Enemy.gameActor.healthHaver.ApplyDamage(60, Vector2.zero, "explody dps go brrt past stupid boss cap thingy", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, true);
            }
            
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
            doDamage = false,
            damage = 10f,
            doDestroyProjectiles = true,
            doForce = false,
            debrisForce = 12f,
            preventPlayerForce = true,
            explosionDelay = 0.1f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = true
        };

        private bool HasReloaded;

        bool starttoggle = false;
        
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

                if(gun.HasChargedProjectileReady && !starttoggle)
                {
                    AkSoundEngine.PostEvent("Play_gln_charge_start", base.gameObject);
                    AkSoundEngine.PostEvent("Play_gln_charge_start", base.gameObject);
                    AkSoundEngine.PostEvent("Play_gln_charge_start", base.gameObject);
                    AkSoundEngine.PostEvent("Play_gln_charge_start", base.gameObject);
                    AkSoundEngine.PostEvent("Play_gln_charge_start", base.gameObject);
                    AkSoundEngine.PostEvent("Play_gln_charge_start", base.gameObject);
                    gun.Attack();
                    starttoggle = true;
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
                AkSoundEngine.PostEvent("Play_gln_reload_001", base.gameObject);
                AkSoundEngine.PostEvent("Play_gln_reload_001", base.gameObject);
                AkSoundEngine.PostEvent("Play_gln_reload_001", base.gameObject);
                AkSoundEngine.PostEvent("Play_gln_reload_001", base.gameObject);
            }
        }


    }
}

