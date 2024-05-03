using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using UnityEngine;
using System.Text;

namespace Knives
{
    
	public class TurretController : BraveBehaviour, IPlayerInteractable
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
			//custom mount sfx here

			//mounting
			if(isOnTurret == false && !interactor.IsDodgeRolling)
            {
				isOnTurret = true;
				AkSoundEngine.PostEvent("Play_tur_mount", base.gameObject);
				AkSoundEngine.PostEvent("Play_tur_mount", base.gameObject);
				AkSoundEngine.PostEvent("Play_tur_mount", base.gameObject);
				AkSoundEngine.PostEvent("Play_tur_mount", base.gameObject);
				interactor.specRigidbody.Position = new Position(base.sprite.WorldCenter - new Vector2(.7f, 0.3f));
				SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
				SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
				
				
			}
            else
            {
				isOnTurret = false;
				SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
				SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white);
			}
			

			//remove movement


			//sprun-like gunswap


			


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
			result = 3.5f;
			return result;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000054B0 File Offset: 0x000036B0
		public override void  OnDestroy()
		{
			base.OnDestroy();
		}

		// Token: 0x04000013 RID: 19
		private RoomHandler m_room;
	}
	
}

