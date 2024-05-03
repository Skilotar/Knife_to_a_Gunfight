using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;

namespace Knives
{

    public class Baba : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Baba Yaga", "pbpp");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:baba_yaga", "ski:baba_yaga");
            gun.gameObject.AddComponent<Baba>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("You Know My Name");
            gun.SetLongDescription("A pocket pistol bloodied by years of contract killing. When it's owner quit the trade, this gun was lost to the gungeon. When reloading, hitting an enemy will steal ammo from them. Luckily, the ammo is universal. Bosses will have full ammo cases stolen from them." +
                "\n\n\n - Knife_to_a_Gunfight");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "pbpp_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 36);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = .75f;
            gun.DefaultModule.cooldownTime = .15f;
            gun.DefaultModule.numberOfShotsInClip = 9;
            gun.SetBaseMaxAmmo(36);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.C;
            gun.encounterTrackable.EncounterGuid = "I guess I'm back";
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            //projectile.baseData allows you to modify the base properties of your projectile module.
            //In our case, our gun uses modified projectiles from the ak-47.
            //Setting static values for a custom gun's projectile stats prevents them from scaling with player stats and bullet modifiers (damage, shotspeed, knockback)
            //You have to multiply the value of the original projectile you're using instead so they scale accordingly. For example if the projectile you're using as a base has 10 damage and you want it to be 6 you use this
            //In our case, our projectile has a base damage of 5.5, so we multiply it by 1.1 so it does 10% more damage from the ak-47.
            projectile.baseData.damage = 7f;
            projectile.baseData.speed *= 1f;
            projectile.transform.parent = gun.barrelOffset;
            gun.shellsToLaunchOnFire = 1;
            Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
            gun.shellCasing = gun2.shellCasing;

            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }


        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_beretta_shot_01", base.gameObject);

        }
        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
        public override void Update()
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
        
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_cata_reload_sfx", base.gameObject);
                

            }

            if (gun.ClipShotsRemaining < gun.ClipCapacity)
            {

               
                Gun swipeFlash = (Gun)PickupObjectDatabase.GetById(335);
                Projectile projectile1 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, player.CenterPosition, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                component.baseData.damage = 2f;
                component.Owner = player;
                ProjectileSlashingBehaviour slasher = component.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashRange = 3.5f;
                slasher.SlashDimensions = 10;
                slasher.SlashVFX = swipeFlash.muzzleFlashEffects;
                slasher.DoSound = false;
                slasher.OnSlashHitEnemy += Slasher_OnSlashHitEnemy;
                
            }


        }

        private void Slasher_OnSlashHitEnemy(PlayerController player, AIActor Enemy, Vector2 forceDirection)
        {
            if (Enemy != null && Enemy.aiActor != null)
            {

                AiactorSpecialStates state = Enemy.aiActor.gameObject.GetOrAddComponent<AiactorSpecialStates>();
                if (Enemy && Enemy.aiActor && Enemy.aiActor.IsNormalEnemy && state.LootedByBaba == false)
                {
                    if (!Enemy.aiActor.healthHaver.IsBoss)
                    {
                       
                        Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(tinyAmmo.ID).gameObject, Enemy.sprite.WorldCenter, vector * -1, 5f, false, false, false);
                        state.LootedByBaba = true;
                        Enemy.aiActor.behaviorSpeculator.Stun(2f, true);
                    }

                    if (Enemy.aiActor.healthHaver.IsBoss)
                    {
                        
                        Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(78).gameObject, Enemy.sprite.WorldCenter, vector * -1, 5f, false, false, false);
                        state.LootedByBaba = true;
                        Enemy.aiActor.behaviorSpeculator.Stun(2f, true);

                    }

                }



            }
        }

    }
}
