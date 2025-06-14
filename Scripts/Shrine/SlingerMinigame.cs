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
    public class SlingerMinigame
    {

        public static void Add()
        {
            var minimapicon = SpriteBuilder.SpriteFromResource("Knives/Resources/Deadeye/Duel_MapIcon");
            
            OldShrineFactory sf = new OldShrineFactory()
            {
                name = "DeadEye",
                modID = "ski",
                spritePath = "Knives/Resources/Deadeye/Deadeye_ready_001.png",

                acceptText = "Sure",
                declineText = "No thanks",
                room = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("Knives/Resources/Deadeye/DeadEye_room.room").room,
                RoomWeight = .4f,

                OnAccept = Accept,
                OnDecline = null,
                CanUse = CanUse,
                offset = new Vector3(5f, -1, 0),
                talkPointOffset = new Vector3(1, 3, 0),
                isToggle = false,
                isBreachShrine = false,
                interactableComponent = typeof(SlingerInteractor),
                icon = minimapicon,
            };

            GameObject obj = sf.Build();


            List<string> idle = new List<string>()
            {
                "Knives/Resources/Deadeye/deadeye_animations/idle/Deadeye_idle_001.png",
                "Knives/Resources/Deadeye/deadeye_animations/idle/Deadeye_idle_002.png",
                "Knives/Resources/Deadeye/deadeye_animations/idle/Deadeye_idle_003.png",
                "Knives/Resources/Deadeye/deadeye_animations/idle/Deadeye_idle_004.png",
            };
            List<string> idleR = new List<string>()
            {
                "Knives/Resources/Deadeye/deadeye_animations/idleR/Deadeye_idleR_001.png",
                "Knives/Resources/Deadeye/deadeye_animations/idleR/Deadeye_idleR_002.png",
                "Knives/Resources/Deadeye/deadeye_animations/idleR/Deadeye_idleR_003.png",
                "Knives/Resources/Deadeye/deadeye_animations/idleR/Deadeye_idleR_004.png",
            };
            List<string> Ready = new List<string>()
            {
               "Knives/Resources/Deadeye/deadeye_animations/ready/Deadeye_ready_001.png",
               "Knives/Resources/Deadeye/deadeye_animations/ready/Deadeye_ready_002.png",
               "Knives/Resources/Deadeye/deadeye_animations/ready/Deadeye_ready_003.png",
               "Knives/Resources/Deadeye/deadeye_animations/ready/Deadeye_ready_004.png",
               "Knives/Resources/Deadeye/deadeye_animations/ready/Deadeye_ready_005.png",
               "Knives/Resources/Deadeye/deadeye_animations/ready/Deadeye_ready_006.png",
               "Knives/Resources/Deadeye/deadeye_animations/ready/Deadeye_ready_007.png",
               "Knives/Resources/Deadeye/deadeye_animations/ready/Deadeye_ready_008.png",
            };
            List<string> Fire = new List<string>()
            {
                "Knives/Resources/Deadeye/deadeye_animations/DRAW/Deadeye_DRAW_001.png",
                "Knives/Resources/Deadeye/deadeye_animations/DRAW/Deadeye_DRAW_002.png",
                "Knives/Resources/Deadeye/deadeye_animations/DRAW/Deadeye_DRAW_003.png",
                "Knives/Resources/Deadeye/deadeye_animations/DRAW/Deadeye_DRAW_004.png",
                "Knives/Resources/Deadeye/deadeye_animations/DRAW/Deadeye_DRAW_005.png",
                "Knives/Resources/Deadeye/deadeye_animations/DRAW/Deadeye_DRAW_006.png",
            };
            List<string> Wake = new List<string>()
            {
                "Knives/Resources/Deadeye/deadeye_animations/WAKEUP/Deadeye_Wakeup_001.png",
                "Knives/Resources/Deadeye/deadeye_animations/WAKEUP/Deadeye_Wakeup_002.png",
                "Knives/Resources/Deadeye/deadeye_animations/WAKEUP/Deadeye_Wakeup_003.png",
                "Knives/Resources/Deadeye/deadeye_animations/WAKEUP/Deadeye_Wakeup_004.png",
                "Knives/Resources/Deadeye/deadeye_animations/WAKEUP/Deadeye_Wakeup_005.png",
                "Knives/Resources/Deadeye/deadeye_animations/WAKEUP/Deadeye_Wakeup_006.png",
            };

            List<string> Sleep = new List<string>()
            {
                "Knives/Resources/Deadeye/deadeye_animations/Sleepy/Deadeye_SleepyIdle_001.png",
                "Knives/Resources/Deadeye/deadeye_animations/Sleepy/Deadeye_SleepyIdle_002.png",
                "Knives/Resources/Deadeye/deadeye_animations/Sleepy/Deadeye_SleepyIdle_003.png",
                "Knives/Resources/Deadeye/deadeye_animations/Sleepy/Deadeye_SleepyIdle_004.png",
            };

            List<string> Hit = new List<string>()
            {
                "Knives/Resources/Deadeye/deadeye_animations/Hit/Deadeye_Hit_001.png",
                "Knives/Resources/Deadeye/deadeye_animations/Hit/Deadeye_Hit_002.png",
                "Knives/Resources/Deadeye/deadeye_animations/Hit/Deadeye_Hit_003.png",
                "Knives/Resources/Deadeye/deadeye_animations/Hit/Deadeye_Hit_004.png",

            };
            List<string> Enflame = new List<string>()
            {
                "Knives/Resources/Deadeye/deadeye_animations/Enflame/Deadeye_Enflame_001.png",
                "Knives/Resources/Deadeye/deadeye_animations/Enflame/Deadeye_Enflame_002.png",
                "Knives/Resources/Deadeye/deadeye_animations/Enflame/Deadeye_Enflame_003.png",
                "Knives/Resources/Deadeye/deadeye_animations/Enflame/Deadeye_Enflame_004.png",
                "Knives/Resources/Deadeye/deadeye_animations/Enflame/Deadeye_Enflame_005.png",
                "Knives/Resources/Deadeye/deadeye_animations/Enflame/Deadeye_Enflame_006.png",
                "Knives/Resources/Deadeye/deadeye_animations/Enflame/Deadeye_Enflame_007.png",
                "Knives/Resources/Deadeye/deadeye_animations/Enflame/Deadeye_Enflame_008.png",

            };

            var idleIdsList = new List<int>();
            var idleRIdsList = new List<int>();
            var ReadyIdsList = new List<int>();
            var FireIdsList = new List<int>();
            var WakeIdsList = new List<int>();
            var sleepIdsList = new List<int>();
            var HitIdsList = new List<int>();
            var EnflameIdsList = new List<int>();

            var collection = obj.GetComponent<tk2dSprite>().Collection;
            tk2dSpriteAnimator animator = obj.GetOrAddComponent<tk2dSpriteAnimator>();
            foreach (string sprite in idle)
            {
                idleIdsList.Add(SpriteBuilder.AddSpriteToCollection(sprite, collection));
            }
            foreach (string sprite in idleR)
            {
                idleRIdsList.Add(SpriteBuilder.AddSpriteToCollection(sprite, collection));
            }
            foreach (string sprite in Ready)
            {
                ReadyIdsList.Add(SpriteBuilder.AddSpriteToCollection(sprite, collection));
            }
            foreach (string sprite in Fire)
            {
                FireIdsList.Add(SpriteBuilder.AddSpriteToCollection(sprite, collection));
            }
            foreach (string sprite in Wake)
            {
                WakeIdsList.Add(SpriteBuilder.AddSpriteToCollection(sprite, collection));
            }
            foreach (string sprite in Sleep)
            {
                sleepIdsList.Add(SpriteBuilder.AddSpriteToCollection(sprite, collection));
            }
            foreach (string sprite in Hit)
            {
                HitIdsList.Add(SpriteBuilder.AddSpriteToCollection(sprite, collection));
            }
            foreach (string sprite in Enflame)
            {
                EnflameIdsList.Add(SpriteBuilder.AddSpriteToCollection(sprite, collection));
            }
            idleanim = NPCAPISpriteBuilder.AddAnimation(animator, collection, idleIdsList, "Slinger_idle", tk2dSpriteAnimationClip.WrapMode.Loop, 4);
            idleRight = NPCAPISpriteBuilder.AddAnimation(animator, collection, idleRIdsList, "Slinger_idleR", tk2dSpriteAnimationClip.WrapMode.Loop, 4);
            readyanim = NPCAPISpriteBuilder.AddAnimation(animator, collection, ReadyIdsList, "Slinger_ready", tk2dSpriteAnimationClip.WrapMode.Loop, 4);
            fireanim = NPCAPISpriteBuilder.AddAnimation(animator, collection, FireIdsList, "Slinger_fire", tk2dSpriteAnimationClip.WrapMode.Once, 8);
            wakeanim = NPCAPISpriteBuilder.AddAnimation(animator, collection, WakeIdsList, "Slinger_wake", tk2dSpriteAnimationClip.WrapMode.Once, 6);
            sleepanim = NPCAPISpriteBuilder.AddAnimation(animator, collection, sleepIdsList, "Slinger_sleep", tk2dSpriteAnimationClip.WrapMode.Loop, 3);
            hitanim = NPCAPISpriteBuilder.AddAnimation(animator, collection, HitIdsList, "Slinger_hit", tk2dSpriteAnimationClip.WrapMode.Once, 6);
            Enflameanim = NPCAPISpriteBuilder.AddAnimation(animator, collection, EnflameIdsList, "Slinger_Enflame", tk2dSpriteAnimationClip.WrapMode.Once, 6);

            npcAnimator = animator;
            
            obj.SetActive(false);
            
        }
        
        public static tk2dSpriteAnimator npcAnimator;

        public static tk2dSpriteAnimationClip idleRight = new tk2dSpriteAnimationClip();
        public static tk2dSpriteAnimationClip idleanim = new tk2dSpriteAnimationClip();
        public static tk2dSpriteAnimationClip readyanim = new tk2dSpriteAnimationClip();
        public static tk2dSpriteAnimationClip fireanim = new tk2dSpriteAnimationClip();
        public static tk2dSpriteAnimationClip wakeanim = new tk2dSpriteAnimationClip();
        public static tk2dSpriteAnimationClip sleepanim = new tk2dSpriteAnimationClip();
        public static tk2dSpriteAnimationClip hitanim = new tk2dSpriteAnimationClip();
        public static tk2dSpriteAnimationClip Enflameanim = new tk2dSpriteAnimationClip();

        public static void ForcePlayAnimation(tk2dSpriteAnimationClip clip)
        {
            npcAnimator.Play(clip);
        }

        public static void Start()
        {
            //ETGModConsole.Log("I am alive");
            askingDouble = false;
            timeOfSnap = 0;
            timeOfShot = 0;
            falseStarts = 0;
            Win = false;
            DoubleOrNothin = false;
            shottaken = false;
            Waittime = 4;
            Playedgameonce = false;

            startup = false;

            
        }
        public static bool startup = false;
        public static bool Slinger;
        private static bool CanUse(PlayerController player, GameObject npc)
        {
            return !Playedgameonce;
        }

        public static GameObject center;
        
        public static void Accept(PlayerController player, GameObject npc)
        {

            player.StartCoroutine(MiniGameManager(player, npc));
            
        }

       
        public static bool WaitingForResponse = false;
        public static bool InGame = false;
        public static bool Emergency = false;
        private static IEnumerator MiniGameManager(PlayerController player, GameObject npc)
        {
            // Startup
            InGame = true;
            
            player.StartCoroutine(StartGame(player, npc)); 
            WaitingForResponse = true;
            while (WaitingForResponse) yield return new WaitForSeconds(.25f);


            // Duel
            if (Emergency == false) player.StartCoroutine(Duel(player, npc));
            WaitingForResponse = true;
            while (WaitingForResponse) yield return new WaitForSeconds(.25f);

            // Reward calc
            if (Emergency == false) player.StartCoroutine(CalcReward(player, npc));
            WaitingForResponse = true;
            while (WaitingForResponse) yield return new WaitForSeconds(.25f);

            // cleanup
            if (Emergency == false) player.StartCoroutine(Cleanup(player, npc));
            WaitingForResponse = true;
            while (WaitingForResponse) yield return new WaitForSeconds(.25f);
           
            InGame = false;
            yield break;
        }

        public static Gun gamegun;
        private static IEnumerator StartGame(PlayerController player, GameObject npc)
        {
            try
            {
                
                player.SetInputOverride("DuelMiniGame");
                player.CurrentRoom.SealRoom();
                Pixelator.Instance.LerpToLetterbox(0f, .25f);
            }
            catch
            {
                ETGModConsole.Log("Error Caught. Debug -DeadEYE Start-");
                Emergency = true;
                AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", player.gameObject);
                player.StartCoroutine(Cleanup(player, npc));
                yield break;
            }
            yield return new WaitForSeconds(.2f);
            try
            {
                tk2dSpriteAnimator animator = npc.GetOrAddComponent<tk2dSpriteAnimator>();
                player.CurrentGun.sprite.renderer.enabled = false;
                if (player.primaryHand != null) player.primaryHand.sprite.renderer.enabled = false;
                player.inventory.GunLocked.SetOverride("Duel_minigame", true, null);
                player.IsGunLocked = true;
                player.MovementModifiers += NoMotionModifier;
                player.ToggleGunRenderers(false, "duel");
                player.ToggleHandRenderers(false, "duel");
                player.ForceIdleFacePoint(new Vector2(1f, -1f), false);
                animator.Play(SlingerMinigame.idleanim);

                

                player.TeleportToPoint(npc.GetComponent<tk2dBaseSprite>().sprite.transform.TransformPoint(-11, 0, 0), false);

            }
            catch
            {
                ETGModConsole.Log("Error Caught. Debug -DeadEYE Start-");
                Emergency = true;
                AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", player.gameObject);
                player.StartCoroutine(Cleanup(player, npc));
                yield break;
            }

            yield return new WaitForSeconds(1.5f);
            try
            {
                Pixelator.Instance.LerpToLetterbox(1f, .20f);


                //camera move 
                target = npc.gameObject.GetOrAddComponent<CameraOverrideTarget>();
                target.startOverrideOnStart = true;
                target.Offset = new Vector2(-5, 0);
                player.CurrentStoneGunTimer = 6f;
            }
            catch
            {
                ETGModConsole.Log("Error Caught. Debug -DeadEYE Start-");
                Emergency = true;
                AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", player.gameObject);
                player.StartCoroutine(Cleanup(player, npc));
                yield break;
            }
            yield return new WaitForSeconds(5f); // wait while talking
            WaitingForResponse = false;

        }
        private static void NoMotionModifier(ref Vector2 voluntaryVel, ref Vector2 involuntaryVel)
        {
            voluntaryVel = Vector2.zero;
        }

        public static CameraOverrideTarget target;
        public static int falseStarts = 0;
        public static bool FIXZOOM = false;
        private static IEnumerator Duel(PlayerController player, GameObject npc)
        {
            //start duel 
            try
            {
                if (m_ExtantShoot != null)
                {
                    UnityEngine.GameObject.Destroy(m_ExtantShoot);
                }
                Win = false;
                tk2dSpriteAnimator animator = npc.GetOrAddComponent<tk2dSpriteAnimator>();
                animator.Play(readyanim);
                player.ToggleHandRenderers(false, "duel");
                player.CurrentStoneGunTimer = 4.5f;
                WaitingForResponse = true;
                shottaken = false;
                player.forceAimPoint = npc.GetComponent<SpeculativeRigidbody>().UnitCenter;

                player.StartCoroutine(doTumble(player, npc));
                PlayerSepiaHandler sep = player.gameObject.GetOrAddComponent<PlayerSepiaHandler>();
                sep.sepiaLevel = 1;
                Pixelator.Instance.LerpToLetterbox(0.20f, 3f);
                GameManager.Instance.MainCameraController.OverrideZoomScale /= .70f;
                AkSoundEngine.PostEvent("Stop_SND_All", player.gameObject);
                AkSoundEngine.StopAll();
                AkSoundEngine.PostEvent("Play_DuelStart_001", npc);
                
                Waittime = UnityEngine.Random.Range(1, 2.5f) + 6; // randomized portion
            }
            catch
            {
                ETGModConsole.Log("Error Caught. Debug -DeadEYE Duel-");
                Emergency = true;
                FIXZOOM = true;
                AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", player.gameObject);
                player.StartCoroutine(Cleanup(player, npc));

                //Notify("ERROR Caught", "Cleaning Up Minigame");
                yield break;
            }
            
            //wait till snap                                                      
            while (Waittime > 0 && shottaken == false)
            {
                Waittime -= BraveTime.DeltaTime;
                if (KeyDown(player, GungeonActions.GungeonActionType.Shoot) && player.CurrentStoneGunTimer <= 0)
                { 
                    try // If they make it this far without erroring its probably safe
                    {
                        shottaken = true;
                        GameObject Shot = SpawnManager.SpawnVFX(EasyVFXDatabase.CowboyPlayerMiss, player.transform.position + new Vector3(1.2f, .4f, 0), Quaternion.identity);
                        tk2dBaseSprite boom = Shot.GetComponent<tk2dBaseSprite>();
                        //if (UnityEngine.Random.Range(1, 3) < 2) lig.FlipX = true;
                        Shot.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(player.transform.position + new Vector3(1.2f, .4f, 0) + new Vector3(-7 / 16f, 3 / 16f, 0), tk2dBaseSprite.Anchor.MiddleLeft);
                        Shot.GetComponent<tk2dSpriteAnimator>().Play();
                        boom.HeightOffGround = 1f;
                        boom.UpdateZDepth();
                        m_ExtantShoot = Shot;
                        player.ToggleHandRenderers(true, "duel");
                        
                        falseStarts++;
                    }
                    catch
                    {
                        ETGModConsole.Log("Error Caught. Debug -DeadEYE Duel-");
                        Emergency = true;
                        FIXZOOM = true;
                        AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", player.gameObject);
                        player.StartCoroutine(Cleanup(player, npc));
                        yield break;
                    }
                    
                    string RebukeText = "No Cheating!";
                    if (falseStarts == 1)
                    {
                        RebukeText = "No Cheating!";
                    }
                    if (falseStarts == 2)
                    {
                        RebukeText = "You cut that out now!";
                    }
                    if (falseStarts == 3)
                    {
                        if(player != null && npc != null)
                        Pixelator.Instance.LerpToLetterbox(0.35f, 0.25f);
                        PlayerSepiaHandler sep1 = player.gameObject.GetOrAddComponent<PlayerSepiaHandler>();
                        sep1.sepiaLevel = 0;
                        RebukeText = "That's it! Duels over..";
                        SlingerInteractor slinger2 = npc.GetComponent<SlingerInteractor>();
                        TextBoxManager.ShowTextBox(slinger2.talkPoint.position, slinger2.talkPoint, 3f, RebukeText, player.characterAudioSpeechTag, instant: false);
                        yield return new WaitForSeconds(1f);
                        string RebukeText2 = "Get outta here fore I shoot ya dead.";

                        TextBoxManager.ShowTextBox(slinger2.talkPoint.position, slinger2.talkPoint, 3f, RebukeText2, player.characterAudioSpeechTag, instant: false);

                        yield return new WaitForSeconds(2f);
                        player.forceAimPoint = null;
                        GameManager.Instance.MainCameraController.OverrideZoomScale *= .70f;

                        AkSoundEngine.PostEvent("Stop_DuelStart_001", npc);
                        Playedgameonce = true;
                        WaitingForResponse = false;
                        player.StartCoroutine(Cleanup(player, npc));
                        yield break;
                    }

                    try
                    {
                        // rebuke
                        GameManager.Instance.MainCameraController.OverrideZoomScale *= .70f;
                        Pixelator.Instance.LerpToLetterbox(0.35f, 0.25f);
                        SlingerInteractor slinger = npc.GetComponent<SlingerInteractor>();
                        TextBoxManager.ShowTextBox(slinger.talkPoint.position, slinger.talkPoint, 3f, RebukeText, player.characterAudioSpeechTag, instant: false);
                        PlayerSepiaHandler sep2 = player.gameObject.GetOrAddComponent<PlayerSepiaHandler>();
                        sep2.sepiaLevel = 0;
                    }
                    catch
                    {
                        ETGModConsole.Log("Error Caught. Debug -DeadEYE Duel-");
                        Emergency = true;
                        FIXZOOM = true;
                        AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", player.gameObject);
                        player.StartCoroutine(Cleanup(player, npc));
                        yield break;
                    }

                    // reset
                    Waittime = 999;
                    yield return new WaitForSeconds(3f);
                    try
                    {
                        Pixelator.Instance.LerpToLetterbox(1f, .25f);
                        AkSoundEngine.PostEvent("Stop_DuelStart_001", npc);
                        player.StartCoroutine(Duel(player, npc));
                        player.ToggleHandRenderers(false, "duel");
                    }
                    catch
                    {
                        ETGModConsole.Log("Error Caught. Debug -DeadEYE Duel-");
                        Emergency = true;
                        FIXZOOM = true;
                        AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", player.gameObject);
                        player.StartCoroutine(Cleanup(player, npc));
                        yield break;
                    }
                    yield break;
                }
                yield return null;
            }

            //SNAP!!
            PlayerSepiaHandler sep3 = player.gameObject.GetOrAddComponent<PlayerSepiaHandler>();
            sep3.sepiaLevel = 0;

            
            //if (UnityEngine.Random.Range(1, 3) < 2) lig.FlipX = true;
            AkSoundEngine.PostEvent("Play_BOSS_Rat_Tail_Whip_01", npc);
            yield return new WaitForSeconds(.05f); // Line up snap with visual
            try
            {
                GameObject Snap = SpawnManager.SpawnVFX(EasyVFXDatabase.CowboySnap, (player.CenterPosition + new Vector2(5, -1)), Quaternion.identity);
                tk2dBaseSprite bang = Snap.GetComponent<tk2dBaseSprite>();
                Snap.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor((player.CenterPosition + new Vector2(5, -1)), tk2dBaseSprite.Anchor.LowerCenter);
                Snap.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
                bang.HeightOffGround = 1f;
                bang.UpdateZDepth();
                timeOfSnap = GameStatsManager.Instance.GetSessionStatValue(TrackedStats.TIME_PLAYED);
                
            }
            catch
            {
                ETGModConsole.Log("Error Caught. Debug -DeadEYE Duel-");
                Emergency = true;
                FIXZOOM = true;
                AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", player.gameObject);
                player.StartCoroutine(Cleanup(player, npc));
                yield break;
            }
            float timetobeat = .25f; // Average human reaction time.                    for gamers probably pretty free
            if (DoubleOrNothin) timetobeat = .20f; //Fast but totally doable            Lock tf in slinger
            Waittime = timetobeat;
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            while (Waittime > 0 && shottaken == false)
            {
                yield return new WaitForEndOfFrame();
                Waittime -= BraveTime.DeltaTime;
                if (KeyDown(player, GungeonActions.GungeonActionType.Shoot))
                {
                    shottaken = true;
                    AkSoundEngine.PostEvent("Play_WPN_colt1851_shot_01", player.gameObject);


                    // do shot timing check
                    timeOfShot = GameStatsManager.Instance.GetSessionStatValue(TrackedStats.TIME_PLAYED);

                    float difference = timeOfShot - timeOfSnap;

                    if (difference <= timetobeat)
                    {
                        Win = true;
                        try
                        {
                            GameObject Shot = SpawnManager.SpawnVFX(EasyVFXDatabase.CowboyPlayerShot, (player.CenterPosition), Quaternion.identity);
                            tk2dBaseSprite boom = Shot.GetComponent<tk2dBaseSprite>();
                            //if (UnityEngine.Random.Range(1, 3) < 2) lig.FlipX = true;
                            Shot.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(player.transform.position + new Vector3(1.2f, .4f, 0) + new Vector3(-7 / 16f, 3 / 16f, 0), tk2dBaseSprite.Anchor.MiddleLeft);
                            Shot.GetComponent<tk2dSpriteAnimator>().Play();
                            boom.HeightOffGround = 1f;
                            boom.UpdateZDepth();
                            m_ExtantShoot = Shot;
                            player.ToggleHandRenderers(true, "duel");
                            
                        }
                        catch
                        {
                            ETGModConsole.Log("Error Caught. Debug -DeadEYE Duel-");
                            Emergency = true;
                            FIXZOOM = true;
                            
                            player.StartCoroutine(Cleanup(player, npc));
                            yield break;
                        }
                        

                        //ETGModConsole.Log("Hit");
                    }
                    else
                    {
                        Win = false;


                        //ETGModConsole.Log("Late");
                    }

                }
                yield return null;
            }


            player.forceAimPoint = null;
            GameManager.Instance.MainCameraController.OverrideZoomScale *= .70f;

            if (DoubleOrNothin)
            {
                player.StartCoroutine(CalcReward(player, npc));
            }
            else
            {
                WaitingForResponse = false;
            }

        }

        public static GameObject m_ExtantShoot;

        private static IEnumerator CalcReward(PlayerController player, GameObject npc)
        {
            WaitingForResponse = true;

            if (shottaken == false) // Who comes to a cowboy duel and just doesn't shoot?
            {
                Win = false;
            }
            if(falseStarts >= 3)
            {
                WaitingForResponse = false;
                yield break;
            }
            if(Pixelator.Instance != null)
            Pixelator.Instance.LerpToLetterbox(0.35f, 0.25f);
            if (Win) //Damage or reward
            {
                // Play Hit
                tk2dSpriteAnimator animator = npc.GetOrAddComponent<tk2dSpriteAnimator>();
                animator.Play(hitanim);
                yield return new WaitForSeconds(.5f);

                // return to idle
                animator.Play(idleanim);

                if (DoubleOrNothin == false)// first time through
                {

                    yield return new WaitForSeconds(.5f);
                    player.StartCoroutine(DoubleOrNothing(player, npc));
                    while (askingDouble)
                    {
                        yield return new WaitForSeconds(.25f);
                        yield return null;
                    }

                    if (DoubleOrNothin == true)
                    {

                        SlingerInteractor slinger = npc.GetComponent<SlingerInteractor>();
                        TextBoxManager.ShowTextBox(slinger.talkPoint.position, slinger.talkPoint, 1f, "Good luck", player.characterAudioSpeechTag, instant: false);
                        //play jam animation and double damage
                        animator.Play(Enflameanim);
                        AkSoundEngine.PostEvent("Play_WPN_SAA_spin_01", npc.gameObject);
                        yield return new WaitForSeconds(.8f);
                        npc.GetComponent<tk2dBaseSprite>().usesOverrideMaterial = true;
                        Material material = npc.GetComponent<tk2dBaseSprite>().renderer.material;
                        material.shader = ShaderCache.Acquire("Brave/LitCutoutUberPhantom");
                        material.SetFloat("_PhantomGradientScale", phantom.GradientScale);
                        material.SetFloat("_PhantomContrastPower", phantom.ContrastPower);
                        AkSoundEngine.PostEvent("Play_ENM_jammer_curse_01",npc.gameObject);
                        player.StartCoroutine(DoCurseSmoke(npc,0));

                        yield return new WaitForSeconds(.5f);
                        animator.Play(readyanim);
                        yield return new WaitForSeconds(.5f);
                        player.StartCoroutine(Duel(player, npc));
                        shottaken = false;
                        Win = false;
                        timeOfShot = 0;
                        timeOfSnap = 0;
                        yield break;

                    }
                    else
                    {
                        SlingerInteractor slinger = npc.GetComponent<SlingerInteractor>();
                        TextBoxManager.ShowTextBox(slinger.talkPoint.position, slinger.talkPoint, 1f, "Fine. See you around slinger", player.characterAudioSpeechTag, instant: false);
                        yield return new WaitForSeconds(1f);

                        // take win and leave
                        SlingerMinigame.Reward(player);
                        Playedgameonce = true;
                    }
                }
                else // Second time through
                {
                    yield return new WaitForSeconds(.5f);
                    SlingerInteractor slinger = npc.GetComponent<SlingerInteractor>();
                    TextBoxManager.ShowTextBox(slinger.talkPoint.position, slinger.talkPoint, 1f, "Looks like I need some more practice..", player.characterAudioSpeechTag, instant: false);
                    yield return new WaitForSeconds(1f);

                    // better prize
                    SlingerMinigame.Reward(player);
                    Playedgameonce = true;
                }

            }
            else // Lost
            {
                try
                {
                    if (DoubleOrNothin)
                    {
                        if (player.healthHaver.GetCurrentHealth() <= 1 && player.healthHaver.Armor == 0)
                        {
                            WaitingForResponse = false;
                            player.StartCoroutine(Cleanup(player, npc));
                            InGame = false;
                        }
                        if (player.ForceZeroHealthState == true && player.healthHaver.Armor == 1)
                        {
                            WaitingForResponse = false;
                            player.StartCoroutine(Cleanup(player, npc));
                            InGame = false;
                        }
                        player.healthHaver.ApplyDamage(1f, Vector2.zero, "Your Own Slow Reflexes", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, true);
                    }
                    else
                    {
                        if (player.healthHaver.GetCurrentHealth() <= .5 && player.healthHaver.Armor == 0)
                        {
                            WaitingForResponse = false;
                            player.StartCoroutine(Cleanup(player, npc));
                            InGame = false;
                        }
                        if (player.ForceZeroHealthState == true && player.healthHaver.Armor == 1)
                        {
                            WaitingForResponse = false;
                            player.StartCoroutine(Cleanup(player, npc));
                            InGame = false;
                        }
                        player.healthHaver.ApplyDamage(.5f, Vector2.zero, "Your Own Slow Reflexes", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, true);
                    }

                    // play fire
                    tk2dSpriteAnimator animator = npc.GetOrAddComponent<tk2dSpriteAnimator>();
                    animator.Play(fireanim);

                    GameObject Shot = SpawnManager.SpawnVFX(EasyVFXDatabase.CowboyNPCShot, npc.GetComponent<tk2dBaseSprite>().WorldCenter, Quaternion.identity);
                    tk2dBaseSprite boom = Shot.GetComponent<tk2dBaseSprite>();
                    //if (UnityEngine.Random.Range(1, 3) < 2) lig.FlipX = true;
                    Shot.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(npc.GetComponent<tk2dBaseSprite>().WorldCenter + new Vector2(-5.5f / 16f, -3 / 16f), tk2dBaseSprite.Anchor.MiddleRight);
                    Shot.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
                    boom.HeightOffGround = 1f;
                    boom.UpdateZDepth();
                    AkSoundEngine.PostEvent("Play_WPN_colt1851_shot_01", npc.gameObject);

                }
                catch
                {
                    ETGModConsole.Log("Error Caught. Debug -DeadEYE Duel-");
                    Emergency = true;
                    player.StartCoroutine(Cleanup(player, npc));
                    yield break;
                }
               
                yield return new WaitForSeconds(.5f);
                if (DoubleOrNothin)
                {
                    SlingerInteractor slinger = npc.GetComponent<SlingerInteractor>();
                    TextBoxManager.ShowTextBox(slinger.talkPoint.position, slinger.talkPoint, 1f, "Not this time...", player.characterAudioSpeechTag, instant: false);
                }
                else
                {
                    SlingerInteractor slinger = npc.GetComponent<SlingerInteractor>();
                    TextBoxManager.ShowTextBox(slinger.talkPoint.position, slinger.talkPoint, 1f, "Better Luck Next Time..", player.characterAudioSpeechTag, instant: false);
                }

                // return to idle
                tk2dSpriteAnimator animator2 = npc.GetOrAddComponent<tk2dSpriteAnimator>();
                animator2.Play(idleanim);
                yield return new WaitForSeconds(1f);
                Playedgameonce = true;
            }
            WaitingForResponse = false;
        }

        private static IEnumerator DoCurseSmoke(GameObject npc, float zOffset = 0f)
        {
            while (DoubleOrNothin)
            {
                if (npc.GetComponent<tk2dBaseSprite>())
                {
                    tk2dBaseSprite targetSprite = npc.GetComponent<tk2dBaseSprite>();
                    Vector3 vector = targetSprite.WorldBottomLeft.ToVector3ZisY(zOffset);
                    Vector3 vector2 = targetSprite.WorldTopRight.ToVector3ZisY(zOffset);
                    float num = (vector2.y - vector.y) * (vector2.x - vector.x);
                    float num2 = 25f * num;
                    int num3 = Mathf.CeilToInt(Mathf.Max(1f, num2 * BraveTime.DeltaTime));
                    int num4 = num3;
                    Vector3 minPosition = vector;
                    Vector3 maxPosition = vector2;
                    Vector3 direction = Vector3.up / 2f;
                    float angleVariance = 120f;
                    float magnitudeVariance = 0.2f;
                    float? startLifetime = new float?(UnityEngine.Random.Range(0.8f, 1.25f));
                    GlobalSparksDoer.DoRandomParticleBurst(num4, minPosition, maxPosition, direction, angleVariance, magnitudeVariance, null, startLifetime, null, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
                }
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(.01f);
                yield return null;

            }
        }

        private static IEnumerator Cleanup(PlayerController player, GameObject npc)
        {
            WaitingForResponse = true;

            if(Emergency) 
            {
                AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", player.gameObject);
                Notify("Error Caught -Deadeye Minigame- ", "Doing Emergency Cleanup",npc);
                if (Win)// Crashed Out After player won.   Give them their shiz   :)
                {
                    SlingerMinigame.Reward(player);
                }
                
            }
            DoubleOrNothin = false;
            if (target != null)
            {
                target.EndOverride();
                UnityEngine.GameObject.Destroy(target);
            }
            if (player != null)
            {
                player.MovementModifiers -= NoMotionModifier;
                if (player.CurrentRoom != null) player.CurrentRoom.UnsealRoom();
                if (player.CurrentGun != null) player.CurrentGun.sprite.renderer.enabled = true;
                if (player.primaryHand != null) player.primaryHand.sprite.renderer.enabled = true;
                if (player.inventory != null) player.inventory.GunLocked.SetOverride("Duel_minigame", false, null);
                player.IsGunLocked = false;
                player.forceAimPoint = null;
                player.ToggleGunRenderers(true, "duel");
                player.ToggleHandRenderers(true, "duel");
                player.ClearInputOverride("DuelMiniGame");
                PlayerSepiaHandler sep2 = player.gameObject.GetOrAddComponent<PlayerSepiaHandler>();
                sep2.sepiaLevel = 0;
                GameManager.Instance.DungeonMusicController.ResetForNewFloor(GameManager.Instance.Dungeon);
                GameManager.Instance.DungeonMusicController.NotifyEnteredNewRoom(player.CurrentRoom);
            }
            if (npc != null)
            {
                npc.GetComponent<tk2dBaseSprite>().usesOverrideMaterial = false;
                npc.GetComponent<tk2dSpriteAnimator>().Play(idleanim);
                AkSoundEngine.PostEvent("Stop_DuelStart_001", npc);
            }
            if (FIXZOOM)
            GameManager.Instance.MainCameraController.OverrideZoomScale *= .70f;
            
            if(Pixelator.Instance != null)
            Pixelator.Instance.LerpToLetterbox(1f, .25f);

            if (m_ExtantShoot != null)
            {
                UnityEngine.GameObject.Destroy(m_ExtantShoot);
            }

            yield return new WaitForSeconds(1f);
            WaitingForResponse = false;
            Win = false;
        }
      

        private static void Reward(PlayerController obj)
        {
            GameObject chestprefab = GameManager.Instance.RewardManager.C_Chest.gameObject;
            if(UnityEngine.Random.Range(0,5f) <= 1) chestprefab = GameManager.Instance.RewardManager.B_Chest.gameObject;

            if (DoubleOrNothin)
            {
                chestprefab = GameManager.Instance.RewardManager.B_Chest.gameObject;
                if (UnityEngine.Random.Range(0, 15f) <= 1) chestprefab = GameManager.Instance.RewardManager.A_Chest.gameObject;
                if (UnityEngine.Random.Range(0, 50f) <= 1) chestprefab = GameManager.Instance.RewardManager.S_Chest.gameObject;
            }
            Chest Giftbox = Chest.Spawn(chestprefab.GetComponent<Chest>(), (obj.CenterPosition + new Vector2(4.5f, 1)).ToIntVector2(), obj.GetAbsoluteParentRoom(),true);

            Giftbox.IsLocked = false;
            Giftbox.PreventFuse = true;
            Giftbox.RegisterChestOnMinimap(Giftbox.GetAbsoluteParentRoom());

        }

        private static IEnumerator DoubleOrNothing(PlayerController player, GameObject npc)
        {
            askingDouble = true;
            int selectedResponse = -1;
            GameManager.Instance.DungeonMusicController.ResetForNewFloor(GameManager.Instance.Dungeon);
            GameManager.Instance.DungeonMusicController.NotifyEnteredNewRoom(player.CurrentRoom);
            Pixelator.Instance.LerpToLetterbox(0.35f, 0.25f);
            SlingerInteractor slinger = npc.GetComponent<SlingerInteractor>();
            TextBoxManager.ShowTextBox(slinger.talkPoint.position, slinger.talkPoint, 3f, "Augh! Damn Good Shot...", player.characterAudioSpeechTag, instant: false);
            yield return new WaitForSeconds(2f);
            TextBoxManager.ClearTextBox(slinger.talkPoint);
            
            TextBoxManager.ShowTextBox(slinger.talkPoint.position, slinger.talkPoint, -1f, "Double or Nothing?", player.characterAudioSpeechTag, instant: false);
            GameUIRoot.Instance.DisplayPlayerConversationOptions(player, null, "Bring it on!", "I know when to fold em'");
            while (!GameUIRoot.Instance.GetPlayerConversationResponse(out selectedResponse))
                yield return null;
            //ETGModConsole.Log(selectedResponse);
            if (selectedResponse == 0) DoubleOrNothin = true;
            TextBoxManager.ClearTextBox(slinger.talkPoint);
            askingDouble = false;
            yield break;
        }

        public static bool askingDouble = false;
        
        public static float timeOfSnap = 0;
        public static float timeOfShot = 0;
        public static bool Win = false;
        public static bool DoubleOrNothin = false;
        public static bool Playedgameonce = false;
        public static bool shottaken = false;
        public static float Waittime = 4;
        public static BlackPhantomProperties phantom = new BlackPhantomProperties();


        public static bool KeyDown(PlayerController player, GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel && player != null)
            {
                return BraveInput.GetInstanceForPlayer(player.PlayerIDX).ActiveActions.GetActionFromType(action).WasPressed;
            }
            else
            {
                return false;
            }

        }


        private static IEnumerator doTumble(PlayerController player, GameObject npc)
        {
            int bounces = 4;
            Vector2 dir = (npc.GetComponent<SpeculativeRigidbody>().UnitCenter + new Vector2(0,1)) - player.CenterPosition;
            GameObject tumble = UnityEngine.Object.Instantiate<GameObject>(TimeKeeper.weed.gameObject, player.transform.TransformPoint(-2, -1, 0), Quaternion.identity);
            DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(tumble, player.transform.TransformPoint(-2, -1, 0), dir, 7, false, false, true, false);

            while (bounces > 0)
            {
                if (GetPrivateType<DebrisObject, bool>(debrisObject, "onGround"))
                {
                    DebrisObject debrisObject2 = LootEngine.DropItemWithoutInstantiating(debrisObject.gameObject, debrisObject.transform.position, dir, 4, false, false, true, false);
                    debrisObject = debrisObject2;

                    bounces -= 1;
                }
                yield return null;
            }
            debrisObject.TriggerDestruction();
            yield return null;
        }

        private static T2 GetPrivateType<T, T2>(T obj, string field)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T2)f.GetValue(obj);
        }
        public static void Notify(string header, string text,GameObject obj)
        {
            var sprite = obj.GetComponent<tk2dSprite>();
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
