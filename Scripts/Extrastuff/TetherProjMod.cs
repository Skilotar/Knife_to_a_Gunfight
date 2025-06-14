using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;

namespace Knives
{
	public class TetherProjMod : MonoBehaviour
	{

		public TetherProjMod()
		{

		}


		private void Start()
		{
			this.projectile = base.GetComponent<Projectile>();
			projectile.OnHitEnemy += ApplyTether;
            projectile.OnDestruction += Projectile_OnDestruction;
			player = projectile.Owner as PlayerController;
		}

        private void Projectile_OnDestruction(Projectile obj)
        {
            if(m_actor != null)
            {
                if (m_actor.gameObject.GetOrAddComponent<TetherEnemyToSpot>())
                {
					if(tetherBackToPlayer != true)
                    {
						TetherEnemyToSpot tether = m_actor.gameObject.gameObject.GetOrAddComponent<TetherEnemyToSpot>();
						tether.TetherPoint = projectile.LastPosition;
						tether.projDead = false;
					}
                    else
                    {
						TetherEnemyToSpot tether = m_actor.gameObject.gameObject.GetOrAddComponent<TetherEnemyToSpot>();
						tether.TetherPoint = player.CenterPosition;
						tether.projDead = false;
					}
				}
            }
        }

        private void ApplyTether(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(m_actor == null)
            {
				if(arg2.aiActor != null && arg3 == false)
                {
					if(arg2.aiActor.healthHaver.IsBoss != true)
                    {
						TetherEnemyToSpot tether = arg2.aiActor.gameObject.GetOrAddComponent<TetherEnemyToSpot>();
						tether.projDead = false;
						if (tetherBackToPlayer != true)
						{
							tether.TetherPoint = projectile.LastPosition;
						}
                        else
                        {
							tether.TetherPoint = player.CenterPosition;
							tether.UpdateToPlayer = true;
							tether.player = player;
						}
						tether.color = color;
						tether.TensileStength = TensileStength;
						tether.freewalkRadius = freewalkRadius;
						tether.minorslowwalkRadius = minorslowwalkRadius;
						tether.majorslowwalkRadius = majorslowwalkRadius;
						tether.majorKB = majorKB;
						tether.minorKB = minorKB;
						tether.StopbreakDistance = StopbreakDistance;
						tether.chainBreakRadius = chainBreakRadius;
						arg2.aiActor.behaviorSpeculator.Stun(.12f);
						arg2.aiActor.knockbackDoer.SetImmobile(false, "ShootBulletScript");
						m_actor = arg2.aiActor;
					}
					
				}
            }
        }

        private void Update()
		{
			if (m_actor != null)
			{
				TetherEnemyToSpot tether = m_actor.aiActor.gameObject.GetOrAddComponent<TetherEnemyToSpot>();
				tether.TetherPoint = projectile.LastPosition;
			}
		}

		public float StopbreakDistance = .5f;
		public float majorKB = 25;
		public float minorKB = 5;
		public bool tetherBackToPlayer = false;
		public PlayerController player;
		public float freewalkRadius = 4;
		public float minorslowwalkRadius = 5;
		public float majorslowwalkRadius = 6;
		public float chainBreakRadius = 8;
		public float TensileStength = .5f;
		public Color color = UnityEngine.Color.white;
		public AIActor m_actor;
		private Projectile projectile;
	}
}