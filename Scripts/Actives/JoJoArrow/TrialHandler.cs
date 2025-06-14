using System;
using ItemAPI;
using UnityEngine;
using Alexandria.Misc;

namespace Knives
{   
	public class JoJoTrial : MonoBehaviour
	{
		
		public JoJoTrial()
		{

		}

		public ChallengeModeType type;
		private void Start()
		{
			player = base.gameObject.GetComponent<PlayerController>();
			Notify("-Trial of the Arrow-", "Survive " + NumberOfTrialRooms + " rooms.");
			player.OnEnteredCombat += BeginTrial;
			player.healthHaver.OnPreDeath += HealthHaver_OnPreDeath;

			type = ChallengeManager.ChallengeModeType;
			ChallengeManager.ChallengeModeType = ChallengeModeType.ChallengeMode;
		}

		public int healthTaken = 2;
	
		private void HealthHaver_OnPreDeath(Vector2 obj)
		{
			SaveLife(Vector2.zero);
		}

		private void SaveLife(Vector2 finalDamageDirection)
		{
			if (Completed == false)
			{
				Failed = true;
				Completed = true;
				InTrial = false;
				Notify("-Trial of the Arrow-", "..Failed..");

				if (player.ForceZeroHealthState == true)
				{ player.healthHaver.Armor = 1; }
				else { player.healthHaver.ApplyHealing(.5f); }

				GameManager.Instance.MainCameraController.SetManualControl(false, false);
				player.ToggleGunRenderers(true, "non-death");
				player.ToggleHandRenderers(true, "non-death");
				player.CurrentInputState = PlayerInputState.AllInput;

				player.healthHaver.TriggerInvulnerabilityPeriod();
				//player.BloopItemAboveHead(base.gameObject.GetComponent<tk2dBaseSprite>(), "Knives/Resources/lifeInsureance");
				player.OnEnteredCombat -= BeginTrial;
				player.healthHaver.OnPreDeath -= HealthHaver_OnPreDeath;
				ChallengeManager.ChallengeModeType = type; //set back to normal;
				player.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.Health, StatModifier.ModifyMethod.ADDITIVE, healthTaken));
				player.stats.RecalculateStats(player, true);

			}
		}

		bool Failed = false;
        bool Completed = false;
		bool InTrial = false;
		int NumberOfTrialRooms = 3;
        private void BeginTrial()
        {
			if(Completed == false)
            {
				InTrial = true;
				NumberOfTrialRooms--;
			}

		}

        private void Update()
		{
			if(player != null)
            {
				if(Failed == false)
                {
					if (InTrial && !player.IsInCombat && NumberOfTrialRooms == 0) // trial ended
					{
						ChallengeManager.ChallengeModeType = type; //set back to normal;
						Notify("-Trial of the Arrow-", "!Complete!");
						player.GiveItem("ski:fate's_blessing");
						player.OnEnteredCombat -= BeginTrial;
						player.healthHaver.OnPreDeath -= HealthHaver_OnPreDeath;
						InTrial = false;
						Completed = true;
					}
					
				}
                
			}
		}


		public void Notify(string header, string text)
		{
			GameObject gameObject = SpriteBuilder.SpriteFromResource(jojo_arrow.resource, null);
			var sprite = gameObject.GetComponent<tk2dSprite>();
			GameUIRoot.Instance.notificationController.DoCustomNotification(
				header,
				text,
				sprite.Collection,
				sprite.spriteId,
				UINotificationController.NotificationColor.PURPLE,
				false,
				false);
		}

		PlayerController player;

		public GameObject ItemtoDestroy;
    }
}