using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using ItemAPI;
using Gungeon;
using System.Collections;
using HutongGames.PlayMaker.Actions;

namespace Knives
{
    class Steam_rifle : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Steam Rifle", "SteamRifle");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:steam_rifle", "ski:steam_rifle");
            
            gun.gameObject.AddComponent<Steam_rifle>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Hot Tempered");
            gun.SetLongDescription("Fire to build up heat, when overheated shots explode.\n\n" +
                "" +
                "A rifle with as firey a temper as its creator. Not wanting to waste perfectly good energy, London J. Punk removed the overheat checks on his earlier patent " +
                "making his gun more of an OSHA violation than it already was." +
                "" +

                "\n\n\n - Knife_to_a_Gunfight");
            gun.SetupSprite(null, "SteamRifle_idle_001", 1);
            gun.SetAnimationFPS(gun.shootAnimation, 9);
            gun.SetAnimationFPS(gun.reloadAnimation, 7);


            for (int i = 0; i < 30; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
                gun.gunSwitchGroup = (PickupObjectDatabase.GetById(24) as Gun).gunSwitchGroup;

            }
            foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
            {
                projectileModule.ammoCost = 1;
                projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                projectileModule.cooldownTime = 1f;
                projectileModule.angleVariance = 14f;
                
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                projectileModule.projectiles[0] = projectile;
                projectile.baseData.damage = 1;
                projectile.baseData.speed *= 1f;
                projectile.baseData.range = 20f;
                projectile.SuppressHitEffects = true;
                projectileModule.numberOfShotsInClip = int.MaxValue;
                projectile.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
                projectile.sprite.renderer.material.SetFloat("_VertexColor", 1f);
                projectile.sprite.color = projectile.sprite.color.WithAlpha(0.75f);
                projectile.sprite.usesOverrideMaterial = true;
                projectile.SetProjectileSpriteRight("steam", 12, 12, false, tk2dBaseSprite.Anchor.MiddleCenter, 12, 12);
                projectile.SuppressHitEffects = true;
                projectile.hitEffects.suppressHitEffectsIfOffscreen = true;
                projectile.hitEffects.suppressMidairDeathVfx = true;
                projectile.objectImpactEventName = null;


                PierceProjModifier stabby = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                stabby.penetratesBreakables = true;
                stabby.penetration = 5;
                BounceProjModifier bnc = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
                bnc.numberOfBounces = 1;

                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                
                gun.barrelOffset.transform.localPosition = new Vector3(2f, .5f, 0f);
               
                
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                gun.DefaultModule.projectiles[0] = projectile;
                bool flag = projectileModule != gun.DefaultModule;
                if (flag)
                {
                    projectileModule.ammoCost = 0;
                }


            }
            gun.PreventNormalFireAudio = true;
            gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            gun.gunClass = GunClass.RIFLE;
            gun.reloadTime = 0f;
            
            gun.DefaultModule.numberOfShotsInClip = int.MaxValue;
            gun.SetBaseMaxAmmo(200);
            gun.quality = PickupObject.ItemQuality.B;


            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.




            SetupCollection();
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            Steam_rifle.StandardID = gun.PickupObjectId;
        }
        public static int StandardID;

        public override void  OnPickup(GameActor owner)
        {
            PlayerController player = (PlayerController)owner;
            try
            {
                base.OnPickup(player);
                player.GunChanged += this.OnGunChanged;
            }
            catch (Exception e)
            {
                Tools.Print("Copper OnPickup", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }

        public override void  OnPostDrop(GameActor owner)

        {
            PlayerController player = (PlayerController)owner;
            try
            {
                player.GunChanged -= this.OnGunChanged;
                base.OnPostDrop(player);
            }
            catch (Exception e)
            {
                Tools.Print("Copper OnPostDrop", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }

        // Token: 0x06000392 RID: 914 RVA: 0x000261A4 File Offset: 0x000243A4
        private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            try
            {
                this.RestartHeatManager(newGun);
            }
            catch (Exception e)
            {
                Tools.Print("Copper OnGunChanged", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }
        private void RestartHeatManager(Gun current)
        {
            try
            {
                bool flag2 = (current == this.gun);
                if (flag2)
                {
                    HeatmanagerisRunning = false;

                }
                else
                {
                    StopCoroutine(HeatManager());
                }
            }
            catch (Exception e)
            {
                Tools.Print("Copper RemoveNutOnGunSwitchOut", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }

        public Vector3 pointInSpace;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            

            if (Steam_rifle.isOverheated == false)
            {
                AkSoundEngine.PostEvent("Play_Steam_Attack", base.gameObject);
                GlobalSparksDoer.DoRandomParticleBurst(20, gun.sprite.WorldBottomLeft, gun.sprite.WorldTopRight, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                Heatlvl = Heatlvl + 3;
            }
            else
            {
                AkSoundEngine.PostEvent("Play_Overheat_attack", base.gameObject);
                GlobalSparksDoer.DoRandomParticleBurst(40, gun.sprite.WorldBottomLeft, gun.sprite.WorldTopRight, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
                pointInSpace = this.gun.barrelOffset.transform.TransformPoint(3f, 0, 0);
                this.DoSafeExplosion(pointInSpace);
                Heatlvl = Heatlvl + 2;
            }
            gun.PreventNormalFireAudio = true;
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            
            gun.PreventNormalFireAudio = true;
            projectile.gameObject.GetOrAddComponent<SlowingBulletsEffect>();
            base.PostProcessProjectile(projectile);

        }
        public void DoSafeExplosion(Vector3 position)
        {

            ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
            this.smallPlayerSafeExplosion.effect = defaultSmallExplosionData2.effect;
            this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
            this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
            Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);

        }

        private ExplosionData smallPlayerSafeExplosion = new ExplosionData
        {
            damageRadius = 5f,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 25f,
            doDestroyProjectiles = true,
            doForce = true,
            debrisForce = 0f,
            preventPlayerForce = false,
            explosionDelay = 0.0f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = false,
            breakSecretWalls = true,
            secretWallsRadius = 3,
            force = 20,
            forceUseThisRadius = true,


        };
        private bool HasReloaded;
      
        
        public static int Heatlvl = 0;
        public override void  Update()
        {

            if (gun.CurrentOwner)
            {
                PlayerController player = this.gun.CurrentOwner as PlayerController;

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;

                }

                if (HeatmanagerisRunning == false)
                {
                    StartCoroutine(HeatManager());
                }

                if(Heatlvl > 23 )
                {
                    Heatlvl = 23;
                    player.StartCoroutine(ShowChargeLevel(player, Heatlvl));
                }
                if(Heatlvl < 0)
                {
                    Heatlvl = 0;
                    player.StartCoroutine(ShowChargeLevel(player, Heatlvl));
                }
                if(Heatlvl <= 23 || Heatlvl >= 0)
                {
                    player.StartCoroutine(ShowChargeLevel(player, Heatlvl));
                }
              

            }
            else
            {
                
                HeatmanagerisRunning = false;
                
            }

            base.Update();
        }
        public static bool isOverheated = false;
        public bool HeatmanagerisRunning = false;
        private IEnumerator HeatManager()
        {
            HeatmanagerisRunning = true;
            if (isOverheated)
            {
                yield return new WaitForSeconds(1.4f);
            }
            else
            {
                yield return new WaitForSeconds(.9f);
            }
           
            if (Heatlvl < 12 && Heatlvl > 0) // cooldown
            {
                Heatlvl--;
                isOverheated = false;
            }
            if(Heatlvl >= 12 && Heatlvl < 23)// overheat
            {
                Heatlvl++;
                isOverheated = true;
            }
            if(Heatlvl >= 23)
            {
                Heatlvl = 0;
                isOverheated = false;
            }
            if(Heatlvl < 0)
            {
                //what? How?
                Heatlvl = 0;
                isOverheated = false;
            }
            yield return new WaitForSeconds(.01f);
            HeatmanagerisRunning = false;
        }

        public override void OnDropped()
        {
            HeatmanagerisRunning = false;

            base.OnDropped();
        }
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                HasReloaded = false;

                base.OnReloadPressed(player, gun, bSOMETHING);
                
            }
        }



        public List<GameObject> extantSprites;
        private static tk2dSpriteCollectionData GunVFXCollection;
        private static GameObject VFXsteam;
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
        private static int Meter11ID;
        private static int Meter12ID;
        private static int Meter13ID;
        private static int Meter14ID;
        private static int Meter15ID;
        private static int Meter16ID;
        private static int Meter17ID;
        private static int Meter18ID;
        private static int Meter19ID;
        private static int Meter20ID;
        private static int Meter21ID;
        private static int Meter22ID;
        private static int Meter23ID;
     

        private static void SetupCollection()
        {
            VFXsteam = new GameObject();
            UnityEngine.Object.DontDestroyOnLoad(VFXsteam);
            Steam_rifle.GunVFXCollection = SpriteBuilder.ConstructCollection(VFXsteam, "steamVFX_collection");
            UnityEngine.Object.DontDestroyOnLoad(Steam_rifle.GunVFXCollection);

            Meter0ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_1", Steam_rifle.GunVFXCollection);
            Meter1ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_1", Steam_rifle.GunVFXCollection);
            Meter2ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_2", Steam_rifle.GunVFXCollection);
            Meter3ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_3", Steam_rifle.GunVFXCollection);
            Meter4ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_4", Steam_rifle.GunVFXCollection);
            Meter5ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_5", Steam_rifle.GunVFXCollection);
            Meter6ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_6", Steam_rifle.GunVFXCollection);
            Meter7ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_7", Steam_rifle.GunVFXCollection);
            Meter8ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_8", Steam_rifle.GunVFXCollection);
            Meter9ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_9", Steam_rifle.GunVFXCollection);
            Meter10ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_10", Steam_rifle.GunVFXCollection);
            Meter11ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_11", Steam_rifle.GunVFXCollection);
            Meter12ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_12", Steam_rifle.GunVFXCollection);
            Meter13ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_13", Steam_rifle.GunVFXCollection);
            Meter14ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_14", Steam_rifle.GunVFXCollection);
            Meter15ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_15", Steam_rifle.GunVFXCollection);
            Meter16ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_16", Steam_rifle.GunVFXCollection);
            Meter17ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_17", Steam_rifle.GunVFXCollection);
            Meter18ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_18", Steam_rifle.GunVFXCollection);
            Meter19ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_19", Steam_rifle.GunVFXCollection);
            Meter20ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_20", Steam_rifle.GunVFXCollection);
            Meter21ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_21", Steam_rifle.GunVFXCollection);
            Meter22ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_22", Steam_rifle.GunVFXCollection);
            Meter23ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Steam/steam_23", Steam_rifle.GunVFXCollection);
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
            newSprite.transform.position = (gunOwner.transform.position + new Vector3(0.7f, 1.4f));
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
                case 11:
                    spriteID = Meter11ID;
                    break;
                case 12:
                    spriteID = Meter12ID;
                    break;
                case 13:
                    spriteID = Meter13ID;
                    break;
                case 14:
                    spriteID = Meter14ID;
                    break;
                case 15:
                    spriteID = Meter15ID;
                    break;
                case 16:
                    spriteID = Meter16ID;
                    break;
                case 17:
                    spriteID = Meter17ID;
                    break;
                case 18:
                    spriteID = Meter18ID;
                    break;
                case 19:
                    spriteID = Meter19ID;
                    break;
                case 20:
                    spriteID = Meter20ID;
                    break;
                case 21:
                    spriteID = Meter21ID;
                    break;
                case 22:
                    spriteID = Meter22ID;
                    break;
                case 23:
                    spriteID = Meter23ID;
                    break;
            }
            m_ItemSprite.SetSprite(Steam_rifle.GunVFXCollection, spriteID);
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
            yield return new WaitForSeconds(.5f);
            if (newSprite)
            {
                extantSprites.Remove(newSprite);
                Destroy(newSprite.gameObject);
            }
            yield break;
        }



    }

}

