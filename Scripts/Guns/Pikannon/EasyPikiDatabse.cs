using Dungeonator;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Knives
{
    class EasyPikiDatabase
    {
        public static GameObject RedBoy;
        public static GameObject YellowBoy;
        public static GameObject BlueBoy;
        public static GameObject PurpleBoy;
        public static GameObject WhiteBoy;
        public static void Init()
        {
            BuildRed();
            BuildYellow();
            BuildBlue();
            BuildPurple();
            BuildWhite();
        }
        private static readonly string red_guid = "ski:RedPiki";
        private static readonly string yellow_guid = "ski:YellowPiki";
        private static readonly string Blue_guid = "ski:BluePiki";
        private static readonly string Purple_guid = "ski:PurplePiki";
        private static readonly string White_guid = "ski:WhitePiki";


        private static void BuildRed()
        {

            bool flag = EasyPikiDatabase.RedBoy != null || CompanionBuilder.companionDictionary.ContainsKey(EasyPikiDatabase.red_guid);
            if (!flag)
            {

                EasyPikiDatabase.RedBoy = CompanionBuilder.BuildPrefab("RedPiki", EasyPikiDatabase.red_guid, "Knives/Resources/Companions/Piki/Red/idleright/red_idle_right_003", new IntVector2(0, 0), new IntVector2(15, 15));

                var companionController = EasyPikiDatabase.RedBoy.AddComponent<CompanionController>();

                RedBoy.AddAnimation("idle_left", "Knives/Resources/Companions/Piki/Red/idleleft", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                RedBoy.AddAnimation("idle_right", "Knives/Resources/Companions/Piki/Red/idleright", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                RedBoy.AddAnimation("run_left", "Knives/Resources/Companions/Piki/Red/runright", 10, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                RedBoy.AddAnimation("run_right", "Knives/Resources/Companions/Piki/Red/runleft", 10, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                RedBoy.AddAnimation("attack_right", "Knives/Resources/Companions/Piki/Red/attackright", 8, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                RedBoy.AddAnimation("attack_left", "Knives/Resources/Companions/Piki/Red/attackleft", 8, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                companionController.aiActor.MovementSpeed = 7f;

                companionController.aiActor.CollisionDamage = 0f;
                
                var bs = RedBoy.GetComponent<BehaviorSpeculator>();
                bs.MovementBehaviors.Add(new CompanionFollowPlayerBehavior() { IdleAnimations = new string[] { "idle" } });
                bs.MovementBehaviors.Add(new SimpleCompanionBehaviours.SimpleCompanionApproach(2));
                bs.AttackBehaviors.Add(new VariablePikiAttackBehaviour());
                RedBoy.GetOrAddComponent<AiactorSpecialStates>().PikiColor = AiactorSpecialStates.PikiColors.RED;
                HealthHaver health = RedBoy.GetOrAddComponent<HealthHaver>();
                health.IsVulnerable = true;
                health.SetHealthMaximum(6f);


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
                    
                   
                    companionController.aiActor.isPassable = true;
                    companionController.aiActor.healthHaver.PreventAllDamage = false;
                    companionController.aiActor.CollisionDamage = 0f;
                    companionController.aiActor.HitByEnemyBullets = true;
                    companionController.aiActor.specRigidbody.CollideWithOthers = true;
                    
                    companionController.aiActor.IsWorthShootingAt = true;
                    
                    companionController.aiActor.specRigidbody.CollideWithTileMap = true;
                }

                

               
            }


        }

        private static void BuildYellow()
        {

            bool flag = EasyPikiDatabase.YellowBoy != null || CompanionBuilder.companionDictionary.ContainsKey(EasyPikiDatabase.yellow_guid);
            if (!flag)
            {

                EasyPikiDatabase.YellowBoy = CompanionBuilder.BuildPrefab("YellowPiki", EasyPikiDatabase.yellow_guid, "Knives/Resources/Companions/Piki/Yellow/idleright/Yellow_idle_right_003", new IntVector2(0, 0), new IntVector2(15, 15));

                var companionController = EasyPikiDatabase.YellowBoy.AddComponent<CompanionController>();

                YellowBoy.AddAnimation("idle_left", "Knives/Resources/Companions/Piki/Yellow/idleleft", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                YellowBoy.AddAnimation("idle_right", "Knives/Resources/Companions/Piki/Yellow/idleright", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                YellowBoy.AddAnimation("run_left", "Knives/Resources/Companions/Piki/Yellow/runright", 10, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                YellowBoy.AddAnimation("run_right", "Knives/Resources/Companions/Piki/Yellow/runleft", 10, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                YellowBoy.AddAnimation("attack_right", "Knives/Resources/Companions/Piki/Yellow/attackright", 7, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                YellowBoy.AddAnimation("attack_left", "Knives/Resources/Companions/Piki/Yellow/attackleft", 7, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                companionController.aiActor.MovementSpeed = 10f;

                companionController.aiActor.CollisionDamage = 0f;

                var bs = YellowBoy.GetComponent<BehaviorSpeculator>();
                bs.MovementBehaviors.Add(new CompanionFollowPlayerBehavior() { IdleAnimations = new string[] { "idle" } });
                bs.MovementBehaviors.Add(new SimpleCompanionBehaviours.SimpleCompanionApproach(2));
                bs.AttackBehaviors.Add(new VariablePikiAttackBehaviour());
                YellowBoy.GetOrAddComponent<AiactorSpecialStates>().PikiColor = AiactorSpecialStates.PikiColors.YELLOW;
                HealthHaver health = YellowBoy.GetOrAddComponent<HealthHaver>();
                health.IsVulnerable = true;
                
                health.SetHealthMaximum(4f);

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
                    

                    companionController.aiActor.isPassable = true;
                    companionController.aiActor.healthHaver.PreventAllDamage = false;
                    companionController.aiActor.CollisionDamage = 0f;
                    companionController.aiActor.HitByEnemyBullets = true;
                    companionController.aiActor.specRigidbody.CollideWithOthers = true;

                    companionController.aiActor.IsWorthShootingAt = true;

                    companionController.aiActor.specRigidbody.CollideWithTileMap = true;
                }




            }


        }

        private static void BuildBlue()
        {

            bool flag = EasyPikiDatabase.BlueBoy != null || CompanionBuilder.companionDictionary.ContainsKey(EasyPikiDatabase.Blue_guid);
            if (!flag)
            {

                EasyPikiDatabase.BlueBoy = CompanionBuilder.BuildPrefab("BluePiki", EasyPikiDatabase.Blue_guid, "Knives/Resources/Companions/Piki/Blue/idleright/blue_idle_right_003", new IntVector2(0, 0), new IntVector2(15, 15));

                var companionController = EasyPikiDatabase.BlueBoy.AddComponent<CompanionController>();

                BlueBoy.AddAnimation("idle_left", "Knives/Resources/Companions/Piki/Blue/idleleft", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                BlueBoy.AddAnimation("idle_right", "Knives/Resources/Companions/Piki/Blue/idleright", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                BlueBoy.AddAnimation("run_left", "Knives/Resources/Companions/Piki/Blue/runright", 6, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                BlueBoy.AddAnimation("run_right", "Knives/Resources/Companions/Piki/Blue/runleft", 6, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                BlueBoy.AddAnimation("attack_right", "Knives/Resources/Companions/Piki/Blue/attackright", 6, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                BlueBoy.AddAnimation("attack_left", "Knives/Resources/Companions/Piki/Blue/attackleft", 6, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                companionController.aiActor.MovementSpeed = 7f;

                companionController.aiActor.CollisionDamage = 0f;

                var bs = BlueBoy.GetComponent<BehaviorSpeculator>();
                bs.MovementBehaviors.Add(new CompanionFollowPlayerBehavior() { IdleAnimations = new string[] { "idle" } });
                bs.MovementBehaviors.Add(new SimpleCompanionBehaviours.SimpleCompanionApproach(3));
                bs.AttackBehaviors.Add(new VariablePikiAttackBehaviour());
                BlueBoy.GetOrAddComponent<AiactorSpecialStates>().PikiColor = AiactorSpecialStates.PikiColors.BLUE;
                HealthHaver health = BlueBoy.GetOrAddComponent<HealthHaver>();
                health.IsVulnerable = true;
                
                health.SetHealthMaximum(6f);

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
                        ManualRightY = 0,
                        
                    });
                    

                    companionController.aiActor.isPassable = true;
                    companionController.aiActor.healthHaver.PreventAllDamage = false;
                    companionController.aiActor.CollisionDamage = 0f;
                    companionController.aiActor.HitByEnemyBullets = true;
                    companionController.aiActor.specRigidbody.CollideWithOthers = true;

                    companionController.aiActor.IsWorthShootingAt = true;

                    companionController.aiActor.specRigidbody.CollideWithTileMap = true;
                }




            }


        }

        private static void BuildPurple()
        {

            bool flag = EasyPikiDatabase.PurpleBoy != null || CompanionBuilder.companionDictionary.ContainsKey(EasyPikiDatabase.Purple_guid);
            if (!flag)
            {

                EasyPikiDatabase.PurpleBoy = CompanionBuilder.BuildPrefab("PurplePiki", EasyPikiDatabase.Purple_guid, "Knives/Resources/Companions/Piki/Purple/idleright/Purple_idle_right_003", new IntVector2(0, 0), new IntVector2(15, 15));

                var companionController = EasyPikiDatabase.PurpleBoy.AddComponent<CompanionController>();

                PurpleBoy.AddAnimation("idle_left", "Knives/Resources/Companions/Piki/Purple/idleleft", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                PurpleBoy.AddAnimation("idle_right", "Knives/Resources/Companions/Piki/Purple/idleright", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                PurpleBoy.AddAnimation("run_left", "Knives/Resources/Companions/Piki/Purple/runright", 4, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                PurpleBoy.AddAnimation("run_right", "Knives/Resources/Companions/Piki/Purple/runleft", 4, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                PurpleBoy.AddAnimation("attack_right", "Knives/Resources/Companions/Piki/Purple/attackright", 6, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                PurpleBoy.AddAnimation("attack_left", "Knives/Resources/Companions/Piki/Purple/attackleft", 6, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                companionController.aiActor.MovementSpeed = 5f;

                companionController.aiActor.CollisionDamage = 0f;

                var bs = PurpleBoy.GetComponent<BehaviorSpeculator>();
                bs.MovementBehaviors.Add(new CompanionFollowPlayerBehavior() { IdleAnimations = new string[] { "idle" } });
                bs.MovementBehaviors.Add(new SimpleCompanionBehaviours.SimpleCompanionApproach(2));
                bs.AttackBehaviors.Add(new VariablePikiAttackBehaviour());
                PurpleBoy.GetOrAddComponent<AiactorSpecialStates>().PikiColor = AiactorSpecialStates.PikiColors.PURPLE;
                HealthHaver health = PurpleBoy.GetOrAddComponent<HealthHaver>();
                health.IsVulnerable = true;
                
                health.SetHealthMaximum(12f);

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
                        ManualRightY = 0,

                    });
                    
                    companionController.aiActor.isPassable = true;
                    companionController.aiActor.healthHaver.PreventAllDamage = false;
                    companionController.aiActor.CollisionDamage = 0f;
                    companionController.aiActor.HitByEnemyBullets = true;
                    companionController.aiActor.specRigidbody.CollideWithOthers = true;

                    companionController.aiActor.IsWorthShootingAt = true;

                    companionController.aiActor.specRigidbody.CollideWithTileMap = true;
                }




            }


        }

        private static void BuildWhite()
        {

            bool flag = EasyPikiDatabase.WhiteBoy != null || CompanionBuilder.companionDictionary.ContainsKey(EasyPikiDatabase.White_guid);
            if (!flag)
            {

                EasyPikiDatabase.WhiteBoy = CompanionBuilder.BuildPrefab("WhitePiki", EasyPikiDatabase.White_guid, "Knives/Resources/Companions/Piki/White/idleright/white_idle_right_003", new IntVector2(0, 0), new IntVector2(15, 15));

                var companionController = EasyPikiDatabase.WhiteBoy.AddComponent<CompanionController>();

                WhiteBoy.AddAnimation("idle_left", "Knives/Resources/Companions/Piki/White/idleleft", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                WhiteBoy.AddAnimation("idle_right", "Knives/Resources/Companions/Piki/White/idleright", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                WhiteBoy.AddAnimation("run_left", "Knives/Resources/Companions/Piki/White/runright", 4, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                WhiteBoy.AddAnimation("run_right", "Knives/Resources/Companions/Piki/White/runleft", 4, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                WhiteBoy.AddAnimation("attack_right", "Knives/Resources/Companions/Piki/White/attackright", 6, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                WhiteBoy.AddAnimation("attack_left", "Knives/Resources/Companions/Piki/White/attackleft", 6, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                companionController.aiActor.MovementSpeed = 7f;

                companionController.aiActor.CollisionDamage = 0f;

                var bs = WhiteBoy.GetComponent<BehaviorSpeculator>();
                bs.MovementBehaviors.Add(new CompanionFollowPlayerBehavior() { IdleAnimations = new string[] { "idle" } });
                bs.MovementBehaviors.Add(new SimpleCompanionBehaviours.SimpleCompanionApproach(4));
                bs.AttackBehaviors.Add(new VariablePikiAttackBehaviour());
                WhiteBoy.GetOrAddComponent<AiactorSpecialStates>().PikiColor = AiactorSpecialStates.PikiColors.WHITE;

                HealthHaver health = WhiteBoy.GetOrAddComponent<HealthHaver>();
                health.IsVulnerable = true;
                
                health.SetHealthMaximum(4f);

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
                        ManualRightY = 0,

                    });
                   

                    companionController.aiActor.isPassable = true;
                    companionController.aiActor.healthHaver.PreventAllDamage = false;
                    companionController.aiActor.CollisionDamage = 0f;
                    companionController.aiActor.HitByEnemyBullets = true;
                    companionController.aiActor.specRigidbody.CollideWithOthers = true;

                    companionController.aiActor.IsWorthShootingAt = true;

                    companionController.aiActor.specRigidbody.CollideWithTileMap = true;
                }




            }


        }


    }

    public class VariablePikiAttackBehaviour : AttackBehaviorBase
    {
        public override void Start()
        { 
            
            HealthHaver health = this.m_aiActor.gameObject.GetComponent<HealthHaver>();
            health.OnPreDeath += VariablePikiAttackBehaviour_OnPreDeath;
            health.OnDamaged += Health_OnDamaged;
            var boycolor = this.m_aiActor.gameObject.GetOrAddComponent<AiactorSpecialStates>().PikiColor;
            if(boycolor == AiactorSpecialStates.PikiColors.RED)
            {
                BaseMoveSpeed = 7;
                BaseAttackDamage = 4f;
                BaseAttackSpeed = 1f;
                BaseAttackAngle = 30f;
                BaseAttackRange = 2.5f;
                PreferredDistance = 2f;
                DeathEffectColor = UnityEngine.Color.red;
            }
            if (boycolor == AiactorSpecialStates.PikiColors.YELLOW)
            {
                BaseMoveSpeed = 10;
                BaseAttackDamage = 5f;
                BaseAttackSpeed = 1.2f;
                BaseAttackAngle = 30f;
                BaseAttackRange = 3;
                PreferredDistance = 2f;
                DeathEffectColor = UnityEngine.Color.yellow;
            }
            if (boycolor == AiactorSpecialStates.PikiColors.BLUE)
            {
                BaseMoveSpeed = 7;
                BaseAttackDamage = 3.5f;
                BaseAttackSpeed = 1f;
                PreferredDistance = 3f;
                DeathEffectColor = UnityEngine.Color.blue;
            }
            if (boycolor == AiactorSpecialStates.PikiColors.PURPLE)
            {
                BaseMoveSpeed = 5;
                BaseAttackDamage = 10f;
                BaseAttackSpeed = 3f;
                BaseAttackAngle = 360f;
                BaseAttackRange = 3f;
                PreferredDistance = 2f;
                DeathEffectColor = new Color(.23f, 0, .55f);
            }
            if (boycolor == AiactorSpecialStates.PikiColors.WHITE)
            {
                BaseMoveSpeed = 7;
                BaseAttackDamage = 2.5f;
                BaseAttackSpeed = 1f;
                PreferredDistance = 4f;
                DeathEffectColor = UnityEngine.Color.white;
            }

           
            base.Start();
        }

        private void Health_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            if (m_aiActor.healthHaver.IsAlive && invul == false)
            {
                m_aiActor.StartCoroutine(invulnJank());
            }
        }
        public bool invul;
        private IEnumerator invulnJank()
        {
            // :(
            invul = true;
            m_aiActor.healthHaver.IsVulnerable = false;
            m_aiActor.HitByEnemyBullets = false;
            yield return new WaitForSeconds(.1f);
            m_aiActor.healthHaver.IsVulnerable = true;
            m_aiActor.HitByEnemyBullets = true;
            invul = false;
        }

        private void VariablePikiAttackBehaviour_OnPreDeath(Vector2 obj)
        {
            
            if (this.m_aiActor.gameObject.GetOrAddComponent<AiactorSpecialStates>().pikiSuck == false) // piki died of natural causes
            {
                //ETGModConsole.Log("AuuUUuuuGh Im Dying!!! I need A MEdic BAG!!!");
                //Do death VFX and sound
                AkSoundEngine.PostEvent("Play_Piki_Die_001", m_aiActor.gameObject);
                GlobalSparksDoer.DoRandomParticleBurst(30, m_aiActor.sprite.WorldBottomLeft, m_aiActor.sprite.WorldTopRight, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, DeathEffectColor, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
            }
            
        }

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
            if (m_aiActor.gameObject.GetOrAddComponent<AiactorSpecialStates>().PikiOverrideTarget != null) // target sent from charge shot of gun
            {
                AIActor Obsession = m_aiActor.gameObject.GetOrAddComponent<AiactorSpecialStates>().PikiOverrideTarget;
                if (Obsession.healthHaver.IsAlive)
                {
                    this.m_aiActor.OverrideTarget = Obsession.specRigidbody;
                }
                else
                {
                    m_aiActor.gameObject.GetOrAddComponent<AiactorSpecialStates>().PikiOverrideTarget = null;
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
                this.isInRange = (Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, overrideTarget.UnitCenter) <= this.PreferredDistance);
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
                    if (aiactor.IsHarmlessEnemy || !aiactor.IsNormalEnemy || aiactor.healthHaver.IsDead || aiactor == this.m_aiActor || aiactor.EnemyGuid == "ba928393c8ed47819c2c5f593100a5bc" || aiactor.gameObject.GetComponent<CompanionController>() != null || aiactor.CompanionOwner != null)
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
                this.m_aiActor.aiAnimator.PlayForDuration("attack_left", 1f);
                while (this.m_aiActor.aiAnimator.IsPlaying("attack_left"))
                {
                    if (this.m_aiActor.spriteAnimator.CurrentFrame == 3 && BadCode)
                    {
                        BadCode = false;
                        
                        dodshoot();
                    }
                    yield return null;
                }
            }
            else
            {
                this.m_aiActor.aiAnimator.PlayForDuration("attack_right", 1f);
                while (this.m_aiActor.aiAnimator.IsPlaying("attack_right"))
                {
                    if (this.m_aiActor.spriteAnimator.CurrentFrame == 3 && BadCode)
                    {
                        BadCode = false;
                      
                        dodshoot();

                    }
                    yield return null;
                }
            }

            isFiring = false;
            this.attackTimer = BaseAttackSpeed;
            this.m_aiActor.MovementSpeed = BaseMoveSpeed;
            yield break;
        }
        System.Random spread = new System.Random();
        public void dodshoot()
        {
            int selectedSpread = spread.Next(-2, 3);
            bool DoSpicy = false;
            if(m_aiActor.CompanionOwner.PlayerHasActiveSynergy("Extra Spicy"))
            {
                DoSpicy = false;
            }
            
            if (m_aiActor.TargetRigidbody)
            {
                if (m_aiActor.gameObject.GetComponent<AiactorSpecialStates>().PikiColor == AiactorSpecialStates.PikiColors.BLUE || m_aiActor.gameObject.GetComponent<AiactorSpecialStates>().PikiColor == AiactorSpecialStates.PikiColors.WHITE) // long range
                {
                    if(m_aiActor.gameObject.GetComponent<AiactorSpecialStates>().PikiColor == AiactorSpecialStates.PikiColors.BLUE)
                    {
                        Projectile projectile2 = ((Gun)ETGMod.Databases.Items[599]).DefaultModule.projectiles[0];
                        GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, this.m_aiActor.sprite.WorldCenter, Quaternion.Euler(0f, 0f, BraveMathCollege.Atan2Degrees((this.m_aiActor.OverrideTarget.specRigidbody.HitboxPixelCollider.UnitCenter - this.m_aiActor.specRigidbody.UnitCenter).normalized) + selectedSpread));
                        Projectile component = gameObject.GetComponent<Projectile>();
                        component.baseData.damage = BaseAttackDamage;
                        component.baseData.speed *= 1.5f;
                        component.baseData.range = 5f;
                        component.Owner = m_aiActor.CompanionOwner;
                        component.Shooter = m_aiActor.CompanionOwner.specRigidbody;
                        component.collidesWithPlayer = false;
                        AkSoundEngine.PostEvent("Play_bubblegun_shot_02", base.m_gameObject);

                        if (DoSpicy)
                        {
                            component.FireApplyChance = .15f;
                            var fireRounds = PickupObjectDatabase.GetById(295) as BulletStatusEffectItem;
                            var fireEffect = fireRounds.FireModifierEffect;
                            component.fireEffect = fireEffect;
                            component.AppliesFire = true;
                        }
                    }

                    if (m_aiActor.gameObject.GetComponent<AiactorSpecialStates>().PikiColor == AiactorSpecialStates.PikiColors.WHITE)
                    {
                        if(m_aiActor.TargetRigidbody.healthHaver != null)
                        {
                            if (m_aiActor.TargetRigidbody.healthHaver.IsAlive)
                            {
                                if (m_aiActor.TargetRigidbody.aiActor.IsBlackPhantom)
                                {
                                    m_aiActor.TargetRigidbody.healthHaver.ApplyDamage(10f, Vector2.zero, "Holy Garlic", CoreDamageTypes.Void, DamageCategory.Unstoppable, true, null, true);
                                    if (DoSpicy)
                                    {
                                        m_aiActor.TargetRigidbody.aiActor.ApplyEffect(StaticStatusEffects.hotLeadEffect);
                                    }
                                }
                                else
                                {
                                    m_aiActor.TargetRigidbody.healthHaver.ApplyDamage(2f, Vector2.zero, "Holy Garlic", CoreDamageTypes.Void, DamageCategory.Unstoppable, false, null, false);
                                    if (DoSpicy)
                                    {
                                        if(UnityEngine.Random.Range(1,5) < 1f)
                                        {
                                            m_aiActor.TargetRigidbody.aiActor.ApplyEffect(StaticStatusEffects.hotLeadEffect);
                                        }
                                    }
                                }
                                GlobalSparksDoer.DoRandomParticleBurst(10, m_aiActor.sprite.WorldBottomLeft, m_aiActor.sprite.WorldTopRight, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, UnityEngine.Color.white, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
                                GlobalSparksDoer.DoRandomParticleBurst(20, m_aiActor.TargetRigidbody.sprite.WorldBottomLeft, m_aiActor.TargetRigidbody.sprite.WorldTopRight, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, UnityEngine.Color.white, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
                            }
                        }
                        //AkSoundEngine.PostEvent("Play_PET_owl_hoot_01", base.m_gameObject);
                    }
                    
                }
                else // closerange
                {


                    Projectile projectile2 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                    GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, this.m_aiActor.sprite.WorldCenter, Quaternion.Euler(0f, 0f, BraveMathCollege.Atan2Degrees((this.m_aiActor.OverrideTarget.specRigidbody.HitboxPixelCollider.UnitCenter - this.m_aiActor.specRigidbody.UnitCenter).normalized) + selectedSpread));
                    Projectile component = gameObject.GetComponent<Projectile>();
                    if (component != null)
                    {

                        if (m_aiActor.gameObject.GetComponent<AiactorSpecialStates>().PikiColor == AiactorSpecialStates.PikiColors.RED)
                        {
                            component.FireApplyChance = .07f;
                            if (DoSpicy) component.FireApplyChance = .50f;

                            var fireRounds = PickupObjectDatabase.GetById(295) as BulletStatusEffectItem;
                            var fireEffect = fireRounds.FireModifierEffect;
                            component.fireEffect = fireEffect;
                            component.AppliesFire = true;
                            AkSoundEngine.PostEvent("Play_ENM_rubber_bounce_01", base.m_gameObject);
                        }
                        if (m_aiActor.gameObject.GetComponent<AiactorSpecialStates>().PikiColor == AiactorSpecialStates.PikiColors.PURPLE)
                        {
                            component.StunApplyChance = .2f;
                            component.AppliesStun = true;
                            if (DoSpicy)
                            {
                                component.FireApplyChance = .15f;
                                var fireRounds = PickupObjectDatabase.GetById(295) as BulletStatusEffectItem;
                                var fireEffect = fireRounds.FireModifierEffect;
                                component.fireEffect = fireEffect;
                                component.AppliesFire = true;
                            }
                            SpawnManager.SpawnVFX(EasyVFXDatabase.MachoBraceDustUpVFX, m_aiActor.specRigidbody.UnitCenter + new Vector2(-1.5f, -2f), Quaternion.identity);
                            AkSoundEngine.PostEvent("Play_ENM_ironmaiden_stomp_01", base.m_gameObject);
                        }
                        if (m_aiActor.gameObject.GetComponent<AiactorSpecialStates>().PikiColor == AiactorSpecialStates.PikiColors.YELLOW)
                        {
                            AkSoundEngine.PostEvent("Play_PET_wolf_bite_01", base.m_gameObject);
                            if (DoSpicy)
                            {
                                component.FireApplyChance = .15f;
                                var fireRounds = PickupObjectDatabase.GetById(295) as BulletStatusEffectItem;
                                var fireEffect = fireRounds.FireModifierEffect;
                                component.fireEffect = fireEffect;
                                component.AppliesFire = true;
                            }
                        }
                       
                        component.baseData.damage = BaseAttackDamage;
                        component.Owner = m_aiActor.CompanionOwner;
                        component.Shooter = m_aiActor.CompanionOwner.specRigidbody;
                        component.collidesWithPlayer = false;
                        ProjectileSlashingBehaviour stabby = component.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
                        stabby.SlashVFX.type = VFXPoolType.None;
                        stabby.SlashRange = BaseAttackRange;
                        stabby.DoSound = false;
                        //stabby.soundToPlay =;
                        stabby.SlashDimensions = BaseAttackAngle;
                        stabby.InteractMode = SlashDoer.ProjInteractMode.IGNORE;
                        //ETGModConsole.Log("I have no mouth and I must KILLLL");
                    }
                }
            }
            this.m_aiActor.MovementSpeed = BaseMoveSpeed;
            attackTimer = .3f;
        }

        public Color DeathEffectColor = UnityEngine.Color.red;
        public float BaseMoveSpeed = 7;
        public float BaseAttackSpeed = .3f;
        public float BaseAttackDamage = 4f;
        public float BaseAttackRange = 2;
        public float BaseAttackAngle = 30;
        public float PreferredDistance = 1.5f;
    }
}
       

