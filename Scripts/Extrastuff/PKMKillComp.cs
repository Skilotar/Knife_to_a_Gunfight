using System;
using ItemAPI;
using UnityEngine;
using System.Collections.Generic;

namespace Knives
{   //this code is by Nevernamed
	// Token: 0x02000192 RID: 402
	public class PlayerDestroyPKMRoomClear : MonoBehaviour
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
		public PlayerDestroyPKMRoomClear()
		{

		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0006765B File Offset: 0x0006585B
		private void Start()
		{
			player = base.GetComponent<PlayerController>();
		}

		private void Update()
		{
			if(player != null)
            {
                if (player.IsInCombat)
                {
					fallingEdgeCombat = true;

				}
                else
                {
					if(fallingEdgeCombat == true)
                    {
						fallingEdgeCombat = false;
						for(int i = PKM.Count -1; i>=0; i--)
                        {
							AIActor boy = PKM[i];
							if(boy != null)
                            {
								SpawnManager.SpawnVFX(EasyVFXDatabase.BloodiedScarfPoofVFX, boy.Position, Quaternion.identity);
								boy.EraseFromExistenceWithRewards(true);
							}
                        }
                    }
					PKM.Clear();
                }
            }
		}

		public List<AIActor> PKM = new List<AIActor>();
		bool fallingEdgeCombat = false;
		private PlayerController player;
		
	}
}