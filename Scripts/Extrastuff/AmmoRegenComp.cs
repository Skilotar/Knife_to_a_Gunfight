using System;
using UnityEngine;

// Token: 0x020016DA RID: 5850
public class AmmoRegenComp : MonoBehaviour
{
	// Token: 0x0600881F RID: 34847 RVA: 0x00375BAC File Offset: 0x00373DAC
	public AmmoRegenComp()
	{
		this.AmmoPer = .5f;
		
		this.m_ammoCounter = 0;
	}

	
	private void Start()
	{
		this.m_gun = base.GetComponent<Gun>();
	}
	public float postShotRestartDelay = 0;
	public void Update()
	{
		if(postShotRestartDelay <= 0)
        {
			if (this.m_gun.CurrentOwner && !this.m_gun.IsFiring && (this.m_gun.CurrentOwner as PlayerController).IsInCombat)
			{
				this.m_ammoCounter += (BraveTime.DeltaTime) * this.AmmoPer;
				if (this.m_ammoCounter > 1f)
				{
					int num = Mathf.FloorToInt(this.m_ammoCounter);
					this.m_ammoCounter -= (float)num;
					this.m_gun.GainAmmo(num);
				}
			}
			
		}
        else
        {
			postShotRestartDelay -= Time.deltaTime;
        }
        if (this.m_gun.IsFiring)
        {
			postShotRestartDelay = .35f;

		}
	}

	// Token: 0x06008822 RID: 34850 RVA: 0x00375C77 File Offset: 0x00373E77
	
	public float intervalTime;
	public bool CombatRequired = false;
	public float AmmoPer;

	private Gun m_gun;
	private float m_ammoCounter;
	private float m_gameTimeOnDisable;
}