using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using System.CodeDom;

namespace Knives
{
    class CoolerChargeRifle : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Charge Rifle", "Charger");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:charge_rifle", "ski:charge_rifle");
            gun.gameObject.AddComponent<CoolerChargeRifle>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("The Bigger They Are...");
            gun.SetLongDescription("Deals damage based on enemy size. The bigger they are the harder they will fall.\n\n" +
                "" +
                "This rifle was designed to rip titans appart in the frontier wars, but has been henceforth retired by the hegemony of man as a weapon too inhumane for even their gruesome standards. \n\n" +
                "Along side dealing more damage to large units this weapon's radiation has a particularly nasty effect on some species of blobulons causing their body structures to dismantle when the gun is charged." +
                "\n\n\n - Knife_to_a_Gunfight");

            
            gun.SetupSprite(null, "Charger_idle_001", 4);
            gun.SetAnimationFPS(gun.chargeAnimation, 5);
            gun.SetAnimationFPS(gun.shootAnimation, 6);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 5;


            gun.AddProjectileModuleFrom((PickupObjectDatabase.GetById(15) as Gun), true, true);
           

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.BEAM;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.8f;
            
            gun.DefaultModule.cooldownTime = .4f;
            gun.DefaultModule.numberOfShotsInClip = 4;
            gun.SetBaseMaxAmmo(80);

            gun.quality = PickupObject.ItemQuality.B;
            gun.carryPixelOffset = new IntVector2(7, 0);

           

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 10f;
            projectile.baseData.speed *= .01f;
            projectile.BossDamageMultiplier = 1.2f;
            projectile.baseData.range = .3f;


            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 1f,
                AdditionalWwiseEvent = "Play_Charger_up"
            };
        
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,

            };
            gun.PreventNormalFireAudio = true;

            List<string> BeamAnimPaths = new List<string>()
            {
                "Knives/Resources/BeamSprites/CrgBm_mid_001",
                "Knives/Resources/BeamSprites/CrgBm_mid_002",
                "Knives/Resources/BeamSprites/CrgBm_mid_003",

            };
            List<string> BeamEndPaths = new List<string>()
            {
                "Knives/Resources/BeamSprites/CrgBm_end_001",
                "Knives/Resources/BeamSprites/CrgBm_end_002",
                "Knives/Resources/BeamSprites/CrgBm_end_003",

            };
            List<string> BeamStartPaths = new List<string>()
            {
                "Knives/Resources/BeamSprites/CrgBm_start_001",
                "Knives/Resources/BeamSprites/CrgBm_start_002",
                "Knives/Resources/BeamSprites/CrgBm_start_003",
            };

            //BULLET STATS
            Projectile BeamProj = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);

            BasicBeamController beamComp = BeamProj.GenerateBeamPrefab(
                "Knives/Resources/BeamSprites/bm_mid_001",
                new Vector2(5, 3),
                new Vector2(0, 1),
                BeamAnimPaths,
                9,
                //Impact
                null,
                -1,
                null,
                null,
                //End
                BeamEndPaths,
                9,
                new Vector2(5, 3),
                new Vector2(0, 1),
                //Beginning
                BeamStartPaths,
                9,
                new Vector2(5, 3),
                new Vector2(0, 1)
                );

            BeamProj.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(BeamProj.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(BeamProj);
            BeamProj.baseData.damage = 40f;
            BeamProj.baseData.force *= 0.1f;
            BeamProj.baseData.range = 20f;
            BeamProj.baseData.speed *= 5f;

            BeamProj.gameObject.GetOrAddComponent<EnemySizeDamageScaleModifier>(); 
            beamComp.penetration = 100;
            beamComp.usesChargeDelay = true;
            beamComp.chargeDelay = 1.2f;
            beamComp.boneType = BasicBeamController.BeamBoneType.Straight;
            beamComp.DamageModifier = 1f;
            beamComp.interpolateStretchedBones = true;

            BeamBulletsBehaviour beamy = projectile.gameObject.GetOrAddComponent<BeamBulletsBehaviour>();
            beamy.beamToFire = BeamProj;
            beamy.firetype = BeamBulletsBehaviour.FireType.FORWARDS;

            ETGMod.Databases.Items.Add(gun, null, "ANY");


        }
        System.Random rng = new System.Random();
        public override void PostProcessProjectile(Projectile projectile)
        {
            
        }
        
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
           
        }
        private bool HasReloaded;
        public bool toggle = false;
        public override void Update()
        {
            if (this.gun.CurrentOwner)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;

                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }

               

                if (gun.GetChargeFraction() == 1)
                {

                    BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer((this.gun.CurrentOwner as PlayerController).PlayerIDX);
                    if (instanceForPlayer.ActiveActions.ShootAction && Time.timeScale != 0 && gun.ClipShotsRemaining != 0 && !instanceForPlayer.ActiveActions.MapAction.IsPressed && Player.IsDodgeRolling == false)
                    {
                        
                        instanceForPlayer.ActiveActions.ShootAction.ClearInputState();
                        AkSoundEngine.PostEvent("Play_Charger_fire", base.gameObject);
                    }
                }
                
            }

            base.Update();
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