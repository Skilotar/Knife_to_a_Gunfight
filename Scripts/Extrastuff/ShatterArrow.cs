using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;

namespace Knives
{
	public class ShatterArrow : MonoBehaviour
	{

		public ShatterArrow()
		{
			
		}


		private void Start()
		{
			this.projectile = base.GetComponent<Projectile>();
			this.parentOwner = this.projectile.ProjectilePlayerOwner();
			this.gun = this.projectile.PossibleSourceGun;

			projectile.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(projectile.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreCollisontile));
			projectile.specRigidbody.OnPreRigidbodyCollision += (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnHitbody));
			AkSoundEngine.PostEvent("Play_WPN_woodbow_shot_01", gun.gameObject);

            if (parentOwner.PlayerHasActiveSynergy("Shatter Slugs"))
            {
				ChancetoSlug = true;
			}
			if (parentOwner.PlayerHasActiveSynergy("B.F.S."))
			{
				shatterto = ((Gun)PickupObjectDatabase.GetById(512)).DefaultModule.projectiles[0].projectile;
				max = 5;
			}

		}
		public bool ChancetoSlug = false;

        private void OnHitbody(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
			Shatter(myRigidbody.projectile, 7, false);
		}

        private void OnPreCollisontile(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
		{
			Shatter(myRigidbody.projectile, 10, true);
		}
		public float max = 7;
		public void Shatter(Projectile projectile, float shatterangs, bool wall)
		{
			AkSoundEngine.PostEvent("Play_Mozam_hammer_001", base.gameObject);
			SpawnManager.SpawnVFX(EasyVFXDatabase.RedFireBlastVFX, projectile.sprite.WorldCenter, Quaternion.identity);

			if (parentOwner.PlayerHasActiveSynergy("Bomb Arrows"))
			{
				this.DoSaferExplosion(projectile.LastPosition);
			}


			float ang = projectile.Direction.ToAngle();
			shatterangs *= this.parentOwner.stats.GetStatValue(PlayerStats.StatType.Accuracy);
			float startang = ang-(shatterangs * (float)Math.Truncate(max / 2));

			if (ChancetoSlug == true)
			{
				if (UnityEngine.Random.Range(0, 4) <= 1)
				{
					max = 0;
					startang = ang;
					shatterangs = 0;
					shatterto = ((Gun)PickupObjectDatabase.GetById(694)).DefaultModule.projectiles[0].projectile;
					shatterto.baseData.damage = 18f;
					shatterto.baseData.speed = 50f;
					shatterto.baseData.range = 50f;
					shatterto.AdditionalScaleMultiplier *= 1f;
					shatterto.gameObject.GetOrAddComponent<PierceProjModifier>().penetration++;
					BounceProjModifier bnc = shatterto.gameObject.GetOrAddComponent<BounceProjModifier>();
					bnc.numberOfBounces = 4;
					
				}
			}

			for (int i = 0; i <= max; i++)
			{
				float var = shatterangs * i;
				float adjang = startang + var;
				Projectile shot = MiscToolMethods.SpawnProjAtPosi(shatterto, projectile.sprite.WorldCenter, parentOwner, adjang);
                if (wall)
                {
					shot.baseData.damage *= 3;
					shot.gameObject.GetOrAddComponent<BounceProjModifier>().numberOfBounces += 2;
				}

				ImprovedAfterImage trail = shot.gameObject.GetOrAddComponent<ImprovedAfterImage>();
				trail.shadowLifetime = .33f;
				trail.shadowTimeDelay = .0001f;
				trail.dashColor = ExtendedColours.carrionRed;
				trail.spawnShadows = true;

			}
			
			DebrisObject debris = projectile.BecomeDebris(projectile.Direction * -2, .5f);
			debris.ClearVelocity();
			projectile.specRigidbody.enabled = false;
		}

		private void Update()
		{
			
		}

		public void DoSaferExplosion(Vector3 position)
		{

			ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
			this.smallPlayerSaferExplosion.effect = EasyVFXDatabase.BloodiedScarfPoofVFX;
			this.smallPlayerSaferExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
			this.smallPlayerSaferExplosion.ss = defaultSmallExplosionData2.ss;
			Exploder.Explode(position, this.smallPlayerSaferExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);

		}

		private ExplosionData smallPlayerSaferExplosion = new ExplosionData
		{
			damageRadius = 4f,
			damageToPlayer = 0f,
			doDamage = true,
			damage = 7f,
			doDestroyProjectiles = true,
			doForce = false,
			debrisForce = 0f,
			preventPlayerForce = false,
			explosionDelay = 0.0f,
			usesComprehensiveDelay = false,
			doScreenShake = false,
			playDefaultSFX = true,

		};

		public bool once = false;
		
		
		public bool hasbeenBuffed = false;

		public bool hasbeenupdated = false;

		private Projectile projectile;
		public Projectile shatterto;
		public Gun gun;

		private PlayerController parentOwner;

		private Projectile TargetProjectile;
	}
}