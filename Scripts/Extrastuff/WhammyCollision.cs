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
    
    public class WhammyCollision : MonoBehaviour
    {

        public WhammyCollision()
        {

        }

        private void Start()
        {

            this.m_projectile = base.GetComponent<Projectile>();
            this.m_projectile.collidesWithProjectiles = true;
            this.m_projectile.UpdateCollisionMask();
            player = (PlayerController)m_projectile.Owner;
            SpeculativeRigidbody specRigidbody = this.m_projectile.specRigidbody;
            specRigidbody.OnPreRigidbodyCollision += this.HandlePreCollision;
            this.m_projectile.OnDestruction += M_projectile_OnDestruction;
            if (player.PlayerHasActiveSynergy("Over The Moon"))
            {
                DoScalingDamageRamp = true;
                origLoaction = m_projectile.LastPosition;
            }

        }

        public void Update()
        {
            if (DoScalingDamageRamp)
            {
                if(Vector2.Distance(origLoaction,m_projectile.LastPosition)>= 6)
                {
                    m_projectile.sprite.color = new Color(.45f, 1f, .14f);
                    AkSoundEngine.PostEvent("Play_WPN_grasshopper_shot_01", base.gameObject); 
                    
                    GlobalSparksDoer.DoRadialParticleBurst(5, m_projectile.sprite.WorldCenter, m_projectile.sprite.WorldCenter, 0, 50, 50, null, null, null, GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT);
                }
            }
        }
       

        private void M_projectile_OnDestruction(Projectile obj)
        {
            DoSafeExplosion(obj.specRigidbody.UnitCenter);
        }

        private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody && otherRigidbody.projectile)
            {
                if (otherRigidbody.projectile.Owner is AIActor)
                {
                    if (otherRigidbody.projectile != lastCollidedProjectile)
                    {
                        foreach(var enemyproj in GetBullets())
                        {
                            if(Vector2.Distance(myRigidbody.UnitCenter,enemyproj.specRigidbody.UnitCenter) <= 3.5)
                            {
                                PassiveReflectItem.ReflectBullet(enemyproj, true, myRigidbody.projectile.Owner, 12, 1, .80f, 30);
                            }
                        }

                        if (player.PlayerHasActiveSynergy("Shoot for the Stars"))
                        {
                            DoStaryBusrt(m_projectile.LastPosition);
                        }

                       
                        myRigidbody.projectile.DieInAir();
                        lastCollidedProjectile = otherRigidbody.projectile;
                    }
                }
                PhysicsEngine.SkipCollision = true;
            }

            if(DoScalingDamageRamp == true)
            {
                if (otherRigidbody && otherRigidbody.aiActor != null)
                {
                    if (otherRigidbody.aiActor.healthHaver != null)
                    {
                        if (otherRigidbody.aiActor.healthHaver.IsAlive)
                        {
                            float mult = 1 + (Vector2.Distance(origLoaction, m_projectile.LastPosition) * .2f); // 20% damage buff for every tile traveled 
                            m_projectile.baseData.damage *= mult;
                        }
                    }
                }
            }
            
        }

        private void DoStaryBusrt(Vector2 vector2)
        {
            int angle = 0;

            for (int i = 0; i < 5; i++)
            {

                Projectile projectile = ((Gun)ETGMod.Databases.Items[52]).DefaultModule.chargeProjectiles[0].Projectile;
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, vector2, Quaternion.Euler(0f, 0f, 90 + angle), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag2 = component != null;
                if (flag2)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.baseData.damage = 3f;
                   
                }
                angle += 72;
            }
        }

        private static List<Projectile> GetBullets()
        {
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                if (projectile && projectile.sprite && !projectile.ImmuneToBlanks && !projectile.ImmuneToSustainedBlanks)
                {
                    if (projectile.collidesWithEnemies = false || projectile.isFakeBullet || projectile.Owner is AIActor || (projectile.Shooter != null && projectile.Shooter.aiActor != null) || projectile.ForcePlayerBlankable || projectile.IsBulletScript)
                    {

                        list.Add(projectile);

                    }

                }
            }
            return list;
        }


        public void DoSafeExplosion(Vector3 position)
        {

            ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
            this.smallPlayerSafeExplosion.effect = defaultSmallExplosionData2.effect;
            this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
            this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
            Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);

        }
        private ExplosionData smallPlayerSafeExplosion = new ExplosionData
        {
            damageRadius = 4f,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 15f,
            doDestroyProjectiles = false,
            doForce = false,
            debrisForce = 10f,
            preventPlayerForce = true,
            explosionDelay = 0.1f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = true,
            breakSecretWalls = false,
        };

        
        public PlayerController player;
        private Projectile m_projectile;
        private Projectile lastCollidedProjectile;
        public bool DoScalingDamageRamp = false;
        public Vector2 origLoaction;
    }
    



}
