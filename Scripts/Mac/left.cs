using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Brave;
using Dungeonator;
using Brave.BulletScript;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using UnityEngine;

namespace Knives
{
    class Lefty : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Lefty", "Lefty");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:lefty", "ski:lefty");
            gun.gameObject.AddComponent<Lefty>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("A strong defence");
            gun.SetLongDescription("" +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "Lefty_idle_001", 8);
           
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(59) as Gun, true, false);
           


            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.HiddenOneHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
           
            gun.reloadTime = 1.2f;
            
            gun.DefaultModule.cooldownTime = .55f;
            gun.CanReloadNoMatterAmmo = true;
            

            
            gun.DefaultModule.numberOfShotsInClip = 600;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.B;
            

            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(.5f, .5f, 0f);

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 12f;
            projectile.BossDamageMultiplier = 2;
            projectile.baseData.speed = 3f;
            projectile.baseData.range = 3f;
            projectile.baseData.force = 5;
            ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
            
            slasher.SlashDimensions = 30;
            slasher.SlashRange = 3f;
            slasher.playerKnockback = 0;
            Gun swipeFlash = (Gun)PickupObjectDatabase.GetById(335);
            slasher.soundToPlay = "Play_gln_swing_miss_001";
            slasher.SlashVFX = swipeFlash.muzzleFlashEffects;
            slasher.InteractMode = SlashDoer.ProjInteractMode.IGNORE;
            gun.gameObject.AddComponent<GunSpecialStates>().DoesMacRage = true;

            SetupCollection();
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        

        public List<GameObject> extantSprites;
        private static tk2dSpriteCollectionData GunVFXCollection;
        private static GameObject VFXScapegoat;
        private static int Meter0ID;
        private static int Meter1ID;
        private static int Meter2ID;
        private static int Meter3ID;
        private static int Meter4ID;
        private static int Meter5ID;
        private static int Meter6ID;
        private static int Meter7ID;
        private static int Meter8ID;
        private static int Meter9ID;
        private static int Meter10ID;

        public override void  OnPickedUpByPlayer(PlayerController player)
        {

            base.OnPickedUpByPlayer(player);
        }
       
        public override void PostProcessProjectile(Projectile projectile)
        {   
            ProjectileSlashingBehaviour slash = projectile.gameObject.GetComponent<ProjectileSlashingBehaviour>();
            PlayerController player = (PlayerController)this.gun.CurrentOwner;
            if(gun.gameObject.GetComponent<GunSpecialStates>().RageCount < 10)
            {
                
                
            }
            else
            {
                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag = component != null;
                if (flag)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.baseData.damage = 300;
                    component.ignoreDamageCaps = true;

                    component.AdditionalScaleMultiplier = .5f;
                    component.baseData.speed *= .5f;
                    component.baseData.range = 3;
                    Invisible(component,0);
                    component.hitEffects.suppressMidairDeathVfx = true;
                    component.SuppressHitEffects = true;
                    component.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(component.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.KO));
                }
                gun.gameObject.GetComponent<GunSpecialStates>().RageCount = 0;
                DingDing = false;


            }
           
            base.PostProcessProjectile(projectile);
        }

        public void KO(Projectile proj,SpeculativeRigidbody spec ,bool imliterallynotgonnausethis)
        {
            AkSoundEngine.PostEvent("Play_KO_hit", gameObject);
            PlayerController player = (PlayerController)this.gun.CurrentOwner;
            RadialSlowInterface the_big_slow = new RadialSlowInterface();
            the_big_slow.RadialSlowHoldTime = .75f;
            the_big_slow.RadialSlowOutTime = .25f;
            the_big_slow.RadialSlowTimeModifier = 0f;
            the_big_slow.DoesSepia = true;
            the_big_slow.UpdatesForNewEnemies = true;
            the_big_slow.RadialSlowInTime = 0f;
            the_big_slow.DoRadialSlow(this.Owner.CenterPosition, player.CurrentRoom);
        }
        public void Invisible(Projectile proj, float f)
        {
            if (proj != null && proj.sprite != null)
            {
                proj.sprite.renderer.enabled = false;
            }
            if (proj.GetComponentInChildren<TrailController>() != null)
            {
                Destroy(proj.GetComponentInChildren<TrailController>());
            }
           
        }
        private static void SetupCollection()
        {
            VFXScapegoat = new GameObject();
            UnityEngine.Object.DontDestroyOnLoad(VFXScapegoat);
            Lefty.GunVFXCollection = SpriteBuilder.ConstructCollection(VFXScapegoat, "LeftyVFX_Collection");
            UnityEngine.Object.DontDestroyOnLoad(Lefty.GunVFXCollection);
            Meter1ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Meter/meter_000", Lefty.GunVFXCollection);
            Meter1ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Meter/meter_001", Lefty.GunVFXCollection);
            Meter2ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Meter/meter_002", Lefty.GunVFXCollection);
            Meter3ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Meter/meter_003", Lefty.GunVFXCollection);
            Meter4ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Meter/meter_004", Lefty.GunVFXCollection);
            Meter5ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Meter/meter_005", Lefty.GunVFXCollection);
            Meter6ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Meter/meter_006", Lefty.GunVFXCollection);
            Meter7ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Meter/meter_007", Lefty.GunVFXCollection);
            Meter8ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Meter/meter_008", Lefty.GunVFXCollection);
            Meter9ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Meter/meter_009", Lefty.GunVFXCollection);
            Meter10ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Meter/meter_010", Lefty.GunVFXCollection);
        }

        private IEnumerator ShowChargeLevel(GameActor gunOwner, int chargeLevel)
        {
            if (extantSprites.Count > 0)
            {
                for (int i = extantSprites.Count - 1; i >= 0; i--)
                {
                    UnityEngine.Object.Destroy(extantSprites[i].gameObject);
                }
                extantSprites.Clear();
            }
            GameObject newSprite = new GameObject("Level Popup", new Type[] { typeof(tk2dSprite) }) { layer = 0 };
            newSprite.transform.position = (gunOwner.transform.position + new Vector3(0.7f, 1.5f));
            tk2dSprite m_ItemSprite = newSprite.AddComponent<tk2dSprite>();
            extantSprites.Add(newSprite);
            int spriteID = -1;
            switch (chargeLevel)
            {
                case 0:
                    spriteID = Meter0ID;
                    break;
                case 1:
                    spriteID = Meter1ID;
                    break;
                case 2:
                    spriteID = Meter2ID;
                    break;
                case 3:
                    spriteID = Meter3ID;
                    break;
                case 4:
                    spriteID = Meter4ID;
                    break;
                case 5:
                    spriteID = Meter5ID;
                    break;
                case 6:
                    spriteID = Meter6ID;
                    break;
                case 7:
                    spriteID = Meter7ID;
                    break;
                case 8:
                    spriteID = Meter8ID;
                    break;
                case 9:
                    spriteID = Meter9ID;
                    break;
                case 10:
                    spriteID = Meter10ID;
                    break;
            }
            m_ItemSprite.SetSprite(Lefty.GunVFXCollection, spriteID);
            m_ItemSprite.PlaceAtPositionByAnchor(newSprite.transform.position, tk2dBaseSprite.Anchor.LowerCenter);
            m_ItemSprite.transform.localPosition = m_ItemSprite.transform.localPosition.Quantize(0.0625f);
            newSprite.transform.parent = gunOwner.transform;
            if (m_ItemSprite)
            {
                //sprite.AttachRenderer(m_ItemSprite);
                m_ItemSprite.depthUsesTrimmedBounds = true;
                m_ItemSprite.UpdateZDepth();
            }
            //sprite.UpdateZDepth();
            yield return new WaitForSeconds(1f);
            if (newSprite)
            {
                extantSprites.Remove(newSprite);
                Destroy(newSprite.gameObject);
            }
            yield break;
        }

        private bool HasReloaded;
        
        public  int LastChargeLevel = 0;
        public bool DingDing = false;

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
                if(gun.ClipShotsRemaining == gun.ClipCapacity)
                {
                    gun.ClipShotsRemaining = gun.ClipCapacity - 1;
                }

                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                if (gun.gameObject.GetComponent<GunSpecialStates>().RageCount >= 10 && DingDing == false)
                {
                   
                    DingDing = true;
                    AkSoundEngine.PostEvent("Play_KO_ready", gameObject);
                }

                if (gun.gameObject.GetComponent<GunSpecialStates>().RageCount > 10)
                {
                    player.StartCoroutine(ShowChargeLevel(player, 10));
                }
                else
                {
                   
                    player.StartCoroutine(ShowChargeLevel(player, gun.gameObject.GetComponent<GunSpecialStates>().RageCount));
                   
                }
                   
                if(LastChargeLevel != gun.gameObject.GetComponent<GunSpecialStates>().RageCount)
                {
                    LastChargeLevel = gun.gameObject.GetComponent<GunSpecialStates>().RageCount;
                    
                    switch (gun.gameObject.GetComponent<GunSpecialStates>().RageCount)
                    {
                        case 0:
                            
                            break;
                        case 1:
                            AkSoundEngine.PostEvent("Play_mac_boop_01", base.gameObject);
                            AkSoundEngine.PostEvent("Play_mac_boop_01", base.gameObject);
                            break;
                        case 2:
                            AkSoundEngine.PostEvent("Play_mac_boop_02", base.gameObject);
                            AkSoundEngine.PostEvent("Play_mac_boop_02", base.gameObject);
                            break;
                        case 3:
                            AkSoundEngine.PostEvent("Play_mac_boop_03", base.gameObject);
                            AkSoundEngine.PostEvent("Play_mac_boop_03", base.gameObject);
                            break;
                        case 4:
                            AkSoundEngine.PostEvent("Play_mac_boop_04", base.gameObject);
                            AkSoundEngine.PostEvent("Play_mac_boop_04", base.gameObject);
                            break;
                        case 5:
                            AkSoundEngine.PostEvent("Play_mac_boop_05", base.gameObject);
                            AkSoundEngine.PostEvent("Play_mac_boop_05", base.gameObject);
                            break;
                        case 6:
                            AkSoundEngine.PostEvent("Play_mac_boop_06", base.gameObject);
                            AkSoundEngine.PostEvent("Play_mac_boop_06", base.gameObject);
                            break;
                        case 7:
                            AkSoundEngine.PostEvent("Play_mac_boop_High", base.gameObject);
                            AkSoundEngine.PostEvent("Play_mac_boop_High", base.gameObject);
                            break;
                        case 8:
                            AkSoundEngine.PostEvent("Play_mac_boop_High", base.gameObject);
                            AkSoundEngine.PostEvent("Play_mac_boop_High", base.gameObject);
                            break;
                        case 9:
                            AkSoundEngine.PostEvent("Play_mac_boop_High", base.gameObject);
                            AkSoundEngine.PostEvent("Play_mac_boop_High", base.gameObject);
                            break;
                        case 10:
                           
                            break;
                    }
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

                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag = component != null;
                if (flag)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.baseData.damage = 0f;

                    ProjectileSlashingBehaviour slasher = component.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                    slasher.SlashDimensions = 45;
                    slasher.SlashRange = 2f;
                    slasher.playerKnockback = 0;
                    slasher.SlashVFX.type = VFXPoolType.None;
                    slasher.soundToPlay = null;
                    slasher.InteractMode = SlashDoer.ProjInteractMode.DESTROY;


                }
            }

            
            
        }


    }
}