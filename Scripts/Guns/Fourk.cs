using System;
using ItemAPI;
using Dungeonator;
using Gungeon;
using SaveAPI;

using Object = UnityEngine.Object;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Knives
{
    class Fourk : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Picori Blade", "Fourk");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:picori_blade", "ski:picori_blade");
            gun.gameObject.AddComponent<Fourk>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Quaternion");
            gun.SetLongDescription("Charge to summon a crew of identical heros. A sword of mysterious power created by the Picori." +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "Fourk_idle_001", 8);
            gun.SetAnimationFPS(gun.chargeAnimation, 5);
            gun.SetAnimationFPS(gun.shootAnimation, 13);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 2;
            gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, .7f, StatModifier.ModifyMethod.ADDITIVE);
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.OneHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            
            gun.reloadTime = .25f;
            gun.DefaultModule.numberOfShotsInClip = int.MaxValue;
            gun.InfiniteAmmo = true;
            gun.DefaultModule.cooldownTime = .7f;
            gun.quality = PickupObject.ItemQuality.D;

            gun.carryPixelOffset = new IntVector2(4, 0);
          


            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(0f, 1f, 0f);
            
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 10f;
            projectile.baseData.speed = 3f;
            projectile.baseData.range = 3f;
            projectile.baseData.force = 5;

            ProjectileSlashingBehaviour slashy = projectile.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            slashy.SlashDamageUsesBaseProjectileDamage = true;
            slashy.InteractMode = SlashDoer.ProjInteractMode.IGNORE;
            slashy.SlashRange = 4f;
            slashy.SlashDimensions = 50f;
            slashy.SlashVFX = (PickupObjectDatabase.GetById(417) as Gun).muzzleFlashEffects;
           

            proj = projectile;
            projectile.transform.parent = gun.barrelOffset;

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f,
                AmmoCost = 1,


            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .2f,
                AmmoCost = 1,

            };
           
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2,
                
            };

            ETGMod.Databases.Items.Add(gun, null, "ANY");
            MiscToolMethods.TrimAllGunSprites(gun);
            StandardID = gun.PickupObjectId;
        }

        public static Projectile proj;
        public static int StandardID;

        public System.Random rng = new System.Random();

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            Spawning = false;
            gun.PreventNormalFireAudio = true;
            
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
           

            

            base.PostProcessProjectile(projectile);
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
                if(gun.GetChargeFraction() == 1)
                {
                    if(Spawning != true)
                    {
                        if (m_extantKages != null)
                        {
                            runClearDudes();
                        }
                        generate();
                    }
                   
                }
                



                if(player.PlayerHasActiveSynergy("Tools of the Heros"))
                {
                    if (player.gameObject.GetOrAddComponent<PicoriSpecialCases>())
                    {
                        player.gameObject.GetOrAddComponent<PicoriSpecialCases>();
                    }
                }
                
            }
        }
       

        
        public static void runClearDudes()
        {
            if(Fourk.ptrail != null)
            {
                Fourk.ptrail.spawnShadows = false;
            }
            
            foreach (KageBunshinController item in m_extantKages)
            {
                if (item != null)
                {
                    if (item.gameObject != null)
                    {
                        GlobalSparksDoer.DoRandomParticleBurst(20, item.transform.position, item.transform.position, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                        UnityEngine.Object.Destroy(item.gameObject);
                    }
                }
            }
            m_extantKages.Clear();

        }

        public static List<KageBunshinController> m_extantKages = new List<KageBunshinController>
        {
        };

        public bool Spawning { get; private set; }

        public void generate()
        {
            Spawning = true;
            int Rotation = 0;
            for (int i = 0; i <= 3; i++)
            {

                if (i != 0)
                {
                    GameObject shadowclone = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(820) as SpawnObjectPlayerItem).objectToSpawn, this.gun.CurrentOwner.specRigidbody.transform.position, Quaternion.identity);
                    KageBunshinController kageBunshin = shadowclone.GetOrAddComponent<KageBunshinController>();
                    kageBunshin.UsesRotationInsteadOfInversion = true;
                    kageBunshin.RotationAngle = Rotation;
                    kageBunshin.Duration = 99f;
                    kageBunshin.InitializeOwner(this.gun.CurrentOwner as PlayerController);

                    kageBunshin.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.Projectile, CollisionLayer.PlayerCollider, CollisionLayer.LowObstacle));
                    m_extantKages.Add(kageBunshin);

                    ImprovedAfterImage trail = shadowclone.GetOrAddComponent<ImprovedAfterImage>();
                    trail.shadowLifetime = .15f;
                    trail.shadowTimeDelay = .0001f;
                    trail.dashColor = colors[i];
                    trail.spawnShadows = true;
                   
                    PicoriCleanup p = shadowclone.gameObject.GetOrAddComponent<PicoriCleanup>();
                    p.player = this.gun.CurrentOwner as PlayerController;
                    p.PicoriBlade = this.gun;

                    shadowclone.GetComponentInChildren<Renderer>().material.SetColor("_FlatColor", colors[i]);
                }
                Rotation += 90;
            }
            ptrail = this.gun.CurrentOwner.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            ptrail.shadowLifetime = .10f;
            ptrail.shadowTimeDelay = .0001f;
            ptrail.dashColor = colors[0];
            ptrail.spawnShadows = true;

        }

        public static ImprovedAfterImage ptrail;

        public List<Color> colors = new List<Color>
        {
            new Color(.20f, .65f, .06f), // green
            new Color(.11f, .37f, .84f), // Blue
            new Color(.87f, .04f, .17f), // Red
            new Color(.45f, .08f, .53f), // Purp
            
        };

      
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            this.Spawning = false;
            runClearDudes();
        }


        public override void OnDropped()
        {

            base.OnDropped();
        }


    }
}