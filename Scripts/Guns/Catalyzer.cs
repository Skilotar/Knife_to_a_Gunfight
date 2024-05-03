
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
    class Catalyzer : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Catalyzer", "Cat");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:catalyzer", "ski:catalyzer");
            gun.gameObject.AddComponent<Catalyzer>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Activation Energy Required");

            gun.SetLongDescription("This Gun's bullets are made of chemicals on the brink of a reaction.\n\n" +
                "They are brimming with terrifying potential. Only the charge shot of the gun holds enough potential to catalyze the reaction of all the cations nearby.\n Because of its explosive nature it is a great tool for boss killing. \n\n\n - Knife_to_a_Gunfight");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "Cat_idle_001", 8);
            
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 48);
            gun.SetAnimationFPS(gun.reloadAnimation, 14);
            gun.SetAnimationFPS(gun.chargeAnimation, 12);
            
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(197) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 4;

            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.EXPLOSIVE;
            gun.gunHandedness = GunHandedness.OneHanded;
            
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = .5f;
            
            gun.DefaultModule.cooldownTime = .15f;
            gun.DefaultModule.numberOfShotsInClip = 3;
            gun.SetBaseMaxAmmo(400);
            gun.barrelOffset.transform.localPosition = new Vector3(1f, .5f, 0f);
            gun.quality = PickupObject.ItemQuality.A;
            gun.encounterTrackable.EncounterGuid = "One Herring Twitch! And Kableewy!";
            gun.muzzleFlashEffects = null;
            gun.PreventNormalFireAudio = true;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 1f;
            projectile.baseData.speed = 10f;
            projectile.baseData.range = 4000f;
            projectile.SetProjectileSpriteRight("cation", 30,30, false, tk2dBaseSprite.Anchor.MiddleCenter, 30, 30);
            projectile.AdditionalScaleMultiplier = .35f;
            projectile.angularVelocity = 500;
            projectile.transform.parent = gun.barrelOffset;
            PierceProjModifier stab = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            stab.penetration = 200;

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            projectile2.baseData.damage = 20f;
            projectile2.baseData.speed = 13f;
            projectile2.baseData.range = 25f;
            projectile2.SetProjectileSpriteRight("plasma", 42, 42, false, tk2dBaseSprite.Anchor.MiddleCenter, 40, 40);
            projectile2.angularVelocity = 500;
            projectile2.AdditionalScaleMultiplier = .7f;
            
            projectile2.transform.parent = gun.barrelOffset;

            Gun gun2 = (Gun)PickupObjectDatabase.GetById(394);
            gun.muzzleFlashEffects = gun2.muzzleFlashEffects;
            gun.PreventNormalFireAudio = true;
            

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f
            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                AmmoCost = 3,
                UsedProperties = ProjectileModule.ChargeProjectileProperties.ammo | ProjectileModule.ChargeProjectileProperties.depleteAmmo,
                Projectile = projectile2,
                ChargeTime = .3f
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2
            };
            

            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        
        
        public override void OnPostFired(PlayerController player, Gun gun)
        {
           
        }
        public System.Random rng = new System.Random();
        public override void PostProcessProjectile(Projectile projectile)
        {
            int sfx = rng.Next(0, 2);
            if (projectile.GetCachedBaseDamage == 1)
            {
               
                if(sfx == 0)
                {
                    AkSoundEngine.PostEvent("Play_cat_fire_sfx_001", base.gameObject);
                   
                }
                else 
                { 

                    AkSoundEngine.PostEvent("Play_cat_fire_sfx_002", base.gameObject);
                    
                   
                }

                HandlePostProcessProjectile(projectile);
            }
            //charged
            if (projectile.GetCachedBaseDamage == 20)
            {
                AkSoundEngine.PostEvent("Play_cat_chargefire_sfx_001", base.gameObject);
              
                toggle = true;
                HandlePostProcessProjectileCharged(projectile);
            }
            
            base.PostProcessProjectile(projectile);
        }

       


        private void HandlePostProcessProjectile(Projectile targetProjectile)
        {
           
            targetProjectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(targetProjectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemy));
        }
        private void HandlePostProcessProjectileCharged(Projectile targetProjectile)
        {

            targetProjectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(targetProjectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemyCharged));
        }
        public OrbitProjectileMotionModule spincheck;

        //This part handles the attaching of a new motion modual after the projectile has landed on the enemy but the start event can be altered
        //This is the important bit!!!!!!!!!!
        //please dont use orbit group 100 it may cause issues with clashing idk  : )
        public void OnHitEnemy(Projectile proj, SpeculativeRigidbody body, bool yes)
        {
            if (body.aiActor != null)
            {
                BounceProjModifier bouncer = proj.gameObject.GetOrAddComponent<BounceProjModifier>();
                bouncer.name = "cation";

                int orbitersInGroup = OrbitProjectileMotionModule.GetOrbitersInGroup(100);
                if (orbitersInGroup >= 20)
                {
                    return;
                }
                bouncer.projectile.specRigidbody.CollideWithTileMap = false;
                bouncer.projectile.ResetDistance();

                bouncer.projectile.baseData.range = Mathf.Max(bouncer.projectile.baseData.range, 500f);
                bouncer.projectile.collidesWithEnemies = false;
                bouncer.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyBulletBlocker, CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable));
                OrbitProjectileMotionModule orbitProjectileMotionModule = new OrbitProjectileMotionModule();
                orbitProjectileMotionModule.lifespan = 4000f;
                if (bouncer.projectile.OverrideMotionModule != null && bouncer.projectile.OverrideMotionModule is HelixProjectileMotionModule)
                {
                    orbitProjectileMotionModule.StackHelix = true;
                    orbitProjectileMotionModule.ForceInvert = (bouncer.projectile.OverrideMotionModule as HelixProjectileMotionModule).ForceInvert;
                }
                orbitProjectileMotionModule.usesAlternateOrbitTarget = true;
                orbitProjectileMotionModule.alternateOrbitTarget = body;
                orbitProjectileMotionModule.MinRadius = 1;
                orbitProjectileMotionModule.MaxRadius = 1;
                spincheck = orbitProjectileMotionModule;
                bouncer.projectile.OverrideMotionModule = spincheck;

            }

        }

        public void OnHitEnemyCharged(Projectile proj, SpeculativeRigidbody body, bool yes)
        {
            PlayerController player = this.gun.CurrentOwner as PlayerController;

            foreach (var projectile in GetBullets())
            {
                if(projectile.name == "cation" && Vector2.Distance(proj.LastPosition, projectile.LastPosition) < 4)   
                {
                    DoSafeExplosion(projectile.LastPosition);
                    if (body.aiActor.healthHaver.IsBoss)
                    {
                        body.aiActor.healthHaver.ApplyDamage(14 * player.stats.GetStatValue(PlayerStats.StatType.Damage), Vector2.zero, "cation", CoreDamageTypes.Void, DamageCategory.Unstoppable, true, null, true);
                    }
                    else
                    {
                        body.aiActor.healthHaver.ApplyDamage(3 * player.stats.GetStatValue(PlayerStats.StatType.Damage), Vector2.zero, "cation", CoreDamageTypes.Void, DamageCategory.Unstoppable, true, null, true);
                    }
                    
                    projectile.DieInAir();
                }
            }
            
        }

        private List<Projectile> GetBullets()
        {
            PlayerController player = (PlayerController)this.gun.CurrentOwner;
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                if (projectile && projectile.sprite && !projectile.ImmuneToBlanks && !projectile.ImmuneToSustainedBlanks)
                {
                    if (projectile.Owner != null)
                    {
                        if (player)
                        {
                            list.Add(projectile);
                        }
                    }
                }
            }
            return list;
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
            damageRadius = 3f,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 18f,
            doDestroyProjectiles = true,
            doForce = false,
            debrisForce = 12f,
            preventPlayerForce = true,
            explosionDelay = 0f,
            usesComprehensiveDelay = false,
            doScreenShake = true,
            playDefaultSFX = true
        };
        private bool HasReloaded;
        public bool toggle = true;

        public override void  Update()
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

                foreach (var projectile in GetBullets())
                {
                    if (projectile.name == "cation" && projectile.LastVelocity.magnitude < .5)
                    {
                        projectile.DieInAir();
                    }
                }

                if (this.gun.GetChargeFraction() == 1 && toggle)
                {
                    AkSoundEngine.PostEvent("Play_sk_charge_ready", base.gameObject);
                    AkSoundEngine.PostEvent("Play_sk_charge_ready", base.gameObject);
                    AkSoundEngine.PostEvent("Play_sk_charge_ready", base.gameObject);
                    

                    toggle = false;
                }


            }




            base.Update();
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_cata_reload_sfx", base.gameObject);
                
                
            }
        }


    }
}

