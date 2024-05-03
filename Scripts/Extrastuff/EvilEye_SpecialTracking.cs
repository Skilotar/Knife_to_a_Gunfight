using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using System.Collections.Generic;

namespace Knives
{
	public class EvilEyeSpecialTracking : MonoBehaviour
	{

		public EvilEyeSpecialTracking()
		{
			
		}


		private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
			
			
		}

		private void Update()
		{
			if (Projectile != null)
			{
				if(targetActor.Count >= 1)
                {
					if (targetActor[targetActor.Count-1] != null && Projectile.ElapsedTime >= .1f)
					{
						if (targetActor[targetActor.Count - 1].healthHaver.IsDead != true)
						{
							if (Vector2.Distance(Projectile.transform.position, targetActor[targetActor.Count - 1].CenterPosition) >= 2f)
							{
								Vector2 vector = targetActor[targetActor.Count - 1].CenterPosition - (Vector2)Projectile.transform.position;
								Projectile.SendInDirection(vector, true, true);
							}
						}
					}
				}
				
			}

		}

		public List<AIActor> targetActor = new List<AIActor>
		{ };

		private Projectile Projectile;
		

	}

}

