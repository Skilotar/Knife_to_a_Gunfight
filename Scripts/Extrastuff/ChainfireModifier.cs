using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;

namespace Knives
{
	public class ChainFireModifier : MonoBehaviour
	{

		public ChainFireModifier()
		{
		
		}
		
		private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
			

			Projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(Projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemy));
			

		}

		private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool fatal)
		{
			
			if (arg2.aiActor != null)
			{
				if (arg2.aiActor.healthHaver != null)
				{
					HealthHaver enemyHealth = arg2.aiActor.healthHaver;
					if (enemyHealth && !enemyHealth.IsBoss && fatal && enemyHealth.aiActor && enemyHealth.aiActor.IsNormalEnemy && !enemyHealth.aiActor.IsHarmlessEnemy)
					{
						string name = enemyHealth.name;
						if (name.StartsWith("Bashellisk"))
						{
							return;
						}
						if (name.StartsWith("Blobulin") || name.StartsWith("Poisbulin"))
						{
							return;
						}


						SetDeadlyDeathBurst(arg2.aiActor);
						
						
					}
					if (arg1.ProjectilePlayerOwner() == null)
                    {
						arg2.aiActor.healthHaver.ApplyDamage(10 * (1 + GameManager.Instance.GetLastLoadedLevelDefinition().enemyHealthMultiplier), Vector2.zero, "Extra", CoreDamageTypes.None, DamageCategory.Unstoppable);
					}
					
				}

			}
		}

		public void SetDeadlyDeathBurst(AIActor ExplodyBoi)
		{
			
				for (int i = 0; i < 8; i++)
				{

					GameObject gameObject = SpawnManager.SpawnProjectile(ChainFire_Reagent.specialproj.gameObject, ExplodyBoi.specRigidbody.UnitCenter, Quaternion.Euler(0f, 0f, (i * 45)), true);
					Projectile component = gameObject.GetComponent<Projectile>();
					bool flag = component != null;
					if (flag)
					{
						
						component.Owner = ExplodyBoi;
						component.collidesWithEnemies = true;
						component.collidesWithPlayer = true;
						component.baseData.damage = 10;
						
						ChainFireModifier chain = component.gameObject.GetOrAddComponent<ChainFireModifier>();
						


					}


				}



		}

		private void Update()
		{
			
		}


		
		private Projectile Projectile;
		private PlayerController parentOwner;
		private Gun parentGun;

	}

}

