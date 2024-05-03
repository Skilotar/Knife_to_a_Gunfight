
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;
//using ItemAPI;
using Gungeon;
using GungeonAPI;


namespace Knives
{
    public class Luna : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Luna 1", "PlasmaBall");
            Game.Items.Rename("outdated_gun_mods:luna_1", "ski:luna_1");
            gun.gameObject.AddComponent<Luna>();
            gun.SetShortDescription("Power Of Science!");
            gun.SetLongDescription("A prototype satellite launcher, The core functionality remains but the propulsion was never enough to reach orbit.\n\n\n - Knife_to_a_gunfight");

            gun.SetupSprite(null, "PlasmaBall_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 10);

            gun.SetAnimationFPS(gun.reloadAnimation, 10);

            
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(20) as Gun, true, false);
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(20) as Gun).gunSwitchGroup;

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).loopStart = 2;

            gun.muzzleFlashEffects.type = VFXPoolType.None;
            ProjectileModule projectileModule = gun.DefaultModule;

            projectileModule.ammoCost = 1;
            projectileModule.shootStyle = ProjectileModule.ShootStyle.Beam;
            projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            projectileModule.cooldownTime = .10f;
            projectileModule.angleVariance = 0f;
            projectileModule.numberOfShotsInClip = 5;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            projectileModule.projectiles[0] = projectile;
            projectile.baseData.damage *= .40f;
            projectile.baseData.range = 20f;
            
            projectile.baseData.force = 0;
            projectile.AdditionalScaleMultiplier = .2f;

            projectile.gameObject.GetOrAddComponent<LunaProjCollide>();

            Alexandria.ItemAPI.FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);

            gun.DefaultModule.projectiles[0] = projectile;
            gun.carryPixelOffset = new IntVector2(3, 0);
            bool flag = projectileModule != gun.DefaultModule;

            BasicBeamController beam = projectile.GetComponentInChildren<BasicBeamController>();
            if (!beam.IsReflectedBeam)
            {
                beam.reflections = 0;
            }
            beam.usesTelegraph = false;

            beam.AdjustPlayerBeamTint(Color.black, 0);
            beam.usesChargeDelay = true;
            beam.chargeDelay = .25f;
            beam.penetration = 0;
            

            gun.gunClass = GunClass.BEAM;
            gun.gunHandedness = GunHandedness.OneHanded;
            gun.reloadTime = 1f;
            gun.SetBaseMaxAmmo(400);
            gun.quality = PickupObject.ItemQuality.B;
            gun.barrelOffset.transform.position = new Vector3(.67f, .5f, 0);

            ETGMod.Databases.Items.Add(gun, null, "ANY");


            Projectile OrbProjectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(15) as Gun).DefaultModule.projectiles[0].projectile);
            OrbProjectile.gameObject.SetActive(false);
            Alexandria.ItemAPI.FakePrefab.MarkAsFakePrefab(OrbProjectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(OrbProjectile);
            OrbProjectile.baseData.damage = 5f;
            OrbProjectile.baseData.range *= 1f;
            OrbProjectile.baseData.speed *= 1.5f;

            BounceProjModifier bnc = OrbProjectile.gameObject.GetOrAddComponent<BounceProjModifier>();
            bnc.numberOfBounces = 4;
            PierceProjModifier stab = OrbProjectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            stab.penetration = 99;
            SlowingBulletsEffect slow = OrbProjectile.gameObject.GetOrAddComponent<SlowingBulletsEffect>();
            slow.ignoresizescale = true;
            slow.LifeSpan = 20;
            slow.Setmult = .85f;
            ProjToggleBeamMod pew = OrbProjectile.gameObject.GetOrAddComponent<ProjToggleBeamMod>();

            //HitboxMonitor.DisplayHitbox(OrbProjectile.specRigidbody);
            OrbProjectile.specRigidbody.PixelColliders.Add(new PixelCollider
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Circle,
                CollisionLayer = CollisionLayer.BeamBlocker,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = -3,
                ManualOffsetY = -2,
                ManualWidth = 0,
                ManualHeight = 0,
                ManualDiameter = 12,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
           
            
            

            OrbProjectile.transform.parent = gun.barrelOffset;
            OrbProjectile.SetProjectileSpriteRight("PlasmaBall", 15, 15, false, tk2dBaseSprite.Anchor.MiddleCenter, 13, 13);

            orb = OrbProjectile;

            ID = gun.PickupObjectId;
        }
        public static int ID;
        public static Projectile orb;
        public bool statchanged = true;

       

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            if (HasShot)
            {
                if (this.Player.HasPassiveItem(241) && statchanged == false)
                {
                    statchanged = true;
                    gun.AddCurrentGunStatModifier(PlayerStats.StatType.Accuracy, -.9f,StatModifier.ModifyMethod.ADDITIVE);
                }
                else
                {
                    gun.RemoveCurrentGunStatModifier(PlayerStats.StatType.Accuracy);
                    statchanged = false;
                }
                HasShot = false;

                if (M_Borb != null) M_Borb.DieInAir();
                gun.LoseAmmo(1);
                gun.ClipShotsRemaining--;
                M_Borb = MiscToolMethods.SpawnProjAtPosi(orb, this.gun.barrelOffset.transform.position, this.gun.CurrentOwner as PlayerController, this.gun.CurrentAngle);
            }
        }



        private bool HasReloaded;
        private bool HasShot = true;
        private bool limiter;

        //This block of code allows us to change the reload sounds.
        protected override void Update()
        {
            if (gun.CurrentOwner)
            {


                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasShot = true;
                    this.HasReloaded = true;
                }
                PlayerController player = (PlayerController)this.gun.CurrentOwner;

                if (gun.IsFiring == false)
                {
                    LunaProjCollide.Toggle = false;
                }
                if (HasShot == false && gun.IsReloading != true)
                {
                    tk2dSpriteAnimationClip clip = gun.spriteAnimator.GetClipByName("PlasmaBall_fire");
                    gun.spriteAnimator.Play(clip, 0, 10, true);
                }
                
            }
        }



        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                if (M_Borb != null) M_Borb.DieInAir();
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_plasmacell_reload_01", base.gameObject);
            }
        }

        public static Projectile M_Borb;
    }
}
