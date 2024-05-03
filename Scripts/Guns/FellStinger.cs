using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections.Generic;
using MultiplayerBasicExample;
using System.Reflection;

namespace Knives
{
    class Fell : AdvancedGunBehaviour
    {

        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Fell Stinger", "Fell");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:fell_stinger", "ski:fell_stinger");
            gun.gameObject.AddComponent<Fell>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Fatality!");
            gun.SetLongDescription("Killing an enemy increases damage by 1, this boost resets each combat.\n\n" +
                "A brave bee will sacrifice its life to protect the queen, but a cunning bee uses its foes weaknesses as an advantage." +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "Fell_idle_001", 30);
            gun.SetAnimationFPS(gun.shootAnimation, 20);
            gun.SetAnimationFPS(gun.reloadAnimation, 4);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 5);



            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 4;
            gun.gunClass = GunClass.FULLAUTO;
            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 10;
            gun.DefaultModule.cooldownTime = .2f;
            gun.SetBaseMaxAmmo(450);
            gun.quality = PickupObject.ItemQuality.B;
            gun.carryPixelOffset = new IntVector2(5, 0);

            gun.muzzleFlashEffects = ((Gun)PickupObjectDatabase.GetById(142)).muzzleFlashEffects;
            gun.PreventNormalFireAudio = true;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(1.75f, .42f, 0f);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 5f;
            projectile.baseData.speed *= 2f;
            projectile.baseData.range *= 1.5f;
            projectile.shouldRotate = true;

            projectile.SetProjectileSpriteRight("Stinger", 19, 5, false, tk2dBaseSprite.Anchor.LowerLeft, 19, 5);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Stingers", "Knives/Resources/Fell_clipfull", "Knives/Resources/Fell_clipempty");

            ImprovedAfterImage trail = projectile.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail.shadowLifetime = .3f;
            trail.shadowTimeDelay = .0001f;
            trail.dashColor = new Color(1.0f, .69f, .0f); // yes that's the actual RGB value, amazing
            trail.spawnShadows = true;


            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");


            ID = gun.PickupObjectId;


        }

        public static int ID;


        public override void OnPostFired(PlayerController player, Gun gun)
        {
           
            AkSoundEngine.PostEvent("Play_WPN_crossbow_shot_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_WPN_bees_impact_01", base.gameObject);


        }



        public override void PostProcessProjectile(Projectile projectile) 
        {
            if(projectile != null)
            {
                projectile.baseData.damage += Stacks;
                projectile.OnHitEnemy += this.OnHitEnemy;
            }


            base.PostProcessProjectile(projectile);
        }
        public int Stacks = 0;
        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            bool venoshocked = false;
            if(arg2.aiActor != null)
            {
                if ((arg1.Owner as PlayerController).PlayerHasActiveSynergy("VenoShock"))
                {
                    if (CheckIfPoisoned(arg2))
                    {
                        if (arg2.healthHaver.GetCurrentHealth() <= 15)
                        {
                            venoshocked = true;
                        }
                        arg2.healthHaver.ApplyDamage(15, Vector2.zero, "Venoshock");
                        AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
                        AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
                    }
                }
                if (arg3 == true || venoshocked)
                {
                    Stacks++;
                    Stacks++;
                    AkSoundEngine.PostEvent("Play_Stat_Up", base.gameObject);
                    if ((arg1.Owner as PlayerController).PlayerHasActiveSynergy("Swarmageddon"))
                    {
                        float temp = 1 / (arg1.Owner as PlayerController).healthHaver.GetCurrentHealthPercentage();
                        int loops = (int)temp + 2;
                        for (int i = 0; i <= loops; i++)
                        {

                            GameObject gameObject = SpawnManager.SpawnProjectile(((Gun)PickupObjectDatabase.GetById(14)).DefaultModule.projectiles[0].projectile.gameObject, arg2.UnitCenter, Quaternion.Euler(0f, 0f, (i * 70)), true);
                            Projectile component = gameObject.GetComponent<Projectile>();
                            bool flag2 = component != null;
                            if (flag2)
                            {
                                component.Owner = (arg1.Owner as PlayerController);
                                component.Shooter = (arg1.Owner as PlayerController).specRigidbody;
                                HomingModifier home = component.gameObject.GetOrAddComponent<HomingModifier>();
                                home.HomingRadius = 40f;
                                home.AngularVelocity = 1000f;
                                PierceProjModifier stab = component.gameObject.GetOrAddComponent<PierceProjModifier>();
                                stab.penetration = 3;

                            }
                        }
                    }
                }
            }
           
        }

        private bool CheckIfPoisoned(SpeculativeRigidbody arg2)
        {
            AIActor boi =  arg2.aiActor;
            
            GameActorEffect effect = boi.GetEffect("poison"); // uses effect Identifier    // Hey Future me. DONT use the resistnaceType on the get effect method. it don't work fam.
            if (effect != null)
            {
                //ETGModConsole.Log(effect.effectIdentifier.ToString());
                return true;
            }
            else
            {
                //ETGModConsole.Log("Null effect");
                return false;
            }
            

            
        }

        public void OnEnteredCombat()
        {
            if(Stacks > 0)
            {
                Stacks = 0;
                AkSoundEngine.PostEvent("Play_Stat_Down", base.gameObject);
            }
            
        }

        public override void OnReload(PlayerController player, Gun gun)
        {
            base.OnReload(player, gun);
            this.HasReloaded = false;
            
            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
 
            AkSoundEngine.PostEvent("Play_WPN_m1911_reload_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_WPN_bees_impact_01", base.gameObject);
        }



        public override void OnPostDrop(GameActor owner)
        {
            PlayerController player = (PlayerController)owner;
           
        }

        private bool setup = false;
        private bool HasReloaded;
        public PlayerController KnownLastOwner;
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

                if (!setup)
                {
                    (gun.CurrentOwner as PlayerController).OnEnteredCombat += this.OnEnteredCombat;
                    KnownLastOwner = (gun.CurrentOwner as PlayerController);
                    setup = true;
                }
              
            }
            else
            {
                if (setup)
                {
                    KnownLastOwner.OnEnteredCombat -= this.OnEnteredCombat;
                    setup = false;
                }
            }

        }

    }
}
