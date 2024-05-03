using System;
using ItemAPI;
using UnityEngine;

namespace Knives
{   //this code is by Nevernamed
	// Token: 0x02000192 RID: 402
	public class PicoriCleanup : MonoBehaviour
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
		public PicoriCleanup()
		{

		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0006765B File Offset: 0x0006585B
		private void Start()
		{
			Dude = base.GetComponent<GameObject>();
			 
		}

		private void Update()
		{
			if (CanClear())
			{
				if (delay > 0)
				{
					delay -= Time.deltaTime;
				}
				else
				{

					Fourk.runClearDudes();
				}
			}
            else
            {
				delay = .5f;
            }

		}

        private bool CanClear()
        {
			return player.CurrentGun.PickupObjectId != PicoriBlade.PickupObjectId && player.CurrentGun.PickupObjectId != 8;
        }

        public PlayerController player;
		public Gun PicoriBlade;
		private GameObject Dude;
        private float delay = .5f;
    }
}