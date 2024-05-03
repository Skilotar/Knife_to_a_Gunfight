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
using Dungeonator;
using Knives;

namespace Knives
{
    class Vengeance : MultiActiveReloadController
    {
        private static readonly string guid = "ski:minisentry";
        public static GameObject prefab;
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Frontier Vengeance", "vengance");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:frontier_vengeance", "ski:frontier_vengeance");
            var Behav = gun.gameObject.AddComponent<Vengeance>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Goodnight, Irene!");
            gun.SetLongDescription("Active reload to place a mini sentry. Gain crits based on the sentry's kills when it dies.\n\n" +
                "" +
                "A marvel of gundromidan technology! This gun is able to convert emotion into energy overcharging its rounds when it experiences loss." +



                "\n\n\n - Knife_to_a_Gunfight");
            gun.SetupSprite(null, "vengance_idle_001", 1);
            gun.SetAnimationFPS(gun.shootAnimation, 9);
            gun.SetAnimationFPS(gun.reloadAnimation, 4);

            for (int i = 0; i < 10; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(157) as Gun, true, false);
                gun.gunSwitchGroup = (PickupObjectDatabase.GetById(157) as Gun).gunSwitchGroup;
            }
            foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
            {
                projectileModule.ammoCost = 1;
                projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                projectileModule.cooldownTime = .8f;
                projectileModule.angleVariance = 9f;
                projectileModule.numberOfShotsInClip = 3;

                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
                projectile.gameObject.GetOrAddComponent<projectileStates>().isveng = true;
                projectile.gameObject.SetActive(false);
                projectileModule.projectiles[0] = projectile;
                projectile.baseData.range = 20 * (1 + UnityEngine.Random.Range(-.2f, .2f));
                projectile.baseData.speed = 20 * (1 + UnityEngine.Random.Range(-.3f, .3f));
                projectile.baseData.damage = 2.5f;
                projectile.AdditionalScaleMultiplier = .5f;
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                
                gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SHOTGUN;
                gun.DefaultModule.projectiles[0] = projectile;
                bool flag = projectileModule != gun.DefaultModule;
                if (flag)
                {
                    projectileModule.ammoCost = 0;
                }

            }
            gun.barrelOffset.transform.localPosition = new Vector3(1.5f, .5f, 0f);
            
            gun.reloadTime = 1f;
            gun.SetBaseMaxAmmo(300);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(157) as Gun).muzzleFlashEffects;
            gun.quality = PickupObject.ItemQuality.B;
            gun.gunClass = GunClass.SHOTGUN;
            gun.CanReloadNoMatterAmmo = true;
            Behav.activeReloadEnabled = true;
            Behav.canAttemptActiveReload = true;

            Behav.reloads = new List<MultiActiveReloadData>
            {
                new MultiActiveReloadData(0.55f, 60, 90, 32, 20, false, false, new ActiveReloadData
                {
                    damageMultiply = 1f,
                }, true, "SpawnMini",6,2)

            };

            Vengeance.BuildPrefab();

            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;
        }

        private static void BuildPrefab()
        {
           
            bool flag = Vengeance.prefab != null || CompanionBuilder.companionDictionary.ContainsKey(Vengeance.guid);
            if (!flag)
            {

                Vengeance.prefab = CompanionBuilder.BuildPrefab("MiniSentry", Vengeance.guid, "Knives/Resources/Companions/mini/idleleft/mini_idle_left_001", new IntVector2(0, 0), new IntVector2(15, 15));

                var companionController = Vengeance.prefab.AddComponent<CompanionController>();

                prefab.AddAnimation("idle_left", "Knives/Resources/Companions/mini/idleleft", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                prefab.AddAnimation("idle_right", "Knives/Resources/Companions/mini/idleright", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                prefab.AddAnimation("run_left", "Knives/Resources/Companions/mini/runright", 12, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                prefab.AddAnimation("run_right", "Knives/Resources/Companions/mini/runleft", 12, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                prefab.AddAnimation("attack_right", "Knives/Resources/Companions/mini/attackright", 12, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                prefab.AddAnimation("attack_left", "Knives/Resources/Companions/mini/attackleft", 12, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);


                prefab.GetOrAddComponent<KnockbackDoer>();
                prefab.GetOrAddComponent<KickMini>();

                companionController.aiActor.MovementSpeed = 0f;
                
                companionController.aiActor.CollisionDamage = 0f;
                
                var bs = prefab.GetComponent<BehaviorSpeculator>();
                bs.AttackBehaviors.Add(new MiniAttackBehaviour());


                

            }
        }

        public static int ID;

        public override void OnPickedUpByPlayer(PlayerController player)
        {
            
            base.OnPickedUpByPlayer(player);
        }
        public override void OnSwitchedAwayFromThisGun()
        {
            if (doingCritRoutine)
            {
                StopCoroutine(CritRoutine(this.gun));
                if(origiColor != null)
                {
                    this.gun.sprite.color = origiColor;
                }
                else
                {
                    this.gun.sprite.color = new Color(1, 1, 1, 1);
                }
                
                doingCritRoutine = false;
            }
            base.OnSwitchedAwayFromThisGun();
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            if(StoredCrits >= 1)
            {
                StoredCrits = StoredCrits - 1;
            }
            base.OnPostFired(player, gun);
        }

        private GameObject m_extantCompanion;
        public int StoredCrits = 0;
        public override void OnActiveReloadSuccess(MultiActiveReload reload)
        {
            base.OnActiveReloadSuccess(reload);

            PlayerController owner = gun.CurrentOwner as PlayerController;
            if (reload.Name == "SpawnMini")
            {
                //ETGModConsole.Log("Mini Sentry");

                
                if(this.m_extantCompanion != null)
                {
                    DoSafeExplosion(m_extantCompanion.transform.position);
                    StoredCrits += m_extantCompanion.gameObject.GetOrAddComponent<AiactorSpecialStates>().GlobalKillsGotten;
                    UnityEngine.Object.Destroy(this.m_extantCompanion);
                    this.m_extantCompanion = null;
                }
                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("ski:minisentry");
                Vector3 vector = owner.transform.position;
                if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                {
                    vector += new Vector3(1.125f, -0.3125f, 0f);
                }
                GameObject extantCompanion2 = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, vector, Quaternion.identity);
                this.m_extantCompanion = extantCompanion2;
                CompanionController companionController = this.m_extantCompanion.GetOrAddComponent<CompanionController>();

                companionController.CanInterceptBullets = true;
                companionController.Initialize(owner);
                extantCompanion2.GetOrAddComponent<HealthHaver>().IsVulnerable = true;
                extantCompanion2.GetOrAddComponent<HealthHaver>().OnPreDeath += OnPreDeath;
                
                extantCompanion2.GetOrAddComponent<HealthHaver>().SetHealthMaximum(15f);
                
                
                if (companionController.aiActor.specRigidbody)
                {
                   
                    companionController.aiAnimator.specRigidbody.PixelColliders.Add(new PixelCollider
                    {
                        ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                        CollisionLayer = CollisionLayer.PlayerHitBox,
                        IsTrigger = false,
                        BagleUseFirstFrameOnly = false,
                        SpecifyBagelFrame = string.Empty,
                        BagelColliderNumber = 0,
                        ManualOffsetX = 1,
                        ManualOffsetY = 2,
                        ManualWidth = 9,
                        ManualHeight = 12,
                        ManualDiameter = 0,
                        ManualLeftX = 0,
                        ManualLeftY = 0,
                        ManualRightX = 0,
                        ManualRightY = 0
                    });
                    PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(companionController.aiActor.specRigidbody, null, false);
                    companionController.aiActor.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(companionController.aiActor.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));

                    
                    companionController.aiActor.isPassable = true;
                    companionController.aiActor.healthHaver.PreventAllDamage = false;
                    companionController.aiActor.CollisionDamage = 0f;
                    companionController.aiActor.specRigidbody.CollideWithOthers = true;
                    
                    companionController.aiActor.specRigidbody.CollideWithTileMap = true;

                    //ETGModConsole.Log("Added Layers");
                }



            }

        }

        private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody)
            {
                if (otherRigidbody == this.gun.CurrentOwner) 
                {
                    PhysicsEngine.SkipCollision = true;
                }
                
            }
        }

        private void OnPreDeath(Vector2 obj)
        {
            StoredCrits += m_extantCompanion.gameObject.GetOrAddComponent<AiactorSpecialStates>().GlobalKillsGotten;
            DoSafeExplosion(m_extantCompanion.transform.position);
        }

        public void DoSafeExplosion(Vector3 position)
        {

            ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
            this.smallPlayerSafeExplosion.effect = EasyVFXDatabase.RedFireBlastVFX;
            this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
            this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;

            Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);

        }
        private ExplosionData smallPlayerSafeExplosion = new ExplosionData
        {
            damageRadius = 2f,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 3f,
            doDestroyProjectiles = false,
            doForce = false,
            debrisForce = .1f,
            preventPlayerForce = true,
            explosionDelay = 0.1f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = false,
            
            
        };

        public override void PostProcessProjectile(Projectile projectile)
        {
            if (projectile.GetComponent<projectileStates>() != null)
            {
                if(projectile.GetComponent<projectileStates>().isveng == true)
                {
                    if (HasCrits)
                    {
                        ImprovedAfterImage trail = projectile.gameObject.GetOrAddComponent<ImprovedAfterImage>();
                        trail.shadowLifetime = .2f;
                        trail.shadowTimeDelay = .0001f;
                        trail.dashColor = new Color(.67f, .08f, .01f);
                        trail.spawnShadows = true;
                        projectile.baseData.speed *= 2f;
                        projectile.UpdateSpeed();
                        projectile.baseData.damage *= 3;
                        projectile.HasDefaultTint = true;
                        projectile.DefaultTintColor = UnityEngine.Color.red;
                       
                        AkSoundEngine.PostEvent("Play_BOSS_lichC_zap_01", base.gameObject);
                        

                    }
                }
            }

            base.PostProcessProjectile(projectile);
        }


        private bool HasReloaded;
        public bool HasCrits;
        public bool doingCritRoutine;

        //This block of code allows us to change the reload sounds.
        public override void Update()
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

                if(StoredCrits > 35)
                {
                    StoredCrits = 35;
                }

                if(StoredCrits >= 1)
                {
                    HasCrits = true;
                    if(doingCritRoutine == false)
                    {
                        StartCoroutine(CritRoutine(this.gun));

                    }
                   
                }
                else
                {
                    HasCrits = false;


                }



            }

            base.Update();
        }
        public Color origiColor;
        private IEnumerator CritRoutine(Gun gun)
        {
            doingCritRoutine = true;
            origiColor = gun.sprite.color;
            while (HasCrits)
            {
                
                gun.sprite.color = Color.red;
                GlobalSparksDoer.DoRandomParticleBurst(1, gun.sprite.WorldBottomLeft, gun.sprite.WorldTopRight, new Vector3(1f, 1f, 0f), 120f, 0.2f, null, null, Color.red , GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT);
                yield return new WaitForSeconds(.1f);
                yield return null;
            }
            if (origiColor != null)
            {
                this.gun.sprite.color = origiColor;
            }
            else
            {
                this.gun.sprite.color = new Color(1, 1, 1, 1);
            }
            doingCritRoutine = false;
            yield return null;
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool manualReload)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                HasReloaded = false;
                base.OnReload(player, gun);

               
            }

        }

    }
    
}



