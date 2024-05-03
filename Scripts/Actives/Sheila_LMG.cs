using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections.Generic;
using MultiplayerBasicExample;
using Alexandria.BreakableAPI;

namespace Knives
{
    class Sheila_LMG : AdvancedGunBehaviour
    {

        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Sheila LMG", "tur");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:sheila_lmg", "ski:sheila_lmg");
            gun.gameObject.AddComponent<Sheila_LMG>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Queen Of All LMGs");
            gun.SetLongDescription("This monsterous mammoth of a gun was so powerful that it can only be weilded by attaching it to a mounting brace." +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "tur_idle_001", 8);
           

            gun.SetAnimationFPS(gun.shootAnimation, 40);
            gun.SetAnimationFPS(gun.reloadAnimation, 3);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(142) as Gun, true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 6;
           
            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 7f;
            gun.DefaultModule.numberOfShotsInClip = 300;
            gun.DefaultModule.cooldownTime = .05f;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.SPECIAL;
            gun.encounterTrackable.EncounterGuid = "She's a beaut!";
            
            Gun shellingFlash = (Gun)PickupObjectDatabase.GetById(96);
            gun.muzzleFlashEffects = shellingFlash.muzzleFlashEffects;
           
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(1.8f, .5f, 0f);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "sail";
            gun.shellsToLaunchOnReload = 1;
            
            gun.shellsToLaunchOnFire = 4;
            Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
            gun.shellCasing = gun2.shellCasing;
            Casing = gun2.shellCasing;
            gun.shellsToLaunchOnReload = 1;
            gun.reloadShellLaunchFrame = 12;
            Mag = BreakableAPIToolbox.GenerateDebrisObject("Knives/Resources/Casings/tur_mag.png", true, 2, 4, 200, 150, null, 3f, null, null, 1).gameObject;


            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 7f;
            projectile.baseData.speed *= 2f;
            projectile.baseData.range *= 1f;
            projectile.baseData.force = 10f;
            BounceProjModifier bnc = projectile.GetComponent<BounceProjModifier>();
            bnc.numberOfBounces = 0;
           
            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }
        public static GameObject Casing;
        public static GameObject Mag;

        public static int ID;
        
        public override void OnPickedUpByPlayer(PlayerController player)
        {
            
            base.OnPickedUpByPlayer(player);
        }
        public override void OnGunsChanged(Gun previous, Gun current, bool newGun)
        {
            if (m_shield != null)
            {
                
                UnityEngine.GameObject.Destroy(m_shield);
            }

            base.OnGunsChanged(previous, current, newGun);
        }
        public bool fireSFXcontroller = true;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            
            
            gun.PreventNormalFireAudio = true;
        }
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
           
            
            base.OnFinishAttack(player, gun);
        }
        public System.Random rng = new System.Random();
        public override void PostProcessProjectile(Projectile projectile)
        {
            projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy));
            PlayerController player1 = base.gun.CurrentOwner as PlayerController;
            if (player1.PlayerHasActiveSynergy("Mas Queso"))
            {

                int random = rng.Next(1, 7);
                if (random == 1)
                {
                    int variance = rng.Next(-5, 5);
                    Projectile projectile2 = ((Gun)ETGMod.Databases.Items[626]).DefaultModule.projectiles[0];
                    GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, player1.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player1.CurrentGun == null) ? 0f : player1.CurrentGun.CurrentAngle + variance), true);
                    Projectile component = gameObject.GetComponent<Projectile>();
                    bool flag = component != null;
                    if (flag)
                    {

                        component.Owner = player1;
                        component.Shooter = player1.specRigidbody;
                        component.baseData.damage = 1f;

                    }
                    int variance2 = rng.Next(-5, 5);
                    Projectile projectile1 = ((Gun)ETGMod.Databases.Items[626]).DefaultModule.projectiles[0];
                    GameObject gameObject1 = SpawnManager.SpawnProjectile(projectile1.gameObject, player1.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player1.CurrentGun == null) ? 0f : player1.CurrentGun.CurrentAngle + variance2), true);
                    Projectile component1 = gameObject1.GetComponent<Projectile>();
                    bool flag1 = component1 != null;
                    if (flag1)
                    {

                        component1.Owner = player1;
                        component1.Shooter = player1.specRigidbody;
                        component1.baseData.damage = 1f;

                    }
                }

            }
          
            base.PostProcessProjectile(projectile);
        }
        public bool HasReloaded = true;
        public bool doing = false;
        public override void  Update()
        {
            if (this.gun.CurrentOwner)
            {
                
                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;

                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    
                    this.HasReloaded = true;
                }
                else
                {
                    gun.shellCasing = Mag;
                }

                if (this.gun.IsReloading)
                {
                    gun.shellCasing = Mag;
                }
                else
                {
                    gun.shellCasing = Casing;

                }

                if ((this.gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy("Projected Bastion"))
                {
                    if(doing == false)
                    {
                        StartCoroutine(ProjectedBarrierHandler());
                    }
                    
                }
            }

            base.Update();
        }
        public GameObject m_shield;
        public bool internalCooldown = false;
        private IEnumerator ProjectedBarrierHandler()
        {
            doing = true;
            PlayerController player = this.gun.CurrentOwner as PlayerController;
           
               
            Vector3 pointInSpace = this.gun.barrelOffset.transform.TransformPoint(1.2f, 0, 0);

            Gun owner = PickupObjectDatabase.GetById(380) as Gun;
            GameObject gameObject1 = owner.ObjectToInstantiateOnReload.gameObject;
            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject1, pointInSpace, Quaternion.identity);
            m_shield = gameObject2;
            gameObject2.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(pointInSpace, tk2dBaseSprite.Anchor.MiddleCenter);


            while (m_shield != null || player.PlayerHasActiveSynergy("Projected Bastion") == false)
            {
                Vector3 pointInSpace2 = this.gun.barrelOffset.transform.TransformPoint(1.2f, 0, 0);
                m_shield.transform.position = pointInSpace2;
                m_shield.GetComponent<SpeculativeRigidbody>().transform.position = pointInSpace2;
                m_shield.GetComponent<SpeculativeRigidbody>().Reinitialize();
                yield return null;
            }
            
            yield return new WaitForSeconds(6);
            
            doing = false;
            m_shield = null;
        }

        private void HandleHitEnemy(Projectile sourceProjectile, SpeculativeRigidbody hitRigidbody, bool fatal)
        {
            if (hitRigidbody.healthHaver.IsBoss)
            {
                hitRigidbody.healthHaver.ApplyDamage(6f, Vector2.zero, "BRRRRRT", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
            }
        }
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                

                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_tur_reload", base.gameObject);
                AkSoundEngine.PostEvent("Play_tur_reload", base.gameObject);
                AkSoundEngine.PostEvent("Play_tur_reload", base.gameObject);
                AkSoundEngine.PostEvent("Play_tur_reload", base.gameObject);
            }

        }
    }
}


