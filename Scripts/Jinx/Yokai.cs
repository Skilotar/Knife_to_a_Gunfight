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
    public class FingerTrap : CompanionItem
    {

        private static readonly string guid = "ski:Fingertrap";
        public static GameObject prefab;
        public static void Init()
        {
            string itemName = "Yokai Finger Trap";
            string resourceName = "Knives/Resources/Companions/Yokai/Yokai_fingertrap";
            GameObject obj = new GameObject();
            FingerTrap item = obj.AddComponent<FingerTrap>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Pranked";
            string longDesc = "The spirit of a mischievous Gunjurer infests this paper finger trap. Once caught the trap will not let go and the ghost will manifest to laugh at you. This Spirit lantern's Bullets will afflict enemies with hex. Creatures afflicted with [Hex] may be damaged upon attacking." +
                "\n\n - reload speed + companion" +
                "\n\n\n  - Knife_to_a_Gunfight";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            item.quality = PickupObject.ItemQuality.B;
            item.CompanionGuid = guid;

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.ReloadSpeed, .45f);

            item.CanBeDropped = false;
            item.PersistsOnDeath = true;
            FingerTrap.BuildPrefab();
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            ID = item.PickupObjectId;

            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Gain a Hexing familiar, Lowers reloading speed";
        }
        
        public static int ID;
        public static void BuildPrefab()
        {
            bool flag = FingerTrap.prefab != null || CompanionBuilder.companionDictionary.ContainsKey(FingerTrap.guid);
            if (!flag)
            {

                FingerTrap.prefab = CompanionBuilder.BuildPrefab("Fingertrap", FingerTrap.guid, "Knives/Resources/Companions/Yokai/idleleft/Yokai_idle_left_001", new IntVector2(0, 0), new IntVector2(15, 15));

                var companionController = FingerTrap.prefab.AddComponent<CompanionController>();

                prefab.AddAnimation("idle_left", "Knives/Resources/Companions/Yokai/idleleft", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                prefab.AddAnimation("idle_right", "Knives/Resources/Companions/Yokai/idleright", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                prefab.AddAnimation("run_left", "Knives/Resources/Companions/Yokai/runright", 12, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                prefab.AddAnimation("run_right", "Knives/Resources/Companions/Yokai/runleft", 12, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                prefab.AddAnimation("attack_right", "Knives/Resources/Companions/Yokai/attackright", 6, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                prefab.AddAnimation("attack_left", "Knives/Resources/Companions/Yokai/attackleft", 6, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                companionController.CanInterceptBullets = false;
                companionController.aiActor.MovementSpeed = 7.5f;
                companionController.aiActor.healthHaver.PreventAllDamage = true;
                companionController.aiActor.CollisionDamage = 0f;
                
                var bs = prefab.GetComponent<BehaviorSpeculator>();
                bs.MovementBehaviors.Add(new CompanionFollowPlayerBehavior() { IdleAnimations = new string[] { "idle" } });
                bs.MovementBehaviors.Add(new SimpleCompanionBehaviours.SimpleCompanionApproach(4));
                bs.AttackBehaviors.Add(new TrapAttackBehaviour());


                


            }

            

        }
    }



    public class TrapAttackBehaviour : AttackBehaviorBase
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
           
            
            if (m_aiActor.TargetRigidbody)
            {
                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[83]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, this.m_aiActor.sprite.WorldCenter, Quaternion.Euler(0f, 0f, BraveMathCollege.Atan2Degrees((this.m_aiActor.OverrideTarget.specRigidbody.HitboxPixelCollider.UnitCenter - this.m_aiActor.specRigidbody.UnitCenter).normalized)));
                Projectile component = gameObject.GetComponent<Projectile>();
                if (component != null)
                {
                    component.AdditionalScaleMultiplier = 2.5f;
                    component.baseData.damage = 3;
                    component.Owner = base.m_aiActor;
                    component.Shooter = Owner.specRigidbody;
                    component.collidesWithPlayer = false;
                    PierceProjModifier stabby = component.gameObject.GetComponent<PierceProjModifier>();
                    stabby.penetration = 0;
                    component.OnHitEnemy += this.onhitenemy;
                    AkSoundEngine.PostEvent("Play_WPN_lamp_shot_01", base.m_gameObject);
                }
            }
            this.m_aiActor.MovementSpeed = 7.5f;
            attackTimer = 4;
        }

        private void onhitenemy(Projectile proj, SpeculativeRigidbody hit, bool fatal)
        {
            //apply Hex
            if(hit.aiActor != null)
            {
                if (!fatal)
                {
                    if (!hit.aiActor.gameObject.GetOrAddComponent<HexStatusEffectController>().statused)// cannot be statused more than once
                    {
                        HexStatusEffectController blighted = hit.aiActor.gameObject.GetOrAddComponent<HexStatusEffectController>();
                        blighted.statused = true;
                    }
                }
            }
            
        }
    }
}