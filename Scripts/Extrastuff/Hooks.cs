using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using UnityEngine;
using Dungeonator;
using SaveAPI;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;

namespace Knives
{
    // Token: 0x02000018 RID: 24
    public static class Hooks
    {
        // Token: 0x060000B7 RID: 183 RVA: 0x00008CE4 File Offset: 0x00006EE4
        public static void Init()
        {
            try
            {


                Hook SetupUnlockTrackerComponent = new Hook(typeof(PlayerController).GetMethod("DoSpinfallSpawn", BindingFlags.Instance | BindingFlags.Public), typeof(Module).GetMethod("RunStartHook"));

                Hook togglemanichook = new Hook(typeof(PlayerController).GetMethod("DoSpinfallSpawn", BindingFlags.Instance | BindingFlags.Public), typeof(TheatreModeToggle).GetMethod("startmanic"));
                //used for unlock
                Hook impressHook = new Hook(typeof(PlayerController).GetMethod("BraveOnLevelWasLoaded", BindingFlags.Instance | BindingFlags.Public), typeof(Module).GetMethod("SpecialTutorialHook"));

                Hook ReinforceAction = new Hook(typeof(RoomHandler).GetMethod("TriggerReinforcementLayer", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("TriggerReinforcementLayerHook"));





            }
            catch (Exception e)
            {
                ItemAPI.Tools.PrintException(e, "FF0000");
            }
        }
        public static bool TriggerReinforcementLayerHook(Func<RoomHandler, int, bool, bool, int, int, bool, bool> orig, RoomHandler self, int index, bool removeLayer = true, bool disableDrops = false, int specifyObjectIndex = -1, int specifyObjectCount = -1, bool instant = false)
        {
            if (OnReinforcementWave != null) { OnReinforcementWave(self); }
            return orig(self, index, removeLayer, disableDrops, specifyObjectIndex, specifyObjectCount, instant);
        }
        public static System.Action<RoomHandler> OnReinforcementWave;
    }
}
