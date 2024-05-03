using System;
using System.Collections;
using System.Collections.Generic;
using Gungeon;
using MonoMod;
using UnityEngine;
using Dungeonator;
using ItemAPI;
using Brave;
using MultiplayerBasicExample;

namespace Knives
{

    public class Bee : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Beetalion Combmander", "bee");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:beetalion_combmander", "ski:beetalion_combmander");
            gun.gameObject.AddComponent<Bee>();

            gun.SetShortDescription("Hold [Dodge] To Select");
            gun.SetLongDescription("Bee the leader your troups need you to bee! Select bee-tween a number of commands for the next group to follow. Have your bee's attack as normal or switch to plan-Bee and have them adopt a defensive tactic. You can target the bees manually or fire an I.C.Bee.M. for some focused fire! Good luck! The hive is counting on you!  \n\n" +
                "Attack mode: Standard bees with low damage. Cost 1 ammo.\n\n" +
                "Defend mode: Bigger bees that stay put and block bullets. Cost 3 ammo.\n\n" +
                "Target mode: Harder Hitting bees with almost no homing. Cost 2 ammo.\n\n" +
                "Rocket mode: Fires Rockets. Cost 5 ammo." +
                "\n\n\n - Knife_to_a_Gunfight");

            gun.SetupSprite(null, "bee_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 1);
            gun.SetAnimationFPS(gun.reloadAnimation, 18);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).loopStart = 13;


            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(14) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to bee.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 3f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = .1f;
            gun.DefaultModule.numberOfShotsInClip = 10;
            gun.SetBaseMaxAmmo(500);
            
            gun.gunClass = GunClass.SILLY;
            gun.quality = PickupObject.ItemQuality.B; // hahaha get it bee 
            //gun.encounterTrackable.EncounterGuid = "ski:beestaff";

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 2f;
            projectile.baseData.speed *= 1f;
            projectile.transform.parent = gun.barrelOffset;

            tk2dSpriteAnimationClip fireClip = gun.sprite.spriteAnimator.GetClipByName(gun.shootAnimation);
            float[] offsetsX = new float[] { 0.0f };
            float[] offsetsY = new float[] { 0.4f };

            for (int i = 0; i < offsetsX.Length && i < offsetsY.Length && i < fireClip.frames.Length; i++)
            {
                int id = fireClip.frames[i].spriteId;
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY[i];
            }

            SetupCollection();
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }

        public static int ID;
        public override void OnPickup(GameActor owner)
        {
            m_player = (PlayerController)this.Owner;
            base.OnPickup(owner);
        }


        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_bees_impact_01", base.gameObject);
           
        }
        public OrbitProjectileMotionModule spincheck;
        public float raduis = 20f;
        public override void PostProcessProjectile(Projectile projectile)
        {
            PlayerController player = this.gun.CurrentOwner as PlayerController;
            base.PostProcessProjectile(projectile);
            if(Current_Select == 0)// attack
            {
                projectile.baseData.damage = 5;
                projectile.HasDefaultTint = true;
                projectile.DefaultTintColor = UnityEngine.Color.red;

            }
            if (Current_Select == 1)// defend
            {
                projectile.baseData.damage = 0;
                projectile.AdditionalScaleMultiplier = 3.5f;
                HungryProjectileModifier hunger = projectile.gameObject.GetOrAddComponent<HungryProjectileModifier>();
                hunger.HungryRadius = .2f;
                hunger.MaximumBulletsEaten = 3;
                projectile.baseData.speed = .1f;

                gun.LoseAmmo(2);

                gun.ClipShotsRemaining = gun.ClipShotsRemaining - 2;
            }
            if (Current_Select == 2)// target
            {
                projectile.baseData.damage = 10;
                projectile.baseData.speed = 20;
                HomingModifier home = projectile.gameObject.GetOrAddComponent<HomingModifier>();
              
                home.HomingRadius = 3;
                home.AngularVelocity = 300;
                projectile.HasDefaultTint = true;
                projectile.AdditionalScaleMultiplier = 1.5f;
                projectile.DefaultTintColor = UnityEngine.Color.green;
                gun.LoseAmmo(1);

                gun.ClipShotsRemaining = gun.ClipShotsRemaining - 1;

            }
            if (Current_Select == 3)// rocket
            {
                projectile.DieInAir();
                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[92]).DefaultModule.projectiles[0];
                projectile2.baseData.damage = 1f;
                GameObject gameObject2 = SpawnManager.SpawnProjectile(projectile2.gameObject, this.Owner.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.Owner.CurrentGun == null) ? 0f : this.Owner.CurrentGun.CurrentAngle), true);
                Projectile component2 = gameObject2.GetComponent<Projectile>();
                gun.LoseAmmo(4);
                
                gun.ClipShotsRemaining = gun.ClipShotsRemaining - 4;
                bool flag = component2 != null;

                if (flag)
                {
                    component2.Owner = this.Owner;
                    component2.Shooter = this.Owner.specRigidbody;
                    component2.AdditionalScaleMultiplier = .5f;
                    if (UnityEngine.Random.Range(1, 11) == 1)
                    {
                        player.DoPostProcessProjectile(component2);
                    }

                }
            }

        }

        
        private bool HasReloaded;
        public bool locked = false;
        public bool shown = false;
       
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
                PlayerController player = this.gun.CurrentOwner as PlayerController;
               
                if (Key(GungeonActions.GungeonActionType.DodgeRoll) && KeyTime(GungeonActions.GungeonActionType.DodgeRoll) > .33f && !locked)
                {
                    shown = true;
                    locked = true;
                }

                if (!Key(GungeonActions.GungeonActionType.DodgeRoll) && locked == true)
                {
                    locked = false;
                    shown = false;
                }
                    

                if (shown)
                {
                   
                    if(player.CurrentGun.CurrentAngle <=45 && player.CurrentGun.CurrentAngle >= -45)// right
                    {
                        Current_Select = 0;
                    }
                    if(player.CurrentGun.CurrentAngle > 45 && player.CurrentGun.CurrentAngle <= 135)// up
                    {
                        Current_Select = 1;
                    }
                    if(player.CurrentGun.CurrentAngle > 135 || player.CurrentGun.CurrentAngle <= -135)// left
                    {
                        Current_Select = 2;
                    }
                    if(player.CurrentGun.CurrentAngle < -45 && player.CurrentGun.CurrentAngle >= -135)// down
                    {
                        Current_Select = 3;
                    }

                   
                    player.StartCoroutine(ShowChargeLevel(player, Current_Select));

                }


            }

        }

        public int Current_Select = 0;

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);



            }
            
        }


        PlayerController m_player;

        public float KeyTime(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel && (this.gun.CurrentOwner as PlayerController) != null)
            {
                return BraveInput.GetInstanceForPlayer((this.gun.CurrentOwner as PlayerController).PlayerIDX).ActiveActions.GetActionFromType(action).PressedDuration;
            }
            else
            {
                return 0;
            }

        }
        public bool KeyDown(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel && (this.gun.CurrentOwner as PlayerController) != null)
            {
                return BraveInput.GetInstanceForPlayer((this.gun.CurrentOwner as PlayerController).PlayerIDX).ActiveActions.GetActionFromType(action).WasPressed;
            }
            else
            {
                return false;
            }

        }
        public bool Key(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel && (this.gun.CurrentOwner as PlayerController) != null)
            {
                return BraveInput.GetInstanceForPlayer((this.gun.CurrentOwner as PlayerController).PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
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
     
        private static void SetupCollection()
        {
            VFXScapegoat = new GameObject();
            UnityEngine.Object.DontDestroyOnLoad(VFXScapegoat);
            Bee.GunVFXCollection = SpriteBuilder.ConstructCollection(VFXScapegoat, "LeftyVFX_Collection");
            UnityEngine.Object.DontDestroyOnLoad(Bee.GunVFXCollection);
            
            Meter1ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Bee/bee_attack_radial", Bee.GunVFXCollection); // attack
            Meter2ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Bee/bee_defend_radial", Bee.GunVFXCollection); // defend
            Meter3ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Bee/bee_target_radial", Bee.GunVFXCollection); // guide
            Meter4ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Bee/bee_rocket_radial", Bee.GunVFXCollection); // rocket
           
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
            newSprite.transform.position = (gunOwner.transform.position + new Vector3(0.7f, -1.5f));
            tk2dSprite m_ItemSprite = newSprite.AddComponent<tk2dSprite>();
            extantSprites.Add(newSprite);
            int spriteID = -1;
            switch (chargeLevel)
            {
         
                case 0:
                    spriteID = Meter1ID;
                    break;
                case 1:
                    spriteID = Meter2ID;
                    break;
                case 2:
                    spriteID = Meter3ID;
                    break;
                case 3:
                    spriteID = Meter4ID;
                    break;
                
                   
            }
            m_ItemSprite.SetSprite(Bee.GunVFXCollection, spriteID);
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

      

        public int LastChargeLevel = 0;
    }
}
