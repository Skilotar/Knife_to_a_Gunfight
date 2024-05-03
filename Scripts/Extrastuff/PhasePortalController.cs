using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ItemAPI;
using UnityEngine;
using SaveAPI;

namespace Knives
{
    class portalController : MonoBehaviour
	{

        public portalController()
        {


        }


		private void Start()
		{

			player = base.GetComponent<PlayerController>();


		}

        public bool isteleporting = false;
        private void Update()
		{
            if (startpoint != null && endpoint != null) // Portals exist
            {
                
                if (Vector2.Distance(player.transform.position, startpoint) < 1.2)//close enough to teleport
                {
                    if (!isteleporting)
                    {
                        TeleportPlayerToCursorPosition.StartTeleport(player, endpoint);
                        isteleporting = true;
                    }

                }
                else
                {
                    if (isteleporting) // has teleported away
                    {
                        isteleporting = false;
                    }
                }


            }
            if(player.CurrentGun.PickupObjectId != PhaseBlade.ID)
            {
                if(PhaseBlade.reticle != null)
                {
                    
                    UnityEngine.GameObject.Destroy(PhaseBlade.reticle);
                }
               
            }

        }

		public Vector3 startpoint;
		public Vector3 endpoint;

		public PlayerController player;
	}
}
