using System;
using ItemAPI;
using UnityEngine;

namespace Knives
{   //this code is by Nevernamed
	// Token: 0x02000192 RID: 402
	public class BulletPopOnDie : MonoBehaviour
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
		public BulletPopOnDie()
		{

		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0006765B File Offset: 0x0006585B
		private void Start()
		{
			Proj = base.GetComponent<Projectile>();
			player = Proj.Owner as PlayerController;
            Proj.OnDestruction += Proj_OnDestruction;
		}

        private void Proj_OnDestruction(Projectile obj)
        {
			for (int i = 0; i < 12; i++)
			{

				GameObject gameObject = SpawnManager.SpawnProjectile(MiscToolMethods.standardproj.gameObject, Proj.specRigidbody.UnitCenter, Quaternion.Euler(0f, 0f, (i * 30)), true);
				Projectile component = gameObject.GetComponent<Projectile>();
				bool flag = component != null;
				if (flag)
				{
					component.baseData.damage = 5 * (1 + GameManager.Instance.GetLastLoadedLevelDefinition().enemyHealthMultiplier);
					component.Owner = player;
					component.collidesWithEnemies = true;
					component.collidesWithPlayer = false;

				}


			}

		}

		private void Update()
		{
			
		}

		PlayerController player;
		public Projectile Proj;
	}
}