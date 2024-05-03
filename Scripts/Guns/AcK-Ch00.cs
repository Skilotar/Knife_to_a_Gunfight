using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections.Generic;
using MultiplayerBasicExample;

namespace Knives
{
    class Ack_Choo : AdvancedGunBehaviour
    {

        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("AcK_Ch00", "AcK-Ch00");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:ack_ch00", "ski:ack_ch00");
            gun.gameObject.AddComponent<Ack_Choo>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Ill Intent");
            gun.SetLongDescription("A weapon forged from a misunderstanding of the term pathogen bio weapon on the initial order. The blacksmith worked day and night to give the weapon life only to curse it with perpetual illness." +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "AcK-Ch00_idle_001", 2);
            gun.SetAnimationFPS(gun.shootAnimation, 6);
            gun.SetAnimationFPS(gun.reloadAnimation, 5);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 4;
            gun.gunClass = GunClass.SHOTGUN;
            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 10;
            gun.DefaultModule.cooldownTime = .8f;
            gun.SetBaseMaxAmmo(400);
            gun.quality = PickupObject.ItemQuality.C;

            
            gun.PreventNormalFireAudio = true;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(.8f, .4f, 0f);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 6f;
            projectile.baseData.speed *= 2f;
            projectile.baseData.range *= 1f;
            projectile.hitEffects.suppressMidairDeathVfx = true;
            projectile.AdditionalScaleMultiplier *= .001f;
            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");




        }

        public override void OnPickedUpByPlayer(PlayerController player)
        {
            
            base.OnPickedUpByPlayer(player);
        }

        

        public override void OnPostFired(PlayerController player, Gun gun)
        {

            StartCoroutine(slightdelay(player));
            int sound = UnityEngine.Random.Range(1, 4);
            if(sound == 1)
            {
                AkSoundEngine.PostEvent("Play_Ack_shoot_001", base.gameObject);
            }
            if(sound == 2)
            {
                AkSoundEngine.PostEvent("Play_Ack_shoot_002", base.gameObject);
            }
            if(sound == 3)
            {
                AkSoundEngine.PostEvent("Play_Ack_shoot_003", base.gameObject);
            }
            gun.PreventNormalFireAudio = true;


        }
        
        private IEnumerator slightdelay(PlayerController player)
        {
            yield return new WaitForSeconds(.25f);
            int angle = -6;

            for (int i = 0; i < 6; i++)
            {
                float angleadjusted = angle;
                if (angle == -6)// set top side pattern
                {
                    angleadjusted = -7.5f;
                }

                if (angle == 6)// set bottom side pattern
                {
                    angleadjusted = 7.5f;
                }
                int pois = UnityEngine.Random.Range(1, 3);
                
                Projectile projectile = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + angleadjusted), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag2 = component != null;
                if (flag2)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.baseData.damage = 6f * player.stats.GetStatValue(PlayerStats.StatType.Damage);
                    component.baseData.speed *= (1f - (Math.Abs(angle) / 40));
                    component.UpdateSpeed();
                    if(pois != 1)
                    {
                        component.OnHitEnemy += this.onHitEnemy;
                        component.baseData.speed *= .9f;
                        component.baseData.damage *= .3f * player.stats.GetStatValue(PlayerStats.StatType.Damage);
                        component.UpdateSpeed();
                        component.DefaultTintColor = new Color(0.46f, 0.59f, 0.13f);
                        component.HasDefaultTint = true;
                    }
                }
                angle++;
                angle++;
            }
        }

        private void onHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.aiActor)
            {
                arg2.aiActor.ApplyEffect(new GameActorillnessEffect());
            }
        }

        public override void OnFinishAttack(PlayerController player, Gun gun)
        {


            base.OnFinishAttack(player, gun);
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            projectile.DieInAir();


            base.PostProcessProjectile(projectile);
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            this.HasReloaded = false;
            

            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            base.OnReloadPressed(player, gun, bSOMETHING);



        }
        
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
               
            }
        }
    }
}
