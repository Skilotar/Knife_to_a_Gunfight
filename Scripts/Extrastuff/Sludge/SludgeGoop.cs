using Alexandria.ItemAPI;
using Alexandria.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Knives
{
    public class SludgeGoop : SpecialGoopBehaviourDoer
    {
        public static void Init()
        {
            EasyGoopDefinitions.SludgeGoop = ScriptableObject.CreateInstance<GoopDefinition>();
            EasyGoopDefinitions.SludgeGoop.CanBeIgnited = false;
            EasyGoopDefinitions.SludgeGoop.damagesEnemies = true;
            EasyGoopDefinitions.SludgeGoop.damagePerSecondtoEnemies = 1;
            EasyGoopDefinitions.SludgeGoop.damagesPlayers = false;
            EasyGoopDefinitions.SludgeGoop.baseColor32 = ExtendedColours.lime;
            EasyGoopDefinitions.SludgeGoop.goopTexture = GoopUtility.PoisonDef.goopTexture;
            EasyGoopDefinitions.SludgeGoop.usesLifespan = true;
            EasyGoopDefinitions.SludgeGoop.lifespan = 8f;
            EasyGoopDefinitions.SludgeGoop.name = "ktgSludgegoop";
            
            EasyGoopDefinitions.SludgeGoop.goopDamageTypeInteractions = new List<GoopDefinition.GoopDamageTypeInteraction>()
            {
                new GoopDefinition.GoopDamageTypeInteraction()
                {
                    freezesGoop = false,
                    electrifiesGoop = false,
                    ignitionMode = GoopDefinition.GoopDamageTypeInteraction.GoopIgnitionMode.NONE,
                    damageType = CoreDamageTypes.None
                }
            };
            GoopUtility.RegisterComponentToGoopDefinition(EasyGoopDefinitions.SludgeGoop, typeof(SludgeGoop));
        }
        public override void DoGoopEffectUpdate(DeadlyDeadlyGoopManager goop, GameActor actor, IntVector2 position)
        {
            if (actor && actor.aiActor && actor.aiActor.HasBeenEngaged && actor.healthHaver)
            {
                actor.ApplyEffect(new GameActorSludgeEffect()
                {
                    duration = 4,
                    stackMode = GameActorEffect.EffectStackingMode.Refresh,
                    SpeedMultiplier = 0.3f,
                });
            }
            base.DoGoopEffectUpdate(goop, actor, position);
        }
    }
}