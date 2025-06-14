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
    class NewBlackStabbith : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Black Stabbith", "black_stabbith");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:black_stabbith", "ski:black_stabbith");
            gun.gameObject.AddComponent<NewBlackStabbith>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Judge Jury And Executioner");
            gun.SetLongDescription("Reload to burst knives.\n\n" +
                "This blade of blades was wielded by an obsessive executioner.\n" +
                "Embued with a forbidden dragun magic the shards of this blade chase down and cripple convicted prey." +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "black_stabbith_idle_001", 8);
            gun.SetAnimationFPS(gun.chargeAnimation, 4);
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(59) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 2;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 1.5f;
            gun.DefaultModule.numberOfShotsInClip = 10;
            gun.DefaultModule.cooldownTime = 1f;
            gun.SetBaseMaxAmmo(80);
            gun.quality = PickupObject.ItemQuality.B;
            //gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, -1f,StatModifier.ModifyMethod.ADDITIVE);
            gun.AddCurrentGunStatModifier(PlayerStats.StatType.Curse, 2f, StatModifier.ModifyMethod.ADDITIVE);


            //swing
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(39/16f, 16/16f, 0f);
            gun.carryPixelOffset = new IntVector2(7, 0);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 35f;
            projectile.baseData.speed = 3f;
            projectile.baseData.range = 3f;
            projectile.baseData.force = 5;
            
            projectile.PlayerKnockbackForce = -35;
            ProjectileSlashingBehaviour stabby = projectile.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            stabby.SlashVFX = ((Gun)ETGMod.Databases.Items["wonderboy"]).muzzleFlashEffects;
            stabby.soundToPlay = "Play_ENM_chainknight_swing_01";
            stabby.SlashRange = 4;
            stabby.SlashDimensions = 90;
            stabby.slashKnockback = 20;
            stabby.SlashDamageUsesBaseProjectileDamage = true;
            stabby.InteractMode = SlashDoer.ProjInteractMode.DESTROY;

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .6f,
                AmmoCost = 0,
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
            };
            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            MiscToolMethods.TrimAllGunSprites(gun);
            ID = gun.PickupObjectId;
        }


        public static int ID;
        public override void OnPickedUpByPlayer(PlayerController player)
        {

            base.OnPickedUpByPlayer(player);
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            StartCoroutine(slightDelay());
            base.PostProcessProjectile(projectile);
        }
       
        //public Vector2 point;
        //public float Angle;
        private IEnumerator slightDelay()
        {
            yield return new WaitForSeconds(.10f);
            throwKnives();
            //DoSafeExplosion(gun.barrelOffset.transform.TransformPoint(1.5f, 0, 0));
            AkSoundEngine.PostEvent("Play_OBJ_crystal_shatter_02", base.gameObject);
            AkSoundEngine.PostEvent("Play_OBJ_crystal_shatter_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_BOSS_dragun_blade_01", base.gameObject);
        }

       
        public List<Projectile> knownKnives = new List<Projectile>();
        private void throwKnives()
        {
            float rotate = -10;
            Projectile projectile = ((Gun)ETGMod.Databases.Items[377]).DefaultModule.projectiles[0];
            for (int i = 1; i <= 5; i++)
            {
                Projectile Knife = MiscToolMethods.SpawnProjAtPosi(projectile, gun.barrelOffset.transform.TransformPoint(1.5f, 0, 0), (this.gun.CurrentOwner as PlayerController), gun, rotate, 1, true);
                Knife.baseData.speed *= 0;
                Knife.baseData.damage = 0;
                Knife.specRigidbody.enabled = false;
                knownKnives.Add(Knife);
                rotate += 5;
            }

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
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                player.StartCoroutine(FireKnives());
            }
        }

        private IEnumerator FireKnives()
        {

            AkSoundEngine.PostEvent("Play_DragunCommunion_001", base.gameObject);
            for (int i = knownKnives.Count - 1; 0 <= i; i--)
            {

                Projectile proj = knownKnives[i];
                if (proj != null)
                {
                    proj.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUberPhantom");
                    proj.sprite.usesOverrideMaterial = true;

                }
            }

            yield return new WaitForSeconds(.5f);

            for (int i = knownKnives.Count - 1; 0 <= i; i--)
            {

                Projectile proj = knownKnives[i];
                if (proj != null)
                {
                    GlobalSparksDoer.DoRadialParticleBurst(6, proj.LastPosition, proj.LastPosition, 360, 10, 5, null, null, null, GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);

                    ParticleSpewer bleh = proj.gameObject.GetOrAddComponent<ParticleSpewer>();
                    bleh.particlesPerSecond = 12;
                    bleh.particleType = GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE;
                    proj.specRigidbody.enabled = true;
                    proj.baseData.speed = 13;
                    proj.baseData.damage = 7f;
                    proj.Update();
                    proj.UpdateSpeed();
                    proj.speedEffect = StaticStatusEffects.tripleCrossbowSlowEffect;
                    proj.AppliesSpeedModifier = true;
                    proj.SpeedApplyChance = .5f;
                    HomingModifier home = proj.gameObject.GetOrAddComponent<HomingModifier>();
                    home.HomingRadius = 6;
                    home.AngularVelocity = 90;

                    ImprovedAfterImage trail = proj.gameObject.GetOrAddComponent<ImprovedAfterImage>();
                    trail.shadowLifetime = .3f;
                    trail.shadowTimeDelay = .0001f;
                    trail.dashColor = new Color(.47f, .30f, .37f);
                    trail.spawnShadows = true;
                }
            }

            knownKnives.Clear();
        }
    }
}


