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
    class MagicHat : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Magic Man", "hat");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:magic_man", "ski:magic_man");
            gun.gameObject.AddComponent<MagicHat>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Miss-Direction");
            gun.SetLongDescription("Charge to place a portal. Reload to recall the portal.\n\n" +
                "" +
                "A wormhole projection device wrapped elegantly inside of a small white magicians hat. The trick to fitting such a large device into such a small hat is to have the device hold itself in its own wormhole." +
                "" +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "hat_idle_001", 8);
            //gun.SetAnimationFPS(gun.chargeAnimation, 3);
            gun.SetAnimationFPS(gun.shootAnimation, 15);
            gun.SetAnimationFPS(gun.reloadAnimation, 16);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(61) as Gun, true, false);
            //gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            //gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 4;



            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.CHARGE;
            gun.gunHandedness = GunHandedness.OneHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = .1f;
            
            gun.DefaultModule.cooldownTime = .75f;
            gun.DefaultModule.numberOfShotsInClip = 20;


           
            gun.quality = PickupObject.ItemQuality.B;
            

            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(0f, .25f, 0f);
            
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 6f;
            projectile.baseData.speed = 15f;
            projectile.baseData.range = 10f;
            ProjectileSplitController split = projectile.gameObject.GetOrAddComponent<ProjectileSplitController>();
            split.distanceBasedSplit = true;
            split.distanceTillSplit = 1f;
            split.numberofsplits = 0;
            split.dmgMultAfterSplit = .8f;
            split.amtToSplitTo = 3;
            split.sizeMultAfterSplit = projectile.AdditionalScaleMultiplier * 1.06f;
            split.splitAngles = 45f;
            split.splitOnEnemy = false;
            PierceProjModifier stabby = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            stabby.penetration = 1;
            split.removeComponentAfterUse = true;
            projectile.ChanceToTransmogrify = 0;
            //portal slash
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            projectile2.baseData.damage = 6f;
            projectile2.baseData.speed = 15f;
            projectile2.baseData.range = 10f;
            ProjectileSplitController split2 = projectile2.gameObject.GetOrAddComponent<ProjectileSplitController>();
            split2.distanceBasedSplit = true;
            split2.distanceTillSplit = 1f;
            split2.numberofsplits = 0;
            split2.dmgMultAfterSplit = .8f;
            split2.amtToSplitTo = 3;
            split2.sizeMultAfterSplit = projectile2.AdditionalScaleMultiplier * 1.06f;
            split2.splitAngles = 20f;
            split2.splitOnEnemy = false;
            PierceProjModifier stabby2 = projectile2.gameObject.GetOrAddComponent<PierceProjModifier>();
            stabby2.penetration = 1;
            split.removeComponentAfterUse = true;
            projectile2.ChanceToTransmogrify = 0;

            MagicHat.BuildPrefabClose();

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f,
                AmmoCost = 0,


            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile2,
                ChargeTime = .5f,


            };
            
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2,
               
            };
           
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        public static void BuildPrefabClose()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/Hatportal/Hatportal_001", null);
            gameObject.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            tk2dSpriteAnimator animator = gameObject.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip
            {
                fps = 4,
                wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop
            };
            GameObject spriteObject = new GameObject("spriteObject");
            ItemBuilder.AddSpriteToObject("spriteObject", "Knives/Resources/Hatportal/Hatportal_001", spriteObject); //add 1
            tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame
            {
                spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId,
                spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection
            };
            tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
            {
                starterFrame
            };
            animationClip.frames = frameArray;
            for (int i = 1; i < 4; i++) // add 2 and on
            {
                GameObject spriteForObject = new GameObject("spriteForObject");
                ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/Hatportal/Hatportal_00{i}", spriteForObject);
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame
                {
                    spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId,
                    spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection
                };
                animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
            animationClip.name = "start";
            animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;
            MagicHat.Close = gameObject;
        }
       
        public override Projectile OnPreFireProjectileModifier(Gun gun, Projectile projectile, ProjectileModule mod)
        {
            PlayerController player = (PlayerController)this.gun.CurrentOwner;

           
            if (this.m_extantReticleQuad) //If the cursor is there, then do the teleport
            {
                
                Vector2 worldCenter = this.m_extantReticleQuad.WorldCenter;
                Vector3 PortalclosePlacement = this.m_extantReticleQuad.WorldCenter;
                worldCenter += new Vector2(1.5f, 1.5f);
                PortalclosePlacement += new Vector3(1.5f, 1.5f,0);
                // place entrance and exit of portal here
                if (m_Close != null)
                {
                    
                    UnityEngine.GameObject.DestroyObject(m_Close);
                }
               
                
                GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(MagicHat.Close, PortalclosePlacement + new Vector3(0f, 0f, 0f), Quaternion.identity);
                gameObject2.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(PortalclosePlacement, tk2dBaseSprite.Anchor.MiddleCenter);
                m_Close = gameObject2;
                

                UnityEngine.Object.Destroy(this.m_extantReticleQuad.gameObject);
                ISPortalOut = true;


            }
            

            return base.OnPreFireProjectileModifier(gun, projectile, mod);
        }
        public bool ISPortalOut = false;
        public bool DoingSFXProcess = false;
        public override void PostProcessProjectile(Projectile proj)
        {
            if (ISPortalOut == true)
            {
                proj.transform.position = MagicHat.m_Close.transform.position + new Vector3(.5f,.5f,0);
            }


            if (DoingSFXProcess == false)
            {
                StartCoroutine(playSFX());

            }


        }

        private IEnumerator playSFX()
        {
            DoingSFXProcess = true;
            yield return new WaitForSeconds(.01f);
            
            if (ISPortalOut)
            {
                AkSoundEngine.PostEvent("Play_WPN_wandbundle_shot_01", base.gameObject);
            }
            else
            {
                AkSoundEngine.PostEvent("Play_WPN_shotgun_shot_01", base.gameObject);
            }
            yield return new WaitForSeconds(.25f);
            DoingSFXProcess = false;
        }


        public override void OnPostFired(PlayerController player, Gun gun)
        {
            if (this.m_extantReticleQuad)
            {
                UnityEngine.GameObject.Destroy(m_extantReticleQuad.gameObject);

            }

            

            base.OnPostFired(player, gun);
        }
        private void UpdateReticlePosition()
        {
            PlayerController user = (PlayerController)this.gun.CurrentOwner;
            if (user && this.gun.HasChargedProjectileReady)
            {
                
                if (BraveInput.GetInstanceForPlayer(user.PlayerIDX).IsKeyboardAndMouse(false))
                {
                    Vector2 vector = user.unadjustedAimPoint.XY();
                    Vector2 vector2 = vector - this.m_extantReticleQuad.GetBounds().extents.XY();
                    this.m_extantReticleQuad.transform.position = vector2;
                }
                else
                {
                    BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(user.PlayerIDX);
                    Vector2 vector3 = user.CenterPosition + (Quaternion.Euler(0f, 0f, this.m_currentAngle) * Vector2.right).XY() * this.m_currentDistance;
                    vector3 += instanceForPlayer.ActiveActions.Aim.Vector * 12f * BraveTime.DeltaTime;
                    this.m_currentAngle = BraveMathCollege.Atan2Degrees(vector3 - user.CenterPosition);
                    this.m_currentDistance = Vector2.Distance(vector3, user.CenterPosition);
                    this.m_currentDistance = Mathf.Min(this.m_currentDistance, 100);
                    vector3 = user.CenterPosition + (Quaternion.Euler(0f, 0f, this.m_currentAngle) * Vector2.right).XY() * this.m_currentDistance;
                    Vector2 vector4 = vector3 - this.m_extantReticleQuad.GetBounds().extents.XY();
                    this.m_extantReticleQuad.transform.position = vector4;
                }

               
            }
            else
            {
                if (m_extantReticleQuad != null)
                {
                    UnityEngine.GameObject.Destroy(this.m_extantReticleQuad.gameObject);
                }

            }


        }

        public PlayerController player;
        private bool HasReloaded;
        public bool zoomout = false;
        public override void  Update()
        {
            PlayerController player = (PlayerController)this.gun.CurrentOwner;

           
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

                if (this.m_extantReticleQuad)
                {
                    this.UpdateReticlePosition();
                    
                }

                player = (PlayerController)this.gun.CurrentOwner;
                if (this.gun.GetChargeFraction() == 1)
                {
                    if (!m_extantReticleQuad)
                    {
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.reticleQuad);
                        reticle = gameObject;
                        this.m_extantReticleQuad = gameObject.GetComponent<tk2dBaseSprite>();
                        this.m_currentAngle = BraveMathCollege.Atan2Degrees(player.unadjustedAimPoint.XY() - player.CenterPosition);
                        this.m_currentDistance = 5f;
                        this.UpdateReticlePosition();
                       
                    }
                   
                }
               
            }
            
        }
        
       
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                UnityEngine.GameObject.DestroyObject(m_Close);
                ISPortalOut = false;
            }
        }
        public MagicHat()
        {
            reticleQuad = (PickupObjectDatabase.GetById(443) as TargetedAttackPlayerItem).reticleQuad;
        }
        public GameObject reticleQuad;
        public tk2dBaseSprite m_extantReticleQuad;
        public static GameObject reticle;
        private float m_currentAngle;
        private float m_currentDistance = 5f;
        public static GameObject Open;
        public static GameObject Close;
        public static GameObject m_Open;
        public static GameObject m_Close;
    }
}

