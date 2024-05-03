using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Knives
{
    class StatusEffectHelper
    {

        public static GameActorillnessEffect GenerateillnessEffect(float duration, float dps, bool tintEnemy, Color bodyTint, bool tintCorpse, Color corpseTint)
        {
            GameActorillnessEffect commonillness = new GameActorillnessEffect
            {
                duration = 10,
                effectIdentifier = "illness",
                resistanceType = EffectResistanceType.None,
                DamagePerSecondToEnemies = 4f,
                ignitesGoops = false,
                AffectsEnemies = true,
                AffectsPlayers = false,
                AppliesOutlineTint = false,
                PlaysVFXOnActor = false,
                AppliesTint = tintEnemy,
                AppliesDeathTint = tintCorpse,
                TintColor = bodyTint,
                DeathTintColor = corpseTint,
                stackMode = GameActorEffect.EffectStackingMode.Refresh,

            };
            return commonillness;
        }

        public class StatusEffectBulletMod : MonoBehaviour
        {
            public StatusEffectBulletMod()
            {
                pickRandom = false;
            }
            private void Start()
            {
                self = base.GetComponent<Projectile>();
                if (pickRandom)
                {
                    List<StatusData> validStatuses = new List<StatusData>();
                    foreach (StatusData data in datasToApply)
                    {
                        if (UnityEngine.Random.value <= data.applyChance)
                        {
                            validStatuses.Add(data);
                        }
                    }
                    if (validStatuses.Count() > 0)
                    {
                        StatusData selectedStatus = BraveUtility.RandomElement(validStatuses);
                        if (selectedStatus.applyTint) self.AdjustPlayerProjectileTint(selectedStatus.effectTint, 1);
                        self.statusEffectsToApply.Add(selectedStatus.effect);
                    }
                }
                else
                {
                    foreach (StatusData data in datasToApply)
                    {
                        if (UnityEngine.Random.value <= data.applyChance)
                        {
                            if (data.applyTint) self.AdjustPlayerProjectileTint(data.effectTint, 1);
                            self.statusEffectsToApply.Add(data.effect);
                        }
                    }
                }
            }

            private Projectile self;
            public List<StatusData> datasToApply = new List<StatusData>();
            public bool pickRandom;
            public class StatusData
            {
                public GameActorEffect effect;
                public float applyChance;
                public Color effectTint;
                public bool applyTint = false;
            }
        }
    }
}
