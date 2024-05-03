using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using System.Collections.Generic;


namespace Knives
{
	public class HighNoonTracker : MonoBehaviour
	{
       
        public HighNoonTracker()
		{

		}


		private void Start()
		{
			this.aiactor = base.GetComponent<AIActor>();
            this.aiactor.healthHaver.OnDamaged += HealthHaver_OnDamaged;
            this.aiactor.healthHaver.OnDeath += HealthHaver_OnDeath;
			newPass.SetFloat("_Radius", Mathf.Lerp(5f, .5f, (8 + currentHighNoonStacks) / this.aiactor.healthHaver.GetCurrentHealth()));
			
		}

        private void HealthHaver_OnDeath(Vector2 obj)
        {
			Pixelator.Instance.DeregisterAdditionalRenderPass(newPass);
			currentHighNoonStacks = 0;
			aiactor.DeregisterOverrideColor("HighNoon");
		}

        private void HealthHaver_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
			currentHighNoonStacks = 0;
        }

        private void Update()
		{

			if (aiactor.healthHaver != null)
			{
				if (player != null)
				{
					if (aiactor.healthHaver.GetCurrentHealth() < (8 + currentHighNoonStacks))
					{

						if (player.CurrentGun.GetChargeFraction() == 1)
						{
							if (whip == false)
							{
								AkSoundEngine.PostEvent("Play_Timekeep_whip", base.gameObject);
								aiactor.RegisterOverrideColor(UnityEngine.Color.red, "HighNoon");
								whip = true;
							}
							if(ring == true)
                            {
								newPass.SetFloat("_Radius", Mathf.Lerp(5f, 0f, (8 + currentHighNoonStacks) / this.aiactor.healthHaver.GetCurrentHealth()));
								newPass.SetVector("_WorldCenter", aiactor.CenterPosition);
							}

						}
						else
						{
							tk2dSpriteAnimationClip clip = player.CurrentGun.spriteAnimator.GetClipByName("TimeKeeper_fire");
							if (player.CurrentGun.spriteAnimator.CurrentClip != clip)
							{
								Pixelator.Instance.DeregisterAdditionalRenderPass(newPass);
								currentHighNoonStacks = 0;
								aiactor.DeregisterOverrideColor("HighNoon");
								whip = false;
							}
							

						}
					}
					else
					{
						if (player.CurrentGun.GetChargeFraction() == 1)
						{
							if(ring == false)
                            {
								newPass.SetFloat("_Radius", Mathf.Lerp(5f, 0f, (8 + currentHighNoonStacks) / this.aiactor.healthHaver.GetCurrentHealth()));
								newPass.SetVector("_WorldCenter", aiactor.CenterPosition);
								Pixelator.Instance.RegisterAdditionalRenderPass(newPass);
								ring = true;
							}
							newPass.SetFloat("_Radius", Mathf.Lerp(5f, 0f, (8 + currentHighNoonStacks) / this.aiactor.healthHaver.GetCurrentHealth()));
							newPass.SetVector("_WorldCenter", aiactor.CenterPosition);
						}
						else
						{
							if(ring == true)
                            {
								Pixelator.Instance.DeregisterAdditionalRenderPass(newPass);
								ring = false;
							}
							
						}
					}
				}
            }

		}

		public bool ring = false;
		public Material newPass = new Material(Shader.Find("Brave/Effects/PartialDesaturationEffect"));
		public PlayerController player;
		private AIActor aiactor;
		public bool whip;
		public int currentHighNoonStacks;
		public float damagemod = 1;
	}


}
