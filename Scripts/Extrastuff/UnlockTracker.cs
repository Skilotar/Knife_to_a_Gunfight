using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;

namespace Knives
{
	public class UnlockablesTracker : MonoBehaviour
	{

		public UnlockablesTracker()
		{
			

		}


		private void Start()
		{

			player = base.GetComponent<PlayerController>();


		}

		bool setup = false;
		private void Update()
		{
			
			if (GameManager.Instance.IsLoadingLevel == false)
            {
				if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.AMMO_STARVED) == false)
				{
					foreach (Gun gun1 in player.inventory.AllGuns)
					{
						if (gun1.CurrentAmmo == 0)
						{
							foreach (Gun gun2 in player.inventory.AllGuns)
							{
								if (gun2.CurrentAmmo == 0 && gun2 != gun1)
								{
									AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.AMMO_STARVED, true);
								}
							}
						}
					}
				}
				if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.FIRST_IMPRESS) == false && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.TUTORIAL && setup == false)
				{
					ETGMod.AIActor.OnPostStart = (Action<AIActor>)Delegate.Combine(ETGMod.AIActor.OnPostStart, new Action<AIActor>(BeatTutorial));
					setup = true;
				}
				if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.FLATLINED) == false)
				{

					if (player.CurrentRoom != null && player.CurrentRoom.HasActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All))
					{
						foreach (AIActor aiactor in player.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All))
						{
							if (aiactor != null && aiactor.healthHaver != null && aiactor.healthHaver.IsBoss)
							{
								if (player.healthHaver.IsDead)
								{
									AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.FLATLINED, true);
								}
							}

						}
					}


				}

				

				
			}
		}

		private void BeatTutorial(AIActor target)
        {
			if(GameStatsManager.Instance.GetSessionStatValue(TrackedStats.TIME_PLAYED) <= 150)
            {
				if (target.EnemyGuid == "fc809bd43a4d41738a62d7565456622c")
				{
					target.healthHaver.OnDeath += (obj) =>
					{

						AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.FIRST_IMPRESS, true);


					};
				}
			}
			
		}

        public PlayerController player;
	}
}
