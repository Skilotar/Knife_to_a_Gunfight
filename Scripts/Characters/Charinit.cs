using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.CharacterAPI;
using UnityEngine;

namespace Knives
{
    class InitCharacters
    {

        public static void Charactersetup()
        {
            //Alexandria.CharacterAPI.ToolsCharApi.EnableDebugLogging = true;
            SetupSkater();
            SetupRollin();
        }


        public static void SetupSkater()
        {
            var Skater = Loader.BuildCharacter("Knives/Scripts/Characters/Skater",
                   "ski:Skater",
                    new Vector3(24.5f, 16.5f, 27.1f),
                     false,
                     new Vector3(15.3f, 19f, 25.3f),
                     true,
                     false,
                     false,
                     true, //Sprites used by paradox
                     false, //Glows
                     null, //Glow Mat
                     null, //Alt Skin Glow Mat
                     0, //Hegemony Cost
                     false, //HasPast
                     ""); //Past ID String


            var doer = Skater.idleDoer;
            doer.coreIdleAnimation = "select_idle";
            doer.phases = new CharacterSelectIdlePhase[]
            {
                new CharacterSelectIdlePhase() { inAnimation = "crossarms", holdAnimation = "crossarms_hold", holdMin = 3, holdMax = 5 },
                new CharacterSelectIdlePhase() { outAnimation = "hair"},

                new CharacterSelectIdlePhase() { inAnimation = "light_up", holdAnimation = "smoking", outAnimation = "trash"},

            };

            

        }

        public static void SetupRollin()
        {

            
            GlowMatDoer glow = new GlowMatDoer(new Color32(225, 225, 225, 225), 0, 50,.2f);

            var Rollin = Loader.BuildCharacter("Knives/Scripts/Characters/Rollin",
                   "ski:Rollin",
                    new Vector3(26.5f, 17, 27.1f),
                     false,
                     new Vector3(17.3f, 19f, 25.3f),
                     true,
                     false,
                     false,
                     true, //Sprites used by paradox
                     true, //Glows
                     glow, //Glow Mat
                     glow, //Alt Skin Glow Mat
                     0, //Hegemony Cost
                     false, //HasPast
                     ""); ; //Past ID String

            
            var doer = Rollin.idleDoer;
            doer.coreIdleAnimation = "select_idle";
            doer.phases = new CharacterSelectIdlePhase[]
            {
                new CharacterSelectIdlePhase() { inAnimation = "crossarms", holdAnimation = "crossarms_hold", holdMin = 3, holdMax = 5 },
                new CharacterSelectIdlePhase() { outAnimation = "hair"},

                new CharacterSelectIdlePhase() { inAnimation = "light_up", holdAnimation = "smoking", outAnimation = "trash"},

            };
            
        }
    }
}
