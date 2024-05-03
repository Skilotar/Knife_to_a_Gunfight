using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;

namespace Knives
{
	public class EmergencyGunUnblocker : MonoBehaviour
	{

		public EmergencyGunUnblocker()
		{
			
		}


		private void Start()
		{
			player = this.gameObject.GetComponent<PlayerController>();
		}



		private void Update()
		{
			if (Projectile != null)
			{

			}
            else
            {
				player.IsGunLocked = false;
               
			}


		}


		public bool isyoyo = false;
		public Projectile Projectile;
		public PlayerController player;

	}

}