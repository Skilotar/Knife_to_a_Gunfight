using System;
using ItemAPI;
using UnityEngine;

namespace Knives
{
	public class HolyNadeController : MonoBehaviour
	{

		public HolyNadeController()
		{


		}


		private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
			this.parentOwner = this.Projectile.ProjectilePlayerOwner();
		}

		public int distance = 0;
		private void Update()
		{
			if (Projectile != null )
			{
				Upgradeprojectile(this.Projectile);
			}
		}

		public void Upgradeprojectile(Projectile projectile)
		{
			if(distance >= 100)
            {
				projectile.AdditionalScaleMultiplier *= 1 + (distance * .01f);
            }
			if(distance <101 && distance > 200)
            {
				projectile.AdditionalScaleMultiplier *= 2 - ((distance - 100) * .01f);
			}
			if (distance < 201 && distance > 300)
			{
				projectile.AdditionalScaleMultiplier *= 1 + ((distance - 200) * .005f);
			}
			if (distance < 101 && distance > 200)
			{
				projectile.AdditionalScaleMultiplier *= 2 - ((distance - 300) * .005f);
			}
		}

		

		public bool hasbeenBuffed = false;

		public bool hasbeenupdated = false;

		private Projectile Projectile;


		private PlayerController parentOwner;

		public BasicBeamController beamComp;

		public Gun gun;
	}
}
