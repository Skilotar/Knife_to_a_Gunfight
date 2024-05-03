using System;
using ItemAPI;
using System.Reflection;
using UnityEngine;

namespace Knives
{
	public class BulletRechargeActiveMod : MonoBehaviour
	{

		public BulletRechargeActiveMod()
		{


		}


		private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
			this.parentOwner = this.Projectile.ProjectilePlayerOwner();
		}


		private void Update()
		{
			if (Projectile != null && !this.hasbeenupdated)
			{
				Upgradeprojectile(this.Projectile);
			}
		}

		public void Upgradeprojectile(Projectile projectile)
		{
			if (projectile != null)
			{

				projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemy));
				this.hasbeenupdated = true;
			}
		}

		public void OnHitEnemy(Projectile proj, SpeculativeRigidbody body, bool yes)
		{
			if (!this.hasbeenBuffed)
			{
				if (body != null && body.aiActor != null)
				{
					if (body.healthHaver != null)
					{
						if (yes)//lethal
						{
							FieldInfo remainingDamageCooldown = typeof(PlayerItem).GetField("remainingDamageCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
							FieldInfo remainingTimeCooldown = typeof(PlayerItem).GetField("remainingTimeCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
							FieldInfo remainingRoomCooldown = typeof(PlayerItem).GetField("remainingRoomCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
							
							PlayerController player = proj.Owner as PlayerController;
							foreach (PlayerItem item in player.activeItems)
							{
								remainingTimeCooldown.SetValue(item, 0);
								remainingDamageCooldown.SetValue(item, 0);
								remainingRoomCooldown.SetValue(item, 0); 
								
							}
							foreach(Gun gun in player.inventory.AllGuns)
                            {
                                if (gun.UsesRechargeLikeActiveItem && gun.encounterTrackable.EncounterGuid != "ski: letranger")
                                {
									gun.RemainingActiveCooldownAmount = 0;
                                }
                            }
							
						}
					}

				}
				hasbeenBuffed = true;

			}



		}


		public bool hasbeenBuffed = false;

		public bool hasbeenupdated = false;

		private Projectile Projectile;


		private PlayerController parentOwner;

		public BasicBeamController beamComp;

		public Gun gun;
	}
}
