using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Knives
{
    static class BeamToolbox
    {
		
			public static bool PosIsNearAnyBoneOnBeam(this BasicBeamController beam, Vector2 positionToCheck, float distance)
			{
				LinkedList<BasicBeamController.BeamBone> linkedList = OMITBReflectionHelpers.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", beam);
				foreach (BasicBeamController.BeamBone bone in linkedList)
				{
					Vector2 bonePosition = BeamToolbox.GetBonePosition(beam,bone);
					bool flag = Vector2.Distance(positionToCheck, bonePosition) < distance;
					if (flag)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06000CFA RID: 3322 RVA: 0x000A9F08 File Offset: 0x000A8108
			public static int GetBoneCount(this BasicBeamController beam)
			{
				bool flag = !beam.UsesBones;
				int result;
				if (flag)
				{
					result = 1;
				}
				else
				{
					LinkedList<BasicBeamController.BeamBone> source = OMITBReflectionHelpers.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", beam);
					result = source.Count<BasicBeamController.BeamBone>();
				}
				return result;
			}

			// Token: 0x06000CFB RID: 3323 RVA: 0x000A9F4C File Offset: 0x000A814C
			public static float GetFinalBoneDirection(this BasicBeamController beam)
			{
				bool flag = !beam.UsesBones;
				float result;
				if (flag)
				{
					result = beam.Direction.ToAngle();
				}
				else
				{
					LinkedList<BasicBeamController.BeamBone> linkedList = OMITBReflectionHelpers.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", beam);
					LinkedListNode<BasicBeamController.BeamBone> last = linkedList.Last;
					result = last.Value.RotationAngle;
				}
				return result;
			}

			// Token: 0x06000CFC RID: 3324 RVA: 0x000A9FA4 File Offset: 0x000A81A4
			public static BasicBeamController.BeamBone GetIndexedBone(this BasicBeamController beam, int boneIndex)
			{
				LinkedList<BasicBeamController.BeamBone> linkedList = OMITBReflectionHelpers.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", beam);
				bool flag = linkedList == null;
				BasicBeamController.BeamBone result;
				if (flag)
				{
					result = null;
				}
				else
				{
					bool flag2 = linkedList.ElementAt(boneIndex) == null;
					if (flag2)
					{
						Debug.LogError("Attempted to fetch a beam bone at an invalid index");
						result = null;
					}
					else
					{
						result = linkedList.ElementAt(boneIndex);
					}
				}
				return result;
			}

			// Token: 0x06000CFD RID: 3325 RVA: 0x000AA000 File Offset: 0x000A8200
			public static Vector2 GetIndexedBonePosition(this BasicBeamController beam, int boneIndex)
			{
				LinkedList<BasicBeamController.BeamBone> source = OMITBReflectionHelpers.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", beam);
				bool flag = source.ElementAt(boneIndex) == null;
				Vector2 result;
				if (flag)
				{
					Debug.LogError("Attempted to fetch the position of a beam bone at an invalid index");
					result = Vector2.zero;
				}
				else
				{
					bool flag2 = !beam.UsesBones;
					if (flag2)
					{
						result = beam.Origin + BraveMathCollege.DegreesToVector(beam.Direction.ToAngle(), source.ElementAt(boneIndex).PosX);
					}
					else
					{
						bool flag3 = beam.ProjectileAndBeamMotionModule != null;
						if (flag3)
						{
							result = source.ElementAt(boneIndex).Position + beam.ProjectileAndBeamMotionModule.GetBoneOffset(source.ElementAt(boneIndex), beam, beam.projectile.Inverted);
						}
						else
						{
							result = source.ElementAt(boneIndex).Position;
						}
					}
				}
				return result;
			}

			// Token: 0x06000CFE RID: 3326 RVA: 0x000AA0D4 File Offset: 0x000A82D4
			public static Vector2 GetBonePosition(this BasicBeamController beam, BasicBeamController.BeamBone bone)
			{
				bool flag = !beam.UsesBones;
				Vector2 result;
				if (flag)
				{
					result = beam.Origin + BraveMathCollege.DegreesToVector(beam.Direction.ToAngle(), bone.PosX);
				}
				else
				{
					bool flag2 = beam.ProjectileAndBeamMotionModule != null;
					if (flag2)
					{
						result = bone.Position + beam.ProjectileAndBeamMotionModule.GetBoneOffset(bone, beam, beam.projectile.Inverted);
					}
					else
					{
						result = bone.Position;
					}
				}
				return result;
			}

			// Token: 0x06000CFF RID: 3327 RVA: 0x000AA154 File Offset: 0x000A8354
			public static BasicBeamController GenerateBeamPrefab(this Projectile projectile, string spritePath, Vector2 colliderDimensions, Vector2 colliderOffsets, List<string> beamAnimationPaths = null, int beamFPS = -1, List<string> impactVFXAnimationPaths = null, int beamImpactFPS = -1, Vector2? impactVFXColliderDimensions = null, Vector2? impactVFXColliderOffsets = null, List<string> endVFXAnimationPaths = null, int beamEndFPS = -1, Vector2? endVFXColliderDimensions = null, Vector2? endVFXColliderOffsets = null, List<string> muzzleVFXAnimationPaths = null, int beamMuzzleFPS = -1, Vector2? muzzleVFXColliderDimensions = null, Vector2? muzzleVFXColliderOffsets = null, float glowAmount = 0f, float emissivecolouramt = 0f)
			{
				BasicBeamController result;
				try
				{
					projectile.specRigidbody.CollideWithOthers = false;
					float num = colliderDimensions.x / 16f;
					float num2 = colliderDimensions.y / 16f;
					float num3 = colliderOffsets.x / 16f;
					float num4 = colliderOffsets.y / 16f;
					int newSpriteId = SpriteBuilder.AddSpriteToCollection(spritePath, ETGMod.Databases.Items.ProjectileCollection);
					tk2dTiledSprite orAddComponent = projectile.gameObject.GetOrAddComponent<tk2dTiledSprite>();
					orAddComponent.SetSprite(ETGMod.Databases.Items.ProjectileCollection, newSpriteId);
					tk2dSpriteDefinition currentSpriteDef = orAddComponent.GetCurrentSpriteDef();
					currentSpriteDef.colliderVertices = new Vector3[]
					{
					new Vector3(num3, num4, 0f),
					new Vector3(num, num2, 0f)
					};
					currentSpriteDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleLeft, null, false, true);
					tk2dSpriteAnimator orAddComponent2 = projectile.gameObject.GetOrAddComponent<tk2dSpriteAnimator>();
					tk2dSpriteAnimation orAddComponent3 = projectile.gameObject.GetOrAddComponent<tk2dSpriteAnimation>();
					orAddComponent3.clips = new tk2dSpriteAnimationClip[0];
					orAddComponent2.Library = orAddComponent3;
					UnityEngine.Object.Destroy(projectile.GetComponentInChildren<tk2dSprite>());
					BasicBeamController orAddComponent4 = projectile.gameObject.GetOrAddComponent<BasicBeamController>();
					projectile.sprite = orAddComponent;
					bool flag = beamAnimationPaths != null;
					if (flag)
					{
						tk2dSpriteAnimationClip tk2dSpriteAnimationClip = new tk2dSpriteAnimationClip
						{
							name = "beam_idle",
							frames = new tk2dSpriteAnimationFrame[0],
							fps = (float)beamFPS
						};
						List<tk2dSpriteAnimationFrame> list = new List<tk2dSpriteAnimationFrame>();
						foreach (string resourcePath in beamAnimationPaths)
						{
							tk2dSpriteCollectionData projectileCollection = ETGMod.Databases.Items.ProjectileCollection;
							int num5 = SpriteBuilder.AddSpriteToCollection(resourcePath, projectileCollection);
							tk2dSpriteDefinition tk2dSpriteDefinition = projectileCollection.spriteDefinitions[num5];
							tk2dSpriteDefinition.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleLeft, null, false, true);
							tk2dSpriteDefinition.colliderVertices = currentSpriteDef.colliderVertices;
							list.Add(new tk2dSpriteAnimationFrame
							{
								spriteId = num5,
								spriteCollection = projectileCollection
							});
						}
						tk2dSpriteAnimationClip.frames = list.ToArray();
						orAddComponent3.clips = orAddComponent3.clips.Concat(new tk2dSpriteAnimationClip[]
						{
						tk2dSpriteAnimationClip
						}).ToArray<tk2dSpriteAnimationClip>();
						orAddComponent4.beamAnimation = "beam_idle";
					}
					bool flag2 = endVFXAnimationPaths != null && endVFXColliderDimensions != null && endVFXColliderOffsets != null;
					if (flag2)
					{
						BeamToolbox.SetupBeamPart(orAddComponent3, endVFXAnimationPaths, "beam_end", beamEndFPS, new Vector2?(endVFXColliderDimensions.Value), new Vector2?(endVFXColliderOffsets.Value), null);
						orAddComponent4.beamEndAnimation = "beam_end";
					}
					else
					{
						BeamToolbox.SetupBeamPart(orAddComponent3, beamAnimationPaths, "beam_end", beamFPS, null, null, currentSpriteDef.colliderVertices);
						orAddComponent4.beamEndAnimation = "beam_end";
					}
					bool flag3 = impactVFXAnimationPaths != null && impactVFXColliderDimensions != null && impactVFXColliderOffsets != null;
					if (flag3)
					{
						BeamToolbox.SetupBeamPart(orAddComponent3, impactVFXAnimationPaths, "beam_impact", beamImpactFPS, new Vector2?(impactVFXColliderDimensions.Value), new Vector2?(impactVFXColliderOffsets.Value), null);
						orAddComponent4.impactAnimation = "beam_impact";
					}
					bool flag4 = muzzleVFXAnimationPaths != null && muzzleVFXColliderDimensions != null && muzzleVFXColliderOffsets != null;
					if (flag4)
					{
						BeamToolbox.SetupBeamPart(orAddComponent3, muzzleVFXAnimationPaths, "beam_start", beamMuzzleFPS, new Vector2?(muzzleVFXColliderDimensions.Value), new Vector2?(muzzleVFXColliderOffsets.Value), null);
						orAddComponent4.beamStartAnimation = "beam_start";
					}
					else
					{
						BeamToolbox.SetupBeamPart(orAddComponent3, beamAnimationPaths, "beam_start", beamFPS, null, null, currentSpriteDef.colliderVertices);
						orAddComponent4.beamStartAnimation = "beam_start";
					}
					bool flag5 = glowAmount > 0f;
					if (flag5)
					{
						EmmisiveBeams orAddComponent5 = projectile.gameObject.GetOrAddComponent<EmmisiveBeams>();
						orAddComponent5.EmissivePower = glowAmount;
						bool flag6 = emissivecolouramt != 0f;
						if (flag6)
						{
							orAddComponent5.EmissiveColorPower = emissivecolouramt;
						}
					}
					result = orAddComponent4;
				}
				catch (Exception ex)
				{
					ETGModConsole.Log(ex.ToString(), false);
					result = null;
				}
				return result;
			}

			// Token: 0x06000D00 RID: 3328 RVA: 0x000AA5A0 File Offset: 0x000A87A0
			public static void SetupBeamPart(tk2dSpriteAnimation beamAnimation, List<string> animSpritePaths, string animationName, int fps, Vector2? colliderDimensions = null, Vector2? colliderOffsets = null, Vector3[] overrideVertices = null)
			{
				tk2dSpriteAnimationClip tk2dSpriteAnimationClip = new tk2dSpriteAnimationClip
				{
					name = animationName,
					frames = new tk2dSpriteAnimationFrame[0],
					fps = (float)fps
				};
				List<tk2dSpriteAnimationFrame> list = new List<tk2dSpriteAnimationFrame>();
				foreach (string resourcePath in animSpritePaths)
				{
					tk2dSpriteCollectionData projectileCollection = ETGMod.Databases.Items.ProjectileCollection;
					int num = SpriteBuilder.AddSpriteToCollection(resourcePath, projectileCollection);
					tk2dSpriteDefinition tk2dSpriteDefinition = projectileCollection.spriteDefinitions[num];
					tk2dSpriteDefinition.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, null, false, true);
					bool flag = overrideVertices != null;
					if (flag)
					{
						tk2dSpriteDefinition.colliderVertices = overrideVertices;
					}
					else
					{
						bool flag2 = colliderDimensions == null || colliderOffsets == null;
						if (flag2)
						{
							ETGModConsole.Log("<size=100><color=#ff0000ff>BEAM ERROR: colliderDimensions or colliderOffsets was null with no override vertices!</color></size>", false);
						}
						else
						{
							Vector2 value = colliderDimensions.Value;
							Vector2 value2 = colliderDimensions.Value;
							tk2dSpriteDefinition.colliderVertices = new Vector3[]
							{
							new Vector3(value2.x / 16f, value2.y / 16f, 0f),
							new Vector3(value.x / 16f, value.y / 16f, 0f)
							};
						}
					}
					list.Add(new tk2dSpriteAnimationFrame
					{
						spriteId = num,
						spriteCollection = projectileCollection
					});
				}
				tk2dSpriteAnimationClip.frames = list.ToArray();
				beamAnimation.clips = beamAnimation.clips.Concat(new tk2dSpriteAnimationClip[]
				{
				tk2dSpriteAnimationClip
				}).ToArray<tk2dSpriteAnimationClip>();
			}

			// Token: 0x06000D01 RID: 3329 RVA: 0x000AA76C File Offset: 0x000A896C
			public static BeamController FreeFireBeamFromAnywhere(Projectile projectileToSpawn, PlayerController owner, GameObject otherShooter, Vector2 fixedPosition, bool usesFixedPosition, float targetAngle, float duration, bool skipChargeTime = false, bool followDirOnProjectile = false, float angleOffsetFromProjectileAngle = 0f)
			{
				Vector2 vector = Vector2.zero;
				SpeculativeRigidbody speculativeRigidbody = null;
				if (usesFixedPosition)
				{
					vector = fixedPosition;
				}
				else
				{
					bool flag = otherShooter.GetComponent<SpeculativeRigidbody>();
					if (flag)
					{
						speculativeRigidbody = otherShooter.GetComponent<SpeculativeRigidbody>();
					}
					else
					{
						bool flag2 = otherShooter.GetComponentInChildren<SpeculativeRigidbody>();
						if (flag2)
						{
							speculativeRigidbody = otherShooter.GetComponentInChildren<SpeculativeRigidbody>();
						}
					}
					bool flag3 = speculativeRigidbody;
					if (flag3)
					{
						vector = speculativeRigidbody.UnitCenter;
					}
				}
				bool flag4 = vector != Vector2.zero;
				BeamController result;
				if (flag4)
				{
					GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, vector, Quaternion.identity, true);
					Projectile component = gameObject.GetComponent<Projectile>();
					component.Owner = owner;
					BeamController component2 = gameObject.GetComponent<BeamController>();
					if (skipChargeTime)
					{
						component2.chargeDelay = 0f;
						component2.usesChargeDelay = false;
					}
					component2.Owner = owner;
					component2.HitsPlayers = false;
					component2.HitsEnemies = true;
					Vector3 vector2 = BraveMathCollege.DegreesToVector(targetAngle, 1f);
					bool flag5 = otherShooter != null && !usesFixedPosition && otherShooter.GetComponent<Projectile>() && followDirOnProjectile;
					if (flag5)
					{
						component2.Direction = (otherShooter.GetComponent<Projectile>().Direction.ToAngle() + angleOffsetFromProjectileAngle).DegreeToVector2();
					}
					else
					{
						component2.Direction = vector2;
					}
					component2.Origin = vector;
					GameManager.Instance.Dungeon.StartCoroutine(BeamToolbox.HandleFreeFiringBeam(component2, speculativeRigidbody, fixedPosition, usesFixedPosition, targetAngle, duration, followDirOnProjectile, angleOffsetFromProjectileAngle));
					result = component2;
				}
				else
				{
					ETGModConsole.Log("ERROR IN BEAM FREEFIRE CODE. SOURCEPOS WAS NULL, EITHER DUE TO INVALID FIXEDPOS OR SOURCE GAMEOBJECT.", false);
					result = null;
				}
				return result;
			}

			// Token: 0x06000D02 RID: 3330 RVA: 0x000AA904 File Offset: 0x000A8B04
			public static IEnumerator HandleFreeFiringBeam(BeamController beam, SpeculativeRigidbody otherShooter, Vector2 fixedPosition, bool usesFixedPosition, float targetAngle, float duration, bool followProjDir, float projFollowOffset)
			{
				float elapsed = 0f;
				yield return null;
				while (elapsed < duration)
				{
					bool flag = otherShooter == null;
					if (flag)
					{
						break;
					}
					Vector2 sourcePos;
					if (usesFixedPosition)
					{
						sourcePos = fixedPosition;
					}
					else
					{
						sourcePos = otherShooter.UnitCenter;
					}
					elapsed += BraveTime.DeltaTime;
					bool flag2 = otherShooter != null && !usesFixedPosition && otherShooter.GetComponent<Projectile>() && followProjDir;
					if (flag2)
					{
						beam.Direction = (otherShooter.GetComponent<Projectile>().Direction.ToAngle() + projFollowOffset).DegreeToVector2();
					}
					beam.Origin = sourcePos;
					beam.LateUpdatePosition(sourcePos);
					yield return null;
					sourcePos = default(Vector2);
				}
				beam.CeaseAttack();
				yield break;
			}
		}
	}

