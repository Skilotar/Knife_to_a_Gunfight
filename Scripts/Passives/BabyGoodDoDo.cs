using System;
using System.Collections.Generic;
using System.Collections;
using Gungeon;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using EnemyAPI;
using GungeonAPI;


namespace Knives
{
    public class BabyGoodDodoGama : CompanionItem
    {

        private static readonly string guid = "ski:Dodogama";
        private static GameObject prefab;
        public static void Init()
        {
            string itemName = "Baby Good Dodogama";
            string resourceName = "Knives/Resources/Baby_good_dodo_main";
            GameObject obj = new GameObject();
            BabyGoodDodoGama item = obj.AddComponent<BabyGoodDodoGama>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Too Cute To Hunt";
            string longDesc = "This crystal crunching cutie is a young dodogama cub. With the Chemical reaction in their saliva Dodogama are able to inflict [BlastBlight] by spitting molten rock. Afflicted enemies will explode after 3 instances of damage or on death."
               + "\n\n\n - Knife_to_a_Gunfight";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            item.quality = PickupObject.ItemQuality.B;
            item.CompanionGuid = guid;



            BabyGoodDodoGama.BuildPrefab();

            ID = item.PickupObjectId;
        }
        public static int ID;
        public static void BuildPrefab()
        {
            bool flag = BabyGoodDodoGama.prefab != null || CompanionBuilder.companionDictionary.ContainsKey(BabyGoodDodoGama.guid);
            if (!flag)
            {

                BabyGoodDodoGama.prefab = CompanionBuilder.BuildPrefab("Dodogama", BabyGoodDodoGama.guid, "Knives/Resources/Companions/Dodogama/Idle/Dodogama_idle_001", new IntVector2(0, 0), new IntVector2(15, 15));

                var companionController = BabyGoodDodoGama.prefab.AddComponent<CompanionController>();

                prefab.AddAnimation("idle_left", "Knives/Resources/Companions/Dodogama/idleright", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                prefab.AddAnimation("idle_right", "Knives/Resources/Companions/Dodogama/idleleft", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                prefab.AddAnimation("run_left", "Knives/Resources/Companions/Dodogama/runright", 12, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                prefab.AddAnimation("run_right", "Knives/Resources/Companions/Dodogama/runleft", 12, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                prefab.AddAnimation("attack_right", "Knives/Resources/Companions/Dodogama/attackright", 6, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                prefab.AddAnimation("attack_left", "Knives/Resources/Companions/Dodogama/attackleft", 6, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                //prefab.AddAnimation("pet", "Knives/Resources/Companions/Dodogama/petleft", 4, CompanionBuilder.AnimationType.Other, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
               
                companionController.CanInterceptBullets = false;
               // companionController.CanBePet = true;
                companionController.aiActor.MovementSpeed = 7.5f;
                companionController.aiActor.healthHaver.PreventAllDamage = true;
                companionController.aiActor.CollisionDamage = 0f;
                var bs = prefab.GetComponent<BehaviorSpeculator>();
                bs.MovementBehaviors.Add(new CompanionFollowPlayerBehavior() { IdleAnimations = new string[] { "idle" } });
                bs.MovementBehaviors.Add(new SimpleCompanionBehaviours.SimpleCompanionApproach(4));
                bs.AttackBehaviors.Add(new DodoAttackBehaviour());

            }

        }
    }



    public class DodoAttackBehaviour : AttackBehaviorBase
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
            return 6f;
        }
        public override float GetMinReadyRange()
        {
            return 6f;
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
                    if (aiactor.IsHarmlessEnemy || !aiactor.IsNormalEnemy || aiactor.healthHaver.IsDead || aiactor == this.m_aiActor || aiactor.EnemyGuid == "ba928393c8ed47819c2c5f593100a5bc")
                    { this.roomEnemies.Remove(aiactor); }
                }
                if (this.roomEnemies.Count == 0) { this.m_aiActor.OverrideTarget = null; }
                else
                {
                    AIActor aiActor = this.m_aiActor;
                    AIActor aiactor2 = this.roomEnemies[UnityEngine.Random.Range(0, this.roomEnemies.Count)];
                    aiActor.OverrideTarget = ((aiactor2 != null) ? aiactor2.specRigidbody : null);
                }
            }
        }

        private List<AIActor> roomEnemies = new List<AIActor>();

        private PlayerController Owner;

        public float TickDelay = 4f;

        public float DesiredDistance = 6f;

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
                this.m_aiActor.aiAnimator.PlayForDuration("attack_left", 1.2f);
                while (this.m_aiActor.aiAnimator.IsPlaying("attack_left"))
                {
                    if (this.m_aiActor.spriteAnimator.CurrentFrame == 7 && BadCode)
                    {
                        BadCode = false;
                        if (this.Owner.PlayerHasActiveSynergy("Tempered Dodogama"))
                        {
                            dodshoot();
                            dodshoot();
                        }
                        dodshoot();
                    }
                    yield return null;
                }
            }
            else
            {
                this.m_aiActor.aiAnimator.PlayForDuration("attack_right", 1.2f);
                while (this.m_aiActor.aiAnimator.IsPlaying("attack_right"))
                {
                    if (this.m_aiActor.spriteAnimator.CurrentFrame == 7 && BadCode)
                    {
                        BadCode = false;
                        if(this.Owner.PlayerHasActiveSynergy("Tempered Dodogama"))
                        {
                            dodshoot();
                            dodshoot();
                        }
                        dodshoot();

                    }
                    yield return null;
                }
            }
            
            isFiring = false;
            this.attackTimer = this.TickDelay;
            this.m_aiActor.MovementSpeed = 7.5f;
            yield break;
        }
        System.Random spread = new System.Random();
        public void dodshoot()
        {
            int selectedSpread;
            if (this.Owner.PlayerHasActiveSynergy("Tempered Dodogama"))
            {
                selectedSpread = spread.Next(-15, 16);

            }
            else
            {
                selectedSpread = spread.Next(-2, 3);
            }
            if (m_aiActor.TargetRigidbody)
            {
                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[83]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, this.m_aiActor.sprite.WorldCenter, Quaternion.Euler(0f, 0f, BraveMathCollege.Atan2Degrees((this.m_aiActor.OverrideTarget.specRigidbody.HitboxPixelCollider.UnitCenter - this.m_aiActor.specRigidbody.UnitCenter).normalized) + selectedSpread));
                Projectile component = gameObject.GetComponent<Projectile>();
                if (component != null)
                {
                    component.AdditionalScaleMultiplier = 2.5f;
                    component.baseData.damage = 10;
                    component.Owner = base.m_aiActor;
                    component.Shooter = Owner.specRigidbody;
                    component.collidesWithPlayer = false;
                    PierceProjModifier stabby = component.gameObject.GetComponent<PierceProjModifier>();
                    stabby.penetration = 0;
                    component.OnHitEnemy += this.onhitenemy;
                    
                    AkSoundEngine.PostEvent("Play_ENM_blobulord_bubble_01", base.m_gameObject);
                }
            }
            this.m_aiActor.MovementSpeed = 7.5f;
            attackTimer = 4;
        }

        private void onhitenemy(Projectile proj, SpeculativeRigidbody hit, bool fatal)
        {
            //apply blastblight
            if (!fatal)
            {
                if (hit.aiActor != null)
                {
                    if (!hit.aiActor.gameObject.GetOrAddComponent<BlastBlightedStatusController>().statused)// cannot be statused more than once
                    {
                        BlastBlightedStatusController blighted = hit.aiActor.gameObject.GetOrAddComponent<BlastBlightedStatusController>();
                        blighted.statused = true;
                        blighted.hitstillpop = 2;
                    }
                }
                
            }
        }
    }
}
       





