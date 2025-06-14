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
    class SkudgePump : AdvancedGunBehaviour
    {
        public static void Add()
        {
            
            Gun gun = ETGMod.Databases.Items.NewGun("Sludge Pump", "skudge_pump");
            
            Game.Items.Rename("outdated_gun_mods:sludge_pump", "ski:sludge_pump");
            gun.gameObject.AddComponent<SkudgePump>();
          
            gun.SetShortDescription("Corrosive Cannon");

            gun.SetLongDescription("A pressurized plumbing system made for lobbing acid.\n" +
                "While illegal for most wartime situations it serves particularly well as pest control on glyphid infested planets." +
                "" +
                " \n\n\n - Knife_to_a_Gunfight" +
                "\n Sprites in collab with Lynceus");
            
            gun.SetupSprite(null, "skudge_pump_idle_001", 6);
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 5);
            gun.SetAnimationFPS(gun.chargeAnimation, 4);

           
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 4;

            gun.DefaultModule.ammoCost = 3;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.CHARGE;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 2f;

            gun.DefaultModule.cooldownTime = .6f;
            gun.DefaultModule.numberOfShotsInClip = 20;
            gun.SetBaseMaxAmmo(400);
            gun.barrelOffset.transform.localPosition = new Vector3(1.5f, .7f, 0f);
            gun.quality = PickupObject.ItemQuality.B;


            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 5f;
            projectile.baseData.speed = 15f;
            projectile.baseData.range = 8f;
            projectile.PlayerKnockbackForce = 4;
            projectile.AdditionalScaleMultiplier = 1f;

            projectile.SetProjectileSpriteRight("sludge_pump_projectile_small", 5, 5, false, tk2dBaseSprite.Anchor.MiddleCenter, 5, 5);

            GoopModifier goop = projectile.gameObject.GetOrAddComponent<GoopModifier>();
            goop.goopDefinition = EasyGoopDefinitions.SludgeGoop;
            goop.CollisionSpawnRadius = 2;
            goop.SpawnGoopOnCollision = true;
            smallGoop = projectile;
            projectileStates state = projectile.gameObject.GetOrAddComponent<projectileStates>();
            state.sludgePumpSmol = true;

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            projectile2.baseData.damage = 1f;
            projectile2.baseData.speed = 20f;
            projectile2.baseData.range = 14f;
            projectile2.PlayerKnockbackForce = 8;
            projectile2.AdditionalScaleMultiplier = 2f;
            projectile2.shouldRotate = true;
            projectile2.angularVelocity = 360;

            projectile2.gameObject.GetOrAddComponent<SludgeBurtsOnDeath>();

            GoopModifier goop2 = projectile2.gameObject.GetOrAddComponent<GoopModifier>();
            goop2.goopDefinition = EasyGoopDefinitions.SludgeGoop;
            goop2.CollisionSpawnRadius = 2;
            goop2.SpawnGoopOnCollision = true;

            projectileStates state2 = projectile2.gameObject.GetOrAddComponent<projectileStates>();
            state2.sludgePumpBeeg = true;

            MiscToolMethods.EasyAnimate(projectile2, new List<string>() {
                "sludge_pump_projectile_big1",
                "sludge_pump_projectile_big2",
                "sludge_pump_projectile_big3",
                "sludge_pump_projectile_big4",
            }, 4, new IntVector2(10, 10), 6, false, tk2dSpriteAnimationClip.WrapMode.Loop);

            projectile.enemyImpactEventName = "Play_WPN_eyeballgun_impact_01";
            projectile.objectImpactEventName = "Play_WPN_eyeballgun_impact_01";
            projectile2.enemyImpactEventName = "Play_WPN_trashgun_impact_01";
            projectile2.objectImpactEventName = "Play_WPN_trashgun_impact_01";
            projectile2.transform.parent = gun.barrelOffset;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "green_beam";
            gun.carryPixelOffset = new IntVector2(2, 2);
            gun.muzzleFlashEffects = ((Gun)PickupObjectDatabase.GetById(207)).muzzleFlashEffects;
            gun.barrelOffset.transform.position = new Vector3(39 / 16f, 16/ 16f, 0);

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f
            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                AmmoCost = 2,
                Projectile = projectile2,
                ChargeTime = .9f
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2
            };


            ETGMod.Databases.Items.Add(gun, null, "ANY");
            MiscToolMethods.TrimAllGunSprites(gun);

            ID = gun.PickupObjectId;
        }

        public static Projectile smallGoop;
        public static int ID;

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //AkSoundEngine.PostEvent("Play_ENM_blobulord_death_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_ENM_blobulord_reform_01", base.gameObject); 
        }
        public System.Random rng = new System.Random();
        public override void PostProcessProjectile(Projectile projectile)
        {
            PlayerController player = this.gun.CurrentOwner as PlayerController;
            if (projectile.gameObject.GetOrAddComponent<projectileStates>().sludgePumpSmol)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                AkSoundEngine.PostEvent("Play_PET_junk_splat_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_WPN_mailbox_shot_01", base.gameObject);
                if (player.PlayerHasActiveSynergy("GooBomber Special"))
                {
                    projectile.baseData.range *= 1.5f;
                    GoopModifier goop = projectile.gameObject.GetOrAddComponent<GoopModifier>();
                    goop.SpawnGoopInFlight = true;
                    goop.InFlightSpawnFrequency = .1f;
                    goop.InFlightSpawnRadius = 1;
                    goop.CollisionSpawnRadius = 1;
                }
                projectile.OnDestruction += Projectile_OnDestruction;
                projectile.OnHitEnemy += this.OnhitEnemy;
            }
            if (projectile.gameObject.GetOrAddComponent<projectileStates>().sludgePumpBeeg)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                AkSoundEngine.PostEvent("Play_WPN_trashgun_shot_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_PET_junk_splat_01", base.gameObject);
                gun.LoseAmmo(2); // total 5
                gun.ClipShotsRemaining -= 2;
                if (player.PlayerHasActiveSynergy("GooBomber Special"))
                {
                    projectile.baseData.range *= 1.5f;
                    GoopModifier goop = projectile.gameObject.GetOrAddComponent<GoopModifier>();
                    goop.SpawnGoopInFlight = true;
                    goop.InFlightSpawnFrequency = .1f;
                    goop.InFlightSpawnRadius = 1;
                    goop.CollisionSpawnRadius = 1;
                }
                projectile.OnHitEnemy += this.OnhitEnemy;
            }

            base.PostProcessProjectile(projectile);
        }

        private void Projectile_OnDestruction(Projectile obj)
        {
            GoopDefinition fire = EasyGoopDefinitions.SludgeGoop;
            DeadlyDeadlyGoopManager goop = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(fire);
            goop.TimedAddGoopCircle(obj.LastPosition, 1.5f, 1f, true);
        }

        private void OnhitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.aiActor != null)
            {
                if (arg2.aiActor.healthHaver != null)
                {
                    if (!arg3)
                    {
                        arg2.aiActor.ApplyEffect(new GameActorSludgeEffect()
                        {
                            duration = 4,
                            stackMode = GameActorEffect.EffectStackingMode.Refresh,
                            SpeedMultiplier = 0.3f,
                        });
                    }
                }
            }
        }

        private bool HasReloaded;
        public bool ReloadPullSound = false;

        public override void Update()
        {
            if (gun.CurrentOwner != null)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
                if(gun.IsReloading && gun.spriteAnimator.CurrentFrame == 6)
                {
                    if (ReloadPullSound == false)
                    {
                        ReloadPullSound = true;
                        AkSoundEngine.PostEvent("Play_OBJ_chain_switch_01", base.gameObject);
                    }
                }



            }




            base.Update();
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                ReloadPullSound = false;
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_OBJ_trashbag_burst_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_OBJ_drum_break_01", base.gameObject);
                
            }
        }


    }
}