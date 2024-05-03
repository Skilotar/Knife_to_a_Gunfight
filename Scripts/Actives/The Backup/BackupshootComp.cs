using System;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;
using ItemAPI;
using UnityEngine;

namespace Knives
{
	public class BackupShootComp : MonoBehaviour
	{

		public BackupShootComp()
		{


		}

		public int DroneHP = 6;
		private void Start()
		{

			this.specbod = base.GetComponent<SpeculativeRigidbody>();
			specbod.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specbod.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));

			
		}
		
		private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
			if (otherRigidbody != null && myRigidbody != null)
			{

				if (otherRigidbody.projectile)
				{
					if (DroneHP <= 0)
					{
						EmergencyDoShootyCleanup();
						GlobalSparksDoer.DoRandomParticleBurst(20, base.gameObject.transform.position, base.gameObject.transform.position, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                        AkSoundEngine.PostEvent("Play_OBJ_turret_fade_01", base.gameObject);
						UnityEngine.Object.Destroy(base.gameObject.gameObject);
					}
					else
					{

						DroneHP--;

					}
					if (DroneHP == 2)//orange
					{
						myRigidbody.sprite.color = new Color(.70f, .40f, .24f);

					}
					if (DroneHP == 1)//red
					{
						myRigidbody.sprite.color = new Color(.33f, .07f, .04f);

					}
				}

			}

		}
		AIActor posibleTarget = new AIActor();
		
		private void Update()
		{
			
			bool flag = this.Orbital != null;
			if (flag)
			{
				bool flag2 = this.parentOwner != null;
				if (flag2)
				{
					if(posibleTarget == null) posibleTarget = GetEnemyTarget();
					
					if (posibleTarget != null && doingShooty == false)
					{
						
						StartCoroutine(DoShootyTheEnemyToBeShooted(posibleTarget));
					}
					if (doingShooty == true)
					{
						if (elapse == .5f)
						{
							EmergencyDoShootyCleanup();
						}
						else
						{
							elapse += Time.deltaTime;
						}
					}




				}
			}
			
		}
		
		private void EmergencyDoShootyCleanup()
		{
			
			doingShooty = false;
			elapse = 0;
		}

		bool doingShooty = false;
		float elapse = 0;

		public IEnumerator DoShootyTheEnemyToBeShooted(AIActor target)
		{
			
			elapse = 0;
			doingShooty = true;
			if (target != null)
			{
				
				Vector2 angprep = (Vector2)target.specRigidbody.UnitCenter - (Vector2)this.Orbital.GetOrAddComponent<tk2dBaseSprite>().WorldCenter;
				float ang = angprep.ToAngle();
				float var = UnityEngine.Random.Range(-5, 6);
				Projectile proj = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, (Vector2)this.Orbital.GetOrAddComponent<tk2dBaseSprite>().WorldCenter, this.parentOwner, ang, var, 1f, false);
				proj.AdditionalScaleMultiplier *= .5f;
				proj.baseData.damage = 2f * parentOwner.stats.GetStatValue(PlayerStats.StatType.Damage);
				proj.sprite.color = UnityEngine.Color.red;

				ImprovedAfterImage trail = proj.gameObject.GetOrAddComponent<ImprovedAfterImage>();
				trail.shadowLifetime = .1f;
				trail.shadowTimeDelay = .0001f;
				trail.dashColor = new Color(.47f, .30f, .37f);
				trail.spawnShadows = true;

				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForSeconds(.4f / parentOwner.stats.GetStatValue(PlayerStats.StatType.RateOfFire));
				AkSoundEngine.PostEvent("Play_WPN_nailgun_shot_01", this.Orbital);

			}


			doingShooty = false;
		}

		 private List<AIActor> roomEnemies = new List<AIActor>();


		private AIActor GetEnemyTarget()
		{
			if (this.Orbital != null)
			{

				if (this.parentOwner != null)
				{
					this.parentOwner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear, ref this.roomEnemies);
					if(this.roomEnemies.Count > 0)
                    {
						for (int i = 0; i < this.roomEnemies.Count; i++)
						{
							AIActor aiactor = this.roomEnemies[i];
							//ETGModConsole.Log(this.roomEnemies[i]);
							if (aiactor.IsHarmlessEnemy || !aiactor.IsNormalEnemy || aiactor.healthHaver.IsDead || aiactor == this.Orbital || aiactor.EnemyGuid == "ba928393c8ed47819c2c5f593100a5bc" || aiactor.gameObject.GetComponent<CompanionController>() != null || aiactor.CompanionOwner != null)
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

		

		



		public GameObject Orbital;
		
		public SpeculativeRigidbody specbod;
		public PlayerController parentOwner;

		

	}
}
