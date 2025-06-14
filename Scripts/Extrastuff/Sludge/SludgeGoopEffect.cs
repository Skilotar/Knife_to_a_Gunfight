using ItemAPI;
using System;
using System.Collections;
using UnityEngine;

namespace Knives
{
	// Token: 0x020000F5 RID: 245
	public class GameActorSludgeEffect : AIActorDebuffEffect
	{
		// Token: 0x060004DA RID: 1242 RVA: 0x0003D980 File Offset: 0x0003BB80
		public GameActorSludgeEffect()
		{
			this.TintColor = ExtendedColours.lime;
			this.DeathTintColor = ExtendedColours.lime;
			this.AppliesTint = true;
			this.AppliesDeathTint = true;
			this.AffectsPlayers = false;
			this.AffectsEnemies = true;
			this.duration = 2;
			this.effectIdentifier = "Sludged";
			this.sludgeAccum = 0f;
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x0003D9E0 File Offset: 0x0003BBE0
		public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1f)
		{
			actor.RemoveEffect("fire");
			base.OnEffectApplied(actor, effectData, partialAmount);
		}

		
		public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			bool flag = this.AffectsEnemies && actor is AIActor;
			if (flag)
			{
				float num = 1f;
				GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
				bool flag2 = lastLoadedLevelDefinition != null;
				if (flag2)
				{
					num = lastLoadedLevelDefinition.enemyHealthMultiplier;
				}
				if(actor.healthHaver.GetCurrentHealth() <= 6f * num * BraveTime.DeltaTime)
                {
					AIActor aiActor = actor.aiActor;
					if (aiActor && aiActor.IsNormalEnemy && (!aiActor.healthHaver || !aiActor.healthHaver.IsBoss))
					{
						GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemyDeath(aiActor, Vector2.zero));
					}
				}
                else
                {
					actor.renderer.material.SetFloat("_BurnAmount", .2f);
					actor.healthHaver.ApplyDamage(3f * num * BraveTime.DeltaTime, Vector2.zero, "ewwwww Grosss", CoreDamageTypes.Poison, DamageCategory.Normal, true, null, true);
				}
				
				
			}

			base.EffectTick(actor, effectData);
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x0003DBF8 File Offset: 0x0003BDF8
		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			actor.renderer.material.SetFloat("_BurnAmount", 0f);
			base.OnEffectRemoved(actor, effectData);
		}
		private IEnumerator HandleEnemyDeath(AIActor target, Vector2 motionDirection)
		{
			
			target.EraseFromExistenceWithRewards(false);
			Transform copyTransform = this.CreateEmptySprite(target);
			tk2dSprite copySprite = copyTransform.GetComponentInChildren<tk2dSprite>();
			copySprite.SetColor(ExtendedColours.lime);

			//copySprite.UpdateColors();
			float elapsed = 0f;
			float duration = 1f;
			
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				float t = elapsed / duration;
				
				copySprite.renderer.material.SetFloat("_BurnAmount", t);
				copyTransform.position += motionDirection.ToVector3ZisY(0f).normalized * BraveTime.DeltaTime * 1f;
				yield return null;
			}

			GoopDefinition fire = EasyGoopDefinitions.SludgeGoop;
			DeadlyDeadlyGoopManager goop = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(fire);
			goop.TimedAddGoopCircle(copyTransform.position, 1.5f, 1f, true);
			UnityEngine.Object.Destroy(copyTransform.gameObject);
			yield break;
		}
		private Transform CreateEmptySprite(AIActor target)
		{
			GameObject gameObject = new GameObject("suck image");
			gameObject.layer = target.gameObject.layer;
			tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
			gameObject.transform.parent = SpawnManager.Instance.VFX;
			tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
			tk2dSprite.transform.position = target.sprite.transform.position;
			GameObject gameObject2 = new GameObject("image parent");
			gameObject2.transform.position = tk2dSprite.WorldCenter;
			tk2dSprite.transform.parent = gameObject2.transform;
			tk2dSprite.usesOverrideMaterial = true;
			if (target.optionalPalette != null)
			{
				tk2dSprite.renderer.material.SetTexture("_PaletteTex", target.optionalPalette);
			}
			if (tk2dSprite.renderer.material.shader.name.Contains("ColorEmissive"))
			{
			}
			return gameObject2.transform;
		}

		private float sludgeAccum;
		
	}
}