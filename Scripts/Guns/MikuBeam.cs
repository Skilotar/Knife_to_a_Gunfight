using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using System.CodeDom;
using Alexandria.BreakableAPI;

namespace Knives
{
    class MikuBeam : AdvancedGunBehaviour
    {
        /* !!!
         * For Anyone looking to see how this thing works.. I urge you to turn back now.
         * Beams should have never been made.
         * !!!
        */

        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Miku Beam", "MikuBeam");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:miku_beam", "ski:miku_beam");
            gun.gameObject.AddComponent<MikuBeam>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Its Time!");
            gun.SetLongDescription("A powerbeam of Unstoppable Hype Energy!\n\n" +
                "Trademark korg year 2058. \n\nThis sound cannon was found to be highly effective in combat situations given the user wears proper ear protection equipment.\n\n" +
                "Due to mass power consumption it requires a long time to recharge." +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "MikuBeam_idle_001", 4);
            gun.SetAnimationFPS(gun.chargeAnimation, 4);
            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 12);
            gun.AddProjectileModuleFrom((PickupObjectDatabase.GetById(15) as Gun), true, true);


            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.BEAM;
            gun.gunHandedness = GunHandedness.OneHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.UsesRechargeLikeActiveItem = true;
            gun.ActiveItemStyleRechargeAmount = 800;
            //gun.ActiveItemStyleRechargeAmount = 1;

            gun.DefaultModule.cooldownTime = 7f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.InfiniteAmmo = true;
            

            gun.quality = PickupObject.ItemQuality.S;
            gun.carryPixelOffset = new IntVector2(7, 0);
            gun.barrelOffset.transform.position = new Vector3(44/16f,11/16f, 0);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "planet";

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = .2005f;
            projectile.baseData.speed *= .01f;
            projectile.BossDamageMultiplier = 1.8f;
            projectile.baseData.range = 50f;
            projectile.shouldRotate = true;
            projectile.shouldFlipVertically = true;
            //projectile.sprite.enabled = false;
            projectile.specRigidbody.enabled = false;
           
           



            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 5.2f,
                
            };

            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
            };
            gun.PreventNormalFireAudio = true;

            List<string> BeamAnimPaths = new List<string>()
            {
                "Knives/Resources/BeamSprites/Miku_Beam_mid_001",
                "Knives/Resources/BeamSprites/Miku_Beam_mid_002",
                "Knives/Resources/BeamSprites/Miku_Beam_mid_003",
                "Knives/Resources/BeamSprites/Miku_Beam_mid_004",
            };
            List<string> BeamEndPaths = new List<string>()
            {
                "Knives/Resources/BeamSprites/Miku_Beam_end_001",
                "Knives/Resources/BeamSprites/Miku_Beam_end_002",
                "Knives/Resources/BeamSprites/Miku_Beam_end_003",
                "Knives/Resources/BeamSprites/Miku_Beam_end_004",
            };
            List<string> BeamStartPaths = new List<string>()
            {
                "Knives/Resources/BeamSprites/Miku_Beam_start_001",
                "Knives/Resources/BeamSprites/Miku_Beam_start_002",
                "Knives/Resources/BeamSprites/Miku_Beam_start_003",
                "Knives/Resources/BeamSprites/Miku_Beam_start_004",
            };
            List<string> BeamImpactPaths = new List<string>()
            {
                "Knives/Resources/BeamSprites/Miku_Beam_impact_001",
                "Knives/Resources/BeamSprites/Miku_Beam_impact_002",
                "Knives/Resources/BeamSprites/Miku_Beam_impact_003",
                "Knives/Resources/BeamSprites/Miku_Beam_impact_004",
            };

            //BULLET STATS
            Projectile BeamProj = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);

            BasicBeamController beamComp = BeamProj.GenerateBeamPrefab(
                "Knives/Resources/BeamSprites/Miku_Beam_mid_001",
                new Vector2(16, 22),
                new Vector2(0, 1),
                BeamAnimPaths,
                12,
                //Impact
                BeamImpactPaths,
                12,
                new Vector2(16, 16),
                new Vector2(0, 1),
                //End
                BeamEndPaths,
                12,
                new Vector2(16, 22),
                new Vector2(0, 1),
                //Beginning
                BeamStartPaths,
                12,
                new Vector2(16, 22),
                new Vector2(0, 1)
                );

            BeamProj.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(BeamProj.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(BeamProj);
            BeamProj.baseData.damage = 300f;
            BeamProj.ignoreDamageCaps = true;
            BeamProj.baseData.force *= 0.1f;
            BeamProj.baseData.range = 20f;
            BeamProj.baseData.speed *= 5f;
            
            //BeamProj.ignoreDamageCaps = true;
            
           
            beamComp.penetration = 120;
            beamComp.usesChargeDelay = true;
            beamComp.chargeDelay = 1.2f;
            
            beamComp.boneType = BasicBeamController.BeamBoneType.Straight;
            beamComp.DamageModifier *= 1f;
            beamComp.interpolateStretchedBones = true;

            beamy = projectile.gameObject.GetOrAddComponent<BeamBulletsBehaviour>();
            beamy.beamToFire = BeamProj;
            beamy.firetype = BeamBulletsBehaviour.FireType.BindToGun;
            beamy.BoundDuration = 9.4f;
            //beamy.GunLockForDuration = true;

            Projectile note1 = UnityEngine.Object.Instantiate<Projectile>(MiscToolMethods.standardproj);
            note1.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(note1.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(note1);
            gun.DefaultModule.projectiles[0] = note1;
            note1.baseData.damage = 7f;
            HomingModifier home = note1.gameObject.GetOrAddComponent<HomingModifier>();
            home.HomingRadius = 40;
            home.AngularVelocity = 270;
            BounceProjModifier bnc = note1.gameObject.GetOrAddComponent<BounceProjModifier>();
            bnc.numberOfBounces = 1;
            note1.SetProjectileSpriteRight("Note_01", 5, 5, false, tk2dBaseSprite.Anchor.MiddleCenter, 5, 5);

            Projectile note2 = UnityEngine.Object.Instantiate<Projectile>(MiscToolMethods.standardproj);
            note2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(note2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(note2);
            gun.DefaultModule.projectiles[0] = note2;
            note2.baseData.damage = 7f;
            HomingModifier home2 = note2.gameObject.GetOrAddComponent<HomingModifier>();
            home2.HomingRadius = 40;
            home2.AngularVelocity = 270;
            BounceProjModifier bnc2 = note2.gameObject.GetOrAddComponent<BounceProjModifier>();
            bnc2.numberOfBounces = 1;
            note2.SetProjectileSpriteRight("Note_02", 5, 5, false, tk2dBaseSprite.Anchor.MiddleCenter, 5, 5);
            ImprovedAfterImage trail = note1.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail.shadowLifetime = .5f;
            trail.shadowTimeDelay = .1f;
            trail.dashColor = ExtendedColours.MikuBlue;
            trail.spawnShadows = true;
            ImprovedAfterImage trail2 = note2.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail2.shadowLifetime = .5f;
            trail2.shadowTimeDelay = .1f;
            trail2.dashColor = ExtendedColours.MikuBlue;
            trail2.spawnShadows = true;

            note1.objectImpactEventName = "";
            note2.objectImpactEventName = "";

            Note1 = note1;
            Note2 = note2;
            main = projectile;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            BuildPrefabFlash();
        }
        public static Projectile main;

        public static void BuildPrefabFlash()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/Muzzleflashes/MikuBeam/Miku_Beam_flash_001", null);
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
            ItemBuilder.AddSpriteToObject("spriteObject", "Knives/Resources/Muzzleflashes/MikuBeam/Miku_Beam_flash_001", spriteObject); //add 1
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
            for (int i = 1; i < 3; i++) // add 2 and on
            {
                GameObject spriteForObject = new GameObject("spriteForObject");
                ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/Muzzleflashes/MikuBeam/Miku_Beam_flash_00{i}", spriteForObject);
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
            Flash = gameObject;
        }

        public static GameObject Flash;
        public static Projectile Note1;
        public static Projectile Note2;

        public static BeamBulletsBehaviour beamy;
        public Projectile BeamSpawner;

        public override void PostProcessProjectile(Projectile projectile)
        {
            if (projectile.GetCachedBaseDamage == .2005f)
            {
                projectile.DieInAir(); // kill you and spawn one that doesnt get post processed
                Projectile proj = MiscToolMethods.SpawnProjAtPosi(MikuBeam.main, gun.barrelOffset.transform.position, this.Owner as PlayerController, this.gun, 0, 1,false);
                BeamSpawner = proj;
                proj.specRigidbody.enabled = false;
                proj.specRigidbody.Reinitialize();
            }
        }
        // beams make me immensely upset

        public override void OnFinishAttack(PlayerController player, Gun gun)
        {

        }
        private bool HasReloaded;
        public bool toggle = false;
        public override void Update()
        {
            if (this.gun.CurrentOwner)
            {
                PlayerController player = gun.CurrentOwner as PlayerController;
                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer((this.gun.CurrentOwner as PlayerController).PlayerIDX);
                if (beamy.BindingGun == null)
                {
                    beamy.BindingGun = this.gun;
                }
                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;

                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }

                if(gun.GetChargeFraction() > 0 || BeamSpawner != null)
                {
                    BeamEnd = false;
                    if (once == false)
                    {
                        AkSoundEngine.PostEvent("Play_MikuBeam_001", base.gameObject);
                        once = true;
                    }
                }
                else
                {
                    if (once == true && BeamEnd == false)
                    {
                        AkSoundEngine.PostEvent("Play_BeamPowerDown", base.gameObject);
                        AkSoundEngine.PostEvent("Stop_MikuBeam_001", base.gameObject);
                        once = false;
                    }
                }


               
                if (gun.GetChargeFraction() == 1)
                {   
                    
                    if (instanceForPlayer.ActiveActions.ShootAction && Time.timeScale != 0 && gun.ClipShotsRemaining != 0 && !instanceForPlayer.ActiveActions.MapAction.IsPressed)
                    {
                        if (player.IsDodgeRolling)
                        {
                            player.ForceStopDodgeRoll();
                            instanceForPlayer.ActiveActions.DodgeRollAction.ClearInputState();
                            player.ToggleGunRenderers(true);
                            player.ToggleHandRenderers(true);
                            player.healthHaver.TriggerInvulnerabilityPeriod(1);
                            player.IsStationary = true;
                        }
                        if(player.HasPassiveItem(436) || PlayerHasModdedItem("ex:bloodied_scarf") || player.DodgeRollIsBlink)
                        {
                            player.ForceStopDodgeRoll();
                            player.ToggleGunRenderers(true);
                            player.ToggleHandRenderers(true);
                            player.sprite.renderer.enabled = true;
                            player.IsStationary = true;
                        }
                        instanceForPlayer.ActiveActions.ShootAction.ClearInputState();
                        player.StartCoroutine(MikuRave());
                        // I am in agony
                    }
                }
                if (instanceForPlayer.ActiveActions.MapAction.IsPressed || GameManager.Instance.IsPaused || GameManager.IsBossIntro)
                {
                    if(gun.GetChargeFraction() > 0)
                    {
                        instanceForPlayer.ActiveActions.ShootAction.ClearInputState();
                        AkSoundEngine.PostEvent("Stop_MikuBeam_001", base.gameObject);
                    }
                    
                }


                if(M_extantMuzzleflash == null)
                {
                    
                    gun.HandleAimRotation(player.transform.TransformPoint(40, 0, 0));
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(MikuBeam.Flash, gun.barrelOffset.transform.position, Quaternion.identity);
                    gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(gun.barrelOffset.transform.position, tk2dBaseSprite.Anchor.MiddleCenter);
                    gameObject.transform.parent = gun.barrelOffset.transform;
                    gameObject.GetComponent<tk2dSprite>().renderer.enabled = false;
                    M_extantMuzzleflash = gameObject;
                }

                if (Setup == false)
                {
                    
                    player.inventory.OnGunChanged += Inventory_OnGunChanged;
                    
                    LastKnownOwner = player;
                    Setup = true;
                }

            }

            base.Update();
        }

        public GameObject M_extantMuzzleflash = null;
        public bool once = false;
        public bool BeamEnd = false;
        public PlayerController LastKnownOwner;

        public bool Setup = false;

        private void Inventory_OnGunChanged(Gun previous, Gun current, Gun previousSecondary, Gun currentSecondary, bool newGun)
        {
            if(previous == this.gun)
            {
                
                AkSoundEngine.PostEvent("Stop_MikuBeam_001", base.gameObject);
                gun.spriteAnimator.Play(gun.idleAnimation);
                if (M_extantMuzzleflash) M_extantMuzzleflash.GetComponent<tk2dSprite>().renderer.enabled = false;
                if (BeamSpawner != null) BeamSpawner.DieInAir();
                LastKnownOwner.IsStationary = false;
            }
        }
        public bool KeyDown(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel)
            {
                return BraveInput.GetInstanceForPlayer((this.gun.CurrentOwner as PlayerController).PlayerIDX).ActiveActions.GetActionFromType(action).WasPressed;
            }
            else
            {
                return false;
            }

        }
        public bool PlayerHasModdedItem(string itemName)
        {
            PickupObject item = Gungeon.Game.Items.GetSafe(itemName);
            if (item != null) // does this item exist??

            {
                return base.gameObject.GetComponent<CustomDodgeRoll>().owner.HasPassiveItem(item.PickupObjectId);
            }
            else
            {
                return false;
            }
        }
        private IEnumerator MikuRave()
        {
            Player.IsStationary = true;
            yield return new WaitForSeconds(.05f); 
            if (BeamSpawner != null)
            {
                BeamEnd = true;
                PlayerController player = (gun.CurrentOwner as PlayerController);
                M_extantMuzzleflash.GetComponent<tk2dSprite>().renderer.enabled = true;
                BeamSpawner.transform.rotation = gun.barrelOffset.rotation;
                BeamSpawner.transform.parent = gun.barrelOffset.transform;
                
                Player.IsStationary = false;
                for (int i = 36; i >= 0; i--) // yes I know this could be a While Loop but the one I used wanted to be a frick and never Stop
                {
                    if (gun != null && gun.CurrentOwner != null)
                    {
                        if(gun == player.CurrentGun) // GUN SWAPPED SWITCH NOW!!!
                        {
                            if (gun.spriteAnimator.CurrentClip.name != gun.shootAnimation || gun.spriteAnimator.CurrentClip.name != gun.criticalFireAnimation)
                            {
                                gun.spriteAnimator.Play(gun.criticalFireAnimation);
                            }

                            int RNG = UnityEngine.Random.Range(0, validColors.Count);
                            RenderSettings.ambientLight = validColors[RNG];
                            CameraController camera = GameManager.Instance.MainCameraController;
                            camera.DoScreenShake(shake, gun.CurrentOwner.CenterPosition);
                            gameObject.transform.rotation = gun.barrelOffset.rotation;

                            Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                            player.knockbackDoer.ApplyKnockback(vector, -11, false);

                            Projectile projectile2 = ((Gun)ETGMod.Databases.Items[3]).DefaultModule.projectiles[0];
                            Projectile proj = MiscToolMethods.SpawnProjAtPosi(projectile2, gun.barrelOffset.position, player, gun);
                            proj.HasDefaultTint = true;
                            proj.DefaultTintColor = new Color(.57f, 1f, .93f); // Miku-Blue
                            proj.baseData.range *= 2f;

                            if (i <= 18)
                            {
                                proj.ignoreDamageCaps = true;
                                proj.AdditionalScaleMultiplier *= 3f;
                                proj.baseData.damage *= 2.5f;
                            }
                            else
                            {
                                proj.AdditionalScaleMultiplier *= 1.5f;
                            }


                            yield return new WaitForSeconds(.125f);
                            if (i <= 18)
                            {
                                float rng = UnityEngine.Random.Range(15, -15);
                                if (gun.barrelOffset.position != null)
                                {
                                    Projectile proj2 = MiscToolMethods.SpawnProjAtPosi(Note1, gun.barrelOffset.position, player, gun, rng);
                                    proj2.ignoreDamageCaps = true;
                                    proj2.baseData.damage = 8f;
                                }
                            }
                            yield return new WaitForSeconds(.125f);
                            if (i <= 18)
                            {
                                float rng = UnityEngine.Random.Range(15, -15);
                                if(gun.barrelOffset.position != null)
                                {
                                    Projectile proj1 = MiscToolMethods.SpawnProjAtPosi(Note2, gun.barrelOffset.position, player, gun, rng);
                                    proj1.ignoreDamageCaps = true;
                                    proj1.baseData.damage = 5f;
                                }
                                
                            }
                            yield return null;
                        }
                        else
                        {
                            i = -1; // break loop and end;
                            if(BeamSpawner!=null) BeamSpawner.DieInAir();

                            AkSoundEngine.PostEvent("Play_BeamPowerDown", base.gameObject);
                            AkSoundEngine.PostEvent("Stop_MikuBeam_001", base.gameObject);
                        }
                    }

                }
                gun.spriteAnimator.Play(gun.idleAnimation);
                M_extantMuzzleflash.GetComponent<tk2dSprite>().renderer.enabled = false;

                yield return null;
            }
            else // BeamSpawner is Nulled
            {
                Player.IsStationary = false;
            }
            yield return null;
        }

        public ScreenShakeSettings shake = new ScreenShakeSettings
        {
            magnitude = .6f,
            speed = 2f,
            time = .5f,
            falloff = .1f,
            direction = new Vector2(0, 0),
            vibrationType = ScreenShakeSettings.VibrationType.Auto,
            simpleVibrationStrength = Vibration.Strength.Medium,
            simpleVibrationTime = Vibration.Time.Instant
        };

        public List<Color> validColors = new List<Color>
        {
            new Color(1,1,1), // White
            new Color(.57f,1f,.93f), // Miku-Blue
            
        };

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

            AkSoundEngine.PostEvent("Play_BeamPowerDown", base.gameObject);
            AkSoundEngine.PostEvent("Stop_MikuBeam_001", base.gameObject);
            gun.spriteAnimator.Play(gun.idleAnimation);
            LastKnownOwner.inventory.OnGunChanged -= Inventory_OnGunChanged;
            if (M_extantMuzzleflash) M_extantMuzzleflash.GetComponent<tk2dSprite>().renderer.enabled = false;
            if (BeamSpawner != null) BeamSpawner.DieInAir();
            LastKnownOwner.IsStationary = false;
            Setup = false;

            base.OnDropped();
        }


    }
}