using ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Knives
{
    public class EasyVFXDatabase
    {
        //Basegame VFX Objects
        public static GameObject WeakenedStatusEffectOverheadVFX = ResourceCache.Acquire("Global VFX/VFX_Debuff_Status") as GameObject;
        
        public static GameObject SpiratTeleportVFX;
        public static GameObject TeleporterPrototypeTelefragVFX = PickupObjectDatabase.GetById(449).GetComponent<TeleporterPrototypeItem>().TelefragVFXPrefab.gameObject;
        public static GameObject BloodiedScarfPoofVFX = PickupObjectDatabase.GetById(436).GetComponent<BlinkPassiveItem>().BlinkpoofVfx.gameObject;
        public static GameObject ChestTeleporterTimeWarp = (PickupObjectDatabase.GetById(573) as ChestTeleporterItem).TeleportVFX;
        public static GameObject MachoBraceDustUpVFX = PickupObjectDatabase.GetById(665).GetComponent<MachoBraceItem>().DustUpVFX;
        public static GameObject MachoBraceBurstVFX = PickupObjectDatabase.GetById(665).GetComponent<MachoBraceItem>().BurstVFX;
        public static GameObject MachoBraceOverheadVFX = PickupObjectDatabase.GetById(665).GetComponent<MachoBraceItem>().OverheadVFX;
        //Projectile Death Effects
        public static GameObject GreenLaserCircleVFX = (PickupObjectDatabase.GetById(89) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
        public static GameObject YellowLaserCircleVFX = (PickupObjectDatabase.GetById(651) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
        public static GameObject RedLaserCircleVFX = (PickupObjectDatabase.GetById(32) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
        public static GameObject BlueLaserCircleVFX = (PickupObjectDatabase.GetById(59) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
        public static GameObject SmoothLightBlueLaserCircleVFX = (PickupObjectDatabase.GetById(576) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
        public static GameObject SmoothLightGreenLaserCircleVFX = (PickupObjectDatabase.GetById(360) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
        public static GameObject WhiteCircleVFX = (PickupObjectDatabase.GetById(330) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
        public static GameObject BlueFrostBlastVFX = (PickupObjectDatabase.GetById(225) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
        public static GameObject RedFireBlastVFX = (PickupObjectDatabase.GetById(125) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
        public static GameObject SmallMagicPuffVFX = (PickupObjectDatabase.GetById(338) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
        //Basegame VFX Pools
        public static VFXPool SpiratTeleportVFXPool;

       
       
        //Stat Up VFX
        public static GameObject DamageUpVFX;
        public static GameObject ShotSpeedUpVFX;
        public static GameObject SpeedUpVFX;
        public static GameObject FirerateUpVFX;
        public static GameObject AccuracyUpVFX;
        public static GameObject KnockbackUpVFX;
        public static GameObject ReloadSpeedUpVFX;
        public static GameObject HexHR;
        public static GameObject HexHL;
        public static GameObject HexVR;
        public static GameObject HexVL;
        public static GameObject HexWind;
        public static GameObject BlastBlight;
        public static GameObject Hexplosion;
        public static GameObject BAB;
        public static GameObject Pit;
        public static GameObject SkaterP;
        public static GameObject RollCharge;
        public static GameObject RollBoom;
        public static GameObject PropBoom;
        public static GameObject Good;
        public static GameObject Evil;
        public static GameObject Shield;
        public static GameObject Lightning;
        public static void Init()
        {
            //Spirat Teleportation VFX
            #region SpiratTP
            GameObject teleportBullet = EnemyDatabase.GetOrLoadByGuid("7ec3e8146f634c559a7d58b19191cd43").bulletBank.GetBullet("self").BulletObject;
            Projectile proj = teleportBullet.GetComponent<Projectile>();
            if (proj != null)
            {
                TeleportProjModifier tp = proj.GetComponent<TeleportProjModifier>();
                if (tp != null)
                {
                    SpiratTeleportVFXPool = tp.teleportVfx;
                    SpiratTeleportVFX = tp.teleportVfx.effects[0].effects[0].effect;
                }
            }
            #endregion

            HEXSetup();
            BlastSetup();
            BABSetup();
            SkateSetup();
            RollSetup();
            SoulSetup();
            ShieldSetup();
            LightningSetup();
            //PropSettup(); 
        }

        public static void HEXSetup()
        {
            //Hex setups          hori right
            GameObject HexObj = ItemBuilder.AddSpriteToObject("Hex_HR_VFX", "Knives/Resources/HexVfx/HR/Hex_horizontal_right_001", null);
            FakePrefab.MarkAsFakePrefab(HexObj);
            UnityEngine.Object.DontDestroyOnLoad(HexObj);
            tk2dSpriteAnimator animator = HexObj.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = HexObj.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData HexHRcollection = SpriteBuilder.ConstructCollection(HexObj, ("Hex_HR_Collection"));

            tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 10 };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 3; i++)
            {
                tk2dSpriteCollectionData collection = HexHRcollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/HexVfx/HR/Hex_horizontal_right_00{i}", collection);
                tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
            }
            idleClip.frames = frames.ToArray();
            idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = false;

            HexHR = HexObj;



            //--------------------------------------------------------------------------  hori left
            GameObject HexObj2 = ItemBuilder.AddSpriteToObject("Hex_HL_VFX", "Knives/Resources/HexVfx/HL/Hex_horizontal_left_001", null);
            FakePrefab.MarkAsFakePrefab(HexObj2);
            UnityEngine.Object.DontDestroyOnLoad(HexObj2);
            tk2dSpriteAnimator animator2 = HexObj2.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation2 = HexObj2.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData HexHRcollection2 = SpriteBuilder.ConstructCollection(HexObj2, ("Hex_HL_Collection"));

            tk2dSpriteAnimationClip idleClip2 = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 10 };
            List<tk2dSpriteAnimationFrame> frames2 = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 3; i++)
            {
                tk2dSpriteCollectionData collection2 = HexHRcollection2;
                int frameSpriteId2 = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/HexVfx/HL/Hex_horizontal_left_00{i}", collection2);
                tk2dSpriteDefinition frameDef2 = collection2.spriteDefinitions[frameSpriteId2];
                frames2.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId2, spriteCollection = collection2 });
            }
            idleClip2.frames = frames2.ToArray();
            idleClip2.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator2.Library = animation2;
            animator2.Library.clips = new tk2dSpriteAnimationClip[] { idleClip2 };
            animator2.DefaultClipId = animator2.GetClipIdByName("start");
            animator2.playAutomatically = false;

            HexHL = HexObj2;


            //-------------------------------------------------------------------------------  vert right
            GameObject HexObj3 = ItemBuilder.AddSpriteToObject("Hex_VR_VFX", "Knives/Resources/HexVfx/VR/Hex_vertical_right_001", null);
            FakePrefab.MarkAsFakePrefab(HexObj3);
            UnityEngine.Object.DontDestroyOnLoad(HexObj3);
            tk2dSpriteAnimator animator3 = HexObj3.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation3 = HexObj3.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData HexHRcollection3 = SpriteBuilder.ConstructCollection(HexObj3, ("Hex_VR_Collection"));

            tk2dSpriteAnimationClip idleClip3 = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 10 };
            List<tk2dSpriteAnimationFrame> frames3 = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 3; i++)
            {
                tk2dSpriteCollectionData collection3 = HexHRcollection3;
                int frameSpriteId3 = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/HexVfx/VR/Hex_vertical_right_00{i}", collection3);
                tk2dSpriteDefinition frameDef3 = collection3.spriteDefinitions[frameSpriteId3];
                frames3.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId3, spriteCollection = collection3 });
            }
            idleClip3.frames = frames3.ToArray();
            idleClip3.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator3.Library = animation3;
            animator3.Library.clips = new tk2dSpriteAnimationClip[] { idleClip3 };
            animator3.DefaultClipId = animator3.GetClipIdByName("start");
            animator3.playAutomatically = false;

            HexVR = HexObj3;

            //------------------------------------------------------------------ vert left

            GameObject HexObj4 = ItemBuilder.AddSpriteToObject("Hex_VL_VFX", "Knives/Resources/HexVfx/VL/Hex_vertical_left_001", null);
            FakePrefab.MarkAsFakePrefab(HexObj4);
            UnityEngine.Object.DontDestroyOnLoad(HexObj4);
            tk2dSpriteAnimator animator4 = HexObj4.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation4 = HexObj4.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData HexHRcollection4 = SpriteBuilder.ConstructCollection(HexObj4, ("Hex_VL_Collection"));

            tk2dSpriteAnimationClip idleClip4 = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 10 };
            List<tk2dSpriteAnimationFrame> frames4 = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 3; i++)
            {
                tk2dSpriteCollectionData collection4 = HexHRcollection4;
                int frameSpriteId4 = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/HexVfx/VL/Hex_vertical_left_00{i}", collection4);
                tk2dSpriteDefinition frameDef4 = collection4.spriteDefinitions[frameSpriteId4];
                frames4.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId4, spriteCollection = collection4 });
            }
            idleClip4.frames = frames4.ToArray();
            idleClip4.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator4.Library = animation4;
            animator4.Library.clips = new tk2dSpriteAnimationClip[] { idleClip4 };
            animator4.DefaultClipId = animator4.GetClipIdByName("start");
            animator4.playAutomatically = false;

            HexVL = HexObj4;


            //--------------------------------------
            //Windup
            GameObject HexObj5 = ItemBuilder.AddSpriteToObject("Hex_WD_VFX", "Knives/Resources/HexVfx/HexWindup/Hex_Windup_001", null);
            FakePrefab.MarkAsFakePrefab(HexObj5);
            UnityEngine.Object.DontDestroyOnLoad(HexObj5);
            tk2dSpriteAnimator animator5 = HexObj5.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation5 = HexObj5.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData HexHRcollection5 = SpriteBuilder.ConstructCollection(HexObj5, ("Hex_WD_Collection"));

            tk2dSpriteAnimationClip idleClip5 = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 6 };
            List<tk2dSpriteAnimationFrame> frames5 = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 4; i++)
            {
                tk2dSpriteCollectionData collection5 = HexHRcollection5;
                int frameSpriteId5 = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/HexVfx/HexWindup/Hex_Windup_00{i}", collection5);
                tk2dSpriteDefinition frameDef5 = collection5.spriteDefinitions[frameSpriteId5];
                frames5.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId5, spriteCollection = collection5 });
            }
            idleClip5.frames = frames5.ToArray();
            idleClip5.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator5.Library = animation5;
            animator5.Library.clips = new tk2dSpriteAnimationClip[] { idleClip5 };
            animator5.DefaultClipId = animator5.GetClipIdByName("start");
            animator5.playAutomatically = false;

            HexWind = HexObj5;


        }

        public static void BlastSetup()
        {
            GameObject Blast = ItemBuilder.AddSpriteToObject("Blast_VFX", "Knives/Resources/Blast/Blight_explosion_001", null);
            FakePrefab.MarkAsFakePrefab(Blast);
            UnityEngine.Object.DontDestroyOnLoad(Blast);
            tk2dSpriteAnimator animator = Blast.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = Blast.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData Blastcollection = SpriteBuilder.ConstructCollection(Blast, ("Blast_Collection"));

            tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 10 };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 9; i++)
            {
                tk2dSpriteCollectionData collection = Blastcollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/Blast/Blight_explosion_00{i}", collection);
                tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
            }
            idleClip.frames = frames.ToArray();
            idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.DefaultClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.playAutomatically = false;

            BlastBlight = Blast;


            GameObject HexBlast = ItemBuilder.AddSpriteToObject("HexBlast_VFX", "Knives/Resources/Hexplosions/Hexplosion_001", null);
            FakePrefab.MarkAsFakePrefab(HexBlast);
            UnityEngine.Object.DontDestroyOnLoad(HexBlast);
            tk2dSpriteAnimator Hexanimator = HexBlast.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation Hexanimation = HexBlast.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData HexBlastcollection = SpriteBuilder.ConstructCollection(HexBlast, (" HexBlast_Collection"));

            tk2dSpriteAnimationClip HexidleClip = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 10 };
            List<tk2dSpriteAnimationFrame> Hexframes = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 9; i++)
            {
                tk2dSpriteCollectionData Hexcollection = HexBlastcollection;
                int HexframeSpriteId = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/Hexplosions/Hexplosion_00{i}", Hexcollection);
                tk2dSpriteDefinition HexframeDef = Hexcollection.spriteDefinitions[HexframeSpriteId];
                Hexframes.Add(new tk2dSpriteAnimationFrame { spriteId = HexframeSpriteId, spriteCollection = Hexcollection });
            }
            HexidleClip.frames = Hexframes.ToArray();
            HexidleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            Hexanimator.Library = Hexanimation;
            Hexanimator.Library.clips = new tk2dSpriteAnimationClip[] { HexidleClip };
            Hexanimator.DefaultClipId = Hexanimator.GetClipIdByName("start");
            Hexanimator.DefaultClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            Hexanimator.playAutomatically = false;

            Hexplosion = HexBlast;
        }

        public static void BABSetup()
        {
            GameObject Bab = ItemBuilder.AddSpriteToObject("BAB_VFX", "Knives/Resources/BabBoom/Bab_explosion_001", null);
            FakePrefab.MarkAsFakePrefab(Bab);
            UnityEngine.Object.DontDestroyOnLoad(Bab);
            tk2dSpriteAnimator animator = Bab.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = Bab.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData Babcollection = SpriteBuilder.ConstructCollection(Bab, ("Bab_Collection"));

            tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 8 };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 8; i++)
            {
                tk2dSpriteCollectionData collection = Babcollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/BabBoom/Bab_explosion_00{i}", collection);
                tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
            }
            idleClip.frames = frames.ToArray();
            idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.DefaultClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.playAutomatically = true;
            animator.PlayAndDisableObject();
            
            
            BAB = Bab;

        }

        public static void SkateSetup()
        {
            GameObject Skate = ItemBuilder.AddSpriteToObject("Skate_VFX", "Knives/Resources/SkaterParry/Skate_roll_parry_001", null);
            FakePrefab.MarkAsFakePrefab(Skate);
            UnityEngine.Object.DontDestroyOnLoad(Skate);
            tk2dSpriteAnimator animator = Skate.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = Skate.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData Skatecollection = SpriteBuilder.ConstructCollection(Skate, ("Skate_Collection"));

            tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 6 };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 2; i++)
            {
                tk2dSpriteCollectionData collection = Skatecollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/SkaterParry/Skate_roll_parry_00{i}", collection);
                tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
            }
            idleClip.frames = frames.ToArray();
            idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.DefaultClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.playAutomatically = true;
            animator.PlayAndDisableObject();
            

            SkaterP = Skate;

        }
        public static void RollSetup()
        {
            GameObject Roll = ItemBuilder.AddSpriteToObject("Roll_charge", "Knives/Resources/DustDown/RollCharge_dust_001", null);
            FakePrefab.MarkAsFakePrefab(Roll);
            UnityEngine.Object.DontDestroyOnLoad(Roll);
            tk2dSpriteAnimator animator = Roll.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = Roll.AddComponent<tk2dSpriteAnimation>();
            

            tk2dSpriteCollectionData Skatecollection = SpriteBuilder.ConstructCollection(Roll, ("Roll_Collection"));

            tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 18 };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 6; i++)
            {
                tk2dSpriteCollectionData collection = Skatecollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/DustDown/RollCharge_dust_00{i}", collection);
                tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
            }
            idleClip.frames = frames.ToArray();
            idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.DefaultClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.playAutomatically = true;
            animator.PlayAndDisableObject();
            

            RollCharge = Roll;


            GameObject RollB = ItemBuilder.AddSpriteToObject("Roll_boom", "Knives/Resources/RollBoom/RollCharge_Boom_001", null);
            FakePrefab.MarkAsFakePrefab(RollB);
            UnityEngine.Object.DontDestroyOnLoad(RollB);
            tk2dSpriteAnimator animator3 = RollB.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation3 = RollB.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData HexHRcollection3 = SpriteBuilder.ConstructCollection(RollB, ("RollBoom_Collection"));

            tk2dSpriteAnimationClip idleClip3 = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 15 };
            List<tk2dSpriteAnimationFrame> frames3 = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 3; i++)
            {
                tk2dSpriteCollectionData collection3 = HexHRcollection3;
                int frameSpriteId3 = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/RollBoom/RollCharge_Boom_00{i}", collection3);
                tk2dSpriteDefinition frameDef3 = collection3.spriteDefinitions[frameSpriteId3];
                frames3.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId3, spriteCollection = collection3 });
            }
            idleClip3.frames = frames3.ToArray();
            idleClip3.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator3.Library = animation3;
            animator3.Library.clips = new tk2dSpriteAnimationClip[] { idleClip3 };
            animator3.DefaultClipId = animator3.GetClipIdByName("start");
            animator3.playAutomatically = false;
            animator3.PlayAndDisableObject();

            RollBoom = RollB;
        }

        public static void SoulSetup()
        {
            GameObject GoodSoul = ItemBuilder.AddSpriteToObject("GoodSoul", "Knives/Resources/Souls/GoodSoul/soul_of_a_lost_gundead_001", null);
            FakePrefab.MarkAsFakePrefab(GoodSoul);
            UnityEngine.Object.DontDestroyOnLoad(GoodSoul);
            tk2dSpriteAnimator animator = GoodSoul.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = GoodSoul.AddComponent<tk2dSpriteAnimation>();


            tk2dSpriteCollectionData GoodSoulCollection = SpriteBuilder.ConstructCollection(GoodSoul, ("GoodSoul_Collection"));

            tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 6 };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 4; i++)
            {
                tk2dSpriteCollectionData collection = GoodSoulCollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/Souls/GoodSoul/soul_of_a_lost_gundead_00{i}", collection);
                tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
            }
            idleClip.frames = frames.ToArray();
            idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.DefaultClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
            animator.playAutomatically = true;
            animator.PlayAndDisableObject();
            Good = GoodSoul;






            GameObject EvilSoul = ItemBuilder.AddSpriteToObject("EvilSoul", "Knives/Resources/Souls/JammedSoul/soul_of_a_lost_jammed_001", null);
            FakePrefab.MarkAsFakePrefab(EvilSoul);
            UnityEngine.Object.DontDestroyOnLoad(EvilSoul);
            tk2dSpriteAnimator animator2 = EvilSoul.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation2 = EvilSoul.AddComponent<tk2dSpriteAnimation>();


            tk2dSpriteCollectionData EvilSoulCollection = SpriteBuilder.ConstructCollection(EvilSoul, ("EvilSoul_Collection"));

            tk2dSpriteAnimationClip idleClip2 = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 6 };
            List<tk2dSpriteAnimationFrame> frames2 = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 4; i++)
            {
                tk2dSpriteCollectionData collection2 = EvilSoulCollection;
                int frameSpriteId2 = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/Souls/JammedSoul/soul_of_a_lost_jammed_00{i}", collection2);
                tk2dSpriteDefinition frameDef2 = collection2.spriteDefinitions[frameSpriteId2];
                frames2.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId2, spriteCollection = collection2 });
            }
            idleClip2.frames = frames2.ToArray();
            idleClip2.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
            animator2.Library = animation2;
            animator2.Library.clips = new tk2dSpriteAnimationClip[] { idleClip2 };
            animator2.DefaultClipId = animator2.GetClipIdByName("start");
            animator2.DefaultClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
            animator2.playAutomatically = true;
            animator2.PlayAndDisableObject();


            Evil = EvilSoul;

        }

        public static void ShieldSetup()
        {
            GameObject Block = ItemBuilder.AddSpriteToObject("Block_VFX", "Knives/Resources/AnchorRage/RageShield_001", null);
            FakePrefab.MarkAsFakePrefab(Block);
            UnityEngine.Object.DontDestroyOnLoad(Block);
            tk2dSpriteAnimator animator = Block.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = Block.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData Blockcollection = SpriteBuilder.ConstructCollection(Block, ("Block_Collection"));

            tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 10 };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 4; i++)
            {
                tk2dSpriteCollectionData collection = Blockcollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/AnchorRage/RageShield_00{i}", collection);
                tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
            }
            idleClip.frames = frames.ToArray();
            idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.DefaultClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.playAutomatically = true;
            


            Shield = Block;

        }

        public static void LightningSetup()
        {

            GameObject Block = ItemBuilder.AddSpriteToObject("Lightning_VFX", "Knives/Resources/ArcaneLightning/ArcaneLightning_001", null);
            FakePrefab.MarkAsFakePrefab(Block);
            UnityEngine.Object.DontDestroyOnLoad(Block);
            tk2dSpriteAnimator animator = Block.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = Block.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData Blockcollection = SpriteBuilder.ConstructCollection(Block, ("lightning_Collection"));

            tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 12 };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 6; i++)
            {
                tk2dSpriteCollectionData collection = Blockcollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/ArcaneLightning/ArcaneLightning_00{i}", collection);
                tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
            }
            idleClip.frames = frames.ToArray();
            idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.DefaultClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.playAutomatically = true;



            Lightning = Block;

        }
    }
}