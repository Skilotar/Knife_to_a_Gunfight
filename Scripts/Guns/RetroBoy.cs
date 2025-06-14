using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using System.Collections.Generic;

namespace Knives
{

    class RetroBoy : AdvancedGunBehavior
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("RetroBoy", "RetroBlaster");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:retroboy", "ski:retro_boy");
            var Behav = gun.gameObject.AddComponent<RetroBoy>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Old Gold");
            gun.SetLongDescription("Reloads gradually. Can fire loaded bullets to interrupt reload. \n" +
                "A relic of the past, this one of a kind Retroboy console case was made to be an immersive experience.\n\n" +
                "It was rapidly recalled after it was found that the capture balls it fired actually worked and children were causing havoc on the local ecosystems." +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "RetroBlaster_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);
            gun.SetAnimationFPS(gun.introAnimation, 6);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).loopStart = 2;

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(89) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 13f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 2.6f;
            gun.DefaultModule.cooldownTime = .15f;
            gun.DefaultModule.numberOfShotsInClip = 25;
            gun.SetBaseMaxAmmo(1025);
            gun.ammo = 1025;
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;
            gun.muzzleFlashEffects = ((Gun)PickupObjectDatabase.GetById(89)).muzzleFlashEffects;

            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            gun.barrelOffset.transform.localPosition = new Vector3(28/16f, 14/16f, 0f);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.SetProjectileSpriteRight("RetroBoyProj", 22, 8, false, tk2dBaseSprite.Anchor.MiddleCenter, 22, 8);
            //projectile.AdditionalScaleMultiplier *= .5f;
            projectile.baseData.damage = 4f;
            projectile.baseData.speed *= 1.2f;
            
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(15) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.usesOptionalFinalProjectile = true;
            gun.DefaultModule.finalAmmoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.finalCustomAmmoType = "recharge";
            gun.DefaultModule.finalProjectile = projectile2;
            gun.DefaultModule.numberOfFinalProjectiles = 0;
            projectile2.baseData.damage = 1f;
            projectile2.baseData.speed *= .8f;
            projectile2.baseData.range = 20;
            projectile2.BossDamageMultiplier *= 10;
            MiscToolMethods.EasyAnimate(projectile2, new List<string>() {
                "Retro_L_01",
                "Retro_L_02",
            }, 6, new IntVector2(15, 15), 6, false, tk2dSpriteAnimationClip.WrapMode.Once,"Bounce_left");
            MiscToolMethods.EasyAnimate(projectile2, new List<string>() {
                "Retro_R_01",
                "Retro_R_02",
            }, 6, new IntVector2(15, 15), 6, false, tk2dSpriteAnimationClip.WrapMode.Once, "Bounce_right");


            projectile2.SetProjectileSpriteRight("RetroBallProj", 13, 13, false, tk2dBaseSprite.Anchor.MiddleCenter, 11, 11);
            PokeBallComp ball = projectile2.gameObject.GetOrAddComponent<PokeBallComp>();
            ball.basegun = gun;
            Ball = projectile2;


            gun.carryPixelOffset = new IntVector2(14, 0);
            GunGradualReload reload = gun.gameObject.GetOrAddComponent<GunGradualReload>();
            reload.emptyclipPunishtime = 0f;
            reload.mincliptofire = 8;
            reload.reloadStartSound = "Play_Retro_Save_001";
            reload.reloadSustainPlaySound = "Play_Retro_Save_002";
            reload.reloadSustainStopSound = "Stop_Retro_Save_002";
            reload.IntroSound = "Play_Retro_intro_001";
            reload.reloadEndSound = "Play_Retro_Save_003";

            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.FULLAUTO;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;

        }

        public static Projectile Ball;
        public PlayerController globalplayer;
        public List<AIActor> pokemon = new List<AIActor>();

        public static int ID;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_Retro_fire_001", base.gameObject);
            //AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
           
            base.PostProcessProjectile(projectile);
        }


        private bool HasReloaded = true;
        public bool Once = false;
        public bool setup = false;
        public float internalCooldown = 0;

        //This block of code allows us to change the reload sounds.
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
                    Once = false;

                }
                if(gun.IsReloading == true && Once == false && internalCooldown <= 0)
                {
                    Once = true;
                    HasReloaded = false;
                    this.gun.LoseAmmo(4);
                    PlayerController owner = gun.CurrentOwner as PlayerController;
                    MiscToolMethods.SpawnProjAtPosi(Ball, gun.barrelOffset.transform.position, owner, this.gun);
                    internalCooldown = 1.5f;
                }
                if(setup == false)
                {
                    gun.CurrentOwner.gameObject.GetOrAddComponent<PlayerDestroyPKMRoomClear>();
                }
                if(internalCooldown > 0)
                {
                    internalCooldown -= BraveTime.DeltaTime;
                }
               
            }
           
        }



        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {

            if (gun.IsReloading && this.HasReloaded)
            {
                
            }

        }

       
    }
}

