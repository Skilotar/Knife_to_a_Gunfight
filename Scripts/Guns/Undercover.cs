using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;

namespace Knives
{

    public class Undercover : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Regular Gun", "Under");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:regular_gun", "ski:regular_gun");
            gun.gameObject.AddComponent<Undercover>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Totally Not A Cop");
            gun.SetLongDescription("Bullets penetrate enemy invulnerability as well as many objects. " +
                "" +
                "A Gun designed to aid Undercover cops by being as inconspicuous as possible. The rounds it fires can penetrate any defense by pretending to be completely non threatening." +
                "" +
                "\n\n\n - Knife_to_a_Gunfight");
           
          
            gun.SetupSprite(null, "Under_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 7);
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
            gun.SetBaseMaxAmmo(300);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.C;
           
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
            projectile.baseData.damage = 0f;
            projectile.baseData.speed *= 1f;
            projectile.PenetratesInternalWalls = true;

            gun.shellsToLaunchOnFire = 1;
            Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
            gun.shellCasing = gun2.shellCasing;

            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }


        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_beretta_shot_01", base.gameObject);

        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            projectile.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));
           
            base.PostProcessProjectile(projectile);
        }

       

        private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherbody, PixelCollider otherPixelCollider)
        {

            if (otherbody != null && otherbody.aiActor != null)
            {
                if (otherbody.healthHaver != null)
                {
                    if (otherbody.healthHaver.GetCurrentHealth() > 0)
                    {
                        PlayerController user = this.gun.CurrentOwner as PlayerController;
                        otherbody.healthHaver.IsVulnerable = true;
                        otherbody.healthHaver.ApplyDamage(7 * user.stats.GetStatValue(PlayerStats.StatType.Damage), Vector2.zero, "Undercover", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                    }
                   
                }
            }

            if(otherbody != null && otherbody.aiActor == null)
            {
                PhysicsEngine.SkipCollision = true;
            }

           

        }

        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
        public override void  Update()
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

        }
    
    }
}
