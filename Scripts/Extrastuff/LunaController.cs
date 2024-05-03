using System;
using ItemAPI;
using UnityEngine;
using Dungeonator;
using System.Collections;

namespace Knives
{
    public class LunaProjCollide : MonoBehaviour
    {

        public LunaProjCollide()
        {

        }
        private void Start()
        {
            try
            {
                m_projectile = base.GetComponent<Projectile>();
                player = m_projectile.Owner as PlayerController;
                BasebeamComp = base.GetComponent<BasicBeamController>();
                beamcomp = base.GetComponent<BeamController>();   
                
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }
        public void Update()
        {
            m_orb = Luna.M_Borb;

            if (m_orb != null)
            {
                if (BasebeamComp.PosIsNearAnyBoneOnBeam(m_orb.sprite.WorldCenter, 1.5f))
                {
                    Toggle = true;
                }
                else
                {
                    Toggle = false;
                }
            }

        }


        public static bool Toggle = false;
        
        private Projectile m_projectile;
        private BasicBeamController BasebeamComp;
        public BeamController beamcomp;
        public Projectile m_orb;
        private PlayerController player;
    }
}