using System;
using ItemAPI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Knives
{
	public class GunSpecialStates : MonoBehaviour
	{

		public GunSpecialStates()
		{
			DoesMacRage = false;
			DoesCountBlocks = false;
			RageCount = 0;
			ReturnHitList = false;
		}


		private void Start()
		{
			this.gun = base.GetComponent<Gun>();

		}


		private void Update()
		{

		}

		public bool DoesMacRage;
		public int RageCount;
		public bool DoesCountBlocks;
		public int successfullBlocks;
		public bool ReturnHitList;
		public List<Projectile> returnList = new List<Projectile>();

		private Gun gun;

        
    }
}

