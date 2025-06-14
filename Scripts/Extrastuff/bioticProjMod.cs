using System;
using ItemAPI;
using UnityEngine;
using Dungeonator;
using System.Collections;

namespace Knives
{
    public class BioticProjMod : MonoBehaviour
    {

        public BioticProjMod()
        {

        }
        public float StoreDamage = 0;
        private void Start()
        {
            try
            {
                proj = base.gameObject.GetComponent<Projectile>();
                this.parentOwner = this.proj.ProjectilePlayerOwner();
                gun = this.proj.PossibleSourceGun;
                StoreDamage = proj.ModifiedDamage;
                
                proj.baseData.damage = 0;
                proj.OnHitEnemy += this.OnHitEnemy;
                var mod = proj.gameObject.GetOrAddComponent<StickyProjectileModifier>();
                mod.stickyContexts.Add(new StickyProjectileModifier.StickyContext() { CanStickToTerrain = false, CanStickEnemies = true });
                mod.OnStick += PleaseStick;
               
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }

        private void PleaseStick(GameObject arg1, StickyProjectileModifier arg2, tk2dBaseSprite arg3, PlayerController arg4)
        {
            arg1.GetComponent<SpeculativeRigidbody>().enabled = false;
        }

        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.aiActor != null)
            {
                if (arg2.aiActor.healthHaver != null)
                {
                    BioticActorDOT dot = arg2.aiActor.gameObject.GetOrAddComponent<BioticActorDOT>();
                    dot.damageThisTick = 2.5f;
                    dot.damageToDo = StoreDamage;
                    dot.Interval = .1f;
                    dot.StartCoroutine(BioticActorDOT.DOT(arg2.aiActor, StoreDamage, 2.5f, .1f));
                }
            }
        }

        public Projectile proj;
        private PlayerController parentOwner;
        public PierceProjModifier poke;
        private Gun gun;
    }

    public class BioticActorDOT : MonoBehaviour
    {

        public BioticActorDOT()
        {

        }
       
        private void Start()
        {
            try
            {
                host = base.gameObject.GetComponent<AIActor>();
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }


        public static IEnumerator DOT(AIActor aiActor, float damageToDo, float damageThisTick, float interval)
        {

            while (damageToDo >= 0 && aiActor.healthHaver.IsAlive)
            {

                float currenthit = Math.Min(damageToDo, damageThisTick);
                damageToDo -= currenthit;
                aiActor.healthHaver.ApplyDamage(currenthit, Vector2.zero, "biotic");
                yield return new WaitForSeconds(interval);

                yield return null;
            }
        }

        public AIActor host;
        public float Interval = .1f;
        public float damageThisTick = 3;
        public float damageToDo = 15;
    }
}