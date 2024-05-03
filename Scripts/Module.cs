using ItemAPI;
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MonoMod.RuntimeDetour;
using UnityEngine;
using Dungeonator;
using UnityEngine.UI;
using SaveAPI;
using NpcApi;
using BepInEx;
using System.IO;
using HarmonyLib;

namespace Knives
{
    

    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency(ETGModMainBehaviour.GUID)]
    public class Module : BaseUnityPlugin
    {
        public const string GUID = "skilotar.etg.knifetoagunfight";
        public const string NAME = "Knife to a Gunfight";
        public const string VERSION = "2.2.0";
        
        public static string FilePath;
        public static readonly string MOD_NAME = "Knife_to_a_Gunfight";
        
        public static readonly string TEXT_COLOR = "#5deba4";
        public static string ModName;
        

        public void Start()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager Manager)
        {
            try
            {
                
                EnemyAPI.Hooks.Init();
                EnemyAPI.EnemyTools.Init();
                EnemyAPI.EnemyBuilder.Init();
                SaveAPIManager.Setup("ski");
                CustomClipAmmoTypeToolbox.Init();
                Hooks.Init();
                NpcApi.Hooks.Init();
                new Harmony(GUID).PatchAll();

                //GungeonAPI.FakePrefabHooks.Init();
                //GungeonAPI.Tools.Init();
                ItemAPI.FakePrefabHooks.Init();
                ItemAPI.Tools.Init();
                ItemBuilder.Init();
                EasyVFXDatabase.Init();
                StaticReferences.Init();
                MultiActiveReloadManager.SetupHooks();
                
                UIHooks.Init();
                //sound stuffs

                FilePath = this.FolderPath();
                StaticStatusEffects.InitCustomEffects();
                EasyGoopDefinitions.DefineDefaultGoops();
                ReloadBreachShrineHooks.Init();

                //register all items and synergies. if text at the bottom doesnt fire something along the way crashed/produced and error
                //Not all scripts are loaded some are saved for a later date.


                // general passive
                Dizzyring.Register();
                Spring_roll.Register();
                Salmon_roll.Register();
                dragun_roll.Register();
                Long_roll_boots.Register();
                Rocket_boots.Register();
                Fly_Friend.Register();
                Space_hammer.Register();
                Sus_rounds.Register();
                Fates_blessing.Register();
                book.Register();
                daft_helm.Register();
                punk_helm.Register();
                clean_soul.Register();
                stardust.Register();
                    //tabletech_dizzy.Register(); // bad items >:(
                    //SCP_323.Register();
                Super_fly.Register();
                Im_blue.Register();
                bad_attitude.Register();
                rubber_man.Register();
                Survivor.Register();
                speedster.Register();
                Danger_dance.Register();
                disco_inferno.Register();
                persuasive_bullets.Register();
                Slide_tech.Register();   // slide baby slide
                Slide_reload.Register();
                Slide_counter.Register();
                Slide_Shatter.Register();
                Slide_Split.Register();
                PeaceStandard.Register();
                SpeedyChamber.Register();
                ChamberofChambers.Register();
                menacing_aura.Register();
                Malware.Register();
                Manuel.Register();
                Farsighted.Register();
                ten_gallon.Register();
                BleakBubbles.Register();
                TableTech_AmpedCover.Register();
                War_paint.Register();
                Neon_bullets.Register();
                FractBullets.Register();
                ChaoticComedy.Register();
                SmokeAmmolet.Register();
                    //Occams.Register();
                Chamber_pendant.Register();
                Radar_pendant.Register();
                blastlets.Register();
                EjectButton.Register();
                Overkill.Register();
                CylindSpurs.Register();
                Bandolier.Register();
                Pilots_Helmet.Register();
                DarkSoles.Register();
                GunZilla_Tail.Register();
                Trinket_of_Kaliber.Register();
                Banana_Mag.Register();
                    //Looplets.Register();
                Warlord_Ire.Register();
                MasterKey.Register();
                Picking_Bone.Register();
                NewRadBoard.Register();
                AdrenalineRush.Register();
                OneTwoPunch.Register();
                MovieMagic.Register();
                HotStreak.Register();

                Stop_pendant.Register();
                RatCrown.Register();
                HobbyHorse.Register();
                AutonomusDefenseDrive.Register();
                ThrustStabilizers.Register();
                BrassKnuckles.Register();
                Guardian_Heart.Register();
                Deft_Hands.Register();
                CheatersDrawArm.Register();
                KylesGoatHoof.Register();
                Fliplets.Register();
                IronStomach.Register();
                MutantLiver.Register();
                Second_Brain.Register();
                ShotGlass.Register();
                IceBoots.Register();
                IgnitionTank.Register();


                //Skater Exclusive
                HoverBoard.Register();
                HeavyImpact.Register();
                JetSetterRadio.Register();
                TaggAr.Add();

                // general active
                Led_Maiden.Register();
                jojo_arrow.Register();
                nano_boost.Register();
                    //rad_board.Register();
                koolbucks.Register();
                sandvich.Register();
                dog.Register();
                power_bracer.Register();
                    //roundabout.Register();
                Eye_of_the_tiger.Register();
                Luft_balloons.Register();
                vodoo_kit.Register();
                BloodyNapkin.Register();
                Pig_Whistle.Register();
                shield.Register();
                    //AndroidReactorCore.Register();
                NEWAndroidReactorCore.Register();
                GnatHat.Register();
                HotelCaliforniaSpecial.Register();
                MindControlHeadband.Register();
                Dullahan_curse.Register();
                    //Rage_shield.Register();
                New_Rage_shield.Register();
                Sheila.Register();
                    //HolyGrenade.Register();
                NewHolyGrenade.Init();
                RedTape.Init();
                Present.Init();
                BombBag.Init();
                Lemonade.Init();
                Bombushka.Init();
                Mares_Leg.Register();
                Shotgate.Init();
                Spark.Init();
                P2020.Add();
                SMRT.Add();
                P2020_item.Register();
                Opossum_ring.Register();
                Broken_controller.Register();
                Riot_Drill.Register();
                WitchWatch.Register();
                Striker_Pack.Register();
                //
                Guardian_Shield.Register();
                Dummy_Shield.Add();
                //
                SideCar.Register();
                EmergencyDanceParty.Register();
                DoubleWHAMMY.Register();
                AOTBelt.Register();

                The_Backup.Register();
                BearArms.Register();
                Surplus_Powder_bag.Register();
                CloudInABottle.Register();


                //Guns
                hail_2_u.Add();
                    //fourth_wall_breaker.Add(); Reference See PeaceMaker
                Za_hando.Add();
                    //violin.Add();
                NewViolin.Add();
                Queen.Add();
                Lance.Add();
                Lance2.Add();
                MagicHat.Add();
                Lil_Boom.Add();
                BlackStabbith.Add();
                Ball.Add();
                    //harpoon.Add();
                NEWharpoon.Add();
                Mozam.Add();
                GunLance.Add();
                hot_coffee.Add();
                NewNewCopperChariot.Add();
                Hells_bells.Add();
                Succ.Add();
                Sheila_LMG.Add();
                TaurenTails.Add();
                ToyAK.Add();
                Neon.Add();
                Maw.Add();
                FireStrike.Add();
                    //BeatDownUnder.Add();
                Catalyzer.Add();
                    //ChargeRifle.Add();
                CoolerChargeRifle.Add();
                Baba.Add();
                HotShot.Add();
                Typewriter.Add();
                Hippo.Add();
                MonkeyBarrel.Add();
                MBSynergyForm.Add();
                Ratapult.Add();
                Levoluer.Add();
                Trash.Add();
                punt.Add();
                Bee.Add();
                Lone.Add();
                StarBurst.Add();
                Superstar.Add();
                Stargazer.Add();
                messenger.Add();
                FlatLine.Add();
                FishBones.Add();
                Undercover.Add();
                Zapistol.Add();
                Static.Add();
                Coiler.Add();
                Raigun.Add();
                PhaseBlade.Add();
                Thumper.Add();
                PeaceMaker.Add();
                //FirstImpression_special.Add();  BEGONE FOUL DEMON!!!
                FirstImpression.Add();
                Followup.Add();
                Steam_overheat.Add();
                Steam_rifle.Add();
                Cav.Add();
                BattlingRam.Add();
                USB.Add();
                Stop.Add();
                Ack_Choo.Add();
                //Powder_bag.Add();
                Bad_Name.Add();
                SIND.Add();
                Express.Add();
                Watch_Charged.Add();
                Watch_Standard.Add();
                BlackRose.Add();
                Rampage.Add();
                Pulstar.Add();
                Alchemizer.Add();
                Bab.Add();
                Fang.Add();
                GoYo.Add();
                TungstenBomber.Add();
                Focal.Add();
                GravitySlingshot.Add();
                EvilEye.Add();
                King.Add();
                Sponsword.Add();
                Umbra_main.Add();
                RockAndStone.Add(); // to the bone!
                    //Salvo.Add();
                Big_Bertha.Add();
                BigBaby.Add();
                VectorMKTwo.Add();
                Snake_Eyes.Add();
                Callahan.Add();
                Vengeance.Add();
                    //pikipiki
                EasyPikiDatabase.Init();
                Pikannon.Add();
                    //pikipiki
                Black_Betty.Add();
                GunChucks.Add();
                Fell.Add();
                Explodia.Add();


                PepperBox.Add();
                Fourk.Add();
                DoomShotty.Add();
                Luna.Add();
                Chainsaw.Add();
                Lamp.Add();
                Pot.Add();
                CandyCain.Add();
                Artillery_Revolver.Add();
                Bison.Add();
                ParticleCannon.Add();
                ExoThermic.Add();
                EndoThermic.Add();
                MacroMissiles.Add();
                HonorGaurd.Add();
                Phalanx.Add();
                Phalanx_syn.Add();
                Arcane_Hand.Add();
                Lynda.Add();
                Cigaretta.Add();
                Claw.Add();
                TimeKeeper.Add();


                //Devtools
                //noclip.Register();
                //ActiveCharger.Register();
                //CurtainTest.Register();
                //Semi.Register();



                //companions
                BabyGoodDodoGama.Init();

                //orbitals
                Stopda.Register();
                Masks_Com.Register();
                Masks_trag.Register();
                GlassBoi.Register();
                nanostone.Register();
                terracottaIoun.Register();
                Micro.Register();
                BackupDrone.Register();
                //Explodia
                Eye_otfg.Register();
                Stock_otfg.Register();
                Core_otfg.Register();
                Drum_otfg.Register();
                Barrel_otfg.Register();



                //JinxedItems
                Greedy.Register();
                Cacophony.Register();
                HexingRounds.Register();
                HexGlasses.Register();
                WickerHeart.Register();
                Book_of_misspelled_spells.Register();
                Fallen_armor.Register();
                FingerTrap.Init();
                WitheringRose.Register();
                LifeInsure.Register();
                Sundail.Register();
                loan.Register();
                StonePotion.Register();  
                MiniGlockodileHeart.Register();
                HexEater.Add();
                Cheshire_purfume.Register();
                DiogenesCoinpurse.Register();
                HellBound.Register();
                Ammochemy_belt.Register();
                ChainFire_Reagent.Register();
                BuiltDifferent.Register();
                PlasmaBuckler.Register();


                //charms
                CharmMaker.Register();

                GnomeCharm.Register();
                KnifeCharm.Register();
                PomaCharm.Register();
                MouseCharm.Register();
                PepperCharm.Register();
                IceCharm.Register();
                ShroomCharm.Register();
                TargetCharm.Register();
                WhammyCharm.Register();
                DangoCharm.Register();
                ClockCharm.Register();
                LuckyStarCharm.Register();
                RubberCharm.Register();
                BatteryCharm.Register();
                PiggyBankCharm.Register();
                ComboCharm.Register();
                FeatherCharm.Register();
                CoffeeCharm.Register();
                MeatCharm.Register();
                SlimeCharm.Register();
                RadioactiveCharm.Register();


                //synergies
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.tomislav() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Daft_Punk() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.split() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.flurry_of_blows() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.BEEES() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Big_problem() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.lich() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Chariot() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.the_World_revolving() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.doubleStandard() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Mozam_hammer() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Mozam_fools() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Mozam_Throw() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Mozam_mazoM() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.MonsterHunter() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.AC() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.DC() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Mas_Queso() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Apex() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Banana() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.BarrelBros() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.JumpShark() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Tempered_dodo() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.DualLoader() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Strike_first() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Mr_Grinch() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Second_Impression() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Scuffed_shoes() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.PinkLemonade() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Walkies() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.AngryMama() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Boomers() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Prepare_for_TitanFall() }).ToArray(); // 33
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Rainbow_in_The_Dark() }).ToArray();   //////////////////////////
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.PaintTheTown() }).ToArray(); 
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Bisharpshooter() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Royals() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Maggies_Favorites() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Mecha_Gunzilla() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Irradiated_blood() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.For_Salvo() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.NoiR() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Insatiable() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.GoYo_AroundTheWorld() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.GoYo_TangledUp() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Orbital_Injection() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.InfiniteMonkeys() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Rampart() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.RainGoAway() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Lok1() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Gravity_Pull() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Burn_it_down() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Rolload() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Honey() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.SwordVPN() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Smart() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.BeastMaster() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.CalculateVectorBetween() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Rotate() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.LerpUnclamped() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Puncture_Wound() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Paper_pusher() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Shoot_For_The_Stars() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Over_The_Moon() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.VenoShock() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Swarm() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Fired_Up() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Brt7() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.ExtraSpicy() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.ToolsOfTheTrade() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.UntilItISDone() }).ToArray();

                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.LabRats() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.SpaceJammin() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.GloryKiller() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Horsey() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.MultiMicro() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.TooMuchBackup() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.RudeBoy() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.ProperKalibration() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.FlipADipDip() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.PerfectForm() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.UltraInstinct() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.GravitonSurge() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.ChainSmoker() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Pots_of_Luck() }).ToArray();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Daft_Hands() }).ToArray();
                CustomSynergies.Add("Upgraded", new List<string> { "ski:sir_lot_o_lance", "alien_engine" }, null, false);
                    //GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { new Customsynergiesknives.Iron_grip() }).ToArray();


                AudioResourceLoader.InitAudio();
                tinyAmmo.Register();
                CharmBauble.Register();
                MeatPickup.Register();

                //shrines
                ShrineFakePrefabHooks.Init();
                ShrineFactory.Init();
                OldShrineFactory.Init();


                CharmingShrine.Add();
                Theatre_Twins.Add();


                ShrineFactory.PlaceBreachShrines();

                //CHARACTERS
                InitCharacters.Charactersetup();
  
                DualGunsManager.AddDual();
                SynergyFormInitialiser.AddSynergyForms();
                
                GameActorillnessEffect.Init();
                BlastBlightedStatusController.Initialise();
                BlobuCrownController.Initialise();
                HexStatusEffectController.Initialise();
                CoolEnemiesStatusController.Initialise();
                Explodia_part_holder_Controller.Initialise();
                ETGModMainBehaviour.Instance.gameObject.GetOrAddComponent<CustomDarknessHandler>();


                ETGMod.AIActor.OnPostStart = (Action<AIActor>)Delegate.Combine(ETGMod.AIActor.OnPostStart, new Action<AIActor>(AssignUnlock));
                System.Random rng = new System.Random();

                //shop
                Morgun.Init();
                ArmsDealer.Init();
                //shop

                if (rng.Next(1, 10) == 1)
                {
                    Log($"Don't bring a {MOD_NAME} v{VERSION}. You'll Win!", TEXT_COLOR);
                }
                else
                {
                    Log($"Don't bring a {MOD_NAME} v{VERSION}. You'll lose!", TEXT_COLOR);
                }
                /*
                List<String> tasks = new List<string>
                {
                    AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_DRAGUN_WITH_MANIC) ? "--- Completed! Earned Mask Twins!\n" : "-- Defeat the Dragun during the Manic Theatre Challenge\n",
                    AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.AMMO_STARVED) ? "--- Completed! Earned Hipp0!\n" : "-- Have two of your guns run out of ammo at once\n",
                    AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.FIRST_IMPRESS) ? "--- Completed! Earned First Impression!\n" : "-- Complete the tutorial in 2:30\n",
                    AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.FLATLINED) ? "--- Completed! Earned FlatLine!\n" : "-- Die to a floor Boss\n",
                    AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HEXLINDED) ? "--- Completed! Earned Witch Watch!\n" : "-- Die to Hex\n",
                    AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HEX_MANIAC) ? "--- Completed! Earned Hex Eater!\n" : "-- Kill Lich with Hex\n",

                    AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.SKATER_HOVER) ? "--- Completed! Earned HoverBoard!\n" : "-- Collect Special Item from Skater's Jackpot.\n",
                    AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.SKATER_JETSET) ? "--- Completed! Earned JetSetter Radio!\n" : "-- Collect Special Item from Skater's Jackpot.\n",
                    AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.SKATER_SLAP) ? "--- Completed! Earned Slap Bass!\n" : "-- Collect Special Item from Skater's Jackpot.\n",
                    AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.SKATER_TAGGAR) ? "--- Completed! Earned Tagg-AR!\n" : "-- Collect Special Item from Skater's Jackpot.\n",

                };
                ETGModConsole.Log("<size=100><color=#5deba4>Knife to a gunfight Unlock tasks list-</color></size>");
                foreach (String task in tasks)
                {
                    ETGModConsole.Log(task); ;
                }
                */
                Commands.Init();
            }
            catch (Exception e)
            {
                ETGModConsole.Log($"<color=#{TEXT_COLOR}>{MOD_NAME}: {e.Message}</color>");
                ETGModConsole.Log(e.StackTrace);

                Log(e.Message);
                Log("\t" + e.StackTrace);
                Log($"Something in Knife_to_a_gunfight broke somewhere...", TEXT_COLOR);
                Log($"If you're reading this please tell me at the gungeon discord and screenshot the error.", TEXT_COLOR);
            }


        }

        public static void Log(string text, string color= "#5deba4")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }

        private void AssignUnlock(AIActor target)
        {
            PlayableCharacters characterIdentity = GameManager.Instance.PrimaryPlayer.characterIdentity;
            
            //Dragun unlock
            if (target.EnemyGuid == "465da2bb086a4a88a803f79fe3a27677")
            {
                target.healthHaver.OnDeath += (obj) =>
                {
                    //Dragun Kill
                    if ((GameManager.Instance.PrimaryPlayer.HasPickupID(ETGMod.Databases.Items["Manic Theatre"].PickupObjectId)))
                    {
                        AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_DRAGUN_WITH_MANIC, true);
                    }
                   
                };
            }

            if (target.EnemyGuid == "7c5d5f09911e49b78ae644d2b50ff3bf")
            {
                target.healthHaver.OnDeath += (obj) =>
                {
                    //infinilich kill
                    if(target.gameObject.GetComponent<HexStatusEffectController>() != null)
                    {
                        if(target.gameObject.GetComponent<HexStatusEffectController>().hitlich)
                        {
                            AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HEX_MANIAC, true);
                        }
                       
                    }

                };

            }
        }
        public static void RunStartHook(Action<PlayerController, float> orig, PlayerController self, float invisibleDelay)
        {
            orig(self, invisibleDelay);
            self.gameObject.GetOrAddComponent<UnlockablesTracker>();
            self.gameObject.GetOrAddComponent<BlobuCrownController>();
            self.gameObject.GetOrAddComponent<JinxItemDisplay>();
        }
        public static void SpecialTutorialHook(Action<PlayerController> orig, PlayerController self)
        {

            orig(self);
            if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.FIRST_IMPRESS) == false)
            {
                self.gameObject.GetOrAddComponent<UnlockablesTracker>();

            }

        }
     
        public void Awake() 
        {

        }
        
    }
}