public class MiniAttackBehaviour : AttackBehaviorBase
{

    public override BehaviorResult Update()
    {

        base.DecrementTimer(ref this.attackTimer, false);
        if (Owner == null)
        {
            if (this.m_aiActor && this.m_aiActor.CompanionOwner)
            {
                Owner = this.m_aiActor.CompanionOwner;
            }
            else
            {
                Owner = GameManager.Instance.BestActivePlayer;
            }
        }
        if (m_aiActor && m_aiActor.OverrideTarget == null)
        {
            PickNewTarget();
        }
        BehaviorResult result;
        if (this.m_aiActor && this.m_aiActor.OverrideTarget)
        {
            SpeculativeRigidbody overrideTarget = this.m_aiActor.OverrideTarget;
            this.isInRange = (Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, overrideTarget.UnitCenter) <= this.DesiredDistance);
            if (isInRange)
            {
                if (overrideTarget != null && this.attackTimer == 0 && !isFiring)
                {
                    AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", base.m_aiActor.gameObject);
                    this.m_aiActor.StartCoroutine(fail());
                    result = BehaviorResult.SkipAllRemainingBehaviors;
                }
                else result = BehaviorResult.Continue;
            }
            else result = BehaviorResult.Continue;
        }
        else result = BehaviorResult.Continue;
        return result;
    }

    public override float GetMaxRange()
    {
        return 10f;
    }
    public override float GetMinReadyRange()
    {
        return 10f;
    }
    public override bool IsReady()
    {
        AIActor aiActor = this.m_aiActor;
        bool flag;
        if (aiActor == null) flag = true;
        else
        {
            SpeculativeRigidbody targetRigidbody = aiActor.TargetRigidbody;
            Vector2? vector = (targetRigidbody != null) ? new Vector2?(targetRigidbody.UnitCenter) : null;
            flag = (vector == null);
        }
        bool flag2 = flag;
        return !flag2 && Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, this.m_aiActor.TargetRigidbody.UnitCenter) <= this.GetMinReadyRange();
    }

    private void PickNewTarget()
    {
        if (this.m_aiActor != null)
        {
            if (this.Owner == null)
            {
                if (this.m_aiActor && this.m_aiActor.CompanionOwner)
                {
                    Owner = this.m_aiActor.CompanionOwner;
                }
                else
                {
                    Owner = GameManager.Instance.BestActivePlayer;
                }
            }
            this.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear, ref this.roomEnemies);
            for (int i = 0; i < this.roomEnemies.Count; i++)
            {
                AIActor aiactor = this.roomEnemies[i];
                if (aiactor.IsHarmlessEnemy || !aiactor.IsNormalEnemy || aiactor.healthHaver.IsDead || aiactor == this.m_aiActor || aiactor.EnemyGuid == "ba928393c8ed47819c2c5f593100a5bc" || aiactor.healthHaver.IsVulnerable == false || !m_aiActor.HasLineOfSightToRigidbody(aiactor.specRigidbody))
                { this.roomEnemies.Remove(aiactor); }
            }
            if (this.roomEnemies.Count == 0) { this.m_aiActor.OverrideTarget = null; }
            else
            {

                AIActor aiActor = this.m_aiActor;
                AIActor aiactor2 = null;
                int knownLowest = 99999;
                for (int i = 0; i < this.roomEnemies.Count; i++)
                {
                    if (Vector2.Distance(roomEnemies[i].Position, aiActor.Position) < knownLowest)
                    {
                        aiactor2 = roomEnemies[i];
                    }
                }

                
                aiActor.OverrideTarget = ((aiactor2 != null) ? aiactor2.specRigidbody : null);
            }
        }
    }

    private List<AIActor> roomEnemies = new List<AIActor>();

    private PlayerController Owner;

    public float TickDelay = 0f;

    public float DesiredDistance = 20f;

    private float attackTimer;

    private bool isInRange;

    private bool isFiring = false;

    

    private IEnumerator fail()
    {
        
        isFiring = true;
        this.m_aiActor.MovementSpeed = 0;
        bool BadCode = true;
        float angletotarget = 0;
        if (m_aiActor.TargetRigidbody)
        {
            angletotarget = OMITBMathsAndLogicExtensions.CalculateVectorBetween(m_aiActor.transform.position, m_aiActor.OverrideTarget.transform.position).ToAngle();
        }
        if (angletotarget > 90 || angletotarget < -90)
        {
            this.m_aiActor.aiAnimator.PlayForDuration("attack_left", .5f);
            while (this.m_aiActor.aiAnimator.IsPlaying("attack_left"))
            {
                if (this.m_aiActor.spriteAnimator.CurrentFrame == 0 && BadCode)
                {

                    BadCode = false;
                    dodshoot();
                }
                if (this.m_aiActor.spriteAnimator.CurrentFrame == 2 && BadCode == false)
                {

                    BadCode = true;
                }
               
                yield return null;
            }
        }
        else
        {
            this.m_aiActor.aiAnimator.PlayForDuration("attack_right", .5f);
            while (this.m_aiActor.aiAnimator.IsPlaying("attack_right"))
            {
                if (this.m_aiActor.spriteAnimator.CurrentFrame == 0 && BadCode)
                {

                    BadCode = false;
                    dodshoot();
                }
                if(this.m_aiActor.spriteAnimator.CurrentFrame == 2 && BadCode == false)
                {

                    BadCode = true;
                }
               
                yield return null;
            }
        }
        this.m_aiActor.OverrideTarget = null;
        isFiring = false;
        this.attackTimer = this.TickDelay;
        this.m_aiActor.MovementSpeed = 0f;
        yield break;
    }

    public void dodshoot()
    {
        if(m_aiActor.TargetRigidbody != null)
        {

            Projectile projectile2 = ((Gun)PickupObjectDatabase.GetById(15)).DefaultModule.projectiles[0].projectile;
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, this.m_aiActor.sprite.WorldCenter, Quaternion.Euler(0f, 0f, BraveMathCollege.Atan2Degrees((this.m_aiActor.OverrideTarget.specRigidbody.HitboxPixelCollider.UnitCenter - this.m_aiActor.specRigidbody.UnitCenter).normalized)));
            Projectile component = gameObject.GetComponent<Projectile>();
            if (component != null)
            {
                component.AdditionalScaleMultiplier *= .3f;
                component.baseData.damage = 3;
                component.Owner = base.m_aiActor;
                component.Shooter = Owner.specRigidbody;
                component.collidesWithPlayer = false;
                component.baseData.speed *= 2f;
                AkSoundEngine.PostEvent("Play_WPN_earthwormgun_shot_01", base.m_gameObject);
                component.OnHitEnemy += this.onhitenemy;

            }

        }

        this.m_aiActor.MovementSpeed = 0f;
        attackTimer = 1f;
    }

    private void onhitenemy(Projectile proj, SpeculativeRigidbody hit, bool fatal)
    {
        
        if (fatal && this.m_aiActor != null)
        {
            this.m_aiActor.gameObject.GetOrAddComponent<AiactorSpecialStates>().GlobalKillsGotten++;
        }
    }
    
}


