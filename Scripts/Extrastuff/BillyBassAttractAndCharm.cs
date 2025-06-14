using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;

namespace Knives
{
	public class CharmProxyCharmBomb : MonoBehaviour
	{

		public CharmProxyCharmBomb()
		{


		}


		private void Start()
		{
			proxy = this.gameObject.GetComponent<ProximityMine>();
			
		}

		private void Update()
		{
            if (room != null)
            {
				List<AIActor> actors = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
				for(int i = actors.Count - 1; i >= 0; i--)
                {
					if (Vector2.Distance(actors[i].Position, proxy.sprite.WorldCenter) <= 2)
                    {
						actors[i].aiActor.ApplyEffect(StaticStatusEffects.charmingRoundsEffect);
					}
					
				}
            }
		}
		
		public ProximityMine proxy;
		public RoomHandler room;
		public SpeculativeRigidbody body;
	}
}