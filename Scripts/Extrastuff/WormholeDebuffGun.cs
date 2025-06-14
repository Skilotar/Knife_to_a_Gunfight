using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using Alexandria.ItemAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Alexandria.Misc;
using Alexandria.Assetbundle;

namespace Knives
{   //this code is by Nevernamed
	// Token: 0x02000192 RID: 402
	public class WormholeDebuffGun : MonoBehaviour
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
		public WormholeDebuffGun()
		{

		}

		
		private void Start()
		{
			gun = base.gameObject.GetComponent<Gun>();
			
			player.GetExtComp().OnPickedUpAmmo += OnAmmoCollected;
			

		}

        private void OnAmmoCollected(PlayerController arg1, AmmoPickup arg2)
        {
			
			gun.AddCurrentGunStatModifier(PlayerStats.StatType.Damage, .10f * (multiplier - 1), StatModifier.ModifyMethod.ADDITIVE);
			gun.AddCurrentGunStatModifier(PlayerStats.StatType.RateOfFire, -.10f * (multiplier - 1), StatModifier.ModifyMethod.ADDITIVE);
			player.stats.RecalculateStats(player);
			UnityEngine.GameObject.Destroy(gun.GetComponent<WormholeDebuffGun>());
		}

        private void Update()
		{
			if(knownMult != multiplier)
            {
				if(multiplier <= 10)
                {
					gun.AddCurrentGunStatModifier(PlayerStats.StatType.Damage, -.10f, StatModifier.ModifyMethod.ADDITIVE);
					gun.AddCurrentGunStatModifier(PlayerStats.StatType.ReloadSpeed, .10f, StatModifier.ModifyMethod.ADDITIVE);
					player.stats.RecalculateStats(player);
					knownMult = multiplier;
				}
                else
                {
					multiplier = 10;

				}
				
			}

		}

		private int knownMult = 0;
		public PlayerController player;
		public Gun gun;
		public int multiplier = 1;
	}
}