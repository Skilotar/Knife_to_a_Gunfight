using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;

namespace Knives
{
	public class ProjectileDoFancyBoomerang : MonoBehaviour
	{

		public ProjectileDoFancyBoomerang()
		{
			isfancy = false;
			max_Distance = 1f; // max distance is recorded in seconds traveled from the origin position. 
			cur_Distance = 0;
			isyoyo = false;
		} 


		private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
			this.parentGun = this.Projectile.ProjectilePlayerOwner().CurrentGun;
			this.parentOwner = this.Projectile.ProjectilePlayerOwner();

			
			Projectile.baseData.range = 60f;
			BounceProjModifier bnc = Projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
			bnc.numberOfBounces = 6;

			HomingModifier home = Projectile.gameObject.GetOrAddComponent<HomingModifier>();
			home.HomingRadius = 1f;

			PierceProjModifier prc = Projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
			prc.penetratesBreakables = true;
			prc.penetration = 1000;
			saveSpeed = Projectile.baseData.speed;

			this.Projectile.angularVelocity = -800;
			initialangle = parentGun.CurrentAngle;
            if (isfancy)
            {
				parentOwner.IsGunLocked = true;
				EmergencyGunUnblocker block = this.parentOwner.gameObject.GetOrAddComponent<EmergencyGunUnblocker>();
				block.Projectile = this.Projectile;
                if(isyoyo)
				{
					block.isyoyo = true;
                }
			}

            if (isyoyo)
            {
				attach(Projectile);
            }

			if(isyoyo && this.parentOwner.PlayerHasActiveSynergy("All Tangled Up"))
            {
				
				bnc.bouncesTrackEnemies = true;
				bnc.TrackEnemyChance = 1;
				
				Projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(Projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemy));
			}
			
		}
		
        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(arg1 != null)
            {

                if (arg2.aiActor != null)
                {
					if (arg2 != null && arg2.healthHaver != null)
					{
						Func<AIActor, bool> isValid = (AIActor a) => a && a.HasBeenEngaged && a.healthHaver && a.healthHaver.IsVulnerable;
						AIActor closestToPosition = BraveUtility.GetClosestToPosition<AIActor>(arg2.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All), arg2.UnitCenter, isValid, new AIActor[]
						{
						arg2.aiActor
						});
						if (closestToPosition != null && closestToPosition.healthHaver.IsDead != true && closestToPosition.specRigidbody != arg2)
						{
							Vector2 vector = closestToPosition.Position - arg1.LastPosition;
							Projectile.SendInDirection(vector, true);
							cur_Distance = cur_Distance * .50f;

						}
						else // no enemies found Stop on cur
						{
							cur_Distance = max_Distance;
						}



					}
				}
				

			}
        }

		public List<AIActor> HitEnemies = new List<AIActor>
		{ };

		public void attach(Projectile arg1)
		{

			this.cable = arg1.gameObject.AddComponent<ArbitraryCableDrawer>();
			this.cable.Attach2Offset = new Vector2(0, 0);
			this.cable.Attach1Offset = arg1.sprite.WorldCenter - arg1.transform.position.XY();
			this.cable.Initialize(arg1.transform , parentOwner.CurrentGun.PrimaryHandAttachPoint);
			

		}
		private ArbitraryCableDrawer cable;

		


		private void Update()
		{
			if (Projectile != null)
			{
				if (cur_Distance < max_Distance)// has proj reached max distance?
				{
					cur_Distance = cur_Distance + Time.deltaTime;
				}
				else //return
				{
					if (isfancy)
					{
						if (held)//hang
						{

                            if (isyoyo && parentOwner.PlayerHasActiveSynergy("Walk The Dog"))
                            {
								Walkies();
                            }
                            else
                            {
								Projectile.baseData.speed = 0.001f;
								Projectile.UpdateSpeed();
							}

						}
						else
						{
							if (isyoyo)
							{
								StartCoroutine(doBurst());
							}
							doReturn();
                            
						}

					}
					else
					{
						doReturn();
					}

				}

				if (isfancy) // hang
				{
					if (held)
					{
						StartCoroutine(fancyHold());
					}

				}

			}


		}

        private void Walkies()
        {
			Projectile.baseData.speed = 4f;
			Vector2 vector = OMITBMathsAndLogicExtensions.DegreeToVector2(this.parentGun.CurrentAngle);
			this.Projectile.SendInDirection(vector, true, false);
			this.Projectile.UpdateSpeed();
		}

        private IEnumerator doBurst()
        {
			isyoyo = false;
			yield return new WaitForSeconds(.001f);
			float adjust = 0;
			AkSoundEngine.PostEvent("Play_WPN_beretta_shot_01", base.gameObject);
			if (parentOwner.PlayerHasActiveSynergy("Around The World"))
			{
				for (int i = 0; i < 7; i++)
				{
					int planet = UnityEngine.Random.Range(0, 8);
					Projectile projectile = ((Gun)ETGMod.Databases.Items[597]).DefaultModule.projectiles[planet];
					GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, this.Projectile.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.parentOwner.CurrentGun == null) ? 0f : initialangle + adjust), true);
					Projectile component = gameObject.GetComponent<Projectile>();
					bool flag2 = component != null;
					if (flag2)
					{
						component.Owner = this.parentOwner;
						component.Shooter = this.parentOwner.specRigidbody;
						component.baseData.damage = 8f;

						component.UpdateSpeed();
					}
					adjust = adjust + 60;

				}
			}
            else
            {
				for (int i = 0; i < 7; i++)
				{

					Projectile projectile = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
					GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, this.Projectile.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.parentOwner.CurrentGun == null) ? 0f : initialangle + adjust), true);
					Projectile component = gameObject.GetComponent<Projectile>();
					bool flag2 = component != null;
					if (flag2)
					{
						component.Owner = this.parentOwner;
						component.Shooter = this.parentOwner.specRigidbody;
						component.baseData.damage = 8f;

						component.UpdateSpeed();
					}
					adjust = adjust + 60;

				}
			}

			
		}

        private IEnumerator fancyHold()
		{
			yield return new WaitForSeconds(.01f);

			held = true;
			BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer((this.parentOwner as PlayerController).PlayerIDX);
			if (!instanceForPlayer.ActiveActions.ShootAction.IsPressed && Time.timeScale != 0 && !instanceForPlayer.ActiveActions.MapAction.IsPressed)
			{
				held = false;
			}

		}

		public void doReturn()
		{
			Projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Remove(Projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemy));
			this.Projectile.Speed = saveSpeed;
			this.Projectile.Speed *= 1.75f;
			
			Vector2 vector = parentOwner.CenterPosition - (Vector2)Projectile.LastPosition;
			this.Projectile.SendInDirection(vector, true, false);
			this.Projectile.UpdateSpeed();
			if (Vector2.Distance(parentOwner.CenterPosition, this.Projectile.LastPosition) < 1)
			{
				Projectile.hitEffects.suppressMidairDeathVfx = true;

				parentOwner.IsGunLocked = false;

				GoYo.RestartIdle = true;
				AkSoundEngine.PostEvent("Play_OBJ_item_pickup_01", base.gameObject);
				Projectile.DieInAir();
				
			}
			


			
		}

      

        public float initialangle;
		public bool isyoyo = false;
		private float cur_Distance;
		public float max_Distance;
		public bool isfancy = false;
		private bool held = true;
		private float saveSpeed;
		private float saveCooldownTimer;
		private Projectile Projectile;
		private PlayerController parentOwner;
		private Gun parentGun;

	}

}

