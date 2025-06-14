using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemAPI;
using UnityEngine;
using Dungeonator;
using HarmonyLib;
using System.Collections;

namespace Knives
{
	class PlayerPauseReloadController : MonoBehaviour
	{
		
	
		public void Start()
		{
			this.player = base.GetComponent<PlayerController>();
			
		}

		public void PauseReloadForDuration(Gun gun, float Duration)
        {
			if(this.player != null)
            {
                if(gun != null)
                {
                    pausedGun = gun;
                    internalduration = Duration;
                    PauseReload = true;
                }
            }
        }

		public void Update()
        {
            if(this.player != null)
            {
                if(pausedGun == player.CurrentGun)
                {
                    if (pausedGun.IsReloading)
                    {
                        if(internalduration >= 0)
                        {
                            internalduration -= BraveTime.DeltaTime;
                        }
                        else
                        {
                            CleanupReloadPause();
                        }
                    }
                    else
                    {
                        CleanupReloadPause();
                    }
                }
                else
                {
                    CleanupReloadPause();
                }
            }
            else
            {
                CleanupReloadPause();
                // if this one ever happens were in deep trouble..
            }
        }

        public void CleanupReloadPause()
        {
            PauseReload = false;
            internalduration = 0;
            pausedGun = null;
           
        }

        public float internalduration = 0;
        public Gun pausedGun = null;
		public static bool PauseReload = false;	
		public PlayerController player;

        [HarmonyPatch(typeof(Gun), nameof(Gun.HandleReload))]
        public class StopReload
        {
            [HarmonyPostfix]
            public static IEnumerator pauseReload(IEnumerator orig)
            {
                
                while (true)
                {
                    if (PlayerPauseReloadController.PauseReload)
                    {
                        // Do nothing this frame.
                        yield return null;

                        // Skip the original enumerator logic for this frame.
                        continue;
                    }
                    else
                    {
                        // Progress the original enumerator by one frame.
                        var shouldContinue = orig.MoveNext();

                        // If the original enumerator is finished, stop this enumerator.
                        if (!shouldContinue)
                            break;

                        // Otherwise, return the original enumerator's return value for this frame.
                        yield return orig.Current;
                    }
                }
            }

            [HarmonyPatch(typeof(Gun), nameof(Gun.Reload))]
            [HarmonyILManipulator]
            public static void ReloadCompatibility_SpaghettiMonsterFix() // For some reason without this the HandleReload patch simply doesnt work. 
            {
                //emotional support code do not remove   :)
            }
        }


        // Special API helped me with these because he's awesome
        [HarmonyPatch(typeof(GameUIReloadBarController), nameof(GameUIReloadBarController.HandlePlayerReloadBar))]
        public class StopBar
        {
            [HarmonyPostfix]
            public static IEnumerator PauseReloadBar_Postfix(IEnumerator orig, GameUIReloadBarController __instance)
            {
                while (true)
                {
                    if (PlayerPauseReloadController.PauseReload)
                    {
                        // Do nothing this frame.
                        yield return null;

                        // Skip the original enumerator logic for this frame.
                        continue;
                    }
                    else
                    {
                        // Progress the original enumerator by one frame.
                        var shouldContinue = orig.MoveNext();

                        // If the original enumerator is finished, stop this enumerator.
                        if (!shouldContinue)
                            break;

                        // Otherwise, return the original enumerator's return value for this frame.
                        yield return orig.Current;
                    }
                }
            }

        }

    }
}
