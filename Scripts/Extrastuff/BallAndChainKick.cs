using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using UnityEngine;
using ItemAPI;
using System.Text;
using System.Collections;

namespace Knives
{

	public class KickProj : BraveBehaviour, IPlayerInteractable
	{
		private Projectile Projectile;
		private void Start()
		{
			this.Projectile = base.gameObject.GetComponent<Projectile>();
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
		private RoomHandler curRoom;
		private void Update()
		{
			bool flag = this.Projectile;
			if (flag)
			{
				bool flag2 = this.curRoom != (this.Projectile.Owner as PlayerController).CurrentRoom;
				if (flag2)
				{
					bool flag3 = this.curRoom != null;
					if (flag3)
					{
						this.ReAssign(this.curRoom, (this.Projectile.Owner as PlayerController).CurrentRoom);
					}
					else
					{
						(this.Projectile.Owner as PlayerController).CurrentRoom.RegisterInteractable(this);
					}
					this.curRoom = (this.Projectile.Owner as PlayerController).CurrentRoom;
				}

			}
		}
		private void ReAssign(RoomHandler oldRoom, RoomHandler newRoom)
		{
			oldRoom.DeregisterInteractable(this);
			newRoom.RegisterInteractable(this);
		}


		public void OnEnteredRange(PlayerController interactor)
		{
			SpriteOutlineManager.AddOutlineToSprite(Projectile.sprite, Color.white);
		}


		public void OnExitRange(PlayerController interactor)
		{
			SpriteOutlineManager.RemoveOutlineFromSprite(Projectile.sprite, true);
		}

		public bool kickOncooldown = false;
		public void Interact(PlayerController interactor)
		{
			LastInteractor = interactor;
			if (kickOncooldown == false)
            {
                if (interactor.IsInCombat)
                {
					StartCoroutine(DoKick(interactor));
				}
                else
                {
					if (interactor.PlayerHasActiveSynergy("Bow Wow"))
					{
						StartCoroutine(DoPet(interactor));
					}
                    else
                    {
						StartCoroutine(DoKick(interactor));
					}
                }
			}

			
		}

        private IEnumerator DoPet(PlayerController interactor)
        {
            while (Key(GungeonActions.GungeonActionType.Interact))
            {
				projectile.spriteAnimator.Play("Chomp_Pet");
				
				interactor.CurrentGun.HandleAimRotation(projectile.LastPosition);
				interactor.ToggleGunRenderers(false, "petting");
				interactor.ToggleHandRenderers(false, "petting");
				interactor.spriteAnimator.Play("pet");
				
				yield return null;
            }
			interactor.ToggleGunRenderers(true, "petting");
			interactor.ToggleHandRenderers(true, "petting");
			projectile.spriteAnimator.Play("idle");
			interactor.spriteAnimator.Play("idle");
			
		}

        public Projectile chain1;
		public Projectile chain2;
        private IEnumerator DoKick(PlayerController interactor)
        {
			kickOncooldown = true;
			Vector2 vector = interactor.unadjustedAimPoint.XY() - interactor.CenterPosition;
			AkSoundEngine.PostEvent("Play_BOSS_Punchout_Punch_Hit_01", Projectile.gameObject);
			Projectile.Speed = 14;
			Projectile.SendInDirection(vector, true);
            if (chain1 != null)
            {
				chain1.Speed = 9;
				chain1.SendInDirection(vector, true);
			}
			if (chain2 != null)
			{
				chain2.Speed = 7;
				chain2.SendInDirection(vector, true);
			}

			BallAndChainMod.IgnoreingReturn = true;
			yield return new WaitForSeconds(.15f);
			Projectile.Owner.knockbackDoer.ApplyKnockback(vector, 10, true);
			yield return new WaitForSeconds(.15f);
			BallAndChainMod.IgnoreingReturn = false;

			Projectile.Speed = 1;
			Projectile.SendInDirection(-vector, true);
			if (chain1 != null)
			{
				chain1.Speed = 1;
				chain1.SendInDirection(-vector, true);
			}
			if (chain2 != null)
			{
				chain2.Speed = 1;
				chain2.SendInDirection(-vector, true);
			}

			yield return new WaitForSeconds(.4f);
			kickOncooldown = false;
		}

       
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

		public bool Key(GungeonActions.GungeonActionType action)
		{
			if (!GameManager.Instance.IsLoadingLevel && (LastInteractor) != null)
			{
				return BraveInput.GetInstanceForPlayer(LastInteractor.PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
			}
			else
			{
				return false;
			}

		}

		public PlayerController LastInteractor = null;
		private RoomHandler m_room;
	}

}