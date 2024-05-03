using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;

namespace Knives
{
	public class DelayedExplosion : MonoBehaviour
	{

		public DelayedExplosion()
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
				projectile.StartCoroutine(Delay(projectile));
				this.hasbeenupdated = true;
			}
		}

        private IEnumerator Delay(Projectile projectile)
        {
			yield return new WaitForSeconds(.5f);
			DoSafeExplosion(projectile.sprite.WorldCenter);
			projectile.DieInAir();
        }

        public void DoSafeExplosion(Vector3 position)
		{

			ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
			this.smallPlayerSafeExplosion.effect = defaultSmallExplosionData2.effect;
			this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
			this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
			Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);


		}

		private ExplosionData smallPlayerSafeExplosion = new ExplosionData
		{
			damageRadius = 3f,
			damageToPlayer = 0f,
			doDamage = true,
			damage = 20f,
			doDestroyProjectiles = true,
			doForce = true,
			debrisForce = 0f,
			preventPlayerForce = true,
			explosionDelay = 0f,
			usesComprehensiveDelay = false,
			doScreenShake = false,
			playDefaultSFX = true,
			breakSecretWalls = true,
			secretWallsRadius = 3,
			force = 2,
			forceUseThisRadius = true,


		};

		public bool hassetTarget = false;



		public bool hasbeenBuffed = false;

		public bool hasbeenupdated = false;

		private Projectile Projectile;

		private PlayerController parentOwner;

		private Projectile TargetProjectile;
	}
}

