using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;

namespace Knives
{
	public class TrailProjMod : MonoBehaviour
	{

		public TrailProjMod()
		{
			TargetProjectile = null;

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
				StartCoroutine(DoTrail(projectile));
				this.hasbeenupdated = true;
			}
		}

        private IEnumerator DoTrail(Projectile projectile)
        {
			yield return new WaitForSeconds(rate);

			float Angle = UnityEngine.Random.Range(-180, 180f);
			Projectile component = MiscToolMethods.SpawnProjAtPosi(boom, projectile.LastPosition, parentOwner, Angle);
			bool flag2 = component != null;
			if (flag2)
			{
				component.Owner = parentOwner;
				component.Shooter = parentOwner.specRigidbody;
				component.baseData.damage = 1f;
				component.baseData.speed = 0;
				component.baseData.range = 1f;
				if (Explosive == true) component.gameObject.GetOrAddComponent<DelayedExplosion>();
				if (Shark == true)
				{
					component.baseData.range = 4f;
					component.gameObject.GetOrAddComponent<DriftModifier>();
					component.baseData.speed = 5;
					SlowDownOverTimeModifier slowDown = component.gameObject.AddComponent<SlowDownOverTimeModifier>();
					slowDown.extendTimeByRangeStat = true;
					slowDown.activateDriftAfterstop = true;
					slowDown.doRandomTimeMultiplier = true;
					slowDown.killAfterCompleteStop = true;
					slowDown.timeTillKillAfterCompleteStop = 1f;
					slowDown.timeToSlowOver = 1f;
					slowDown.targetSpeed = component.baseData.speed * 0.3f;
				}
			}

			if(projectile != null)
            {
				StartCoroutine(DoTrail(projectile));
			}
           
		}

        
		
		public bool hassetTarget = false;

		public float rate = .1f;
		public Projectile boom = ((Gun)ETGMod.Databases.Items[83]).DefaultModule.projectiles[0];
		public bool Shark = false;
		public bool Explosive = false;


		public bool hasbeenBuffed = false;

		public bool hasbeenupdated = false;

		private Projectile Projectile;

		private PlayerController parentOwner;

		private Projectile TargetProjectile;
	}
}
