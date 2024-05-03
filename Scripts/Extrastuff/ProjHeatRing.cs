using System;
using System.Collections;
using Dungeonator;
using UnityEngine;

using ItemAPI;


namespace Knives
{
	public class ProjectileHeatRingModifier : MonoBehaviour
	{
	
		public bool IsActive
		{
			get
			{
				return this.m_indicator;
			}
		}
		public void Start()
        {
			projectile = base.gameObject.GetComponent<Projectile>();
			HandleRadialIndicator(rad, projectile);
			Trigger(rad, dur, StaticStatusEffects.hotLeadEffect, projectile);
        }

		public void Update()
        {

        }
	
		public void Trigger(float Radius, float Duration, GameActorFireEffect HeatEffect, Projectile proj)
		{
			proj.StartCoroutine(this.HandleHeatEffectsCR(Radius, Duration, HeatEffect, proj));
		}

	
		private IEnumerator HandleHeatEffectsCR(float Radius, float Duration, GameActorFireEffect HeatEffect, Projectile proj)
		{
			this.HandleRadialIndicator(Radius, proj);
			float elapsed = 0f;
			RoomHandler r = proj.transform.position.GetAbsoluteRoom();
			Action<AIActor, float> AuraAction = delegate (AIActor actor, float dist)
			{
				actor.ApplyEffect(HeatEffect, 1f, null);
			};
			while (elapsed < Duration)
			{
				elapsed += BraveTime.DeltaTime;
				r.ApplyActionToNearbyEnemies(proj.transform.position, Radius, AuraAction);
				yield return null;
			}
			this.UnhandleRadialIndicator();
			yield break;
		}

	
		private void HandleRadialIndicator(float Radius, Projectile proj)
		{
			if (!this.m_indicator)
			{
				Vector3 vector = proj.LastPosition;
				m_indicator = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), vector, Quaternion.identity, proj.transform)).GetComponent<HeatIndicatorController>();
				m_indicator.CurrentRadius = Radius;
			}
		}

	
		private void UnhandleRadialIndicator()
		{
			if (this.m_indicator)
			{
				this.m_indicator.EndEffect();
				this.m_indicator = null;
			}
		}

		public float dur = 1f;
		public float rad = 3f;
		private HeatIndicatorController m_indicator;
		public Projectile projectile;
	}
}
