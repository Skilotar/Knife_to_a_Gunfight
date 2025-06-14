using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using System.Runtime.InteropServices;
using System.Text;
using Brave;
using Brave.BulletScript;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using UnityEngine;
using System.Reflection;

namespace Knives
{
    class BaikenBlade : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Urami", "Baiken");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:urami", "ski:urami");
            gun.gameObject.AddComponent<BaikenBlade>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Ensnared By Revenge");
            gun.SetLongDescription("Reload to throw a tethering bell. \n\n" +
                "Once wielded by a ronin with one arm, this katana has been bloodied through years of combat. \n\n" +
                "Enemies are most vulnerable to a combo of attacks when you can keep them close." +

                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "Baiken_idle_001", 8);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 10);
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);

            gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, 1f, StatModifier.ModifyMethod.ADDITIVE);

            gun.muzzleFlashEffects = null;
            gun.DefaultModule.ammoCost = 0;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = int.MaxValue;
            gun.DefaultModule.cooldownTime = .55f;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Bell", "Knives/Resources/Bell_clipfull", "Knives/Resources/Bell_clipempty");
            gun.SetBaseMaxAmmo(100);
            gun.CanReloadNoMatterAmmo = true;
            gun.quality = PickupObject.ItemQuality.B;
            gun.carryPixelOffset = new IntVector2(2, -2);


            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(32 / 16f, 35 / 16f, 0f);

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 17f;
            projectile.baseData.speed = 10f;
            projectile.baseData.range = 3f;
            projectile.baseData.force = 80;
            projectile.BossDamageMultiplier *= 1.5f;
            
            gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1, StatModifier.ModifyMethod.ADDITIVE);

            Projectile Bell = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

            Bell.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(Bell.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(Bell);

            Bell.baseData.damage = 7f;
            Bell.baseData.speed = 17f;
            Bell.baseData.range = 8f;
            Bell.baseData.force = -5;
            Bell.shouldRotate = true;
            Bell.angularVelocity = -120;
            Bell.AdditionalScaleMultiplier *= .75f;
            Bell.pierceMinorBreakables = true;
            BounceProjModifier bnc = Bell.gameObject.GetOrAddComponent<BounceProjModifier>();
            bnc.numberOfBounces = 1;
            Bell.SetProjectileSpriteRight("Bell", 12, 12, false, tk2dBaseSprite.Anchor.MiddleCenter, 10, 10);
            
            BellProj = Bell;

            projectile.transform.parent = gun.barrelOffset;
            MiscToolMethods.TrimAllGunSprites(gun);
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;
        }
        public static int ID;

        public static Projectile BellProj;

        public override void OnPostFired(PlayerController player, Gun gun)
        {

            gun.PreventNormalFireAudio = true;

        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);

            
            ComboManager(projectile);

            base.PostProcessProjectile(projectile);
            

        }

        public int Animswap = 1;
        public void ComboManager(Projectile projectile)
        {
            if (ComboNumber == 1) //H-slash
            {
                gun.DefaultModule.cooldownTime = .55f;
                StartCoroutine(comboCountdown());
                
                ComboNumber = 2;
                extend = true;

                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 20;
                slasher.SlashDamageUsesBaseProjectileDamage = true;
                
                slasher.SlashRange = 4.6f;
                slasher.playerKnockback = 0;
                slasher.slashKnockback = 80f;
                slasher.soundToPlay = "Play_obj_katana_slash_01";
                slasher.InteractMode = SlashDoer.ProjInteractMode.IGNORE;
                return;
            }
            if (ComboNumber == 2) //V slash
            {
                gun.spriteAnimator.StopAndResetFrameToDefault();
                gun.spriteAnimator.Play("Baiken_critical_fire");
                
                extend = true;
                ComboNumber = 3;

                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 90;
                slasher.SlashDamageUsesBaseProjectileDamage = true;
                
                slasher.SlashRange = 3.6f;
                slasher.slashKnockback = 70f;
                slasher.playerKnockback = 0;
                slasher.soundToPlay = "Play_obj_katana_slash_01";
                slasher.InteractMode = SlashDoer.ProjInteractMode.IGNORE;
                return;
            }
            if (ComboNumber == 3) // Kick
            {
                gun.DefaultModule.cooldownTime = .6f;
                ComboNumber = 1;
                gun.spriteAnimator.StopAndResetFrameToDefault();
                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 90;
                slasher.SlashDamageUsesBaseProjectileDamage = false;
                slasher.SlashDamage = 75;
                slasher.SlashRange = 3f;
                slasher.slashKnockback = 150f;
                slasher.playerKnockback = 0;
                
                slasher.SlashVFX = ((Gun)PickupObjectDatabase.GetById(335)).muzzleFlashEffects;
                slasher.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
                slasher.OnSlashHitEnemy += Slasher_OnSlashHitEnemy;

                FieldInfo leEnabler = typeof(PlayerController).GetField("m_handlingQueuedAnimation", BindingFlags.Instance | BindingFlags.NonPublic);
                leEnabler.SetValue(player, true);
                string text = GetAnimationName(player);
                string str = (!(player.CurrentGun == null) || player.ForceHandless) ? "_hand" : "_twohands";
                string text2 = (!player.UseArmorlessAnim) ? string.Empty : "_armorless";
                bool renderBodyhand = !player.ForceHandless && player.CurrentSecondaryGun == null && (player.CurrentGun == null || player.CurrentGun.Handedness != GunHandedness.TwoHanded);
                if (renderBodyhand && player.spriteAnimator.GetClipByName(text + str + text2) != null)
                {
                    player.spriteAnimator.Play(text + str + text2);
                }
                else if (player.spriteAnimator.GetClipByName(text + text2) != null)
                {
                    player.spriteAnimator.Play(text + text2);
                }
                return;
            }


        }

        private void Slasher_OnSlashHitEnemy(PlayerController player, AIActor hitEnemy, Vector2 forceDirection)
        {
            if(hitEnemy.aiActor != null)
            {
                if (hitEnemy.healthHaver.IsAlive)
                {
                    if(hitEnemy.gameObject.GetComponent<TetherEnemyToSpot>() != null)
                    {
                        hitEnemy.gameObject.GetComponent<TetherEnemyToSpot>().DoBreak();
                    }
                }
            }
        }

        public static string GetAnimationName(PlayerController player)
        {

            if (player.CurrentGun.CurrentAngle <= 45 && player.CurrentGun.CurrentAngle >= -45)// right
            {
                return "tablekick_right";
            }
            if (player.CurrentGun.CurrentAngle > 45 && player.CurrentGun.CurrentAngle <= 135)// up
            {
                return "tablekick_up";
            }
            if (player.CurrentGun.CurrentAngle > 135 || player.CurrentGun.CurrentAngle <= -135)// left
            {

                return "tablekick_right";
            }
            if (player.CurrentGun.CurrentAngle < -45 && player.CurrentGun.CurrentAngle >= -135)// down
            {
                return "tablekick_down";
            }
            return "error";

        }

        public int ComboNumber = 1;
        public bool extend = false;
        private IEnumerator comboCountdown()
        {
            Animswap = 1;
            yield return new WaitForSeconds(1.3f);
            if (ComboNumber == 1)
            {
                Animswap = 1;
            }
            if (ComboNumber == 2)
            {
                if (extend)
                {
                    StartCoroutine(comboCountdown());
                    extend = false;
                }
                else
                {
                    Animswap = 1;
                    ComboNumber = 1;
                }

            }
            if (ComboNumber == 3)
            {
                Animswap = 1;
                ComboNumber = 1;
                
                
            }

        }

        private bool HasReloaded;


        public override void Update()
        {

            if (gun.CurrentOwner)
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
                if(internalCooldown > 0)
                {
                    internalCooldown -= BraveTime.DeltaTime;
                }
                if (m_bell != null)
                {
                    if (m_laser1 == null)
                    {
                        m_laser1 = RenderLaserSight(gun.barrelOffset.transform.position, 100, 1, player.CurrentGun.CurrentAngle, true, ExtendedColours.maroon);
                    }
                    else
                    {
                       
                        m_laser1.transform.rotation = Quaternion.Euler(0, 0, (m_bell.specRigidbody.UnitCenter - (player.CenterPosition) ).ToAngle());
                        tk2dTiledSprite sprite = m_laser1.GetComponent<tk2dTiledSprite>();
                        sprite.dimensions = new Vector2(Vector2.Distance(m_bell.specRigidbody.UnitCenter, player.CenterPosition) * 16, 1);
                        sprite.renderer.enabled = true;
                        m_laser1.transform.position = (player.CenterPosition + new Vector2(0, -1 / 16f));
                    }
                }
                else
                {
                    if (m_laser1 != null)
                    {
                        tk2dTiledSprite sprite = m_laser1.GetComponent<tk2dTiledSprite>();
                        sprite.renderer.enabled = false;
                    }
                }

                if (setup == false)
                {
                    //PassiveItem.IncrementFlag((PlayerController)this.gun.CurrentOwner, typeof(LiveAmmoItem));
                    player.inventory.OnGunChanged += Inventory_OnGunChanged;
                    KnownOwner = player;
                    setup = true;
                }
                
            }
        }
        public PlayerController KnownOwner;
        private void Inventory_OnGunChanged(Gun previous, Gun current, Gun previousSecondary, Gun currentSecondary, bool newGun)
        {
            if(previous == this.gun)
            {
                if (m_laser1 != null)
                {
                    
                    tk2dTiledSprite sprite = m_laser1.GetComponent<tk2dTiledSprite>();
                    sprite.renderer.enabled = false;
                    
                    KnownOwner.inventory.OnGunChanged -= Inventory_OnGunChanged;
                    setup = false;
                }
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (m_bell == null && internalCooldown <= 0 && gun.ammo > 0)
            {
                AkSoundEngine.PostEvent("Play_ENM_chainknight_swing_01", base.gameObject);
                gun.spriteAnimator.StopAndResetFrameToDefault();
                gun.spriteAnimator.Play("Baiken_reload");
                StartCoroutine(WaitforBell());
                internalCooldown = 1f;
                gun.LoseAmmo(1);
            }

        }
        public Projectile m_bell;
        private float internalCooldown = 0;
        public UnityEngine.GameObject m_laser1;
        private bool setup = false;

        private IEnumerator WaitforBell()
        {
            while(gun.spriteAnimator.CurrentFrame != 2)
            {
                yield return null;
            }
            if(m_bell == null)
            {
                m_bell = MiscToolMethods.SpawnProjAtPosi(BellProj, gun.barrelOffset.transform.position, Player, gun,0,1,false);
                TetherProjMod rope = m_bell.gameObject.GetOrAddComponent<TetherProjMod>();
                rope.tetherBackToPlayer = true;
                rope.minorslowwalkRadius = 6;
                rope.majorslowwalkRadius = 8;
                rope.minorKB = 1f;
                rope.majorKB = 2;
                rope.StopbreakDistance = 3f;
                rope.color = ExtendedColours.maroon;
            }
            
        }
        public static GameObject RenderLaserSight(Vector2 position, float length, float width, float angle, bool alterColour = false, Color? colour = null)
        {
            GameObject laserSightPrefab = LoadHelper.LoadAssetFromAnywhere("assets/resourcesbundle/global vfx/vfx_lasersight.prefab") as GameObject;

            GameObject gameObject = SpawnManager.SpawnVFX(laserSightPrefab, position, Quaternion.Euler(0, 0, angle));

            tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
            float newWidth = 1f;
            if (width != -1) newWidth = width;
            component2.dimensions = new Vector2(length, newWidth);
            if (alterColour && colour != null)
            {
                component2.usesOverrideMaterial = true;
                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                component2.sprite.renderer.material.SetColor("_OverrideColor", (Color)colour);
                component2.sprite.renderer.material.SetColor("_EmissiveColor", (Color)colour);
                component2.sprite.renderer.material.SetFloat("_EmissivePower", 1);
                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", .5f);
            }
            return gameObject;
        }
    }
}