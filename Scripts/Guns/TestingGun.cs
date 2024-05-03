using System;
using System.Collections;
using System.Collections.Generic;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using Brave;
using Dungeonator;
using HutongGames.PlayMaker.Actions;
using MultiplayerBasicExample;
using System.Reflection;


namespace Knives
{

    class testing_gun : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("testing gun", "wand");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:testing_gun", "ski:testing_gun");
            gun.gameObject.AddComponent<testing_gun>();

            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Check 1 testing testing");
            gun.SetLongDescription("bopmo bip!");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "wand_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.

            gun.AddProjectileModuleFrom("ak-47", true, false);
            //I would reccomend replacing the line above with 'gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(id) as Gun, true, false);'
            //'id' in that code I just gave you should be the numerical Id of the gun you want your projectiles to be based off of.
            gun.DefaultModule.angleVariance = 1f;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.0f;
            gun.DefaultModule.cooldownTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 2;
            gun.SetBaseMaxAmmo(200);


            gun.quality = PickupObject.ItemQuality.B;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 10f;
            projectile.baseData.speed *= 0f;
            projectile.baseData.range = 7f;
            projectile.AdditionalScaleMultiplier = 1f;
            
            projectile.transform.parent = gun.barrelOffset;

            ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
            slasher.doSpinAttack = true;

            ETGMod.Databases.Items.Add(gun, null, "ANY");
            
        }

        

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;
           
            
            
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

                if (gun.IsReloading)
                {
                   

                }

                
            }
        }

        public int count = 0;
        public override void OnAutoReload(PlayerController player, Gun gun)
        {
           
            if (this.gun.CurrentOwner)
            {
                
            }
        }

     
    } 
}
