using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knives
{
	// Token: 0x02000411 RID: 1041
	public class TrailFireModifier : MonoBehaviour
	{
		// Token: 0x060015D1 RID: 5585 RVA: 0x001080A4 File Offset: 0x001062A4
		public TrailFireModifier()
		{
			AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
			TrailFireModifier.goopDefs = new List<GoopDefinition>();
			foreach (string text in TrailFireModifier.goops)
			{
				GoopDefinition goopDefinition;
				try
				{
					GameObject gameObject = assetBundle.LoadAsset(text) as GameObject;
					goopDefinition = gameObject.GetComponent<GoopDefinition>();
				}
				catch
				{
					goopDefinition = (assetBundle.LoadAsset(text) as GoopDefinition);
				}
				goopDefinition.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
				TrailFireModifier.goopDefs.Add(goopDefinition);
			}
			List<GoopDefinition> list = TrailFireModifier.goopDefs;
		}

		// Token: 0x060015D2 RID: 5586 RVA: 0x00108180 File Offset: 0x00106380
		private void Awake()
		{
			this.m_projectile = base.GetComponent<Projectile>();
		}

		// Token: 0x060015D3 RID: 5587 RVA: 0x00108190 File Offset: 0x00106390
		private void Update()
		{
			if(useSpecialGoop == false)
            {
				bool flag = this.goopType == 0 && this.needsToUseGreenFire;
				if (flag)
				{
					GoopDefinition goopDefinition = (PickupObjectDatabase.GetById(698) as Gun).DefaultModule.projectiles[0].GetComponent<GoopModifier>().goopDefinition;
					DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goopDefinition);
					goopManagerForGoopType.AddGoopCircle(this.m_projectile.sprite.WorldCenter, this.goopRadius, -1, false, -1);
				}
				else
				{
					DeadlyDeadlyGoopManager goopManagerForGoopType2 = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(TrailFireModifier.goopDefs[this.goopType]);
					goopManagerForGoopType2.AddGoopCircle(this.m_projectile.sprite.WorldCenter, this.goopRadius, -1, false, -1);
				}
			}
            else
            {
				if(goopy != null)
                {
					DeadlyDeadlyGoopManager goopManagerForGoopType2 = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goopy);
					goopManagerForGoopType2.AddGoopCircle(this.m_projectile.sprite.WorldCenter, this.goopRadius, -1, false, -1);
				}
				
			}
			
		}

		// Token: 0x04000F10 RID: 3856
		public int goopType = 0;

		// Token: 0x04000F11 RID: 3857
		public float goopRadius = 0.5f;

		// Token: 0x04000F12 RID: 3858
		public bool needsToUseGreenFire = false;
		public bool useSpecialGoop = false;
		public GoopDefinition goopy = null;

		// Token: 0x04000F13 RID: 3859
		private Projectile m_projectile;

		// Token: 0x04000F14 RID: 3860
		private static List<GoopDefinition> goopDefs;

		// Token: 0x04000F15 RID: 3861
		private static string[] goops = new string[]
		{
			"assets/data/goops/napalmgoopthatworks.asset",
			"assets/data/goops/poison goop.asset",
			"assets/data/goops/water goop.asset",
			"assets/data/goops/napalmgoopthatworks.asset"
		};
	}
}