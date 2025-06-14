using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;

namespace Knives
{
	public class Agent47BulletTracker : MonoBehaviour
	{

		public Agent47BulletTracker()
		{

		}

		private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
			Projectile.OnHitEnemy = OnHitEnemy;
		}

		private void OnHitEnemy(Projectile proj, SpeculativeRigidbody arg2, bool fatal)
		{
			if(CurrentTarget != null)
            {
                if (arg2.aiActor != null)
                {
					if (CurrentTarget == arg2.aiActor)
					{
						if (CurrentTarget.healthHaver != null)
						{
							if (fatal) // this hit will kill... Probably.
							{

								CurrentTarget.CanDropCurrency = true;
								CurrentTarget.AssignedCurrencyToDrop += 1;
								CurrentTarget.AssignedCurrencyToDrop *= 5;

								//ETGModConsole.Log(holder.healthHaver);
								List<AIActor> actors = new List<AIActor> { };
								RoomHandler absoluteRoom = player.transform.position.GetAbsoluteRoom();
								actors = absoluteRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
								absoluteRoom.ClearReinforcementLayers();
								GameObject pool = PickupObjectDatabase.GetById(436).GetComponent<BlinkPassiveItem>().BlinkpoofVfx.gameObject;

								if (actors.Count > 0)
								{
									for (int i = actors.Count - 1; i >= 0; i--)
									{
										if (actors[i] != CurrentTarget && actors[i].EnemyGuid != "a446c626b56d4166915a4e29869737fd" && actors[i].EnemyGuid != "699cd24270af4cd183d671090d8323a1") // exclude target, key, and chance
										{
											SpawnManager.SpawnVFX(pool, actors[i].Position, Quaternion.identity);
											actors[i].EraseFromExistence();
										}

									}
								}
							}
						}
					}
				}
            }
		}

		
		private void Update()
		{

		}


		public static AIActor CurrentTarget = null;
		private Projectile Projectile;
		public static PlayerController player;

	}

}
