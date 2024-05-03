using System;
using ItemAPI;
using UnityEngine;

namespace Knives
{   //this code is by Nevernamed
	// Token: 0x02000192 RID: 402
	public class LanceCleanup : MonoBehaviour
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
		public LanceCleanup()
		{

		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0006765B File Offset: 0x0006585B
		private void Start()
		{
			
		}

		private void Update()
		{
			if (CanClear())
			{
				Lance.Kill = true;
			}
			
		}

		private bool CanClear()
		{
			return player.CurrentGun.PickupObjectId != Lance.lnc;
		}

		public PlayerController player;
		
	}
}