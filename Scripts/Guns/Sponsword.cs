using System;
using ItemAPI;
using Dungeonator;
using Gungeon;
using SaveAPI;

using Object = UnityEngine.Object;
using System.Collections;
using UnityEngine;

namespace Knives
{
    class Sponsword : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Sponsword", "Sponsword");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:sponsword", "ski:sponsword");
            gun.gameObject.AddComponent<Sponsword>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("This Run Is Sponsored By!");
            gun.SetLongDescription("The blade once held by a distant king's appointed knight. His mission was to brave the gungeon deep and return with the relic to fix the past.\n\n" +
                "When hitting an enemy this sword will produce coins. Gains a 3% damage boost per coin owned." +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "Sponsword_idle_001", 8);


            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, .7f, StatModifier.ModifyMethod.ADDITIVE);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1, StatModifier.ModifyMethod.ADDITIVE);
            gun.reloadTime = .25f;
            gun.DefaultModule.numberOfShotsInClip = int.MaxValue;
            gun.DefaultModule.cooldownTime = .8f;
            gun.RequiresFundsToShoot = true;

            gun.quality = PickupObject.ItemQuality.B;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Coin", "Knives/Resources/coin_clipfull", "Knives/Resources/coin_clipempty");

            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(0f, 0f, 0f);
            gun.carryPixelOffset = new IntVector2(14, -2);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 20f;
            projectile.baseData.speed = 3f;
            projectile.baseData.range = 3f;
            projectile.baseData.force = 5;
            gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            
            projectile.transform.parent = gun.barrelOffset;

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            projectile2.baseData.damage = 10;
            projectile2.baseData.speed = 10f;
            projectile2.baseData.range = 10f;
            projectile2.shouldRotate = true;
            projectile2.hitEffects.suppressMidairDeathVfx = true;
            projectile2.objectImpactEventName = null;
            projectile2.projectileHitHealth = 5;
            projectile2.collidesWithProjectiles = true;
            projectile2.SetProjectileSpriteRight("SwordVPN", 12, 35, false, tk2dBaseSprite.Anchor.MiddleCenter, 12, 35);
            
            VPN = projectile2;

            ETGMod.Databases.Items.Add(gun, null, "ANY");
            MiscToolMethods.TrimAllGunSprites(gun);
            Sponsword.StandardID = gun.PickupObjectId;
        }

        public static Projectile VPN;

        public static int StandardID;

        public System.Random rng = new System.Random();



        public override void OnPostFired(PlayerController player, Gun gun)
        {

            gun.PreventNormalFireAudio = true;

        }
       
        public override void PostProcessProjectile(Projectile projectile)
        {
            float num = 0;  // Funky little coin number goes brrrrrrrr
            PlayerController player = (this.gun.CurrentOwner as PlayerController);
            num = projectile.baseData.damage;
            float adjust = player.carriedConsumables.Currency / 33.34f; // 3% boost per coin
            num = num * (1f + adjust);
            ProjectileSlashingBehaviour stabby = projectile.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            stabby.SlashDamage = num;

            stabby.SlashDamageUsesBaseProjectileDamage = false;
            stabby.SlashVFX.type = VFXPoolType.None;
            stabby.SlashRange = 4.5f;
            stabby.DoSound = true;
            stabby.SlashDimensions = 69; // nice
            stabby.OnSlashHitEnemy += Stabby_OnSlashHitEnemy;
            stabby.InteractMode = SlashDoer.ProjInteractMode.DESTROY;

            if (player.PlayerHasActiveSynergy("Honey"))
            {
                int Rng = UnityEngine.Random.Range(1, 3);
                if (Rng == 1)
                {
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, player.specRigidbody.UnitCenter, Vector2.zero, 1f, false, true, true);
                }
            }

            if (player.PlayerHasActiveSynergy("SwordVPN"))
            {

                for (int i = 0; i < 3; i++)
                {
                    float angle = projectile.transform.eulerAngles.magnitude;
                    angle = angle - 30;
                    GameObject gameObject = SpawnManager.SpawnProjectile(VPN.gameObject, this.gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : angle + (i*30)), true);
                    Projectile component = gameObject.GetComponent<Projectile>();
                    bool flag = component != null;
                    if (flag)
                    {

                        component.Owner = this.gun.CurrentOwner;
                        component.Shooter = this.gun.CurrentOwner.specRigidbody;
                        component.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(component.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnpreRigidbodyCollision));
                        if (UnityEngine.Random.Range(1, 11) == 1)
                        {
                            player.DoPostProcessProjectile(component);
                        }

                    }
                    
                }




            }

                base.PostProcessProjectile(projectile);
        }

        private void OnpreRigidbodyCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if(myRigidbody != null)
            {
                if(otherRigidbody != null)
                {
                    if(otherRigidbody.projectile != null)
                    {
                        if(otherRigidbody.projectile.Owner != null)
                        {
                            if(otherRigidbody.projectile.Owner == this.Owner)
                            {
                                PhysicsEngine.SkipCollision = true;
                            }
                        }
                    }
                }
            }
        }

        public AnimationCurve ScalingCurve;
        public void Stabby_OnSlashHitEnemy(PlayerController player, AIActor hitEnemy, Vector2 forceDirection)
        {

            int rng = UnityEngine.Random.Range(1, 13);
            if (rng == 1)//silver
            {
                int rng2 = UnityEngine.Random.Range(1, 30);
                if (rng2 == 1)// gold
                {
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(74).gameObject, hitEnemy.specRigidbody.UnitCenter, Vector2.zero, 1f, false, true, true);
                }
                else
                {
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, hitEnemy.specRigidbody.UnitCenter, Vector2.zero, 2f, false, true, true);
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, hitEnemy.specRigidbody.UnitCenter, Vector2.up, 2f, false, true, true);
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, hitEnemy.specRigidbody.UnitCenter, Vector2.right, 2f, false, true, true);
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, hitEnemy.specRigidbody.UnitCenter, Vector2.left, 2f, false, true, true);
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, hitEnemy.specRigidbody.UnitCenter, Vector2.down, 2f, false, true, true);
                }

            }
            else//bronze
            {
                
                LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, hitEnemy.specRigidbody.UnitCenter, Vector2.zero, 1f, false, true, true);
                
            }

        }



        private bool HasReloaded;
        public bool setup = false;
        public int knownMoneys = 0;

        public override void Update()
        {
            PlayerController player = this.gun.CurrentOwner as PlayerController;
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
              
                if(setup == false)
                {
                    setup = true;
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, player.specRigidbody.UnitCenter, Vector2.zero, 2f, false, true, true);
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, player.specRigidbody.UnitCenter, Vector2.up, 2f, false, false, true);
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, player.specRigidbody.UnitCenter, Vector2.right, 2f, false, false, true);
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, player.specRigidbody.UnitCenter, Vector2.left, 2f, false, false, true);
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, player.specRigidbody.UnitCenter, Vector2.down, 2f, false, false, true);
                }

                
               

                
            }




        }

        public static bool hasAttackedThisCombat;
      
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {


                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);

                base.OnReloadPressed(player, gun, bSOMETHING);


            }

        }


        public override void OnDropped()
        {
           
            base.OnDropped();
        }


    }
}