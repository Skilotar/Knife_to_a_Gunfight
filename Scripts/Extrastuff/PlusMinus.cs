using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using System.Collections.Generic;

namespace Knives
{
	public class PlusMinusComp : MonoBehaviour
	{

		public PlusMinusComp()
		{

		}


		private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
			this.parentOwner = this.Projectile.ProjectilePlayerOwner();

		}

		bool upgradera = false;
		private void Update()
		{
			if (Projectile != null && SpouceProj == null)
			{
                // search for wife 
                List<Projectile> potentialWife = GetPossibleSpouceBullets();
				if(potentialWife.Count > 0)
                {
					SpouceProj = potentialWife[potentialWife.Count - 1];
					SpouceProj.gameObject.GetOrAddComponent<projectileStates>().Married = true;
					// by the power vested in me I pronouce you man and wife
				}

			}
			if (Projectile != null && SpouceProj != null)
			{
				if (upgradera == false)
				{
					
					// do chain stuff here
					ChainLightningModifier chain = Projectile.gameObject.GetOrAddComponent<ChainLightningModifier>();
					chain.CanChainToAnyProjectile = false;
					chain.UseForcedLinkProjectile = true;
					chain.ForcedLinkProjectile = SpouceProj;
					chain.maximumLinkDistance = 20f;
					chain.LinkVFXPrefab = (PickupObjectDatabase.GetById(298) as ComplexProjectileModifier).ChainLightningVFX;
					chain.damagePerHit = 5;

					HomingModifier home = Projectile.GetComponent<HomingModifier>();
					home.AngularVelocity = 0;

					ImprovedAfterImage trail = Projectile.gameObject.GetOrAddComponent<ImprovedAfterImage>();
					trail.shadowLifetime = .1f;
					trail.shadowTimeDelay = .01f;
					trail.dashColor = new Color(.25f, .75f, .92f);
					trail.spawnShadows = true;

					ImprovedAfterImage trail2 = SpouceProj.gameObject.GetOrAddComponent<ImprovedAfterImage>();
					trail2.shadowLifetime = .1f;
					trail2.shadowTimeDelay = .01f;
					trail2.dashColor = new Color(.82f, 0f, .0f);
					trail2.spawnShadows = true;

					ChildProjCleanup clean = SpouceProj.gameObject.GetOrAddComponent<ChildProjCleanup>();
					clean.parentProjectile = Projectile;
					clean.doColor = false;

					PierceProjModifier stabby = Projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
					stabby.penetratesBreakables = true;
					stabby.penetration = 1;
					PierceProjModifier stabby2 = SpouceProj.gameObject.GetOrAddComponent<PierceProjModifier>();
					stabby2.penetratesBreakables = true;
					stabby2.penetration = 1;

					

					upgradera = true;
				}
			}
		}

		public List<Projectile> GetPossibleSpouceBullets()
		{
			List<Projectile> list = new List<Projectile>();
			var allProjectiles = StaticReferenceManager.AllProjectiles;
			for (int i = 0; i < allProjectiles.Count; i++)
			{
				Projectile projectile1 = allProjectiles[i];
				if (projectile1.Owner != null)
				{
					if (projectile1.GetComponent<projectileStates>())
					{
						if (projectile1.GetComponent<projectileStates>().PlusMinus == true && projectile1.GetComponent<projectileStates>().Married == false)
						{
							if(Vector2.Distance(Projectile.LastPosition, projectile1.LastPosition) < 10)
                            {
								list.Add(projectile1);
							}
							
						}
					}
					
				}
			}
			return list;
		}

		public void Upgradeprojectile(Projectile projectile)
		{
			if (projectile != null)
			{

				this.hasbeenupdated = true;
			}
		}



		public bool hasbeenBuffed = false;
		public bool hasbeenupdated = false;

		private Projectile Projectile;

		private PlayerController parentOwner;

		private Projectile SpouceProj;
	}
}