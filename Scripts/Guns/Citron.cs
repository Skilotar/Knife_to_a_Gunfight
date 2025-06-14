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
    class Citron : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Citron", "Citron");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:citron", "ski:citron");
            gun.gameObject.AddComponent<Citron>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Orange Aid");
            gun.SetLongDescription("Fires Electic Balls\n" +
                "A strange plant from the future that is so acidic it surges with electrons. " +

                "\n\n\n - Knife_to_a_Gunfight");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "Citron_idle_001", 8);

            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);
            gun.SetAnimationFPS(gun.chargeAnimation, 6);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(59) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 4;

            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.CHARGE;
            gun.gunHandedness = GunHandedness.OneHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1f;

            gun.DefaultModule.cooldownTime = .15f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.SetBaseMaxAmmo(50);
            gun.barrelOffset.transform.localPosition = new Vector3(30/16f, 14/16f, 0f);
            gun.carryPixelOffset = new IntVector2(5, 0);
            gun.quality = PickupObject.ItemQuality.A;
            gun.gunSwitchGroup = ((Gun)PickupObjectDatabase.GetById(365)).gunSwitchGroup;
            
            gun.PreventNormalFireAudio = true;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 13f;
            projectile.baseData.speed = 10f;
            projectile.baseData.range = 4000f;
            //projectile.objectImpactEventName = "Play_WPN_energy_impact_01";

            projectile.angularVelocity = 270;
            projectile.transform.parent = gun.barrelOffset;
            PierceProjModifier stab = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            stab.penetration = 200;
            BounceProjModifier bnc = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
            bnc.numberOfBounces = 3;
            OrangeProjectileModifier tenta = projectile.gameObject.GetOrAddComponent<OrangeProjectileModifier>();


            MiscToolMethods.EasyAnimate(projectile, new List<string>() {
                "Citron_proj_001",
                "Citron_proj_002",
                "Citron_proj_003",
                "Citron_proj_004",

            }, 4, new IntVector2(22,22), 6, true, tk2dSpriteAnimationClip.WrapMode.Loop);

            Gun gun2 = (Gun)PickupObjectDatabase.GetById(365);
            gun.muzzleFlashEffects = gun2.muzzleFlashEffects;
            //gun.PreventNormalFireAudio = true;


            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .8f,
                AdditionalWwiseEvent = "Play_Charger_up"
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
            };
            main = projectile;
            MiscToolMethods.TrimAllGunSprites(gun);

            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;
        }
        public static int ID;
        public static Projectile main;


        public override void OnPostFired(PlayerController player, Gun gun)
        {

        }
        public System.Random rng = new System.Random();
        public override void PostProcessProjectile(Projectile projectile)
        {
            

            base.PostProcessProjectile(projectile);
        }




        private bool HasReloaded;
        public bool reset = true;

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

                if (gun.GetChargeFraction() > .1f)
                {
                    if (reset == false)
                    {
                        AkSoundEngine.PostEvent("Play_Charger_up", base.gameObject);
                        AkSoundEngine.PostEvent("Play_Retro_Save_002", base.gameObject);
                        reset = true;
                    }
                    
                }
                else
                {
                    if (reset == true) 
                    {
                        AkSoundEngine.PostEvent("Stop_Retro_Save_002", base.gameObject);
                        reset = false;
                    }
                }

                if((this.Owner as PlayerController).PlayerHasActiveSynergy("Citrush") && Player.HasActiveItem(485))
                {
                    BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer((this.gun.CurrentOwner as PlayerController).PlayerIDX);
                    if (gun.GetChargeFraction() == 1)
                    {
                        
                        if (!instanceForPlayer.ActiveActions.ShootAction && Time.timeScale != 0 && !instanceForPlayer.ActiveActions.MapAction.IsPressed && Player.IsDodgeRolling == false)
                        {
                            instanceForPlayer.ActiveActions.ShootAction.ClearInputState();
                            if(doing == false) StartCoroutine(Rush());

                        }
                    }

                    if((this.Owner as PlayerController).CurrentItem.PickupObjectId == 485)
                    {
                        if (instanceForPlayer.ActiveActions.UseItemAction)
                        {
                            this.gun.GainAmmo(2000);
                            
                        }
                    }
                }

            }
            else
            {
                //gun.spriteAnimator.Play("cata_charge");
            }

            base.Update();
        }
        bool doing = false;
        private IEnumerator Rush()
        {
            doing = true;
            yield return new WaitForSeconds(.1f);
            MiscToolMethods.SpawnProjAtPosi(Citron.main, gun.barrelOffset.transform.position, (this.Owner as PlayerController), this.gun, +10, 1, true);
            gun.spriteAnimator.PlayForDuration("Citron_fire", .3f, "Citron_idle");
            AkSoundEngine.PostEvent("Play_WPN_bountyhunterarm_shot_03", base.gameObject);
            gun.LoseAmmo(1);
            MiscToolMethods.SpawnProjAtPosi(Citron.main, gun.barrelOffset.transform.position, (this.Owner as PlayerController), this.gun, -10, 1, true);
            gun.spriteAnimator.PlayForDuration("Citron_fire", .3f, "Citron_idle");
            AkSoundEngine.PostEvent("Play_WPN_bountyhunterarm_shot_03", base.gameObject);
            gun.LoseAmmo(1);
            yield return new WaitForSeconds(.1f);
            doing = false;
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_screechgun_reload_01", base.gameObject);


            }
        }


    }
}

