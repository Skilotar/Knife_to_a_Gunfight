using System;
using System.Collections.Generic;
using ItemAPI;
using UnityEngine;

namespace Knives
{
	public class BubbleBulletMod : MonoBehaviour
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
		public BubbleBulletMod()
		{

		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0006765B File Offset: 0x0006585B
		private void Start()
		{
			Proj = base.GetComponent<Projectile>();
			//player = Proj.Owner as PlayerController; // Please assign players localy

			if (player != null && m_bubble == null)
            {
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(((Gun)PickupObjectDatabase.GetById(15)).DefaultModule.projectiles[0]);
				projectile.gameObject.SetActive(false);
				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				PierceProjModifier stabby = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
				stabby.penetration = 9;
				projectile.baseData.damage = 5f;
				projectile.baseData.force = 0;
				projectile.baseData.range *= 1f;
				projectile.baseData.speed *= 1f;
				MiscToolMethods.EasyAnimate(projectile, new List<string>() {
				"bubble_projectile_big_001",
				"bubble_projectile_big_002",
				"bubble_projectile_big_003",
				}, 3, new IntVector2(Proj.specRigidbody.HitboxPixelCollider.Dimensions.x + 10, Proj.specRigidbody.HitboxPixelCollider.Dimensions.x + 10), 3, false, tk2dSpriteAnimationClip.WrapMode.Loop);
				projectile.enemyImpactEventName = "";
				projectile.objectImpactEventName = "";
				SlowDownOverTimeModifier slow = projectile.gameObject.GetOrAddComponent<SlowDownOverTimeModifier>();
				slow.extendTimeByRangeStat = true;
				slow.doRandomTimeMultiplier = true;
				slow.killAfterCompleteStop = true;
				slow.timeTillKillAfterCompleteStop = 1f;

				Projectile bubble = MiscToolMethods.SpawnProjAtPosi(projectile, Proj.LastPosition, player, Proj.Direction.ToAngle());
				m_bubble = bubble;
				
				Proj.Speed = 0;
				Proj.specRigidbody.enabled = false;
			}
			if(m_bubble !=  null)
            {
				m_bubble.OnDestruction += Proj_OnDestruction;
			}

			AkSoundEngine.PostEvent("Play_Bubble_shot_001", base.gameObject);
		}

		private void Proj_OnDestruction(Projectile obj)
		{
			if(Proj != null)
            {
				Proj.Speed = 25;
				Proj.transform.SetParent(null, true);
				Proj.specRigidbody.enabled = true;
				Proj.specRigidbody.Reinitialize();
				Proj.SendInDirection(OMITBMathsAndLogicExtensions.GetVectorToNearestEnemy(Proj.LastPosition), true, true);
			}
			
		}

		private void Update()
		{
			if(m_bubble != null)
            {
				Proj.ResetDistance();
				Proj.transform.position = m_bubble.specRigidbody.UnitCenter;
				Proj.transform.parent = m_bubble.transform;
				Proj.specRigidbody.enabled = false;
				Proj.specRigidbody.Reinitialize();
				Proj.ResetDistance();
			}
            else
            {
				if (Proj != null && Sent == false)
				{
					Proj.Speed = 25;
					Proj.transform.SetParent(null, true);
					Proj.specRigidbody.enabled = true;
					Proj.specRigidbody.Reinitialize();
					Proj.SendInDirection(OMITBMathsAndLogicExtensions.GetVectorToNearestEnemy(Proj.LastPosition), true, true);
					Sent = true;
				}
			}
		}

		public bool Sent = false;
		public PlayerController player;
		public Projectile Proj;
		public Projectile m_bubble;
	}
}