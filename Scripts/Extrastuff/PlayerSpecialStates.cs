using System;
using ItemAPI;
using UnityEngine;
using System.Collections;

namespace Knives
{
	public class PlayerSpecialStates : MonoBehaviour
	{

		public PlayerSpecialStates()
		{
			theatrefreebegotten = false;


          
		}
       

		private void Start()
		{
			this.player = base.GetComponent<PlayerController>();
           
        }
    

        private void Update()
		{

		}



		
		public bool theatrefreebegotten;
		public int AmmochemyFlipper = 0;


		private PlayerController player;
	}
}
