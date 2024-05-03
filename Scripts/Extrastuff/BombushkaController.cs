using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;

namespace Knives
{
	public class BombushkaController : MonoBehaviour
	{

		public BombushkaController()
		{


		}


		private void Start()
		{

			proxy = this.gameObject.GetComponent<ProximityMine>();
			StartCoroutine(onPreDet());

		}

		private IEnumerator onPreDet()
		{

			yield return new WaitForSeconds(proxy.explosionDelay - .005f);
			
			if (gameobjecttospawn != null && this.thisBombNumber <= 5)
			{
                try
                {
					GameObject bomb = UnityEngine.Object.Instantiate<GameObject>(gameobjecttospawn, this.gameObject.GetComponent<tk2dBaseSprite>().sprite.WorldCenter, Quaternion.identity);


					BombushkaController granny = bomb.GetOrAddComponent<BombushkaController>();
					granny.gameobjecttospawn = Bombushka.BuildPrefab();
					granny.thisBombNumber = this.thisBombNumber + 1;
					proxy.explosionData.damage = 11 - this.thisBombNumber;
					tk2dBaseSprite component4 = bomb.GetComponent<tk2dBaseSprite>();
					if (component4)
					{
						component4.PlaceAtPositionByAnchor(this.gameObject.GetComponent<tk2dBaseSprite>().sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
						float scale = 1 - this.thisBombNumber * .15f;
						component4.scale *= scale;
					}

					Vector2 vector3;

					if (DoesSynergy)
                    {
                        granny.DoesSynergy = true;
						granny.gameObject.GetComponent<tk2dBaseSprite>().sprite.color = UnityEngine.Color.red;
						vector3 = OMITBMathsAndLogicExtensions.GetVectorToNearestEnemy(this.gameObject.GetComponent<tk2dBaseSprite>().sprite.WorldCenter);
					}
                    else
                    {
						randomizedAngle = UnityEngine.Random.Range(0, 360);
						vector3 = OMITBMathsAndLogicExtensions.DegreeToVector2(randomizedAngle);
					}
					
					ForceDec = (float)Math.Sqrt(5 * this.thisBombNumber);
					DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(bomb, this.gameObject.GetComponent<tk2dBaseSprite>().sprite.WorldCenter, vector3, 7 - ForceDec, false, false, true, false);

				}
                catch(Exception e)
                {
					ETGModConsole.Log(e.ToString()); ;
				}
			}


		}

		private void Update()
		{

		}

		public bool DoesSynergy = false;
		public ProximityMine proxy;
		public GameObject gameobjecttospawn;
		public int thisBombNumber;
		private float randomizedAngle;
		private float ForceDec;


	}
}