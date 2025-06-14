using System;
using System.Collections.Generic;
using System.Collections;
using Gungeon;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using EnemyAPI;
using GungeonAPI;
using HarmonyLib;
using Alexandria.SoundAPI;
using Pathfinding;

namespace Knives
{
    public class RhythmicHeart : CompanionItem
    {

        private static readonly string guid = "ski:Cadence";
        private static GameObject prefab;
        public static void Init()
        {
            string itemName = "Rhythmic Heart";
            string resourceName = "Knives/Resources/Cadence/RhythmicHeart";
            GameObject obj = new GameObject();
            RhythmicHeart item = obj.AddComponent<RhythmicHeart>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Dance For Your Life";
            string longDesc = "Hit active reloads to build up combo. Lose combo on getting hit.\n" +
                "A heart beating exactly to the haunting rhythms hanging in the air."
               + "\n\n\n - Knife_to_a_Gunfight";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            item.quality = PickupObject.ItemQuality.A;
            item.CompanionGuid = guid;

            RhythmicHeart.BuildPrefab();

            ID = item.PickupObjectId;
        }

        public static int ID;
        public static void BuildPrefab()
        {
            bool flag = RhythmicHeart.prefab != null || CompanionBuilder.companionDictionary.ContainsKey(RhythmicHeart.guid);
            if (!flag)
            {

                RhythmicHeart.prefab = CompanionBuilder.BuildPrefab("Cadence", RhythmicHeart.guid, "Knives/Resources/Cadence/Cadence_level1_001", new IntVector2(0, 0), new IntVector2(15, 15));

                var companionController = RhythmicHeart.prefab.AddComponent<CompanionController>();
                companionController.CanInterceptBullets = false;
                companionController.aiActor.MovementSpeed = 7f;
                companionController.aiActor.healthHaver.PreventAllDamage = true;
                companionController.aiActor.CollisionDamage = 0f;
                var bs = prefab.GetComponent<BehaviorSpeculator>();
                bs.MovementBehaviors.Add(new CompanionFollowPlayerBehavior() { IdleAnimations = new string[] { "idle" } });
                bs.MovementBehaviors.Add(new SimpleCompanionBehaviours.SimpleCompanionApproach(2));
                prefab.GetOrAddComponent<NecroAttackBehaviour>();
                

                List<string> idle1 = new List<string>()
                {
                    
                    "Knives/Resources/Cadence/Cadence_level1_005.png",
                    "Knives/Resources/Cadence/Cadence_level1_006.png",
                    "Knives/Resources/Cadence/Cadence_level1_001.png",
                    "Knives/Resources/Cadence/Cadence_level1_002.png",
                    "Knives/Resources/Cadence/Cadence_level1_003.png",
                    "Knives/Resources/Cadence/Cadence_level1_004.png",
                };
                List<string> idle2 = new List<string>()
                {
                    
                    "Knives/Resources/Cadence/Cadence_level2_004.png",
                    "Knives/Resources/Cadence/Cadence_level2_005.png",
                    "Knives/Resources/Cadence/Cadence_level2_006.png",
                    "Knives/Resources/Cadence/Cadence_level2_001.png",
                    "Knives/Resources/Cadence/Cadence_level2_002.png",
                    "Knives/Resources/Cadence/Cadence_level2_003.png",
                };
                List<string> idle3 = new List<string>()
                {
                    "Knives/Resources/Cadence/Cadence_level3_003.png",
                    "Knives/Resources/Cadence/Cadence_level3_004.png",
                    "Knives/Resources/Cadence/Cadence_level3_001.png",
                    "Knives/Resources/Cadence/Cadence_level3_002.png",
                };

                var collection = prefab.GetComponent<tk2dSprite>().Collection;
                tk2dSpriteAnimator animator = prefab.GetOrAddComponent<tk2dSpriteAnimator>();

                var idle1IdsList = new List<int>();
                var idle2IdsList = new List<int>();
                var idle3IdsList = new List<int>();
                foreach (string sprite in idle1)
                {
                    idle1IdsList.Add(SpriteBuilder.AddSpriteToCollection(sprite, collection));
                }
                foreach (string sprite in idle2)
                {
                    idle2IdsList.Add(SpriteBuilder.AddSpriteToCollection(sprite, collection));
                }
                foreach (string sprite in idle3)
                {
                    idle3IdsList.Add(SpriteBuilder.AddSpriteToCollection(sprite, collection));
                }
                Level1 = SpriteBuilder.AddAnimation(animator,collection,idle1IdsList,"Level_1");
                Level2 = SpriteBuilder.AddAnimation(animator, collection, idle2IdsList, "Level_2");
                Level3 = SpriteBuilder.AddAnimation(animator, collection, idle3IdsList, "Level_3");
            }

        }
        public static tk2dSpriteAnimationClip Level1 = new tk2dSpriteAnimationClip();
        public static tk2dSpriteAnimationClip Level2 = new tk2dSpriteAnimationClip();
        public static tk2dSpriteAnimationClip Level3 = new tk2dSpriteAnimationClip();
        public static PlayerController user;
        public static int countsuccess = 0;

