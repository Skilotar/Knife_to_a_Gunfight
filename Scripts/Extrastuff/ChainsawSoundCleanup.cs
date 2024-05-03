using System;
using ItemAPI;
using UnityEngine;

namespace Knives
{   //this code is by Nevernamed
	// Token: 0x02000192 RID: 402
	public class ChainsawCleanup : MonoBehaviour
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
		public ChainsawCleanup()
		{

		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0006765B File Offset: 0x0006585B
		private void Start()
		{
			Dude = base.GetComponent<GameObject>();

		}

		private void Update()
		{
			if(player.CurrentGun != null && this.saw != null)
            {
				
				if (CanClear())
				{
					if (delay > 0)
					{
						delay -= Time.deltaTime;
					}
					else
					{
						AkSoundEngine.SetRTPCValue("SAW_Sound", 50);
						player.primaryHand.ForceRenderersOff = false;
						Chainsaw.Holding = false;
					}

				}
				else
				{
					if (Chainsaw.Holding == false) AkSoundEngine.PostEvent("Play_Chainsaw_Starter", base.gameObject);
					Chainsaw.Holding = true;
					player.primaryHand.ForceRenderersOff = true;
					delay = .2f;
				}


			}


		}

		private bool CanClear()
		{
			return player.CurrentGun.PickupObjectId != saw.PickupObjectId;
		}

		public PlayerController player;
		public Gun saw;
		private GameObject Dude;
		private float delay = .5f;
	}
}