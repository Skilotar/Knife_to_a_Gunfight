
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using static GungeonAPI.OldShrineFactory;
using Gungeon;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Alexandria.Misc;
using System.Collections;
using Alexandria.DungeonAPI;
using Alexandria.NPCAPI;
using NpcApi;

namespace Knives
{
    public static class CharmingShrine
    {

        public static void Add()
        {
            OldShrineFactory sf = new OldShrineFactory()
            {
                name = "Charming_Shrine",
                modID = "ski",
                spritePath = "Knives/Resources/Charm/Shrine/idle/CharmingShrine_idle_001.png",
                text = "A old clay shrine with a coinslot in its head.\n\n" +
                "Insert coins?",
                acceptText = "Yes <15 casings>",
                declineText = "No thanks.",
                room = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("Knives/Resources/Charm/Shrine/CharmRoom.room").room,
                RoomWeight = 1f, 

                OnAccept = Accept,
                OnDecline = null,
                CanUse = CanUse,
                offset = new Vector3(-1.5f, 0, 0),
                talkPointOffset = new Vector3(2, 5, 0),
                isToggle = false,
                isBreachShrine = false,


            };
            
            var obj = sf.Build();


            List<string> idleSpritePaths = new List<string>()
            {
                "Knives/Resources/Charm/Shrine/idle/CharmingShrine_idle_001"
            };

            List<string> effectSpritePaths = new List<string>()
            {
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_001",
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_002",
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_003",
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_004",
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_005",
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_006",
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_007",
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_008",
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_009",
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_010",
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_011",
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_012",
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_013",
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_014",
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_015",
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_016",
                "Knives/Resources/Charm/Shrine/buy/CharmingShrine_Buy_017",

            };

            var idleIdsList = new List<int>();
            var effectIdsList = new List<int>();


            var collection = obj.GetComponent<tk2dSprite>().Collection;
            tk2dSpriteAnimator animator = obj.GetOrAddComponent<tk2dSpriteAnimator>();
            foreach (string sprite in idleSpritePaths)
            {
                idleIdsList.Add(SpriteBuilder.AddSpriteToCollection(sprite, collection));
            }
            foreach (string sprite in effectSpritePaths)
            {
                effectIdsList.Add(SpriteBuilder.AddSpriteToCollection(sprite, collection));
            }
            idleanim = NPCAPISpriteBuilder.AddAnimation(animator, collection, idleIdsList, "idle", tk2dSpriteAnimationClip.WrapMode.Loop, 6);
            effectanim = NPCAPISpriteBuilder.AddAnimation(animator, collection, effectIdsList, "do_effect", tk2dSpriteAnimationClip.WrapMode.Once, 10);



            obj.SetActive(false);
        }
        public static tk2dSpriteAnimationClip idleanim = new tk2dSpriteAnimationClip();
        public static tk2dSpriteAnimationClip effectanim = new tk2dSpriteAnimationClip();


        private static bool CanUse(PlayerController player, GameObject npc)
        {

            return player.carriedConsumables.Currency >= 15;
        }


        public static void Accept(PlayerController player, GameObject npc)
        {
            player.carriedConsumables.Currency -= 15;
            player.StartCoroutine(DoBuy(npc));

        }
        private static IEnumerator DoBuy(GameObject npc)
        {
            tk2dSpriteAnimator animator = npc.GetOrAddComponent<tk2dSpriteAnimator>();

            //ETGModConsole.Log(effectanim.name);
            animator.Play(effectanim);
            yield return new WaitForSeconds(1.7f);
            //int charmID = GetRandomCharmID();
            //LootEngine.SpawnItem(PickupObjectDatabase.GetById(charmID).gameObject, npc.transform.position - new Vector3(-1, 1), Vector2.zero, 1, true, true);
            LootEngine.SpawnItem(PickupObjectDatabase.GetById(CharmBauble.ID).gameObject, npc.transform.position - new Vector3(-1, 1), Vector2.zero, 1, true, true);
            animator.Play(idleanim);
        }

        private static int GetRandomCharmID()
        {
            int rng = UnityEngine.Random.Range(0, GlobalCharmList.Charmslist.Count);
            return GlobalCharmList.Charmslist[rng];
        }

        
    }
}
