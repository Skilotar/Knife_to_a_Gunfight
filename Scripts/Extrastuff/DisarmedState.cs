using System;
using System.Collections;
using ItemAPI;
using UnityEngine;

namespace Knives
{
	public class DisarmedStatus : MonoBehaviour
	{

		public DisarmedStatus()
		{
			
			// failed
		}
		
		public void Start()
		{
			this.AiActor = base.GetComponent<AIActor>();
			DisarmedStatus.fleeData = new FleePlayerData();
			DisarmedStatus.fleeData.Player = player;
			DisarmedStatus.fleeData.StartDistance = 20f;
          
		}

        

        public void Update()
		{

			AiActor.aiShooter.customShootCooldownPeriod = 999999;
			AiActor.CanTargetPlayers = false;
			AiActor.CanTargetEnemies = false;
			AiActor.behaviorSpeculator.FleePlayerData = DisarmedStatus.fleeData;
			FleePlayerData fleePlayerData = new FleePlayerData();


		}

		private static FleePlayerData fleeData;
		public PlayerController player;
		public BulletScriptSource source;
		public AIShooter shooter;
		private AIActor AiActor;
	}
}