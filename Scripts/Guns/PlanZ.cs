using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using Alexandria.Misc;

namespace Knives
{

    public class PlanZ : AdvancedGunBehaviour
    {
        public static void Add()
        {
            
            Gun gun = ETGMod.Databases.Items.NewGun("Plan Z", "Plan_Z");
            
            Game.Items.Rename("outdated_gun_mods:plan_z", "ski:plan_z");
            gun.gameObject.AddComponent<PlanZ>();
            
            gun.SetShortDescription("Not Out Of Options");
            gun.SetLongDescription("Doubles base damage for ever empty gun in your inventory. \n\n" +
                "A simple pistol designed to be reliable in dire situations.\n Many soldiers never got the chance to use it but, the few who got the chance praised it for saving their lives. " + 
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Plan_Z_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 14);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 3f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = .75f;
            gun.DefaultModule.cooldownTime = .15f;
            gun.DefaultModule.numberOfShotsInClip = 6;
            gun.SetBaseMaxAmmo(325);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.C;

            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            gun.barrelOffset.transform.position = new Vector3(25 / 16f, 11 / 16f, 0);
            projectile.baseData.damage = 4f;
            projectile.baseData.speed *= 1f;
            projectile.gameObject.GetOrAddComponent<projectileStates>().planZ = true;



            gun.shellsToLaunchOnFire = 1;
            Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
            gun.shellCasing = gun2.shellCasing;



            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;
        }

        public static int ID;
        public static int NumberOfGunsOutOfAmmo;

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_beretta_shot_01", base.gameObject);
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            if(projectile.gameObject.GetComponent<projectileStates>() != null)
            {
                if(projectile.gameObject.GetComponent<projectileStates>().planZ == true)
                {
                    projectile.baseData.damage = (4 * (1 + NumberOfGunsOutOfAmmo)) * Player.stats.GetStatValue(PlayerStats.StatType.Damage);
                }
            }



            base.PostProcessProjectile(projectile);
        }

        public bool Startup = false;
        private bool HasReloaded;
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

                if(Startup == false)
                {
                    PlayerController player = gun.CurrentOwner as PlayerController;
                    player.inventory.OnGunChanged += Inventory_OnGunChanged;
                    player.GetExtComp().OnPickedUpAmmo += this.Gainedammo;
                    LastKnownOwner = player;
                    Startup = true;
                }


                

            }

        }

        
        PlayerController LastKnownOwner;
        public bool reeval = false;
        
        private void Inventory_OnGunChanged(Gun previous, Gun current, Gun previousSecondary, Gun currentSecondary, bool newGun)
        {
            recalcguns();
        }
        private void Gainedammo(PlayerController arg1, AmmoPickup arg2)
        {
            recalcguns();
        }

        public void recalcguns()
        {
           
            NumberOfGunsOutOfAmmo = 0;
            foreach (Gun gunz in LastKnownOwner.inventory.AllGuns)
            {
                if (gunz.CurrentAmmo == 0)
                {
                    NumberOfGunsOutOfAmmo++;
                }
            }
            
        }

        public override void OnDropped()
        {
            
            LastKnownOwner.GetExtComp().OnPickedUpAmmo -= this.Gainedammo;
            LastKnownOwner.inventory.OnGunChanged -= Inventory_OnGunChanged;
            Startup = false;
            base.OnDropped();
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
        public StatModifier reloadMod;
        public StatModifier damageMod;
    }
    
}
