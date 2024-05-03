using System;
using UnityEngine;

namespace Knives
{
	// Token: 0x02000031 RID: 49
	public class CustomDarknessHandler : MonoBehaviour
	{
		// Token: 0x0600013D RID: 317 RVA: 0x00010980 File Offset: 0x0000EB80
		private void Start()
		{
			GameObject gameObject = LoadHelper.LoadAssetFromAnywhere<GameObject>("_ChallengeManager");
			CustomDarknessHandler.DarknessEffectShader = (gameObject.GetComponent<ChallengeManager>().PossibleChallenges[5].challenge as DarknessChallengeModifier).DarknessEffectShader;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x000109C0 File Offset: 0x0000EBC0
		private bool ReturnShouldBeDark()
		{
			return CustomDarknessHandler.shouldBeDark.Value && !CustomDarknessHandler.shouldBeLightOverride.Value;
		}

		// Token: 0x0600013F RID: 319 RVA: 0x000109FC File Offset: 0x0000EBFC
		private void Update()
		{
			bool flag = this.ReturnShouldBeDark() && !CustomDarknessHandler.isDark;
			if (flag)
			{
				this.EnableDarkness();
			}
			else
			{
				bool flag2 = !this.ReturnShouldBeDark() && CustomDarknessHandler.isDark;
				if (flag2)
				{
					this.DisableDarkness();
				}
			}
			bool flag3 = CustomDarknessHandler.isDark;
			if (flag3)
			{
				bool flag4 = Pixelator.Instance.AdditionalCoreStackRenderPass == null;
				if (flag4)
				{
					CustomDarknessHandler.m_material = new Material(CustomDarknessHandler.DarknessEffectShader);
					Pixelator.Instance.AdditionalCoreStackRenderPass = CustomDarknessHandler.m_material;
				}
				bool flag5 = CustomDarknessHandler.m_material != null;
				if (flag5)
				{
					float num = GameManager.Instance.PrimaryPlayer.FacingDirection;
					bool flag6 = num > 270f;
					if (flag6)
					{
						num -= 360f;
					}
					bool flag7 = num < -270f;
					if (flag7)
					{
						num += 360f;
					}
					CustomDarknessHandler.m_material.SetFloat("_ConeAngle", this.FlashlightAngle);
					Vector4 centerPointInScreenUV = CustomDarknessHandler.GetCenterPointInScreenUV(GameManager.Instance.PrimaryPlayer.CenterPosition);
					centerPointInScreenUV.z = num;
					Vector4 vector = centerPointInScreenUV;
					bool flag8 = GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER;
					if (flag8)
					{
						num = GameManager.Instance.SecondaryPlayer.FacingDirection;
						bool flag9 = num > 270f;
						if (flag9)
						{
							num -= 360f;
						}
						bool flag10 = num < -270f;
						if (flag10)
						{
							num += 360f;
						}
						vector = CustomDarknessHandler.GetCenterPointInScreenUV(GameManager.Instance.SecondaryPlayer.CenterPosition);
						vector.z = num;
					}
					CustomDarknessHandler.m_material.SetVector("_Player1ScreenPosition", centerPointInScreenUV);
					CustomDarknessHandler.m_material.SetVector("_Player2ScreenPosition", vector);
				}
			}
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00010BC0 File Offset: 0x0000EDC0
		private static Vector4 GetCenterPointInScreenUV(Vector2 centerPoint)
		{
			Vector3 vector = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp(0f));
			return new Vector4(vector.x, vector.y, 0f, 0f);
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00010C10 File Offset: 0x0000EE10
		private void EnableDarkness()
		{
			bool flag = CustomDarknessHandler.isDark;
			if (!flag)
			{
				CustomDarknessHandler.m_material = new Material(CustomDarknessHandler.DarknessEffectShader);
				Pixelator.Instance.AdditionalCoreStackRenderPass = CustomDarknessHandler.m_material;
				CustomDarknessHandler.isDark = true;
			}
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00010C50 File Offset: 0x0000EE50
		private void DisableDarkness()
		{
			bool flag = !CustomDarknessHandler.isDark;
			if (!flag)
			{
				bool flag2 = Pixelator.Instance;
				if (flag2)
				{
					Pixelator.Instance.AdditionalCoreStackRenderPass = null;
				}
				CustomDarknessHandler.isDark = false;
			}
		}

		// Token: 0x0400015B RID: 347
		public static OverridableBool shouldBeDark = new OverridableBool(false);

		// Token: 0x0400015C RID: 348
		public static OverridableBool shouldBeLightOverride = new OverridableBool(false);

		// Token: 0x0400015D RID: 349
		public static bool isDark = false;

		// Token: 0x0400015E RID: 350
		public static Shader DarknessEffectShader;

		// Token: 0x0400015F RID: 351
		public float FlashlightAngle = 25f;

		// Token: 0x04000160 RID: 352
		private static Material m_material;
	}
}
