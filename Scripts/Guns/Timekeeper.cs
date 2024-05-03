using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using Alexandria.ItemAPI;
using MultiplayerBasicExample;
using System.Collections.Generic;
using System.Reflection;
using Alexandria.BreakableAPI;


namespace Knives
{

    public class TimeKeeper : AdvancedGunBehavior
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("TimeKeeper", "TimeKeeper");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:timekeeper", "ski:timekeeper");
            gun.gameObject.AddComponent<TimeKeeper>();

            gun.SetShortDescription("Killing Time.");
            UpdateTimeKeep(gun);
            gun.SetLongDescription("Charge for High Noon!!\n\n" +
                "The long lost revolver of the Deadeyed lawbringer. Many a devilish soul met their maker at the wrong end of his barrel.\n\n" +
                "Everyone's time comes eventually, the reaper's watch is always on time..." +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "TimeKeeper_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 3;

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            
            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = .3f;
            gun.DefaultModule.numberOfShotsInClip = 6;
            gun.SetBaseMaxAmmo(300);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;
            gun.barrelOffset.transform.localPosition = new Vector3(41/16f, 26 / 16f, 0f);

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(15) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 8f;
            projectile.baseData.speed *= 1.2f;


            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(15) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            projectile2.baseData.damage = 0f;
            projectile2.baseData.speed *= 100f;
            projectile2.baseData.range = 1f;
            projectile2.AdditionalScaleMultiplier *= .01f;
            projectile2.hitEffects.suppressMidairDeathVfx = true;
            //gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, -1f, StatModifier.ModifyMethod.ADDITIVE);

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f
            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile2,
                ChargeTime = 1.2f,
                
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2
            };

            gun.DefaultModule.maxChargeTime = 7;
            gun.carryPixelOffset = new IntVector2(4, 1);
            MiscToolMethods.TrimAllGunSprites(gun);

            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            weed = BreakableAPIToolbox.GenerateDebrisObject("Knives/Resources/TumbleWeed_001", true, 7, 10, 100, 0, null, .2f, null, null, 0);

