using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using SaveAPI;

namespace Knives
{

    public class HexEater : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Hexeater", "Hex");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:hexeater", "ski:hexeater");
            gun.gameObject.AddComponent<HexEater>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Give in");
            gun.SetLongDescription("Reloading hexes the user. While held lowers the chance to take hex damage when firing. In order to gain power sacrifices must be made. \n\n" +
                "By risking yourself to dark magic you are able to cast its burden onto others. " +
                "" +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Hex_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 15);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(377) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.3f;
            gun.DefaultModule.cooldownTime = .42f;
            gun.DefaultModule.numberOfShotsInClip = 6;
            gun.SetBaseMaxAmmo(400);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.C;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            
            gun.barrelOffset.transform.localPosition = new Vector3(1f, 1.3f, 0f);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.HasDefaultTint = true;
            projectile.DefaultTintColor = new Color(.90f, .10f, .39f);
            projectile.AdditionalScaleMultiplier = 2;
            projectile.baseData.damage = 10f;
            projectile.baseData.speed *= 1f;
            

            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.PISTOL;

            
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.HEX_MANIAC,true);

            ID = gun.PickupObjectId;
        }

        public static int ID;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
           

        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            if(projectile.Owner != null)
            {
                projectile.SuppressHitEffects = true;
                projectile.sprite.enabled = false;
                projectile.hitEffects.suppressMidairDeathVfx = true;
                projectile.DieInAir();
                StartCoroutine(slightshootdelay());
                
            }
            base.PostProcessProjectile(projectile);
        }

        private IEnumerator slightshootdelay()
        {
            yield return new WaitForSeconds(.05f);
            if(this.gun.CurrentOwner != null)
            {
                if (this.gun.CurrentOwner.gameObject.GetOrAddComponent<HexStatusEffectController>().statused)
                {
                    

                    AkSoundEngine.PostEvent("Play_WPN_magnum_shot_01", base.gameObject);
                    Projectile projectile2 = ((Gun)ETGMod.Databases.Items[377]).DefaultModule.projectiles[0];
                    GameObject gameObject2 = SpawnManager.SpawnProjectile(projectile2.gameObject, this.gun.sprite.WorldCenter + new Vector2(.5f,-.2f), Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : this.gun.CurrentAngle), true);
                    Projectile component2 = gameObject2.GetComponent<Projectile>();
                    component2.AdditionalScaleMultiplier = 2f;
                    component2.Owner = this.gun.CurrentOwner;
                    component2.HasDefaultTint = true;
                    component2.DefaultTintColor = new Color(.90f, .10f, .39f);
                    component2.baseData.damage *= 6f;
                    component2.baseData.speed *= 2f;
                    component2.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(component2.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemy));
                }
                else
                {


                    AkSoundEngine.PostEvent("Play_WPN_beretta_shot_01", base.gameObject);
                    Projectile projectile2 = ((Gun)ETGMod.Databases.Items[377]).DefaultModule.projectiles[0];
                    GameObject gameObject2 = SpawnManager.SpawnProjectile(projectile2.gameObject, this.gun.sprite.WorldCenter + new Vector2(.5f, -.2f), Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : this.gun.CurrentAngle), true);
                    Projectile component2 = gameObject2.GetComponent<Projectile>();
                    component2.AdditionalScaleMultiplier = 1.5f;
                    component2.Owner = this.gun.CurrentOwner;
                    component2.baseData.damage *= 1f;
                    component2.baseData.speed *= 1f;
                    
                }
            }
            
        }

        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2 && arg2.aiActor != null )
            {
                AIActor aiActor = arg2.aiActor;
                if (aiActor.IsNormalEnemy && !aiActor.IsHarmlessEnemy)
                {

                    HexStatusEffectController hexen = aiActor.gameObject.GetOrAddComponent<HexStatusEffectController>();
                    hexen.statused = true;

                }
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
                
                player.gameObject.GetOrAddComponent<HexStatusEffectController>().statused = true;
            }

        }

    }
}
