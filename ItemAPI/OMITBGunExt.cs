using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Knives
{
	// Token: 0x020002C6 RID: 710
	internal static class OMITBGunExtensions
	{
		// Token: 0x06000EEB RID: 3819 RVA: 0x000C8F30 File Offset: 0x000C7130
		public static bool IsCurrentGun(this Gun gun)
		{
			bool flag = gun && gun.CurrentOwner;
			bool result;
			if (flag)
			{
				bool flag2 = gun.CurrentOwner.CurrentGun == gun;
				result = flag2;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000EEC RID: 3820 RVA: 0x000C8F7C File Offset: 0x000C717C
		public static PlayerController GunPlayerOwner(this Gun bullet)
		{
			bool flag = bullet && bullet.CurrentOwner && bullet.CurrentOwner is PlayerController;
			PlayerController result;
			if (flag)
			{
				result = (bullet.CurrentOwner as PlayerController);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000EED RID: 3821 RVA: 0x000C8FC8 File Offset: 0x000C71C8
		public static ProjectileModule AddProjectileModuleToRawVolley(this Gun gun, ProjectileModule projectile)
		{
			gun.RawSourceVolley.projectiles.Add(projectile);
			return projectile;
		}

		// Token: 0x06000EEE RID: 3822 RVA: 0x000C8FF0 File Offset: 0x000C71F0
		public static ProjectileModule AddProjectileModuleToRawVolleyFrom(this Gun gun, Gun other, bool cloned = true, bool clonedProjectiles = true)
		{
			ProjectileModule defaultModule = other.DefaultModule;
			bool flag = !cloned;
			ProjectileModule result;
			if (flag)
			{
				result = gun.AddProjectileModuleToRawVolley(defaultModule);
			}
			else
			{
				ProjectileModule projectileModule = ProjectileModule.CreateClone(defaultModule, false, -1);
				projectileModule.projectiles = new List<Projectile>(defaultModule.projectiles.Capacity);
				for (int i = 0; i < defaultModule.projectiles.Count; i++)
				{
					projectileModule.projectiles.Add((!clonedProjectiles) ? defaultModule.projectiles[i] : OMITBGunExtensions.ClonedPrefab(defaultModule.projectiles[i]));
				}
				result = gun.AddProjectileModuleToRawVolley(projectileModule);
			}
			return result;
		}

		public static Projectile ClonedPrefab(this Projectile orig)
		{
			if (orig == null)
			{
				return null;
			}
			orig.gameObject.SetActive(false);
			Projectile component = UnityEngine.Object.Instantiate<GameObject>(orig.gameObject).GetComponent<Projectile>();
			orig.gameObject.SetActive(true);
			component.name = orig.name;
			UnityEngine.Object.DontDestroyOnLoad(component.gameObject);
			return component;
		}

		public static ProjectileModule RawDefaultModule(this Gun self)
		{
			bool flag = self.RawSourceVolley;
			ProjectileModule result;
			if (flag)
			{
				bool modulesAreTiers = self.RawSourceVolley.ModulesAreTiers;
				if (modulesAreTiers)
				{
					for (int i = 0; i < self.RawSourceVolley.projectiles.Count; i++)
					{
						ProjectileModule projectileModule = self.RawSourceVolley.projectiles[i];
						bool flag2 = projectileModule != null;
						if (flag2)
						{
							int num = (projectileModule.CloneSourceIndex < 0) ? i : projectileModule.CloneSourceIndex;
							bool flag3 = num == self.CurrentStrengthTier;
							if (flag3)
							{
								return projectileModule;
							}
						}
					}
				}
				result = self.RawSourceVolley.projectiles[0];
			}
			else
			{
				result = self.singleModule;
			}
			return result;
		}

		// Token: 0x06000EF0 RID: 3824 RVA: 0x000C9154 File Offset: 0x000C7354
		public static void AddStatToGun(this Gun item, PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
		{
			StatModifier statModifier = new StatModifier
			{
				amount = amount,
				statToBoost = statType,
				modifyType = method
			};
			bool flag = item.passiveStatModifiers == null;
			if (flag)
			{
				item.passiveStatModifiers = new StatModifier[]
				{
					statModifier
				};
			}
			else
			{
				item.passiveStatModifiers = item.passiveStatModifiers.Concat(new StatModifier[]
				{
					statModifier
				}).ToArray<StatModifier>();
			}
		}

		// Token: 0x06000EF1 RID: 3825 RVA: 0x000C91BC File Offset: 0x000C73BC
		public static void RemoveStatFromGun(this Gun item, PlayerStats.StatType statType)
		{
			List<StatModifier> list = new List<StatModifier>();
			for (int i = 0; i < item.passiveStatModifiers.Length; i++)
			{
				bool flag = item.passiveStatModifiers[i].statToBoost != statType;
				if (flag)
				{
					list.Add(item.passiveStatModifiers[i]);
				}
			}
			item.passiveStatModifiers = list.ToArray();
		}
	}
}

