using System;
using ItemAPI;
using UnityEngine;

namespace Knives
{   
	public class PlayerSepiaHandler : MonoBehaviour
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
		public PlayerSepiaHandler()
		{

		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0006765B File Offset: 0x0006585B
		private void Start()
		{
			player = base.GetComponent<PlayerController>();

		}

		private void Update()
		{
			if(GameManager.Instance.IsPaused != true)
            {
				if(sepiaLevel > 0)
                {

					Pixelator.Instance.SetFreezeFramePower(sepiaLevel, false);
					empowered = true;
				}
                else
                {
					if(empowered == true)
                    {
						Pixelator.Instance.SetFreezeFramePower(0, false);
						empowered = false;
					}
					
				}
            }
           
		}

		public bool empowered = false;
		public float sepiaLevel = 0;
		public PlayerController player;
		
	}
}