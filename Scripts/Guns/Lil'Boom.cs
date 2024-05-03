using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;

using HutongGames.PlayMaker.Actions;

namespace Knives
{
    class Lil_Boom :AdvancedGunBehaviour
    {

        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("lil Boom", "lil");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:lil_boom", "ski:lil_boom");
            gun.gameObject.AddComponent<Lil_Boom>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("You Are Already Dead.");
            gun.SetLongDescription("This cursed blade once struck down the entire guyon dynasty in a single strike. After which it and its ruler grew to power until" +
                " the day that the blade became tired of its weilder's peaceful nature and turned on him seeking out a new host to give it the bloodshed it required." +
                " It somehow made its way to the gungeon the only place that could feed its need for bloodshed.\n\n" +
                "___________________________________________\n" +
                "Damage increses the longer the charge is held. Reflects projectiles in a radius and slows time." +
                "\n\n\n - Knife_to_a_Gunfight");
            gun.SetupSprite(null, "lil_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 24);

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(541) as Gun, false, false);
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.DefaultModule.preventFiringDuringCharge = true;
            Gun gun3 = (Gun)ETGMod.Databases.Items["wonderboy"];
            gun.gunClass = GunClass.SILLY;
            gun.muzzleFlashEffects = gun3.muzzleFlashEffects;
            gun.reloadTime = 5f;
            gun.DefaultModule.cooldownTime = .0f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.S;
            gun.encounterTrackable.EncounterGuid = "teleports behind you";
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            
            projectile.baseData.damage = 4f;
            projectile.baseData.speed = 1f;
            projectile.baseData.range = .001f;
            
            projectile.transform.parent = gun.barrelOffset;
            projectile.AppliesBleed = true;
           
            

            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }
       

       

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            
           

        }
        int effects_controller = 0;
        int multiplier = 0;
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
            if (player.IsInCombat)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                if (effects_controller == 1)
                {
                    RadialSlowInterface the_big_slow = new RadialSlowInterface();
                    the_big_slow.RadialSlowHoldTime = .75f;
                    the_big_slow.RadialSlowOutTime = .25f;
                    the_big_slow.RadialSlowTimeModifier = 0f;
                    the_big_slow.DoesSepia = true;
                    the_big_slow.UpdatesForNewEnemies = true;
                    the_big_slow.RadialSlowInTime = 0f;
                    the_big_slow.DoRadialSlow(this.Owner.CenterPosition, this.gun.CurrentOwner.GetAbsoluteParentRoom());
                    RoomHandler room = player.CurrentRoom;


                    foreach (var enemy in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                    {
                        if(multiplier > 1500)
                        {
                            enemy.healthHaver.ApplyDamage(80, Vector2.zero, "bracer", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, true);
                            this.DoSafeExplosion(enemy.specRigidbody.UnitCenter);
                        }
                        else
                        {
                            enemy.healthHaver.ApplyDamage(2 * multiplier / 40, Vector2.zero, "bracer", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, true);
                            this.DoSafeExplosion(enemy.specRigidbody.UnitCenter);
                        }
                        if (enemy.healthHaver.IsBoss)
                        {
                            enemy.healthHaver.ApplyDamage(45, Vector2.zero, "bracer", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, true);
                            this.DoSafeExplosion(enemy.specRigidbody.UnitCenter);
                            this.DoSafeExplosion(enemy.specRigidbody.UnitCenter);
                        }
                    }

                    foreach (var projectile in GetBullets())
                    {
                        if (projectile.Owner.healthHaver.IsBoss)
                        {
                            PassiveReflectItem.ReflectBullet(projectile, true, this.Owner, 15f, 1f, 15f, 0f);
                        }
                        else
                        {
                            PassiveReflectItem.ReflectBullet(projectile, true, this.Owner, 15f, 1f, 3f, 0f);
                        }
                    }
                    effects_controller = 0;
                    multiplier = 0;
                }
                base.OnFinishAttack(player, gun);
            }
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
            damageRadius = 2.5f,
            damageToPlayer = 0f,
            doDamage = false,
            damage = 0f,
            doDestroyProjectiles = true,
            doForce = false,
            debrisForce = 30f,
            preventPlayerForce = true,
            explosionDelay = 0.1f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = true
        };
        private  List<Projectile> GetBullets()
        {
            Vector2 Standing = this.gun.CurrentOwner.CenterPosition;
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                if (projectile && projectile.sprite && !projectile.ImmuneToBlanks && !projectile.ImmuneToSustainedBlanks)
                {
                    if (projectile.Owner != null)
                    {
                        if (projectile.collidesWithEnemies = false || projectile.isFakeBullet || projectile.Owner is AIActor || (projectile.Shooter != null && projectile.Shooter.aiActor != null) || projectile.ForcePlayerBlankable || projectile.IsBulletScript)
                        {
                            if (Vector2.Distance(projectile.LastPosition, Standing) <= 2.5)
                            {
                                list.Add(projectile);
                            }
                        }
                    }
                }
            }
            return list;
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

                if (this.gun.HasChargedProjectileReady)
                {
                    effects_controller = 1;
                    RadialSlowInterface the_little_slow = new RadialSlowInterface();
                    the_little_slow.RadialSlowHoldTime = .0f;
                    the_little_slow.RadialSlowOutTime = .25f;
                    the_little_slow.RadialSlowInTime = 5;
                    the_little_slow.RadialSlowTimeModifier = .4f;
                    the_little_slow.DoesSepia = false;
                    the_little_slow.UpdatesForNewEnemies = true;

                    the_little_slow.RadialSlowInTime = 0f;
                    the_little_slow.DoRadialSlow(this.Owner.CenterPosition, this.gun.CurrentOwner.GetAbsoluteParentRoom());


                    multiplier++;
                }
            }

           
            base.Update();
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
