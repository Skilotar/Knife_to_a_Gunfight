using System;
using System.Collections;
using System.Collections.Generic;
using ItemAPI;
using UnityEngine;

namespace Knives
{
	public class CoolEnemiesStatusController : MonoBehaviour
	{

		public CoolEnemiesStatusController()
		{
			statused = false;
		}
		public static GameObject CoolVFX;
		public static GameActorDecorationEffect DummyEffect;
		public static void Initialise()
		{

			CoolVFX = SpriteBuilder.SpriteFromResource("Knives/Resources/Cool.png", new GameObject("CoolIcon"));
			CoolVFX.SetActive(false);
			tk2dBaseSprite vfxSprite = CoolVFX.GetComponent<tk2dBaseSprite>();
			vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, vfxSprite.GetCurrentSpriteDef().position3);
			FakePrefab.MarkAsFakePrefab(CoolVFX);
			UnityEngine.Object.DontDestroyOnLoad(CoolVFX);

			DummyEffect = new GameActorDecorationEffect()
			{
				AffectsEnemies = true,
				OverheadVFX = CoolVFX,
				AffectsPlayers = false,
				AppliesTint = false,
				AppliesDeathTint = false,
				AppliesOutlineTint = true,
				OutlineTintColor = ExtendedColours.gildedBulletsGold,
				duration = float.MaxValue,
				effectIdentifier = "CoolIcon",
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
				if (aIActor.bulletBank != null)
				{
					aIActor.bulletBank.OnProjectileCreated += this.OnAiTriedToShoot;
				}
				if (aIActor.aiShooter != null)
				{
					aIActor.aiShooter.PostProcessProjectile += this.OnAiTriedToShoot;
				}
				if(aIActor.healthHaver != null)
                {
                    aIActor.healthHaver.OnDeath += HealthHaver_OnDeath;
                }
			}

		}

        private void HealthHaver_OnDeath(Vector2 obj)
        {
			if (aIActor.gameObject.GetComponent<AiactorSpecialStates>() == null)
			{
				aIActor.gameObject.GetOrAddComponent<AiactorSpecialStates>();
			}
			AiactorSpecialStates state = aIActor.gameObject.GetOrAddComponent<AiactorSpecialStates>();

			float cappedCoolness = state.EnemyCoolnessLevel;
			if (state.EnemyCoolnessLevel > 20) cappedCoolness = 20;
			if (aIActor.healthHaver.IsBoss && state.EnemyCoolnessLevel >= 15) cappedCoolness = 15;
			
			if (UnityEngine.Random.Range(0, 120) < cappedCoolness) // Max 1/6 chance on kill
			{
				int itemget = GetItem();
				if (itemget == 0000000)
                {
					GameManager.Instance.RewardManager.SpawnRoomClearChestAt((aIActor.GetAbsoluteParentRoom()).GetBestRewardLocation(new IntVector2(2, 1)));

				}
				else
                {
					LootEngine.SpawnItem(PickupObjectDatabase.GetById(itemget).gameObject, aIActor.sprite.WorldCenter, new Vector3(0, 0, 1), 7f, false, false, false);
				}
				
			}

			//ETGModConsole.Log("Coolness:" + state.EnemyCoolnessLevel);
		}

        private void OnAiTriedToShoot(Projectile obj)
        {
			if (obj.gameObject.GetComponent<EnemyProjOnHitPlayer>() == null)
			{
				EnemyProjOnHitPlayer proj = obj.gameObject.GetOrAddComponent<EnemyProjOnHitPlayer>();
				proj.IsForCoolStatus = true;
			}
		}
		public int KnownCool = 0;
		public void Update()
		{
			if (statused)
			{
				if (aIActor != null)
				{
					if (aIActor.healthHaver)
					{
						if (aIActor.healthHaver.IsAlive)
						{
							//visual effects
							aIActor.ApplyEffect(DummyEffect);
							if (aIActor.gameObject.GetComponent<AiactorSpecialStates>() == null)
							{
								aIActor.gameObject.GetOrAddComponent<AiactorSpecialStates>();
							}
							AiactorSpecialStates state = aIActor.gameObject.GetOrAddComponent<AiactorSpecialStates>();
							if (KnownCool != state.EnemyCoolnessLevel)
							{
								KnownCool = state.EnemyCoolnessLevel;
								GlobalSparksDoer.DoRadialParticleBurst((int)(state.EnemyCoolnessLevel / 2), aIActor.specRigidbody.UnitBottomLeft, aIActor.specRigidbody.UnitTopRight, 360, 10, 0, null, null, ExtendedColours.gildedBulletsGold, GlobalSparksDoer.SparksType.SOLID_SPARKLES);
							}
						}
					}

				}
				else
				{
					if (aIActor != null)
					{
						aIActor.RemoveEffect(DummyEffect);

						aIActor.ClearOverrideOutlineColor();
					}

				}
			}
		}
		
		private int GetItem()
		{
			int rng = UnityEngine.Random.Range(0, CoolEnemiesStatusController.Itemlist.Count - 1);

			return CoolEnemiesStatusController.Itemlist[rng];
		}


		public static List<int> Itemlist = new List<int>
		{
			tinyAmmo.ID,
			tinyAmmo.ID,
			tinyAmmo.ID,
			600, // spread
			600,
			67,  // key
			67,
			224, // blank
			224,
			73,  // heart
			73,
			73,
			73,  
			565, // glass
			565,
			565,
			120, // armor
			120,
			120,  
			0000000, // chest drop 1/20
		};

		public bool statused = false;
		private AIActor aIActor;
	}
}
