using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using ItemAPI;
using Gungeon;


namespace Knives
{
    public class FirstImpress_FormController : MonoBehaviour
    {
        public FirstImpress_FormController()
        {
            this.NonSynergyGunId = FirstImpression.StandardID;
            this.SynergyGunId = FirstImpression_special.SpecialID;
           
        }
       
        private void Awake()
        {
            this.m_gun = base.GetComponent<Gun>();
        }

        private void Update()
        {
            
            

            if (Dungeon.IsGenerating || Dungeon.ShouldAttemptToLoadFromMidgameSave)
            {
                return;
            }
            PlayerController player = this.m_gun.CurrentOwner as PlayerController;
            if (this.m_gun && this.m_gun.CurrentOwner is PlayerController)
            {

                if (!this.m_gun.enabled)
                {
                    return;
                }
                if (FirstImpression.hasAttackedThisCombat == false && !this.m_transformed)
                {
                    this.m_transformed = true;
                    this.m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(this.SynergyGunId) as Gun);

                    if (this.ShouldResetAmmoAfterTransformation)
                    {
                        this.m_gun.ammo = this.ResetAmmoCount;
                    }
                }
                else if (FirstImpression.hasAttackedThisCombat == true && this.m_transformed)
                {
                    this.m_transformed = false;
                    this.m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(this.NonSynergyGunId) as Gun);
                    if (this.ShouldResetAmmoAfterTransformation)
                    {
                        this.m_gun.ammo = this.ResetAmmoCount;
                    }
                }
            }
            else if (this.m_gun && !this.m_gun.CurrentOwner && this.m_transformed)
            {
                this.m_transformed = false;
                this.m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(this.NonSynergyGunId) as Gun);
                if (this.ShouldResetAmmoAfterTransformation)
                {
                    this.m_gun.ammo = this.ResetAmmoCount;
                }
            }
            this.ShouldResetAmmoAfterTransformation = false;


            

        }


        public PlayerController player;
       
        public string SynergyToCheck;
        public int NonSynergyGunId;
        public int SynergyGunId;
        private Gun m_gun;
        private bool m_transformed;
        public bool ShouldResetAmmoAfterTransformation;
        public int ResetAmmoCount;
    }
}