using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using Alexandria.ItemAPI;
using SaveAPI;
using Gungeon;
using UnityEngine;
using System.Collections;

namespace Knives
{

    public class ReverseSpeedRampMod : MonoBehaviour
    {

        public ReverseSpeedRampMod()
        {

        }

        private void Start()
        {

            this.m_projectile = base.GetComponent<Projectile>();
             
            m_projectile.UpdateSpeed();
            player = (PlayerController)m_projectile.Owner;
            if(player != null)
            {
                if(player.CurrentGun != null)
                {
                    if (!player.CurrentGun.HasShootStyle(ProjectileModule.ShootStyle.Beam)) m_projectile.baseData.speed *= .2f;

                }

            }

            SpeculativeRigidbody specRigidbody = this.m_projectile.specRigidbody;
            specRigidbody.OnPreRigidbodyCollision += this.HandlePreCollision;
            origLoaction = m_projectile.LastPosition;
            
            
        }

        public void Update()
        {
            if (doeffects)
            {
                if (Vector2.Distance(origLoaction, m_projectile.LastPosition) >= 1)
                {
                    m_projectile.sprite.color = ExtendedColours.gildedBulletsGold;
                    GlobalSparksDoer.DoRadialParticleBurst(1, m_projectile.sprite.WorldCenter, m_projectile.sprite.WorldCenter, 0, 1, 1, null, null, null, GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT);
                }

            }
            
        }

        private void FixedUpdate()
        {
            if(!player.CurrentGun.HasShootStyle(ProjectileModule.ShootStyle.Beam))
            {
                m_projectile.baseData.speed *= 1.05f;
                m_projectile.UpdateSpeed();
            }

        }

        private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            
            if (otherRigidbody && otherRigidbody.aiActor != null)
            {
                if (otherRigidbody.aiActor.healthHaver != null)
                {
                    if (otherRigidbody.aiActor.healthHaver.IsAlive)
                    {
                        if (isGun && isCharm) DoubleMult = 2;
                        float distance = (Vector2.Distance(origLoaction, m_projectile.LastPosition));
                        if (distance > 10) distance = 10; // max 10 tile ramp (max 70%)
                        float mult = 1 + ((distance * .3f) * DoubleMult ); // 3% damage buff for every tile traveled // if is both gun and clock charm double effect
                        m_projectile.baseData.damage *= mult;
                    }
                }
            }
            

        }

       
        public int DoubleMult = 1;
        public bool isCharm = false;
        public bool isGun = false;
        public PlayerController player;
        private Projectile m_projectile;
        public bool doeffects = true;
        public bool DoScalingDamageRamp = false;
        public Vector2 origLoaction;
    }




}
