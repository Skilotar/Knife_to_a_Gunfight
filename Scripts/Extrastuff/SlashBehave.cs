using System.Runtime.InteropServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Knives
{
	// Token: 0x02000018 RID: 24
	public class ProjectileSlashingBehaviour : MonoBehaviour
	{
		
		public event ProjectileSlashingBehaviour.PostProcessSlashHandler PostProcessSlash;

		
		public event ProjectileSlashingBehaviour.OnSlashHit OnSlashHitEnemy;

		
		public event ProjectileSlashingBehaviour.OnSlashEnded OnSlashEnd;

		
		public event ProjectileSlashingBehaviour.OnHitFirstEnemy OnFirstEnemyHit;

		// Token: 0x060000A5 RID: 165 RVA: 0x000068A8 File Offset: 0x00004AA8
		public ProjectileSlashingBehaviour()
		{
			this.DestroyBaseAfterFirstSlash = true;
			this.timeBetweenSlashes = 1f;
			this.DoSound = true;
			this.slashKnockback = 5f;
			this.SlashDamage = 15f;
			this.SlashBossMult = 1f;
			this.SlashJammedMult = 1f;
			this.playerKnockback = 1f;
			this.SlashDamageUsesBaseProjectileDamage = true;
			this.InteractMode = SlashDoer.ProjInteractMode.IGNORE;
			this.SlashDimensions = 90f;
			this.SlashRange = 2.5f;
			this.SlashVFX = (ETGMod.Databases.Items["wonderboy"] as Gun).muzzleFlashEffects;
			this.soundToPlay = "Play_WPN_blasphemy_shot_01";
			this.DoesMultipleSlashes = false;
			this.UsesAngleVariance = false;
			this.MinSlashAngleOffset = 1f;
			this.MaxSlashAngleOffset = 4f;
			this.delayBeforeSlash = 0f;
			this.AppliesStun = false;
			this.StunApplyChance = 0f;
			this.StunTime = 0f;
			this.NoCost = false;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000069B4 File Offset: 0x00004BB4
		private void Start()
		{
			this.m_projectile = base.GetComponent<Projectile>();
			bool flag = this.m_projectile.Owner && this.m_projectile.Owner is PlayerController;
			if (flag)
			{
				this.owner = (this.m_projectile.Owner as PlayerController);
			}
			this.m_projectile.sprite.renderer.enabled = false;
			this.m_projectile.collidesWithEnemies = false;
			this.m_projectile.UpdateCollisionMask();
			Projectile projectile = this.m_projectile;
			this.effects.AddRange(projectile.statusEffectsToApply);
			bool flag2 = projectile.AppliesFire && UnityEngine.Random.value <= projectile.FireApplyChance;
			if (flag2)
			{
				this.effects.Add(projectile.fireEffect);
			}
			bool flag3 = projectile.AppliesCharm && UnityEngine.Random.value <= projectile.CharmApplyChance;
			if (flag3)
			{
				this.effects.Add(projectile.charmEffect);
			}
			bool flag4 = projectile.AppliesCheese && UnityEngine.Random.value <= projectile.CheeseApplyChance;
			if (flag4)
			{
				this.effects.Add(projectile.cheeseEffect);
			}
			bool flag5 = projectile.AppliesBleed && UnityEngine.Random.value <= projectile.BleedApplyChance;
			if (flag5)
			{
				this.effects.Add(projectile.bleedEffect);
			}
			bool flag6 = projectile.AppliesFreeze && UnityEngine.Random.value <= projectile.FreezeApplyChance;
			if (flag6)
			{
				this.effects.Add(projectile.freezeEffect);
			}
			bool flag7 = projectile.AppliesPoison && UnityEngine.Random.value <= projectile.PoisonApplyChance;
			if (flag7)
			{
				this.effects.Add(projectile.healthEffect);
			}
			bool flag8 = projectile.AppliesSpeedModifier && UnityEngine.Random.value <= projectile.SpeedApplyChance;
			if (flag8)
			{
				this.effects.Add(projectile.speedEffect);
			}
			bool flag9 = this.m_projectile;
			if (flag9)
			{
				bool flag10 = this.doSpinAttack;
				if (flag10)
				{
					this.DestroyBaseAfterFirstSlash = false;
					this.m_projectile.StartCoroutine(this.DoSlash(90f, 0.15f + this.delayBeforeSlash));
					this.m_projectile.StartCoroutine(this.DoSlash(180f, 0.3f + this.delayBeforeSlash));
					this.m_projectile.StartCoroutine(this.DoSlash(-90f, 0.45f + this.delayBeforeSlash));
					base.Invoke("Suicide", 0.01f);
				}
				else
				{
					bool doesMultipleSlashes = this.DoesMultipleSlashes;
					if (doesMultipleSlashes)
					{
						this.m_projectile.StartCoroutine(this.DoMultiSlash(0f, this.delayBeforeSlash, this.AmountOfMultiSlashes, this.DelayBetweenMultiSlashes));
					}
					else
					{
						this.m_projectile.StartCoroutine(this.DoSlash(0f, 0f + this.delayBeforeSlash));
					}
				}
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00006CC1 File Offset: 0x00004EC1
		private void Update()
		{
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00006CC4 File Offset: 0x00004EC4
		private IEnumerator DoSlash(float angle, float delay)
		{
			yield return new WaitForSeconds(delay);
			if (NoCost == true)
            {
				Projectile proj = base.gameObject.GetComponent<Projectile>();
				Gun gun = proj.PossibleSourceGun;
				gun.GainAmmo(1);
				gun.MoveBulletsIntoClip(1);

			}
			bool slashDamageUsesBaseProjectileDamage = this.SlashDamageUsesBaseProjectileDamage;
			if (slashDamageUsesBaseProjectileDamage)
			{
				this.SlashDamage = this.m_projectile.baseData.damage;
				this.SlashBossMult = this.m_projectile.BossDamageMultiplier;
				this.SlashJammedMult = this.m_projectile.BlackPhantomDamageMultiplier;
				this.slashKnockback = this.m_projectile.baseData.force;
			}
			bool usesAngleVariance = this.UsesAngleVariance;
			if (usesAngleVariance)
			{
				angle += UnityEngine.Random.Range(this.MinSlashAngleOffset, this.MaxSlashAngleOffset);
			}
			ProjectileSlashingBehaviour slash = base.GetComponent<ProjectileSlashingBehaviour>();
			bool flag = slash != null;
			if (flag)
			{
				bool flag2 = this.PostProcessSlash != null;
				if (flag2)
				{
					this.PostProcessSlash(this.owner, slash);
				}
			}
			SlashDoer.GrabBoolsAndValuesAndShitForTheFuckingSlashingApplyEffect(this.AppliesStun, this.StunApplyChance, this.StunTime,this.DeflectionDegree);
			SlashDoer.DoSwordSlash(this.m_projectile.specRigidbody.UnitCenter, this.m_projectile.Direction.ToAngle() + angle, this.owner, this.playerKnockback, this.InteractMode, this.SlashDamage, this.slashKnockback, this.effects, null, this.SlashJammedMult, this.SlashBossMult, this.SlashRange, this.SlashDimensions, this.m_projectile);
			bool doSound = this.DoSound;
			if (doSound)
			{
				AkSoundEngine.PostEvent(this.soundToPlay, this.m_projectile.gameObject);
			}
			this.SlashVFX.SpawnAtPosition(this.m_projectile.specRigidbody.UnitCenter, this.m_projectile.Direction.ToAngle() + angle, null, null, null, new float?(-0.05f), false, null, null, false);
			bool destroyBaseAfterFirstSlash = this.DestroyBaseAfterFirstSlash;
			if (destroyBaseAfterFirstSlash)
			{
				this.Suicide();
			}
			yield break;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00006CE4 File Offset: 0x00004EE4
		public void DoOnHitEffects(AIActor enemy, Vector2 forceDir)
		{
			bool flag = this.OnSlashHitEnemy != null;
			if (flag)
			{
				this.OnSlashHitEnemy(this.owner, enemy, forceDir);
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00006D18 File Offset: 0x00004F18
		public void DoOnSlashEndEffects(bool hitEnemies)
		{
			bool flag = this.OnSlashEnd != null;
			if (flag)
			{
				this.OnSlashEnd(hitEnemies, this.owner);
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00006D48 File Offset: 0x00004F48
		public void DoOnHitFirstEnemyEffects(AIActor enemy)
		{
			bool flag = this.OnFirstEnemyHit != null;
			if (flag)
			{
				this.OnFirstEnemyHit(this.owner, enemy);
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00006D78 File Offset: 0x00004F78
		private IEnumerator DoMultiSlash(float angle, float delay, int AmountOfMultiSlashes, float DelayBetweenMultiSlashes)
		{
			yield return new WaitForSeconds(delay);
			int num;
			for (int i = 0; i < AmountOfMultiSlashes; i = num + 1)
			{
				bool slashDamageUsesBaseProjectileDamage = this.SlashDamageUsesBaseProjectileDamage;
				if (slashDamageUsesBaseProjectileDamage)
				{
					this.SlashDamage = this.m_projectile.baseData.damage;
					this.SlashBossMult = this.m_projectile.BossDamageMultiplier;
					this.SlashJammedMult = this.m_projectile.BlackPhantomDamageMultiplier;
					this.slashKnockback = this.m_projectile.baseData.force;
				}
				bool usesAngleVariance = this.UsesAngleVariance;
				if (usesAngleVariance)
				{
					angle += UnityEngine.Random.Range(this.MinSlashAngleOffset, this.MaxSlashAngleOffset);
				}
				ProjectileSlashingBehaviour slash = base.GetComponent<ProjectileSlashingBehaviour>();
				bool flag = slash != null;
				if (flag)
				{
					bool flag2 = this.PostProcessSlash != null;
					if (flag2)
					{
						this.PostProcessSlash(this.m_projectile.Owner as PlayerController, slash);
					}
				}
				SlashDoer.GrabBoolsAndValuesAndShitForTheFuckingSlashingApplyEffect(this.AppliesStun, this.StunApplyChance, this.StunTime,this.DeflectionDegree);
				SlashDoer.DoSwordSlash(this.m_projectile.specRigidbody.UnitCenter, this.m_projectile.Direction.ToAngle() + angle, this.owner, this.playerKnockback, this.InteractMode, this.SlashDamage, this.slashKnockback, this.m_projectile.statusEffectsToApply, null, this.SlashJammedMult, this.SlashBossMult, this.SlashRange, this.SlashDimensions, null);
				bool doSound = this.DoSound;
				if (doSound)
				{
					AkSoundEngine.PostEvent(this.soundToPlay, this.m_projectile.gameObject);
				}
				this.SlashVFX.SpawnAtPosition(this.m_projectile.specRigidbody.UnitCenter, this.m_projectile.Direction.ToAngle() + angle, null, null, null, new float?(-0.05f), false, null, null, false);
				yield return new WaitForSeconds(DelayBetweenMultiSlashes);
				slash = null;
				num = i;
			}
			this.Suicide();
			yield break;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00006DA4 File Offset: 0x00004FA4
		private void ApplyOnHitEffects()
		{
			this.m_projectile = base.GetComponent<Projectile>();
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00006DB3 File Offset: 0x00004FB3
		private void Suicide()
		{
			UnityEngine.Object.Destroy(this.m_projectile.gameObject);
		}

		// Token: 0x0400007C RID: 124
		public List<GameActorEffect> effects = new List<GameActorEffect>();

		// Token: 0x0400007D RID: 125
		private float timer;

		// Token: 0x0400007E RID: 126
		public string soundToPlay;

		// Token: 0x0400007F RID: 127
		public float delayBeforeSlash;

		// Token: 0x04000080 RID: 128
		public VFXPool SlashVFX;

		// Token: 0x04000081 RID: 129
		public float timeBetweenSlashes;

		// Token: 0x04000082 RID: 130
		public bool doSpinAttack;

		// Token: 0x04000083 RID: 131
		public float playerKnockback;

		// Token: 0x04000084 RID: 132
		public float slashKnockback;

		// Token: 0x04000085 RID: 133
		public bool DoSound;

		// Token: 0x04000086 RID: 134
		public float SlashJammedMult;

		// Token: 0x04000087 RID: 135
		public float SlashBossMult;

		// Token: 0x04000088 RID: 136
		public float SlashDamage;

		// Token: 0x04000089 RID: 137
		public float SlashRange;

		// Token: 0x0400008A RID: 138
		public float SlashDimensions;

		// Token: 0x0400008B RID: 139
		public bool SlashDamageUsesBaseProjectileDamage;

		// Token: 0x0400008C RID: 140
		public SlashDoer.ProjInteractMode InteractMode;

		// Token: 0x0400008D RID: 141
		public bool DestroyBaseAfterFirstSlash;

		// Token: 0x0400008E RID: 142
		public bool DoesMultipleSlashes;

		// Token: 0x0400008F RID: 143
		public float MinSlashAngleOffset;

		// Token: 0x04000090 RID: 144
		public float MaxSlashAngleOffset;

		// Token: 0x04000091 RID: 145
		public bool UsesAngleVariance;

		// Token: 0x04000092 RID: 146
		public int AmountOfMultiSlashes;

		// Token: 0x04000093 RID: 147
		public float DelayBetweenMultiSlashes;

		// Token: 0x04000094 RID: 148
		public bool AppliesStun;

		// Token: 0x04000095 RID: 149
		public float StunApplyChance;

		// Token: 0x04000096 RID: 150
		public float StunTime;

		public bool NoCost;

        public float DeflectionDegree = 180f;

		// Token: 0x04000097 RID: 151
		private Projectile m_projectile;

		// Token: 0x04000098 RID: 152
		private PlayerController owner;

		// Token: 0x02000053 RID: 83
		// (Invoke) Token: 0x06000244 RID: 580
		public delegate void OnSlashHit(PlayerController player, AIActor hitEnemy, Vector2 forceDirection);

		// Token: 0x02000054 RID: 84
		// (Invoke) Token: 0x06000248 RID: 584
		public delegate void PostProcessSlashHandler(PlayerController player, ProjectileSlashingBehaviour slash);

		// Token: 0x02000055 RID: 85
		// (Invoke) Token: 0x0600024C RID: 588
		public delegate void OnSlashEnded(bool hitAnEnemy, PlayerController player);

		// Token: 0x02000056 RID: 86
		// (Invoke) Token: 0x06000250 RID: 592
		public delegate void OnHitFirstEnemy(PlayerController player, AIActor hitEnemy);
	}
}