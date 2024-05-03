using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dungeonator;
using Knives;
using UnityEngine;

namespace Knives
{
	// Token: 0x0200001B RID: 27
	public class SlashDoer
	{
		// Token: 0x060000B8 RID: 184 RVA: 0x000074C4 File Offset: 0x000056C4
		public static void DoSwordSlash(Vector2 position, float angle, PlayerController owner, float playerKnockbackForce, SlashDoer.ProjInteractMode intmode, float damageToDeal, float enemyKnockbackForce, List<GameActorEffect> statusEffects = null, Transform parentTransform = null, float jammedDamageMult = 1f, float bossDamageMult = 1f, float SlashRange = 2.5f, float SlashDimensions = 90f, Projectile sourceProjectile = null)
		{
			GameManager.Instance.StartCoroutine(SlashDoer.HandleSlash(position, angle, owner, playerKnockbackForce, intmode, damageToDeal, enemyKnockbackForce, statusEffects, jammedDamageMult, bossDamageMult, SlashRange, SlashDimensions, sourceProjectile));
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x000074F8 File Offset: 0x000056F8
		private static IEnumerator HandleSlash(Vector2 position, float angle, PlayerController owner, float knockbackForce, SlashDoer.ProjInteractMode intmode, float damageToDeal, float enemyKnockback, List<GameActorEffect> statusEffects, float jammedDMGMult, float bossDMGMult, float SlashRange, float SlashDimensions, Projectile sourceProjectile = null)
		{
			int slashId = Time.frameCount;
			List<SpeculativeRigidbody> alreadyHit = new List<SpeculativeRigidbody>();
			bool flag = knockbackForce != 0f && owner != null;
			if (flag)
			{
				owner.knockbackDoer.ApplyKnockback(BraveMathCollege.DegreesToVector(angle, 1f), knockbackForce, 0.25f, false);
			}
			float ela = 0f;
			while (ela < 0.2f)
			{
				ela += BraveTime.DeltaTime;
				SlashDoer.HandleHeroSwordSlash(alreadyHit, position, angle, slashId, owner, intmode, damageToDeal, enemyKnockback, statusEffects, jammedDMGMult, bossDMGMult, SlashRange, SlashDimensions, sourceProjectile);
				yield return null;
			}
			yield break;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00007570 File Offset: 0x00005770
		private static bool ProjectileIsValid(Projectile proj)
		{
			return proj && (!(proj.Owner is PlayerController) || proj.ForcePlayerBlankable);
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000075AC File Offset: 0x000057AC
		private static bool ObjectWasHitBySlash(Vector2 ObjectPosition, Vector2 SlashPosition, float slashAngle, float SlashRange, float SlashDimensions)
		{
			bool flag = Vector2.Distance(ObjectPosition, SlashPosition) < SlashRange;
			bool result;
			if (flag)
			{
				float numberToCheck = BraveMathCollege.Atan2Degrees(ObjectPosition - SlashPosition);
				float num = Math.Min(SlashDimensions, -SlashDimensions);
				float num2 = Math.Max(SlashDimensions, -SlashDimensions);
				bool flag2 = false;
				float num3 = slashAngle + num2;
				float num4 = slashAngle + num;
				bool flag3 = numberToCheck.IsBetweenRange(num4, num3);
				if (flag3)
				{
					flag2 = true;
				}
				bool flag4 = num3 > 180f;
				if (flag4)
				{
					float num5 = num3 - 180f;
					bool flag5 = numberToCheck.IsBetweenRange(-180f, -180f + num5);
					if (flag5)
					{
						flag2 = true;
					}
				}
				bool flag6 = num4 < -180f;
				if (flag6)
				{
					float num6 = num4 + 180f;
					bool flag7 = numberToCheck.IsBetweenRange(180f + num6, 180f);
					if (flag7)
					{
						flag2 = true;
					}
				}
				result = flag2;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public static float DeflectionDegree;
		// Token: 0x060000BC RID: 188 RVA: 0x0000768C File Offset: 0x0000588C
		private static void HandleHeroSwordSlash(List<SpeculativeRigidbody> alreadyHit, Vector2 arcOrigin, float slashAngle, int slashId, PlayerController owner, SlashDoer.ProjInteractMode intmode, float damageToDeal, float enemyKnockback, List<GameActorEffect> statusEffects, float jammedDMGMult, float bossDMGMult, float slashRange, float slashDimensions, Projectile sourceProjectile = null)
		{
			bool hitEnemies = false;
			ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
			for (int i = allProjectiles.Count - 1; i >= 0; i--)
			{
				Projectile projectile = allProjectiles[i];
				bool flag = SlashDoer.ProjectileIsValid(projectile);
				if (flag)
				{
					Vector2 worldCenter = projectile.sprite.WorldCenter;
					bool flag2 = SlashDoer.ObjectWasHitBySlash(worldCenter, arcOrigin, slashAngle, slashRange, slashDimensions);
					if (flag2)
					{
						bool flag3 = intmode != SlashDoer.ProjInteractMode.IGNORE || projectile.collidesWithProjectiles;
						if (flag3)
						{
							bool flag4 = intmode == SlashDoer.ProjInteractMode.DESTROY || intmode == SlashDoer.ProjInteractMode.IGNORE;
							if (flag4)
							{

								projectile.DieInAir(false, true, true, true);
								if (owner.CurrentGun.GetComponent<GunSpecialStates>() != null)
                                {
									if (owner.CurrentGun.GetComponent<GunSpecialStates>().DoesMacRage)
									{
										projectile.DieInAir(false, true, true, true);
										owner.CurrentGun.GetComponent<GunSpecialStates>().RageCount = owner.CurrentGun.GetComponent<GunSpecialStates>().RageCount + 1;
									}
									if (owner.CurrentGun.GetComponent<GunSpecialStates>().DoesCountBlocks)
									{
										projectile.DieInAir(false, true, true, true);
										owner.CurrentGun.GetComponent<GunSpecialStates>().successfullBlocks = owner.CurrentGun.GetComponent<GunSpecialStates>().successfullBlocks + 1;
									}
								}
                            

							}
							else
							{
								bool flag5 = intmode == SlashDoer.ProjInteractMode.REFLECT;
								if (flag5)
								{
									bool flag6 = projectile.LastReflectedSlashId != slashId;
									if (flag6)
									{
										PassiveReflectItem.ReflectBullet(projectile, true, owner, 2f, 1f, 1f, 0f);
										projectile.LastReflectedSlashId = slashId;

									
									}
								}
								bool flag7 = intmode == SlashDoer.ProjInteractMode.DEFLECT;
								if (flag7)
								{
									bool flag8 = projectile.LastReflectedSlashId != slashId;
									if (flag8)
									{
										PassiveReflectItem.ReflectBullet(projectile, true, owner, 2f, 1f, 1f, DeflectionDegree);
										projectile.LastReflectedSlashId = slashId;


									}
								}
								bool flag9 = intmode == SlashDoer.ProjInteractMode.STOP;
								if (flag9)
								{
									bool flag10 = projectile.LastReflectedSlashId != slashId;
									if (flag10)
									{	
										// Hey future ski. I know youre coming back to try to fix the color thing
										// Don't try it
										// Seriously youre wasting your time again...
										PassiveReflectItem.ReflectBullet(projectile, true, owner, 2f, 1f, 4f, 0f);
										projectile.RemoveBulletScriptControl();
										projectile.BulletScriptSettings.preventPooling = true;
										projectile.baseData.speed *= 0f;
										
										projectile.baseData.damage *= 0.5f;
										projectile.UpdateSpeed();
										projectile.persistTime = 7;
										
										ChildProjCleanup cleanup = projectile.gameObject.GetOrAddComponent<ChildProjCleanup>();
										cleanup.delay = 7f;
										cleanup.parentProjectile = null;
										cleanup.doColorCleanup = false;
										cleanup.doColor = false;
									}
								}

								bool flag11 = intmode == SlashDoer.ProjInteractMode.SPLIT;
								if (flag11)
								{
									bool flag12 = projectile.LastReflectedSlashId != slashId;
									if (flag12)
									{
										
										PassiveReflectItem.ReflectBullet(projectile, true, owner, 2f, 1f, 4f, 0f);
										projectile.Direction = owner.CenterPosition - projectile.specRigidbody.UnitCenter;
										ProjectileSplitController split = projectile.gameObject.GetOrAddComponent<ProjectileSplitController>();
										split.distanceTillSplit = 0.1f;
										split.amtToSplitTo = 1;
										split.distanceBasedSplit = true;
										split.numberofsplits = 1;
										split.splitAngles = 50f;
										split.removeComponentAfterUse = true;
										projectile.LastReflectedSlashId = slashId;
										if(owner.CurrentGun.PickupObjectId == Chainsaw.StandardID) owner.CurrentGun.LoseAmmo(1);


									}
								}

								bool flag13 = intmode == SlashDoer.ProjInteractMode.RETURNPROJ;
								if (flag13)
								{
									
									bool flag14 = projectile.LastReflectedSlashId != slashId;
									if (flag14)
									{
										
										if (owner.CurrentGun.GetComponent<GunSpecialStates>() != null)
										{
											
											if (owner.CurrentGun.GetComponent<GunSpecialStates>().ReturnHitList)
											{
												
												owner.CurrentGun.GetComponent<GunSpecialStates>().returnList.Add(projectile);
												
												PassiveReflectItem.ReflectBullet(projectile, true, owner, 2f, 1f, 4f, 0f);
												projectile.RemoveBulletScriptControl();
												projectile.BulletScriptSettings.preventPooling = true;
												projectile.baseData.speed *= 0f;
												projectile.baseData.damage *= .5f;
												projectile.UpdateSpeed();
												
											}
										}
									}
								}
							}
						}
					}
				}
			}
			SlashDoer.DealDamageToEnemiesInArc(owner, arcOrigin, slashAngle, slashRange, damageToDeal, enemyKnockback, statusEffects, jammedDMGMult, bossDMGMult, slashDimensions, out hitEnemies, alreadyHit, sourceProjectile);
			List<MinorBreakable> allMinorBreakables = StaticReferenceManager.AllMinorBreakables;
			for (int j = allMinorBreakables.Count - 1; j >= 0; j--)
			{
				MinorBreakable minorBreakable = allMinorBreakables[j];
				bool flag7 = minorBreakable && minorBreakable.specRigidbody;
				if (flag7)
				{
					bool flag8 = !minorBreakable.IsBroken && minorBreakable.sprite;
					if (flag8)
					{
						bool flag9 = SlashDoer.ObjectWasHitBySlash(minorBreakable.sprite.WorldCenter, arcOrigin, slashAngle, slashRange, slashDimensions);
						if (flag9)
						{
							minorBreakable.Break();
						}
					}
				}
			}
			List<MajorBreakable> allMajorBreakables = StaticReferenceManager.AllMajorBreakables;
			for (int k = allMajorBreakables.Count - 1; k >= 0; k--)
			{
				MajorBreakable majorBreakable = allMajorBreakables[k];
				bool flag10 = majorBreakable && majorBreakable.specRigidbody;
				if (flag10)
				{
					bool flag11 = !alreadyHit.Contains(majorBreakable.specRigidbody);
					if (flag11)
					{
						bool flag12 = !majorBreakable.IsSecretDoor && !majorBreakable.IsDestroyed;
						if (flag12)
						{
							bool flag13 = SlashDoer.ObjectWasHitBySlash(majorBreakable.specRigidbody.UnitCenter, arcOrigin, slashAngle, slashRange, slashDimensions);
							if (flag13)
							{
								float num = damageToDeal;
								bool flag14 = majorBreakable.healthHaver;
								if (flag14)
								{
									num *= 0.2f;
								}
								majorBreakable.ApplyDamage(num, majorBreakable.specRigidbody.UnitCenter - arcOrigin, false, false, false);
								alreadyHit.Add(majorBreakable.specRigidbody);
							}
						}
					}
				}
			}
			bool flag15 = sourceProjectile != null;
			if (flag15)
			{
				bool flag16 = sourceProjectile.gameObject.GetComponent<ProjectileSlashingBehaviour>() != null;
				if (flag16)
				{
					ProjectileSlashingBehaviour component = sourceProjectile.gameObject.GetComponent<ProjectileSlashingBehaviour>();
					component.DoOnSlashEndEffects(hitEnemies);
				}
			}
		}

		// Token: 0x060000BD RID: 189 RVA: 0x0000798C File Offset: 0x00005B8C
		private static void DealDamageToEnemiesInArc(PlayerController owner, Vector2 arcOrigin, float arcAngle, float arcRadius, float overrideDamage, float overrideForce, List<GameActorEffect> statusEffects, float jammedDMGMult, float bossDMGMult, float slashDimensions, out bool flag2, List<SpeculativeRigidbody> alreadyHit = null, Projectile sourceProjectile = null)
		{
			flag2 = false;
			bool flag3 = true;
			RoomHandler currentRoom = owner.CurrentRoom;
			bool flag4 = currentRoom == null;
			if (!flag4)
			{
				List<AIActor> activeEnemies = currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
				bool flag5 = activeEnemies == null;
				if (!flag5)
				{
					for (int i = 0; i < activeEnemies.Count; i++)
					{
						AIActor aiactor = activeEnemies[i];
						bool flag6 = aiactor && aiactor.specRigidbody && aiactor.IsNormalEnemy && !aiactor.IsGone && aiactor.healthHaver;
						if (flag6)
						{
							bool flag7 = alreadyHit == null || !alreadyHit.Contains(aiactor.specRigidbody);
							if (flag7)
							{
								for (int j = 0; j < aiactor.healthHaver.NumBodyRigidbodies; j++)
								{
									SpeculativeRigidbody bodyRigidbody = aiactor.healthHaver.GetBodyRigidbody(j);
									PixelCollider hitboxPixelCollider = bodyRigidbody.HitboxPixelCollider;
									bool flag8 = hitboxPixelCollider != null;
									if (flag8)
									{
										Vector2 vector = BraveMathCollege.ClosestPointOnRectangle(arcOrigin, hitboxPixelCollider.UnitBottomLeft, hitboxPixelCollider.UnitDimensions);
										float dist = Vector2.Distance(vector, arcOrigin);
										bool flag9 = SlashDoer.ObjectWasHitBySlash(vector, arcOrigin, arcAngle, arcRadius, slashDimensions);
										if (flag9)
										{
											bool flag10 = true;
											int rayMask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable);
											RaycastResult raycastResult;
											bool flag11 = PhysicsEngine.Instance.Raycast(arcOrigin, vector - arcOrigin, dist, out raycastResult, true, true, rayMask, null, false, null, null) && raycastResult.SpeculativeRigidbody != bodyRigidbody;
											if (flag11)
											{
												flag10 = false;
											}
											RaycastResult.Pool.Free(ref raycastResult);
											bool flag12 = flag10;
											if (flag12)
											{
												float damage = SlashDoer.DealSwordDamageToEnemy(owner, aiactor, arcOrigin, vector, arcAngle, overrideDamage, overrideForce, statusEffects, bossDMGMult, jammedDMGMult, sourceProjectile);
												bool flag13 = flag3;
												if (flag13)
												{
													flag3 = false;
													bool flag14 = sourceProjectile != null;
													if (flag14)
													{
														bool flag15 = sourceProjectile.gameObject.GetComponent<ProjectileSlashingBehaviour>() != null;
														if (flag15)
														{
															ProjectileSlashingBehaviour component = sourceProjectile.gameObject.GetComponent<ProjectileSlashingBehaviour>();
															component.DoOnHitFirstEnemyEffects(aiactor);
														}
													}
												}
												bool flag16 = alreadyHit != null;
												if (flag16)
												{
													bool flag17 = alreadyHit.Count == 0;
													if (flag17)
													{
														StickyFrictionManager.Instance.RegisterSwordDamageStickyFriction(damage);
													}
													alreadyHit.Add(aiactor.specRigidbody);
													flag2 = true;
												}
												break;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00007BFC File Offset: 0x00005DFC
		private static float DealSwordDamageToEnemy(PlayerController owner, AIActor targetEnemy, Vector2 arcOrigin, Vector2 contact, float angle, float damage, float knockback, List<GameActorEffect> statusEffects, float bossDMGMult, float jammedDMGMult, Projectile sourceProjectile = null)
		{
			bool flag = targetEnemy.healthHaver;
			if (flag)
			{
				float num = damage;
				bool isBoss = targetEnemy.healthHaver.IsBoss;
				if (isBoss)
				{
					num *= bossDMGMult;
				}
				bool isBlackPhantom = targetEnemy.IsBlackPhantom;
				if (isBlackPhantom)
				{
					num *= jammedDMGMult;
				}
				targetEnemy.healthHaver.ApplyDamage(num, contact - arcOrigin, owner.ActorName, CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
				bool flag2 = SlashDoer.appliesStun;
				if (flag2)
				{
					bool flag3 = UnityEngine.Random.value <= SlashDoer.stunApplyChance;
					if (flag3)
					{
						targetEnemy.behaviorSpeculator.Stun(SlashDoer.stunTime, true);
					}
				}
			}
			bool flag4 = targetEnemy.knockbackDoer;
			if (flag4)
			{
				targetEnemy.knockbackDoer.ApplyKnockback(contact - arcOrigin, knockback, false);
			}
			bool flag5 = statusEffects != null && statusEffects.Count > 0;
			if (flag5)
			{
				foreach (GameActorEffect effect in statusEffects)
				{
					targetEnemy.ApplyEffect(effect, 1f, null);
				}
			}
			bool flag6 = sourceProjectile != null;
			if (flag6)
			{
				bool flag7 = sourceProjectile.gameObject.GetComponent<ProjectileSlashingBehaviour>() != null;
				if (flag7)
				{
					ProjectileSlashingBehaviour component = sourceProjectile.gameObject.GetComponent<ProjectileSlashingBehaviour>();
					component.DoOnHitEffects(targetEnemy, contact - arcOrigin);
				}
			}
			return damage;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00007D78 File Offset: 0x00005F78
		public static void GrabBoolsAndValuesAndShitForTheFuckingSlashingApplyEffect(bool AppliesStun, float StunApplyChance, float StunTime, float DeflectionDegree)
		{
			SlashDoer.appliesStun = AppliesStun;
			SlashDoer.stunApplyChance = StunApplyChance;
			SlashDoer.stunTime = StunTime;
			SlashDoer.DeflectionDegree = DeflectionDegree;
		}

		// Token: 0x04000099 RID: 153
		private static bool appliesStun;

		// Token: 0x0400009A RID: 154
		private static float stunApplyChance;

		// Token: 0x0400009B RID: 155
		private static float stunTime;

		

		// Token: 0x02000059 RID: 89
		public enum ProjInteractMode
		{
			// Token: 0x04000185 RID: 389
			IGNORE,
			// Token: 0x04000186 RID: 390
			DESTROY,
			// Token: 0x04000187 RID: 391
			REFLECT,

			DEFLECT,

			STOP,

			SPLIT,
            RETURNPROJ,
        }
	}
}

