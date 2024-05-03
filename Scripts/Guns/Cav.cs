using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using System.Collections.Generic;

namespace Knives
{

    public class Cav : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Cavalry", "Cav");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:cavalry", "ski:cavalry");
            gun.gameObject.AddComponent<Cav>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Parry This You Filthy Casual");
            gun.SetLongDescription("Press reload or shoot while reloading to attempt a parry. Successfully parrying a bullet will instantly reload the weapon.\n\n" +
                "A 3 burst rifle from the Hegemony of Man's prototypes and experimental department. Despite its simple nature a well timed parry can effectively disarm incomming fire and the adenaline rush was enough to get troops to reload quicker." +

                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Cav_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 2);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).loopStart = 1;

            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 5f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            
            gun.reloadTime = 2.6f;
            gun.DefaultModule.cooldownTime = .5f;
            gun.DefaultModule.numberOfShotsInClip = 12;
            gun.SetBaseMaxAmmo(300);
            gun.quality = PickupObject.ItemQuality.C;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(182) as Gun).muzzleFlashEffects;
            
            gun.gameObject.AddComponent<GunSpecialStates>().DoesCountBlocks = true;
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
           
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 8;
            projectile.baseData.force = 0;
            projectile.baseData.speed *= 1f;
            projectile.transform.parent = gun.barrelOffset;
            gun.shellsToLaunchOnReload = 0;

            gun.shellsToLaunchOnFire = 3;
            Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
            gun.shellCasing = gun2.shellCasing;

            gun.gunClass = GunClass.RIFLE;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            SetupCollection();

        }
        public override void  OnPickup(GameActor owner)
        {
            AkSoundEngine.PostEvent("Play_Cav_pickup", base.gameObject);
            base.OnPickup(owner);
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
           
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            switch (soundInc)
            {
                case 1:
                    AkSoundEngine.PostEvent("Play_Cav_shoot_001", base.gameObject);
                    StartCoroutine(Firebullets());
                    soundInc++;
                    break;
                case 2:
                    AkSoundEngine.PostEvent("Play_Cav_shoot_002", base.gameObject);
                    StartCoroutine(Firebullets());
                    soundInc++;
                    break;
                case 3:
                    AkSoundEngine.PostEvent("Play_Cav_shoot_003", base.gameObject);
                    StartCoroutine(Firebullets());
                    soundInc = 1;
                    break;
            }

            base.PostProcessProjectile(projectile);
        }

        public IEnumerator Firebullets()
        {
            yield return new WaitForSeconds(.1f);
            PlayerController player = (PlayerController)this.gun.CurrentOwner;
            Projectile projectile = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, gun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : gun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = this.gun.CurrentOwner;
                component.Shooter = this.gun.CurrentOwner.specRigidbody;
                component.baseData.damage = 8 * player.stats.GetStatValue(PlayerStats.StatType.Damage);
                
                
            }

            yield return new WaitForSeconds(.1f);
            
            Projectile projectile2 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
            GameObject gameObject2 = SpawnManager.SpawnProjectile(projectile2.gameObject, gun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : gun.CurrentAngle), true);
            Projectile component2 = gameObject2.GetComponent<Projectile>();
            bool flag3 = component2 != null;
            if (flag3)
            {
                component2.Owner = this.gun.CurrentOwner;
                component2.Shooter = this.gun.CurrentOwner.specRigidbody;
                component2.baseData.damage = 8 * player.stats.GetStatValue(PlayerStats.StatType.Damage);


            }
        }
        private bool HasReloaded;
        public bool HasParried;
        public bool leftOffset = false;
        public bool reloadDelay = false;
        
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
                    this.HasParried = false;
                    this.reloadDelay = false;
                       
                    
                }

                if (gun.IsReloading && reloadDelay == true)// press R to parry and skip reload
                {
                    PlayerController player = (PlayerController)this.gun.CurrentOwner;
                    if (KeyDown(GungeonActions.GungeonActionType.Reload, (PlayerController)this.gun.CurrentOwner) || KeyDown(GungeonActions.GungeonActionType.Shoot, (PlayerController)this.gun.CurrentOwner))
                    {
                        
                        if (HasParried == false)
                        {
                            
                            if (player.CurrentGun.CurrentAngle <= 90 && player.CurrentGun.CurrentAngle >= -90)  // right
                            {
                                //do right parry ani
                                leftOffset = false;
                                StartCoroutine(rightanim());

                            }
                            else // NOT right
                            {
                                //do left parry ani
                                leftOffset = true;
                                StartCoroutine(leftanim());
                            }

                            this.HasParried = true;
                            //do invuln and parry timing
                            StartCoroutine(ParryProcess(player));

                        }
                    }

                    if (this.gun.GetComponent<GunSpecialStates>().successfullBlocks > 0)
                    {
                        
                        AkSoundEngine.PostEvent("Play_Parry_Success", base.gameObject);
                        gun.ForceImmediateReload();
                        gun.GetComponent<GunSpecialStates>().successfullBlocks = 0;


                    }
                    if(LastChargeLevel < 0 || LastChargeLevel > 8)
                    {
                        LastChargeLevel = 0;
                    }
                    player.StartCoroutine(ShowChargeLevel(player, LastChargeLevel));

                }

            }

        }

        private IEnumerator rightanim()
        {
            yield return new WaitForSeconds(.001f);
            LastChargeLevel = 5;
            yield return new WaitForSeconds(.05f);
            LastChargeLevel = 6;
            yield return new WaitForSeconds(.05f);
            LastChargeLevel = 7;
            yield return new WaitForSeconds(.05f);
            LastChargeLevel = 8;
            yield return new WaitForSeconds(.1f);
            LastChargeLevel = 0;
            leftOffset = false;
        }

        private IEnumerator leftanim()
        {
            yield return new WaitForSeconds(.001f);
            LastChargeLevel = 1;
            yield return new WaitForSeconds(.05f);
            LastChargeLevel = 2;
            yield return new WaitForSeconds(.05f);
            LastChargeLevel = 3;
            yield return new WaitForSeconds(.01f);
            LastChargeLevel = 4;
            yield return new WaitForSeconds(.1f);
            LastChargeLevel = 0;
            leftOffset = false;
        }

        public IEnumerator ParryProcess(PlayerController player)
        {
            yield return new WaitForSeconds(.01f);

            Projectile projectile1 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, this.gun.CurrentOwner.transform.position + new Vector3(.5f,.5f,.0f), Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : this.gun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            component.AdditionalScaleMultiplier = .001f;
            component.Owner = this.gun.CurrentOwner;
            component.baseData.damage = 15;
            ProjectileSlashingBehaviour slashy = component.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            slashy.SlashRange = 2f;
            slashy.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
            slashy.SlashDimensions = 60;
            slashy.SlashVFX.type = VFXPoolType.None;

        }

        public int soundInc = 1;
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_Cav_reload", base.gameObject);
                AkSoundEngine.PostEvent("Play_Cav_reload", base.gameObject);
                soundInc = 1;
                gun.GetComponent<GunSpecialStates>().successfullBlocks = 0;
                StartCoroutine(slightparydelay());
            }

        }

        private IEnumerator slightparydelay()
        {
            yield return new WaitForSeconds(.1f);
            reloadDelay = true;
        }

        public bool KeyDown(GungeonActions.GungeonActionType action, PlayerController user)
        {
            if(!GameManager.Instance.IsLoadingLevel && user != null)
            {
                return BraveInput.GetInstanceForPlayer(user.PlayerIDX).ActiveActions.GetActionFromType(action).WasPressed;
            }
            else
            {
                return false;
            }
            
        }

        public List<GameObject> extantSprites;
        private static tk2dSpriteCollectionData GunVFXCollection;
        private static GameObject VFXScapegoat;

        private static int Meter1ID;
        private static int Meter2ID;
        private static int Meter3ID;
        private static int Meter4ID;
        private static int Meter5ID;
        private static int Meter6ID;
        private static int Meter7ID;
        private static int Meter8ID;
        private static int Meter9ID;


        private static void SetupCollection()
        {
            VFXScapegoat = new GameObject();
            UnityEngine.Object.DontDestroyOnLoad(VFXScapegoat);
            Cav.GunVFXCollection = SpriteBuilder.ConstructCollection(VFXScapegoat, "CavVFX_Collection");
            UnityEngine.Object.DontDestroyOnLoad(Cav.GunVFXCollection);

            Meter1ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Parry/Cav_parryL_001", Cav.GunVFXCollection); 
            Meter2ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Parry/Cav_parryL_002", Cav.GunVFXCollection); 
            Meter3ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Parry/Cav_parryL_003", Cav.GunVFXCollection); 
            Meter4ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Parry/Cav_parryL_004", Cav.GunVFXCollection); 
            Meter5ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Parry/Cav_parryR_001", Cav.GunVFXCollection);
            Meter6ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Parry/Cav_parryR_002", Cav.GunVFXCollection);
            Meter7ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Parry/Cav_parryR_003", Cav.GunVFXCollection); 
            Meter8ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Parry/Cav_parryR_004", Cav.GunVFXCollection); 
            Meter9ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Parry/Cav_parry_000", Cav.GunVFXCollection); 

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
            newSprite.transform.localScale *= 1.5f;
            if (leftOffset)
            {
                newSprite.transform.position = (gunOwner.transform.position + new Vector3(-1f, 0f));
            }
            else
            {
                newSprite.transform.position = (gunOwner.transform.position + new Vector3(1f, 0f));
            }
            tk2dSprite m_ItemSprite = newSprite.AddComponent<tk2dSprite>();
            extantSprites.Add(newSprite);
            int spriteID = -1;
            switch (chargeLevel)
            {

                case 0:
                    spriteID = Meter9ID;
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
                

            }
            m_ItemSprite.SetSprite(Cav.GunVFXCollection, spriteID);
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
            yield return new WaitForSeconds(.1f);
            if (newSprite)
            {
                extantSprites.Remove(newSprite);
                Destroy(newSprite.gameObject);
            }
            yield break;
        }



        public int LastChargeLevel = 0;
    }
}