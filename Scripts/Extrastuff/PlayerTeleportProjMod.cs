using System;
using System.Collections;
using System.Diagnostics;
using Dungeonator;
using UnityEngine;

namespace Knives
{
	// I got this code from NeverNamed 
	public class PlayerProjectileTeleportModifier : BraveBehaviour
	{
		
		public PlayerProjectileTeleportModifier()
		{
			this.trigger = PlayerProjectileTeleportModifier.TeleportTrigger.AngleToTarget;
			this.minAngleToTeleport = 70f;
			this.distToTeleport = 3f;
			this.type = PlayerProjectileTeleportModifier.TeleportType.BackToSpawn;
			this.behindTargetDistance = 5f;
		}
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnTeleport;

		
		private bool ShowMMinAngleToTeleport()
		{
			return this.trigger == PlayerProjectileTeleportModifier.TeleportTrigger.AngleToTarget;
		}

		
		private bool ShowDistToTeleport()
		{
			return this.trigger == PlayerProjectileTeleportModifier.TeleportTrigger.DistanceFromTarget;
		}

		
		private bool ShowBehindTargetDistance()
		{
			return this.type == PlayerProjectileTeleportModifier.TeleportType.BehindTarget;
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x000DDD3C File Offset: 0x000DBF3C
		public void Start()
		{
			if ((base.projectile.Owner as PlayerController).IsInCombat)
			{
				bool flag = !base.sprite;
				if (flag)
				{
					base.sprite = base.GetComponentInChildren<tk2dSprite>();
				}
				bool flag2 = base.projectile && base.projectile.Owner is AIActor;
				if (flag2)
				{
					this.m_targetRigidbody = (base.projectile.Owner as AIActor).TargetRigidbody;
				}
				else
				{
					bool flag3 = base.projectile && base.projectile.Owner is PlayerController;
					if (flag3)
					{
						AIActor randomActiveEnemy = MiscToolMethods.GetAbsoluteRoomFromProjectile(base.projectile).GetRandomActiveEnemy(false);
						this.m_targetRigidbody = randomActiveEnemy.specRigidbody;
					}
				}
				bool flag4 = !this.m_targetRigidbody;
				if (flag4)
				{
					base.enabled = false;
				}
				else
				{
					this.m_startingPos = base.transform.position;
				}
			}
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x000DDE28 File Offset: 0x000DC028
		public void Update()
		{
			if ((base.projectile.Owner as PlayerController).IsInCombat)
			{
				bool isTeleporting = this.m_isTeleporting;
				if (!isTeleporting)
				{
					bool flag = this.m_cooldown > 0f;
					if (flag)
					{
						this.m_cooldown -= BraveTime.DeltaTime;
					}
					else
					{
						bool flag2 = this.numTeleports > 0 && this.ShouldTeleport();
						if (flag2)
						{
							base.StartCoroutine(this.DoTeleport());
						}
					}
				}
			}
		}

		// Token: 0x060010F6 RID: 4342 RVA: 0x000DDE8E File Offset: 0x000DC08E
		public override void OnDestroy()
		{
			base.StopAllCoroutines();
			base.OnDestroy();
		}

		// Token: 0x060010F7 RID: 4343 RVA: 0x000DDEA0 File Offset: 0x000DC0A0
		private bool ShouldTeleport()
		{
			bool flag = this.m_targetRigidbody;
			bool result;
			if (flag)
			{
				Vector2 unitCenter = this.m_targetRigidbody.GetUnitCenter(ColliderType.HitBox);
				bool flag2 = this.trigger == PlayerProjectileTeleportModifier.TeleportTrigger.AngleToTarget;
				if (flag2)
				{
					float a = (unitCenter - base.specRigidbody.UnitCenter).ToAngle();
					float b = base.specRigidbody.Velocity.ToAngle();
					result = (BraveMathCollege.AbsAngleBetween(a, b) > this.minAngleToTeleport);
				}
				else
				{
					result = (this.trigger == PlayerProjectileTeleportModifier.TeleportTrigger.DistanceFromTarget && Vector2.Distance(unitCenter, base.specRigidbody.UnitCenter) < this.distToTeleport);
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x000DDF4C File Offset: 0x000DC14C
		private Vector2 GetTeleportPosition()
		{
			bool flag = this.type == PlayerProjectileTeleportModifier.TeleportType.BackToSpawn;
			Vector2 result;
			if (flag)
			{
				result = this.m_startingPos;
			}
			else
			{
				bool flag2 = this.type == PlayerProjectileTeleportModifier.TeleportType.BehindTarget && this.m_targetRigidbody && this.m_targetRigidbody.gameActor;
				if (flag2)
				{
					Vector2 unitCenter = this.m_targetRigidbody.GetUnitCenter(ColliderType.HitBox);
					float facingDirection = this.m_targetRigidbody.gameActor.FacingDirection;
					Dungeon dungeon = GameManager.Instance.Dungeon;
					for (int i = 0; i < 18; i++)
					{
						Vector2 vector = unitCenter + BraveMathCollege.DegreesToVector(facingDirection + 180f + (float)(i * 20), this.behindTargetDistance);
						bool flag3 = !dungeon.CellExists(vector) || !dungeon.data.isWall((int)vector.x, (int)vector.y);
						if (flag3)
						{
							return vector;
						}
						vector = unitCenter + BraveMathCollege.DegreesToVector(facingDirection + 180f + (float)(i * -20), this.behindTargetDistance);
						bool flag4 = !dungeon.CellExists(vector) || !dungeon.data.isWall((int)vector.x, (int)vector.y);
						if (flag4)
						{
							return vector;
						}
					}
				}
				result = this.m_startingPos;
			}
			return result;
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x000DE0BF File Offset: 0x000DC2BF
		private IEnumerator DoTeleport()
		{
			VFXPool vfxpool = this.teleportVfx;
			Vector3 position = base.specRigidbody.UnitCenter;
			Transform transform = base.transform;
			vfxpool.SpawnAtPosition(position, 0f, transform, null, null, null, false, null, null, false);
			bool flag = this.teleportPauseTime > 0f;
			if (flag)
			{
				this.m_isTeleporting = true;
				base.sprite.renderer.enabled = false;
				base.projectile.enabled = false;
				base.specRigidbody.enabled = false;
				bool flag2 = base.projectile.braveBulletScript;
				if (flag2)
				{
					base.projectile.braveBulletScript.enabled = false;
				}
				yield return new WaitForSeconds(this.teleportPauseTime);
				bool flag3 = !this || !this.m_targetRigidbody;
				if (flag3)
				{
					yield break;
				}
				this.m_isTeleporting = false;
				base.sprite.renderer.enabled = true;
				base.projectile.enabled = true;
				base.specRigidbody.enabled = true;
				bool flag4 = base.projectile.braveBulletScript;
				if (flag4)
				{
					base.projectile.braveBulletScript.enabled = true;
				}
			}
			Vector2 newPosition = this.GetTeleportPosition();
			base.transform.position = newPosition;
			base.specRigidbody.Reinitialize();
			VFXPool vfxpool2 = this.teleportVfx;
			position = base.specRigidbody.UnitCenter;
			transform = base.transform;
			vfxpool2.SpawnAtPosition(position, 0f, transform, null, null, null, false, null, null, false);
			Vector2 firingCenter = base.specRigidbody.UnitCenter;
			Vector2 targetCenter = this.m_targetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
			PlayerController targetPlayer = this.m_targetRigidbody.gameActor as PlayerController;
			bool flag5 = this.leadAmount > 0f && targetPlayer;
			if (flag5)
			{
				Vector2 targetVelocity = (!targetPlayer) ? this.m_targetRigidbody.Velocity : targetPlayer.AverageVelocity;
				Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(targetCenter, targetVelocity, firingCenter, base.projectile.Speed);
				targetCenter = Vector2.Lerp(targetCenter, predictedPosition, this.leadAmount);
				targetVelocity = default(Vector2);
				predictedPosition = default(Vector2);
			}
			base.projectile.SendInDirection(targetCenter - firingCenter, true, true);
			bool flag6 = base.projectile.braveBulletScript && base.projectile.braveBulletScript.bullet != null;
			if (flag6)
			{
				base.projectile.braveBulletScript.bullet.Position = newPosition;
				base.projectile.braveBulletScript.bullet.Direction = (targetCenter - newPosition).ToAngle();
			}
			this.numTeleports--;
			this.m_cooldown = this.teleportCooldown;
			bool flag7 = this.OnTeleport != null;
			if (flag7)
			{
				this.OnTeleport();
			}
			yield break;
		}

		// Token: 0x04000D3F RID: 3391
		public PlayerProjectileTeleportModifier.TeleportTrigger trigger;

		// Token: 0x04000D40 RID: 3392
		[ShowInInspectorIf("ShowMMinAngleToTeleport", true)]
		public float minAngleToTeleport;

		// Token: 0x04000D41 RID: 3393
		[ShowInInspectorIf("ShowDistToTeleport", true)]
		public float distToTeleport;

		// Token: 0x04000D42 RID: 3394
		public PlayerProjectileTeleportModifier.TeleportType type;

		// Token: 0x04000D43 RID: 3395
		[ShowInInspectorIf("ShowBehindTargetDistance", true)]
		public float behindTargetDistance;

		// Token: 0x04000D44 RID: 3396
		public int numTeleports;

		// Token: 0x04000D45 RID: 3397
		public float teleportPauseTime;

		// Token: 0x04000D46 RID: 3398
		public float leadAmount;

		// Token: 0x04000D47 RID: 3399
		public float teleportCooldown;

		// Token: 0x04000D48 RID: 3400
		public VFXPool teleportVfx;

		// Token: 0x04000D49 RID: 3401
		private SpeculativeRigidbody m_targetRigidbody;

		// Token: 0x04000D4A RID: 3402
		private Vector3 m_startingPos;

		// Token: 0x04000D4B RID: 3403
		private bool m_isTeleporting;

		// Token: 0x04000D4C RID: 3404
		private float m_cooldown;

		// Token: 0x02000533 RID: 1331
		public enum TeleportTrigger
		{
			// Token: 0x0400154A RID: 5450
			AngleToTarget = 10,
			// Token: 0x0400154B RID: 5451
			DistanceFromTarget = 20
		}

		// Token: 0x02000534 RID: 1332
		public enum TeleportType
		{
			// Token: 0x0400154D RID: 5453
			BackToSpawn = 10,
			// Token: 0x0400154E RID: 5454
			BehindTarget = 20
		}
	}
}