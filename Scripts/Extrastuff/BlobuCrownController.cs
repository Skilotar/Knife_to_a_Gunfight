using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemAPI;
using UnityEngine;
using Dungeonator;

namespace Knives
{
    class BlobuCrownController : MonoBehaviour
    {
		public static GameObject BlobuCrown;
		public static GameActorDecorationEffect BlobDummyEffect;
		public static void Initialise()
		{
			BlobuCrown = SpriteBuilder.SpriteFromResource("Knives/Resources/lord2.png", new GameObject("BlightIcon"));
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
						aiactor.gameObject.GetOrAddComponent<SkilotarSplitBehave>();
						skilotar_ = aiactor;
						Event_started = true;
						GameUIBossHealthController gameUIBossHealthController = (!skilotar_.healthHaver.UsesVerticalBossBar) ? (skilotar_.healthHaver.UsesSecondaryBossBar ? GameUIRoot.Instance.bossController2 : GameUIRoot.Instance.bossController) :
						GameUIRoot.Instance.bossControllerSide;
						gameUIBossHealthController.RegisterBossHealthHaver(skilotar_.healthHaver, "Skilotar_");

						player.OnRoomClearEvent += Player_OnRoomClearEvent;
					}
				}
			}
		}

		public AIActor skilotar_;

        public bool Event_started { get; private set; }
        System.Random rng = new System.Random();
		public bool rename = false;

		public void Update()
		{
			if(skilotar_ != null)
            {
				GameUIBossHealthController gameUIBossHealthController = (!skilotar_.healthHaver.UsesVerticalBossBar) ? (skilotar_.healthHaver.UsesSecondaryBossBar ? GameUIRoot.Instance.bossController2 : GameUIRoot.Instance.bossController) :
				GameUIRoot.Instance.bossControllerSide;
				if (skilotar_.isActiveAndEnabled && gameUIBossHealthController.name != "Skilotar_")
                {
					
					gameUIBossHealthController.RegisterBossHealthHaver(skilotar_.healthHaver, "Skilotar_");
					
				}
				
			}
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

			if(obj.CurrentRoom.PlayerHasTakenDamageInThisRoom == false)
            {
				Chest Giftbox2 = Chest.Spawn(chestprefab.GetComponent<Chest>(), obj.CurrentRoom.GetBestRewardLocation(new IntVector2(2, 1)));

				Giftbox2.IsLocked = false;
				Giftbox2.overrideMimicChance = 0;
				Giftbox2.PreventFuse = true;
				Giftbox2.RegisterChestOnMinimap(Giftbox.GetAbsoluteParentRoom());
			}
			
			player.OnRoomClearEvent -= Player_OnRoomClearEvent;

		}

        public bool hasHat = false;
		public PlayerController player;
	}
}
