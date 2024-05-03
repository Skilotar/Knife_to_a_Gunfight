using System;
using ItemAPI;
using UnityEngine;

namespace Knives
{	//this code is by Nevernamed
	// Token: 0x02000192 RID: 402
	public class ProjectileSplitController : MonoBehaviour
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
		public ProjectileSplitController()
		{
			this.distanceTillSplit = 7.5f;
			this.splitAngles = 35f;
			this.amtToSplitTo = 3;
			this.dmgMultAfterSplit = 0.66f;
			this.sizeMultAfterSplit = 0.8f;
			this.removeComponentAfterUse = true;
			this.splitOnEnemy = false;
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0006765B File Offset: 0x0006585B
		private void Start()
		{
			this.parentProjectile = base.GetComponent<Projectile>();
			this.parentOwner = this.parentProjectile.ProjectilePlayerOwner();
			parentProjectile.OnHitEnemy += this.Splitprocess;
		}

        private void Splitprocess(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (splitOnEnemy)
            {
                if (arg2.aiActor)
                {
					this.SplitProjectile();
					main = arg2.aiActor;
				}

			}
        }

		AIActor main;

        // Token: 0x0600086D RID: 2157 RVA: 0x0006767C File Offset: 0x0006587C
        private void Update()
		{
			bool flag = this.parentProjectile != null && this.distanceBasedSplit;
			if (flag)
			{
				bool flag2 = this.parentProjectile.GetElapsedDistance() > this.distanceTillSplit;
				if (flag2)
				{
					this.SplitProjectile();
					
				}
			}
		}

		// Token: 0x0600086E RID: 2158 RVA: 0x000676D4 File Offset: 0x000658D4
		private void SplitProjectile()
		{
			PlayerController player = this.parentOwner;
			float num = this.splitAngles / ((float)this.amtToSplitTo);
			float num2 = this.parentProjectile.Direction.ToAngle();
			float num3 = num2 + this.splitAngles * 0.5f;
			int num4 = 0;
			if (parentOwner.CurrentGun.Volley.projectiles.Count > 10)
			{
				// too many projectiles ajkla;sdlf;lfjksladlf
				ProjectileSplitController component2 = parentProjectile.gameObject.GetComponent<ProjectileSplitController>();
				UnityEngine.Object.Destroy(component2);
			}
			else
			{

				for (int i = 0; i < this.amtToSplitTo; i++)
				{
					float num5 = num3 - num * (float)num4;
					GameObject prefab = FakePrefab.Clone(this.parentProjectile.gameObject);
					GameObject gameObject = SpawnManager.SpawnProjectile(prefab, this.parentProjectile.sprite.WorldCenter, Quaternion.Euler(0f, 0f, num5), true);
					Projectile component = gameObject.GetComponent<Projectile>();
					bool flag = component != null;
					if (flag)
					{
						component.Owner = this.parentOwner;
						component.Shooter = this.parentOwner.specRigidbody;
						component.baseData.damage *= this.dmgMultAfterSplit;
						component.RuntimeUpdateScale(this.sizeMultAfterSplit);
						ProjectileSplitController component2 = component.gameObject.GetComponent<ProjectileSplitController>();
						bool flag2 = component2 && this.removeComponentAfterUse;
						if (flag2)
						{
							UnityEngine.Object.Destroy(component2);
						}
						if (parentProjectile.AdditionalScaleMultiplier >= 1.04f)
						{
							UnityEngine.Object.Destroy(component2);
						}

                        if (splitOnEnemy)
                        {
							PierceProjModifier stabby = component2.gameObject.GetOrAddComponent<PierceProjModifier>();
							component2.gameObject.GetComponent<SpeculativeRigidbody>().OnPreRigidbodyCollision += this.HitMain;
                        }

					}
					num4++;
				}
				float mirrorAngle = splitAngles * -1;
				float number = mirrorAngle / ((float)this.amtToSplitTo);
				float number2 = this.parentProjectile.Direction.ToAngle();
				float number3 = number2 + mirrorAngle * 0.5f;
				int number4 = 0;
				for (int i = 0; i < this.amtToSplitTo; i++)
				{
					float number5 = number3 - number * (float)number4;
					GameObject prefab = FakePrefab.Clone(this.parentProjectile.gameObject);
					GameObject gameObject = SpawnManager.SpawnProjectile(prefab, this.parentProjectile.sprite.WorldCenter, Quaternion.Euler(0f, 0f, number5), true);
					Projectile component = gameObject.GetComponent<Projectile>();
					bool flag = component != null;
					if (flag)
					{
						component.Owner = this.parentOwner;
						component.Shooter = this.parentOwner.specRigidbody;
						if (this.parentProjectile.baseData.damage < this.parentProjectile.GetCachedBaseDamage * .4)
						{

						}
						else
						{
							component.baseData.damage *= this.dmgMultAfterSplit;
						}

						component.RuntimeUpdateScale(this.sizeMultAfterSplit);
						ProjectileSplitController component2 = component.gameObject.GetComponent<ProjectileSplitController>();

						bool flag2 = component2 && this.removeComponentAfterUse;
						if (flag2)
						{
							UnityEngine.Object.Destroy(component2);
						}
						if (parentProjectile.AdditionalScaleMultiplier >= 1.04f)
						{
							UnityEngine.Object.Destroy(component2);
						}
						
						
					}
					number4++;
				}


				UnityEngine.Object.Destroy(this.parentProjectile.gameObject);
			}
		}

        private void HitMain(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if(otherRigidbody.aiActor == main)
            {
				PhysicsEngine.SkipCollision = true;
            }
        }


        // Token: 0x040004C7 RID: 1223
        private Projectile parentProjectile;

		// Token: 0x040004C8 RID: 1224
		private PlayerController parentOwner;

		// Token: 0x040004C9 RID: 1225
		

		// Token: 0x040004CA RID: 1226
		public bool distanceBasedSplit;

		// Token: 0x040004CB RID: 1227
		public float distanceTillSplit;

		// Token: 0x040004CC RID: 1228
		public bool splitOnEnemy;

		// Token: 0x040004CD RID: 1229
		public float splitAngles;

		// Token: 0x040004CE RID: 1230
		public int amtToSplitTo;

		// Token: 0x040004CF RID: 1231
		public float dmgMultAfterSplit;

		// Token: 0x040004D0 RID: 1232
		public float sizeMultAfterSplit;

		// Token: 0x040004D1 RID: 1233
		public bool removeComponentAfterUse;

		public bool shorten;

		public int numberofsplits;
	}
}