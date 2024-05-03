using System;
using UnityEngine;

namespace Knives
{
	// Token: 0x020000B4 RID: 180
	internal class GunVolleyModificationSynergy : MonoBehaviour
	{
		// Token: 0x06000543 RID: 1347 RVA: 0x00040C68 File Offset: 0x0003EE68
		private void Awake()
		{
			this.m_gun = base.GetComponent<Gun>();
			Gun gun = this.m_gun;
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x00040C8C File Offset: 0x0003EE8C
		private void Update()
		{
			bool flag = this.m_gun;
			if (flag)
			{
				bool flag2 = this.m_gun.GunPlayerOwner();
				if (flag2)
				{
					bool flag3 = this.m_gun.GunPlayerOwner() != this.lastRegisteredOwner;
					if (flag3)
					{
						bool flag4 = this.lastRegisteredOwner != null;
						if (flag4)
						{
							this.AssignVolley(this.lastRegisteredOwner, false);
						}
						this.AssignVolley(this.m_gun.GunPlayerOwner(), true);
						this.lastRegisteredOwner = this.m_gun.GunPlayerOwner();
					}
				}
				else
				{
					bool flag5 = this.lastRegisteredOwner != null;
					if (flag5)
					{
						this.AssignVolley(this.lastRegisteredOwner, false);
					}
				}
			}
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x00040D48 File Offset: 0x0003EF48
		private void Destroy()
		{
			bool flag = this.lastRegisteredOwner;
			if (flag)
			{
				this.AssignVolley(this.lastRegisteredOwner, false);
			}
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x00040D74 File Offset: 0x0003EF74
		private void AssignVolley(PlayerController target, bool assign)
		{
			if (assign)
			{
				target.stats.AdditionalVolleyModifiers += this.ModifyVolley;
				target.stats.RecalculateStats(target, false, false);
			}
			else
			{
				target.stats.AdditionalVolleyModifiers -= this.ModifyVolley;
				target.stats.RecalculateStats(target, false, false);
			}
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x00040DDC File Offset: 0x0003EFDC
		public void ModifyVolley(ProjectileVolleyData volleyToModify)
		{
			int count = volleyToModify.projectiles.Count;
			for (int i = 0; i < count; i++)
			{
				ProjectileModule projectileModule = volleyToModify.projectiles[i];
				for (int j = 0; j < 2; j++)
				{
					int sourceIndex = i;
					bool flag = projectileModule.CloneSourceIndex >= 0;
					if (flag)
					{
						sourceIndex = projectileModule.CloneSourceIndex;
					}
					ProjectileModule projectileModule2 = ProjectileModule.CreateClone(projectileModule, false, sourceIndex);
					projectileModule2.angleVariance *= 1.2f;
					projectileModule2.ignoredForReloadPurposes = true;
					projectileModule2.ammoCost = 0;
					volleyToModify.projectiles.Add(projectileModule2);
				}
			}
		}

		// Token: 0x0400057A RID: 1402
		private PlayerController lastRegisteredOwner;

		// Token: 0x0400057B RID: 1403
		private Gun m_gun;
	}
}