        public override void Pickup(PlayerController player)
        {
            AkSoundEngine.SetRTPCValue("DANCER_Sound", 100);
            player.healthHaver.OnDamaged += HealthHaver_OnDamaged;
            player.OnEnteredCombat += OnEnterCombat;
            if (player.IsPrimaryPlayer)
            {
                Gun.ActiveReloadActivated = true;
            }
            else
            {
                Gun.ActiveReloadActivatedPlayerTwo = true;
            }
            user = player;
            countsuccess = 0;
            base.Pickup(player);

            
        }
        bool regrabsoundsetting = false;
        public override void Update()
        {
            if(this.Owner != null)
            {
                if(WasInCombat == true && Owner.IsInCombat == false) // leaving combat reset song
                {
                    WasInCombat = false;
                    this.StopAllCoroutines();
                    
                    if (NecroAttackBehaviour.ai_Level == 1)
                    {
                        StartCoroutine(LerpSound(60, 80, false));
                    }
                    if (NecroAttackBehaviour.ai_Level == 2)
                    {
                        StartCoroutine(LerpSound(30, 50, false));
                    }
                    if (NecroAttackBehaviour.ai_Level >= 3)
                    {
                        StartCoroutine(LerpSound(0, 20, false));
                    }

                }
                if (GameManager.Instance.IsPaused)
                {
                    regrabsoundsetting = true;
                }
                if(regrabsoundsetting == true && !GameManager.Instance.IsPaused)
                {
                    regrabsoundsetting = false;
                    int valueType = (int)AkQueryRTPCValue.RTPCValue_Global;
                    float rtpcValue;
                    AKRESULT result = AkSoundEngine.GetRTPCValue("VOL_MUS", null, 0, out rtpcValue, ref valueType);
                    DesiredSoundVolume = rtpcValue;
                    if(Owner.IsInCombat == true)
                    {
                        if (NecroAttackBehaviour.ai_Level == 1) // relerp sound again
                        {
                            StartCoroutine(LerpSound(80, 60, true));
                        }
                        if (NecroAttackBehaviour.ai_Level == 2)
                        {
                            StartCoroutine(LerpSound(50, 30, true));
                        }
                        if (NecroAttackBehaviour.ai_Level >= 3)
                        {
                            StartCoroutine(LerpSound(20, 0, true));
                        }
                    }
                    
                }
                
            }

            base.Update();
        }
        public bool WasInCombat = false;
        private void OnEnterCombat()
        {
            NecroAttackBehaviour.TeleportToOwner(Owner);
            WasInCombat = true;

            if (countsuccess <= 0)
            { 
                NecroAttackBehaviour.ai_Level = 1;
                StartCoroutine(LerpSound(80, 60, true));
            }
            if (countsuccess >= 2 && countsuccess <= 3)
            {
                NecroAttackBehaviour.ai_Level = 2;
                StartCoroutine(LerpSound(50, 30, true));
            }
            if (countsuccess >= 4)
            {
                NecroAttackBehaviour.ai_Level = 3;
                StartCoroutine(LerpSound(20, 0, true));
            }
            
        }
        public float DesiredSoundVolume;
        public float MusicVol;
        public IEnumerator LerpSound(int TargetLerp, int StartLerp, bool soundin)
        {
            DesiredSoundVolume = GameManager.Options.MusicVolume; // current Sound setting volume

            if (DesiredSoundVolume <= 50) 
            if (DesiredSoundVolume <= 30 && DesiredSoundVolume > 50) // anything above 50 is max volume
            {
                TargetLerp = StartLerp + (int)(20 * (DesiredSoundVolume / 30));
            }
            if (DesiredSoundVolume < 10) TargetLerp = StartLerp + 5; // minum sound scaling.


            if (soundin) // going up or down
            {
                int valueType = (int)AkQueryRTPCValue.RTPCValue_Global;
                float rtpcValue;
                AKRESULT result = AkSoundEngine.GetRTPCValue("VOL_MUS", null, 0, out rtpcValue, ref valueType);
                MusicVol = rtpcValue; // current Actual sound volume

                

                for (int i = StartLerp; i < TargetLerp; i++) // lerp up from initial
                {
                    AkSoundEngine.SetRTPCValue("DANCER_Sound", i);
                   
                    yield return new WaitForSeconds(.01f);
                }
                for (int i = (int)MusicVol; i > 0; i--) // current sound vol go down to 0;
                {

                    AkSoundEngine.SetRTPCValue("VOL_MUS", i);
                    yield return new WaitForSeconds(.01f);
                }
                
            }
            else
            {
                
                for (int i = StartLerp; i > TargetLerp; i--) // lerp down from initial
                {
                    AkSoundEngine.SetRTPCValue("DANCER_Sound", i);
                    yield return new WaitForSeconds(.001f);
                }
               
                AkSoundEngine.SetRTPCValue("DANCER_Sound", 100);

                DesiredSoundVolume = GameManager.Options.MusicVolume; // current Sound setting volume
                
                for (int i = 0; i < DesiredSoundVolume; i++) // go from 0 to desired sound volume
                {
                    AkSoundEngine.SetRTPCValue("VOL_MUS", i);
                    yield return new WaitForSeconds(.001f);
                }
                regrabsoundsetting = true;
            }
            yield return null;
        }

