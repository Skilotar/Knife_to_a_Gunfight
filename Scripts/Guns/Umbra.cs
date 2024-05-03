
using System;
using ItemAPI;
using Dungeonator;
using Gungeon;
using SaveAPI;

using Object = UnityEngine.Object;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Knives
{
    class Umbra_main : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Umbra", "Umbra");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:umbra", "ski:umbra");
            gun.gameObject.AddComponent<Umbra_main>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("GunBrella");
            gun.SetLongDescription("Charge to block incoming shots.\n\n" +
                "" +
                "A 6 shot rifle consealed in plain sight by a small sheet of high flexability kevlar. This weapon was used to assassinate the ruthless Queen Elizabeth the LXI on a particularly rainy day in New New England. " +
                "Fortunately for the resistance spy the guards did not find it suspicious that the shadowy figure lurking through the crowd was deciding to get soaked rather then use the umbrella he was carrying." +

                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "Umbra_idle_001", 8);

            gun.SetAnimationFPS(gun.chargeAnimation, 8);
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(9) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 5;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.RIFLE;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 1.2f;
            gun.DefaultModule.numberOfShotsInClip = 6;
            gun.DefaultModule.cooldownTime = .6f;
            gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 2, StatModifier.ModifyMethod.ADDITIVE);

            gun.SetBaseMaxAmmo(450);
            gun.ammo = 450;
            gun.quality = PickupObject.ItemQuality.B;
            gun.PreventOutlines = true;

            GunSpecialStates spec = gun.gameObject.GetOrAddComponent<GunSpecialStates>();

            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 18f;
            projectile.baseData.speed *= 1f;
            projectile.baseData.range = 20f;
            projectile.baseData.force = 10;
            projectile.AdditionalScaleMultiplier = .7f;
            gun.barrelOffset.transform.localPosition = new Vector3(2f, 1f, 0f);
            gun.carryPixelOffset = new IntVector2(6, 0);

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f,
                AmmoCost = 1,


            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .15f,
                AmmoCost = 1,

            };
            
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2,
                
            };


            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            Umbra_main.BaseID = gun.PickupObjectId;
        }

        public static int BaseID;


        public System.Random rng = new System.Random();
        

        
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
           
            
            base.OnFinishAttack(player, gun);
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            AkSoundEngine.PostEvent("Play_WPN_m1rifle_shot_01", base.gameObject);
            base.PostProcessProjectile(projectile);
        }
        
        private bool HasReloaded;
        public bool doingCo = false;
       
        public override void  Update()
        {
            PlayerController player = this.gun.CurrentOwner as PlayerController;
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
                
                if (this.gun.HasChargedProjectileReady && doingCo == false && this.gun.GetChargeFraction() == 1)
                {
                    StartCoroutine(HandleBlocking());
                }


            }




        }

        public IEnumerator HandleBlocking()
        {
            doingCo = true;

            yield return new WaitForSeconds(.3f);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            doslashy();
            this.gun.LoseAmmo(1);

            doingCo = false;
        }

        public void doslashy()
        {
            Projectile projectile1 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, this.gun.CurrentOwner.CenterPosition, Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : this.gun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            component.Owner = this.gun.CurrentOwner;
            component.Shooter = this.Owner.specRigidbody;
            component.baseData.damage = 0;
            ProjectileSlashingBehaviour slasher = component.gameObject.AddComponent<ProjectileSlashingBehaviour>();
            slasher.SlashDamageUsesBaseProjectileDamage = false;
            slasher.SlashDimensions = 40;
            slasher.DoSound = false;
            slasher.SlashRange = 3f;
            slasher.playerKnockback = 0;
            slasher.SlashDamage = 0;
            slasher.SlashVFX.type = VFXPoolType.None;
            slasher.InteractMode = SlashDoer.ProjInteractMode.DESTROY;

            Projectile projectile2 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
            GameObject gameObject2 = SpawnManager.SpawnProjectile(projectile2.gameObject, this.gun.CurrentOwner.CenterPosition, Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : this.gun.CurrentAngle), true);
            Projectile component2 = gameObject2.GetComponent<Projectile>();
            component2.Owner = this.gun.CurrentOwner;
            component2.Shooter = this.Owner.specRigidbody;
            component2.baseData.damage = 0;
            ProjectileSlashingBehaviour slasher2 = component2.gameObject.AddComponent<ProjectileSlashingBehaviour>();
            slasher2.SlashDamageUsesBaseProjectileDamage = false;
            slasher2.SlashDimensions = 60;
            slasher2.DoSound = false;
            slasher2.SlashRange = 2f;
            slasher2.playerKnockback = 0;
            slasher2.SlashDamage = 0;
            slasher2.SlashVFX.type = VFXPoolType.None;
            slasher2.InteractMode = SlashDoer.ProjInteractMode.DESTROY;

            if((this.gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy("Rain Rain Go Away"))
            {
                slasher.DeflectionDegree = 80;
                slasher2.DeflectionDegree = 100;
                slasher.InteractMode = SlashDoer.ProjInteractMode.DEFLECT;
                slasher2.InteractMode = SlashDoer.ProjInteractMode.DEFLECT;
                this.gun.LoseAmmo(2);
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {


                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                AkSoundEngine.PostEvent("Play_WPN_duelingpistol_reload_01", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);


                if (this.gun.ClipShotsRemaining == 0 && (this.gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy("It's Raining Kin") && player.IsInCombat)
                {
                    string Kin = "01972dee89fc4404a5c408d50007dad5";
                    AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(Kin);
                    RoomHandler room = player.GetAbsoluteParentRoom();
                    IntVector2 vector = (IntVector2)room.GetRandomAvailableCell();
                    Vector2 Posi = vector.ToVector2();
                    AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, player.CenterPosition, room, true, (AIActor.AwakenAnimationType)2, true);
                    PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
                    aiactor.CanTargetEnemies = true;
                    aiactor.CanTargetPlayers = false;
                    aiactor.IsHarmlessEnemy = true;
                    aiactor.CanDropCurrency = false;

                    aiactor.IgnoreForRoomClear = true;
                    aiactor.MovementSpeed = 5.95f;
                    aiactor.CompanionOwner = player;
                    aiactor.IsBuffEnemy = true;
                    aiactor.isPassable = true;
                    aiactor.gameObject.AddComponent<KillOnRoomClear>();
                    aiactor.reinforceType = (AIActor.ReinforceType)2;
                    aiactor.HandleReinforcementFallIntoRoom(0.1f);

                    aiactor.specRigidbody.AddCollisionLayerIgnoreOverride(
                        CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.PlayerHitBox,
                        CollisionLayer.Projectile, CollisionLayer.PlayerCollider, CollisionLayer.PlayerBlocker, CollisionLayer.BeamBlocker)
                        );
                    aiactor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable, CollisionLayer.Trap));
                    if (aiactor.healthHaver != null)
                    {
                        aiactor.healthHaver.PreventAllDamage = true;
                    }
                    if (aiactor.bulletBank != null)
                    {
                        AIBulletBank bulletBank = aiactor.bulletBank;
                        bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.OnPostProcessProjectile2));
                    }
                    if (aiactor.aiShooter != null)
                    {
                        AIShooter aiShooter = aiactor.aiShooter;
                        aiShooter.PostProcessProjectile = (Action<Projectile>)Delegate.Combine(aiShooter.PostProcessProjectile, new Action<Projectile>(this.OnPostProcessProjectile2));
                    }

                }

            }

        }

        public void OnPostProcessProjectile2(Projectile proj)
        {
            try
            {
                if (proj.Owner is AIActor && !(proj.Owner as AIActor).CompanionOwner)
                {
                    return; //to prevent the OnPostProcessProjectile from affecting enemies projectiles
                }
                

                proj.Owner = this.gun.CurrentOwner; //to allow the projectile damage modif, otherwise it stays at 10 for some reasons

                proj.baseData.damage = 1;
                if (this.gun.CurrentOwner is PlayerController)
                {
                    proj.baseData.damage *= (this.gun.CurrentOwner as PlayerController).stats.GetStatValue(PlayerStats.StatType.Damage) + .5f;
                }
                if (proj.IsBlackBullet)
                {
                    proj.baseData.damage *= 2.5f;
                }
                proj.collidesWithPlayer = false;
                proj.collidesWithEnemies = true;
                //proj.sprite.color = UnityEngine.Color.yellow;
                proj.TreatedAsNonProjectileForChallenge = true;
                proj.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(proj.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));
            }
            catch (Exception e)
            {
                Tools.Print("Copper OnPostProcessProjectile", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }
        private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            bool flag = otherRigidbody && otherRigidbody.healthHaver && otherRigidbody.aiActor && otherRigidbody.aiActor.CompanionOwner;
            if (flag)
            {
                float damage = myRigidbody.projectile.baseData.damage;
                myRigidbody.projectile.baseData.damage = 0f;
                GameManager.Instance.StartCoroutine(NewNewCopperChariot.ChangeProjectileDamage(myRigidbody.projectile, damage));
            }
        }
        public static IEnumerator ChangeProjectileDamage(Projectile bullet, float oldDamage)
        {
            yield return new WaitForSeconds(0.1f);
            bool flag = bullet != null;
            if (flag)
            {
                bullet.baseData.damage = oldDamage;
            }
            yield break;
        }
    }
}