using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using Alexandria.ItemAPI;
using MultiplayerBasicExample;
using Dungeonator;
using System.Collections.Generic;

namespace Knives
{

    public class Static : AdvancedGunBehavior
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Galvanizer", "Static");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:galvanizer", "ski:galvanizer");
            gun.gameObject.AddComponent<Static>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Cling Sling");
            gun.SetLongDescription("Multiple changes in momentum will charge up the next shot. Carpeted floor materials will drastically increase the charge gained\n\n" +
                "" +
                "A Sp4rk brand rifle optimized to use very low levels of electrity. " +
                "This was done to optimize usage of a single battery pack. " +
                "However, due to this low voltage system they were commonly modded to accept static power as an overclock! \n\n" +
                "" +

                "\n\n\n - Knife_to_a_Gunfight");

            gun.SetupSprite(null, "Static_idle_001", 8);
            
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 4);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = .75f;
            gun.DefaultModule.cooldownTime = .3f;
            gun.DefaultModule.numberOfShotsInClip = 6;
            gun.SetBaseMaxAmmo(200);

            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;
            gun.barrelOffset.transform.localPosition = new Vector3(.5f, .2f, 0f);
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 12f;
            projectile.baseData.speed = 60;
            gun.carryPixelOffset = new IntVector2(7, 0);
            LightningProjectileComp zappy = projectile.gameObject.GetOrAddComponent<LightningProjectileComp>();

            List<string> BeamAnimPaths = new List<string>()
            {

                "Knives/Resources/BeamSprites/Sp4rk_001",
                "Knives/Resources/BeamSprites/Sp4rk_002",
                "Knives/Resources/BeamSprites/Sp4rk_003",
            };
            projectile.AddTrailToProjectile(
                 "Knives/Resources/BeamSprites/Sp4rk_001",
                new Vector2(3, 2),
                new Vector2(1, 1),
                BeamAnimPaths, 20,
                BeamAnimPaths, 20,
                -1,
                0.0001f,
                5,
                true
                );
            Gun muzzle = (Gun)PickupObjectDatabase.GetById(142);
            gun.muzzleFlashEffects = muzzle.muzzleFlashEffects;
            projectile.damageTypes = CoreDamageTypes.Electric;
            gun.gunClass = GunClass.RIFLE;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_ENM_pop_shot_01", base.gameObject);
            static_charge = 0;
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            if(static_charge < 10)
            {
                static_charge = 0;
            }
            if(static_charge >= 10 && static_charge < 20)
            {
                static_charge = 0;
                projectile.baseData.damage *= 1.1f;
                projectile.AdditionalScaleMultiplier = 1.2f;
                AkSoundEngine.PostEvent("Play_neon_001", base.gameObject);
                
            }
            if (static_charge >= 20 && static_charge < 30)
            {
                static_charge = 0;
                projectile.baseData.damage *= 1.5f;
                projectile.AdditionalScaleMultiplier = 1.3f;
                AkSoundEngine.PostEvent("Play_neon_002", base.gameObject);
               
            }
            if (static_charge >= 30 && static_charge < 40)
            {
                static_charge = 0;
                projectile.baseData.damage *= 2f;
                projectile.AdditionalScaleMultiplier = 1.4f;
                AkSoundEngine.PostEvent("Play_neon_003", base.gameObject);
                
            }
            if (static_charge >= 40)
            {
                static_charge = 0;
                projectile.baseData.damage *= 2.5f;
                projectile.AdditionalScaleMultiplier = 1.6f;
                AkSoundEngine.PostEvent("Play_neon_004", base.gameObject);
            }

            
            base.PostProcessProjectile(projectile);
        }

        private bool HasReloaded;
        public Vector2 last_recorded_direction = new Vector2(-1,1);
        public int static_charge;
        protected override void Update()
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
                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                if(gun.ClipShotsRemaining > 0)
                {
                    if (player.NonZeroLastCommandedDirection != last_recorded_direction && static_charge <= 50 && can_charge)
                    {
                        last_recorded_direction = player.NonZeroLastCommandedDirection;

                        if (static_charge < 10)
                        {
                           
                            static_charge++;
                        }
                        if(static_charge == 10)
                        {
                            AkSoundEngine.PostEvent("Play_mac_boop_02", base.gameObject);
                            AkSoundEngine.PostEvent("Play_mac_boop_02", base.gameObject);
                        }
                        if (static_charge >= 10 && static_charge < 20)
                        {
                            
                            static_charge++;
                        }
                        if (static_charge == 20)
                        {
                            AkSoundEngine.PostEvent("Play_mac_boop_03", base.gameObject);
                            AkSoundEngine.PostEvent("Play_mac_boop_03", base.gameObject);
                        }
                        if (static_charge >= 20 && static_charge < 30)
                        {
                            
                            static_charge++;
                        }
                        if (static_charge == 30)
                        {
                            AkSoundEngine.PostEvent("Play_mac_boop_05", base.gameObject);
                            AkSoundEngine.PostEvent("Play_mac_boop_05", base.gameObject);
                        }
                        if (static_charge >= 30 && static_charge < 40)
                        {
                            
                            static_charge++;
                        }
                        if (static_charge == 40)
                        {
                            AkSoundEngine.PostEvent("Play_mac_boop_High", base.gameObject);
                            AkSoundEngine.PostEvent("Play_mac_boop_High", base.gameObject);
                        }
                        if (static_charge >= 40 && static_charge < 50)
                        {
                            
                            static_charge++;
                        }
                        CellVisualData.CellFloorType floorTypeFromPosition = GameManager.Instance.Dungeon.GetFloorTypeFromPosition(player.specRigidbody.UnitCenter);
                        if (floorTypeFromPosition == CellVisualData.CellFloorType.Carpet)
                        {
                            static_charge++;
                            static_charge++;
                            GlobalSparksDoer.DoRandomParticleBurst(2, player.sprite.WorldBottomLeft, player.sprite.WorldBottomRight, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT);
                        }
                        GlobalSparksDoer.DoRandomParticleBurst(2, player.sprite.WorldBottomLeft, player.sprite.WorldBottomRight, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT);
                        StartCoroutine(statictimemanager());
                    }

                    
                }

            }

        }

        public bool can_charge = true;
        private IEnumerator statictimemanager()
        {
            can_charge = false;
            yield return new WaitForSecondsRealtime(.06f);
            can_charge = true;
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