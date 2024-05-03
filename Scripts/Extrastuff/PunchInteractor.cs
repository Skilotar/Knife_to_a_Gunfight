using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using UnityEngine;
using System.Text;
using System.Collections;

namespace Knives
{

	public class PunchInteractor : BraveBehaviour, IPlayerInteractable
	{
		private AIActor Victim;
		private void Start()
		{
			this.Victim = base.aiActor;
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
				Vector2 dir = interactor.CenterPosition.CalculateVectorBetween(base.sprite.WorldCenter);
				bool flag = interactor.CurrentGun;
				if (flag)
				{
					dir = interactor.CurrentGun.CurrentAngle.DegreeToVector2();
				}
				bool flag2 = !this.isPunting && !interactor.IsDodgeRolling;
				if (flag2)
				{
					base.StartCoroutine(this.HandlePunt(interactor, dir));
				}
			}
		}

		private IEnumerator HandlePunt(PlayerController punter, Vector2 dir)
		{
			float knockbackForce = 12f;

			this.currentPuntDir = dir;

			this.isPunting = true;

			bool flag2 = base.knockbackDoer;
			if (flag2)
			{
				base.knockbackDoer.ApplyKnockback(dir, knockbackForce, false);
			}
			else
			{
				Debug.LogError("Exception: Victim doesn't have a Knockbackdoer!");
			}
			if (base.healthHaver != null)
			{
				if (base.healthHaver.IsAlive && healthHaver.GetCurrentHealth() > 7 * punter.stats.GetStatValue(PlayerStats.StatType.Damage))
                {
                    base.healthHaver.ApplyDamage(7, Vector2.zero, "Owow Don't punch Me!");
                }
                else
                {

                    Projectile comp = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, Victim.specRigidbody.UnitCenter, punter, punter.CurrentGun, 0, 1, false);
                    comp.AdditionalScaleMultiplier *= 1.5f;
					comp.baseData.damage = 7f * punter.stats.GetStatValue(PlayerStats.StatType.Damage);

					comp.sprite.renderer.enabled = false;
                    comp.SuppressHitEffects = true;
                    comp.hitEffects.suppressMidairDeathVfx = true;
                    comp.gameObject.GetOrAddComponent<KilledEnemiesBecomeProjectileModifier>();
                }

			}
			punter.CurrentStoneGunTimer = 1f;
			AkSoundEngine.PostEvent("Play_BOSS_Punchout_Punch_Hit_01", this.Victim.gameObject);
			yield return new WaitForSeconds(1f);
			this.isPunting = false;
			yield break;
		}
		private bool isPunting = false;
		
		private Vector2 currentPuntDir = Vector2.zero;
		private void Update()
		{
			bool flag = this.Victim;
			if (flag)
			{

				
			}
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
			result = 2.5f;
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