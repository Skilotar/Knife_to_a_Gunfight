using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;

namespace Knives
{
	public class GunApplyFlyMod : MonoBehaviour
	{

		public GunApplyFlyMod()
		{
			
		}


		private void Start()
		{
			this.gun = base.gameObject.GetComponent<Gun>();
			this.Boi = gun.CurrentOwner as PlayerController;
            Boi.GunChanged += Boi_GunChanged;
			Boi.SetIsFlying(true, "FlyModFly");
		}

        private void Boi_GunChanged(Gun oldgun, Gun newgun, bool arg3)
        {
            
			if (newgun.gameObject.GetComponent<GunApplyFlyMod>() != null) // new also has
            {
				Boi.AdditionalCanDodgeRollWhileFlying.SetOverride("Feather", true);
				Boi.SetIsFlying(true, "FlyModFly");
			}
            else // new doesnt has
            {
				Boi.AdditionalCanDodgeRollWhileFlying.SetOverride("Feather", false);
				Boi.SetIsFlying(false, "FlyModFly");
			}

        }

        private void Update()
		{
			
		}
		public PlayerSpecialStates state;
		private Gun gun;
		private PlayerController Boi;

	}
}
