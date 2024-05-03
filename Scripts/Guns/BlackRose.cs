using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;

namespace Knives
{

    public class BlackRose : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Black Rose", "Black_rose");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:black_rose", "ski:black_rose");
            gun.gameObject.AddComponent<BlackRose>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Slay It With Flowers");
            gun.SetLongDescription("Crits When stealthed. Reload at full clip to spend ammo and go into stealth\n\n" +
                "" +
                "A Knife Pistol Combo for the Spy on the Fly. Sneak around and hit your enemies where it hurts." +
                "" +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Black_rose_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 17);
            gun.SetAnimationFPS(gun.introAnimation, 12);
           
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(182) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = .75f;
            gun.DefaultModule.cooldownTime = .5f;
            gun.DefaultModule.numberOfShotsInClip = 8;
            gun.SetBaseMaxAmmo(300);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.C;

            //MimicFinderComp mimi = gun.gameObject.GetOrAddComponent<MimicFinderComp>();
            //mimi.MimicFoundAnimation = gun.criticalFireAnimation;
            //mimi.MimicFoundFPS = 10;

            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.AdditionalScaleMultiplier *= .7f;
            projectile.baseData.damage = 14f;
            projectile.baseData.speed *= 1f;
            gun.shellsToLaunchOnFire = 0;
            gun.shellsToLaunchOnReload = 8;
            Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
            gun.shellCasing = gun2.shellCasing;


            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            MiscToolMethods.TrimAllGunSprites(gun);
        }

       

        // Token: 0x06000392 RID: 914 RVA: 0x000261A4 File Offset: 0x000243A4
        public void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            try
            {
                WasrecentlyStealthed = false;
                PlayerController player = this.gun.CurrentOwner as PlayerController;
                Player_OnDidUnstealthyAction(player);
            }
            catch (Exception e)
            {
                Tools.Print("BlackRose OnGunChanged", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_beretta_shot_01", base.gameObject);
            
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            if (WasrecentlyStealthed)
            {
                gun.spriteAnimator.ForceClearCurrentClip();
                gun.spriteAnimator.Play("Black_rose_critical_fire");
                projectile.baseData.damage *= 3f;
                projectile.HasDefaultTint = true;
                projectile.DefaultTintColor = UnityEngine.Color.black;
                
            }


            base.PostProcessProjectile(projectile);
        }



       
        private bool HasReloaded;
        public bool WasrecentlyStealthed = false;
        public bool doingCoroutine;
        public bool doingstealthtimer = false;
        public PlayerController play;
        //This block of code allows us to change the reload sounds.
        public override void Update()
        {
            if (gun.CurrentOwner)
            {
                play = gun.CurrentOwner as PlayerController;
                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;

                }

                if (gun.CurrentOwner.IsStealthed)
                {
                    WasrecentlyStealthed = true;
                }
                else
                {
                    if (WasrecentlyStealthed == true && doingCoroutine != true)
                    {
                        StartCoroutine(sligthstealthdelay());
                        
                    }
                }

                if(stealthtimer > 0)
                {
                    stealthtimer -= Time.deltaTime;
                    doingstealthtimer = true;
                }
                if(stealthtimer == 0 && doingstealthtimer)
                {
                    doingstealthtimer = false;
                    PlayerController player = this.gun.CurrentOwner as PlayerController;
                    player.SetIsStealthed(false, "Blackrose");
                    player.SetCapableOfStealing(false, "Blackrose", null);
                    AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", base.gameObject);
                    player.ChangeSpecialShaderFlag(1, 0f);
                    player.OnDidUnstealthyAction -= Player_OnDidUnstealthyAction;
                    player.PlayEffectOnActor(EasyVFXDatabase.BloodiedScarfPoofVFX, Vector3.zero);

                }
            }

        }
        public float stealthtimer = 5;
        private IEnumerator sligthstealthdelay()
        {
            doingCoroutine = true;
            yield return new WaitForSeconds(.2f);
            WasrecentlyStealthed = false;
            doingCoroutine = false;
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if(gun.ClipCapacity == gun.ClipShotsRemaining && player.IsStealthed == false)
            {
                player.SetIsStealthed(true, "Blackrose");
                player.OnDidUnstealthyAction += Player_OnDidUnstealthyAction;
                player.SetCapableOfStealing(true, "Blackrose", null);
                player.ChangeSpecialShaderFlag(1, 1f);
                gun.LoseAmmo(5); 
                AkSoundEngine.PostEvent("Play_ENM_wizardred_vanish_01", base.gameObject);
                player.GunChanged += this.OnGunChanged;
                player.PlayEffectOnActor(EasyVFXDatabase.BloodiedScarfPoofVFX, Vector3.zero);

            }

            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_cata_reload_sfx", base.gameObject);
               
            }

        }

        private void Player_OnDidUnstealthyAction(PlayerController obj)
        {
            PlayerController player = this.gun.CurrentOwner as PlayerController;
            player.SetIsStealthed(false, "Blackrose");
            obj.SetCapableOfStealing(false, "Blackrose", null);
            AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", base.gameObject);
            player.ChangeSpecialShaderFlag(1, 0f);
            player.OnDidUnstealthyAction -= Player_OnDidUnstealthyAction;
            player.GunChanged -= this.OnGunChanged;
            player.PlayEffectOnActor(EasyVFXDatabase.BloodiedScarfPoofVFX, Vector3.zero);
            WasrecentlyStealthed = false;
            
        }


        public override void OnDropped()
        {
            PlayerController player = play;
            player.SetIsStealthed(false, "Blackrose");
            player.SetCapableOfStealing(false, "Blackrose", null);
            AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", base.gameObject);
            player.ChangeSpecialShaderFlag(1, 0f);
            player.OnDidUnstealthyAction -= Player_OnDidUnstealthyAction;
            player.PlayEffectOnActor(EasyVFXDatabase.BloodiedScarfPoofVFX, Vector3.zero);


            base.OnDropped();
        }
    }
}

