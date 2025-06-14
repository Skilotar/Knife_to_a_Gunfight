using System;
using System.Collections;
using System.Collections.Generic;
using ItemAPI;
using UnityEngine;

namespace Knives
{
	public class SleepStatus : MonoBehaviour
	{

		public SleepStatus()
		{
			statused = false;
		}
		public static GameObject SleepVFX;
		public static GameActorDecorationEffect SleepDummyEffect;
		public static void Initialise()
		{

			SleepVFX = BuildPrefab();
			SleepVFX.SetActive(false);
			tk2dBaseSprite vfxSprite = SleepVFX.GetComponent<tk2dBaseSprite>();
			vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, vfxSprite.GetCurrentSpriteDef().position3);



			FakePrefab.MarkAsFakePrefab(SleepVFX);
			UnityEngine.Object.DontDestroyOnLoad(SleepVFX);

			

			SleepDummyEffect = new GameActorDecorationEffect()
			{
				AffectsEnemies = true,
				OverheadVFX = SleepVFX,
				AffectsPlayers = false,
				AppliesTint = true,
				AppliesDeathTint = false,
				AppliesOutlineTint = false,
				TintColor = ExtendedColours.cursedBulletsPurple,
				DeathTintColor = ExtendedColours.cursedBulletsPurple,
				OutlineTintColor = ExtendedColours.cursedBulletsPurple,
				duration = float.MaxValue,
				effectIdentifier = "Sleep",
				resistanceType = EffectResistanceType.None,
				PlaysVFXOnActor = false,
				stackMode = GameActorEffect.EffectStackingMode.Ignore,
			};
		}
		public static GameObject BuildPrefab()
		{
			bool flag = SleepStatus.SleepVFX != null;
			if (!flag)
			{
				GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/sleep", null);
				gameObject.name = "SleepVfx";
				
				tk2dSpriteAnimation animation = gameObject.GetOrAddComponent<tk2dSpriteAnimation>();
				tk2dSpriteAnimator animator = gameObject.GetOrAddComponent<tk2dSpriteAnimator>();
				tk2dSpriteCollectionData drone = SpriteBuilder.ConstructCollection(gameObject, ("sleep_Collection"));

				tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip() { name = "sleep_idle", frames = new tk2dSpriteAnimationFrame[0], fps = 5 };
				List<tk2dSpriteAnimationFrame> frames4 = new List<tk2dSpriteAnimationFrame>();
				for (int i = 1; i <= 4; i++)
				{
					tk2dSpriteCollectionData collection4 = drone;
					int frameSpriteId4;
					
						frameSpriteId4 = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/sleep/sleep_00{i}", collection4);
					
					tk2dSpriteDefinition frameDef4 = collection4.spriteDefinitions[frameSpriteId4];
					frames4.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId4, spriteCollection = collection4 });
				}
				clip.frames = frames4.ToArray();
				clip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
				animator.Library = animation;
				animator.Library.clips = new tk2dSpriteAnimationClip[] { clip };
				animator.DefaultClipId = animation.GetClipIdByName("sleep_idle");
				animator.playAutomatically = true;


				return gameObject;
			}
			return null;
		}
		public void Start()
		{
			if (base.GetComponent<AIActor>() != null)
			{
				this.aIActor = base.GetComponent<AIActor>();
				
				if (aIActor.healthHaver != null)
				{
                    aIActor.healthHaver.OnDamaged += HealthHaver_OnDamaged;
				}
			}

		}

        private void HealthHaver_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
			statused = false;
        }

		public void Update()
		{
			if (statused)
			{

				if (aIActor != null)
				{

                    if (!aIActor.healthHaver.IsBoss)
                    {
						if (aIActor.behaviorSpeculator.enabled == true)
						{
							aIActor.ApplyEffect(SleepDummyEffect);
							aIActor.behaviorSpeculator.Stun(.1f,false);
							
							aIActor.behaviorSpeculator.enabled = false;

						}
					}
                    else
                    {
						aIActor.ApplyEffect(StaticStatusEffects.frostBulletsEffect,.5f);
                    }
					
					
				}
				if (duration >= 0)
				{
					duration -= BraveTime.DeltaTime;
				}
				else
				{
					statused = false;
				}
			}
            else
            {
				if (aIActor != null)
				{
					if (!aIActor.healthHaver.IsBoss)
					{
						if (aIActor.behaviorSpeculator.enabled == false)
						{
							aIActor.RemoveEffect(SleepDummyEffect);
							
							aIActor.behaviorSpeculator.enabled = true;

						}
                    }
                    else
                    {

                    }
				}

			}
		}


		public bool statused = false;
		public float duration = 6;
		private AIActor aIActor;
	}
}