using Gungeon;
using System.Collections.Generic;
using UnityEngine;
using ItemAPI;
using Kvant;
using System.Collections;
using System;
using System.Reflection;

namespace Knives
{

    class GravitySlingshot : AdvancedGunBehaviour
    {

        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Gravity Slingshot", "Gravity_Slingshot");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:gravity_slingshot", "ski:gravity_slingshot");
            gun.gameObject.AddComponent<GravitySlingshot>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Assisted Ascent");
            gun.SetLongDescription("A toy made by a lab technician working under the guidance of Dr. Accretion Sr. \n\n" +
                "It amplifies the user's Gravitational pull to redirect energy pellets around them. Though it looks like a slingshot this should not be used to chuck stones." +
                "\n\n\n - Knife_to_a_Gunfight");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "Gravity_Slingshot_idle_001", 4);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 2);
            gun.SetAnimationFPS(gun.reloadAnimation, 5);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(181) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 1;

            gun.quality = PickupObject.ItemQuality.B;

            
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.


            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.directionlessScreenShake = true;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = 0.0f;
            gun.DefaultModule.numberOfShotsInClip = 6;
            gun.SetBaseMaxAmmo(800);

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(.5f, .5f, 0f);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "pulse";

            gun.DefaultModule.maxChargeTime = 5f;

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 0f;

            gun.carryPixelOffset = new IntVector2(14, -3);
            


            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .2f,
                AmmoCost = 0,
            };

            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item
            };
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;
        }
        public static int ID;
        
        public override void OnPickup(GameActor owner)
        {
            PlayerController player = (PlayerController)owner;
            try
            {
                base.OnPickup(player);
                player.GunChanged += this.OnGunChanged;
            }
            catch (Exception e)
            {
                Tools.Print("Copper OnPickup", "FFFFFF", true);
                Tools.PrintException(e);
            }
            base.OnPickup(owner);
        }

        private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            try
            {
                this.doingChargeUp = false;
               
            }
            catch (Exception e)
            {
                Tools.Print("Copper OnGunChanged", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }

      
        public override void OnPostDrop(GameActor owner)
        {
            PlayerController player = (PlayerController)owner;
            try
            {
                player.GunChanged -= this.OnGunChanged;
                base.OnPostDrop(player);
            }
            catch (Exception e)
            {
                Tools.Print("Copper OnPostDrop", "FFFFFF", true);
                Tools.PrintException(e);
            }

            this.doingChargeUp = false;
        }
        System.Random rng = new System.Random();
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
            
            base.OnFinishAttack(player, gun);
        }
        public override void PostProcessProjectile(Projectile projectile)
        {

            if(projectile.GetCachedBaseDamage == 0)
            {
                m_effects = projectile.statusEffectsToApply;
                projectile.hitEffects.suppressMidairDeathVfx = true;
                projectile.DieInAir();
                StartCoroutine(runBurst());
            }
            
            
            
            
            
           
            
           
            base.PostProcessProjectile(projectile);
        }

        private IEnumerator runBurst()
        {

            for(int i = 0; i <= knownOrbit.Count - 1; i++)
            {
                while(knownOrbit[i] == null && i < knownOrbit.Count) // should skip any broken parts of the array chain
                {
                    i++;
                }
               
                redirect(i);
                yield return new WaitForSeconds(.05f / (this.gun.CurrentOwner as PlayerController).stats.GetStatValue(PlayerStats.StatType.RateOfFire));

            }
            
            knownOrbit.Clear();
            doingChargeUp = false;
            yield return null;
        }
        public List<GameActorEffect> m_effects = new List<GameActorEffect>();
        public void redirect(int i)
        {
            if (knownOrbit[i] != null)
            {
                
                OrbitProjectileMotionModule orbit = spincheck;
                Projectile proj = knownOrbit[i];
                proj.statusEffectsToApply = m_effects;
                OMITBReflectionHelpers.InvokeMethod(typeof(OrbitProjectileMotionModule), "DeregisterSelfWithDictionary", orbit, new object[] { });
                proj.OverrideMotionModule = null;
                proj.SendInDirection(OMITBMathsAndLogicExtensions.CalculateVectorBetween(proj.specRigidbody.UnitCenter, (Vector2)this.gun.barrelOffset.transform.TransformPoint(7f, 0, 0)), true, true);
                proj.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Remove(proj.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));
                proj.specRigidbody.CollideWithTileMap = true;
                proj.UpdateCollisionMask();
                AkSoundEngine.PostEvent("Play_WPN_sflaser_shot_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_BOSS_space_zap_01", base.gameObject);

                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    (this.gun.CurrentOwner as PlayerController).DoPostProcessProjectile(proj);
                }

                if((this.gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy("Orbital Injection"))
                {
                    proj.OnHitEnemy += this.Onhitenemy;
                }

                if((this.gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy("Gravity Pull"))
                {

                    proj.Speed *= .5f;
                    proj.UpdateSpeed();
                    HomingModifier home = proj.gameObject.GetOrAddComponent<HomingModifier>();
                    home.HomingRadius = 4f;
                    home.AngularVelocity = 480f;
                   

                }

            }
        }
        public OrbitProjectileMotionModule spincheck2;
        private void Onhitenemy(Projectile proj, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.aiActor != null)
            {
                BounceProjModifier bouncer = proj.gameObject.GetOrAddComponent<BounceProjModifier>();
                bouncer.name = "Slingshot";
                PierceProjModifier stabby = proj.gameObject.GetOrAddComponent<PierceProjModifier>();
                stabby.penetration = 5;

                int orbitersInGroup = OrbitProjectileMotionModule.GetOrbitersInGroup(232);

                bouncer.projectile.specRigidbody.CollideWithTileMap = false;
                proj.UpdateCollisionMask();
                bouncer.projectile.ResetDistance();
                bouncer.numberOfBounces = 1;
                bouncer.projectile.baseData.range = Mathf.Max(bouncer.projectile.baseData.range, 10f);
                bouncer.projectile.collidesWithEnemies = true;

                OrbitProjectileMotionModule orbitProjectileMotionModule = new OrbitProjectileMotionModule();
                orbitProjectileMotionModule.lifespan = 2f;
                if (bouncer.projectile.OverrideMotionModule != null && bouncer.projectile.OverrideMotionModule is HelixProjectileMotionModule)
                {
                    orbitProjectileMotionModule.StackHelix = true;
                    orbitProjectileMotionModule.ForceInvert = (bouncer.projectile.OverrideMotionModule as HelixProjectileMotionModule).ForceInvert;
                }
                orbitProjectileMotionModule.usesAlternateOrbitTarget = true;
                orbitProjectileMotionModule.alternateOrbitTarget = arg2;

                orbitProjectileMotionModule.MinRadius = 1;
                orbitProjectileMotionModule.MaxRadius = 3;
                spincheck2 = orbitProjectileMotionModule;
                bouncer.projectile.OverrideMotionModule = spincheck2;
                proj.OnHitEnemy -= this.Onhitenemy;
            }
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {

            gun.PreventNormalFireAudio = true;


        }

        public bool doingChargeUp = false;

        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
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

                if (gun.HasChargedProjectileReady && doingChargeUp == false)
                {
                    StartCoroutine(handleChargeUp(gun.CurrentOwner as PlayerController));
                }
                
            }


            base.Update();
        }


        private IEnumerator handleChargeUp(PlayerController currentOwner)
        {
            doingChargeUp = true;
            if (gun.HasChargedProjectileReady)
            {
                gun.LoseAmmo(1);
                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[57]).DefaultModule.chargeProjectiles[0].Projectile;
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, this.gun.CurrentOwner.specRigidbody.UnitCenter, Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : this.gun.CurrentOwner.CurrentGun.CurrentAngle), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                component.Owner = currentOwner;
                component.baseData.damage *= 2f;
                component.baseData.speed *= 1.5f;
                component.pierceMinorBreakables = true;
                component.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(component.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));
                
                PierceProjModifier stabby = component.gameObject.GetOrAddComponent<PierceProjModifier>();
                stabby.penetratesBreakables = true;
                stabby.penetration = 1;
                
                
                DoAddSpin(component);
                knownOrbit.Add(component);
                
            }
            float chargeupdelay;
            PlayerController player = this.gun.CurrentOwner as PlayerController;
            chargeupdelay = .4f / player.stats.GetStatModifier(PlayerStats.StatType.RateOfFire);

            yield return new WaitForSeconds(chargeupdelay);
            doingChargeUp = false;
        }
        private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherbody, PixelCollider otherPixelCollider)
        {

            if (otherbody != null && otherbody.aiActor == null)
            {
                PhysicsEngine.SkipCollision = true;
            }
            if(otherPixelCollider != null && otherPixelCollider.IsTileCollider)
            {
                PhysicsEngine.SkipCollision = true;
            }
           

        }

        public List<Projectile> knownOrbit = new List<Projectile>
        { };

        public OrbitProjectileMotionModule spincheck;
        public void DoAddSpin(Projectile proj)
        {
            
            BounceProjModifier bouncer = proj.gameObject.GetOrAddComponent<BounceProjModifier>();
            bouncer.name = "Slingshot";

            int orbitersInGroup = OrbitProjectileMotionModule.GetOrbitersInGroup(231);
            if (orbitersInGroup >= 6)
            {
                return;
            }
            bouncer.projectile.specRigidbody.CollideWithTileMap = false;
            proj.UpdateCollisionMask();
            bouncer.projectile.ResetDistance();
            bouncer.numberOfBounces = 1;
            bouncer.projectile.baseData.range = Mathf.Max(bouncer.projectile.baseData.range, 500f);
            bouncer.projectile.collidesWithEnemies = true;
            
            OrbitProjectileMotionModule orbitProjectileMotionModule = new OrbitProjectileMotionModule();
            orbitProjectileMotionModule.lifespan = 4000f;
            if (bouncer.projectile.OverrideMotionModule != null && bouncer.projectile.OverrideMotionModule is HelixProjectileMotionModule)
            {
                orbitProjectileMotionModule.StackHelix = true;
                orbitProjectileMotionModule.ForceInvert = (bouncer.projectile.OverrideMotionModule as HelixProjectileMotionModule).ForceInvert;
            }
            orbitProjectileMotionModule.usesAlternateOrbitTarget = false;
            
            orbitProjectileMotionModule.MinRadius = 1;
            orbitProjectileMotionModule.MaxRadius = 1;
            spincheck = orbitProjectileMotionModule;
            bouncer.projectile.OverrideMotionModule = spincheck;

        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);

                StartCoroutine(runBurst());
                 
                AkSoundEngine.PostEvent("Play_BOSS_space_zap_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_ENM_electric_charge_01", base.gameObject);
            }
           
        }

      

    }
}