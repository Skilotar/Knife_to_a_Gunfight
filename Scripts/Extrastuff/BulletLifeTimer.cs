using System;
using UnityEngine;

namespace Knives
{
	// Token: 0x020002D6 RID: 726
	public class BulletLifeTimer : MonoBehaviour
	{
		// Token: 0x06000F39 RID: 3897 RVA: 0x000CBDEC File Offset: 0x000C9FEC
		public BulletLifeTimer()
		{
			this.secondsTillDeath = 1f;
			this.eraseInsteadOfDie = false;
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x000CBE08 File Offset: 0x000CA008
		private void Start()
		{
			this.timer = this.secondsTillDeath;
			this.m_projectile = base.GetComponent<Projectile>();
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x000CBE24 File Offset: 0x000CA024
		private void FixedUpdate()
		{
			bool flag = this.m_projectile != null;
			if (flag)
			{
				bool flag2 = this.timer > 0f;
				if (flag2)
				{
					this.timer -= BraveTime.DeltaTime;
				}
				bool flag3 = this.timer <= 0f;
				if (flag3)
				{
					bool flag4 = this.eraseInsteadOfDie;
					if (flag4)
					{
						UnityEngine.Object.Destroy(this.m_projectile.gameObject);
					}
					else
					{
						this.m_projectile.DieInAir(false, true, true, false);
					}
				}
			}
		}

		// Token: 0x04000B45 RID: 2885
		public float secondsTillDeath;

		// Token: 0x04000B46 RID: 2886
		public bool eraseInsteadOfDie;

		// Token: 0x04000B47 RID: 2887
		private float timer;

		// Token: 0x04000B48 RID: 2888
		private Projectile m_projectile;
	}
}
