using System;
using ItemAPI;
using UnityEngine;

namespace Knives
{   //this code is by Nevernamed
	// Token: 0x02000192 RID: 402
	public class WreckingSpeedBooster : MonoBehaviour
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
		public WreckingSpeedBooster()
		{

		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0006765B File Offset: 0x0006585B
		private void Start()
		{
			player = base.GetComponent<PlayerController>();

			zoomy = player.gameObject.GetOrAddComponent<ImprovedAfterImage>();
			zoomy.dashColor = new Color(220f / 255f, 220f / 255f, 220f / 255f);
			zoomy.spawnShadows = false;
			zoomy.shadowTimeDelay = .1f;
			zoomy.shadowLifetime = .25f;
			zoomy.minTranslation = 0.1f;
			zoomy.OverrideImageShader = ShaderCache.Acquire("Brave/Internal/DownwellAfterImage");


		}
		private ImprovedAfterImage zoomy;
		private void Update()
		{
			if(player != null)
            {
				if(WreckingBoosted > 0)
                {
					WreckingBoosted -= BraveTime.DeltaTime;
					if(StatsOn == false)
                    {
						StatsOn = true;
						player.ownerlessStatModifiers.Add(WreckingBall.SpeedyModifier);
						player.stats.RecalculateStats(player);
						AkSoundEngine.PostEvent("Play_Speed_Up", base.gameObject);
						zoomy.spawnShadows = true;
					}

                }
                else
                {
					if (StatsOn == true)
					{
						StatsOn = false;
						player.ownerlessStatModifiers.Remove(WreckingBall.SpeedyModifier);
						player.stats.RecalculateStats(player);
						AkSoundEngine.PostEvent("Play_Speed_Down", base.gameObject);
						zoomy.spawnShadows = false;
					}
				}
			}

		}

		public bool StatsOn = false;

		public PlayerController player;
		public float WreckingBoosted;
	}
}