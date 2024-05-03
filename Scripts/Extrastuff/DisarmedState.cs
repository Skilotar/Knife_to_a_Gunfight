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
			if (AiActor.GetComponent<AIShooter>())
            {
				this.shooter = AiActor.GetComponent<AIShooter>();
				if (shooter.CurrentGun != null && AiActor.healthHaver.IsBoss == false)
				{

					
					shooter.ToggleGunAndHandRenderers(false, "Disarmed");
					shooter.customShootCooldownPeriod = 999;
					

				}
                else
                {

                }
			}
			
			
		}

		bool toggle = false;
		

		public void Update()
		{
            if (toggle)
            {
				
				
			}
		}


		public BulletScriptSource source;
		public AIShooter shooter;
		private AIActor AiActor;
	}
}