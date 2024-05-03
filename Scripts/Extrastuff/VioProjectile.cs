using System;
using UnityEngine;

namespace Knives
{
	// Token: 0x0200002E RID: 46
	internal class vio_projectile : MonoBehaviour
	{
		// Token: 0x060000EB RID: 235 RVA: 0x00007FA4 File Offset: 0x000061A4
		public void Start()
		{
			this.projectile = base.GetComponent<Projectile>();
			this.player = (this.projectile.Owner as PlayerController);
			Projectile projectile = this.projectile;
			this.projectile.sprite.spriteId = this.projectile.sprite.GetSpriteIdByName("vio_projectile_001");
		}

		// Token: 0x04000054 RID: 84
		private Projectile projectile;

		// Token: 0x04000055 RID: 85
		private PlayerController player;
	}
}