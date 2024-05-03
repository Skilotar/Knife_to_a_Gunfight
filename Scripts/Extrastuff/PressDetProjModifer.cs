using System;
using ItemAPI;
using UnityEngine;
using Dungeonator;

namespace Knives
{
	public class pressDetProjModifier : MonoBehaviour
	{

		public pressDetProjModifier()
		{
			isThumperRocket = false;

		}


		private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
			this.parentOwner = this.Projectile.ProjectilePlayerOwner();
			
		}

		
		public void Update()
		{
			
			if(Projectile != null)
            {
				if (this.parentOwner != null)
				{
					if (Projectile.GetComponent<pressDetProjModifier>())
					{
						if (Projectile.GetComponent<pressDetProjModifier>().isThumperRocket)
						{
							BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer((this.parentOwner as PlayerController).PlayerIDX);

							if (!instanceForPlayer.ActiveActions.ShootAction.IsPressed && Time.timeScale != 0 && !instanceForPlayer.ActiveActions.MapAction.IsPressed)
							{

								HereComesTheBOOM();
								RoomHandler room = parentOwner.CurrentRoom;
								if (room != null)
								{
									foreach (var enemy in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
									{
										if (enemy.aiActor != null)
										{
											if (Vector2.Distance(enemy.Position, this.Projectile.LastPosition) < 5)
											{
                                                if (enemy.healthHaver.IsAlive)
                                                {
													enemy.ApplyEffect(PickupObjectDatabase.GetById(295).GetComponent<BulletStatusEffectItem>().FireModifierEffect);
												}
												
											}
										}

									}
								}

							}
						}
					}
				}
			}
			

		}

		public void HereComesTheBOOM()
        {
            
				this.DoSafeExplosion(this.Projectile.LastPosition);
			
				this.Projectile.DieInAir();
			
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
			damageRadius = 5f,
			damageToPlayer = 0f,
			doDamage = true,
			damage = 14f,
			doDestroyProjectiles = true,
			doForce = true,
			debrisForce = 0f,
			preventPlayerForce = false,
			explosionDelay = 0.0f,
			usesComprehensiveDelay = false,
			doScreenShake = false,
			playDefaultSFX = true,
			breakSecretWalls = true,
			secretWallsRadius = 3,
			force = 20,
			forceUseThisRadius = true,
			
			
		};

		public Gun gun;
		public bool isThumperRocket;
		private Projectile Projectile;
		public bool hasbeenBuffed = false;
		public bool hasbeenupdated = false;
		private PlayerController parentOwner;

	}
}


