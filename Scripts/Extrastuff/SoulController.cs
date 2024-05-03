using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using UnityEngine;
using System.Text;

namespace Knives
{

	public class SoulController : BraveBehaviour, IPlayerInteractable
	{
		// Token: 0x0600005F RID: 95 RVA: 0x000051D9 File Offset: 0x000033D9
		private void Start()
		{
			this.m_room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Round));
			this.m_room.RegisterInteractable(this);

		}

		

		// Token: 0x06000060 RID: 96 RVA: 0x00005214 File Offset: 0x00003414
		public float GetDistanceToPoint(Vector2 point)
		{
			bool flag = !base.sprite;
			float result;
			if (flag)
			{
				result = float.MaxValue;
			}
			else
			{
				Bounds bounds = base.sprite.GetBounds();
				bounds.SetMinMax(bounds.min + base.transform.position, bounds.max + base.transform.position);
				float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
				float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
				result = Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2));
			}
			return result;
		}


		public void OnEnteredRange(PlayerController interactor)
		{
			SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
			SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white);
		}


		public void OnExitRange(PlayerController interactor)
		{
			SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
			SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
		}


		public void Interact(PlayerController interactor)
		{
			if(interactor.CurrentRoom != null )
            {
				string guid = GetGuid();
				AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
				IntVector2? intVector = new IntVector2?(interactor.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
				AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, this.sprite.WorldBottomCenter, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Default, true);
				aiactor.CanTargetEnemies = true;
				aiactor.CanTargetPlayers = false;
				PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
				aiactor.gameObject.AddComponent<KillOnRoomClear>();
				aiactor.IsHarmlessEnemy = true;
				aiactor.CompanionOwner = interactor;
				aiactor.CanDropCurrency = false;
				aiactor.IsNormalEnemy = true;
				aiactor.HitByEnemyBullets = true;
				aiactor.isPassable = true;
				aiactor.IgnoreForRoomClear = true;
				aiactor.reinforceType = AIActor.ReinforceType.Instant;
				aiactor.HandleReinforcementFallIntoRoom(0f);
				aiactor.gameObject.GetOrAddComponent<AiactorSpecialStates>().Rez = true;
				aiactor.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(aiactor.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));
				if (Evil)
                {
					aiactor.BecomeBlackPhantom();
					aiactor.MovementSpeed *= 2f;
					aiActor.projectile.baseData.damage *= 2f;
                }
				AkSoundEngine.PostEvent("Play_ENM_jammer_curse_01", base.gameObject);

				UnityEngine.GameObject.Destroy(base.gameObject);

			}

		}
		private void Update()
		{
			if (GameManager.Instance.PrimaryPlayer.IsInCombat == false)
            {
				UnityEngine.GameObject.Destroy(base.gameObject);
			}
		}


		private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
			if (otherRigidbody)
			{
				if (otherRigidbody == myRigidbody.aiActor.CompanionOwner)
				{
					PhysicsEngine.SkipCollision = true;
				}

			}
		}

		private string GetGuid()
		{
			int rng = UnityEngine.Random.Range(0, necro.Count - 1);

			return necro[rng];
		}

		List<string> necro = new List<string>()
        {
			

			"cf27dd464a504a428d87a8b2560ad40a",// Stoner
			"4db03291a12144d69fe940d5a01de376",// Hollow

            "336190e29e8a4f75ab7486595b700d4a",// Skullmet
			"336190e29e8a4f75ab7486595b700d4a",

            "336190e29e8a4f75ab7486595b700d4a",// Skullet
			"336190e29e8a4f75ab7486595b700d4a",
			"336190e29e8a4f75ab7486595b700d4a",
			"336190e29e8a4f75ab7486595b700d4a",
			"336190e29e8a4f75ab7486595b700d4a",

		};
        public bool Evil = false;

		public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
		{
			shouldBeFlipped = false;
			return string.Empty;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00005478 File Offset: 0x00003678
		public float GetOverrideMaxDistance()
		{
			float result;
			result = 3f;
			return result;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000054B0 File Offset: 0x000036B0
		public override void OnDestroy()
		{
			base.OnDestroy();
		}

		// Token: 0x04000013 RID: 19
		private RoomHandler m_room;
	}

}
