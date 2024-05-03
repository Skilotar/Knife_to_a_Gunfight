using System;
using System.Collections;
using UnityEngine;

namespace Knives
{
	// Token: 0x02000052 RID: 82
	public class KnivesWeatherController : BraveBehaviour
	{   /*
		// Token: 0x060002F5 RID: 757 RVA: 0x0001CB1C File Offset: 0x0001AD1C
		public KnivesWeatherController()
		{
			this.isActive = false;
			this.isSecretFloor = true;
			this.MinTimeBetweenLightningStrikes = 5f;
			this.MaxTimeBetweenLightningStrikes = 10f;
			this.AmbientBoost = 1f;
			this.RainIntensity = 250f;
			this.useCustomIntensity = true;
			this.enableLightning = false;
			this.isLocalToRoom = false;
			this.ThunderShake = new ScreenShakeSettings
			{
				magnitude = 0.2f,
				speed = 3f,
				time = 0f,
				falloff = 0.5f,
				direction = new Vector2(1f, 0f),
				vibrationType = ScreenShakeSettings.VibrationType.Auto,
				simpleVibrationTime = Vibration.Time.Normal,
				simpleVibrationStrength = Vibration.Strength.Medium
			};
			this.m_lightningTimer = UnityEngine.Random.Range(this.MinTimeBetweenLightningStrikes, this.MaxTimeBetweenLightningStrikes);
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x0001CBFC File Offset: 0x0001ADFC
		private void Start()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(DungeonDatabase.GetOrLoadByName("finalscenario_guide").PatternSettings.flows[0].AllNodes[0].overrideExactRoom.placedObjects[0].nonenemyBehaviour.gameObject.transform.Find("Rain").gameObject);
			gameObject.name = "NoirRain";
			this.m_StormController = gameObject.GetComponent<ThunderstormController>();
			ParticleSystem component = this.m_StormController.RainSystemTransform.GetComponent<ParticleSystem>();
			bool flag = this.useCustomIntensity;
			if (flag)
			{
				BraveUtility.SetEmissionRate(component, this.RainIntensity);
			}
			this.m_StormController.DecayVertical = this.isLocalToRoom;
			this.m_StormController.DoLighting = false;
			this.LightningRenderers = this.m_StormController.LightningRenderers;
			gameObject.transform.parent = base.gameObject.transform;
			this.isActive = true;
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x0001CCF4 File Offset: 0x0001AEF4
		private void Update()
		{
			bool isLoadingLevel = GameManager.Instance.IsLoadingLevel;
			if (!isLoadingLevel)
			{
				bool flag = this.isSecretFloor;
				if (flag)
				{
					PlayerController primaryPlayer = GameManager.Instance.PrimaryPlayer;
					bool flag2 = primaryPlayer;
					if (flag2)
					{
						this.CheckForWeatherFX(primaryPlayer, this.RainIntensity);
					}
				}
				bool flag3 = !this.isActive;
				if (!flag3)
				{
					bool flag4 = this.enableLightning;
					if (flag4)
					{
						this.m_lightningTimer -= ((!GameManager.IsBossIntro) ? BraveTime.DeltaTime : GameManager.INVARIANT_DELTA_TIME);
						bool flag5 = this.m_lightningTimer <= 0f && this.isActive;
						if (flag5)
						{
							base.StartCoroutine(this.DoLightningStrike());
							bool flag6 = this.LightningRenderers != null;
							if (flag6)
							{
								for (int i = 0; i < this.LightningRenderers.Length; i++)
								{
									base.StartCoroutine(this.ProcessLightningRenderer(this.LightningRenderers[i]));
								}
							}
							base.StartCoroutine(this.HandleLightningAmbientBoost());
							this.m_lightningTimer = UnityEngine.Random.Range(this.MinTimeBetweenLightningStrikes, this.MaxTimeBetweenLightningStrikes);
						}
					}
				}
			}
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x0001CE24 File Offset: 0x0001B024
		private void CheckForWeatherFX(PlayerController player, float RainIntensity)
		{
			try
			{
				GameManager.Instance.StartCoroutine(this.ToggleRainFX(player, RainIntensity));
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.ToString(), false);
			}
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0001CE6C File Offset: 0x0001B06C
		private IEnumerator ToggleRainFX(PlayerController player, float cachedRate)
		{
			bool flag = !this.m_StormController;
			if (flag)
			{
				yield break;
			}
			bool Active = true;
			bool flag2 = player.CurrentRoom.IsShop | player.CurrentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS;
			if (flag2)
			{
				Active = false;
			}
			yield return null;
			bool flag3 = !Active;
			if (flag3)
			{
				bool flag4 = !this.m_StormController.enabled && !this.isActive;
				if (flag4)
				{
					yield break;
				}
				AkSoundEngine.PostEvent("Stop_ENV_rain_loop_01", this.m_StormController.gameObject);
				BraveUtility.SetEmissionRate(this.m_StormController.RainSystemTransform.GetComponent<ParticleSystem>(), 0f);
				yield return new WaitForSeconds(2f);
				this.m_StormController.enabled = false;
				this.isActive = false;
				yield return null;
				yield break;
			}
			else
			{
				bool flag5 = this.m_StormController.enabled && this.isActive;
				if (flag5)
				{
					yield break;
				}
				BraveUtility.SetEmissionRate(this.m_StormController.RainSystemTransform.GetComponent<ParticleSystem>(), cachedRate);
				this.m_StormController.enabled = true;
				this.isActive = true;
				yield break;
			}
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0001CE89 File Offset: 0x0001B089
		public void ForceStopRain(bool DestroySelf, string Name = "NoirRain")
		{
			GameManager.Instance.StartCoroutine(this.DisableRain(DestroySelf, Name));
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0001CE9F File Offset: 0x0001B09F
		private IEnumerator DisableRain(bool DestroySelf, string Name)
		{
			bool flag = !this.m_StormController;
			if (flag)
			{
				yield break;
			}
			bool flag2 = !this.m_StormController.enabled && !this.isActive;
			if (flag2)
			{
				yield break;
			}
			AkSoundEngine.PostEvent("Stop_ENV_rain_loop_01", this.m_StormController.gameObject);
			BraveUtility.SetEmissionRate(this.m_StormController.RainSystemTransform.GetComponent<ParticleSystem>(), 0f);
			this.m_StormController.enabled = false;
			this.isActive = false;
			if (DestroySelf)
			{
				KnivesWeatherController expandWeatherController = GameManager.Instance.Dungeon.gameObject.GetComponent<KnivesWeatherController>();
				bool flag3 = expandWeatherController;
				if (flag3)
				{
					bool flag4 = expandWeatherController.name == Name;
					if (flag4)
					{
						UnityEngine.Object.Destroy(expandWeatherController);
					}
				}
				expandWeatherController = null;
			}
			yield return null;
			yield break;
		}

		// Token: 0x060002FC RID: 764 RVA: 0x0001CEBC File Offset: 0x0001B0BC
		protected IEnumerator HandleLightningAmbientBoost()
		{
			Color cachedAmbient = RenderSettings.ambientLight;
			Color modAmbient = new Color(cachedAmbient.r + this.AmbientBoost, cachedAmbient.g + this.AmbientBoost, cachedAmbient.b + this.AmbientBoost);
			GameManager.Instance.Dungeon.OverrideAmbientLight = true;
			int num;
			for (int i = 0; i < 2; i = num + 1)
			{
				float elapsed = 0f;
				float duration = 0.15f * (float)(i + 1);
				while (elapsed < duration)
				{
					elapsed += GameManager.INVARIANT_DELTA_TIME;
					float t = elapsed / duration;
					GameManager.Instance.Dungeon.OverrideAmbientColor = Color.Lerp(modAmbient, cachedAmbient, t);
					yield return null;
				}
				num = i;
			}
			yield return null;
			GameManager.Instance.Dungeon.OverrideAmbientLight = false;
			yield break;
		}

		// Token: 0x060002FD RID: 765 RVA: 0x0001CECB File Offset: 0x0001B0CB
		protected IEnumerator ProcessLightningRenderer(Renderer target)
		{
			target.enabled = true;
			yield return base.StartCoroutine(this.InvariantWait(0.05f));
			target.enabled = false;
			yield return base.StartCoroutine(this.InvariantWait(0.1f));
			target.enabled = true;
			yield return base.StartCoroutine(this.InvariantWait(0.1f));
			target.enabled = false;
			yield break;
		}

		// Token: 0x060002FE RID: 766 RVA: 0x0001CEE1 File Offset: 0x0001B0E1
		protected IEnumerator InvariantWait(float duration)
		{
			float elapsed = 0f;
			while (elapsed < duration)
			{
				elapsed += GameManager.INVARIANT_DELTA_TIME;
				yield return null;
			}
			yield break;
		}

		// Token: 0x060002FF RID: 767 RVA: 0x0001CEF7 File Offset: 0x0001B0F7
		protected IEnumerator DoLightningStrike()
		{
			AkSoundEngine.PostEvent("Play_ENV_thunder_flash_01", GameManager.Instance.PrimaryPlayer.gameObject);
			PlatformInterface.SetAlienFXColor(new Color(1f, 1f, 1f, 1f), 0.25f);
			yield return new WaitForSeconds(0.25f);
			GameManager.Instance.MainCameraController.DoScreenShake(this.ThunderShake, null, false);
			yield break;
		}

		// Token: 0x06000300 RID: 768 RVA: 0x0001CF06 File Offset: 0x0001B106
		public override void  OnDestroy()
		{
			base.OnDestroy();
		}

		// Token: 0x0400022A RID: 554
		public bool isActive;

		// Token: 0x0400022B RID: 555
		public bool isSecretFloor;

		// Token: 0x0400022C RID: 556
		public float MinTimeBetweenLightningStrikes;

		// Token: 0x0400022D RID: 557
		public float MaxTimeBetweenLightningStrikes;

		// Token: 0x0400022E RID: 558
		public float AmbientBoost;

		// Token: 0x0400022F RID: 559
		public float RainIntensity;

		// Token: 0x04000230 RID: 560
		public bool useCustomIntensity;

		// Token: 0x04000231 RID: 561
		public bool enableLightning;

		// Token: 0x04000232 RID: 562
		public bool isLocalToRoom;

		// Token: 0x04000233 RID: 563
		public ScreenShakeSettings ThunderShake;

		// Token: 0x04000234 RID: 564
		public Renderer[] LightningRenderers;

		// Token: 0x04000235 RID: 565
		private ThunderstormController m_StormController;

		// Token: 0x04000236 RID: 566
		private float m_lightningTimer;

		*/
	}
}

