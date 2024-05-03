using System;
using System.Collections;

using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using System.Runtime.InteropServices;
using System.Text;
using Brave;
using Brave.BulletScript;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using UnityEngine;

namespace Knives
{
    class FireStrike : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Crusader mk II", "Firestrike");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:crusader_mk_ii", "ski:crusader_mk_ii");
            gun.gameObject.AddComponent<FireStrike>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Precision German Engineering");
            gun.SetLongDescription("The faster the user is moving the more powerful the projectile will be!\n\n" +
                "" +
                "An early version of the main thruster for many of the hegemony of man's smaller ships. This version was not strong enough for the hegemony's " +
                "taste so they were relegated to being used as weapons." +
                
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "Firestrike_idle_001", 8);
            gun.SetAnimationFPS(gun.idleAnimation, 3);
            gun.SetAnimationFPS(gun.chargeAnimation, 7);
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 3);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(125) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 4;
            gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, .7f, StatModifier.ModifyMethod.ADDITIVE);


            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.FIRE;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 1.5f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.DefaultModule.cooldownTime = .5f;


            gun.SetBaseMaxAmmo(80);
            
            gun.quality = PickupObject.ItemQuality.B;
            //gun.encounterTrackable.EncounterGuid = "Bring me another";

            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(1.5f, 0f, 0f);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "flare";
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 10f;
            projectile.baseData.speed = 12f;
            projectile.baseData.range *= 1.5f;
            projectile.baseData.force = 5;
            projectile.BossDamageMultiplier = 2;
            projectile.AppliesFire = true;
            projectile.FireApplyChance = 100;
            projectile.AdditionalScaleMultiplier = 2.5f;
            projectile.gameObject.AddComponent<PlayerVelocityBuffProjectileModifier>();

            tk2dSpriteAnimationClip fireClip = gun.sprite.spriteAnimator.GetClipByName("Firestrike_charge");
            float[] offsetsX = new float[] { 0f, 0f, 0f, -.4f, -1.2f, -1.1f, -1.1f, -1.1f };
            float[] offsetsY = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, -0.0f, 0.0f, 0.0f, 0.0f };

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

            tk2dSpriteAnimationClip fireClip2 = gun.sprite.spriteAnimator.GetClipByName("Firestrike_fire");
            float[] offsetsX2 = new float[] { 1f, 2.4f, 2.2f, 2.1f, 2f , 1.9f, 2f, };
            float[] offsetsY2 = new float[] { -1.8f, -1.6f, .5f, .6f, .7f, .8f, .7f, };

            for (int i = 0; i < offsetsX2.Length && i < offsetsY2.Length && i < fireClip2.frames.Length; i++)
            {
                int id = fireClip2.frames[i].spriteId;
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY2[i];
            }

            tk2dSpriteAnimationClip fireClip3 = gun.sprite.spriteAnimator.GetClipByName("Firestrike_reload");
            float[] offsetsX3 = new float[] {  0f, 0f, 0f , 0f };
            float[] offsetsY3 = new float[] {  0.0f, 0.0f, -0.0f, 0.0f };

            for (int i = 0; i < offsetsX3.Length && i < offsetsY3.Length && i < fireClip3.frames.Length; i++)
            {
                int id = fireClip3.frames[i].spriteId;
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY3[i];
            }

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .3f,
                AmmoCost = 0,


            };
           
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
               

            };
            SetupCollection();
            //MiscToolMethods.TrimAllGunSprites(gun);
            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
           
        }


        public override void OnPostFired(PlayerController player, Gun gun)
        {
           
            gun.PreventNormalFireAudio = true;
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            PlayerController player = (PlayerController)this.gun.CurrentOwner;
            Projectile projectile2 = ((Gun)ETGMod.Databases.Items[384]).DefaultModule.projectiles[0];
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag = component != null;
            if (flag)
            {
                component.Owner = player;
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                ProjectileSlashingBehaviour slasher = component.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDamageUsesBaseProjectileDamage = false;
                slasher.SlashDimensions = 90;
                slasher.SlashRange = 4.5f;
                slasher.SlashVFX.type = VFXPoolType.None;
                slasher.SlashDamage = 10 + SpeedMult / 20;
                slasher.playerKnockback = 0;
                slasher.soundToPlay = "Play_ENM_gunnut_swing_01";
                slasher.InteractMode = SlashDoer.ProjInteractMode.REFLECT;

            }

            base.PostProcessProjectile(projectile);
            
        }

        private int SpeedMult = 0;
        private bool HasReloaded;
        public bool startup = false;
        public bool flameon = true;
        public bool resist = false;
        public PlayerController KnownOwner;
        public override void  Update()
        {
           
            if (gun.CurrentOwner)
            {
                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                KnownOwner = player;
                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
                if(gun.GetChargeFraction() > .01f)
                {
                    
                    player.healthHaver.damageTypeModifiers.Add(this.m_fireImmunity);

                    Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                    player.knockbackDoer.ApplyKnockback(vector, .003f * SpeedMult);
                    if(SpeedMult < 550)
                    {
                        SpeedMult++;
                        SpeedMult++;
                    }
                    else
                    {
                        if(SpeedMult >= 550)
                        {
                            player.healthHaver.damageTypeModifiers.Add(this.m_fireImmunity);
                            Projectile proj = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, player.CenterPosition, player, this.gun);
                            ProjectileSlashingBehaviour slasher = proj.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                            slasher.SlashDamageUsesBaseProjectileDamage = false;
                            
                            slasher.SlashDimensions = 90;
                            slasher.SlashRange = 2.5f;
                            slasher.SlashVFX.type = VFXPoolType.None;
                            slasher.soundToPlay = null;
                            slasher.SlashDamage = 0;
                            slasher.playerKnockback = 2;
                            slasher.slashKnockback = 0;
                            
                            slasher.InteractMode = SlashDoer.ProjInteractMode.DESTROY;


                            GoopDefinition fire = PickupObjectDatabase.GetById(242).GetComponent<DirectionalAttackActiveItem>().goopDefinition;
                            fire.lifespan = 2.5f;
                            DeadlyDeadlyGoopManager goop = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(fire);
                            goop.TimedAddGoopCircle(this.Owner.sprite.WorldBottomCenter, .75f, .05f, true);

                            flameon = true;
                        }
                    }
                    
                }
                else
                {
                    SpeedMult = 0;
                    if (flameon)
                    {
                        player.healthHaver.damageTypeModifiers.Remove(this.m_fireImmunity);
                        flameon = false;
                    }
                }
                int speedlvl = (int)player.AverageVelocity.magnitude;
                if(speedlvl <= 14)
                {
                    player.StartCoroutine(ShowChargeLevel(player, speedlvl));
                }
                if(speedlvl > 14)
                {
                    player.StartCoroutine(ShowChargeLevel(player, 14));
                }

                if(startup == false)
                {
                    this.m_fireImmunity = new DamageTypeModifier();
                    this.m_fireImmunity.damageMultiplier = 0f;
                    this.m_fireImmunity.damageType = CoreDamageTypes.Fire;

                    this.KnownOwner.gameObject.GetOrAddComponent<TrackHighSpeed>();

                    startup = true;
                }
                if(player != KnownOwner)
                {
                    startup = false;
                }
            }
           


        }
        
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {


                HasReloaded = false;
              
                base.OnReloadPressed(player, gun, bSOMETHING);


            }

        }


        public List<GameObject> extantSprites;
        private static tk2dSpriteCollectionData GunVFXCollection;
        private static GameObject VFXFirestrike;
        public DamageTypeModifier m_fireImmunity;
        private static int Meter1ID;
        private static int Meter2ID;
        private static int Meter3ID;
        private static int Meter4ID;
        private static int Meter5ID;

        private static void SetupCollection()
        {
            VFXFirestrike = new GameObject();
            UnityEngine.Object.DontDestroyOnLoad(VFXFirestrike);
            FireStrike.GunVFXCollection = SpriteBuilder.ConstructCollection(VFXFirestrike, "FirestrikeVFX_collection");
            UnityEngine.Object.DontDestroyOnLoad(FireStrike.GunVFXCollection);

            Meter1ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Firestrike/speed_20", FireStrike.GunVFXCollection); 
            Meter2ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Firestrike/speed_40", FireStrike.GunVFXCollection); 
            Meter3ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Firestrike/speed_60", FireStrike.GunVFXCollection); 
            Meter4ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Firestrike/speed_80", FireStrike.GunVFXCollection);
            Meter5ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Firestrike/speed_top", FireStrike.GunVFXCollection);
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
                    spriteID = Meter1ID;
                    break;
                case 1:
                    spriteID = Meter1ID;
                    break;
                case 2:
                    spriteID = Meter1ID;
                    break;
                case 3:
                    spriteID = Meter2ID;
                    break;
                case 4:
                    spriteID = Meter2ID;
                    break;
                case 5:
                    spriteID = Meter2ID;
                    break;
                case 6:
                    spriteID = Meter3ID;
                    break;
                case 7:
                    spriteID = Meter3ID;
                    break;
                case 8:
                    spriteID = Meter3ID;
                    break;
                case 9:
                    spriteID = Meter4ID;
                    break;
                case 10:
                    spriteID = Meter4ID;
                    break;
                case 11:
                    spriteID = Meter4ID;
                    break;
                case 12:
                    spriteID = Meter5ID;
                    break;
                case 13:
                    spriteID = Meter5ID;
                    break;
                case 14:
                    spriteID = Meter5ID;
                    break;



            }
            m_ItemSprite.SetSprite(FireStrike.GunVFXCollection, spriteID);
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

