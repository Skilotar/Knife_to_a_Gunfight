using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections;

namespace Knives
{
    class HellBound : PassiveItem
    {
        public static void Register()
        {

            string itemName = "Hell Bound";


            string resourceName = "Knives/Resources/Hell_bound";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<HellBound>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Unrelenting";
            string longDesc = "Move slower while dragging the chain, gain trail of fire. An abnormally hot ball and chain that feels heavier than it looks. It whispers your name and drags fire into the earth. \n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            item.quality = PickupObject.ItemQuality.C;
            item.CanBeDropped = false;
            item.PersistsOnDeath = true;
            
            itemID = item.PickupObjectId;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Move slower while dragging the chain, gain trail of fire";

            Gun gun = (Gun)ETGMod.Databases.Items[15];
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0].projectile);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            //projectile2.shouldRotate = true;
            projectile2.baseData.damage = 10f;
            projectile2.baseData.range = 9999f;
            projectile2.hitEffects.suppressMidairDeathVfx = true;
            projectile2.pierceMinorBreakables = true;
            PierceProjModifier stabby = projectile2.gameObject.GetOrAddComponent<PierceProjModifier>();
            stabby.penetratesBreakables = true;
            stabby.penetration = 100;
            
            projectile2.SetProjectileSpriteRight("WeightBall", 16, 16, false, tk2dBaseSprite.Anchor.MiddleCenter, 16, 16);
            Ball = projectile2;

            
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0].projectile);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.shouldRotate = true;
            projectile.baseData.damage = 10f;
            projectile.baseData.range = 9999f;
            projectile.AdditionalScaleMultiplier *= .8f;
            projectile.hitEffects.suppressMidairDeathVfx = true;
            projectile.pierceMinorBreakables = true;
            PierceProjModifier stabby2 = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            stabby2.penetratesBreakables = true;
            stabby2.penetration = 100;
            projectile.SetProjectileSpriteRight("SmallChain", 9, 9, false, tk2dBaseSprite.Anchor.MiddleCenter, 9, 9);
            chain = projectile;


            Projectile chomp = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0].projectile);
            chomp.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(chomp.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(chomp);
            //projectile2.shouldRotate = true;
            chomp.baseData.damage = 20f;
            chomp.baseData.range = 9999f;
            chomp.hitEffects.suppressMidairDeathVfx = true;
            chomp.pierceMinorBreakables = true;
            chomp.shouldRotate = false;
            chomp.shouldFlipHorizontally = true;
            PierceProjModifier stabby3 = chomp.gameObject.GetOrAddComponent<PierceProjModifier>();
            stabby3.penetratesBreakables = true;
            stabby3.penetration = 100;

            MiscToolMethods.EasyAnimate(chomp, new List<string>() {
                "WeightBall_chomp_001",
                "WeightBall_chomp_002",
                "WeightBall_chomp_003",
                "WeightBall_chomp_004",

            }, 4, new IntVector2(16, 18), 8, false, tk2dSpriteAnimationClip.WrapMode.Loop);

            MiscToolMethods.EasyAnimate(chomp, new List<string>() {
                "WeightBall_chomp_pet_001",
                "WeightBall_chomp_pet_002",
                "WeightBall_chomp_pet_003",
                "WeightBall_chomp_pet_004",

            }, 4, new IntVector2(16, 18), 8, false, tk2dSpriteAnimationClip.WrapMode.Loop,"Chomp_Pet");

            MiscToolMethods.EasyAnimate(chomp, new List<string>() {
                "WeightBall_chomp_lunge_001",
                

            }, 1, new IntVector2(16, 18), 2, false, tk2dSpriteAnimationClip.WrapMode.Loop, "Chomp_Lunge");


            Chomp = chomp;

        }


        public static Projectile Chomp;
        public static Projectile Ball;
        public static Projectile chain;
        public static int itemID;
        public override void Pickup(PlayerController player)
        {
            this.m_fireImmunity = new DamageTypeModifier();
            this.m_fireImmunity.damageMultiplier = 0f;
            this.m_fireImmunity.damageType = CoreDamageTypes.Fire;
            player.healthHaver.damageTypeModifiers.Add(this.m_fireImmunity);
            
            base.Pickup(player);
        }
        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.damageTypeModifiers.Remove(this.m_fireImmunity);
            return base.Drop(player);
        }
        public static Projectile m_Ball;
        public static Projectile m_chain1;
        public static Projectile m_chain2;
        public bool hassyn = false;
        public override void  Update()
        {
            base.Update();
            if (this.Owner != null)
            {

                if (this.Owner == true && !GameManager.Instance.IsLoadingLevel)
                {

                    if (m_chain1 == null)
                    {

                        BallAndChainMod.IgnoreingReturn = false;
                        m_chain1 = MiscToolMethods.SpawnProjAtPosi(chain, this.Owner.CenterPosition, this.Owner);
                        m_chain1.baseData.range = int.MaxValue;
                        m_chain1.baseData.damage = 0f;
                        m_chain1.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(m_chain1.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.onPreTileCollison));
                        BallAndChainMod ball = m_chain1.gameObject.GetOrAddComponent<BallAndChainMod>();
                        ball.DrawRange = 1.5f;
                    }
                    if (m_chain2 == null)
                    {
                        BallAndChainMod.IgnoreingReturn = false;
                        m_chain2 = MiscToolMethods.SpawnProjAtPosi(chain, this.Owner.CenterPosition, this.Owner);
                        m_chain2.baseData.range = int.MaxValue;
                        m_chain2.baseData.damage = 0f;
                        m_chain2.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(m_chain2.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.onPreTileCollison)); BallAndChainMod ball = m_chain2.gameObject.GetOrAddComponent<BallAndChainMod>();
                        ball.DrawRange = 0.5f;
                    }

                    if (m_Ball == null)
                    {

                        if (this.Owner.PlayerHasActiveSynergy("Bow Wow"))
                        {
                            BallAndChainMod.IgnoreingReturn = false;
                            m_Ball = MiscToolMethods.SpawnProjAtPosi(Chomp, this.Owner.CenterPosition, this.Owner);
                            m_Ball.baseData.range = int.MaxValue;
                            m_Ball.baseData.damage = 20f;
                            m_Ball.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(m_Ball.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.onPreTileCollison));
                            BallAndChainMod ball = m_Ball.gameObject.GetOrAddComponent<BallAndChainMod>();
                            ball.mainBall = true;
                            KickProj kick = m_Ball.gameObject.GetOrAddComponent<KickProj>();
                            kick.chain1 = m_chain1;
                            kick.chain2 = m_chain2;
                        }
                        else
                        {
                            BallAndChainMod.IgnoreingReturn = false;
                            m_Ball = MiscToolMethods.SpawnProjAtPosi(Ball, this.Owner.CenterPosition, this.Owner);
                            m_Ball.baseData.range = int.MaxValue;
                            m_Ball.baseData.damage = 10f;
                            m_Ball.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(m_Ball.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.onPreTileCollison));
                            BallAndChainMod ball = m_Ball.gameObject.GetOrAddComponent<BallAndChainMod>();
                            ball.mainBall = true;
                            KickProj kick = m_Ball.gameObject.GetOrAddComponent<KickProj>();
                            kick.chain1 = m_chain1;
                            kick.chain2 = m_chain2;
                        }

                    }
                    if (this.Owner.PlayerHasActiveSynergy("Bow Wow"))
                    {
                        if (hassyn == false)
                        {
                            if (m_Ball != null) m_Ball.DieInAir();
                            hassyn = true;
                        }

                        if (m_Ball != null)
                        {
                            if (target == null) // dont have a target
                            {
                                target = GetEnemyTarget();

                            }
                            else
                            {
                                if (target != null)
                                {

                                    if (internalCooldown <= 0)
                                    {
                                        m_Ball.StartCoroutine(lunge(m_Ball));
                                        internalCooldown = 4;
                                    }
                                }
                                if (internalCooldown > 0)
                                {
                                    internalCooldown -= BraveTime.DeltaTime;
                                }
                            }

                            if (this.Owner.CurrentRoom != null) //room exists?  
                            {
                                RoomHandler room = this.Owner.CurrentRoom;

                                float dist = 15;
                                IPlayerInteractable nearestInteractable = room.GetNearestInteractable(this.Owner.CenterPosition, dist, this.Owner); // any interactable Objects?
                                if (nearestInteractable != null)
                                {
                                    if (nearestInteractable is Chest) // is it a chest? 
                                    {
                                        Chest chest = nearestInteractable as Chest;
                                        if (chest.IsMimic) // Mimic Spotted!
                                        {
                                            if (doingBark == false)
                                            {
                                                StartCoroutine(bark(m_Ball));
                                            }

                                        }
                                    }
                                }

                            }
                        }
                        

                    }
                    else
                    {
                        if (hassyn == true)
                        {
                            if (m_Ball != null) m_Ball.DieInAir();
                            hassyn = false;
                        }
                    }
                }

            }
        }

        private IEnumerator bark(Projectile m_Ball)
        {
            doingBark = true;
            AkSoundEngine.PostEvent("Play_PET_wolf_growl_01", m_Ball.gameObject);
            yield return new WaitForSeconds(.5f);
            doingBark = false;
        }

        private IEnumerator lunge(Projectile m_Ball)
        {
            BallAndChainMod.IgnoreingReturn = true;
            m_Ball.spriteAnimator.Play("Chomp_Lunge");
            AkSoundEngine.PostEvent("Play_PET_dog_bark_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_PET_dog_bark_01", base.gameObject);
            Vector2 vector = target.CenterPosition - (Vector2)m_Ball.LastPosition;


            m_Ball.baseData.speed = 14;
            m_Ball.UpdateSpeed();
            m_Ball.SendInDirection(vector, true);
            yield return new WaitForSeconds(.1f);
            Vector2 vector2 = (Vector2)m_Ball.LastPosition - (Vector2)m_chain1.LastPosition;
            if (m_chain1 != null)
            {
                m_chain1.Speed = 12;
                m_chain1.SendInDirection(vector2, true);
            }
            yield return new WaitForSeconds(.1f);
            Vector2 vector3 = (Vector2)m_chain1.LastPosition - (Vector2)m_chain2.LastPosition;
            if (m_chain2 != null)
            {
                m_chain2.Speed = 10;
                m_chain2.SendInDirection(vector3, true);
            }
            if (m_Ball != null)
                m_Ball.Owner.knockbackDoer.ApplyKnockback(vector, 10, true);
            yield return new WaitForSeconds(.1f);

            AkSoundEngine.PostEvent("Play_PET_dog_bark_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_PET_dog_bark_01", base.gameObject);

            yield return new WaitForSeconds(.1f);
            if (m_Ball != null)
            { 
                m_Ball.spriteAnimator.Play("idle");
                m_Ball.baseData.speed = 1;
                m_Ball.UpdateSpeed();
                m_Ball.SendInDirection(-vector, true); 
            }
            if (m_chain1 != null)
            {
                m_chain1.Speed = 1;
                m_chain1.SendInDirection(-vector, true);
            }
            if (m_chain2 != null)
            {
                m_chain2.Speed = 1;
                m_chain2.SendInDirection(-vector, true);
            }
            BallAndChainMod.IgnoreingReturn = false;
            target = null;
            yield return new WaitForSeconds(.4f);
        }

        public float internalCooldown = 0;
        private List<AIActor> roomEnemies = new List<AIActor>();
        private AIActor GetEnemyTarget()
        {

            if (this.Owner != null && !GameManager.Instance.IsLoadingLevel)
            {
                if(this.Owner.CurrentRoom != null)
                {
                    this.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear, ref this.roomEnemies);
                    if (this.roomEnemies.Count > 0)
                    {
                        for (int i = 0; i < this.roomEnemies.Count; i++)
                        {
                            AIActor aiactor = this.roomEnemies[i];
                            //ETGModConsole.Log(this.roomEnemies[i]);
                            if (aiactor.IsHarmlessEnemy || !aiactor.IsNormalEnemy || aiactor.healthHaver.IsDead || aiactor.EnemyGuid == "ba928393c8ed47819c2c5f593100a5bc" || aiactor.gameObject.GetComponent<CompanionController>() != null || aiactor.CompanionOwner != null)
                            { this.roomEnemies.Remove(aiactor); }
                        }
                        if (this.roomEnemies.Count == 0) { return null; }
                        else
                        {
                            AIActor aiactor2 = this.roomEnemies[UnityEngine.Random.Range(0, this.roomEnemies.Count)];
                            return aiactor2;
                        }
                    }
                }
                
            }

            return null;

        }
        public AIActor target = null;
        private void onPreTileCollison(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
        {
            if (tile != null)
            {
                PhysicsEngine.SkipCollision = true;
            }
        }
        public DamageTypeModifier m_fireImmunity;
        private bool doingBark;
    }


	public class BallAndChainMod : MonoBehaviour
	{

		public BallAndChainMod()
		{


		}

        public float DrawRange = 2.5f;
        public bool mainBall = false;
		public bool Outgoing = false;
        private bool ChainTaught = false;
        private bool beingslowed = false;
        public StatModifier SpeedModifier = null;

        private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
			this.parentOwner = this.Projectile.ProjectilePlayerOwner();
            this.SpeedModifier = new StatModifier();
            this.SpeedModifier.statToBoost = PlayerStats.StatType.MovementSpeed;
            this.SpeedModifier.modifyType = StatModifier.ModifyMethod.ADDITIVE;
            this.SpeedModifier.amount = -1.9f;
            Projectile.OnDestruction += Projectile_OnDestruction;
        }

        private void Projectile_OnDestruction(Projectile obj)
        {
            if (mainBall == true)
            {
                if(beingslowed == true)
                {
                    this.parentOwner.ownerlessStatModifiers.Remove(SpeedModifier);
                    this.parentOwner.stats.RecalculateStats(parentOwner);
                }
            }
        }
        public static bool IgnoreingReturn = false;
        private void Update()
		{
			if (parentOwner != null)
			{
                if (IgnoreingReturn != true) doReturn();

                if (mainBall == true)
                {
                    
                    GoopDefinition fire = PickupObjectDatabase.GetById(242).GetComponent<DirectionalAttackActiveItem>().goopDefinition;
                    fire.lifespan = 10f;
                    DeadlyDeadlyGoopManager goop = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(fire);
                    goop.TimedAddGoopCircle(Projectile.LastPosition, .5f, .05f, true);
                    if (ChainTaught)
                    {
                        if(beingslowed == false)
                        {
                            this.parentOwner.ownerlessStatModifiers.Add(SpeedModifier);
                            this.parentOwner.stats.RecalculateStats(parentOwner);
                            beingslowed = true;
                        }
                        
                    }
                    else
                    {
                        if (beingslowed == true)
                        {
                            this.parentOwner.ownerlessStatModifiers.Remove(SpeedModifier);
                            this.parentOwner.stats.RecalculateStats(parentOwner);
                            beingslowed = false;
                        }

                    }
                }
			}
		}

		public void doReturn()
		{
			if (Vector2.Distance(parentOwner.CenterPosition, this.Projectile.LastPosition) <= DrawRange + .5f)
			{
				if (Vector2.Distance(parentOwner.CenterPosition, this.Projectile.LastPosition) <= DrawRange)
				{
					this.Projectile.baseData.speed = 0f;
					this.Projectile.UpdateSpeed();
					Vector2 vector = parentOwner.CenterPosition - (Vector2)Projectile.LastPosition;
					this.Projectile.SendInDirection(vector, true, true);
                    ChainTaught = false;
				}
				else
				{
					Vector2 vector = parentOwner.CenterPosition - (Vector2)Projectile.LastPosition;
					this.Projectile.SendInDirection(vector, true, true);
					this.Projectile.baseData.speed = 5f;
                    if (parentOwner.PlayerHasActiveSynergy("Bow Wow")) this.Projectile.baseData.speed = 6.5f;
                    this.Projectile.UpdateSpeed();
                    ChainTaught = true;
                }
			}
			else // very far away emergency checks
			{
				this.Projectile.baseData.speed = 12;
				this.Projectile.UpdateSpeed();
				Vector2 vector = parentOwner.CenterPosition - (Vector2)Projectile.LastPosition;
				this.Projectile.SendInDirection(vector, true, true);
                ChainTaught = true;

                if(Vector2.Distance(parentOwner.CenterPosition, this.Projectile.LastPosition) >= 20) // something has gone seriously horribly wrong. Reset and redo;
                {
                    IgnoreingReturn = false;
                    ChainTaught = false;
                    this.parentOwner.ownerlessStatModifiers.Remove(SpeedModifier);
                    this.parentOwner.stats.RecalculateStats(parentOwner);
                    Projectile.DieInAir();
                }
            }
		}
       
        private Projectile Projectile;
		private PlayerController parentOwner;


	}

	
}
