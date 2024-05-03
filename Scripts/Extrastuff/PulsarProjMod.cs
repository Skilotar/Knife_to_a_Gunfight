using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;

namespace Knives
{
	public class PulstarProjMod : MonoBehaviour
	{

		public PulstarProjMod()
		{
			Life = 0;
			PulseTimer = .75f;
			DetTimer = 2f;
		}


		private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
			this.parentOwner = this.Projectile.ProjectilePlayerOwner();
			
		}


		private void Update()
		{
			if(Projectile != null)
            {
				Life = Life + Time.deltaTime;

				if (Life >= PulseTimer && hasbeenupdated == false)
                {
					Upgradeprojectile(Projectile);
                }

				if(Life >= DetTimer && once == false)
                {
					DoSafeExplosion(Projectile.LastPosition);
					
					Projectile.DieInAir();
                }
				
            }

		}

		public void Upgradeprojectile(Projectile projectile)
		{
			if (projectile != null)
			{
				AkSoundEngine.PostEvent("Play_Pulstar_grow_001", base.gameObject);
				
				projectile.RuntimeUpdateScale(2f);
				
				projectile.OnHitEnemy += this.Onhitenemy;
				this.hasbeenupdated = true;
			}
		}
		public bool once = false;
        private void Onhitenemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
			if(arg1 != null && once == false)
            {
				if (arg2.aiActor != null)
				{
					DoSafeExplosion(arg1.LastPosition);
				}

			}
			
        }

		public void DoSafeExplosion(Vector3 position)
		{
			once = true;
			ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
			this.smallPlayerSafeExplosion.effect = defaultSmallExplosionData2.effect;
			this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
			this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
			Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);


		}

		private ExplosionData smallPlayerSafeExplosion = new ExplosionData
		{
			damageRadius = 2f,
			damageToPlayer = 0f,
			doDamage = true,
			damage = 15f,
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
			force = 10,
			forceUseThisRadius = true,


		};

		public float Life;
		public float PulseTimer;
		public float DetTimer;

		public bool hasbeenBuffed = false;

		public bool hasbeenupdated = false;

		private Projectile Projectile;

		private PlayerController parentOwner;

		private Projectile TargetProjectile;
	}
}
