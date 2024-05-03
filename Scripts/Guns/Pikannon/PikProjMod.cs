using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using Dungeonator;

namespace Knives
{
	public class PikiProjMod : MonoBehaviour
	{

		public PikiProjMod()
		{
			Pikitospawn = EasyPikiDatabase.RedBoy;
		}


		private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
			gun = Projectile.PossibleSourceGun;
			this.parentOwner = this.Projectile.ProjectilePlayerOwner();
            Projectile.OnDestruction += Projectile_OnDestruction;
			Projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(Projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemy));
		}

       

        private void Projectile_OnDestruction(Projectile obj)
		{

			if( Pikitospawn != null && parentOwner != null)
            {

				RoomHandler room = parentOwner.CurrentRoom;
				CellData cell = room.GetNearestCellToPosition(obj.LastPosition);
				Vector2 vector = cell.position.ToCenterVector2();
				AIActor boy = AIActor.Spawn(Pikitospawn.gameObject.GetComponent<AIActor>(), cell.position, room, true, AIActor.AwakenAnimationType.Awaken);
				boy.IgnoreForRoomClear = true;
				boy.HitByEnemyBullets = true;
				boy.IsNormalEnemy = false;
				boy.CanTargetPlayers = false;
				boy.IsWorthShootingAt = true;
				boy.isPassable = true;
				boy.AssignedCurrencyToDrop = 0;
				boy.PreventAutoKillOnBossDeath = true;
				boy.CompanionOwner = this.parentOwner;
				CompanionController companionController = boy.gameObject.GetOrAddComponent<CompanionController>();
				companionController.CanInterceptBullets = true;
				companionController.Initialize(parentOwner);
				boy.gameObject.GetOrAddComponent<AiactorSpecialStates>().PikiOverrideTarget = SavedOverrideTarget;
				
				
			}

		}

		private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
		{
			if(arg2.aiActor != null)
            {
				SavedOverrideTarget = arg2.aiActor;
            }
		}


		private void Update()
		{
			
		}


		public AIActor SavedOverrideTarget = null;
		public Gun gun;
		public GameObject Pikitospawn = null;
		private Projectile Projectile;
		private PlayerController parentOwner;
		
	}
}