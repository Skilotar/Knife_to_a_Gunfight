using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;

namespace Knives
{
	public class TrackHighSpeed : MonoBehaviour
	{

		public TrackHighSpeed()
		{


		}


		private void Start()
		{

			player = base.GetComponent<PlayerController>();


		}

		private void Update() // keeps track of the last know Highest speed of the player resetting every .15f seconds
		{
			if(player.Velocity.magnitude > 0)
            {
				if (player.Velocity.magnitude > HighSpeed) HighSpeed = player.Velocity.magnitude;

				if(HighSpeed != 0)
                {
					DecayTimer -= Time.deltaTime;
					if(DecayTimer <= 0)
                    {
						DecayTimer = decay;
                    }
                }
            }
            else
            {
				HighSpeed = 0;
            }
		}

		public float decay = .15f;
		public float DecayTimer = .15f;
		public float HighSpeed;
		public PlayerController player;
	}
}
