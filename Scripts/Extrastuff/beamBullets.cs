using System;
using UnityEngine;

namespace Knives
{
	// Token: 0x020002EA RID: 746
	public class BeamBulletsBehaviour : MonoBehaviour 
	{
		// Token: 0x06000F7A RID: 3962 RVA: 0x000CD942 File Offset: 0x000CBB42
		public BeamBulletsBehaviour()
		{
			
			this.firetype = BeamBulletsBehaviour.FireType.PLUS;
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x000CD960 File Offset: 0x000CBB60
		private void Start()
		{
			this.m_projectile = base.GetComponent<Projectile>();
			bool flag = this.m_projectile.Owner && this.m_projectile.Owner is PlayerController;
			if (flag)
			{
				this.m_owner = (this.m_projectile.Owner as PlayerController);
			}
			base.Invoke("BeginBeamFire", 0.1f);
		}

		// Token: 0x06000F7C RID: 3964 RVA: 0x000CD9D0 File Offset: 0x000CBBD0
		private void BeginBeamFire()
		{
			bool flag = this.firetype == BeamBulletsBehaviour.FireType.FORWARDS;
			if (flag)
			{
				BeamController beamController = BeamToolbox.FreeFireBeamFromAnywhere(this.beamToFire, this.m_owner, this.m_projectile.gameObject, Vector2.zero, false, 0f, 1000f, true, true, 0f);
				Projectile component = beamController.GetComponent<Projectile>();
			}
			bool flag2 = this.firetype == BeamBulletsBehaviour.FireType.BACKWARDS;
			if (flag2)
			{
				BeamController beamController2 = BeamToolbox.FreeFireBeamFromAnywhere(this.beamToFire, this.m_owner, this.m_projectile.gameObject, Vector2.zero, false, 180f, 1000f, true, true, 180f);
				Projectile component2 = beamController2.GetComponent<Projectile>();
			}
			bool flag3 = this.firetype == BeamBulletsBehaviour.FireType.CROSS || this.firetype == BeamBulletsBehaviour.FireType.STAR;
			if (flag3)
			{
				BeamController beamController3 = BeamToolbox.FreeFireBeamFromAnywhere(this.beamToFire, this.m_owner, this.m_projectile.gameObject, Vector2.zero, false, 45f, 1000f, true, false, 0f);
				Projectile component3 = beamController3.GetComponent<Projectile>();
				component3.baseData.damage *= this.m_owner.stats.GetStatValue(PlayerStats.StatType.Damage);
				BeamController beamController4 = BeamToolbox.FreeFireBeamFromAnywhere(this.beamToFire, this.m_owner, this.m_projectile.gameObject, Vector2.zero, false, 135f, 1000f, true, false, 0f);
				Projectile component4 = beamController4.GetComponent<Projectile>();
				component4.baseData.damage *= this.m_owner.stats.GetStatValue(PlayerStats.StatType.Damage);
				BeamController beamController5 = BeamToolbox.FreeFireBeamFromAnywhere(this.beamToFire, this.m_owner, this.m_projectile.gameObject, Vector2.zero, false, -45f, 1000f, true, false, 0f);
				Projectile component5 = beamController5.GetComponent<Projectile>();
				component5.baseData.damage *= this.m_owner.stats.GetStatValue(PlayerStats.StatType.Damage);
				BeamController beamController6 = BeamToolbox.FreeFireBeamFromAnywhere(this.beamToFire, this.m_owner, this.m_projectile.gameObject, Vector2.zero, false, -135f, 1000f, true, false, 0f);
				Projectile component6 = beamController6.GetComponent<Projectile>();
				component6.baseData.damage *= this.m_owner.stats.GetStatValue(PlayerStats.StatType.Damage);
			}
			bool flag4 = this.firetype == BeamBulletsBehaviour.FireType.PLUS || this.firetype == BeamBulletsBehaviour.FireType.STAR;
			if (flag4)
			{
				BeamController beamController7 = BeamToolbox.FreeFireBeamFromAnywhere(this.beamToFire, this.m_owner, this.m_projectile.gameObject, Vector2.zero, false, 0f, 1000f, true, false, 0f);
				Projectile component7 = beamController7.GetComponent<Projectile>();
				component7.baseData.damage *= this.m_owner.stats.GetStatValue(PlayerStats.StatType.Damage);
				BeamController beamController8 = BeamToolbox.FreeFireBeamFromAnywhere(this.beamToFire, this.m_owner, this.m_projectile.gameObject, Vector2.zero, false, 90f, 1000f, true, false, 0f);
				Projectile component8 = beamController8.GetComponent<Projectile>();
				component8.baseData.damage *= this.m_owner.stats.GetStatValue(PlayerStats.StatType.Damage);
				BeamController beamController9 = BeamToolbox.FreeFireBeamFromAnywhere(this.beamToFire, this.m_owner, this.m_projectile.gameObject, Vector2.zero, false, 180f, 1000f, true, false, 0f);
				Projectile component9 = beamController9.GetComponent<Projectile>();
				component9.baseData.damage *= this.m_owner.stats.GetStatValue(PlayerStats.StatType.Damage);
				BeamController beamController10 = BeamToolbox.FreeFireBeamFromAnywhere(this.beamToFire, this.m_owner, this.m_projectile.gameObject, Vector2.zero, false, -90f, 1000f, true, false, 0f);
				Projectile component10 = beamController10.GetComponent<Projectile>();
				component10.baseData.damage *= this.m_owner.stats.GetStatValue(PlayerStats.StatType.Damage);
			}
			bool flag5 = this.firetype == BeamBulletsBehaviour.FireType.TRAIL_V;
			if (flag5)
			{
				BeamController beamController11 = BeamToolbox.FreeFireBeamFromAnywhere(this.beamToFire, this.m_owner, this.m_projectile.gameObject, Vector2.zero, false, 180f, 1000f, true, true, 180f - vTrailAngle);
				Projectile component11 = beamController11.GetComponent<Projectile>();
				BeamController beamController12 = BeamToolbox.FreeFireBeamFromAnywhere(this.beamToFire, this.m_owner, this.m_projectile.gameObject, Vector2.zero, false, 180f, 1000f, true, true, -180f + vTrailAngle);
				Projectile component12 = beamController12.GetComponent<Projectile>();
			}
		}

		public float vTrailAngle = 40;
		// Token: 0x06000F7D RID: 3965 RVA: 0x000CDDC4 File Offset: 0x000CBFC4
		private void Update()
		{
		}

		// Token: 0x04000BAC RID: 2988
		private Projectile m_projectile;

		// Token: 0x04000BAD RID: 2989
		private PlayerController m_owner;

		// Token: 0x04000BAE RID: 2990
		public Projectile beamToFire;

		// Token: 0x04000BAF RID: 2991
		public BeamBulletsBehaviour.FireType firetype;

		// Token: 0x0200051D RID: 1309
		public enum FireType
		{
			// Token: 0x040014C8 RID: 5320
			PLUS,
			// Token: 0x040014C9 RID: 5321
			CROSS,
			// Token: 0x040014CA RID: 5322
			STAR,
			// Token: 0x040014CB RID: 5323
			FORWARDS,
			// Token: 0x040014CC RID: 5324
			BACKWARDS,
			//
			TRAIL_V,
		}
	}
}