using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Knives
{ 
    public class CompanionisedEnemyBulletModifiers : BraveBehaviour //----------------------------------------------------------------------------------------------
    {
        public CompanionisedEnemyBulletModifiers()
        {
            this.baseBulletDamage = 10f;
            this.TintBullets = true;
            this.TintColor = Color.grey;
            this.jammedDamageMultiplier = 2f;
        }
        public void Start()
        {
            enemy = base.aiActor;
            AIBulletBank bulletBank2 = enemy.bulletBank;
            foreach (AIBulletBank.Entry bullet in bulletBank2.Bullets)
            {
                bullet.BulletObject.GetComponent<Projectile>().BulletScriptSettings.preventPooling = true;
            }
            if (enemy.aiShooter != null)
            {
                AIShooter aiShooter = enemy.aiShooter;
                aiShooter.PostProcessProjectile = (Action<Projectile>)Delegate.Combine(aiShooter.PostProcessProjectile, new Action<Projectile>(this.PostProcessSpawnedEnemyProjectiles));
            }

            if (enemy.bulletBank != null)
            {
                AIBulletBank bulletBank = enemy.bulletBank;
                bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.PostProcessSpawnedEnemyProjectiles));
            }
        }
        private void PostProcessSpawnedEnemyProjectiles(Projectile proj)
        {
            if (TintBullets) { proj.AdjustPlayerProjectileTint(this.TintColor, 1); }
            if (enemy != null)
            {
                if (enemy.aiActor != null)
                {
                    proj.baseData.damage = baseBulletDamage;
                    if (enemy.aiActor.IsBlackPhantom) { proj.baseData.damage = baseBulletDamage * jammedDamageMultiplier; }
                }
            }
            else { ETGModConsole.Log("Shooter is NULL"); }
        }

        private AIActor enemy;
        public float baseBulletDamage;
        public float jammedDamageMultiplier;
        public bool TintBullets;
        public Color TintColor;

    }
}
