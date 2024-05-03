using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;

namespace Knives
{
	public class EnemyProjOnHitPlayer : MonoBehaviour
	{

		public EnemyProjOnHitPlayer()
		{
			
		}


		private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
			
		}


		private void Update()
		{
			if (Projectile != null && once == false)
			{
				
				
				Upgradeprojectile(Projectile);
				

				
			}

		}

		public void Upgradeprojectile(Projectile projectile)
		{
			if (projectile != null)
			{
				projectile.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));

				once = true;
			}
		}

        private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if(otherRigidbody.GetComponent<PlayerController>() != null)
            {
				PlayerController player = otherRigidbody.GetComponent<PlayerController>();
                if (player.healthHaver.IsAlive == true)
                {
					if(player.healthHaver.IsVulnerable == true)// player probably took damage
                    {
						if(myRigidbody != null)
                        {
							hit = true;
							owner = (AIActor)(myRigidbody.projectile.Owner);

                            if (IsForCoolStatus)
                            {
								doCoolChanceCalc();
							}
                        }
                    }
                }

			}

        }

        private void doCoolChanceCalc()
        {
			if (owner.gameObject.GetComponent<AiactorSpecialStates>() == null)
			{
				owner.gameObject.GetOrAddComponent<AiactorSpecialStates>();
			}
			AiactorSpecialStates state = owner.gameObject.GetOrAddComponent<AiactorSpecialStates>();

			float cappedCoolness = state.EnemyCoolnessLevel;
			if (state.EnemyCoolnessLevel > 20) cappedCoolness = 20;
			float chance = 20 - cappedCoolness; // max chance 100%
			if (UnityEngine.Random.Range(0, 25) < cappedCoolness)
			{
				int itemget = GetItem();
				if (itemget == 0000000)
				{
					GameManager.Instance.RewardManager.SpawnRoomClearChestAt((owner.GetAbsoluteParentRoom()).GetBestRewardLocation(new IntVector2(2, 1)));
					
				}
				else
				{
					LootEngine.SpawnItem(PickupObjectDatabase.GetById(itemget).gameObject, owner.sprite.WorldCenter, new Vector3(0, 0, 1), 7f, false, false, false);
				}

			}

			
		}

		private int GetItem()
		{
			int rng = UnityEngine.Random.Range(0, CoolEnemiesStatusController.Itemlist.Count - 1);

			return CoolEnemiesStatusController.Itemlist[rng];
		}

		public bool once = false;
		public bool hit = false;
		public AIActor owner;
		private Projectile Projectile;
		public bool IsForCoolStatus = false;
		private PlayerController parentOwner;

		private Projectile TargetProjectile;
	}
}
