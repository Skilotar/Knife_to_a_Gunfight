using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;
using System.Reflection;
using System;
using Dungeonator;
using System.Collections.Generic;

namespace Knives
{
    class RatCrown : PassiveItem
    {
       
        public static void Register()
        {
            //The name of the item
            string itemName = "Rat Crown";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/RatKing";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<RatCrown>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Makes All The Rules";
            string longDesc = "Kills spawn vicious rats\n\n" +
                "" +
                "You have been shown favor by the Rat Queen, Aemilia. Her army is at your back. While gross they do prove effective." +
                "\n\n\n - Knife_to_a_Gunfight";


            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            
            item.quality = PickupObject.ItemQuality.D;
            ID = item.PickupObjectId;
        }

        public static int ID;

        public override void Pickup(PlayerController player)
        {
           
            player.OnKilledEnemyContext += this.OnKilledEnemy;
           
            base.Pickup(player);
            
        }
        static string rat = "6ad1cafc268f4214a101dca7af61bc91";

        public void OnKilledEnemy(PlayerController arg1, HealthHaver arg2)
        {
            //ETGModConsole.Log("tryRatting");
            SpawnRat(arg1, arg2.aiActor.sprite.WorldCenter);
            if (arg1.PlayerHasActiveSynergy("LabRats")) { SpawnRat(arg1, arg2.aiActor.sprite.WorldCenter); }

        }

        public static void SpawnRat(PlayerController arg1,Vector2 posi)
        {
            AIActor spawn = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(rat), posi, arg1.CurrentRoom, true, AIActor.AwakenAnimationType.Default);
            spawn.CanTargetEnemies = true;
            spawn.CanTargetPlayers = false;
            spawn.IgnoreForRoomClear = true;
            spawn.HitByEnemyBullets = false;
            spawn.IsNormalEnemy = false;
            spawn.DiesOnCollison = false;
            spawn.IsWorthShootingAt = false;
            spawn.isPassable = true;
            spawn.AssignedCurrencyToDrop = 0;

            spawn.CompanionOwner = arg1;
            spawn.MovementSpeed = 10;
            spawn.healthHaver.SetHealthMaximum(3);
            CompanionController friend = spawn.gameObject.GetOrAddComponent<CompanionController>();
            friend.Initialize(arg1);
            var bs = spawn.aiActor.GetComponent<BehaviorSpeculator>();
            bs.MovementBehaviors.Clear();
            bs.MovementBehaviors.Add(new CompanionFollowPlayerBehavior());
            bs.MovementBehaviors.Add(new SimpleCompanionBehaviours.SimpleCompanionApproach(2));
            bs.AttackBehaviors.Clear();
            bs.AttackBehaviors.Add(new RatAttack());

        }

        public override void Update()
        {

            if (this.Owner != null)
            {
                if ((this.Owner as PlayerController).PlayerHasActiveSynergy("LabRats"))
                {
                    HasSynergy = true;
                }
                else
                {
                    HasSynergy = false;
                }
            }
            base.Update();
        }
        public static bool HasSynergy = false;
        
        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            player.OnKilledEnemyContext -= this.OnKilledEnemy;

            return debrisObject;
        }


        public class RatAttack : AttackBehaviorBase
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

            public float TickDelay = .5f;

            public float DesiredDistance = 2f;

            private float attackTimer;

            private bool isInRange;

            private bool isFiring = false;

            private IEnumerator fail()
            {
                isFiring = true;
                this.m_aiActor.MovementSpeed = 7;
                dodshoot();
              
                isFiring = false;
                this.attackTimer = this.TickDelay;
                this.m_aiActor.MovementSpeed = 10f;
                yield break;
            }
            
            public void dodshoot()
            {
                
                if (m_aiActor.TargetRigidbody)
                {
                    Projectile projectile2 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0].projectile;
                    GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, this.m_aiActor.sprite.WorldCenter, Quaternion.Euler(0f, 0f, BraveMathCollege.Atan2Degrees((this.m_aiActor.OverrideTarget.specRigidbody.HitboxPixelCollider.UnitCenter - this.m_aiActor.specRigidbody.UnitCenter).normalized)));
                    Projectile component = gameObject.GetComponent<Projectile>();
                    if (component != null)
                    {
                        
                        component.baseData.damage = 5;
                        component.Owner = Owner;
                        component.Shooter = Owner.specRigidbody;
                        component.collidesWithPlayer = false;
                        ProjectileSlashingBehaviour stabby = component.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
                        stabby.OnSlashHitEnemy += Stabby_OnSlashHitEnemy;
                        stabby.SlashRange = 3.5f;
                        stabby.SlashDimensions = 180f;
                        stabby.SlashDamage = 3f;
                        stabby.SlashVFX = (PickupObjectDatabase.GetById(335) as Gun).muzzleFlashEffects;
                        stabby.soundToPlay = "";

                    }
                }
                this.m_aiActor.MovementSpeed = 11f;
                attackTimer = .5f;
            }

            private void Stabby_OnSlashHitEnemy(PlayerController player, AIActor hitEnemy, Vector2 forceDirection)
            {
                m_aiActor.EraseFromExistence();
            }

          
        }
    }
}

