using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Gungeon;
using Dungeonator;
using MonoMod;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class ParticleCannon : MultiActiveReloadController
    {
        public static void Add()
        {

            Gun gun = ETGMod.Databases.Items.NewGun("Particle Cannon", "Particle_cannon");
            Game.Items.Rename("outdated_gun_mods:particle_cannon", "ski:particle_cannon");
            var behav = gun.gameObject.AddComponent<ParticleCannon>();

            behav.preventNormalFireAudio = true;
            gun.SetShortDescription("Ogon' Po Gotovnosti!");
            gun.SetLongDescription("Active Reload to deploy a personal barrier for 5 ammo. Projectiles that hit your barrier will charge up your beam.\n\n" +
                "A 350kg particle cannon able to consume kinetic energy to empower its beam. To a select few the weight of this cannon is merely a warmup."+
                "\n\n\n - Knife_to_a_Gunfight");

            gun.SetupSprite(null, "Particle_cannon_idle_001", 4);

            gun.SetAnimationFPS(gun.shootAnimation, 6);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(86) as Gun, true, false);

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).loopStart = 3;

            //gun.encounterTrackable.EncounterGuid = "prepare for titanfall";

            //GUN STATS
            gun.doesScreenShake = false;
            gun.DefaultModule.ammoCost = 5;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Beam;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 1.3f;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.DefaultModule.cooldownTime = 0.01f;
            gun.DefaultModule.numberOfShotsInClip = 20;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.BEAM;
            gun.gunClass = GunClass.BEAM;
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(107) as Gun).gunSwitchGroup;
            gun.barrelOffset.transform.localPosition = new Vector3(2f, .5f, 0f);
            gun.SetBaseMaxAmmo(800);


            List<string> BeamAnimPaths = new List<string>()
            {
                "Knives/Resources/BeamSprites/ParticleBm_mid_001",
                "Knives/Resources/BeamSprites/ParticleBm_mid_002",
                "Knives/Resources/BeamSprites/ParticleBm_mid_003",

            };
            List<string> BeamEndPaths = new List<string>()
            {
                "Knives/Resources/BeamSprites/ParticleBm_end_001",
                "Knives/Resources/BeamSprites/ParticleBm_end_002",
                "Knives/Resources/BeamSprites/ParticleBm_end_003",

            };
            List<string> BeamStartPaths = new List<string>()
            {
                "Knives/Resources/BeamSprites/ParticleBm_start_001",
                "Knives/Resources/BeamSprites/ParticleBm_start_002",
                "Knives/Resources/BeamSprites/ParticleBm_start_003",
            };

            //BULLET STATS
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);

            BasicBeamController beamComp = projectile.GenerateBeamPrefab(
                "Knives/Resources/BeamSprites/ParticleBm_mid_001",
                new Vector2(5, 3),
                new Vector2(0, 1),
                BeamAnimPaths,
                9,
                //Impact
                null,
                -1,
                null,
                null,
                //End
                BeamEndPaths,
                9,
                new Vector2(5, 3),
                new Vector2(0, 1),
                //Beginning
                BeamStartPaths,
                9,
                new Vector2(5, 3),
                new Vector2(0, 1)
                );

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.baseData.damage = 35f;
            projectile.baseData.force *= 0.1f;
            projectile.baseData.range = 10f;
            projectile.baseData.speed *= 5f;
            projectile.gameObject.GetOrAddComponent<ParticleCannonBeamController>();
            beamComp.penetration = 0;
           
            beamComp.boneType = BasicBeamController.BeamBoneType.Straight;
            beamComp.DamageModifier = 1f;
           
            gun.DefaultModule.projectiles[0] = projectile;

            gun.quality = PickupObject.ItemQuality.A;


            behav.activeReloadEnabled = true;
            behav.canAttemptActiveReload = true;

            behav.reloads = new List<MultiActiveReloadData>
            {
                new MultiActiveReloadData(0.35f, 20, 80, 80, 20, false, false, new ActiveReloadData
                {
                    damageMultiply = 1f,
                }, true, "Bubble",6,2)

            };


            SetupCollection();
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }


        public bool Startup = false;
        public static int ID;
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
            if (gun.IsReloading != true)
            {
                
                doing = false;
            }
            base.OnFinishAttack(player, gun);
        }

        public float internaltimer = 1f;
        private bool HasReloaded;
        
        public override void Update()
        {
            if (this.gun.CurrentOwner != null)
            {

                PlayerController player = (PlayerController)this.gun.CurrentOwner;

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }

                if (gun.IsFiring)
                {
                    AkSoundEngine.PostEvent("Play_Zar_fire_001", base.gameObject);
                }
                else
                {
                    AkSoundEngine.PostEvent("Stop_Zar_fire_001", base.gameObject);
                }

                if (Energy > 0)
                {
                    if (doing == false && m_shield == null) 
                    { 
                        StartCoroutine(ItDoGoDown(player));
                    }
                }

                if(Startup == false)
                {
                    Energy = 0;
                    Startup = true;
                }
                
                this.gun.CurrentOwner.StartCoroutine(ShowChargeLevel(this.gun.CurrentOwner, Mathf.RoundToInt(Energy / 10)));
            }


        }

        private IEnumerator ItDoGoDown(PlayerController player)
        {
            doing = true;
            yield return new WaitForSeconds(2.5f);
            Energy--;
            if (Energy < 0) Energy = 0;
            doing = false;
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_Zar_reload_001", base.gameObject);
                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer((this.gun.CurrentOwner as PlayerController).PlayerIDX);
                instanceForPlayer.ActiveActions.ShootAction.ClearInputState();
                if (player.PlayerHasActiveSynergy("Graviton Surge") && Energy >= 50)
                {
                    
                    Energy -= 10;
                    Projectile voidball = ((Gun)PickupObjectDatabase.GetById(169)).DefaultModule.projectiles[0].projectile;
                    Projectile voidball2 = MiscToolMethods.SpawnProjAtPosi(voidball, player.CenterPosition, player, gun);
                    voidball2.baseData.damage = 5;
                    
                }
            }
        }

        public override void OnActiveReloadSuccess(MultiActiveReload reload)
        {
            base.OnActiveReloadSuccess(reload);
            if (reload.Name == "Bubble")
            {
                gun.LoseAmmo(5);
                AkSoundEngine.PostEvent("Play_Zar_bubble_001", base.gameObject);
                StartCoroutine(BubbleMeScotty(this.gun.CurrentOwner as PlayerController, gun));
            }
        }


        public static int Energy = 0;
        private GameObject m_shield;
        public bool doing = false;

        public bool HeatmanagerisRunning { get; private set; }
        
        private IEnumerator BubbleMeScotty(PlayerController player, Gun gun)
        {
            
            Vector3 pointInSpace = player.specRigidbody.UnitCenter;

            Gun owner = PickupObjectDatabase.GetById(380) as Gun;
            GameObject gameObject1 = owner.ObjectToInstantiateOnReload.gameObject;
            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject1, pointInSpace, Quaternion.identity);
            m_shield = gameObject2;
            gameObject2.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(pointInSpace, tk2dBaseSprite.Anchor.MiddleCenter);
            gameObject2.transform.localScale = new Vector3(1.8f, 1.8f, 1);

            MajorBreakable breakable =  m_shield.GetOrAddComponent<MajorBreakable>();
            breakable.OnDamaged += this.Hitshield;
            breakable.MaxHitPoints = 8f;
            BreakableShieldController controller = m_shield.GetOrAddComponent<BreakableShieldController>();
            controller.maxDuration = 2;
            
            while (m_shield != null)
            {
                
                Vector3 pointInSpace2 = player.specRigidbody.UnitCenter + new Vector2(0,.25f);
                m_shield.transform.position = pointInSpace2;
                m_shield.GetComponent<SpeculativeRigidbody>().transform.position = pointInSpace2;
                m_shield.GetComponent<SpeculativeRigidbody>().Reinitialize();
                
                yield return null;
            }

            ChargeGainOnCurShield = 0;
            breakable.OnDamaged -= this.Hitshield;
           
            m_shield = null;
            
        }
        public int ChargeGainOnCurShield = 0;
        private void Hitshield(float obj)
        {
            
            if(ChargeGainOnCurShield <= 55)
            {
                ChargeGainOnCurShield += 7;
                Energy += 7;
            }

            if (Energy >= 100)
            {
                AkSoundEngine.PostEvent("Play_WPN_energy_impact_01", base.gameObject);
                Energy = 100;
            }

        }

        public List<GameObject> extantSprites;
        private static tk2dSpriteCollectionData GunVFXCollection;
        private static GameObject VFXzar;
        
        private static int Meter1ID;
        private static int Meter2ID;
        private static int Meter3ID;
        private static int Meter4ID;
        private static int Meter5ID;
        private static int Meter6ID;
        private static int Meter7ID;
        private static int Meter8ID;
        private static int Meter9ID;
        private static int Meter10ID;
        


        private static void SetupCollection()
        {
            VFXzar = new GameObject();
            UnityEngine.Object.DontDestroyOnLoad(VFXzar);
            ParticleCannon.GunVFXCollection = SpriteBuilder.ConstructCollection(VFXzar, "particleVFX_collection");
            UnityEngine.Object.DontDestroyOnLoad(ParticleCannon.GunVFXCollection);

           
            Meter1ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/PartOver/ParticleOverhead_001", ParticleCannon.GunVFXCollection);
            Meter2ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/PartOver/ParticleOverhead_002", ParticleCannon.GunVFXCollection);
            Meter3ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/PartOver/ParticleOverhead_003", ParticleCannon.GunVFXCollection);
            Meter4ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/PartOver/ParticleOverhead_004", ParticleCannon.GunVFXCollection);
            Meter5ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/PartOver/ParticleOverhead_005", ParticleCannon.GunVFXCollection);
            Meter6ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/PartOver/ParticleOverhead_006", ParticleCannon.GunVFXCollection);
            Meter7ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/PartOver/ParticleOverhead_007", ParticleCannon.GunVFXCollection);
            Meter8ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/PartOver/ParticleOverhead_008", ParticleCannon.GunVFXCollection);
            Meter9ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/PartOver/ParticleOverhead_009", ParticleCannon.GunVFXCollection);
            Meter10ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/PartOver/ParticleOverhead_010", ParticleCannon.GunVFXCollection);
           
        }

        private IEnumerator ShowChargeLevel(GameActor gunOwner, int chargeLevel)
        {
            if (extantSprites.Count > 0)
            {
                for (int i = extantSprites.Count - 1; i >= 0; i--)
                {
                    UnityEngine.Object.Destroy(extantSprites[i].gameObject);
                }
                extantSprites.Clear();
            }
            GameObject newSprite = new GameObject("Level Popup", new Type[] { typeof(tk2dSprite) }) { layer = 0 };
            newSprite.transform.position = (gunOwner.transform.position + new Vector3(0.7f, 1f));
            tk2dSprite m_ItemSprite = newSprite.AddComponent<tk2dSprite>();
            extantSprites.Add(newSprite);
            int spriteID = -1;
            switch (chargeLevel)
            {

                case 0:
                    spriteID = Meter1ID;
                    break;
                case 1:
                    spriteID = Meter1ID;
                    break;
                case 2:
                    spriteID = Meter2ID;
                    break;
                case 3:
                    spriteID = Meter3ID;
                    break;
                case 4:
                    spriteID = Meter4ID;
                    break;
                case 5:
                    spriteID = Meter5ID;
                    break;
                case 6:
                    spriteID = Meter6ID;
                    break;
                case 7:
                    spriteID = Meter7ID;
                    break;
                case 8:
                    spriteID = Meter8ID;
                    break;
                case 9:
                    spriteID = Meter9ID;
                    break;
                case 10:
                    spriteID = Meter10ID;
                    break;
                
            }
            m_ItemSprite.SetSprite(ParticleCannon.GunVFXCollection, spriteID);
            m_ItemSprite.PlaceAtPositionByAnchor(newSprite.transform.position, tk2dBaseSprite.Anchor.LowerCenter);
            m_ItemSprite.transform.localPosition = m_ItemSprite.transform.localPosition.Quantize(0.0625f);
            newSprite.transform.parent = gunOwner.transform;
            if (m_ItemSprite)
            {
                //sprite.AttachRenderer(m_ItemSprite);
                m_ItemSprite.depthUsesTrimmedBounds = true;
                m_ItemSprite.UpdateZDepth();
            }
            //sprite.UpdateZDepth();
            yield return new WaitForSeconds(.5f);
            if (newSprite)
            {
                extantSprites.Remove(newSprite);
                Destroy(newSprite.gameObject);
            }
            yield break;
        }


    }

    class ParticleCannonBeamController : MonoBehaviour
    {

        public ParticleCannonBeamController()
        {


        }


        private void Start()
        {
            this.Projectile = base.GetComponent<Projectile>();
           
            this.gun = this.Projectile.PossibleSourceGun;
            
           
        }

        private void Update()
        {
            if (Projectile != null)
            {
                Upgradeprojectile(this.Projectile);
            }
            if(this.Projectile != null)
            {
                if (ParticleCannon.Energy >= 80 && gun.IsFiring)
                {
                    GlobalSparksDoer.DoRandomParticleBurst(1, gun.barrelOffset.transform.position, gun.barrelOffset.transform.TransformPoint(10 + (.05f * ParticleCannon.Energy), 0,0), OMITBMathsAndLogicExtensions.DegreeToVector2(gun.CurrentAngle), 30, 6, null, .5f, ExtendedColours.vibrantOrange, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
                }
            }
        }

        public void Upgradeprojectile(Projectile projectile)
        {
            if (projectile != null)
            {
                Projectile.baseData.range = 10 + (.05f * ParticleCannon.Energy);
                Projectile.baseData.damage = 35 + (.4f * ParticleCannon.Energy);
                BasicBeamController bm = Projectile.GetComponent<BasicBeamController>();
                
                if(ParticleCannon.Energy <80 && ParticleCannon.Energy > 75)
                {
                    bm.AdjustPlayerBeamTint(ExtendedColours.orange, 1);
                }

                if (ParticleCannon.Energy >= 80)
                {
                    PierceProjModifier stabby = Projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                    stabby.penetration = 99;
                    stabby.penetratesBreakables = true;

                    Projectile.AppliesFire = true;
                    Projectile.FireApplyChance = .1f;
                    Projectile.fireEffect = StaticStatusEffects.hotLeadEffect;
                    bm.AdjustPlayerBeamTint(ExtendedColours.vibrantOrange, 1);
                }
                
            }
        }

        public bool hasbeenBuffed = false;

        public bool hasbeenupdated = false;

        private Projectile Projectile;
        private Gun gun;
        
    }
}

