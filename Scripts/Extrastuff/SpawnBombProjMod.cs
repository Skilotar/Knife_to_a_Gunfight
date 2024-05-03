using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;

namespace Knives
{
	public class SpawnBombProjMod : MonoBehaviour
	{

		public SpawnBombProjMod()
		{
			tossforce = 0f;

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
				if(gameobjecttospawn != null)
                {
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(gameobjecttospawn, this.parentOwner.CurrentGun.barrelOffset.position, Quaternion.identity);
					Vector3 vector = parentOwner.unadjustedAimPoint - parentOwner.LockedApproximateSpriteCenter;
					Vector3 vector2 = parentOwner.specRigidbody.UnitCenter;
					if (vector.y > 0f)
					{
						vector2 += Vector3.up * 0.25f;
					}
					tk2dBaseSprite component4 = gameObject.GetComponent<tk2dBaseSprite>();
					if (component4)
					{
						component4.PlaceAtPositionByAnchor(vector2, tk2dBaseSprite.Anchor.MiddleCenter);
					}
					Vector2 vector3 = parentOwner.unadjustedAimPoint - parentOwner.LockedApproximateSpriteCenter;
					DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(gameObject, this.parentOwner.CurrentGun.barrelOffset.position, vector3, tossforce, false, false, true, false);


                    if (doesSpecialVfx)
                    {
						PlayBombCustomVFX play = gameObject.GetOrAddComponent<PlayBombCustomVFX>();
						play.VFX = VFX;
					}
					

                    if (DoesBlast)
                    {
						KTGStatusBomb stat = gameObject.GetOrAddComponent<KTGStatusBomb>();
						stat.DoesBlast = true;
                    }
                    if (DoesHex)
                    {
						KTGStatusBomb stat = gameObject.GetOrAddComponent<KTGStatusBomb>();
						stat.DoesHex = true;
					}

                    if (Therm)
                    {
						ThermBombDoer therm = gameObject.AddComponent<ThermBombDoer>();
						therm.player = parentOwner;
						therm.rotate = parentOwner.CurrentGun.CurrentAngle;
					}

					if (hitreallyhard)
					{
						KTGStatusBomb stat = gameObject.GetOrAddComponent<KTGStatusBomb>();
						stat.hitHard = true;
						stat.radius = 9;
					}

					projectile.SuppressHitEffects = true;
					projectile.hitEffects.suppressMidairDeathVfx = true;
					projectile.transform.position = new Vector3(0, 0);
					projectile.DieInAir();
				}
				
				this.hasbeenupdated = true;
			}
		}

       
        public bool hassetTarget = false;

		public GameObject gameobjecttospawn = null;
		public bool doesSpecialVfx;
		public GameObject VFX = null;

		public bool Therm = false;
		public bool DoesHex = false;
		public bool DoesBlast = false;
		public bool hasbeenBuffed = false;
		public bool hitreallyhard = false;
		
		public bool hasbeenupdated = false;

		private Projectile Projectile;

		private PlayerController parentOwner;
        public float tossforce;
    }
}
