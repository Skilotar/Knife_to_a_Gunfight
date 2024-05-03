using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemAPI;
using UnityEngine;
using Dungeonator;

namespace Knives
{
	class SkilotarSplitBehave : MonoBehaviour
	{
		
		private void Start()
		{
			this.skilotar_ = base.GetComponent<AIActor>();
		}
		System.Random rng = new System.Random();

		private void Update()
		{
            if (skilotar_.healthHaver.GetCurrentHealth() <= (skilotar_.healthHaver.GetMaxHealth() / 3) && Toggle == false)
            {
                skilotar_.LocalTimeScale = 1;

                Toggle = true;
                string guid;
                guid = "1b5810fafbec445d89921a4efb4e42b7";
                PlayerController owner = GameManager.Instance.PrimaryPlayer;
                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
                IntVector2? intVector = new IntVector2?(owner.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
                AIActor blob2 = AIActor.Spawn(orLoadByGuid.aiActor, skilotar_.Position, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Default, true);
                blob2.CanTargetEnemies = false;
                blob2.CanTargetPlayers = true;
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(blob2.specRigidbody, null, false);
                blob2.gameObject.AddComponent<KillOnRoomClear>();
                blob2.IsHarmlessEnemy = false;
                blob2.IgnoreForRoomClear = false;
                blob2.healthHaver.SetHealthMaximum(blob2.healthHaver.GetMaxHealth() / 3);
                blob2.reinforceType = AIActor.ReinforceType.Instant;
                blob2.HandleReinforcementFallIntoRoom(0f);
                //blob2.OverrideDisplayName = "Skilotar_";
            }
            
        }
        public bool Toggle = false;
		public AIActor skilotar_;
	}
}
