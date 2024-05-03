using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemAPI;
using UnityEngine;
using Dungeonator;

namespace Knives
{
    class BlobuCrownController :MonoBehaviour
    {
		public static GameObject BlobuCrown;
		public static GameActorDecorationEffect BlobDummyEffect;
		public static void Initialise()
		{
			BlobuCrown = SpriteBuilder.SpriteFromResource("Knives/Resources/lord.png", new GameObject("BlightIcon"));
			BlobuCrown.SetActive(false);
			tk2dBaseSprite vfxSprite = BlobuCrown.GetComponent<tk2dBaseSprite>();
			vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, vfxSprite.GetCurrentSpriteDef().position3);
			FakePrefab.MarkAsFakePrefab(BlobuCrown);
			UnityEngine.Object.DontDestroyOnLoad(BlobuCrown);

			BlobDummyEffect = new GameActorDecorationEffect()
			{
				AffectsEnemies = true,
				OverheadVFX = BlobuCrown,
				AffectsPlayers = false,
				AppliesTint = false,
				AppliesDeathTint = false,
				AppliesOutlineTint = false,
				duration = float.MaxValue,
				effectIdentifier = "Lord's Crown",
				resistanceType = EffectResistanceType.None,
				PlaysVFXOnActor = false,
				stackMode = GameActorEffect.EffectStackingMode.Ignore,
			};
		}
		public void Start()
		{
			this.player = base.GetComponent<PlayerController>();
			player.OnEnteredCombat = (Action)Delegate.Combine(player.OnEnteredCombat, new Action(this.OnEnteredCombat));
		}

		bool Event_started = false;
		bool setup = false;
        private void OnEnteredCombat()
        {
			RoomHandler currentRoom = player.CurrentRoom;
			foreach (AIActor aiactor in currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
			{
				if (aiactor.EnemyGuid == "1b5810fafbec445d89921a4efb4e42b7" && aiactor.healthHaver.GetCurrentHealthPercentage() == 1 && !aiactor.gameObject.GetComponent<SkilotarSplitBehave>())
				{
					if (rng.Next(1, 20) == 1 || this.player.GetComponent<Explodia_part_holder_Controller>() != null)
					{
						aiactor.ApplyEffect(BlobDummyEffect);

                        //aiactor.ActorName = "Skilotar_";
                        //aiactor.OverrideDisplayName = "Skilotar_";
						//aiactor.gameObject.name = "Skilotar_";
						aiactor.LocalTimeScale = 1.05f;
						aiactor.gameObject.GetOrAddComponent<SkilotarSplitBehave>();
						Event_started = true;
						
						player.OnRoomClearEvent += Player_OnRoomClearEvent;
					}
				}
			}
		}
		

        System.Random rng = new System.Random();

		public void Update()
		{
			
		}

        private void Player_OnRoomClearEvent(PlayerController obj)
        {
			Event_started = false;
			
			GameObject chestprefab = GameManager.Instance.RewardManager.Synergy_Chest.gameObject;
			Chest Giftbox = Chest.Spawn(chestprefab.GetComponent<Chest>(), obj.CurrentRoom.GetBestRewardLocation(new IntVector2(2, 1)));
			
			Giftbox.IsLocked = false;
			Giftbox.overrideMimicChance = 0;
			Giftbox.PreventFuse = true;
			Giftbox.RegisterChestOnMinimap(Giftbox.GetAbsoluteParentRoom());
			player.OnRoomClearEvent -= Player_OnRoomClearEvent;

		}

        public bool hasHat = false;
		public PlayerController player;
	}
}
