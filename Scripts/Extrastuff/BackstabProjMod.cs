using System;
using System.Collections;
using Dungeonator;
using UnityEngine;

using ItemAPI;
using System.Collections.Generic;

namespace Knives
{
	public class BackStabBullets : MonoBehaviour
	{

		public BackStabBullets()
		{

		}
		
		public void Start()
		{
			projectile = base.gameObject.GetComponent<Projectile>();
			projectile.OnHitEnemy += this.onHitEnemy;
			Owner = (PlayerController)projectile.Owner;
			BounceProjModifier bounce = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
			bounce.numberOfBounces = 1;
		}
		public Vector2 OrigDir;
        private void onHitEnemy(Projectile proj, SpeculativeRigidbody arg2, bool arg3)
        {
            if(arg2.aiActor != null)
            {
				if (arg3 == false)
				{
					
					AIActor aiactor = arg2.gameObject.GetComponent<AIActor>();
					proj.shouldRotate = true;
					OrigDir = proj.Direction;
					proj.SendInDirection((Vector2)aiactor.CenterPosition - (Vector2)proj.LastPosition, true, true);
					float DotProd = Vector2.Dot(proj.Direction, aiactor.FacingDirection.DegreeToVector2());
					if (DotProd >= .25f && arg2.aiActor.healthHaver.IsAlive)
					{
						float mult = 1;
						if (Owner.HasPickupID(BlackRose.ID) && Owner.HasPickupID(SpyBullets.ID)) mult = 2f;
						arg2.healthHaver.ApplyDamage(10 * mult, Vector2.zero, "Backstab");

						AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
						AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
						
					}
                    else
                    {
						proj.SendInDirection(OrigDir, false, true);
					}
				}
            }
        }

        public Projectile Kproj;
		public Projectile projectile;
		public PlayerController Owner;
	}
}