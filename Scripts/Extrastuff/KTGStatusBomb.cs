using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;

namespace Knives
{
	public class KTGStatusBomb : MonoBehaviour
	{

		public KTGStatusBomb()
		{
			

		}


		private void Start()
		{

			proxy = this.gameObject.GetComponent<ProximityMine>();
			StartCoroutine(onPreDet());
           
		}
		private void OnDestroy()
		{
			if (radioactiveContamination)
			{
				if(DiedByExplosion == false)
                {
					PlayerController Owner = GameManager.Instance.GetPlayerClosestToPoint(centerPosition);
					LootEngine.SpawnItem(PickupObjectDatabase.GetById(RadioactiveCharm.ID).gameObject, Owner.CenterPosition, Vector2.zero, 1, true, true);
					RenderSettings.ambientLight = ExtendedColours.lime;
				}
				
			}
		}
		public bool DiedByExplosion = false;
		
		public Vector2 centerPosition;
		private IEnumerator onPreDet()
        {
			
			yield return new WaitForSeconds(proxy.explosionDelay - .01f);

			centerPosition = this.gameObject.GetComponent<tk2dSprite>().sprite.WorldCenter;
			PlayerController Owner = GameManager.Instance.GetPlayerClosestToPoint(centerPosition);

			if (radioactiveContamination)
			{
				DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.PoisonDef);
				goopManagerForGoopType.TimedAddGoopCircle(centerPosition, 3f, .5f, false);
				LootEngine.SpawnItem(PickupObjectDatabase.GetById(RadioactiveCharm.ID).gameObject, centerPosition, Vector2.zero, 1, true, true);
				RenderSettings.ambientLight = ExtendedColours.lime;
				DiedByExplosion = true;
			}

			List<AIActor> activeEnemies = Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
			
			foreach (AIActor aiactor in activeEnemies)
			{

				if(aiactor != null)
                {
					bool flag3 = Vector2.Distance(aiactor.CenterPosition, centerPosition) < radius && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && Owner != null;

                    if (flag3 && aiactor.healthHaver.IsDead != true)
                    {
                        if (DoesBlast)
                        {
                            BlastBlightedStatusController blast = aiactor.gameObject.GetOrAddComponent<BlastBlightedStatusController>();
                            blast.statused = true;
                        }

                        if (DoesHex)
                        {
                            HexStatusEffectController hex = aiactor.gameObject.GetOrAddComponent<HexStatusEffectController>();
                            hex.statused = true;
                        }

						if (hitHard)
						{

							aiactor.healthHaver.ApplyDamage(50, Vector2.zero, "Big Angry",CoreDamageTypes.None,DamageCategory.Normal,true,null,true);

						}



					}



				}


			}
            

		}

		private void Update()
		{
			
		}


		public ProximityMine proxy;
		public float radius = 3;
		public bool hitHard = false;
		public bool DoesHex = false;
		public bool DoesBlast = false;
		public bool radioactiveContamination = false;
		
	}
}