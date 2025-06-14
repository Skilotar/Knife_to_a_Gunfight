using System;
using UnityEngine;

namespace Knives
{
	// Token: 0x02000339 RID: 825
	public class CameraOverrideTarget : MonoBehaviour
	{
		// Token: 0x060010D0 RID: 4304 RVA: 0x000DD374 File Offset: 0x000DB574
		public CameraOverrideTarget()
		{
			this.startOverrideOnStart = false;
			this.duration = -1f;
			this.timer = -1f;
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x000DD39B File Offset: 0x000DB59B
		private void OnDestroy()
		{
			GameManager.Instance.MainCameraController.SetManualControl(false, true);
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x000DD3B0 File Offset: 0x000DB5B0
		private void Start()
		{
			this.targetObject = base.GetComponent<SpeculativeRigidbody>();
			bool flag = this.startOverrideOnStart;
			if (flag)
			{
				this.BeginOverride();
			}
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x000DD3DC File Offset: 0x000DB5DC
		private void Update()
		{
			bool flag = this.targetObject;
			if (flag)
			{
				GameManager.Instance.MainCameraController.OverridePosition = this.targetObject.UnitCenter + Offset;
			}
			bool flag2 = this.duration > 0f;
			if (flag2)
			{
				bool flag3 = this.timer <= 0f;
				if (flag3)
				{
					this.EndOverride();
				}
				else
				{
					this.timer -= BraveTime.DeltaTime;
				}
			}
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x000DD460 File Offset: 0x000DB660
		public void BeginOverride()
		{
			GameManager.Instance.MainCameraController.SetManualControl(true, false);
			bool flag = this.duration > 0f;
			if (flag)
			{
				this.timer = this.duration;
			}
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x000DD49D File Offset: 0x000DB69D
		public void EndOverride()
		{
			GameManager.Instance.MainCameraController.SetManualControl(false, true);
		}

		// Token: 0x04000D1D RID: 3357
		public bool startOverrideOnStart;

		// Token: 0x04000D1E RID: 3358
		private SpeculativeRigidbody targetObject;

		// Token: 0x04000D1F RID: 3359
		public float duration;

		// Token: 0x04000D20 RID: 3360
		private float timer;

		public Vector2 Offset = new Vector2(0, 0);
	}
}