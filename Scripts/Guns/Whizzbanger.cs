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
using System.Reflection;
using HarmonyLib;

namespace Knives
{
    class WhizzBanger : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Whizzbanger", "Whizzbanger");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:whizzbanger", "ski:whizzbanger");
            gun.gameObject.AddComponent<WhizzBanger>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Less is More");
            gun.SetLongDescription("Attack while reloading to swing your hammer.\n\n" +
                "Budget cuts in the hegemony of man's R&D department created this monstrosity. The accounting department requested the most cut down sniper R&D could develop.\n" +
                "This was the development team's snarky response." +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Whizzbanger_idle_001", 2);
            gun.SetAnimationFPS(gun.chargeAnimation, 8);
            gun.SetAnimationFPS(gun.shootAnimation, 4);
            gun.SetAnimationFPS(gun.reloadAnimation, 5);
            gun.SetAnimationFPS(gun.introAnimation, 8);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 8);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 3;


            gun.AddProjectileModuleFrom((PickupObjectDatabase.GetById(15) as Gun), true, true);
            gun.AddStatToGun(PlayerStats.StatType.Curse, .5f);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.CHARGE;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.7f;

            gun.DefaultModule.cooldownTime = .4f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.SetBaseMaxAmmo(50);

            gun.quality = PickupObject.ItemQuality.B;
            //gun.carryPixelOffset = new IntVector2(7, 0);
            gun.barrelOffset.transform.position = new Vector3(43 / 16f, 17 / 16f, 0 / 16f);

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0].projectile);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.speed *= 2f;
            projectile.baseData.damage = 60f;
            projectile.AppliesKnockbackToPlayer = true;
            projectile.PlayerKnockbackForce = 15;
            PierceProjModifier poke = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            poke.penetratesBreakables = true;
            poke.penetration = 6;

            ImprovedAfterImage trail = projectile.gameObject.GetOrAddComponent<ImprovedAfterImage>();

            trail.shadowLifetime = .2f;
            trail.shadowTimeDelay = .001f;
            trail.dashColor = ExtendedColours.vibrantOrange;
            trail.spawnShadows = true;

            projectile.SetProjectileSpriteRight("BIG_projectile_001", 23, 16, false, tk2dBaseSprite.Anchor.MiddleCenter, 23, 16);
            projectile.shouldFlipVertically = true;
            projectile.shouldRotate = true;
            projectile.AdditionalScaleMultiplier *= .75f;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "kirkcannon";
            gun.muzzleFlashEffects = ((Gun)PickupObjectDatabase.GetById(122)).muzzleFlashEffects;


            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .5f,
                
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
            };
            gun.PreventNormalFireAudio = true;

           

            ETGMod.Databases.Items.Add(gun, null, "ANY");
            MiscToolMethods.TrimAllGunSprites(gun);


            ID = gun.PickupObjectId;
        }
        public static int ID;
        System.Random rng = new System.Random();
        public override void PostProcessProjectile(Projectile projectile)
        {
            meleeCooldown = 0;
            LastKnownOwner.gameObject.GetOrAddComponent<PlayerPauseReloadController>();
            AkSoundEngine.PostEvent("Play_BOSS_tank_bullet_01", base.gameObject);
            DoSafeExplosion(gun.barrelOffset.transform.position);
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
            damageRadius = 4f,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 15f,
            doDestroyProjectiles = true,
            doForce = true,
            debrisForce = 0f,
            preventPlayerForce = true,
            explosionDelay = 0f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = true,
            breakSecretWalls = false,
            secretWallsRadius = 3,
            force = 2,
            forceUseThisRadius = true,

        };

        public override void OnFinishAttack(PlayerController player, Gun gun)
        {

        }
        private bool HasReloaded;
        public bool toggle = false;
        public float meleeCooldown = 0;
        public static PlayerController LastKnownOwner;
        public bool soundone = false;
        public bool soundtwo = false;
        public override void Update()
        {
            if (this.gun.CurrentOwner)
            {
                LastKnownOwner = this.gun.CurrentOwner as PlayerController;
                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;

                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                    soundone = false;
                    soundtwo = false;
                }
    
                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer((this.gun.CurrentOwner as PlayerController).PlayerIDX);
                if (gun.IsReloading && instanceForPlayer.ActiveActions.ShootAction && meleeCooldown <= 0 && gun.spriteAnimator.CurrentFrame > 3)
                {
                    StartCoroutine(HammerSlappa());
                }
                if (gun.IsReloading)
                {
                    if(gun.spriteAnimator.CurrentFrame == 3)
                    {
                        if(soundone == false) 
                        {
                            soundone = true;
                            AkSoundEngine.PostEvent("Play_OBJ_anvil_strike_01", base.gameObject);
                        }
                    }

                    if (gun.spriteAnimator.CurrentFrame == 8)
                    {
                        if (soundtwo == false)
                        {
                            soundtwo = true;
                            AkSoundEngine.PostEvent("Play_WPN_duelingpistol_reload_01", base.gameObject);
                        }
                    }
                }
                else
                {
                    soundone = false;
                    soundtwo = false;
                }


            }

            base.Update();
        }
        public static bool PauseReload = false;
        private IEnumerator HammerSlappa()
        {
            meleeCooldown = 0f;
            gun.spriteAnimator.Play("Whizzbanger_critical_fire");
            Projectile proj = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, (this.gun.CurrentOwner.CenterPosition), (this.gun.CurrentOwner as PlayerController), this.gun);
            ProjectileSlashingBehaviour stabby = proj.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            stabby.SlashDamage = 15;
            stabby.SlashDimensions = 60;
            stabby.slashKnockback = 80;
            stabby.SlashRange = 4f;
            stabby.SlashDamageUsesBaseProjectileDamage = false;
            stabby.StunApplyChance = 1;
            stabby.AppliesStun = true;
            stabby.StunTime = .75f;
            stabby.OnSlashHitEnemy += Stabby_OnSlashHitEnemy;
            stabby.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
            meleeCooldown = .7f;

            PlayerController player = (this.gun.CurrentOwner as PlayerController);
            player.gameObject.GetOrAddComponent<PlayerPauseReloadController>().PauseReloadForDuration(this.gun, meleeCooldown - .1f);
            if (player.PlayerHasActiveSynergy("Think Fast!"))
            {
                MiscToolMethods.SpawnProjAtPosi((PickupObjectDatabase.GetById(91) as Gun).DefaultModule.finalProjectile, gun.barrelOffset.transform.position, player, gun);
                meleeCooldown = .9f;
            }
            while (meleeCooldown >= 0 && LastKnownOwner == this.gun.CurrentOwner)
            {   

                if(gun.spriteAnimator.CurrentClip.name != gun.criticalFireAnimation)
                {
                    gun.spriteAnimator.PlayForDuration("Whizzbanger_empty",4);
                }
                meleeCooldown -= BraveTime.DeltaTime;
                yield return new WaitForEndOfFrame();
                yield return null;
            }

            

            gun.spriteAnimator.Play(spriteAnimator.GetClipByName("Whizzbanger_reload"),1,5);
        }

        private void Stabby_OnSlashHitEnemy(PlayerController player, AIActor hitEnemy, Vector2 forceDirection)
        {
            meleeCooldown += .25f;
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