using System;
using System.Collections;
using Dungeonator;
using UnityEngine;

using ItemAPI;
using System.Collections.Generic;

namespace Knives
{
    public class PierceWallsProjMod : MonoBehaviour
    {
        public PierceWallsProjMod()
        {

        }

        private void Start()
        {

            this.m_projectile = base.GetComponent<Projectile>();
            if (this.m_projectile.Owner is PlayerController)
            {
                this.projOwner = this.m_projectile.Owner as PlayerController;
            }
            SpeculativeRigidbody specRigidBody = this.m_projectile.specRigidbody;
            this.m_projectile.BulletScriptSettings.surviveTileCollisions = true;

            this.m_projectile.pierceMinorBreakables = true;
            m_projectile.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(m_projectile.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreCollisontile));

        }

        public bool isinwall = false;
        private void OnPreCollisontile(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
        {
            if (PiercedOneWall != true)
            {

                isinwall = true;

                PhysicsEngine.SkipCollision = true;
            }
            else
            {
                m_projectile.DieInAir();

            }

        }

        private void Update()
        {
            keeptrackofinwallstate();
        }
        bool Once = false;
        bool PiercedOneWall = false;
        private void keeptrackofinwallstate()
        {
            if (isinwall == true) // if was in wall
            {
                if (m_projectile.specRigidbody.HasTriggerCollisions == false) // check if leaves the wall because no collisions
                {
                    isinwall = false;
                    Once = true;
                }
            }
            else
            {

                if (m_projectile.LastPosition.GetAbsoluteRoom() != null)
                {

                    if (m_projectile.LastPosition.GetAbsoluteRoom() == this.projOwner.CurrentRoom)
                    {
                        if (Once == true)
                        {
                            PiercedOneWall = true;
                            Once = false;
                        }
                    }
                    else
                    {
                        m_projectile.DieInAir(); // Destroys projectile if it goes in a room that you arnt in.
                    }
                }

            }
        }

        private PlayerController projOwner;
        private Projectile m_projectile;
    }
}