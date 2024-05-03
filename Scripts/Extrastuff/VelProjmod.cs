using System;
using ItemAPI;
using UnityEngine;

namespace Knives
{   //this code is by Nevernamed
	// Token: 0x02000192 RID: 402
	public class PlayerVelocityBuffProjectileModifier : MonoBehaviour
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
		public PlayerVelocityBuffProjectileModifier()
		{
			
			
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0006765B File Offset: 0x0006585B
		private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
			this.parentOwner = this.Projectile.ProjectilePlayerOwner();
			speed = parentOwner.gameObject.GetOrAddComponent<TrackHighSpeed>();
		}

		// Token: 0x0600086D RID: 2157 RVA: 0x0006767C File Offset: 0x0006587C
		private void Update()
		{
			if(Projectile != null && !this.hasbeenupdated)
            {
				Upgradeprojectile(this.Projectile);
            }
		}

		public void Upgradeprojectile(Projectile projectile)
        {
			if(projectile != null)
            {
				if (speed.HighSpeed > 20)
				{ 
					projectile.sprite.color = ExtendedColours.carrionRed;
					GoopModifier goopModifier = projectile.gameObject.GetOrAddComponent<GoopModifier>();
					goopModifier.SpawnGoopInFlight = true;
					goopModifier.InFlightSpawnRadius = .75f;
					GoopDefinition fire = PickupObjectDatabase.GetById(242).GetComponent<DirectionalAttackActiveItem>().goopDefinition;
					fire.lifespan = 2.5f;
					goopModifier.goopDefinition = fire;
					AkSoundEngine.PostEvent("Play_TRP_flame_torch_01", base.gameObject);
					AkSoundEngine.PostEvent("Play_ENV_oilfire_ignite_01", base.gameObject);

				}
				AkSoundEngine.PostEvent("Play_TRP_flame_torch_01", base.gameObject);
				

				projectile.Speed *= 1 + speed.HighSpeed;
				projectile.UpdateSpeed();
				projectile.baseData.damage *= 1 + (speed.HighSpeed / 4f);
				projectile.BossDamageMultiplier = 2;
				this.hasbeenupdated = true;

			}
        }


		public bool hasbeenupdated = false;
		// Token: 0x040004C7 RID: 1223
		private Projectile Projectile;

		// Token: 0x040004C8 RID: 1224
		private PlayerController parentOwner;
		public TrackHighSpeed speed;
		public float ProjSpeedMult;

		public float ProjDamageMult;

		public float ProjSizeMult;
	}
}
