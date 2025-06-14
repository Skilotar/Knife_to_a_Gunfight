using Dungeonator;
using Mono.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Knives
{
	// Token: 0x02000339 RID: 825
	public class PlayerGoUp : MonoBehaviour
	{
		// Token: 0x060010D0 RID: 4304 RVA: 0x000DD374 File Offset: 0x000DB574
		public PlayerGoUp()
		{
			
			
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x000DD39B File Offset: 0x000DB59B
		private void OnDestroy()
		{
			
		}

	
		private void Start()
		{
			
			player = base.GetComponent<PlayerController>();
			origiHightOffGround = player.sprite.HeightOffGround;
			//ETGModConsole.Log(origiHightOffGround);
			prefabToAttachToPlayer = ((PickupObjectDatabase.GetById(106).gameObject.GetComponent<JetpackItem>().prefabToAttachToPlayer));
			

		}

		private void Update()
		{
			bool flag = this.player;
			if (flag)
			{
                if(Up == true)// go up and hold
                {
                    if (Airborn == false)
                    {
						Airborn = true;
						Effect = player.PlayEffectOnActor(HellFire.AirPillar, new Vector3(0, 0, 0), true);
						AkSoundEngine.PostEvent("Play_OBJ_jetpack_start_01", base.gameObject);
						this.instanceJetpack = player.RegisterAttachedObject(this.prefabToAttachToPlayer, "jetpack", 0f);
						this.instanceJetpackSprite = this.instanceJetpack.GetComponent<tk2dSprite>();
						GlobalSparksDoer.DoRadialParticleBurst(20, player.specRigidbody.UnitBottomLeft, player.specRigidbody.UnitBottomRight, 0, 50, 20, null, null, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
					}

					if (t <= 1)
                    {
						
						t += BraveTime.DeltaTime * 2.75f;
                    }
                }
				else // go down and stop
				{
					
					if (t > 0)
					{
						t -= BraveTime.DeltaTime * 1.2f;
					}
				}
				if(t > 0)
                {

					player.SetIsFlying(true, "Valk", false, false);
					player.ToggleGunRenderers(false, "Valk");
					player.ToggleHandRenderers(false, "Valk");
					player.CurrentStoneGunTimer = 7;
					
					player.healthHaver.IsVulnerable = false;
					currY = Mathf.Lerp(0, Height, t);
					selected = Math.Max(currY, 0);

					player.sprite.transform.localPosition = player.sprite.transform.localPosition.WithY(selected);
					player.sprite.HeightOffGround = selected;
					player.sprite.UpdateZDepth();

                    if (!doingSlashy)
                    {
						StartCoroutine(doSlash());
						StartCoroutine(doTable());
						
                    }
				}
                else
                {
					
					if(Airborn == true)
                    {

						AkSoundEngine.PostEvent("Stop_OBJ_jetpack_loop_01", base.gameObject);
						player.DeregisterAttachedObject(this.instanceJetpack, true);
						
						player.SetIsFlying(false, "Valk", false, false);
						player.ToggleGunRenderers(true, "Valk");
						player.ToggleHandRenderers(true, "Valk");
						
						player.healthHaver.IsVulnerable = true;
						Airborn = false;
                        player.CurrentStoneGunTimer = 0;
						
						player.sprite.transform.localPosition = player.sprite.transform.localPosition.WithY(0);
						player.sprite.HeightOffGround = -.5f;
						player.sprite.UpdateZDepth();

						UnityEngine.GameObject.Destroy(instanceJetpack);
						UnityEngine.GameObject.Destroy(instanceJetpackSprite);
						UnityEngine.GameObject.Destroy(Effect);
						UnityEngine.GameObject.Destroy(this);
					}

				}


			}
		}

       
        private IEnumerator doTable()
        {
			if (player.CurrentRoom != null)
			{
				IPlayerInteractable nearestInteractable = player.CurrentRoom.GetNearestInteractable(player.specRigidbody.UnitCenter, 3.5f, player);
				if (nearestInteractable != null)
				{

					if (nearestInteractable is FlippableCover)
					{
						FlippableCover Table = nearestInteractable as FlippableCover;
						if (!Table.IsFlipped)
						{
							Table.Flip(player.specRigidbody);
							Vector2 vel = Table.specRigidbody.UnitCenter - player.specRigidbody.UnitCenter;
							Table.specRigidbody.Velocity = BraveMathCollege.DegreesToVector(vel.ToAngle()).normalized * 20;
							yield return new WaitForSeconds(1f);
							Table.specRigidbody.Velocity = new Vector2(0, 0);
						}
                        
					}
				}
			}
			yield return new WaitForSeconds(.25f);
			yield return null;
		}
		

        private IEnumerator doSlash()
        {
			doingSlashy = true;

			yield return new WaitForSeconds(.25f);
			Projectile proj = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, player.specRigidbody.UnitBottomCenter, player);
			ProjectileSlashingBehaviour stab = proj.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
			stab.SlashRange = 3;
			stab.SlashDimensions = 180;
			stab.SlashDamageUsesBaseProjectileDamage = false;
			stab.slashKnockback = 20;
			stab.playerKnockback = 0;
			stab.SlashDamage = 2.5f;
			stab.soundToPlay = "";
			stab.SlashVFX.type = VFXPoolType.None;

			Exploder.DoRadialPush(player.specRigidbody.UnitCenter, 20, 5f);



			doingSlashy = false;
		}

        public float selected;
		public float currY;
		public bool Airborn = false;
		private PlayerController player;
		public float Height = 6;
		public bool Up;
		public float t = 0;
		public float origiHightOffGround;
		private GameObject instanceJetpack;
		private GameObject Effect;
		private tk2dSprite instanceJetpackSprite;
		public GameObject prefabToAttachToPlayer;
        private bool doingSlashy;
    }
}