using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;

using MultiplayerBasicExample;
using System.Collections.Generic;
using Alexandria.ItemAPI;

namespace Knives
{

    public class Galileo : AdvancedGunBehavior
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Galileo", "Galileo");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:galileo", "ski:galileo");
            gun.gameObject.AddComponent<Galileo>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Magnifico");
            gun.SetLongDescription("The gun of a slinger more focused on the heavenly bodies than the follies of man.\n" +
                "Aim true and let the movements of the stars guide you." +
                "\n\n\n - Knife_to_a_Gunfight");
            gun.SetupSprite(null, "Galileo_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(36) as Gun, true, false);
            
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.3f;
            gun.DefaultModule.cooldownTime = .25f;
            gun.DefaultModule.numberOfShotsInClip = 6;
            gun.SetBaseMaxAmmo(400);
            gun.carryPixelOffset = new IntVector2(7, 0);
            gun.barrelOffset.transform.position = new Vector3(49 / 16f, 25 / 16f, 0);
            gun.quality = PickupObject.ItemQuality.B;


            gun.muzzleFlashEffects = ((Gun)PickupObjectDatabase.GetById(223)).muzzleFlashEffects;
            gun.finalMuzzleFlashEffects = ((Gun)PickupObjectDatabase.GetById(385)).muzzleFlashEffects;


            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 6f;
            projectile.baseData.speed *= 2.5f;
            projectile.baseData.range = 16;
            

            MiscToolMethods.EasyAnimate(projectile, new List<string>() {
                "Galileo_001",
                "Galileo_002",

            }, 4, new IntVector2(5, 5), 6, false, tk2dSpriteAnimationClip.WrapMode.Loop);

            Projectile fin = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            fin.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(fin.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(fin);
            
            fin.baseData.damage = 20f;
            fin.baseData.speed *= 3f;
            fin.baseData.range = 40;
            fin.SuppressHitEffects = true;
            fin.hitEffects.suppressMidairDeathVfx = true;
            PierceProjModifier stabby = fin.gameObject.GetOrAddComponent<PierceProjModifier>();
            stabby.penetration = 12;
            stabby.penetratesBreakables = true;
            

            StarPathingMod star = fin.gameObject.GetOrAddComponent<StarPathingMod>();

            gun.DefaultModule.finalProjectile = fin;
            gun.DefaultModule.numberOfFinalProjectiles = 1;
            gun.DefaultModule.usesOptionalFinalProjectile = true;

            List<string> BeamAnimPaths = new List<string>()
            {
                "Knives/Resources/BeamSprites/Blue_lzr_001",
                "Knives/Resources/BeamSprites/Blue_lzr_002",
                "Knives/Resources/BeamSprites/Blue_lzr_003",
            };
            fin.AddTrailToProjectile(
                 "Knives/Resources/BeamSprites/Blue_lzr_001",
                new Vector2(3, 2),
                new Vector2(1, 1),
                BeamAnimPaths, 20,
                BeamAnimPaths, 20,
                -1,
                0.0001f,
                6,
                false
                );
            /*
            projectile.AddTrailToProjectile(
                 "Knives/Resources/BeamSprites/Blue_lzr_001",
                new Vector2(3, 2),
                new Vector2(1, 1),
                BeamAnimPaths, 20,
                BeamAnimPaths, 20,
                -1,
                0.001f,
                4,
                false
                );
            */
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "pulse blue";

            gun.DefaultModule.finalAmmoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.finalCustomAmmoType = "planet";


            MiscToolMethods.TrimAllGunSprites(gun);
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");


        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            if (projectile.GetCachedBaseDamage == 6)
            {
                AkSoundEngine.PostEvent("Play_WPN_spellactionrevolver_shot_01", projectile.gameObject);
                AkSoundEngine.PostEvent("Play_WPN_e12_shot_01", projectile.gameObject);
            }
            if (projectile.GetCachedBaseDamage == 20)
            {
                AkSoundEngine.PostEvent("Play_WPN_spellactionrevolver_shot_01", projectile.gameObject);
                AkSoundEngine.PostEvent("Play_WPN_plasmacell_shot_01", projectile.gameObject);
                gun.spriteAnimator.ForceClearCurrentClip();
                tk2dSpriteAnimationClip clip = gun.spriteAnimator.GetClipByName("Galileo_critical_fire");
                gun.spriteAnimator.Play(clip, 0, 12);
            }

            base.PostProcessProjectile(projectile);
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {
           
        }

        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
        protected override void Update()
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
                AkSoundEngine.PostEvent("Play_cata_reload_sfx", base.gameObject);
            }



        }
        
    }
    public class StarPathingMod : MonoBehaviour
    {
        public StarPathingMod()
        {

        }

        private void Start()
        {

            this.m_projectile = base.GetComponent<Projectile>();
            if (this.m_projectile.Owner is PlayerController)
            {
                this.projOwner = this.m_projectile.Owner as PlayerController;
            }
            m_projectile.OnHitEnemy += this.onHit;
            RenderSettings.ambientLight = ExtendedColours.MikuBlue;
        }

        private void onHit(Projectile proj, SpeculativeRigidbody body, bool fatal)
        {
            if(body.aiActor != null)
            {
                if (!alreadyHit.Contains(body.aiActor))
                {
                    if (alreadyHit.Count > 0) body.healthHaver.ApplyDamage(10, Vector2.zero, "StarStruck", CoreDamageTypes.Void, DamageCategory.Unstoppable, true, null, true);
                    alreadyHit.Add(body.aiActor);
                    Vector2 dir = OMITBMathsAndLogicExtensions.GetVectorToNearestEnemy(proj.LastPosition, 0, 0, null, alreadyHit);
                    proj.SendInDirection(dir, true, false);
                    m_projectile.PossibleSourceGun.LoseAmmo(1);
                    VFXPool star = ((Gun)PickupObjectDatabase.GetById(97)).muzzleFlashEffects;
                    star.SpawnAtPosition(body.UnitCenter,90);
                }
            }
        }

        private void Update()
        {
          
        }

        public List<AIActor> alreadyHit = new List<AIActor>();
        private PlayerController projOwner;
        private Projectile m_projectile;
    }
}