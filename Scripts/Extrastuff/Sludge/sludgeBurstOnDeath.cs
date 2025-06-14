using System;
using ItemAPI;
using UnityEngine;
using Dungeonator;

namespace Knives
{
	public class SludgeBurtsOnDeath : MonoBehaviour
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
		public SludgeBurtsOnDeath()
		{

		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0006765B File Offset: 0x0006585B
		private void Start()
		{
			main = base.gameObject.GetComponent<Projectile>();
			
			main.specRigidbody.OnPreTileCollision += HitWall;
			main.specRigidbody.OnPreRigidbodyCollision += HitBody;
            main.OnDestruction += Main_OnDestruction;
		}

        private void Main_OnDestruction(Projectile obj)
        {
			burst(obj.projectile);
		}

        private void HitBody(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
			if(myRigidbody.projectile != null)
            {
				if(otherRigidbody.minorBreakable == false)
                {
					burst(myRigidbody.projectile);
				}
			}
        }

        private void HitWall(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
        {
			if (myRigidbody.projectile != null)
			{
				burst(myRigidbody.projectile);
			}
		}

        private void HitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
			if (arg1 != null)
			{
				burst(arg1);
			}
		}

		public static void burst(Projectile arg1)
        {
			float rotate = 0;
			float mainAng = arg1.Direction.ToAngle();
			for(int i = 0; i <= 5; i++)
            {
				Projectile ball = MiscToolMethods.SpawnProjAtPosi(SkudgePump.smallGoop, arg1.LastPosition, arg1.Owner as PlayerController,mainAng,rotate);
				ball.baseData.range = 3f;
				GoopModifier goop = ball.gameObject.GetOrAddComponent<GoopModifier>();
				goop.goopDefinition = EasyGoopDefinitions.SludgeGoop;
				goop.SpawnGoopInFlight = true;
				goop.InFlightSpawnRadius = 2;
				goop.InFlightSpawnFrequency = .01f;
				if ((arg1.Owner as PlayerController).PlayerHasActiveSynergy("GooBomber Special"))
				{
					ball.baseData.range = 1f;
				}
				rotate += 72;
			}
			arg1.DieInAir();
        }

        private void Update()
		{
			
		}

		public Projectile main;
	}
}