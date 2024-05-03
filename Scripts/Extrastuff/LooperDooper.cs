using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;

namespace Knives
{
	public class LoopyDoopy: MonoBehaviour
	{

		public LoopyDoopy()
		{
			MainProjectile = null;

		}


		private void Start()
		{
			this.MainProjectile = base.GetComponent<Projectile>();
			this.parentOwner = this.MainProjectile.ProjectilePlayerOwner();

		}


		private void Update()
		{
			if (MainProjectile != null && !this.hasbeenupdated)
			{
				Upgradeprojectile(this.MainProjectile);
			}

		}

		public void Upgradeprojectile(Projectile projectile)
		{
			if (projectile != null)
			{
				projectile.gameObject.GetOrAddComponent<projectileStates>().LooperLooped = false;
				projectile.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(projectile.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreCollison));

			}
		}

		private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
		{
			if (myRigidbody.projectile != null)
			{
				if (myRigidbody.projectile.gameObject.GetOrAddComponent<projectileStates>().LooperLooped == false)
				{


					PhysicsEngine.SkipCollision = true;
					myRigidbody.CollideWithTileMap = false;
					myRigidbody.projectile.UpdateCollisionMask();
					


					StartCoroutine(Looptyloo(myRigidbody.projectile));


				}
			}



		}
		public Vector3 posi = new Vector3(0, 0,0);

		private IEnumerator Looptyloo(Projectile projectile)
		{
			Projectile projectile2 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.chargeProjectiles[0].Projectile;
			GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, projectile.specRigidbody.UnitCenter, Quaternion.Euler(0f, 0f, (projectile.Direction * -1).ToAngle()), true);
			Projectile component = gameObject.GetComponent<Projectile>();
			component.Owner = parentOwner;
			component.baseData.damage *= 0f;
			component.baseData.speed = 100f;
			component.SuppressHitEffects = true;
			component.hitEffects.suppressMidairDeathVfx = true;
			component.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(component.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreCollison2));

			while (posi != new Vector3(0, 0,0))
			{
				yield return new WaitForEndOfFrame();
			}

			projectile.transform.position = posi;

		}

		private void OnPreCollison2(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
		{
			posi =  myRigidbody.projectile.transform.position;
		}



		public bool hasbeenBuffed = false;

		public bool hasbeenupdated = false;

		private Projectile MainProjectile;

		private PlayerController parentOwner;

		private Projectile TargetProjectile;
	}
}

