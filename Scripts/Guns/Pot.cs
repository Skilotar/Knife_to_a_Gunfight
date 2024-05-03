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
using System.CodeDom;

namespace Knives
{
    class Pot : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Jar Pitcher", "BeamOS");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:jar_pitcher", "ski:jar_pitcher");
            gun.gameObject.AddComponent<Pot>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Clod Hopper");
            gun.SetLongDescription("Shoots Jars.\n\n" +
                "An oozing clay pot gurgling with thick black sentient tar. \n" +
                "This tar uses all sorts of containers as a protective shell but prefers clay pots as they are easier to hang onto." +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "BeamOS_idle_001", 4);
            gun.SetAnimationFPS(gun.chargeAnimation, 5);
            gun.SetAnimationFPS(gun.shootAnimation, 6);
            
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 2;


            gun.AddProjectileModuleFrom((PickupObjectDatabase.GetById(15) as Gun), true, true);
            

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.CHARGE;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 0f;

            gun.DefaultModule.cooldownTime = .5f;
            gun.DefaultModule.numberOfShotsInClip = int.MaxValue;
            gun.SetBaseMaxAmmo(120);

            gun.quality = PickupObject.ItemQuality.B;
            gun.carryPixelOffset = new IntVector2(7, 0);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "pot";


            gun.muzzleFlashEffects = ((Gun)PickupObjectDatabase.GetById(37)).muzzleFlashEffects;
            gun.gunSwitchGroup = ((Gun)PickupObjectDatabase.GetById(37)).gunSwitchGroup;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 14f;
            projectile.baseData.speed *= 1.5f;
            projectile.BossDamageMultiplier = 1.2f;
            projectile.baseData.range *= 1.5f;
            projectile.SetProjectileSpriteRight("pot_projectile", 15, 12, false, tk2dBaseSprite.Anchor.MiddleCenter, 15, 12);
            projectile.shouldRotate = true;
            projectile.shouldFlipVertically = true;
            projectile.hitEffects = ((Gun)PickupObjectDatabase.GetById(152)).DefaultModule.projectiles[0].projectile.hitEffects;
            

            PotStickToWall stick = projectile.gameObject.GetOrAddComponent<PotStickToWall>();

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .7f,
               
            };

            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,

            };
            gun.PreventNormalFireAudio = true;

            
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;

        }
        public static int ID;


        public override void PostProcessProjectile(Projectile projectile)
        {

        }

        public override void OnFinishAttack(PlayerController player, Gun gun)
        {

        }
        private bool HasReloaded;
        public bool toggle = false;
        public bool setup = false;
        public override void Update()
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
                if(setup == false)
                {
                    (this.gun.CurrentOwner as PlayerController).healthHaver.OnDamaged += HealthHaver_OnDamaged;
                    setup = true;
                }

              

            }

            base.Update();
        }

        private bool CheckIfCharmed(AIActor boi)
        {
            
            GameActorEffect effect = boi.GetEffect(StaticStatusEffects.charmingRoundsEffect.effectIdentifier); // uses effect Identifier    // Hey Future me. DONT use the resistnaceType on the get effect method. it don't work fam.
            if (effect != null)
            {
                //ETGModConsole.Log(effect.effectIdentifier.ToString());
                return true;
            }
            else
            {
                //ETGModConsole.Log("Null effect");
                return false;
            }



        }

        private void HealthHaver_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            if(resultValue > 0)
            {
                if((this.gun.CurrentOwner as PlayerController).CurrentGun == this.gun)
                {
                    this.gun.LoseAmmo(40);
                    AkSoundEngine.PostEvent("Play_OBJ_pot_shatter_01",base.gameObject);
                    if(this.gun.CurrentAmmo == 0)
                    {
                        this.gun.spriteAnimator.Play("BeamOS_empty");
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

            }
        }


    }

    public class PotStickToWall : MonoBehaviour
    {
        public PotStickToWall()
        {

        }
        public Projectile Pot;
        private void Start()
        {
            
            this.m_projectile = base.GetComponent<Projectile>();
            if (this.m_projectile.Owner is PlayerController)
            {
                this.projOwner = this.m_projectile.Owner as PlayerController;
            }
            SpeculativeRigidbody specRigidBody = this.m_projectile.specRigidbody;
            this.m_projectile.BulletScriptSettings.surviveTileCollisions = true;
            this.m_projectile.BulletScriptSettings.surviveRigidbodyCollisions = true;
            this.m_projectile.pierceMinorBreakables = true;
            m_projectile.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(m_projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));
            m_projectile.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(m_projectile.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreCollisontile));
            m_projectile.OnDestruction += M_projectile_OnDestruction;

            this.projOwner.OnEnteredCombat += entercombat;
        }


        private void entercombat()
        {
            
            if(m_projectile!= null)
            {
                AkSoundEngine.PostEvent("Play_OBJ_pot_shatter_01", base.gameObject);
                UnityEngine.GameObject.Destroy(m_projectile.gameObject);
            }
           
        }

        private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)//Non Wall Collision
        {
            if (otherRigidbody.aiActor)
            {

            }
            else
            {
                if (!otherRigidbody.minorBreakable)
                {
                    DoStop();
                }
            }

        }


        private void M_projectile_OnDestruction(Projectile obj)
        {
            if(this.projOwner != null)
            {
                if(projOwner.PlayerHasActiveSynergy("Pot Smasher"))
                {
                    if(UnityEngine.Random.Range(0,10) < 1)
                    {
                        RoomHandler room = projOwner.CurrentRoom;
                        CellData cell = room.GetNearestCellToPosition(obj.LastPosition);
                        Vector2 vector = cell.position.ToCenterVector2();
                        AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("c182a5cb704d460d9d099a47af49c913");
                        AIActor boy = AIActor.Spawn(orLoadByGuid, cell.position, room, true, AIActor.AwakenAnimationType.Awaken);
                        boy.IgnoreForRoomClear = true;
                        boy.HitByEnemyBullets = true;
                        boy.IsNormalEnemy = false;
                        boy.CanTargetPlayers = false;
                        boy.IsWorthShootingAt = true;

                        boy.isPassable = true;
                        boy.AssignedCurrencyToDrop = 0;
                        boy.PreventAutoKillOnBossDeath = true;
                        boy.CompanionOwner = this.projOwner;
                        
                        CompanionController companionController = boy.gameObject.GetOrAddComponent<CompanionController>();
                        companionController.CanInterceptBullets = true;
                        companionController.Initialize(projOwner);
                        companionController.aiActor.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(companionController.aiActor.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));


                    }
                }
            }
            AkSoundEngine.PostEvent("Play_OBJ_pot_shatter_01", base.gameObject);
        }

        private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody)
            {
                if (otherRigidbody == this.projOwner)
                {
                    PhysicsEngine.SkipCollision = true;
                }

            }
        }

        private void OnPreCollisontile(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
        {
            DoStop();
            
        }

        public void DoStop()
        {


            AkSoundEngine.PostEvent("Play_WPN_trashgun_impact_01", m_projectile.gameObject);
            this.m_projectile.baseData.speed *= 0f;
            this.m_projectile.UpdateSpeed();
            m_projectile.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Remove(m_projectile.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreCollisontile));
            m_projectile.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Remove(m_projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));

            StartCoroutine(DeathTimer());
            StartCoroutine(DoTracking());
        }

        private IEnumerator DoTracking()
        {
           
            yield return new WaitForSeconds(.15f);
            while (m_projectile != null)
            {

                if(this.projOwner.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear) == true)
                {
                    float dist;
                    AIActor target = this.projOwner.CurrentRoom.GetNearestEnemy(m_projectile.sprite.WorldCenter, out dist);
                    if(target!= null)
                    {
                        
                        Vector2 angprep = (Vector2)target.specRigidbody.UnitCenter - (Vector2)m_projectile.sprite.WorldCenter;
                        float ang = angprep.ToAngle();
                        AkSoundEngine.PostEvent("Play_WPN_peashooter_shot_01",m_projectile.gameObject);
                        Projectile component = MiscToolMethods.SpawnProjAtPosi(((Gun)PickupObjectDatabase.GetById(197)).DefaultModule.projectiles[0].projectile, m_projectile.sprite.WorldCenter, this.projOwner, ang);
                        component.baseData.damage = 5;
                        component.HasDefaultTint = true;
                        component.DefaultTintColor = UnityEngine.Color.grey;
                        component.BulletScriptSettings.surviveTileCollisions = true;
                        component.OnHitEnemy += Sloweffect;
                        component.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(component.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreCollisontile2));
                        component.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(component.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison2));
                        m_projectile.SendInDirection(angprep, false, true);
                        
                       
                    }
                    
                }

                yield return new WaitForSeconds(.5f);

            }
            yield return null;
        }

        private void Sloweffect(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(arg2.aiActor != null)
            {
                arg1.statusEffectsToApply.Add(StaticStatusEffects.tripleCrossbowSlowEffect);
                
            }
        }

        private void OnPreCollison2(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody.aiActor)
            {

            }
            else
            {
                if (!otherRigidbody.minorBreakable || !otherRigidbody.projectile)
                {
                    PhysicsEngine.SkipCollision = true;
                    myRigidbody.projectile.ResetDistance();
                }

            }
        }

        private void OnPreCollisontile2(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
        {
            PhysicsEngine.SkipCollision = true;
            myRigidbody.projectile.ResetDistance();
        }

        private IEnumerator DeathTimer()
        {   
            yield return new WaitForSeconds(2f);
            if (m_projectile)
            {
                this.m_projectile.DieInAir();
            }
            
        }

        private Projectile m_projectile;
      
        private PlayerController projOwner;

    }
}