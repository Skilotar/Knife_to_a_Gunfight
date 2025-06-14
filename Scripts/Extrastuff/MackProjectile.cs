using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;

namespace Knives
{
	public class MackProjMod : MonoBehaviour
	{

		public MackProjMod()
		{
			
			
		}

		public bool Outgoing = false;
		private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
			this.parentOwner = this.Projectile.ProjectilePlayerOwner();
			lockon = Projectile.gameObject.GetOrAddComponent<LockOnHomingModifier>();
			lockon.enabled = false;
			lockon.HomingRadius = int.MaxValue;
			lockon.AngularVelocity = 180;
			Speed = Projectile.baseData.speed;
			DashSpeed = Speed * 2.5f;
			PierceProjModifier stabby = Projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
			stabby.penetration = int.MaxValue;
			Projectile.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(Projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));
			Projectile.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(Projectile.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.onPreTileCollison));

			Projectile.OnHitEnemy += this.onHitEnemy;

		}

        private void onHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(UnityEngine.Random.Range(0,5) < 1)  //chance to retarget if current target isnt bleeding 
            {
				target = GetEnemyTarget();
			}
        }

        public float DashSpeed;
		public float Speed;
        public AIActor target;
		private void Update()
		{
			if(parentOwner != null)
            {
				if(target == null) // dont have a target
                {
					target = GetEnemyTarget();
					doReturn();
                }
                else
                {
					lockon.enabled = true;
					lockon.AssignTargetManually(target);

					if (OMITBMathsAndLogicExtensions.PositionBetweenRelativeValidAngles(target.Position, Projectile.LastPosition, 70, 5, 50)) // cone of vision for dash attack
                    {
						this.Projectile.baseData.speed = DashSpeed;
						this.Projectile.UpdateSpeed();
						Vector2 vector = (Vector2)target.Position - (Vector2)Projectile.LastPosition;
						this.Projectile.SendInDirection(vector, true, true);
					}
                    else
                    {
						this.Projectile.baseData.speed = Speed;
						this.Projectile.UpdateSpeed();
					}
                }

				
            }
		}

		public void doReturn()
		{
			if (Vector2.Distance(parentOwner.CenterPosition, this.Projectile.LastPosition) <= 4)
			{


				if (Vector2.Distance(parentOwner.CenterPosition, this.Projectile.LastPosition) <= 3)
				{
					this.Projectile.baseData.speed = 0f;
					this.Projectile.UpdateSpeed();
					Vector2 vector = parentOwner.CenterPosition - (Vector2)Projectile.LastPosition;
					this.Projectile.SendInDirection(vector, true, true);

				}
                else
                {
					Vector2 vector = parentOwner.CenterPosition - (Vector2)Projectile.LastPosition;
					this.Projectile.SendInDirection(vector, true, true);
					this.Projectile.baseData.speed = 5f;
					this.Projectile.UpdateSpeed();
				}
			}
            else
            {
				
                
				this.Projectile.baseData.speed = Speed;
				this.Projectile.UpdateSpeed();
				Vector2 vector = parentOwner.CenterPosition - (Vector2)Projectile.LastPosition;
				this.Projectile.SendInDirection(vector, true, true);

				if(Vector2.Distance(parentOwner.CenterPosition, this.Projectile.LastPosition) >= 30)
                {
					Projectile.DieInAir();
                }
				


			}
		}

		private List<AIActor> roomEnemies = new List<AIActor>();
		private AIActor GetEnemyTarget()
		{
			
			if (this.parentOwner != null && !GameManager.Instance.IsLoadingLevel)
			{
				if (this.parentOwner.CurrentRoom != null)
				{
					this.parentOwner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear, ref this.roomEnemies);
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

		private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherbody, PixelCollider otherPixelCollider)
		{

			if (otherbody != null && otherbody.aiActor == null)
			{
				PhysicsEngine.SkipCollision = true;
			}
			if (otherPixelCollider != null && otherPixelCollider.IsTileCollider)
			{
				PhysicsEngine.SkipCollision = true;
			}

		}

		private void onPreTileCollison(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
		{
			if (tile != null)
			{
				PhysicsEngine.SkipCollision = true;
			}
		}


		private LockOnHomingModifier lockon;
		private Projectile Projectile;
		private PlayerController parentOwner;
		

	}

}