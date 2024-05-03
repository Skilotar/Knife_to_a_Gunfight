using GungeonAPI;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SaveAPI;
using HutongGames;
using Random = UnityEngine.Random;
using StatType = PlayerStats.StatType;

namespace Knives
{
    public static class Theatre_Twins
    {
       
        public static void Add()
        {
            ShrineFactory sf = new ShrineFactory()
            {
                name = "Theatre_Twins",
                modID = "ski",
                spritePath = "Knives/Resources/play/twins_idle_001.png",
                
                acceptText = "Uh.. Sure?",
                declineText = "No thanks.",
                OnAccept = Accept,
                OnDecline = null,
                CanUse = CanUse,
                offset = new Vector3(200.1f, 19f, 20.4f),
                talkPointOffset = new Vector3(12 / 16f, 42 / 16f, 0),
                isToggle = false,
                isBreachShrine = true,
                interactableComponent = typeof(TwinsInteractable)
                
            };
            //register shrine
            var obj = sf.Build();
            
            obj.AddAnimation("idle", "Knives/Resources/play/idle/", 6, NPCBuilder.AnimationType.Idle);
            obj.AddAnimation("talk", "Knives/Resources/play/talk/", 11, NPCBuilder.AnimationType.Talk);
            obj.AddAnimation("talk_start", "Knives/Resources/play/talk/", 11, NPCBuilder.AnimationType.Other);
            obj.AddAnimation("do_effect", "Knives/Resources/play/talk/", 11, NPCBuilder.AnimationType.Other);

            var npc = obj.GetComponent<TwinsInteractable>();
            npc.conversation = new List<string>() {
                "Excuse us, We're Big fans of your plays!",
                "We watch your killing sprees from just beyond the stage walls.",
                "Such rage! SUCH SORROW! You're so good its like you're not acting at all!",
                "But action plays get tiring...",
                "Would you endulge us with a Comedy?"
            };
            obj.SetActive(false);
        }

        private static bool CanUse(PlayerController player, GameObject npc)
        {
           return true;
        }
        

        public static void Accept(PlayerController player, GameObject npc)
        {

            if (TheatreModeToggle.theatreOn)
            {
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.THEATRETOGGLE, false);
                Notify("Manic Theatre Mode: Disabled", "");
            }
            else
            {

                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.THEATRETOGGLE, true);
                Notify("Manic Theatre Mode: Enabled", "");
            }
            
        }

        public static void Notify(string header, string text)
        {
            var sprite = PickupObjectDatabase.GetById(ChaoticComedy.ID).sprite;
            GameUIRoot.Instance.notificationController.DoCustomNotification(
                header,
                text,
                sprite.Collection,
                sprite.spriteId,
                UINotificationController.NotificationColor.SILVER,
                false,
                false);
        }


    }
}