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
    class SIND : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Shot In The Dark", "SIND");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:shot_in_the_dark", "ski:shot_in_the_dark");
            gun.gameObject.AddComponent<SIND>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Blind Fire");
            gun.SetLongDescription("A pocket sized hardlight pistol that conserves space by drawing light from its suroundings as opposed to generating light itself." +
                "\n- Knife_to_a_Gunfight");
            
            gun.SetupSprite(null, "SIND_idle_001", 4);
            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 24);
            gun.SetAnimationFPS(gun.chargeAnimation, 6);


            gun.AddProjectileModuleFrom((PickupObjectDatabase.GetById(180) as Gun), true, true);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 8;

            Gun gun2 = (PickupObjectDatabase.GetById(180) as Gun);
            gun.muzzleFlashEffects = gun2.muzzleFlashEffects;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.PISTOL;
            gun.gunHandedness = GunHandedness.OneHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 0.0f;
            gun.DefaultModule.cooldownTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.SetBaseMaxAmmo(80);

            gun.quality = PickupObject.ItemQuality.B;




            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0].projectile);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 70f;
            projectile.BossDamageMultiplier = 2f;
           
            projectile.baseData.speed *= 1f;
            projectile.BossDamageMultiplier = 2f;
            projectile.HasDefaultTint = true;
            projectile.DefaultTintColor = UnityEngine.Color.cyan;


            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 1.2f,
                
            };
          
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
              
            };
            gun.PreventNormalFireAudio = true;

            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;

        }
        public static int ID;
        public override void PostProcessProjectile(Projectile projectile)
        {
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_WPN_grasshopper_shot_01", base.gameObject);
            PlayerController player = this.gun.CurrentOwner as PlayerController;
            if (player.PlayerHasActiveSynergy("Rainbow In The Dark"))
            {
                BeamBulletsBehaviour beamy = projectile.gameObject.GetOrAddComponent<BeamBulletsBehaviour>();
                beamy.beamToFire = ((Gun)PickupObjectDatabase.GetById(100)).DefaultModule.projectiles[0].projectile;
                beamy.firetype = BeamBulletsBehaviour.FireType.TRAIL_V;
                projectile.baseData.range *= 2;
                projectile.ResetDistance();
            }
        }

        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
           
        }
        private bool HasReloaded;
        public bool setup = true;
        public bool DarknessCalled = false;
       
        public override void  Update()
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

                if(this.gun.GetChargeFraction() > 0)
                {
                    if(DarknessCalled == false)
                    {
                        CustomDarknessHandler.shouldBeDark.SetOverride("SIND", true);
                        ETGModMainBehaviour.Instance.gameObject.GetOrAddComponent<CustomDarknessHandler>().FlashlightAngle = 0;
                        
                        DarknessCalled = true;
                    }
                    RenderSettings.ambientLight = new Color(0, 0, 0);
                    
                }
                else
                {
                    ETGModMainBehaviour.Instance.gameObject.GetOrAddComponent<CustomDarknessHandler>().FlashlightAngle = 25;
                    CustomDarknessHandler.shouldBeDark.RemoveOverride("SIND");
                    
                    DarknessCalled = false;
                    
                }

            }

            base.Update();
        }

        public override void OnReload(PlayerController player, Gun gun)
        {

            AkSoundEngine.PostEvent("Play_WPN_plasmacell_reload_01", base.gameObject);
        }
        public override void OnDropped()
        {
            ETGModMainBehaviour.Instance.gameObject.GetOrAddComponent<CustomDarknessHandler>().FlashlightAngle = 25;
            CustomDarknessHandler.shouldBeDark.RemoveOverride("SIND");
            base.OnDropped();
        }

    }
}