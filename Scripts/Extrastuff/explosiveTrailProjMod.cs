using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;

namespace Knives
{
	public class ExplosiveTrailProjMod : MonoBehaviour
	{

		public ExplosiveTrailProjMod()
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
			yield return new WaitForSeconds(.1f);
			Projectile boom = ((Gun)ETGMod.Databases.Items[83]).DefaultModule.projectiles[0];
			GameObject gameObject = SpawnManager.SpawnProjectile(boom.gameObject, projectile.sprite.WorldCenter, Quaternion.Euler(0f, 0f, 0f), true);
			Projectile component = gameObject.GetComponent<Projectile>();
			bool flag2 = component != null;
			if (flag2)
			{
				component.Owner = parentOwner;
				component.Shooter = parentOwner.specRigidbody;
				component.baseData.damage = 2f;
				component.baseData.speed = 0;
				component.baseData.range = 1f;
				component.gameObject.GetOrAddComponent<DelayedExplosion>();
				
			}

			if(projectile != null)
            {
				StartCoroutine(DoTrail(projectile));
			}
           
		}

        
		
		public bool hassetTarget = false;
		
		

		public bool hasbeenBuffed = false;

		public bool hasbeenupdated = false;

		private Projectile Projectile;

		private PlayerController parentOwner;

		private Projectile TargetProjectile;
	}
}
