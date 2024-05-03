using System;
using ItemAPI;
using UnityEngine;

namespace Knives
{
	// Token: 0x0200007F RID: 127
	public class GameActorillnessEffect : GameActorHealthEffect
	{
		// Token: 0x060003D7 RID: 983 RVA: 0x00025DA0 File Offset: 0x00023FA0
		public static void Init()
		{
			GameActorillnessEffect.Illness = SpriteBuilder.SpriteFromResource("Knives/Resources/Illness", new GameObject("IllnessIcon"));
			GameActorillnessEffect.Illness.SetActive(false);
			tk2dBaseSprite component = GameActorillnessEffect.Illness.GetComponent<tk2dBaseSprite>();
			component.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, component.GetCurrentSpriteDef().position3, true);
			FakePrefab.MarkAsFakePrefab(GameActorillnessEffect.Illness);
			UnityEngine.Object.DontDestroyOnLoad(GameActorillnessEffect.Illness);
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x00025E14 File Offset: 0x00024014
		public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1f)
		{
			Transform transform = actor.transform.Find("IllnessVFX");
			bool flag = transform == null;
			if (flag)
			{
				

				GameObject gameObject = GameActorillnessEffect.Illness;
				tk2dSprite component = UnityEngine.Object.Instantiate<GameObject>(gameObject, actor.specRigidbody.UnitTopCenter, Quaternion.identity, actor.transform).GetComponent<tk2dSprite>();
				component.transform.position.WithZ(component.transform.position.z + 99999f);
				component.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(actor.CenterPosition, tk2dBaseSprite.Anchor.MiddleCenter);
				actor.sprite.AttachRenderer(component.GetComponent<tk2dBaseSprite>());
				component.name = GameActorillnessEffect.vfxName;
				component.PlaceAtPositionByAnchor(actor.sprite.WorldTopCenter, tk2dBaseSprite.Anchor.LowerCenter);
				component.scale = Vector3.one;

				if (RatCrown.HasSynergy)
				{
					victim = actor.gameObject.GetComponent<AIActor>();

					actor.healthHaver.OnPreDeath += HealthHaver_OnPreDeath;
				}
			}
			else
			{
				base.OnEffectRemoved(actor, effectData);
			}
		}

		public AIActor victim;

		// Token: 0x060003D9 RID: 985 RVA: 0x00025F00 File Offset: 0x00024100
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

				actor.healthHaver.ApplyDamage(3f * num * BraveTime.DeltaTime, Vector2.zero, "I Dont Feel So Good", CoreDamageTypes.Poison, DamageCategory.Normal, true, null, true);
			}
			
		}

        private void HealthHaver_OnPreDeath(Vector2 obj)
        {
			RatCrown.SpawnRat(GameManager.Instance.BestActivePlayer, victim.sprite.WorldCenter);
			RatCrown.SpawnRat(GameManager.Instance.BestActivePlayer, victim.sprite.WorldCenter);
		}

        // Token: 0x060003DA RID: 986 RVA: 0x00025F74 File Offset: 0x00024174
        public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			bool flag = this.AffectsEnemies && actor is AIActor;
			
			base.OnEffectRemoved(actor, effectData);
			actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
		}

		// Token: 0x040002E6 RID: 742
		public static string vfxName = "IllnessVFX";

		// Token: 0x040002E7 RID: 743
		public static GameObject Illness;
	}
}
