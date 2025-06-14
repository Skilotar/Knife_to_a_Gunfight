using System;
using ItemAPI;
using UnityEngine;
using Dungeonator;

namespace Knives
{   
	public class ExplodeInFire : MonoBehaviour
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
		public ExplodeInFire()
		{

		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0006765B File Offset: 0x0006585B
		private void Start()
		{
			main = base.gameObject;
		}

		
		private void Update()
		{
			if(main != null)
            {
				
                if (IsPointOnFire(main.GetComponent<tk2dSprite>().WorldCenter))
                {
					DoSafeExplosion(main.GetComponent<tk2dSprite>().WorldCenter);
					UnityEngine.GameObject.Destroy(main);
				}
                
            }
		}

		private bool IsPointOnFire(Vector2 testPos)
		{
			if(testPos != null)
            {
				IntVector2 Burnyoustupidbook = (testPos / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
				if(Burnyoustupidbook != null)
                {
					if (DeadlyDeadlyGoopManager.allGoopPositionMap.ContainsKey(Burnyoustupidbook))
					{
						DeadlyDeadlyGoopManager deadlyDeadlyGoopManager = DeadlyDeadlyGoopManager.allGoopPositionMap[Burnyoustupidbook];
						return deadlyDeadlyGoopManager.IsPositionOnFire(testPos);
					}
					return false;
				}
				return false;
			}
			return false;
		}

		public void DoSafeExplosion(Vector3 position)
		{
			ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
			this.smallPlayerSafeExplosion.effect = defaultSmallExplosionData2.effect;
			this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
			this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
			Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);
		}

		private ExplosionData smallPlayerSafeExplosion = new ExplosionData
		{
			damageRadius = 4.5f,
			damageToPlayer = 0f,
			doDamage = true,
			damage = 25f,
			doDestroyProjectiles = true,
			doForce = true,
			debrisForce = 0f,
			preventPlayerForce = true,
			explosionDelay = 0f,
			usesComprehensiveDelay = false,
			doScreenShake = false,
			playDefaultSFX = true,
			breakSecretWalls = true,
			secretWallsRadius = 3,
			force = 2,
			forceUseThisRadius = true,

		};


		public ExplosionData boom;
		public GameObject main;
	}
}