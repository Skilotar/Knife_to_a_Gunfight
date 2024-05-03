using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;

namespace Knives
{
	public class FollowProjMod : MonoBehaviour
	{

		public FollowProjMod()
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
			if(TargetProjectile != null)
            {
				Vector2 vector = TargetProjectile.sprite.WorldCenter - Projectile.sprite.WorldCenter;
				base.transform.eulerAngles = new Vector3(0f, 0f, vector.ToAngle());
			}
		}

		public void Upgradeprojectile(Projectile projectile)
		{
			if (projectile != null)
			{
                parentOwner.PostProcessProjectile += ParentOwner_PostProcessProjectile;
				projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemy));
				this.hasbeenupdated = true;
			}
		}
		public bool hassetTarget = false;
        private void ParentOwner_PostProcessProjectile(Projectile arg1, float arg2)
        {
			if(hassetTarget == false)
            {
				TargetProjectile = arg1;
				hassetTarget = true;
			}
			
        }

        public void OnHitEnemy(Projectile proj, SpeculativeRigidbody body, bool yes)
		{
			if (TargetProjectile != null)
            {
				if(body.aiActor != null)
                {
					if (body.aiActor.healthHaver != null && body.aiActor.knockbackDoer != null)
					{
						if (!body.aiActor.healthHaver.IsBoss)
						{
							body.aiActor.Velocity.Set(0, 0);
							Vector2 vector = TargetProjectile.sprite.WorldCenter - proj.sprite.WorldCenter;
							body.aiActor.knockbackDoer.ApplyKnockback(vector, (float)Vector2.Distance(proj.sprite.WorldCenter, TargetProjectile.sprite.WorldCenter) * 8, true);
						}

					}
				}
				
				
            }
            else
            {
				proj.baseData.force = 10;
            }
			
		}


		public bool hasbeenBuffed = false;

		public bool hasbeenupdated = false;

		private Projectile Projectile;

		private PlayerController parentOwner;

		private Projectile TargetProjectile;
	}
}
