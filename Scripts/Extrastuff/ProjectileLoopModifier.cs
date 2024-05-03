using System;
using ItemAPI;
using UnityEngine;
using Dungeonator;
using System.Collections;
using System.Collections.Generic;

namespace Knives
{
	public class ProjectileLoopModifier : MonoBehaviour
	{

		public ProjectileLoopModifier()
		{

			islooper = false;
			hasbeenupdated = false;

		}


		private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
			this.parentOwner = this.Projectile.ProjectilePlayerOwner();
			
		}


		private void Update()
		{

			if (this.parentOwner != null)
			{
				if (Projectile.GetComponent<ProjectileLoopModifier>())
				{
					if (Projectile.GetComponent<ProjectileLoopModifier>().islooper)
					{
						if (hasbeenupdated == false)
                        {
							Projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(Projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy));
							originpoint = Projectile.LastPosition;
							hasbeenupdated = true;
						}
					}
				}
			}

		}

		
		private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
			SpawnProjectile(arg1);
			Projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Remove(Projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy));
		}

		private void SpawnProjectile(Projectile arg1)
		{
			
			GameObject obj = SpawnManager.SpawnProjectile(arg1.gameObject, originpoint, Quaternion.Euler(0,0, arg1.Direction.ToAngle()));
			Projectile component = obj.GetComponent<Projectile>();
			if (component != null)
			{
				component.Owner = parentOwner;
				component.Shooter = parentOwner.specRigidbody;
				component.collidesWithPlayer = false;
				component.sprite.usesOverrideMaterial = true;
				component.baseData.damage *= .5f;
				Material material = component.sprite.renderer.material;
				material.shader = ShaderCache.Acquire("Brave/Internal/HologramShader");
				material.SetFloat("_IsGreen", 0f);
			}

		}


		public Gun gun;
		public bool islooper;
		private Projectile Projectile;
		
		public bool hasbeenupdated = false;
		private PlayerController parentOwner;
        private Vector3 originpoint;
    }
}