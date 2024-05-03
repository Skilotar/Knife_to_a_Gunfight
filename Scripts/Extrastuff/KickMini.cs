using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using UnityEngine;
using System.Text;
using System.Collections;

namespace Knives
{

	public class KickMini : BraveBehaviour, IPlayerInteractable
	{
		private AIActor Mini;
		private void Start()
		{
			this.Mini = base.aiActor;
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
            if (base.knockbackDoer != null)
            {
				Vector2 dir = base.aiActor.CompanionOwner.CenterPosition.CalculateVectorBetween(base.sprite.WorldCenter);
				bool flag = base.aiActor.CompanionOwner.CurrentGun;
				if (flag)
				{
					dir = base.aiActor.CompanionOwner.CurrentGun.CurrentAngle.DegreeToVector2();
				}
				bool flag2 = !this.isPunting;
				if (flag2)
				{
					base.StartCoroutine(this.HandlePunt(interactor, dir));
				}
			}



		}

		private IEnumerator HandlePunt(PlayerController punter, Vector2 dir)
		{
			float knockbackForce = 20f;
			
			this.currentPuntDir = dir;
			
			this.isPunting = true;
			
			bool flag2 = base.knockbackDoer;
			if (flag2)
			{
				base.knockbackDoer.ApplyKnockback(dir, knockbackForce, false);
			}
			else
			{
				Debug.LogError("Exception: Mini doesn't have a Knockbackdoer!");
			}
			bool flag3 = base.behaviorSpeculator;
			if (flag3)
			{
				base.behaviorSpeculator.Stun(1f, false);
			}
			else
			{
				Debug.LogError("Exception: Mini doesn't have a behaviorSpeculator!");
			}
			if(base.healthHaver != null)
            {
                if (base.healthHaver.IsAlive)
                {
					base.healthHaver.ApplyDamage(3, Vector2.zero, "Owow Don't Kick Me!");
                }
            }
			AkSoundEngine.PostEvent("Play_BOSS_Punchout_Punch_Hit_01", this.Mini.gameObject);
			yield return new WaitForSeconds(0.5f);
			this.isPunting = false;
			yield break;
		}
		private bool isPunting = false;
		private RoomHandler curRoom;
		private Vector2 currentPuntDir = Vector2.zero;
		private void Update()
		{
			bool flag = this.Mini && this.Mini.CompanionOwner;
			if (flag)
			{
				bool flag2 = this.curRoom != this.Mini.CompanionOwner.CurrentRoom;
				if (flag2)
				{
					bool flag3 = this.curRoom != null;
					if (flag3)
					{
						this.ReAssign(this.curRoom, this.Mini.CompanionOwner.CurrentRoom);
					}
					else
					{
						this.Mini.CompanionOwner.CurrentRoom.RegisterInteractable(this);
					}
					this.curRoom = this.Mini.CompanionOwner.CurrentRoom;
				}
				
			}
		}
		private void ReAssign(RoomHandler oldRoom, RoomHandler newRoom)
		{
			oldRoom.DeregisterInteractable(this);
			newRoom.RegisterInteractable(this);
		}

		public bool isOnTurret = false;

		public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
		{
			shouldBeFlipped = false;
			return string.Empty;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00005478 File Offset: 0x00003678
		public float GetOverrideMaxDistance()
		{
			float result;
			result = 2f;
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