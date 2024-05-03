using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using Alexandria.ItemAPI;

using Knives;

namespace Knives
{
    
    public class kensuke : GunBehaviour
    {
  
     
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Kensuke", "Black_rose");
            
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:kensuke", "gm:kensuke");
            gun.gameObject.AddComponent<kensuke>();
            
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Un-fin-ished Business");
            gun.SetLongDescription("A curious firearm brought to the Gungeon by a mysterious ocean dweller.");
            
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip.
            // //You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "Black_rose_idle_001", 8);
            
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 20);
            gun.SetAnimationFPS(gun.reloadAnimation, 20);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 20);
            
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);

            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.InfiniteAmmo = true;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = 0.6f;
            gun.DefaultModule.numberOfShotsInClip = int.MaxValue;
            gun.muzzleFlashEffects = null;
            gun.doesScreenShake = true;
            gun.gunScreenShake = new ScreenShakeSettings(1f, 1f, 0.5f, 0.5f);

            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.A;
            //gun.encounterTrackable.EncounterGuid = "this is kensuke";
            
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;

            //MimicFinder (thx Ski!!)
            //MimicFinderComp kensukemimic = gun.gameObject.GetOrAddComponent<MimicFinderComp>();
            //kensukemimic.MimicFoundAnimation = gun.criticalFireAnimation;
            //kensukemimic.MimicFoundFPS = 10;

            //projectile.baseData allows you to modify the base properties of your projectile module.
            //In our case, our gun uses modified projectiles from the ak-47.
            //You can modify a good number of stats but for now, let's just modify the damage and speed.
            projectile.baseData.damage = 12f;
            projectile.baseData.speed = 16f;
			projectile.transform.parent = gun.barrelOffset;
            gun.barrelOffset.transform.localPosition += new Vector3(8f / 16f, 28f / 16f);
            projectile.AppliesKnockbackToPlayer = true;
            projectile.PlayerKnockbackForce = -10f;


            ProjectileSlashingBehaviour kensukestabby = projectile.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            kensukestabby.SlashRange = 3.6f;
            kensukestabby.SlashDimensions = 70f;
            kensukestabby.SlashDamage = 12f;
            kensukestabby.slashKnockback = 2f;
            kensukestabby.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
            //kensukestabby.playerKnockback = -2f;


            //This determines what sprite you want your projectile to use. Note this isn't necessary if you don't want to have a custom projectile sprite.
            //The x and y values determine the size of your custom projectile
            //projectile.SetProjectileSpriteRight("build_projectile", x, y, null, null);

            ETGMod.Databases.Items.Add(gun, null, "ANY");
       
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;
            //AkSoundEngine.PostEvent("Play_WPN_smileyrevolver_shot_01", gameObject);
        }
        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
       protected void Update()
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
			}
		}

        // While() loops the stuff inside until the condition its checking is false. if used wrong this can cause the whole game to stop and wait for an event that wont come.
        // coroutines can be set up like this one to perform a set of tasks alongside but apart from the main code.
        // That's why I can use a while() and not have it freeze.
        //This should create an invisible non damaging slash every .1 seconds while the gun is reloading.

        

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
                StartCoroutine(WhileReloadDeflect(player));
                HasReloaded = false;
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				base.OnReloadPressed(player, gun, bSOMETHING);
				//AkSoundEngine.PostEvent("Play_WPN_SAA_reload_01", base.gameObject);
			}
		}

        private IEnumerator WhileReloadDeflect(PlayerController player)
        {
            while (player.CurrentGun.IsReloading)
            {
                Projectile projectile1 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                component.baseData.damage = 0f;
                component.Owner = player;
                ProjectileSlashingBehaviour slasher = component.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashRange = 4f;
                slasher.SlashDimensions = 5;
                slasher.SlashVFX.type = VFXPoolType.None;
                slasher.InteractMode = SlashDoer.ProjInteractMode.DEFLECT;
                slasher.DeflectionDegree = 0;
                slasher.DoSound = false;
                yield return new WaitForSeconds(.1f);
            }
            yield return null;
        }

        //Now add the Tools class to your project.
        //All that's left now is sprite stuff. 
        //Your sprites should be organized, like how you see in the mod folder. 
        //Every gun requires that you have a .json to match the sprites or else the gun won't spawn at all
        //.Json determines the hand sprites for your character. You can make a gun two handed by having both "SecondaryHand" and "PrimaryHand" in the .json file, which can be edited through Notepad or Visual Studios
        //By default this gun is a one-handed weapon
        //If you need a basic two handed .json. Just use the jpxfrd2.json.
        //And finally, don't forget to add your Gun to your ETGModule class!
    }
}
