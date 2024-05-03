using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;

namespace Knives
{
	public class LifeRampModifier : MonoBehaviour
	{

		public LifeRampModifier()
		{


		}


		private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
			this.parentOwner = this.Projectile.ProjectilePlayerOwner();
			
		}


		private void Update()
		{
			if (Projectile != null && !this.hasbeenupdated)
			{
				Upgradeprojectile(this.Projectile);
			}
		}

		public void Upgradeprojectile(Projectile projectile)
		{
			if (projectile != null)
			{

				projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemy));
				this.hasbeenupdated = true;
			}
		}

		public void OnHitEnemy(Projectile proj, SpeculativeRigidbody body, bool yes)
		{
			
			if(body.aiActor != null)
            {
				if (body.healthHaver != null)
                {
					float percentile = body.healthHaver.GetCurrentHealthPercentage();
					if(percentile > .75)
                    {
						proj.baseData.damage = 100;
                    }
					if (percentile < .75 && percentile >.50)
					{
						proj.baseData.damage = 50;

					}
					if (percentile < .50 && percentile > .25)
					{
						proj.baseData.damage = 25;

					}
					if (percentile < .25)
					{
						proj.baseData.damage = 6;

					}
					
                }
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

