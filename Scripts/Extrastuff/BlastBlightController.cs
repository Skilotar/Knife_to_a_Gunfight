using System;
using System.Collections;
using ItemAPI;
using UnityEngine;

namespace Knives
{
	public class BlastBlightedStatusController : MonoBehaviour
	{

		public BlastBlightedStatusController()
		{
			statused = false;
			
		}
		public static GameObject BlastBlightVFX;
		public static GameActorDecorationEffect BlastDummyEffect;
		public static void Initialise()
		{

			BlastBlightVFX = SpriteBuilder.SpriteFromResource("Knives/Resources/blast.png", new GameObject("BlightIcon"));
			BlastBlightVFX.SetActive(false);
			tk2dBaseSprite vfxSprite = BlastBlightVFX.GetComponent<tk2dBaseSprite>();
			vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, vfxSprite.GetCurrentSpriteDef().position3);
			FakePrefab.MarkAsFakePrefab(BlastBlightVFX);
			UnityEngine.Object.DontDestroyOnLoad(BlastBlightVFX);

			BlastDummyEffect = new GameActorDecorationEffect()
			{
				AffectsEnemies = true,
				OverheadVFX = BlastBlightVFX,
				AffectsPlayers = false,
				AppliesTint = false,
				AppliesDeathTint = false,
				AppliesOutlineTint = true,
				OutlineTintColor = new Color(.70f, .40f, .24f),
				duration = float.MaxValue,
				effectIdentifier = "blastIcon",
				resistanceType = EffectResistanceType.None,
				PlaysVFXOnActor = false,
				stackMode = GameActorEffect.EffectStackingMode.Ignore,
			};
		}
		public void Start()
		{
            if (base.GetComponent<AIActor>() != null)
            {
				this.aIActor = base.GetComponent<AIActor>();
				aIActor.healthHaver.OnDamaged += HealthHaver_OnDamaged;
			}
			
		}

        private void HealthHaver_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
			if (statused)
			{
				if (aIActor.healthHaver)
				{
					if (aIActor.healthHaver.IsAlive)
					{
						if(hitstillpop == 0)
                        {
							//boom


							if (aIActor.gameObject.GetOrAddComponent<HexStatusEffectController>().statused)
							{
								this.DoSaferExplosion(aIActor.CenterPosition);
								AkSoundEngine.PostEvent("Play_BlastBlight", base.gameObject);
								GameObject Blast = SpawnManager.SpawnVFX(EasyVFXDatabase.Hexplosion, aIActor.sprite.WorldCenter, Quaternion.identity);
								Blast.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(aIActor.CenterPosition + new Vector2(0, 0f), tk2dBaseSprite.Anchor.MiddleCenter);
								Blast.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
								SpreadHex(aIActor.CenterPosition);
								
							}
							else
							{
								this.DoSafeExplosion(aIActor.sprite.WorldCenter);
								AkSoundEngine.PostEvent("Play_BlastBlight", base.gameObject);
								GameObject Blast = SpawnManager.SpawnVFX(EasyVFXDatabase.BlastBlight, aIActor.sprite.WorldCenter, Quaternion.identity);

								Blast.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(aIActor.CenterPosition + new Vector2(0, 0f), tk2dBaseSprite.Anchor.MiddleCenter);
								Blast.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");

							}

							//reset
							statused = false;
							hitstillpop = 3;
                        }
						if(hitstillpop > 0)
                        {
							hitstillpop--;
                        }

					}
                    else
                    {


						if (aIActor.gameObject.GetOrAddComponent<HexStatusEffectController>().statused)
						{
							this.DoSaferExplosion(aIActor.sprite.WorldCenter);
							AkSoundEngine.PostEvent("Play_BlastBlight", base.gameObject);
							GameObject Blast = SpawnManager.SpawnVFX(EasyVFXDatabase.Hexplosion, aIActor.sprite.WorldCenter, Quaternion.identity);
							Blast.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(aIActor.CenterPosition + new Vector2(0, 0f), tk2dBaseSprite.Anchor.MiddleCenter);
							Blast.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
							SpreadHex(aIActor.CenterPosition);


						}
						else
						{
							this.DoSafeExplosion(aIActor.sprite.WorldCenter);
							AkSoundEngine.PostEvent("Play_BlastBlight", base.gameObject);
							GameObject Blast = SpawnManager.SpawnVFX(EasyVFXDatabase.BlastBlight, aIActor.sprite.WorldCenter,Quaternion.identity);
							
							Blast.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(aIActor.CenterPosition + new Vector2(0, 0f), tk2dBaseSprite.Anchor.MiddleCenter);
							Blast.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");

						}
						
						statused = false;
						
						
                    }
				}
			}
		}

        public void SpreadHex(Vector2 posi)
        {

            if (posi.GetPositionOfNearestEnemy(true) != null)
			{
				posi.GetAbsoluteRoom().ApplyActionToNearbyEnemies(posi, 6f, new Action<AIActor, float>(this.ProcessEnemy));
			}

			
			
		}
		public void ProcessEnemy(AIActor arg1, float arg2)
		{

			HexStatusEffectController hex = arg1.gameObject.GetOrAddComponent<HexStatusEffectController>();
			hex.statused = true;
		}

		public void Update()
		{
            if (statused)
            {
				if(aIActor != null)
                {
					if (aIActor.healthHaver)
					{
						if (aIActor.healthHaver.IsAlive)
						{
							//visual effects
							aIActor.ApplyEffect(BlastDummyEffect);
							aIActor.ForceBlackPhantomParticles = true;

						}
					}
				}
			
			}
            else
            {
				if(aIActor != null)
                {
					aIActor.RemoveEffect(BlastDummyEffect);
					aIActor.ForceBlackPhantomParticles = false;
					aIActor.ClearOverrideOutlineColor();
				}
				
			}
		}

		public void DoSafeExplosion(Vector3 position)
		{

			ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
			this.smallPlayerSafeExplosion.effect = null;
			this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
			this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
			Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);

		}

		private ExplosionData smallPlayerSafeExplosion = new ExplosionData
		{
			damageRadius = 4f,
			damageToPlayer = 0f,
			doDamage = true,
			damage = 30f,
			doDestroyProjectiles = true,
			doForce = false,
			debrisForce = 0f,
			preventPlayerForce = false,
			explosionDelay = 0.0f,
			usesComprehensiveDelay = false,
			doScreenShake = false,
			playDefaultSFX = true,
			


		};
		public void DoSaferExplosion(Vector3 position)
		{

			ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
			this.smallPlayerSaferExplosion.effect = null;
			this.smallPlayerSaferExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
			this.smallPlayerSaferExplosion.ss = defaultSmallExplosionData2.ss;
			Exploder.Explode(position, this.smallPlayerSaferExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);

		}

		private ExplosionData smallPlayerSaferExplosion = new ExplosionData
		{
			damageRadius = 4f,
			damageToPlayer = 0f,
			doDamage = true,
			damage = 10f,
			doDestroyProjectiles = true,
			doForce = false,
			debrisForce = 0f,
			preventPlayerForce = false,
			explosionDelay = 0.0f,
			usesComprehensiveDelay = false,
			doScreenShake = false,
			playDefaultSFX = true,



		};
		public bool statused = false;
		public int hitstillpop = 2;
		public bool lit = false;
		
		private AIActor aIActor;
	}
}
