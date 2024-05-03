using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Knives
{
    public class StaticStatusEffects
    {
        //---------------------------------------BASEGAME STATUS EFFECTS
        //Fires
        public static GameActorFireEffect hotLeadEffect = PickupObjectDatabase.GetById(295).GetComponent<BulletStatusEffectItem>().FireModifierEffect;
        public static GameActorFireEffect greenFireEffect = PickupObjectDatabase.GetById(706).GetComponent<Gun>().DefaultModule.projectiles[0].fireEffect;


        //Freezes
        public static GameActorFreezeEffect frostBulletsEffect = PickupObjectDatabase.GetById(278).GetComponent<BulletStatusEffectItem>().FreezeModifierEffect;
        public static GameActorFreezeEffect chaosBulletsFreeze = PickupObjectDatabase.GetById(569).GetComponent<ChaosBulletsItem>().FreezeModifierEffect;

        //Poisons
        public static GameActorHealthEffect irradiatedLeadEffect = PickupObjectDatabase.GetById(204).GetComponent<BulletStatusEffectItem>().HealthModifierEffect;

        //Charms
        public static GameActorCharmEffect charmingRoundsEffect = PickupObjectDatabase.GetById(527).GetComponent<BulletStatusEffectItem>().CharmModifierEffect;
        public static GameActorillnessEffect ill;
        //Cheeses

        //Speed Changes
        public static GameActorSpeedEffect tripleCrossbowSlowEffect = (PickupObjectDatabase.GetById(381) as Gun).DefaultModule.projectiles[0].speedEffect;


        //----------------------------------------CUSTOM STATUS EFFECTS
        public static GameActorSpeedEffect FriendlyWebGoopSpeedMod;

        //Plague Effects
        public static GameActorillnessEffect StandardillnessEffect;
        public static void InitCustomEffects()
        {
            FriendlyWebGoopSpeedMod = new GameActorSpeedEffect
            {
                duration = 1,
                TintColor = tripleCrossbowSlowEffect.TintColor,
                DeathTintColor = tripleCrossbowSlowEffect.DeathTintColor,
                effectIdentifier = "FriendlyWebSlow",
                AppliesTint = false,
                AppliesDeathTint = false,
                resistanceType = EffectResistanceType.None,
                SpeedMultiplier = 0.40f,

                //Eh
                OverheadVFX = null,
                AffectsEnemies = true,
                AffectsPlayers = false,
                AppliesOutlineTint = false,
                OutlineTintColor = tripleCrossbowSlowEffect.OutlineTintColor,
                PlaysVFXOnActor = false,
            };

            ill = new GameActorillnessEffect
            {
                DamagePerSecondToEnemies = 0f,
                effectIdentifier = "illness",
                AffectsEnemies = true,
                resistanceType = EffectResistanceType.Poison,
                duration = 5f,
                TintColor = new Color(0.46f, 0.59f, 0.13f),
                AppliesTint = true,
                AppliesDeathTint = true,
                PlaysVFXOnActor = true,
                OverheadVFX = GameActorillnessEffect.Illness,
                maxStackedDuration = 1,

                stackMode = GameActorEffect.EffectStackingMode.Stack
            };
        }   
    }
}
