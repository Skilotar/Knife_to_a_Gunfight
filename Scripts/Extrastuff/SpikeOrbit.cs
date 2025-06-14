using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;

namespace Knives
{
	public class SpikeOrbit : MonoBehaviour
	{

		public SpikeOrbit()
		{
			min = 2f;
			max = 2.7f;
		}

		private void Start()
		{
			obj = base.gameObject;
			Orbit = base.gameObject.GetComponent<PlayerOrbital>();
		}

		private void Update()
		{
			if(Orbit != null)
            {
				Orbit.orbitRadius = Mathf.Lerp(min, max, LerpPoint / 200);
				if (LerpPoint >= 200) up = false;
				if (LerpPoint <= 0) up = true;
				if (up) LerpPoint++;
				else LerpPoint--;
			}
		}

		bool up = true;
		GameObject obj;
		PlayerOrbital Orbit;
		float min;
		float max;
		public float LerpPoint = 0;
	}

}