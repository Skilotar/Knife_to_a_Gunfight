using System;
using ItemAPI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Knives
{   
	public class Procrast_Respawn : MonoBehaviour
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
		public Procrast_Respawn()
		{
			
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0006765B File Offset: 0x0006585B
		private void Start()
		{
			Owner = base.gameObject.GetComponent<PlayerController>();
			Owner.OnEnteredCombat += this.enteredCombat;
			 
		}

		
        private void enteredCombat()
        {
            if(Owner.CurrentRoom != null)
            {
				
				// is boss room? 
				bool bossroom = false;
				foreach(AIActor actor in Owner.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All))
                {
					if(actor != null)
                    {
						if (actor.healthHaver != null)
						{
							if (actor.healthHaver.IsBoss)
							{
								bossroom = true;
							}
						}
					}
                    
                }

				if(bossroom == true)
                {
					StartCoroutine(TriggerRespawn());
                }
            }
        }

        private IEnumerator TriggerRespawn()
        {
			//ETGModConsole.Log("STARTING SPAWN PROCESS! Enemy Count:" + GUIDs.Count);
			List<AIActor> bois = new List<AIActor>();
			for (int i = GUIDs.Count - 1; i >= 0; i--)
			{
				//ETGModConsole.Log(GUIDs[i]);
				bois.Add(EnemyDatabase.GetOrLoadByGuid(GUIDs[i]));
			}
			yield return new WaitForSeconds(1);
			
			for(int i = GUIDs.Count - 1; i >= 0; i--)
            {

				if (bois[i] != null)
				{
					IntVector2? intVector = new IntVector2?(Owner.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
					AIActor aiactor = AIActor.Spawn(bois[i], intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Default, true);
					aiactor.healthHaver.ApplyDamage(aiactor.healthHaver.GetMaxHealth() * .33f, Vector2.zero, "portalDamage");

					GameObject VFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Teleport_Beam");
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(VFX, aiactor.specRigidbody.UnitCenter, Quaternion.identity);
					gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(aiactor.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.MiddleCenter);
				}

				gun.GainAmmo(1);
				yield return new WaitForSeconds(1);
				yield return null;
			}

			GUIDs.Clear();

			
		}

        private void Update()
		{
			
		}

		private PlayerController Owner;
		public Gun gun;
		public List<string> GUIDs = new List<string> { };
	}
}