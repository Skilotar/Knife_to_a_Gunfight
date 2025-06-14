using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using Alexandria.BreakableAPI;
using System.Collections.Generic;

namespace Knives
{

    public class Briefcase : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Deploy UC 25", "briefcase");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:deploy_uc_25", "ski:deploy_uc_25");
            gun.gameObject.AddComponent<Briefcase>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("High Class Brass");
            gun.SetLongDescription("Increases Firerate when swapped to.\n\n" +
                "A heavier bullpup model in the Under Cover line of weapons by Hammond Arms.\n" +
                "" +

                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "briefcase_idle_001", 240);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 13);
            gun.SetAnimationFPS(gun.introAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 1);

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.idleAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.idleAnimation).loopStart = 1;

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.criticalFireAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.criticalFireAnimation).loopStart = 1;
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 4f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;

            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.2f;
            gun.DefaultModule.cooldownTime = .15f;
            gun.DefaultModule.numberOfShotsInClip = 20;
            gun.SetBaseMaxAmmo(800);
            gun.quality = PickupObject.ItemQuality.B;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(15) as Gun).muzzleFlashEffects;
            gun.shellCasing = (PickupObjectDatabase.GetById(56) as Gun).shellCasing;



            gun.shellsToLaunchOnFire = 1;
            gun.shellsToLaunchOnReload = 0;
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.carryPixelOffset = new IntVector2(15, -3);
            gun.barrelOffset.transform.position = new Vector3(38 / 16f, 23 / 16f, 0 / 16f);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 5f;
            projectile.baseData.force = 0;
            projectile.baseData.speed *= 1.2f;
            projectile.AppliesKnockbackToPlayer = true;
            projectile.PlayerKnockbackForce = 1f;
            //projectile.DefaultTintColor = ExtendedColours.silvedBulletsSilver;
            projectile.HasDefaultTint = true;



            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.FULLAUTO;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            MiscToolMethods.TrimAllGunSprites(gun);

            ID = gun.PickupObjectId;
            //ETGModConsole.Log("1");
            pen = BreakableAPIToolbox.GenerateDebrisObject("Knives/Resources/pen_debris", true, 7, 10, 100, 0, null, .2f, null, null, 0);
            //ETGModConsole.Log("2");
            stapler = BreakableAPIToolbox.GenerateDebrisObject("Knives/Resources/stapler_debris", true, 7, 10, 100, 0, null, .2f, null, null, 0);
            //ETGModConsole.Log("3");
            paper_clip = BreakableAPIToolbox.GenerateDebrisObject("Knives/Resources/paper_clip_debris", true, 7, 10, 100, 0, null, .2f, null, null, 0);
            //ETGModConsole.Log("4");
            paper_1 = BreakableAPIToolbox.GenerateDebrisObject("Knives/Resources/paper_debris_001", true, 7, 10, 100, 0, null, .2f, null, null, 0);
            //ETGModConsole.Log("5");
            paper_2 = BreakableAPIToolbox.GenerateDebrisObject("Knives/Resources/paper_debris_002", true, 7, 10, 100, 0, null, .2f, null, null, 0);
            //ETGModConsole.Log("6");

        }

        //public List<DebrisObject> supplies;
        public static DebrisObject pen;
        public static DebrisObject paper_1;
        public static DebrisObject paper_2;
        public static DebrisObject paper_clip;
        public static DebrisObject stapler;

        public static int ID;
        public bool internalcooldown = false;
        public bool buffclip = false;
       

        // Token: 0x06000392 RID: 914 RVA: 0x000261A4 File Offset: 0x000243A4
        private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            try
            {
                if (newGun == this.gun) // swap to
                {
                    if (internalcooldown != true)
                    {
                        buffclip = true;
                        globalplayer.StartCoroutine(waitcooldown());

                        for(int i = 0; i < 3; i++)
                        {
                            int rng = UnityEngine.Random.Range(0, 6);
                            GameObject selectedObject = Briefcase.pen.gameObject;
                            if(rng == 1)
                            {
                                selectedObject = Briefcase.stapler.gameObject;
                            }
                            if (rng == 2)
                            {
                                selectedObject = Briefcase.paper_clip.gameObject;
                            }
                            if (rng == 3)
                            {
                                selectedObject = Briefcase.paper_2.gameObject;
                            }
                            if (rng == 4)
                            {
                                selectedObject = Briefcase.paper_1.gameObject;
                            }
                            if (rng == 5)
                            {
                                selectedObject = Briefcase.paper_2.gameObject;
                            }
                            if (rng == 6)
                            {
                                selectedObject = Briefcase.paper_1.gameObject;
                            }
                            int var = UnityEngine.Random.Range(-5,5);
                            Vector2 dir = this.Owner.CurrentGun.barrelOffset.transform.position - this.Owner.CurrentGun.barrelOffset.transform.TransformPoint(5, var, 0);
                            GameObject tumble = UnityEngine.Object.Instantiate<GameObject>(selectedObject, this.gun.sprite.WorldCenter, Quaternion.identity);
                            DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(tumble, this.gun.barrelOffset.transform.position, dir * -1, 7 + var, false, false, true, false);
                            debrisObject.canRotate = true;
                            debrisObject.angularVelocity = UnityEngine.Random.Range(-50, 50);
                        }

                        AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Trigger_01", base.gameObject);
                    }
                    AkSoundEngine.PostEvent("Play_OBJ_box_cover_01", base.gameObject);

                    
                }
                else
                {
                    buffclip = false;
                }
               
            }
            catch (Exception e)
            {
                Tools.Print("Copper OnGunChanged", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }

        private IEnumerator waitcooldown()
        {
            internalcooldown = true;
            yield return new WaitForSeconds(5f);
            AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Active_01", globalplayer.gameObject);
            internalcooldown = false;
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {

        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            AkSoundEngine.PostEvent("Play_WPN_deserteagle_shot_01", base.gameObject);
            base.PostProcessProjectile(projectile);
        }

        private bool HasReloaded;
        public bool toggle = false;
        public bool setup = false;
        public PlayerController globalplayer;

        public override void Update()
        {
            if (gun.CurrentOwner)
            {
                PlayerController player = (PlayerController)gun.CurrentOwner;
                globalplayer = player;
                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;

                }

                if(buffclip == true)
                {

                    

                    if (toggle == false)
                    {
                       
                        gun.AddCurrentGunStatModifier(PlayerStats.StatType.RateOfFire, .4f, StatModifier.ModifyMethod.ADDITIVE);
                        player.stats.RecalculateStats(player);

                       
                        toggle = true;
                    }

                }
                else
                {
                    if (toggle == true)
                    {
                        
                        gun.AddCurrentGunStatModifier(PlayerStats.StatType.RateOfFire, -.4f, StatModifier.ModifyMethod.ADDITIVE);
                        player.stats.RecalculateStats(player);

                        toggle = false;
                    }
                }

                if(gun.ClipShotsRemaining == 0)
                {
                    buffclip = false;
                }
                
                if(setup == false)
                {
                    player.GunChanged += this.OnGunChanged;
                    setup = true;
                }

            }
            else
            {
                if(gun != null)
                {
                    if (gun.spriteAnimator != null)
                    {
                        gun.spriteAnimator.PlayForDuration("briefcase_critical_fire", 999);
                    }
                }
               
                
                if (setup == true)
                {
                    globalplayer.GunChanged -= this.OnGunChanged;
                    setup = false;
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
                AkSoundEngine.PostEvent("Play_WPN_deserteagle_reload_01", base.gameObject);
                buffclip = false;
            }

        }

      
    }
}
