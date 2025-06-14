using System;
using System.Collections.Generic;
using UnityEngine;
using Dungeonator;
using Alexandria.ItemAPI;

namespace Knives
{
	// Token: 0x020002D6 RID: 726
	public class OrangeProjectileModifier : MonoBehaviour
	{
		// Token: 0x06000F39 RID: 3897 RVA: 0x000CBDEC File Offset: 0x000C9FEC
		public OrangeProjectileModifier()
		{
			
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x000CBE08 File Offset: 0x000CA008
		private void Start()
		{
			
			this.m_projectile = base.GetComponent<Projectile>();
			this.Owner = m_projectile.Owner as PlayerController;
			basespeed = m_projectile.baseData.speed;

		}

		
		private void Update()
        {
			
		}


		public float internalcooldown = 0;
		public int number;
		private void FixedUpdate()
		{
			bool flag = this.m_projectile != null;
			if (flag)
			{
				if(internalcooldown <= 0)
                {
					if (Owner.IsInCombat)
					{

						RoomHandler room = m_projectile.LastPosition.GetAbsoluteRoom();
						if (room != null)
						{
							number = 0;
							List<AIActor> activeEnemies = ((PlayerController)m_projectile.Owner).CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
							if (activeEnemies.Count > 0 && activeEnemies != null)
							{

								foreach (AIActor target in activeEnemies)
								{

									if (target != null && target.gameObject != null)
									{

										if (Vector2.Distance(m_projectile.LastPosition, target.specRigidbody.UnitCenter) <= 6)
										{
											number++;
											float Angle = (target.CenterPosition - m_projectile.specRigidbody.UnitCenter).ToAngle();
											Projectile pew = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, m_projectile.LastPosition, Owner, Angle);
											pew.baseData.damage = 7;
											LightningProjectileComp zap = pew.gameObject.GetOrAddComponent<LightningProjectileComp>();
											List<string> BeamAnimPaths = new List<string>()
											{

											"Knives/Resources/BeamSprites/Sp4rk_001",
											"Knives/Resources/BeamSprites/Sp4rk_002",
											"Knives/Resources/BeamSprites/Sp4rk_003",
											};
											pew.AddTrailToProjectile(
												 "Knives/Resources/BeamSprites/Sp4rk_001",
												new Vector2(3, 2),
												new Vector2(1, 1),
												BeamAnimPaths, 20,
												BeamAnimPaths, 20,
												-1,
												0.0001f,
												5,
												true
												);
											internalcooldown = .15f;

										}

									}

								}
								float divmod = (2 * number);
								if (divmod == 0) divmod = 1;
								m_projectile.baseData.speed = (basespeed * Owner.stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed)) / divmod;
								m_projectile.UpdateSpeed();
							}
							float divmod2 = (2 * number);
							if (divmod2 == 0) divmod2 = 1;
							m_projectile.baseData.speed = (basespeed * Owner.stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed)) / divmod2;
							m_projectile.UpdateSpeed();

						}
					}
					
                }
                else
                {
					internalcooldown -= BraveTime.DeltaTime;
                }
				
			}

		}

		
		private List<AIActor> Bound = new List<AIActor>();

		private float basespeed;
		
		
		private PlayerController Owner;
		private Projectile m_projectile;
	}


}
