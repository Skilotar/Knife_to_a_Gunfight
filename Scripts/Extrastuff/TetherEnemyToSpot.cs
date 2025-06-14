using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;

namespace Knives
{
	public class TetherEnemyToSpot : MonoBehaviour
	{

		public TetherEnemyToSpot()
		{

		}


		private void Start()
		{
			Host = base.GetComponent<AIActor>();
			baseMoveSpeed = Host.MovementSpeed;
            Host.healthHaver.OnPreDeath += HealthHaver_OnPreDeath;
			tk2dTiledSprite Chain = GrapplePrefab.GetComponentInChildren<tk2dTiledSprite>();
			if (m_chain != null) UnityEngine.GameObject.Destroy(m_chain);
		}

        private void HealthHaver_OnPreDeath(Vector2 obj)
        {
			TetherEnemyToSpot thisClass = Host.gameObject.GetComponent<TetherEnemyToSpot>();
			if (m_chain != null) UnityEngine.GameObject.Destroy(m_chain);
			DoLaser = false;
			Host.MovementSpeed = baseMoveSpeed;
			UnityEngine.GameObject.Destroy(thisClass);
			
		}

		private void Update()
		{
			if(Host != null)
            {
                if (Host.healthHaver.IsAlive)
                {
					if (TetherPoint != null)
					{
						float Distance = Vector2.Distance(Host.CenterPosition, TetherPoint);
						if(Distance < StopbreakDistance)
                        {
							if(Host.specRigidbody.Velocity.magnitude > 10)
                            {
								StartCoroutine(stopWhiplash());
							}
                        }
						if (Distance < freewalkRadius && Distance < minorslowwalkRadius && Distance < majorslowwalkRadius)
						{
							Host.MovementSpeed = baseMoveSpeed;
							
						}
						if (Distance > freewalkRadius && Distance < minorslowwalkRadius && Distance < majorslowwalkRadius)
						{
							Host.MovementSpeed = baseMoveSpeed * .5f;
							Host.knockbackDoer.ApplyKnockback(TetherPoint - Host.CenterPosition, minorKB, false);
						}
						if (Distance > freewalkRadius && Distance > minorslowwalkRadius && Distance < majorslowwalkRadius)
						{
							Host.MovementSpeed = baseMoveSpeed * .2f;
							Host.knockbackDoer.ApplyKnockback(TetherPoint - Host.CenterPosition, majorKB, false);

						}
						if (Distance > chainBreakRadius)
						{
							if (elapsed <= TensileStength)
							{
                                if (projDead)
                                {
									elapsed += BraveTime.DeltaTime;
								}
                                else
                                {
									Host.knockbackDoer.ApplyKnockback(TetherPoint - Host.CenterPosition, 10, false);
								}
							}
							else
							{
								DoBreak();
							}
                        }
                        else
                        {
							elapsed = 0;
                        }

						if (m_chain == null && DoLaser == true)
						{
							doFunkyTetherGarbage(Host);
						}
						if(m_chain != null)
                        {
							m_chain.transform.rotation = Quaternion.Euler(0, 0, (Host.CenterPosition - TetherPoint).ToAngle());
							tk2dTiledSprite sprite = m_chain.GetComponent<tk2dTiledSprite>();
							sprite.dimensions = new Vector2(Vector2.Distance(Host.CenterPosition, TetherPoint) * 16, 1);
							m_chain.transform.position = TetherPoint;
						}
						if(UpdateToPlayer == true && player != null)
                        {
							TetherPoint = player.CenterPosition;
						}
						if(UpdateToObject == true && UpdateToPlayer == false)
                        {
							if(obj != null)
                            {
								if (obj.GetComponent<tk2dSprite>() != null)
								{
									TetherPoint = obj.GetComponent<tk2dSprite>().sprite.WorldCenter;

								}
								else
								{
									TetherPoint = obj.transform.position; // backup point
								}
							}
                            else
                            {
								DoBreak();
                            }
                        }

					}
				}
              
            }
		}
		public void DoBreak()
        {
			TetherEnemyToSpot thisClass = Host.gameObject.GetComponent<TetherEnemyToSpot>();
			if (m_chain != null) UnityEngine.GameObject.Destroy(m_chain);
			DoLaser = false;
			Host.MovementSpeed = baseMoveSpeed;

			UnityEngine.GameObject.Destroy(thisClass);
		}

        private IEnumerator stopWhiplash()
        {
			Host.knockbackDoer.SetImmobile(true, "StopWhiplash");
			yield return new WaitForSeconds(.15f);
			Host.knockbackDoer.SetImmobile(false, "StopWhiplash");
		}

        private void doFunkyTetherGarbage(AIActor target) // this is trash... 
		{
			float ang = (Host.CenterPosition - TetherPoint).ToAngle();
			UnityEngine.GameObject laser = RenderTether(TetherPoint, Vector2.Distance(target.CenterPosition, TetherPoint) * 16, 1, ang, true, color);
			m_chain = laser;
		}

		public static GameObject RenderTether(Vector2 position, float length, float width, float angle, bool alterColour = false, Color? colour = null)
		{
			GameObject laserSightPrefab = LoadHelper.LoadAssetFromAnywhere("assets/resourcesbundle/global vfx/vfx_lasersight.prefab") as GameObject;

			GameObject gameObject = SpawnManager.SpawnVFX(laserSightPrefab, position, Quaternion.Euler(0, 0, angle));

			tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
			float newWidth = 1f;
			if (width != -1) newWidth = width;
			component2.dimensions = new Vector2(length, newWidth);
			if (alterColour && colour != null)
			{
				component2.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.SetColor("_OverrideColor", (Color)colour);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", (Color)colour);
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 50);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
			}
			return gameObject;
		}

		public GameObject GrapplePrefab = ((GrapplingHookItem)PickupObjectDatabase.GetById(250)).GrapplePrefab;
		private tk2dTiledSprite Chain;

		public float StopbreakDistance = .5f;
		public float minorKB = 5;
		public float majorKB = 25;
		private bool DoLaser = true;
		public bool projDead = true;
		private float elapsed = 0;
		public float TensileStength = 1f;
		public Color color = UnityEngine.Color.white;
		private float baseMoveSpeed;
        private GameObject m_extantGrapple;
        public AIActor Host;
		public Vector2 TetherPoint;
		public float freewalkRadius = 4;
		public float minorslowwalkRadius = 5;
		public float majorslowwalkRadius = 6;
		public float chainBreakRadius = 8;
        private GameObject m_chain;
		public PlayerController player;
		public bool UpdateToObject = false;
		public GameObject obj;

		public bool UpdateToPlayer = false;
    }
}