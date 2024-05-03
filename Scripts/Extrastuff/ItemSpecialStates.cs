using System;
using ItemAPI;
using UnityEngine;
using System.Collections;

namespace Knives
{
	public class ItemSpecialStates : MonoBehaviour
	{

		public ItemSpecialStates()
		{

			MarkedMasterRound = false;
		}


		private void Start()
		{
			this.player = base.GetComponent<PlayerController>();

		}


		private void Update()
		{

		}




		public bool MarkedMasterRound;



		private PlayerController player;
	}
}
