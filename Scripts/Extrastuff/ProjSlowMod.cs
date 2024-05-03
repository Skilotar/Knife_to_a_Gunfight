using System;
using ItemAPI;
using UnityEngine;
using Dungeonator;
using System.Collections;

namespace Knives
{
	public class SlowingBulletsEffect : MonoBehaviour
	{
      
        public SlowingBulletsEffect()
        {
            
        }
        private void Start()
        {
            try
            {
                this.m_projectile = base.GetComponent<Projectile>();

                this.m_projectile.UpdateSpeed();
                this.speedMultiplierPerFrame = UnityEngine.Random.Range(80f, 98f) / 100f;
                if(ignoresizescale == false)
                {
                    this.SizeMultPerFrame = UnityEngine.Random.Range(1f, 1.3f) / 100f;
                    this.m_projectile.RuntimeUpdateScale(UnityEngine.Random.Range(120f, 300f) / 100f);
                }
                if (Setmult != 0) speedMultiplierPerFrame = Setmult;
                shouldSpeedModify = true;

            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }
        private Projectile m_projectile;
        private float speedMultiplierPerFrame;
        private bool shouldSpeedModify;
        public bool HasSynergyHunterSpores;
        public float SizeMultPerFrame;

        private void FixedUpdate()
        {
            if (shouldSpeedModify)
            {
                if (m_projectile.baseData.speed > 0.01f)
                {
                    m_projectile.baseData.speed *= speedMultiplierPerFrame;
                    m_projectile.AdditionalScaleMultiplier *= SizeMultPerFrame;
                    m_projectile.UpdateSpeed();
                    
                }
                else
                {
                    StartCoroutine(FloatHandler());
                    shouldSpeedModify = false;
                }
            }
            //GlobalSparksDoer.DoRandomParticleBurst(3, m_projectile.sprite.WorldBottomLeft, m_projectile.sprite.WorldTopRight, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
        }
        private IEnumerator FloatHandler()
        {
            yield return new WaitForSeconds(LifeSpan);
            m_projectile.DieInAir();
        }

        public float LifeSpan = 3;
        public float Setmult = 0;
        public bool ignoresizescale = false;
        
    }
}