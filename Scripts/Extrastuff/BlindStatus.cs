using System;
using System.Collections;
using System.Collections.Generic;
using ItemAPI;
using UnityEngine;

namespace Knives
{
	public class GameActorBlindEffect : AIActorDebuffEffect
	{


		public static GameObject BlindVFX;
		public GameActorBlindEffect BlindEffect;
		public static void Initialise()
		{

			BlindVFX = SpriteBuilder.SpriteFromResource("Knives/Resources/Blind.png", new GameObject("BlindIcon"));
			BlindVFX.SetActive(false);
			tk2dBaseSprite vfxSprite = BlindVFX.GetComponent<tk2dBaseSprite>();
			vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, vfxSprite.GetCurrentSpriteDef().position3);
			FakePrefab.MarkAsFakePrefab(BlindVFX);
			UnityEngine.Object.DontDestroyOnLoad(BlindVFX);

			
		}
		
		// Token: 0x060004DB RID: 1243 RVA: 0x0003D9E0 File Offset: 0x0003BBE0
		public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1f)
		{
			GameObject gameObject = new GameObject("fakeBody");
			this.m_fakeActor = gameObject.AddComponent<NonActor>();
			this.m_fakeActor.HasShadow = false;
			this.m_fakeTargetRigidbody = gameObject.AddComponent<SpeculativeRigidbody>();
			this.m_fakeTargetRigidbody.PixelColliders = new List<PixelCollider>();
			this.m_fakeTargetRigidbody.CollideWithTileMap = false;
			this.m_fakeTargetRigidbody.CollideWithOthers = false;
			this.m_fakeTargetRigidbody.CanBeCarried = false;
			this.m_fakeTargetRigidbody.CanBePushed = false;
			this.m_fakeTargetRigidbody.CanCarry = false;
			PixelCollider pixelCollider = new PixelCollider();
			pixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
			pixelCollider.CollisionLayer = CollisionLayer.TileBlocker;
			pixelCollider.ManualWidth = 4;
			pixelCollider.ManualHeight = 4;
			this.m_fakeTargetRigidbody.PixelColliders.Add(pixelCollider);

			m_fakeTargetRigidbody.transform.position = actor.GetAbsoluteParentRoom().GetRandomVisibleClearSpot(2, 2).ToCenterVector2();
			m_fakeTargetRigidbody.Reinitialize();
			actor.aiActor.OverrideTarget = m_fakeTargetRigidbody;

			base.OnEffectApplied(actor, effectData, partialAmount);

		}

		bool left = false;
        public SpeculativeRigidbody m_fakeTargetRigidbody;
        private NonActor m_fakeActor;
		public int reposition = 0;
		public int hitCount = 0;

        public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			if(actor.aiActor != null)
            {
                if (actor.aiActor.healthHaver.IsAlive)
                {
					bool flag = this.AffectsEnemies && actor is AIActor;
					if (flag && Time.timeScale != 0 && !GameManager.Instance.IsPaused)
					{

						hitCount++;
						if (hitCount == 75)
						{
							if (left == false)
							{
								left = true;
								if (actor.aiAnimator != null)
								{
									actor.aiAnimator.PlayHitState(Vector2.right);
								}

							}
							else
							{
								left = false;
								if (actor.aiAnimator != null)
								{
									actor.aiAnimator.PlayHitState(Vector2.left);
								}
							}
							hitCount = 0;
						}

						reposition++;
						if (reposition == 200)
						{
							m_fakeTargetRigidbody.transform.position = actor.GetAbsoluteParentRoom().GetRandomVisibleClearSpot(2, 2).ToCenterVector2();
							m_fakeTargetRigidbody.Reinitialize();
							actor.aiActor.OverrideTarget = m_fakeTargetRigidbody;

							if (actor.aiActor.aiShooter != null)
							{
								if (actor.aiActor.OverrideTarget != null)
								{
									Vector2 dir = actor.aiActor.OverrideTarget.UnitCenter - actor.aiActor.CenterPosition;
									actor.aiActor.aiShooter.ShootInDirection(dir);
								}

							}

							reposition = 0;
						}


					}

				}

			}

			base.EffectTick(actor, effectData);
		}
		
		
		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			
			actor.aiActor.OverrideTarget = null;
			UnityEngine.GameObject.Destroy(m_fakeTargetRigidbody.gameActor);
			base.OnEffectRemoved(actor, effectData);
		}
		
	}
}