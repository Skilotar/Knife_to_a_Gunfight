using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Knives
{
    public static class Explosions
    {
        public static ExplosionData DefaultSmall =
            GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
        public static ExplosionData DefaultLarge =
            GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;
        public static ExplosionData AirStrike =
            PickupObjectDatabase.GetById(252).GetComponent<DirectionalAttackActiveItem>().barrageExplosionData;
        public static ExplosionData ExplosiveRounds =
            PickupObjectDatabase.GetById(304).GetComponent<ComplexProjectileModifier>().ExplosionData;
        public static ExplosionData FlareGun =
            PickupObjectDatabase.GetById(275).GetComponent<BarrageSynergyProcessor>().Barrage.barrageExplosionData;
        public static ExplosionData MeltedRock =
            PickupObjectDatabase.GetById(403).GetComponent<CorpseExplodeActiveItem>().CorpseExplosionData;
       
    }
}