            tk2dSpriteAnimation animation = weed.gameObject.GetOrAddComponent<tk2dSpriteAnimation>();
            tk2dSpriteAnimator animator = weed.gameObject.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteCollectionData drone = SpriteBuilder.ConstructCollection(weed.gameObject, ("Tumble_Collection"));

            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip() { name = "Weed_idle", frames = new tk2dSpriteAnimationFrame[0], fps = 6 };
            List<tk2dSpriteAnimationFrame> frames4 = new List<tk2dSpriteAnimationFrame>();
            for (int i = 1; i <= 8; i++)
            {
                tk2dSpriteCollectionData collection4 = drone;
                int frameSpriteId4;
                if (i < 10)
                {
                    frameSpriteId4 = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/tumbleWeed/TumbleWeed_00{i}", collection4);
                }
                else
                {
                    frameSpriteId4 = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/tumbleWeed/TumbleWeed_0{i}", collection4);
                }
                tk2dSpriteDefinition frameDef4 = collection4.spriteDefinitions[frameSpriteId4];
                frames4.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId4, spriteCollection = collection4 });
            }
            clip.frames = frames4.ToArray();
            clip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { clip };
            animator.DefaultClipId = animation.GetClipIdByName("Weed_idle");
            animator.playAutomatically = true;



            ID = gun.PickupObjectId;
        }
        public static DebrisObject weed;
        public static int ID;

        public override void OnGunsChanged(Gun previous, Gun current, bool newGun)
        {
            if(current != this.gun)
            {
                PlayerController player = this.gun.CurrentOwner as PlayerController;
                Pixelator.Instance.SetFreezeFramePower(0, false);
                this.gun.RemoveStatFromGun(PlayerStats.StatType.MovementSpeed);
                player.stats.RecalculateStats(player);

                AkSoundEngine.PostEvent("Stop_Timekeep_HighNoon", base.gameObject);
               

                base.OnGunsChanged(previous, current, newGun);
            }
           
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            Shootsound();
            if (Targets != null)
            {
                if (projectile.GetCachedBaseDamage == 0)
                {
                    StartCoroutine(Draw());
                }
            }

            base.PostProcessProjectile(projectile);
        }

        private void Shootsound()
        {
            if(UnityEngine.Random.Range(0,4) < 1)
            {
                AkSoundEngine.PostEvent("Play_Timekeep_shoot_001", base.gameObject);
            }
            else
            {
                AkSoundEngine.PostEvent("Play_Timekeep_shoot_002", base.gameObject);
            }
        }

        private IEnumerator Draw()
        {
            
            PlayerController player = this.gun.CurrentOwner as PlayerController;
            for (int i = Targets.Count - 1; i >= 0; i--)
            {
                if(Targets[i] != null)
                {
                    AIActor actor = Targets[i];
                    if (actor != null)
                    {
                        if (actor.specRigidbody != null)
                        {
                            if (OMITBMathsAndLogicExtensions.PositionBetweenRelativeValidAngles(Targets[i].specRigidbody.UnitCenter, player.CurrentGun.barrelOffset.transform.position, gun.CurrentAngle, 35, 80))
                            {
                                Shootsound();
                                Vector2 vector = actor.CenterPosition - (Vector2)this.gun.barrelOffset.transform.position;
                                float angle = vector.ToAngle();
                                Projectile Shot = MiscToolMethods.SpawnProjAtPosi((PickupObjectDatabase.GetById(15) as Gun).DefaultModule.projectiles[0], this.gun.barrelOffset.transform.position, player, angle);
                                Shot.baseData.damage = 10f + (actor.gameObject.GetOrAddComponent<HighNoonTracker>().currentHighNoonStacks);
                                Shot.baseData.speed *= 2f;
                                Shot.AdditionalScaleMultiplier *= .8f;
                                Shot.ignoreDamageCaps = true;
                                ImprovedAfterImage trail = Shot.gameObject.GetOrAddComponent<ImprovedAfterImage>();
                                trail.shadowLifetime = .3f;
                                trail.shadowTimeDelay = .0001f;
                                trail.dashColor = UnityEngine.Color.grey;
                                trail.spawnShadows = true;

                                gun.spriteAnimator.Play("TimeKeeper_fire");
                                gun.LoseAmmo(1);
                                gun.ClipShotsRemaining--;
                                
                                yield return new WaitForSeconds(.13f);
                            }
                        }
                    }
                }
            }
            gun.reloadTime = 2;
            gun.Reload();
            gun.reloadTime = 1;
        }

        public int currentrotate;
        private bool HasReloaded;
        public bool setup = false;
        protected override void Update()
        {
            if (gun.CurrentOwner)
            {
                PlayerController player = this.gun.CurrentOwner as PlayerController;
                if(setup == false)
                {
                    TimeKeeperCleanup buzzbuzz = player.gameObject.GetOrAddComponent<TimeKeeperCleanup>();
                    buzzbuzz.player = player;
                    buzzbuzz.gun = this.gun;

                }
                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;

                }
                if (gun.GetChargeFraction() > .1f)
                { 
                    Pixelator.Instance.SetFreezeFramePower(gun.GetChargeFraction(), false);
                    reset = true;
                }
                else
                {
                    if (reset == true)
                    {
                        reset = false;
                        Pixelator.Instance.SetFreezeFramePower(0, false);
                    }
                }
               
                if (gun.GetChargeFraction() == 1)
                {
                    gun.SetShortDescription("ITS HIGH NOOON!");
                   
                    if(knownTime > 0)
                    {
                        StartCoroutine(doTumble());
                        
                        this.gun.RemoveStatFromGun(PlayerStats.StatType.MovementSpeed);
                        this.gun.AddStatToGun(PlayerStats.StatType.MovementSpeed, -2.5f,StatModifier.ModifyMethod.ADDITIVE);
                        player.stats.RecalculateStats(player);
                        AkSoundEngine.PostEvent("Play_Timekeep_HighNoon", base.gameObject);
                        knownTime = -1;
                        internaltimer = .2f;
                    }
                    
                    if (internaltimer <= 0)
                    {
                        if (player.CurrentRoom != null)
                        {
                            if (player.IsInCombat)
                            {
                                Targets = player.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All);
                                if(Targets != null)
                                {
                                    for (int i = Targets.Count - 1; i >= 0; i--)
                                    {
                                        AIActor actor = Targets[i];

                                        if (actor != null)
                                        {
                                            if (OMITBMathsAndLogicExtensions.PositionBetweenRelativeValidAngles(actor.specRigidbody.UnitCenter, player.CurrentGun.barrelOffset.transform.position, gun.CurrentAngle, 35, 80))
                                            {

                                                HighNoonTracker state = actor.gameObject.GetOrAddComponent<HighNoonTracker>();
                                                state.currentHighNoonStacks ++;
                                                
                                                state.player = player;

                                            }
                                        }
                                    }
                                    internaltimer = .075f;
                                }
                                
                            }
                        }
                    }
                   
                }
                else
                {
                    
                    if (System.DateTime.Now.Minute != knownTime)
                    {
                        AkSoundEngine.PostEvent("Stop_Timekeep_HighNoon", base.gameObject);
                        this.gun.RemoveStatFromGun(PlayerStats.StatType.MovementSpeed);
                        player.stats.RecalculateStats(player);
                        UpdateTimeKeep(this.gun);
                        knownTime = System.DateTime.Now.Minute;
                    }

                    

                }

                if(internaltimer >= 0)
                {
                    internaltimer -= BraveTime.DeltaTime;
                }

            }

        }

        private IEnumerator doTumble()
        {
            int bounces = 4;
            Vector2 dir = this.gun.barrelOffset.transform.position - this.gun.barrelOffset.transform.TransformPoint(0, 5, 0);
            GameObject tumble = UnityEngine.Object.Instantiate<GameObject>(weed.gameObject, this.gun.barrelOffset.transform.TransformPoint(0, 5, 0), Quaternion.identity);
            DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(tumble, this.gun.barrelOffset.transform.TransformPoint(0, 5, 0), dir, 7, false, false, true, false);
            
            while(bounces > 0)
            {
                if (GetPrivateType<DebrisObject, bool>(debrisObject, "onGround"))
                {
                    DebrisObject debrisObject2 = LootEngine.DropItemWithoutInstantiating(debrisObject.gameObject, debrisObject.transform.position, dir, 4, false, false, true, false);
                    debrisObject = debrisObject2;
                    
                    bounces -= 1;
                }
                yield return null;
            }
            debrisObject.TriggerDestruction();
            yield return null;
        }

       
        public List<AIActor> Targets = new List<AIActor>();
        public bool reset = false;
        int knownTime = -1;
        private float internaltimer;

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {

                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);

                AkSoundEngine.PostEvent("Play_WPN_rpg_reload_01", base.gameObject);


            }

        }

        public static void UpdateTimeKeep(Gun gun)
        {
            bool morning = true;
            int hour = System.DateTime.Now.Hour;
            if (hour > 12)
            {
                hour -= 12;
                morning = false;
            }
            int min = System.DateTime.Now.Minute;
            string minstring = System.DateTime.Now.Minute.ToString();
            if (min < 10) minstring = "0" + min;

            string time = hour + ":" + minstring;

            if (morning) gun.SetShortDescription("It's Roughly " + time + "AM");
            if (!morning) gun.SetShortDescription("It's Roughly " + time + "PM");
            if (time == "12:00") gun.SetShortDescription("ITS HIGH NOOON!");
            
        }
        private static T2 GetPrivateType<T, T2>(T obj, string field)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T2)f.GetValue(obj);
        }

    }
}

