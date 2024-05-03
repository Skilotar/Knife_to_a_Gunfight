using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Gungeon;
using Dungeonator;
using MonoMod;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    public class ChargeRifle : AdvancedGunBehaviour
    {
        public static void Add()
        {

            Gun gun = ETGMod.Databases.Items.NewGun("Charge Rifle", "Charger");
            Game.Items.Rename("outdated_gun_mods:charge_rifle", "ski:charge_rifle");
            var behav = gun.gameObject.AddComponent<ChargeRifle>();

            behav.preventNormalFireAudio = true;
            gun.SetShortDescription("The Bigger They Are...");
            gun.SetLongDescription("Deals damage based on enemy size. The bigger they are the harder they will fall.\n\n" +
                "" +
                "This rifle was designed to rip titans appart in the frontier wars, but has been henceforth retired by the hegemony of man as a weapon too inhumane for even their gruesome standards. \n\n" +
                "Along side dealing more damage to large units this weapon's radiation has a particularly nasty effect on some species of blobulons causing their body structures to dismantle when the gun is charged." +
                "\n\n\n - Knife_to_a_Gunfight");

            gun.SetupSprite(null, "Charger_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 9);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(86) as Gun, true, false);

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).loopStart = 14;

            gun.encounterTrackable.EncounterGuid = "prepare for titanfall";

            //GUN STATS
            gun.doesScreenShake = false;
            gun.DefaultModule.ammoCost = 10;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Beam;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            
            gun.reloadTime = 1f;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.DefaultModule.cooldownTime = 0.01f;
            gun.DefaultModule.numberOfShotsInClip = 10;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.gunClass = GunClass.CHARGE;
            gun.DefaultModule.customAmmoType = "yellow_beam";
            gun.barrelOffset.transform.localPosition = new Vector3(1.4f, .3f, 0f);
            gun.SetBaseMaxAmmo(400);
            

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
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);

            BasicBeamController beamComp = projectile.GenerateBeamPrefab(
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

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.baseData.damage = 45f;
            projectile.baseData.force *= 0.1f;
            projectile.baseData.range = 20f;
            projectile.baseData.speed *= 5f;
            
            projectile.gameObject.GetOrAddComponent<EnemySizeDamageScaleModifier>();
           


            beamComp.penetration = 100;
            beamComp.usesChargeDelay = true;
            beamComp.chargeDelay = 1.2f;
            beamComp.boneType = BasicBeamController.BeamBoneType.Straight;
            beamComp.DamageModifier = 1f;
            beamComp.interpolateStretchedBones = true;
            gun.DefaultModule.projectiles[0] = projectile;
            
            gun.quality = PickupObject.ItemQuality.B;

            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }

        public static int ID;
        public override void PostProcessProjectile(Projectile projectile)
        {

            
            base.PostProcessProjectile(projectile);

        }


        private bool HasReloaded;
        public bool limiter = false; //controls sound functions in update only firing once
        public System.Random rng = new System.Random();
        public override void  Update()
        {
            if (this.gun.CurrentOwner != null)
            {

                PlayerController player = (PlayerController)this.gun.CurrentOwner;

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }


                if (this.gun.ClipShotsRemaining > 1 && gun.IsFiring && limiter == false && this.HasReloaded && Time.timeScale > 0f)
                {

                   
                    int sound = rng.Next(1, 3);
                    if (sound == 1)
                    {
                        AkSoundEngine.PostEvent("Play_Crg_fire_001", base.gameObject);
                        AkSoundEngine.PostEvent("Play_Crg_fire_001", base.gameObject);
                    }
                    if (sound == 2)
                    {
                        AkSoundEngine.PostEvent("Play_Crg_fire_002", base.gameObject);
                        AkSoundEngine.PostEvent("Play_Crg_fire_002", base.gameObject);
                    }
                    if (sound == 3)
                    {
                        AkSoundEngine.PostEvent("Play_Crg_fire_003", base.gameObject);
                        AkSoundEngine.PostEvent("Play_Crg_fire_003", base.gameObject);
                    }
                    limiter = true;

                    RoomHandler currentRoom = (this.Owner as PlayerController).CurrentRoom;
                    if(currentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All) > 0)
                    {
                        foreach (AIActor aiactor in currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                        {
                            if (aiactor != null)
                            {
                                for (int i = 0; i < this.wetwork.Count; i++)
                                {
                                    bool isin = aiactor.EnemyGuid == this.wetwork[i];

                                    if (isin)
                                    {
                                        aiactor.healthHaver.ApplyDamage(3f, Vector2.zero, "radiation");

                                    }
                                }

                            }

                        }
                    }
                    
                }

                if (!gun.IsFiring && limiter)
                {
                    
                    AkSoundEngine.PostEvent("Stop_Crg_fire_001", base.gameObject);
                    AkSoundEngine.PostEvent("Stop_Crg_fire_002", base.gameObject);
                    AkSoundEngine.PostEvent("Stop_Crg_fire_003", base.gameObject);
                    AkSoundEngine.PostEvent("Stop_Crg_fire_001", base.gameObject);
                    AkSoundEngine.PostEvent("Stop_Crg_fire_002", base.gameObject);
                    AkSoundEngine.PostEvent("Stop_Crg_fire_003", base.gameObject);

                    limiter = false;
                }

                if (gun.IsFiring && player.IsDodgeRolling && limiter)
                {

                    
                    AkSoundEngine.PostEvent("Stop_Crg_fire_001", base.gameObject);
                    AkSoundEngine.PostEvent("Stop_Crg_fire_002", base.gameObject);
                    AkSoundEngine.PostEvent("Stop_Crg_fire_003", base.gameObject);

                    limiter = false;
                }
               

            }

            
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                this.gun.Update();


                limiter = false;

                HasReloaded = false;

                base.OnReloadPressed(player, gun, bSOMETHING);


            }

        }

        public ChargeRifle()
        {

        }

        public List<string> wetwork = new List<string>
        {   "042edb1dfb614dc385d5ad1b010f2ee3",
            "42be66373a3d4d89b91a35c9ff8adfec",
            "0239c0680f9f467dbe5c4aab7dd1eca6",
            "7b0b1b6d9ce7405b86b75ce648025dd6",
            "864ea5a6a9324efc95a0dd2407f42810",
            "022d7c822bc146b58fe3b0287568aaa2",
            "f155fd2759764f4a9217db29dd21b7eb",
            "9189f46c47564ed588b9108965f975c9",
            "e61cab252cfb435db9172adc96ded75f",
            "fe3fe59d867347839824d5d9ae87f244",
            "b8103805af174924b578c98e95313074",
            "6e972cd3b11e4b429b888b488e308551",
            "044a9f39712f456597b9762893fbc19c",
            "479556d05c7c44f3b6abb3b2067fc778",
            "1bc2a07ef87741be90c37096910843ab",
            "475c20c1fd474dfbad54954e7cba29c1",
            "1b5810fafbec445d89921a4efb4e42b7",
        };


    }
}
       
