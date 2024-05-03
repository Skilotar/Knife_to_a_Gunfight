using System;
using System.Collections.Generic;
using ItemAPI;
using UnityEngine;

namespace Knives
{
	public class FragileGuonController : MonoBehaviour
	{

		public FragileGuonController()
		{


		}

		public int clayHP = 4;
		private void Start()
		{
			this.Orbital = base.GetComponent<GameObject>();
			this.playerOrbital = base.GetComponent<PlayerOrbital>();
			this.parentOwner = playerOrbital.Owner;
			this.specbod = base.GetComponent<SpeculativeRigidbody>();
			specbod.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specbod.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));
			
		}

        private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
			if (otherRigidbody != null && myRigidbody != null)
			{
				
				if (otherRigidbody.projectile)
				{
					if (clayHP <= 0)
					{
						shieldPop(myRigidbody);
						UnityEngine.GameObject.Destroy(myRigidbody.gameObject);
						AkSoundEngine.PostEvent("Play_OBJ_pot_shatter_01", base.gameObject);
						AkSoundEngine.PostEvent("Play_OBJ_pot_shatter_01", base.gameObject);
					}
					else
					{
						
						clayHP--;

					}
					if (clayHP == 2)//orange
					{
						myRigidbody.sprite.color = new Color(.70f, .40f, .24f);

					}
					if (clayHP == 1)//red
					{
						myRigidbody.sprite.color = new Color(.33f, .07f, .04f);

					}
				}

			}

        }
		
		private void Update()
		{
			
		}
		
		public GameObject Orbital;
		public PlayerOrbital playerOrbital;
		public SpeculativeRigidbody specbod;
		private PlayerController parentOwner;

		public void shieldPop(SpeculativeRigidbody body)
        {
			PlayerController owner = parentOwner;
			GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
			AkSoundEngine.PostEvent("Play_OBJ_silenceblank_small_01", base.gameObject);
			GameObject gameObject = new GameObject("silencer");
			SilencerInstance silencerInstance = gameObject.AddComponent<SilencerInstance>();
			float additionalTimeAtMaxRadius = 0.25f;
			silencerInstance.TriggerSilencer(body.UnitCenter, 25f, 3.5f, silencerVFX, 0f, 3f, 3f, 3f, 250f, 3.5f, additionalTimeAtMaxRadius, owner, false, false);
		}

	
	}
}
