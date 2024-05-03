using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;

namespace Knives
{
	public class ThermBombDoer : MonoBehaviour
	{

		public ThermBombDoer()
		{


		}


		private void Start()
		{

			proxy = this.gameObject.GetComponent<ProximityMine>();
			
			StartCoroutine(onPreDet());

		}
		public Vector2 centerPosition;
		private IEnumerator onPreDet()
		{

			yield return new WaitForSeconds(proxy.explosionDelay - .01f);

			centerPosition = this.gameObject.GetComponent<tk2dSprite>().sprite.WorldCenter;

			DeadlyDeadlyGoopManager goop = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(PickupObjectDatabase.GetById(242).GetComponent<DirectionalAttackActiveItem>().goopDefinition);

			gameObject.transform.rotation = Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : rotate);

			Vector3 pointinspace = gameObject.transform.TransformPoint(0,6, 0);
			Vector3 Negpointinspace = gameObject.transform.TransformPoint(0, -6, 0);


			goop.TimedAddGoopLine(centerPosition, pointinspace, 1.2f, .5f);
			goop.TimedAddGoopLine(centerPosition, Negpointinspace, 1.2f, .5f);

		}

		private void Update()
		{

		}




		public ProximityMine proxy;
		public PlayerController player;
		public float rotate;

	}
}