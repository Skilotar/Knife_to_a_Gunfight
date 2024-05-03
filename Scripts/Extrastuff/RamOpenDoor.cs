using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using Dungeonator;

namespace Knives
{
	public class RamOpenDoor : MonoBehaviour
	{

		public RamOpenDoor()
		{
			Life = 3;
			
		}


		private void Start()
		{
			// Make sure to appy this to the parent object and not a single part of the door. 
			this.Door = base.gameObject.GetComponent<DungeonDoorController>();
		}


		private void Update()
		{
			

		}

		public void DoHealthUpdate(RoomHandler room)
        {
			Life = Life - 1;
			ETGModConsole.Log(Life.ToString());
			if (Door.IsSealed)
            {
				if (Life == 2)
				{
					Door.sprite.color = new Color(.70f, .40f, .24f);
				}
				if (Life == 1)
				{
					Door.sprite.color = new Color(.33f, .07f, .04f);
				}
				if (Life <= 0)
				{
					Door.DoUnseal(room);
					Door.Open();
					
				}
			}
            else
            {
				ETGModConsole.Log("unsealed");
				Door.Open();
			}


		}
		

		public int Life = 3;
		public DungeonDoorController Door;
	}
}