        [HarmonyPatch(typeof(Gun), nameof(Gun.OnActiveReloadSuccess))]
        public class CountSuccesses
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (user != null)
                {
                    if (user.HasPickupID(ID))
                    {
                        countsuccess++;
                    }
                }
            }

        }

        private void HealthHaver_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            countsuccess = 0;
            AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", base.gameObject);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.OnDamaged -= HealthHaver_OnDamaged;
            if (player.IsPrimaryPlayer)
            {
                Gun.ActiveReloadActivated = false;
            }
            else
            {
                Gun.ActiveReloadActivatedPlayerTwo = false;
            }
            AkSoundEngine.PostEvent("Stop_KnightToCSharp_Loop", base.gameObject);
            AkSoundEngine.PostEvent("Stop_Coral_Riff_Loop", base.gameObject);
            AkSoundEngine.PostEvent("Stop_Portabellohead_Loop", base.gameObject);
            AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", base.gameObject);
            AkSoundEngine.SetRTPCValue("DANCER_Sound", 100);
            AkSoundEngine.SetRTPCValue("VOL_MUS", DesiredSoundVolume);
            countsuccess = 0;
            NecroAttackBehaviour.ai_Level = 1;
            return base.Drop(player);
        }

    }
    public class NecroAttackBehaviour : MonoBehaviour
    {
        public static int ai_Level = 1;
        public AIActor m_aiActor;
        public static AIActor StActor;
        public void Start()
        {
            this.m_aiActor = base.GetComponent<AIActor>();
            StActor = m_aiActor;
            Owner = m_aiActor.CompanionOwner;
            animationLevel = 0;
            Reloading = false;
            doingRestart = false;
        }

        bool doingRestart = false;
        int animationLevel = 0;
        bool Reloading = false;
        public static void TeleportToOwner(PlayerController Owner)
        {
            Vector2 vector = StActor.specRigidbody.UnitBottomCenter - StActor.transform.position.XY();
            CellData data = Owner.GetAbsoluteParentRoom().GetNearestCellToPosition(Owner.CenterPosition);
            if (data != null)
            {
                IntVector2? randomAvailableCell = data.position;
                StActor.transform.position = Pathfinder.GetClearanceOffset(randomAvailableCell.Value, StActor.Clearance).WithY((float)randomAvailableCell.Value.y) - vector;
                StActor.specRigidbody.Reinitialize();
            }
            
        }

        public int FireBeat = 2;
        public int frames = 4;
        public float TimerPerBeat;
        public float BeatsPerMeasure;
        public int ShotsPerMeasure;

        public void Update()
        {
            if (Owner == null)
            {
                if (this.m_aiActor && this.m_aiActor.CompanionOwner)
                {
                    Owner = this.m_aiActor.CompanionOwner;

                }
                else
                {
                    Owner = GameManager.Instance.BestActivePlayer;
                }
            }
            else
            {
                // Getting these to line up consistantly is a personal hell and I just HAD to pick songs that didn't share time signatures
                // Brilliant move ski. truely amazing


                if (NecroAttackBehaviour.ai_Level > 3) NecroAttackBehaviour.ai_Level = 3; // Hard cap in case of overflow 

                if (ai_Level != animationLevel && (!GameManager.Instance.IsPaused && !GameManager.Instance.IsLoadingLevel)) // songchange restart and fix timings.
                {
                    // rhythm should be a full bar
                    if (NecroAttackBehaviour.ai_Level == 1)  // 12/8  150bpm
                    {
                        Rhythm = 75f; //time per measure
                        frames = 6;    //animation will loop twice per measure
                        
                        animationLevel = ai_Level;
                        BeatsPerMeasure = 6;
                        TimerPerBeat = 0.4f;
                        ShotsPerMeasure = 2;

                        Reloading = false;
                        if (doingRestart == false)
                        {
                            StartCoroutine(musicStartupBeatDelay("Play_Portabellohead_Loop", 0));
                        }

                    }
                    if (NecroAttackBehaviour.ai_Level == 2) // 4/4  123bpm
                    {
                        Rhythm = 123;
                        frames = 6;     //animation will loop twice per measure
                        animationLevel = ai_Level;
                        BeatsPerMeasure = 4;
                        TimerPerBeat = 0.4878f;
                        ShotsPerMeasure = 2;

                        Reloading = false;
                        if (doingRestart == false)
                        {
                            StartCoroutine(musicStartupBeatDelay("Play_KnightToCSharp_Loop", 0));
                        }
                    }
                    if (NecroAttackBehaviour.ai_Level >= 3) // 4/4   126
                    {
                        Rhythm = 126;  //time per measure
                        frames = 4;
                        
                        animationLevel = ai_Level;
                        BeatsPerMeasure = 4;
                        TimerPerBeat = 0.4762f;
                        ShotsPerMeasure = 2;

                        Reloading = false;
                        if (doingRestart == false)
                        {
                            StartCoroutine(musicStartupBeatDelay("Play_Coral_Riff_Loop", 0));
                        }

                    }
                }
                if (Owner.IsInCombat)
                {
                    if(this.m_aiActor.spriteAnimator != null)
                    {
                        if (this.m_aiActor.spriteAnimator.CurrentFrame == 1 && Reloading == true) Reloading = false;
                        if (this.m_aiActor.spriteAnimator.CurrentFrame == 0 && Reloading == false)
                        {

                            Reloading = true;
                            switch (ai_Level)
                            {
                                case 1:
                                    doshoot(ai_Level);
                                    break;
                                case 2:
                                    doshoot(ai_Level);
                                    break;
                                case 3:
                                    doshoot(ai_Level);
                                    break;
                            }

                        }
                        //if (this.m_aiActor.spriteAnimator.CurrentFrame == 1 || this.m_aiActor.spriteAnimator.CurrentFrame == 3) StartCoroutine(DoDiscoFloor());
                        if (m_aiActor.OverrideTarget == null) GetNewTargetForCadence();

                    }
                    
                }
                
                if (this.m_aiActor.spriteAnimator != null)
                {
                    if (this.m_aiActor.spriteAnimator.CurrentFrame == FireBeat || this.m_aiActor.spriteAnimator.CurrentFrame == FireBeat - 1) m_aiActor.MovementSpeed = 0;
                    else m_aiActor.MovementSpeed = 14;
                }


                if (GameManager.Instance.IsPaused || GameManager.Instance.IsLoadingLevel)
                {
                    animationLevel = 0;
                    AkSoundEngine.PostEvent("Stop_KnightToCSharp_Loop", base.gameObject);
                    AkSoundEngine.PostEvent("Stop_Coral_Riff_Loop", base.gameObject);
                    AkSoundEngine.PostEvent("Stop_Portabellohead_Loop", base.gameObject);
                }
            }

        }

        public IEnumerator musicStartupBeatDelay(string song, float delay)
        {
            doingRestart = true;
            this.m_aiActor.spriteAnimator.ForceClearCurrentClip();
            tk2dSpriteAnimationClip clip = m_aiActor.spriteAnimator.GetClipByName("Level_" + ai_Level);
            float bps = (Rhythm / 60); 
            float fps = (bps * BeatsPerMeasure) / ShotsPerMeasure; 

            this.m_aiActor.spriteAnimator.Play(clip, 0, fps); // one frame per beat.
            yield return new WaitForSeconds(delay);
            AkSoundEngine.PostEvent(song, base.gameObject);
            doingRestart = false;
        }

       
        private void GetNewTargetForCadence()
        {

            List<AIActor> actors = new List<AIActor> { };
            RoomHandler absoluteRoom = this.Owner.CurrentRoom;
            if (absoluteRoom != null)
            {
                if (absoluteRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
                {
                    actors = MiscToolMethods.RandomNoRepeats(absoluteRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear), 1);
                }
            }
            m_aiActor.OverrideTarget = actors[0].specRigidbody;
            
        }

        private PlayerController Owner;
        public float Rhythm = 0;
        
        public void doshoot(int ShootLevel)
        {
            //AkSoundEngine.PostEvent("Play_WPN_e12_shot_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_ENM_rubber_bounce_01", base.gameObject);
            m_aiActor.MovementSpeed = 0;
            for (int i = 0; i < 10; i++)
            {
                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[3]).DefaultModule.projectiles[0];
                Projectile component = MiscToolMethods.SpawnProjAtPosi(projectile2, m_aiActor.CenterPosition, Owner, 0, (i * 36));
                bool flag = component != null;
                if (flag)
                {
                    component.Owner = Owner;
                    component.Shooter = Owner.specRigidbody;
                    component.AdditionalScaleMultiplier *= 1f;
                    component.baseData.range = 5 * ShootLevel;
                    if (ShootLevel == 1) component.baseData.damage = 1;
                    if (ShootLevel == 2) component.baseData.damage = 2;
                    if (ShootLevel == 3) component.baseData.damage = 4;
                    component.DefaultTintColor = color_levels[ShootLevel];
                    component.HasDefaultTint = true;
                }
            }

            int notes = 1;
            if (ShootLevel == 2) notes = 2;
            if (ShootLevel >= 3) notes = 3;

            for (int i = 0; i < notes; i++)
            {
                    
                Projectile component2 = MiscToolMethods.SpawnProjAtPosi(MikuBeam.Note2, m_aiActor.Position, Owner, 0, UnityEngine.Random.Range(-180, 180));
                bool flag = component2 != null;
                if (flag)
                {
                    component2.Owner = Owner;
                    component2.Shooter = Owner.specRigidbody;
                    component2.baseData.damage = 2;
                    component2.DefaultTintColor = color_levels[ShootLevel];
                    component2.HasDefaultTint = true;
                    ImprovedAfterImage image = component2.GetComponent<ImprovedAfterImage>();
                    image.dashColor = color_levels[ShootLevel];
                }
            }
        }

        Dictionary<int,Color> color_levels = new Dictionary<int, Color>
        {
           {1,ExtendedColours.MikuBlue},
           {2,ExtendedColours.charmPink},
           {3,ExtendedColours.lime},
        };

        
    }
    
}