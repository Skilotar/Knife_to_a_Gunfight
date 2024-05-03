using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemAPI;
using UnityEngine;
using Dungeonator;

namespace Knives
{
	class Explodia_part_holder_Controller : MonoBehaviour
	{
		public static GameObject HolderCrown;
		public static GameActorDecorationEffect HolderDummyEffect;
		public static void Initialise()
		{
			HolderCrown = SpriteBuilder.SpriteFromResource("Knives/Resources/Holder.png", new GameObject("BlightIcon"));
			HolderCrown.SetActive(false);
			tk2dBaseSprite vfxSprite = HolderCrown.GetComponent<tk2dBaseSprite>();
			vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, vfxSprite.GetCurrentSpriteDef().position3);
			FakePrefab.MarkAsFakePrefab(HolderCrown);
			UnityEngine.Object.DontDestroyOnLoad(HolderCrown);

			HolderDummyEffect = new GameActorDecorationEffect()
			{
				AffectsEnemies = true,
				OverheadVFX = HolderCrown,
				AffectsPlayers = false,
				AppliesTint = true,
				AppliesDeathTint = false,
				AppliesOutlineTint = true,
				duration = float.MaxValue,
				effectIdentifier = "HolderCrown",
				resistanceType = EffectResistanceType.None,
				PlaysVFXOnActor = false,
				stackMode = GameActorEffect.EffectStackingMode.Ignore,
				OutlineTintColor = new Color(.75f, .52f, .13f),
				TintColor = new Color(.80f, .52f, .13f),
			};
		}
		public void Start()
		{
			this.player = base.GetComponent<PlayerController>();
			this.
			player.OnEnteredCombat = (Action)Delegate.Combine(player.OnEnteredCombat, new Action(this.OnEnteredCombat));
			player.OnNewFloorLoaded += this.OnNewFloorLoad;
			
			if (StartedSet == false)
            {
				
				FloorEventStartedMultiplier = MiscToolMethods.getPlayerDepth();
				FloorEventStartedMultiplier = (float)Math.Floor(FloorEventStartedMultiplier);
				StartedSet = true;
				reset();
			}
			

		}

        private void OnNewFloorLoad(PlayerController obj)
        {
			FloorEventStartedMultiplier--;
			if (FloorEventStartedMultiplier < 1) FloorEventStartedMultiplier = 1;
			reset();
        }

		public void reset()
        {
			EventSpawnNumber = 0;
			EventSpawnNumber += (int)FloorEventStartedMultiplier;
			weightedmax = 35;
			weightedmax -= (int)FloorEventStartedMultiplier;
		}

		public int EventSpawnNumber = 1;
		public bool StartedSet = false;
        public float FloorEventStartedMultiplier = 1;
		public AIActor holder = null;
		public int weightedmax = 25;
		public bool obtainedExplodia = false;
		private void OnEnteredCombat()
		{
			if(obtainedExplodia == false)
            {
				if (EventSpawnNumber > 0 && holder == null)
				{
					if (UnityEngine.Random.Range(0, weightedmax) < 1)
					{
						if (EventSpawnNumber != 0)
						{
							List<AIActor> actors = new List<AIActor> { };
							RoomHandler absoluteRoom = base.transform.position.GetAbsoluteRoom();
							if (absoluteRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
							{
								actors = MiscToolMethods.RandomNoRepeats(absoluteRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear), 1);
							}

							if (actors[0] != null)
							{
								holder = actors[0];
								actors.Clear();
								if (holder.healthHaver.IsBoss != true)
								{
									holder.ApplyEffect(HolderDummyEffect);
									holder.ForceBlackPhantomParticles = true;
									holder.LocalTimeScale = 1.5f;
									holder.healthHaver.SetHealthMaximum(holder.healthHaver.GetMaxHealth() * 2);
									holder.healthHaver.OnPreDeath += HealthHaver_OnPreDeath;

									AkSoundEngine.PostEvent("Play_ENM_jammer_curse_01", base.gameObject);

									AIActor Grenade = EnemyDatabase.GetOrLoadByGuid("4d37ce3d666b4ddda8039929225b7ede");
									ExplodeOnDeath DoYouWantToExplode = holder.gameObject.AddComponent<ExplodeOnDeath>();
									ExplosionData explosionData = Grenade.GetComponent<ExplodeOnDeath>().explosionData;

									explosionData.damageToPlayer = 1;
									DoYouWantToExplode.explosionData = explosionData;

									EventSpawnNumber--;
									weightedmax = 25;
									weightedmax -= (int)FloorEventStartedMultiplier;

								}

							}
						}

					}
					else
					{
						weightedmax -= 5;
						if (weightedmax < 4)
						{
							weightedmax = 2;
						}
					}
					//ETGModConsole.Log(weightedmax.ToString());
				}
			}
			
            
			
		}

		public void SetDeadlyDeathBurst(AIActor ExplodyBoi)
		{


			for (int i = 0; i < 12; i++)
			{

				GameObject gameObject = SpawnManager.SpawnProjectile(ChainFire_Reagent.specialproj.gameObject, ExplodyBoi.specRigidbody.UnitCenter, Quaternion.Euler(0f, 0f, (i * 30)), true);
				Projectile component = gameObject.GetComponent<Projectile>();
				bool flag = component != null;
				if (flag)
				{
					component.baseData.damage = 28 * (1 + GameManager.Instance.GetLastLoadedLevelDefinition().enemyHealthMultiplier);
					component.Owner = ExplodyBoi;
					component.collidesWithEnemies = true;
					component.collidesWithPlayer = true;

					ChainFireModifier chain = component.gameObject.GetOrAddComponent<ChainFireModifier>();



				}


			}

		}
		private void HealthHaver_OnPreDeath(Vector2 obj)
        {
			
			int ID = GetNextItem();
			if(ID != 0)
            {
				LootEngine.SpawnItem(PickupObjectDatabase.GetById(ID).gameObject, holder.CenterPosition, new Vector3(0, 0, 1), 7f, false, false, false);
			}
			SetDeadlyDeathBurst(holder.aiActor);
			holder = null;
			

		}
		private bool hadC = false;
		private bool hadE = false;
		private bool hadB = false;
		private bool hadS = false;
		private bool hadD = false;

		public int GetNextItem()
        {
			// items will be obtained From Drops in an specific order unless those exist already
			if (!player.HasPassiveItem(Core_otfg.ID) && hadC ==false)
			{
				return Core_otfg.ID;
			}
			else hadC = true;
			if (!player.HasPassiveItem(Eye_otfg.ID) && hadE == false)
			{
				return Eye_otfg.ID;

			}
			else hadE = true;
			if (!player.HasPassiveItem(Barrel_otfg.ID) && hadB == false)
			{
				return Barrel_otfg.ID;

			}
			else hadB = true;
			if (!player.HasPassiveItem(Stock_otfg.ID) && hadS == false)
			{
				return Stock_otfg.ID;

			}
			else hadS = true;
			if (!player.HasPassiveItem(Drum_otfg.ID) && hadD == false)
			{
				return Drum_otfg.ID;

			}
			else hadD = true;
			return 0;
		}

		public void Update()
		{

		}


		public bool hasHat = false;
		public PlayerController player;
	}
}
