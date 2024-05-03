using System;
using ItemAPI;
using UnityEngine;

namespace Knives
{   //this code is by Nevernamed
	// Token: 0x02000192 RID: 402
	public class ChildProjCleanup : MonoBehaviour
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
		public ChildProjCleanup()
		{
			
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0006765B File Offset: 0x0006585B
		private void Start()
		{
			childProj = base.GetComponent<Projectile>();
			if(doColor)
            {
				childProj.ChangeColor(.2f, color);
            }
		}

		public float delay = .5f;
		private void Update()
		{
			bool flag = this.parentProjectile == null;
			if (flag)
			{
				if(delay > 0)
                {
					delay -= Time.deltaTime;
                }
                else
                {
                    if (doColorCleanup)
                    {
						childProj.ChangeColor(0, Color.red);
                    }
					childProj.DieInAir();
				}
				
			}
		}

		// color should really only be used on Player Projectiles that we are doing a cleanup on. 
		public Color color;
		public Projectile parentProjectile;
		public Projectile childProj;
		public bool doColor = false;
        public bool doColorCleanup;
    }
}