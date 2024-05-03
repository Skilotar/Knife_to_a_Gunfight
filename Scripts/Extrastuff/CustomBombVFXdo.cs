using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;

namespace Knives
{
	public class PlayBombCustomVFX : MonoBehaviour
	{

		public PlayBombCustomVFX()
		{
			VFX = null;

		}


		private void Start()
		{

			proxy = this.gameObject.GetComponent<ProximityMine>();
			StartCoroutine(onPreDet());

		}
		public Vector2 centerPosition;
		private IEnumerator onPreDet()
		{

			yield return new WaitForSeconds(proxy.explosionDelay - .005f);
			if(VFX != null)
            {
				if (gameObject.GetComponent<ProximityMine>() != null)
				{
					GameObject BAB = SpawnManager.SpawnVFX(VFX, gameObject.transform.localPosition, Quaternion.identity);
					BAB.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(gameObject.GetComponent<tk2dBaseSprite>().WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
					BAB.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
					if(VFX = EasyVFXDatabase.BAB)
                    {
						AkSoundEngine.PostEvent("Play_BOSS_spacebaby_explode_01", base.gameObject);
						RenderSettings.ambientLight = new Color(1, 0, .22f);
					}
					
				}
			}
			

		}

		private void Update()
		{

		}

		public GameObject VFX;
		public ProximityMine proxy;


	}
}
