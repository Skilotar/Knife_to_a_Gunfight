using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemAPI;
using UnityEngine;
using Dungeonator;
using System.Collections;

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

				skilotar_.gameActor.StartCoroutine(this.LerpToSize(skilotar_));
				blob2.gameActor.StartCoroutine(this.LerpToSize(blob2));

                //blob2.OverrideDisplayName = "Skilotar_";
            }
            
        }
		
		// Token: 0x06000CF7 RID: 3319 RVA: 0x000A9DB3 File Offset: 0x000A7FB3
		private IEnumerator LerpToSize(AIActor target)
		{
			target.RemoveEffect(BlobuCrownController.BlobDummyEffect);
            float height = target.specRigidbody.PixelColliders[0].ManualHeight;
            float width = target.specRigidbody.PixelColliders[0].ManualWidth;
            float elapsed = 0f;
			Vector2 startScale = target.EnemyScale;
			int cachedLayer = target.gameObject.layer;
			target.gameObject.layer = LayerMask.NameToLayer("Unpixelated");
			int cachedOutlineLayer = SpriteOutlineManager.ChangeOutlineLayer(target.sprite, LayerMask.NameToLayer("Unpixelated"));
			while (elapsed < 1f)
			{
				elapsed += target.LocalDeltaTime;
                target.EnemyScale = startScale * Mathf.Lerp(1, .6f, elapsed / 1f);
				
				yield return null;
			}
			 //Stupid frickin player collider hitboxes dont scale with the enemy so Screw it we'll just turn them off
			 // Im so sick of fighting this code     >:(
			target.specRigidbody.ForceAlwaysUpdate = true;
			target.specRigidbody.AxialScale *= .6f;
			target.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerCollider,CollisionLayer.PlayerBlocker,CollisionLayer.PlayerHitBox));
			PixelCollider pixel = target.specRigidbody.GetPixelCollider(ColliderType.HitBox);
			pixel.Regenerate(target.specRigidbody.transform, false, true);
			
			yield break;
		}

		public bool Toggle = false;
		public AIActor skilotar_;
	}
}
