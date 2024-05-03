using System;
using ItemAPI;
using UnityEngine;
using Dungeonator;
using System.Collections;

namespace Knives
{
    public class VectorMK2BulletsEffect : MonoBehaviour
    {

        public VectorMK2BulletsEffect()
        {

        }
        private void Start()
        {
            try
            {
                this.m_projectile = base.GetComponent<Projectile>();
                this.m_projectile.UpdateSpeed();
                gun = this.m_projectile.PossibleSourceGun;
                owner = this.m_projectile.Owner as PlayerController;
                this.speedMultiplierPerFrame = UnityEngine.Random.Range(90f, 94f) / 100f;
                
                Outgoing = true;
                origiModSpeed = m_projectile.baseData.speed;
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }
        public Gun gun;
        public PlayerController owner;
        private Projectile m_projectile;
        private float speedMultiplierPerFrame;
        public float origiModSpeed;
        private bool Outgoing;
        public bool HasSynergyHunterSpores;
        public float SizeMultPerFrame;

        private void FixedUpdate()
        {
            if (Outgoing)
            {
                float minspeedReversal = 0.1f;
                if (owner.PlayerHasActiveSynergy("LerpUnclamped();")) minspeedReversal = 1f;
                if (m_projectile.baseData.speed > minspeedReversal)
                {
                    m_projectile.baseData.speed *= speedMultiplierPerFrame;
                    m_projectile.UpdateSpeed();

                }
                else
                {

                    Outgoing = false;
                }
            }
            else
            {
                if (m_projectile.baseData.speed < origiModSpeed)
                {
                    
                    m_projectile.baseData.speed /= (.98f);
                    m_projectile.UpdateSpeed();
                    
                }
                
                
                if (Vector2.Distance(owner.CenterPosition, this.m_projectile.LastPosition) > 1)
                {
                    
                    if (owner.PlayerHasActiveSynergy("CalculateVectorBetween(Enemy);"))
                    {
                        float distance = 5f;
                        AIActor Target = owner.CurrentRoom.GetNearestEnemy(this.m_projectile.LastPosition, out distance, true, true);
                        if(Target != null)
                        {
                            Vector2 Vector2Vector2 = Target.CenterPosition - (Vector2)m_projectile.LastPosition;
                            m_projectile.SendInDirection(Vector2Vector2, false, true);
                        }
                        else
                        {
                            Vector2 Vector2Vector2 = owner.CenterPosition - (Vector2)m_projectile.LastPosition;
                            m_projectile.SendInDirection(Vector2Vector2, true, true);
                        }

                    }
                    else
                    {
                        Vector2 Vector2Vector2 = owner.CenterPosition - (Vector2)m_projectile.LastPosition;
                        m_projectile.SendInDirection(Vector2Vector2, true, true);
                    }
                    
                }
                else
                {
                    if (owner.PlayerHasActiveSynergy("Rotate();"))
                    {
                        DoAddSpin(this.m_projectile);
                    }
                    else
                    {
                        m_projectile.hitEffects.suppressMidairDeathVfx = true;
                        if (UnityEngine.Random.Range(0, 2) == 1)
                        {
                            gun.GainAmmo(1);

                        }

                        m_projectile.DieInAir();
                    }
                   

                }
                

                
            }
           
        }
        public OrbitProjectileMotionModule spincheck;
        public void DoAddSpin(Projectile proj)
        {

            BounceProjModifier bouncer = proj.gameObject.GetOrAddComponent<BounceProjModifier>();
            bouncer.name = "Vector.Rotate()";

            int orbitersInGroup = OrbitProjectileMotionModule.GetOrbitersInGroup(1000);
            if (orbitersInGroup >= 20)
            {
                return;
            }
            bouncer.projectile.specRigidbody.CollideWithTileMap = false;
            proj.UpdateCollisionMask();
            bouncer.projectile.ResetDistance();
            bouncer.numberOfBounces = 1;
            bouncer.projectile.baseData.range = Mathf.Max(bouncer.projectile.baseData.range, 50f);
            bouncer.projectile.collidesWithEnemies = true;

            OrbitProjectileMotionModule orbitProjectileMotionModule = new OrbitProjectileMotionModule();
            orbitProjectileMotionModule.lifespan = 4000f;
            if (bouncer.projectile.OverrideMotionModule != null && bouncer.projectile.OverrideMotionModule is HelixProjectileMotionModule)
            {
                orbitProjectileMotionModule.StackHelix = true;
                orbitProjectileMotionModule.ForceInvert = (bouncer.projectile.OverrideMotionModule as HelixProjectileMotionModule).ForceInvert;
            }
            orbitProjectileMotionModule.usesAlternateOrbitTarget = false;

            orbitProjectileMotionModule.MinRadius = 1;
            orbitProjectileMotionModule.MaxRadius = 2;
            spincheck = orbitProjectileMotionModule;
            bouncer.projectile.OverrideMotionModule = spincheck;

        }

    }
}