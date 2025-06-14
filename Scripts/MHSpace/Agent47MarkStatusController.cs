using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using UnityEngine;
using Dungeonator;
using System.Collections;

namespace Knives
{
	class HitManMarkedStatus : MonoBehaviour
	{
		public static GameObject HolderCrown;
		public static GameActorDecorationEffect HolderDummyEffect;
		public static void Initialise()
		{
			HolderCrown = SpriteBuilder.SpriteFromResource("Knives/Resources/Alc_clipfull", new GameObject("MarkedIcon")); // Overhead Sprite for Marked enemy.
			HolderCrown.SetActive(false);
			tk2dBaseSprite vfxSprite = HolderCrown.GetComponent<tk2dBaseSprite>();
			vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, vfxSprite.GetCurrentSpriteDef().position3);
			FakePrefab.MarkAsFakePrefab(HolderCrown);
			UnityEngine.Object.DontDestroyOnLoad(HolderCrown);

			HolderDummyEffect = new GameActorDecorationEffect() // visual effects for statused enemy
			{
				AffectsEnemies = true,
				OverheadVFX = HolderCrown,
				AffectsPlayers = false,
				AppliesTint = true,
				AppliesDeathTint = false,
				AppliesOutlineTint = true,
				duration = float.MaxValue,
				effectIdentifier = "MarkedCrown",
				resistanceType = EffectResistanceType.None,
				PlaysVFXOnActor = false,
				stackMode = GameActorEffect.EffectStackingMode.Ignore,
				OutlineTintColor = new Color(.25f, .90f, .97f),
				TintColor = new Color(1f, 1f, 1f),
			};
		}
		public void Start()
		{
			HitManMarkedStatus.Initialise();
			
			this.player = base.GetComponent<PlayerController>();
			Agent47BulletTracker.player = player;
			player.OnEnteredCombat = (Action)Delegate.Combine(player.OnEnteredCombat, new Action(this.OnEnteredCombat));


		}
		public AIActor holder = null;
		public void OnEnteredCombat()
		{
            //if(player.HasGun(YourGunIDHere))   //player owns the gun at all?
            {

				//if(UnityEngine.Random.Range(0,10f) <1) //10% chance
                {
					List<AIActor> actors = new List<AIActor> { };
					RoomHandler absoluteRoom = player.CurrentRoom;
					//ETGModConsole.Log(absoluteRoom.GetRoomName()); // did we even get a room?
					if (absoluteRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
					{
						actors = HitManMarkedStatus.RandomNoRepeats(absoluteRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear), 1);
					}


					if (actors.Count > 0)
					{
						if (actors[0] != null)
						{
							holder = actors[0];
							if (holder != null)
							{
								actors.Clear();
								if (holder.healthHaver.IsBoss != true)
								{
									holder.ApplyEffect(HolderDummyEffect);
									Agent47BulletTracker.CurrentTarget = holder;
									AkSoundEngine.PostEvent("Play_ENM_jammer_curse_01", base.gameObject);
								}
							}


						}
						else
						{
							ETGModConsole.Log("Mark had no target in [0]");
						}

					}
					else
					{
						ETGModConsole.Log("Mark had no targets at all. Who's agent 47 gonna kill now?");
					}

				}


			}

		}


		
		public void Update()
		{

		}
		public static List<T> RandomNoRepeats<T>(List<T> candidates, int count)
		{

			for(int E = candidates.Count - 1; E >= 0; E--)
            {
				if(Excluded_bois.Contains((candidates[E] as AIActor).EnemyGuid))
				{
					candidates.Remove(candidates[E]);
				}
            }


			List<T> outcomes = new List<T> { };
			int i = 0;
			do
			{
				i++;
				int V = UnityEngine.Random.Range(0, candidates.Count);
				if (!outcomes.Contains(candidates[V]))
				{
					outcomes.Add(candidates[V]);
				}


			} while (i < count * 3 && outcomes.Count < count);

			return outcomes;
		}

		public static List<string> Excluded_bois = new List<string>
		{
			//low health
			"42be66373a3d4d89b91a35c9ff8adfec", // blobulin
			"249db525a9464e5282d02162c88e0357", // Spent
			"c2f902b7cbe745efb3db4399927eab34", // skusket_head

			//selfdistructors
			"2feb50a6a40f4f50982e89fd276f6f15", // bullat
			"2d4f8b5404614e7d8b235006acde427a", // shotgat
			"7ec3e8146f634c559a7d58b19191cd43", // spirat
			"b4666cb6ef4f4b038ba8924fd8adf38f", // grenat
			"4d37ce3d666b4ddda8039929225b7ede", // grenade_kin
			"c0260c286c8d4538a697c5bf24976ccf", // dynamite_kin
			"4538456236f64ea79f483784370bc62f", // fusebot
			"5f15093e6f684f4fb09d3e7e697216b4", // m80_kin
			"19b420dec96d4e9ea4aebc3398c0ba7a", // bombshee
			
			//Potential issues
			"b5e699a0abb94666bda567ab23bd91c4", // bullet_kings_toadie
			"d4dd2b2bbda64cc9bcec534b4e920518", // bullet_kings_toadie_revenge
			"02a14dec58ab45fb8aacde7aacd25b01", // old_kings_toadie
		};


		public class GameActorDecorationEffect : GameActorEffect
		{
		}

		public PlayerController player;
	}


}
