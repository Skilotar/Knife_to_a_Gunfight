using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Gungeon;
using System.Reflection;
using Brave.BulletScript;
using System.Collections;
using MonoMod.RuntimeDetour;
using SaveAPI;

namespace Knives
{
    public class TheatreModeToggle
    {
        public static void Init()
        {



            SaveAPIManager.SetFlag(CustomDungeonFlags.THEATRETOGGLE, false);

           
        }
        
    

        public static bool theatreOn
        {
            get
            {
                return (SaveAPIManager.GetFlag(CustomDungeonFlags.THEATRETOGGLE));
            }
        }

        public static void startmanic(Action<PlayerController, float> orig, PlayerController self, float invisibleDelay)
        {
            orig(self,invisibleDelay);
            if (SaveAPIManager.GetFlag(CustomDungeonFlags.THEATRETOGGLE))
            {
                self.givemania();
            }
        }

    }
    static class Mania
    {
        public static void givemania(this PlayerController controller)
        {

            if (GameManager.Instance.PrimaryPlayer != null)
            {
                if (SaveAPIManager.GetFlag(CustomDungeonFlags.THEATRETOGGLE))
                {
                    GameManager.Instance.PrimaryPlayer.GiveItem("ski:manic_theatre");
                }

            }
            if (GameManager.Instance.SecondaryPlayer!= null)
            {

                if (SaveAPIManager.GetFlag(CustomDungeonFlags.THEATRETOGGLE))
                {
                    GameManager.Instance.SecondaryPlayer.GiveItem("ski:manic_theatre");
                }
            }
        }

    }
  
}