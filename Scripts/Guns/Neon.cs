using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MonoMod.RuntimeDetour;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


using MultiplayerBasicExample;

namespace Knives
{

    class Neon : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Neon Desimator", "Neon_desimator");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:neon_desimator", "ski:neon_desimator");
            gun.gameObject.AddComponent<Neon>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("High Voltage!");
            gun.SetLongDescription("Crits against wet enemies! Some enemies are wet naturally others will need to be forced into it." +
                "\n\n\n - Knife_to_a_Gunfight");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "Neon_desimator_idle_001", 3);
            

            gun.SetAnimationFPS(gun.shootAnimation, 15);
            gun.SetAnimationFPS(gun.reloadAnimation, 9);

            gun.AddProjectileModuleFrom("dl45", true, true);


            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 1;
            gun.gunClass = GunClass.PISTOL;
            gun.gunHandedness = GunHandedness.OneHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 2f;
            gun.DefaultModule.numberOfShotsInClip = 9;
            gun.DefaultModule.cooldownTime = .25f;
            


            gun.SetBaseMaxAmmo(300);
            gun.quality = PickupObject.ItemQuality.C;
            gun.encounterTrackable.EncounterGuid = "*Krzirrrrt!";

            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "samus";
            Gun flash = (Gun)PickupObjectDatabase.GetById(58);
            gun.barrelOffset.transform.localPosition = new Vector3(2f, .7f, 0f);
            gun.muzzleFlashEffects = flash.muzzleFlashEffects;
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 7f;
            projectile.baseData.speed = 15f;
            projectile.baseData.range = 20f;
            projectile.baseData.force = 5;
            

            gun.sprite.usesOverrideMaterial = true;

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", new Color32(255, 5,5, 255));
            mat.SetFloat("_EmissiveColorPower", 6f);
            mat.SetFloat("_EmissivePower", 4);
            
            MeshRenderer component = gun.GetComponent<MeshRenderer>();
            if (!component)
            {
                ETGModConsole.Log("nope");
                return;
            }
            Material[] sharedMaterials = component.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i].shader == mat)
                {
                    return;
                }
            }
            Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
            Material material = new Material(mat);
            material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
            sharedMaterials[sharedMaterials.Length - 1] = material;
            component.sharedMaterials = sharedMaterials;

            

            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }


        public System.Random rng = new System.Random();

        public override void  OnPickedUpByPlayer(PlayerController player)
        {
           
            base.OnPickedUpByPlayer(player);
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            int sound = rng.Next(1, 4);
            switch (sound)
            {
                case 1:
                    AkSoundEngine.PostEvent("Play_neon_001", base.gameObject);
                    
                    break;
                case 2:
                    AkSoundEngine.PostEvent("Play_neon_002", base.gameObject);
                    
                    break;
                case 3:
                    AkSoundEngine.PostEvent("Play_neon_003", base.gameObject);
                    
                    break;
                default: // case 4
                    AkSoundEngine.PostEvent("Play_neon_004", base.gameObject);
                    
                    break;
            }


            gun.PreventNormalFireAudio = true;
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            
            projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemy));
            base.PostProcessProjectile(projectile);
        }
        public void OnHitEnemy(Projectile proj, SpeculativeRigidbody body, bool yes)
        {
            
            if (body != null)
            {
                if(body.aiActor != null)
                {
                    if (proj != null)
                    {
                        if (this.IsPointGoopy(body.aiActor.CenterPosition) || this.IsPointfirey(body.aiActor.CenterPosition))
                        {

                            Krzirrrrt(body);

                        }

                        for (int i = 0; i < this.wetwork.Count; i++)
                        {
                            bool isin = body.aiActor.EnemyGuid == this.wetwork[i];

                            if (isin)
                            {

                                Krzirrrrt(body);
                            }
                        }
                    }
                }
               
            }
        }

        public List<string> wetwork = new List<string>
        {   "042edb1dfb614dc385d5ad1b010f2ee3",
            "42be66373a3d4d89b91a35c9ff8adfec",
            "0239c0680f9f467dbe5c4aab7dd1eca6",
            "7b0b1b6d9ce7405b86b75ce648025dd6",
            "864ea5a6a9324efc95a0dd2407f42810",
            "022d7c822bc146b58fe3b0287568aaa2",
            "f155fd2759764f4a9217db29dd21b7eb",
            "9189f46c47564ed588b9108965f975c9",
            "e61cab252cfb435db9172adc96ded75f",
            "fe3fe59d867347839824d5d9ae87f244",
            "b8103805af174924b578c98e95313074",
            "6e972cd3b11e4b429b888b488e308551",
            "044a9f39712f456597b9762893fbc19c",
            "479556d05c7c44f3b6abb3b2067fc778",
            "1bc2a07ef87741be90c37096910843ab",
            "475c20c1fd474dfbad54954e7cba29c1",
            "1b5810fafbec445d89921a4efb4e42b7",
        };

        public void Krzirrrrt(SpeculativeRigidbody body)
        {
           
            Projectile projectile = ((Gun)ETGMod.Databases.Items[370]).DefaultModule.chargeProjectiles[1].Projectile;
            projectile.baseData.damage = 27f;
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, body.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.Owner.CurrentGun == null) ? 0f : this.gun.CurrentOwner.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            component.baseData.range = .01f;
            component.baseData.speed = 1f;
            PierceProjModifier stab = component.gameObject.GetOrAddComponent<PierceProjModifier>();
            stab.penetration = 0;
            component.Owner = this.gun.CurrentOwner;
            
            AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
            AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
            AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
          

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
                AkSoundEngine.PostEvent("Play_neon_reload", base.gameObject);

                
            }
        }
        private bool IsPointGoopy(Vector2 testPos)
        {
            IntVector2 Stupidfrickinggoop = (testPos / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
            if (DeadlyDeadlyGoopManager.allGoopPositionMap.ContainsKey(Stupidfrickinggoop))
            {
                DeadlyDeadlyGoopManager deadlyDeadlyGoopManager = DeadlyDeadlyGoopManager.allGoopPositionMap[Stupidfrickinggoop];
                return deadlyDeadlyGoopManager.IsPositionInGoop(testPos);
            }
            return false;
        }
        private bool IsPointfirey(Vector2 testPos)
        {
            IntVector2 Stupidfrickinggoop = (testPos / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
            if (DeadlyDeadlyGoopManager.allGoopPositionMap.ContainsKey(Stupidfrickinggoop))
            {
                DeadlyDeadlyGoopManager deadlyDeadlyGoopManager = DeadlyDeadlyGoopManager.allGoopPositionMap[Stupidfrickinggoop];
                return deadlyDeadlyGoopManager.IsPositionOnFire(testPos);
            }
            return false;
        }

    }
}


