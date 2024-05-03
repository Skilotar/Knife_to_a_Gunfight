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

    public class Stop: AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Riot Hammer", "STOP");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:riot_hammer", "ski:riot_hammer");
            gun.gameObject.AddComponent<Stop>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("It Time To STOP");
            gun.SetLongDescription("Hit a Tipper to STOP your enemies in place. \n\n" +
                "A simple traffic control device torn from its position and reforged into a melee weapon." +
                "" +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "STOP_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            gun.SetAnimationFPS(gun.chargeAnimation, 6);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 3;
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);
            gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, .3f, StatModifier.ModifyMethod.ADDITIVE);
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.gunHandedness = GunHandedness.TwoHanded;
            Gun casey = (Gun)PickupObjectDatabase.GetById(541);
            gun.gunSwitchGroup = casey.gunSwitchGroup;
            gun.reloadTime = 0f;
            gun.DefaultModule.cooldownTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.InfiniteAmmo = true;
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.C;
            gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1, StatModifier.ModifyMethod.ADDITIVE);
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(15) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 8f;
            projectile.baseData.speed *= 1f;
           
            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .8f
            };
           
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
               
            };

            
            
            tk2dSpriteAnimationClip fireClip = gun.sprite.spriteAnimator.GetClipByName("STOP_charge");
            
            float[] offsetsX2 = new float[] { 0.0500f, -0.3000f, -0.4875f , -0.6125f };
            float[] offsetsY2 = new float[] { 0.0000f, 0.0625f, 0.3750f, 0.5000f };

            for (int i = 0; i < offsetsX2.Length && i < offsetsY2.Length && i < fireClip.frames.Length; i++)
            {
                int id = fireClip.frames[i].spriteId;
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX2[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY2[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX2[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY2[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX2[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY2[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX2[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY2[i];
            }
            tk2dSpriteAnimationClip fireClip3 = gun.sprite.spriteAnimator.GetClipByName("STOP_fire");
            float[] offsetsX3 = new float[] { -2.1250f, -1.6250f, -1.6250f };
            float[] offsetsY3 = new float[] { -0.6875f, -0.5625f, -0.5000f };
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
            gun.carryPixelOffset = new IntVector2(14, 0);



            
            gun.gunClass = GunClass.CHARGE;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            //MiscToolMethods.TrimAllGunSprites(gun);
            ID = gun.PickupObjectId;
        }

        public static int ID;

        public override void OnPostFired(PlayerController player, Gun gun)
        {

        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            AkSoundEngine.PostEvent("Play_ENM_chainknight_swing_01", base.gameObject);
            projectile.SuppressHitEffects = true;
            projectile.sprite.enabled = false;
            projectile.hitEffects.suppressMidairDeathVfx = true;
            projectile.DieInAir();
            StartCoroutine(slightslashdelay());
            base.PostProcessProjectile(projectile);
        }
        public IEnumerator slightslashdelay()
        {


            yield return new WaitForSeconds(.15f);
            Projectile projectile1 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, this.gun.CurrentOwner.transform.position + new Vector3(.5f,.5f,.0f), Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : this.gun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            component.AdditionalScaleMultiplier = .001f;
            component.Owner = this.gun.CurrentOwner;
            component.baseData.damage = 35;
            ProjectileSlashingBehaviour slashy = component.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            slashy.SlashRange = 4.7f;
            slashy.DoSound = false;
            slashy.SlashDimensions = 50;
            slashy.InteractMode = SlashDoer.ProjInteractMode.STOP;
            slashy.SlashVFX.type = VFXPoolType.None;
            slashy.OnSlashHitEnemy += Slashy_OnSlashHitEnemy;
            
            
           
        }

        private void Slashy_OnSlashHitEnemy(PlayerController player, AIActor hitEnemy, Vector2 forceDirection)
        {
            float dist = Vector2.Distance(player.CenterPosition, hitEnemy.CenterPosition);
            if (dist > 4)
            {
                GameActorSpeedEffect speed = new GameActorSpeedEffect();
                speed.duration = 6;
                speed.SpeedMultiplier = .5f;
                hitEnemy.ApplyEffect(speed, 4, null);
                hitEnemy.behaviorSpeculator.Stun(8, true);
                AkSoundEngine.PostEvent("Play_Metal_hit_002", base.gameObject);
                hitEnemy.healthHaver.ApplyDamage(25, Vector2.zero, "STOP tipper", CoreDamageTypes.Magic, DamageCategory.Normal, true, null, true);

                if (hitEnemy.healthHaver.IsBoss)
                {
                    
                    hitEnemy.healthHaver.ApplyDamage(50, Vector2.zero, "STOP tipper", CoreDamageTypes.Magic, DamageCategory.Normal, true, null, true);
                }
            }
            else
            {
                GameActorSpeedEffect speed = new GameActorSpeedEffect();
                speed.duration = 6;
                speed.SpeedMultiplier = .5f;
                hitEnemy.ApplyEffect(speed, 2, null);
                //hitEnemy.behaviorSpeculator.Stun(2, true);
                AkSoundEngine.PostEvent("Play_Metal_hit_001", base.gameObject);
            }
        }



        private bool HasReloaded;

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

             
            }

        }


    }
}

