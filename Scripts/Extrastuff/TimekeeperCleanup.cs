using System;
using ItemAPI;
using UnityEngine;

namespace Knives
{   //this code is by Nevernamed
	// Token: 0x02000192 RID: 402
	public class TimeKeeperCleanup : MonoBehaviour
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
		public TimeKeeperCleanup()
		{

		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0006765B File Offset: 0x0006585B
		private void Start()
		{
			Dude = base.GetComponent<GameObject>();

		}

		private void Update()
		{
			if (player.CurrentGun != null && this.gun != null)
			{

				if (CanClear())
				{
					if (delay > 0)
					{
						delay -= Time.deltaTime;
					}
					else
					{
						AkSoundEngine.PostEvent("Stop_Timekeep_HighNoon", base.gameObject);
						PlayerController player = this.gun.CurrentOwner as PlayerController;
						Pixelator.Instance.SetFreezeFramePower(0, false);
						this.gun.RemoveStatFromGun(PlayerStats.StatType.MovementSpeed);
						player.stats.RecalculateStats(player);
						
					}

				}
				else
				{
					
					delay = .2f;
				}


			}


		}

		private bool CanClear()
		{
			return player.CurrentGun.PickupObjectId != gun.PickupObjectId;
		}

		public PlayerController player;
		public Gun gun;
		private GameObject Dude;
		private float delay = .5f;
	}
}