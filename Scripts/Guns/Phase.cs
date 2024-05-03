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
    class PhaseBlade : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Phase Blade", "phase");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:phase_blade", "ski:phase_blade");
            gun.gameObject.AddComponent<PhaseBlade>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Tear The Veil");
            gun.SetLongDescription("Tap for normal strikes or charge to cut through the curtain leaving a lasting scar between points.\n\n" +
                "" +
                "A compact phase driver is built into the core of this blade. The Hegemony of Man's Ships would use similar drivers to tear into the first dimension to increase interstellar efficiency. " +
                "" +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "phase_idle_001", 8);
            gun.SetAnimationFPS(gun.chargeAnimation, 3);
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 16);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 4;



            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.CHARGE;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 2f;
            
            gun.DefaultModule.cooldownTime = .75f;
            gun.DefaultModule.numberOfShotsInClip = int.MaxValue;


            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.A;
            

            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(2f, .25f, 0f);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "samus";
            gun.muzzleFlashEffects.type = VFXPoolType.None;

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 30f;
            projectile.baseData.speed = 3f;
            projectile.baseData.range = 3f;

            


            ProjectileSlashingBehaviour slasher = projectile.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            slasher.SlashDimensions = 45;
            slasher.SlashRange = 5f;
            slasher.playerKnockback = 0;
            

            //portal slash
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            projectile2.baseData.damage = 50f;
            projectile2.baseData.speed = 3f;
            
            
            ProjectileSlashingBehaviour slasher2 = projectile2.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            slasher2.SlashDimensions = 45;
            slasher2.SlashRange = 5f;
            slasher2.playerKnockback = 0;
            
            slasher2.InteractMode = SlashDoer.ProjInteractMode.DESTROY;



            PhaseBlade.BuildPrefabOpen();
            PhaseBlade.BuildPrefabClose();

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f,
                AmmoCost = 0,


            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile2,
                ChargeTime = 1.25f,


            };
            
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2,
               
            };
            gun.sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", new Color32(243, 148, 00, 255));
            mat.SetFloat("_EmissiveColorPower", 6f);
            mat.SetFloat("_EmissivePower", 4);
            mat.SetFloat("_EmissiveThresholdSensitivity", .20f);

            MeshRenderer component = gun.GetComponent<MeshRenderer>();
            if (!component)
            {
                ETGModConsole.Log("nope");
                return;
            }
            Material[] sharedMaterials = component.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i].shader == mat)
                {
                    return;
                }
            }
            Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
            Material material = new Material(mat);
            material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
            sharedMaterials[sharedMaterials.Length - 1] = material;
            component.sharedMaterials = sharedMaterials;

            
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }

        public static int ID;
        public static void BuildPrefabOpen()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/Phase_portal/Portal_open_001", null);
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
            ItemBuilder.AddSpriteToObject("spriteObject", "Knives/Resources/Phase_portal/Portal_open_001", spriteObject); //add 1
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
                ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/Phase_portal/Portal_open_00{i}", spriteForObject);
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
            PhaseBlade.Open = gameObject;
        }


    
    public static void BuildPrefabClose()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/Phase_portal/Portal_close_001", null);
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
            ItemBuilder.AddSpriteToObject("spriteObject", "Knives/Resources/Phase_portal/Portal_close_001", spriteObject); //add 1
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
                ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/Phase_portal/Portal_close_00{i}", spriteForObject);
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
            PhaseBlade.Close = gameObject;
        }
       
        public override Projectile OnPreFireProjectileModifier(Gun gun, Projectile projectile, ProjectileModule mod)
        {
            PlayerController player = (PlayerController)this.gun.CurrentOwner;

            if (player.gameObject.GetComponent<portalController>() == null)
            {
                player.gameObject.GetOrAddComponent<portalController>();
            }
            if (this.m_extantReticleQuad) //If the cursor is there, then do the teleport
            {
                
                Vector2 worldCenter = this.m_extantReticleQuad.WorldCenter;
                Vector3 PortalclosePlacement = this.m_extantReticleQuad.WorldCenter;
                worldCenter += new Vector2(1.5f, 1.5f);
                PortalclosePlacement += new Vector3(1.5f, 1.5f,0);
                // place entrance and exit of portal here
                if (m_Open != null || m_Close != null)
                {
                    UnityEngine.GameObject.DestroyObject(m_Open);
                    UnityEngine.GameObject.DestroyObject(m_Close);
                }
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PhaseBlade.Open, player.transform.position + new Vector3(0.0f, 0f, 0f), Quaternion.identity);
                gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(player.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.MiddleCenter);
                
                m_Open = gameObject;
                
                
                GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(PhaseBlade.Close, PortalclosePlacement + new Vector3(0f, 0f, 0f), Quaternion.identity);
                gameObject2.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(PortalclosePlacement, tk2dBaseSprite.Anchor.MiddleCenter);
                m_Close = gameObject2;
                

                UnityEngine.Object.Destroy(this.m_extantReticleQuad.gameObject);
                player.gameObject.GetComponent<portalController>().startpoint = player.transform.position;
                player.gameObject.GetComponent<portalController>().endpoint = PortalclosePlacement;
                //TeleportPlayerToCursorPosition.StartTeleport(player, worldCenter);

            }
            

            return base.OnPreFireProjectileModifier(gun, projectile, mod);
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
        public override void Update()
        {
            PlayerController player = (PlayerController)this.gun.CurrentOwner;
            if (player != null)
            {
                if (player.gameObject.GetComponent<portalController>() == null)
                {
                    player.gameObject.GetOrAddComponent<portalController>();
                }
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
        public PhaseBlade()
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