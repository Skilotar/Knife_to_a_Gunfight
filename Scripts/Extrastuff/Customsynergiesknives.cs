using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using JetBrains.Annotations;

namespace Knives
{
    class Customsynergiesknives
    {
        public static int LichEyes = 815;
        public class Daft_Punk : AdvancedSynergyEntry
        {

            public Daft_Punk()
            {
                this.NameKey = "Harder! Better! Faster! Stronger!";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                    {
                    daft_helm.ID,
                    punk_helm.ID
                    };
                this.IgnoreLichEyeBullets = true;
                this.statModifiers = new List<StatModifier>(0)
                {
                StatModifier.Create(PlayerStats.StatType.MovementSpeed,StatModifier.ModifyMethod.ADDITIVE, 2f),
                StatModifier.Create(PlayerStats.StatType.KnockbackMultiplier,StatModifier.ModifyMethod.ADDITIVE, .3f),
                StatModifier.Create(PlayerStats.StatType.Damage,StatModifier.ModifyMethod.ADDITIVE, .4f),
                StatModifier.Create(PlayerStats.StatType.Accuracy,StatModifier.ModifyMethod.ADDITIVE, -.2f),
                StatModifier.Create(PlayerStats.StatType.RateOfFire,StatModifier.ModifyMethod.ADDITIVE, .2f)
                };

                this.bonusSynergies = new List<CustomSynergyType>();
                
            }

        }


        /*
        public class Super_Duper_Fly : AdvancedSynergyEntry
        {

            public Super_Duper_Fly()
            {
                this.NameKey = "Super Duper Fly";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                    ETGMod.Databases.Items["Fly Friend"].PickupObjectId,
                    ETGMod.Databases.Items["Super Fly"].PickupObjectId,

                };
                this.IgnoreLichEyeBullets = true;
                this.statModifiers = new List<StatModifier>(0)
                {
                StatModifier.Create(PlayerStats.StatType.Coolness,StatModifier.ModifyMethod.ADDITIVE, 4f),
                 StatModifier.Create(PlayerStats.StatType.Accuracy,StatModifier.ModifyMethod.ADDITIVE, .2f),

                };

                this.bonusSynergies = new List<CustomSynergyType>();
            }

        }
        */
        public class tomislav : AdvancedSynergyEntry
        {

            public tomislav()
            {
                this.NameKey = "Tomislav";
                this.MandatoryGunIDs = new List<int>
                {
                    84
                };
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                    sandvich.ID,
                    
                };
                this.OptionalItemIDs = new List<int>
                {
                   
                    LichEyes
                };
                this.IgnoreLichEyeBullets = true;
                this.statModifiers = new List<StatModifier>(0)
                {
                    StatModifier.Create(PlayerStats.StatType.KnockbackMultiplier,StatModifier.ModifyMethod.ADDITIVE, -.80f),
                    StatModifier.Create(PlayerStats.StatType.Accuracy,StatModifier.ModifyMethod.ADDITIVE, -.80f),
                    StatModifier.Create(PlayerStats.StatType.RateOfFire,StatModifier.ModifyMethod.ADDITIVE, -.20f),
                };

                this.bonusSynergies = new List<CustomSynergyType>();
            }

        }

        public class split : AdvancedSynergyEntry
        {

            public split()
            {
                this.NameKey = "Split Personality";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                    bad_attitude.ID,
                    187

                };
                this.IgnoreLichEyeBullets = true;
                this.statModifiers = new List<StatModifier>(0)
                {

                    StatModifier.Create(PlayerStats.StatType.GlobalPriceMultiplier,StatModifier.ModifyMethod.ADDITIVE, -.1f),
                    StatModifier.Create(PlayerStats.StatType.MoneyMultiplierFromEnemies,StatModifier.ModifyMethod.ADDITIVE, 2f)
                };

                this.bonusSynergies = new List<CustomSynergyType>();
            }


        }
        public class flurry_of_blows : AdvancedSynergyEntry
        {

            public flurry_of_blows()
            {
                this.NameKey = "Flurry Rush";
                this.MandatoryGunIDs = new List<int>
                {
                   hail_2_u.ID
                };
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                     

                };
                this.OptionalItemIDs = new List<int>
                {
                   stardust.ID,
                    Fates_blessing.ID,
                    LichEyes
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                    StatModifier.Create(PlayerStats.StatType.RateOfFire,StatModifier.ModifyMethod.ADDITIVE, 8f),
                    StatModifier.Create(PlayerStats.StatType.Damage,StatModifier.ModifyMethod.ADDITIVE, -.7f),
                    StatModifier.Create(PlayerStats.StatType.DamageToBosses,StatModifier.ModifyMethod.ADDITIVE, -.7f),
                    StatModifier.Create(PlayerStats.StatType.AdditionalClipCapacityMultiplier,StatModifier.ModifyMethod.ADDITIVE, 8f),
                    StatModifier.Create(PlayerStats.StatType.AdditionalGunCapacity,StatModifier.ModifyMethod.ADDITIVE, 1f),

                };

                this.bonusSynergies = new List<CustomSynergyType>();
            }
        }

        public class BEEES : AdvancedSynergyEntry
        {

            public BEEES()
            {
                this.NameKey = "Biolgical Warfare";
                this.MandatoryGunIDs = new List<int>
                {

                };
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                   Rocket_boots.Item_ID

                };
                this.OptionalItemIDs = new List<int>
                {
                    92,14,630,138,
                };
                this.IgnoreLichEyeBullets = true;
                this.statModifiers = new List<StatModifier>(0)
                {


                };

                this.bonusSynergies = new List<CustomSynergyType>();
            }
        }

        /*
        public class nano : AdvancedSynergyEntry
        {

            public nano()
            {
                this.NameKey = "You're powered up!";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                    ETGMod.Databases.Items["nano boost"].PickupObjectId,
                    ETGMod.Databases.Items["nanostone"].PickupObjectId,
                };
                this.IgnoreLichEyeBullets = true;
                this.statModifiers = new List<StatModifier>(0)
                {


                };

                this.bonusSynergies = new List<CustomSynergyType>();
            }
        }*/
        public class Big_problem : AdvancedSynergyEntry
        {

            public Big_problem()
            {
                this.NameKey = "A Really Big Problem";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                    dog.ID,
                    645,


                };
                this.IgnoreLichEyeBullets = true;
                this.statModifiers = new List<StatModifier>(0)
                {

                    
                };

                this.bonusSynergies = new List<CustomSynergyType>();
            }


        }
        public class lich : AdvancedSynergyEntry
        {

            public lich()
            {
                this.NameKey = "Whole Again";
                this.MandatoryGunIDs = new List<int>
                {
                    Za_hando.ID
                };
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                     

                };
                this.OptionalItemIDs = new List<int>
                {
                    213,
                    LichEyes
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                    StatModifier.Create(PlayerStats.StatType.ReloadSpeed,StatModifier.ModifyMethod.ADDITIVE, -.3f),

                    StatModifier.Create(PlayerStats.StatType.RateOfFire,StatModifier.ModifyMethod.ADDITIVE, .2f),


                };

                this.bonusSynergies = new List<CustomSynergyType>();
            }
        }
        public class Chariot : AdvancedSynergyEntry
        {

            public Chariot()
            {
                this.NameKey = "Droppable Armor";
                this.MandatoryGunIDs = new List<int>
                {
                    NewNewCopperChariot.ID
                };
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                     

                };
                this.OptionalItemIDs = new List<int>
                {
                    545,
                    LichEyes
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                  


                };

                this.bonusSynergies = new List<CustomSynergyType>();
            }
        }
        public class the_World_revolving : AdvancedSynergyEntry
        {

            public the_World_revolving()
            {
                this.NameKey = "Chaos Chaos CHAOS!";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                    {
                     ChamberofChambers.ID,
                     SpeedyChamber.ID
                    };
                this.IgnoreLichEyeBullets = true;
                this.statModifiers = new List<StatModifier>(0)
                {
               
                StatModifier.Create(PlayerStats.StatType.ReloadSpeed,StatModifier.ModifyMethod.ADDITIVE, -3f),
                
                StatModifier.Create(PlayerStats.StatType.RateOfFire,StatModifier.ModifyMethod.ADDITIVE, .5f)
                };

                this.bonusSynergies = new List<CustomSynergyType>();
            }

        }

        public class doubleStandard : AdvancedSynergyEntry
        {

            public doubleStandard()
            {
                this.NameKey = "Double Standards";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                    PeaceStandard.ID,
                    529

                };
                this.IgnoreLichEyeBullets = true;
                this.statModifiers = new List<StatModifier>(0)
                {


                };

                this.bonusSynergies = new List<CustomSynergyType>();
            }
        }

        public class Mozam_hammer : AdvancedSynergyEntry
        {

            public Mozam_hammer()
            {
                this.NameKey = "Hop-up: Hammer Point Rounds";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                    {
                     Mozam.ID

                    };
                this.OptionalItemIDs = new List<int>
                    {
                     


                     111,//heavy bullets
                     
                     256,// heavy boots
                     457,//spiked armor
                     271,//riddle of lead
                      LichEyes
                    };
                this.OptionalGunIDs = new List<int>
                {
                     390,//cobalt hammer
                     91,//the hammer
                     610,//woodbeam
                     393,//anvilain,
                     157,// big iron
                     545,//ac15
                     601,//big shotgun
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }
        }
        public class Mozam_fools : AdvancedSynergyEntry
        {
            public Mozam_fools()
            {
                this.NameKey = "Hop-up: April Fools";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                    Mozam.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                    241,// skatter
                    //PickupObjectDatabase.GetByEncounterName("Table Tech Dizzy").PickupObjectId,
                    //PickupObjectDatabase.GetByEncounterName("Slide Tech Slide").PickupObjectId,
                    Eye_of_the_tiger.ID,
                    //409,//tv
                    216, // box
                     LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                    539, // boxing glove
                    340, //lower r
                    10,  //watergun
                    503, //bullet
                    512 // shell
                      
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }

        }

        public class Mozam_Throw : AdvancedSynergyEntry
        {
            public Mozam_Throw()
            {
                this.NameKey = "Hop-up: Throw Away Joke";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                    Mozam.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                   500, // hip holdster
                   LichEyes


                };
                this.OptionalGunIDs = new List<int>
                {
                   510,
                   126, // shotbow
                   8,   // bow
                   31,  // klobbe

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {
                    StatModifier.Create(PlayerStats.StatType.ThrownGunDamage,StatModifier.ModifyMethod.ADDITIVE, 20f),
                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }

        }
        /*
        public class Mozam_Shatter : AdvancedSynergyEntry
        {
            public Mozam_Shatter()
            {
                this.NameKey = "Hop-up: Shattering Tier Lists";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                    PickupObjectDatabase.GetByEncounterName("Mozambique").PickupObjectId,
                    PickupObjectDatabase.GetByEncounterName("World Shatter").PickupObjectId

                };
                this.OptionalItemIDs = new List<int>
                {
                  
                  


                };
                this.OptionalGunIDs = new List<int>
                {
                
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {
                   
                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }

        }
        */
        public class Mozam_mazoM : AdvancedSynergyEntry
        {
            public Mozam_mazoM()
            {
                this.NameKey = "Hop-up: Double Up";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                    Mozam.ID

                };
                this.OptionalItemIDs = new List<int>
                {

                };
                this.OptionalGunIDs = new List<int>
                {
                    93,
                    329,
                    122,
                    51
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }

        }

        public class MonsterHunter : AdvancedSynergyEntry
        {
            public MonsterHunter()
            {
                this.NameKey = "Dragun Slayer";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                   Lance.lnc,
                   

                };
                this.OptionalItemIDs = new List<int>
                {
                    Rage_shield.ID,
                     LichEyes


                };
                this.OptionalGunIDs = new List<int>
                {

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }

        }

        public class DC : AdvancedSynergyEntry
        {
            public DC()
            {
                this.NameKey = "Old Chimes";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                    Hells_bells.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                    237,
                     LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                     506
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }

        }
        public class AC : AdvancedSynergyEntry
        {
            public AC()
            {
                this.NameKey = "New Waves";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                   Hells_bells.ID

                };
                this.OptionalItemIDs = new List<int>
                {

                    469,471,468,470,467,
                     LichEyes

                };
                this.OptionalGunIDs = new List<int>
                {
                     149,
                     230,
                     602,

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }

        }
        public class Mas_Queso : AdvancedSynergyEntry
        {
            public Mas_Queso()
            {
                this.NameKey = "Mas Queso";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                   Sheila.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                     667,
                     663,
                     662,
                      LichEyes



                };
                this.OptionalGunIDs = new List<int>
                {
                     626
                     

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }

        }
        /*
        public class Iron_grip : AdvancedSynergyEntry
        {
            public Iron_grip()
            {
                this.NameKey = "Iron Grip";
                this.MandatoryItemIDs = new List<int> //Look in the items ID map in the gungeon code for the ids.
                {
                    punt.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                     
                     256




                };
                this.OptionalGunIDs = new List<int>
                {
                    


                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }
            

        }
        */
        public class Banana : AdvancedSynergyEntry
        {
            public Banana()
            {
                this.NameKey = "Bananarmaments";
                this.MandatoryItemIDs = new List<int>
                {
                    MonkeyBarrel.mbID


                };
                this.OptionalItemIDs = new List<int>
                {


                     LichEyes

                };
                this.OptionalGunIDs = new List<int>
                {
                    478

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class BarrelBros : AdvancedSynergyEntry
        {
            public BarrelBros()
            {
                this.NameKey = "Barrel Bros";
                this.MandatoryItemIDs = new List<int>
                {
                    


                };
                this.OptionalItemIDs = new List<int>
                {

                     LichEyes


                };
                this.OptionalGunIDs = new List<int>
                {
                   7,
                    MonkeyBarrel.mbID,
                    MBSynergyForm.mbakID

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        
        public class Apex : AdvancedSynergyEntry
        {
            public Apex()
            {
                this.NameKey = "Apex Predator";
                this.MandatoryItemIDs = new List<int>
                {

                     MonkeyBarrel.mbID

                };
                this.OptionalItemIDs = new List<int>
                {
                    Banana_Mag.ID,

                     LichEyes

                };
                this.OptionalGunIDs = new List<int>
                {
                     PickupObjectDatabase.GetById(15).PickupObjectId

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        
        public class JumpShark : AdvancedSynergyEntry
        {
            public JumpShark()
            {
                this.NameKey = "Jump The Shark";
                this.MandatoryItemIDs = new List<int>
                {
                    FishBones.ID


                };
                this.OptionalItemIDs = new List<int>
                {
                     LichEyes



                };
                this.OptionalGunIDs = new List<int>
                {
                    359,


                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        
        /*
        public class Noir : AdvancedSynergyEntry
        {
            public Noir()
            {
                this.NameKey = "Stormy Nights";
                this.MandatoryItemIDs = new List<int>
                {
                    PickupObjectDatabase.GetByEncounterName("Chicago Typewriter").PickupObjectId,


                };
                this.OptionalItemIDs = new List<int>
                {
                    PickupObjectDatabase.GetByEncounterName("Cigarlet").PickupObjectId,



                };
                this.OptionalGunIDs = new List<int>
                {
                     PickupObjectDatabase.GetByEncounterName("Baba Yaga").PickupObjectId,

                     PickupObjectDatabase.GetByEncounterName("Le'Voleur").PickupObjectId,
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        */



        public class Tempered_dodo : AdvancedSynergyEntry
        {
            public Tempered_dodo()
            {
                this.NameKey = "Tempered Dodogama";
                this.MandatoryItemIDs = new List<int>
                {
                    BabyGoodDodoGama.ID


                };
                this.OptionalItemIDs = new List<int>
                {
                    Rage_shield.ID,

                    403,

                };
                this.OptionalGunIDs = new List<int>
                {
                     GunLance.ID,

                     Lance.lnc
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class Strike_first : AdvancedSynergyEntry
        {
            public Strike_first()
            {
                this.NameKey = "Strike First";
                this.MandatoryItemIDs = new List<int>
                {
                    FirstImpression.StandardID

                };
                this.OptionalItemIDs = new List<int>
                {
                   373,
                    LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                   
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class Mr_Grinch : AdvancedSynergyEntry
        {
            public Mr_Grinch()
            {
                this.NameKey = "Mr. Grinch";
                this.MandatoryItemIDs = new List<int>
                {
                    Present.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                   663,// rat sack
                   364,//ice heart

                };
                this.OptionalGunIDs = new List<int>
                {

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }

        }

        public class Second_Impression : AdvancedSynergyEntry
        {
            public Second_Impression()
            {
                this.NameKey = "Second Impression";
                this.MandatoryItemIDs = new List<int>
                {
                    FirstImpression.StandardID,
                    EjectButton.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                   
                };
                this.OptionalGunIDs = new List<int>
                {

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class Scuffed_shoes : AdvancedSynergyEntry
        {
            public Scuffed_shoes()
            {
                this.NameKey = "Scuffed Soles";
                this.MandatoryItemIDs = new List<int>
                {
                    Mares_Leg.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                    315,
                    Rocket_boots.Item_ID,
                    Long_roll_boots.ID,
                    526,
                    667,
                };
                this.OptionalGunIDs = new List<int>
                {

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        /*
        public class Bandit_extraordinaire : AdvancedSynergyEntry
        {
            public Bandit_extraordinaire()
            {
                this.NameKey = "Bandit Extraordinaire";
                this.MandatoryItemIDs = new List<int>
                {
                    PickupObjectDatabase.GetByEncounterName("Mares Leg").PickupObjectId,
                    PickupObjectDatabase.GetByEncounterName("StarBurst").PickupObjectId,
                    PickupObjectDatabase.GetByEncounterName("LoneStar").PickupObjectId,


                };
                this.OptionalItemIDs = new List<int>
                {
                   
                };
                this.OptionalGunIDs = new List<int>
                {

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {
                    StatModifier.Create(PlayerStats.StatType.ReloadSpeed,StatModifier.ModifyMethod.ADDITIVE, -.5f),
                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        */
       
        public class DualLoader : AdvancedSynergyEntry
        {
            public DualLoader()
            {
                this.NameKey = "Dual Loader";
                this.MandatoryItemIDs = new List<int>
                {


                    Lone.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                    213,
                    396,
                    168,
                     LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                    9,
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class Boomers : AdvancedSynergyEntry
        {
            public Boomers()
            {
                this.NameKey = "Boomers";
                this.MandatoryItemIDs = new List<int>
                {
                    Express.ID,
                    Queen.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                    LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                   
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {
                    StatModifier.Create(PlayerStats.StatType.ReloadSpeed,StatModifier.ModifyMethod.ADDITIVE, -.1f),
                    StatModifier.Create(PlayerStats.StatType.RateOfFire,StatModifier.ModifyMethod.ADDITIVE, -.2f),

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class PinkLemonade : AdvancedSynergyEntry
        {
            public PinkLemonade()
            {
                this.NameKey = "Pink Lemonade";
                this.MandatoryItemIDs = new List<int>
                {


                    Lemonade.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                    527,

                };
                this.OptionalGunIDs = new List<int>
                {
                   379,
                   200,
                   Bad_Name.ID

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class Walkies : AdvancedSynergyEntry
        {
            public Walkies()
            {
                this.NameKey = "Walk The Dog";
                this.MandatoryItemIDs = new List<int>
                {


                    GoYo.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                    //300, dog
                    492, // angry dog
                    dog.ID,
                    LichEyes

                };
                this.OptionalGunIDs = new List<int>
                {
                  

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class AngryMama : AdvancedSynergyEntry
        {
            public AngryMama()
            {
                this.NameKey = "Angry Mama";
                this.MandatoryItemIDs = new List<int>
                {


                    Bombushka.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                    323,
                    bad_attitude.ID,
                    Rage_shield.ID

                };
                this.OptionalGunIDs = new List<int>
                {
                     Bab.ID
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class Prepare_for_TitanFall : AdvancedSynergyEntry
        {
            public Prepare_for_TitanFall()
            {
                this.NameKey = "Prepare for Titanfall";
                this.MandatoryItemIDs = new List<int>
                {
                    P2020_item.ID
                };
                this.OptionalItemIDs = new List<int>
                {

                     Spark.ID

                };
                this.OptionalGunIDs = new List<int>
                {
                    Mozam.ID,
                    ChargeRifle.ID,
                    Watch_Standard.StandardID,
                    Watch_Charged.Charged,
                    Rampage.ID,
                    ThrustStabilizers.ID,
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {
                    StatModifier.Create(PlayerStats.StatType.MovementSpeed,StatModifier.ModifyMethod.ADDITIVE, .5f),
                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class Rainbow_in_The_Dark : AdvancedSynergyEntry
        {
            public Rainbow_in_The_Dark()
            {
                this.NameKey = "Rainbow In The Dark";
                this.MandatoryItemIDs = new List<int>
                {
                    SIND.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                     LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                    100
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {
                    StatModifier.Create(PlayerStats.StatType.MovementSpeed,StatModifier.ModifyMethod.ADDITIVE, .5f),
                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class PaintTheTown : AdvancedSynergyEntry
        {
            public PaintTheTown()
            {
                this.NameKey = "Paint The Town RED";
                this.MandatoryItemIDs = new List<int>
                {
                   Rampage.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                    253,
                    242,
                     LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                    384,
                    125,
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {
                    
                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }


        public class Bisharpshooter : AdvancedSynergyEntry
        {
            public Bisharpshooter()
            {
                this.NameKey = "Bisharpshooter";
                this.MandatoryItemIDs = new List<int>
                {
                    King.ID,
                    

                };
                this.OptionalItemIDs = new List<int>
                {
                    273,
                    NewHolyGrenade.ID,
                     LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {
                    
                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class Royals : AdvancedSynergyEntry
        {
            public Royals()
            {
                this.NameKey = "King and Queen";
                this.MandatoryItemIDs = new List<int>
                {
                    King.ID,
                    Queen.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                     LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class Maggies_Favorites : AdvancedSynergyEntry
        {
            public Maggies_Favorites()
            {
                this.NameKey = "Warlord's Best Friend";
                this.MandatoryItemIDs = new List<int>
                {
                    Warlord_Ire.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                    Mares_Leg.ID,
                };
                this.OptionalGunIDs = new List<int>
                {
                    Mozam.ID,
                    PeaceMaker.ID,
                    Superstar.ID,
                    143,
                    601,
                    51,
                    
                    1,

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {
                    StatModifier.Create(PlayerStats.StatType.ReloadSpeed,StatModifier.ModifyMethod.ADDITIVE, -.10f),
                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class Mecha_Gunzilla : AdvancedSynergyEntry
        {
            public Mecha_Gunzilla()
            {
                this.NameKey = "Mecha Gunzilla";
                this.MandatoryItemIDs = new List<int>
                {
                    GunZilla_Tail.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                    114,
                    318,
                    314,

                };
                this.OptionalGunIDs = new List<int>
                {
                    575,

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {
                   
                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class Irradiated_blood : AdvancedSynergyEntry
        {
            public Irradiated_blood()
            {
                this.NameKey = "Irradiated Blood";
                this.MandatoryItemIDs = new List<int>
                {
                    GunZilla_Tail.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                   204,
                   313,

                };
                this.OptionalGunIDs = new List<int>
                {
                    329,
                    87
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class For_Salvo : AdvancedSynergyEntry
        {
            public For_Salvo()
            {
                this.NameKey = "Freedom For Salvo";
                this.MandatoryItemIDs = new List<int>
                {

                    Riot_Drill.ID,

                  
                };
                this.OptionalItemIDs = new List<int>
                {
                   LichEyes

                };
                this.OptionalGunIDs = new List<int>
                {
                      Stop.ID

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class NoiR : AdvancedSynergyEntry
        {
            public NoiR()
            {
                this.NameKey = "NoiR";
                this.MandatoryItemIDs = new List<int>
                {
                    Typewriter.ID,
                    
                   
                };
                this.OptionalItemIDs = new List<int>
                {

                    LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                    340

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class Insatiable : AdvancedSynergyEntry
        {
            public Insatiable()
            {
                this.NameKey = "Insatiable";
                this.MandatoryItemIDs = new List<int>
                {
                    Hippo.ID,
                    

                };
                this.OptionalItemIDs = new List<int>
                {
                    655,
                    LichEyes

                };
                this.OptionalGunIDs = new List<int>
                {
                    169

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class GoYo_AroundTheWorld : AdvancedSynergyEntry
        {
            public GoYo_AroundTheWorld()
            {
                this.NameKey = "Around The World";
                this.MandatoryItemIDs = new List<int>
                {
                    GoYo.ID,


                };
                this.OptionalItemIDs = new List<int>
                {
                    daft_helm.ID,
                    punk_helm.ID,
                    491,
                    661,
                    232,
                    301,
                    LichEyes

                };
                this.OptionalGunIDs = new List<int>
                {
                    597,
                    GravitySlingshot.ID,
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class GoYo_TangledUp : AdvancedSynergyEntry
        {
            public GoYo_TangledUp()
            {
                this.NameKey = "All Tangled Up";
                this.MandatoryItemIDs = new List<int>
                {
                    GoYo.ID,

                };
                this.OptionalItemIDs = new List<int>
                {
                    323,

                    LichEyes

                };
                this.OptionalGunIDs = new List<int>
                {
                    175
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class Orbital_Injection : AdvancedSynergyEntry
        {
            public Orbital_Injection()
            {
                this.NameKey = "Orbital Injection";
                this.MandatoryItemIDs = new List<int>
                {
                    GravitySlingshot.ID
                    
                };
                this.OptionalItemIDs = new List<int>
                {
                    232,
                    491,
                    661,
                    LichEyes

                };
                this.OptionalGunIDs = new List<int>
                {
                    597,
                    208,
                    20

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class InfiniteMonkeys : AdvancedSynergyEntry
        {
            public InfiniteMonkeys()
            {
                this.NameKey = "Infinite Monkey Theorem";
                this.MandatoryItemIDs = new List<int>
                {
                    Typewriter.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                    MonkeyBarrel.mbID,
                    MBSynergyForm.mbakID

                };
                this.OptionalGunIDs = new List<int>
                {
                    
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class Rampart : AdvancedSynergyEntry
        {
            public Rampart()
            {
                this.NameKey = "Projected Bastion";
                this.MandatoryItemIDs = new List<int>
                {
                    Sheila.ID,
                };
                this.OptionalItemIDs = new List<int>
                {
                    shield.ID,
                    314
                };
                this.OptionalGunIDs = new List<int>
                {
                    380,

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class RainGoAway : AdvancedSynergyEntry
        {
            public RainGoAway()
            {
                this.NameKey = "Rain Rain Go Away";
                this.MandatoryItemIDs = new List<int>
                {
                    Umbra_main.BaseID,
                };
                this.OptionalItemIDs = new List<int>
                {
                    64,
                    447,
                    264,
                   

                };
                this.OptionalGunIDs = new List<int>
                {
                     380,
                     LichEyes
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        /*
        public class Its_raining_kin : AdvancedSynergyEntry
        {
            public Its_raining_kin()
            {
                this.NameKey = "It's Raining Kin";
                this.MandatoryItemIDs = new List<int>
                {
                    Umbra_main.BaseID,
                };
                this.OptionalItemIDs = new List<int>
                {
                   

                };
                this.OptionalGunIDs = new List<int>
                {
                     38,
                     551,
                     51,
                     15,
                     LichEyes
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        */
        public class Lok1 : AdvancedSynergyEntry
        {
            public Lok1()
            {
                this.NameKey = "Expanded Ram";
                this.MandatoryItemIDs = new List<int>
                {
                    EvilEye.ID,
                };
                this.OptionalItemIDs = new List<int>
                {
                    Malware.ID,
                    LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                    USB.ID,
                    BattlingRam.ID,
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class Gravity_Pull : AdvancedSynergyEntry
        {
            public Gravity_Pull()
            {
                this.NameKey = "Gravity Pull";
                this.MandatoryItemIDs = new List<int>
                {
                    GravitySlingshot.ID,

                };
                this.OptionalItemIDs = new List<int>
                {
                   536,
                   155,
                    LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                    169,
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class Burn_it_down : AdvancedSynergyEntry
        {
            public Burn_it_down()
            {
                this.NameKey = "Burn The House Down!";
                this.MandatoryItemIDs = new List<int>
                {
                    Rampage.ID,

                };
                this.OptionalItemIDs = new List<int>
                {
                   295,
                   
                    LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                    275,
                    83,
                    60,
                    336,

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class Rolload : AdvancedSynergyEntry
        {
            public Rolload()
            {
                this.NameKey = "Rolload";
                this.MandatoryItemIDs = new List<int>
                {
                    King.ID,

                };
                this.OptionalItemIDs = new List<int>
                {
                   190,
                   315,
                   165,
                   Mares_Leg.ID,
                   Chamber_pendant.ID,
                   526,
                   Rocket_boots.Item_ID,
                   567,
                    LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                    
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        /*
        public class SaveFile : AdvancedSynergyEntry
        {
            public SaveFile()
            {
                this.NameKey = "Save_State_Load";
                this.MandatoryItemIDs = new List<int>
                {
                    USB.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                    Malware.ID,
                    165,
                    536,
                    439,
                    634,
                    375,
                    Chamber_pendant.ID,
                    Slide_reload.ID,
                    
                    LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                    Hippo.ID
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        */


        public class SwordVPN : AdvancedSynergyEntry
        {
            public SwordVPN()
            {
                this.NameKey = "SwordVPN";
                this.MandatoryItemIDs = new List<int>
                {
                    Sponsword.StandardID

                };
                this.OptionalItemIDs = new List<int>
                {
                    Malware.ID,
                    Broken_controller.ID,
                    LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                    USB.ID,
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class Honey : AdvancedSynergyEntry
        {
            public Honey()
            {
                this.NameKey = "Honey";
                this.MandatoryItemIDs = new List<int>
                {
                    Sponsword.StandardID

                };
                this.OptionalItemIDs = new List<int>
                {
                    432,
                    Bee.ID,
                    LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                    14,
                    92,
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class Smart : AdvancedSynergyEntry
        {
            public Smart()
            {
                this.NameKey = "Seer Kit";
                this.MandatoryItemIDs = new List<int>
                {
                    P2020_item.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                    Malware.ID,
                    Pilots_Helmet.ID,
                };
                this.OptionalGunIDs = new List<int>
                {
                    USB.ID,
                    EvilEye.ID,
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class BeastMaster : AdvancedSynergyEntry
        {
            public BeastMaster()
            {
                this.NameKey = "BeastMaster";
                this.MandatoryItemIDs = new List<int>
                {
                    RockAndStone.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                    249, // owl.
                    301, // space turt
                    645, // turtle
                    LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                    369, // bait
                   
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class CalculateVectorBetween : AdvancedSynergyEntry
        {
            public CalculateVectorBetween()
            {
                this.NameKey = "CalculateVectorBetween(Enemy);";
                this.MandatoryItemIDs = new List<int>
                {
                    VectorMKTwo.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                    323,
                    284,
                    273,
                    LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                    
                   
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class LerpUnclamped : AdvancedSynergyEntry
        {
            public LerpUnclamped()
            {
                this.NameKey = "LerpUnclamped();";
                this.MandatoryItemIDs = new List<int>
                {
                    VectorMKTwo.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                   LichEyes,
                   270,
                   69
                };
                this.OptionalGunIDs = new List<int>
                {


                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class Rotate : AdvancedSynergyEntry
        {
            public Rotate()
            {
                this.NameKey = "Rotate();";
                this.MandatoryItemIDs = new List<int>
                {
                    VectorMKTwo.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                   LichEyes,
                   260,
                   262,
                   263,
                   264,
                   466,
                   269,
                   Stopda.ID,
                   GlassBoi.ID,
                   Micro.ID
                };
                this.OptionalGunIDs = new List<int>
                {


                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class Paper_pusher : AdvancedSynergyEntry
        {
            public Paper_pusher()
            {
                this.NameKey = "Paper Pusher";
                this.MandatoryItemIDs = new List<int>
                {
                    Callahan.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                   LichEyes,
                   216,
                   BombBag.ID,
                };
                this.OptionalGunIDs = new List<int>
                {
                    Typewriter.ID,
                    477,

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class Puncture_Wound : AdvancedSynergyEntry
        {
            public Puncture_Wound()
            {
                this.NameKey = "Puncture Wound";
                this.MandatoryItemIDs = new List<int>
                {
                    Callahan.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                   LichEyes,
                   63,
                   240,
                   259,
                   437,
                   285

                };
                this.OptionalGunIDs = new List<int>
                {


                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class Shoot_For_The_Stars : AdvancedSynergyEntry
        {
            public Shoot_For_The_Stars()
            {
                this.NameKey = "Shoot for the Stars";
                this.MandatoryItemIDs = new List<int>
                {
                    DoubleWHAMMY.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                    LichEyes,
                    stardust.ID,
                    433,
                };
                this.OptionalGunIDs = new List<int>
                {
                    Lone.ID,
                    Superstar.ID,   
                    StarBurst.ID,
                    52,
                    507,

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class Over_The_Moon : AdvancedSynergyEntry
        {
            public Over_The_Moon()
            {
                this.NameKey = "Over The Moon";
                this.MandatoryItemIDs = new List<int>
                {
                    DoubleWHAMMY.ID

                };
                this.OptionalItemIDs = new List<int>
                {
                    
                    
                };
                this.OptionalGunIDs = new List<int>
                {
                    Pikannon.ID,
                    20, 
                    597,
                    162
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class VenoShock : AdvancedSynergyEntry
        {
            public VenoShock()
            {
                this.NameKey = "Venoshock";
                this.MandatoryItemIDs = new List<int>
                {
                    Fell.ID,
                    LichEyes
                };
                this.OptionalItemIDs = new List<int>
                {
                    204,
                    313,


                };
                this.OptionalGunIDs = new List<int>
                {
                   207,
                   208,

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class Swarm : AdvancedSynergyEntry
        {
            public Swarm()
            {
                this.NameKey = "Swarmageddon";
                this.MandatoryItemIDs = new List<int>
                {
                    Fell.ID,
                    LichEyes
                };
                this.OptionalItemIDs = new List<int>
                {
                    432,
                    630,
                    138,
                };
                this.OptionalGunIDs = new List<int>
                {
                    Bee.ID,
                    14,
                    92
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class Fired_Up : AdvancedSynergyEntry
        {
            public Fired_Up()
            {
                this.NameKey = "Thermal Linkage";
                this.MandatoryItemIDs = new List<int>
                {
                    Rampage.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                    
                };
                this.OptionalGunIDs = new List<int>
                {
                    Watch_Standard.StandardID,
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class Brt7 : AdvancedSynergyEntry
        {
            public Brt7()
            {
                this.NameKey = "Plasma Spray";
                this.MandatoryItemIDs = new List<int>
                {
                    Followup.ID
                };
                this.OptionalItemIDs = new List<int>
                {

                };
                this.OptionalGunIDs = new List<int>
                {
                    RockAndStone.ID,

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        
        public class ExtraSpicy : AdvancedSynergyEntry
        {
            public ExtraSpicy()
            {
                this.NameKey = "Extra Spicy";
                this.MandatoryItemIDs = new List<int>
                {
                    Pikannon.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                    
                    295,
                    666,
                    LichEyes
                };
                this.OptionalGunIDs = new List<int>
                {
                    hot_coffee.ID,
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class ToolsOfTheTrade : AdvancedSynergyEntry
        {
            public ToolsOfTheTrade()
            {
                this.NameKey = "Tools of the Heros";
                this.MandatoryItemIDs = new List<int>
                {
                    Fourk.StandardID,
                };
                this.OptionalItemIDs = new List<int>
                {
                    109,
                    448,
                    108,
                };
                this.OptionalGunIDs = new List<int>
                {
                   8
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }


        public class UntilItISDone : AdvancedSynergyEntry
        {
            public UntilItISDone()
            {
                this.NameKey = "Until It Is Done!";
                this.MandatoryItemIDs = new List<int>
                {
                    DoomShotty.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                   
                };
                this.OptionalGunIDs = new List<int>
                {
                   21, // BSG
                   60, // demon head
                   464, // shelegun
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class LabRats : AdvancedSynergyEntry
        {
            public LabRats()
            {
                this.NameKey = "LabRats";
                this.MandatoryItemIDs = new List<int>
                {
                    Ratapult.ID,
                    RatCrown.ID
                };
                this.OptionalItemIDs = new List<int>
                {

                };
                this.OptionalGunIDs = new List<int>
                {
                   
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class SpaceJammin : AdvancedSynergyEntry
        {
            public SpaceJammin()
            {
                this.NameKey = "Space Jammin";
                this.MandatoryItemIDs = new List<int>
                {
                    Luna.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                    EmergencyDanceParty.ID,
                    daft_helm.ID,
                    punk_helm.ID,
                    JetSetterRadio.ID,
                    disco_inferno.ID
                   
                };
                this.OptionalGunIDs = new List<int>
                {
                    149,

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class GloryKiller : AdvancedSynergyEntry
        {
            public GloryKiller()
            {
                this.NameKey = "Glory Kill!";
                this.MandatoryItemIDs = new List<int>
                {
                    Chainsaw.StandardID
                };
                this.OptionalItemIDs = new List<int>
                {
                    
                };
                this.OptionalGunIDs = new List<int>
                {
                    DoomShotty.ID,
                    21
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class Horsey : AdvancedSynergyEntry
        {
            public Horsey()
            {
                this.NameKey = "200% More Horse";
                this.MandatoryItemIDs = new List<int>
                {
                    Lance.lnc,
                    HobbyHorse.ID
                };
                this.OptionalItemIDs = new List<int>
                {

                };
                this.OptionalGunIDs = new List<int>
                {
                    
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class MultiMicro : AdvancedSynergyEntry
        {
            public MultiMicro()
            {
                this.NameKey = "Backup's Backup";
                this.MandatoryItemIDs = new List<int>
                {
                    Micro.ID,
                    The_Backup.ID
                };
                this.OptionalItemIDs = new List<int>
                {

                };
                this.OptionalGunIDs = new List<int>
                {

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class TooMuchBackup : AdvancedSynergyEntry
        {
            public TooMuchBackup()
            {
                this.NameKey = "Calling All Units";
                this.MandatoryItemIDs = new List<int>
                {
                    
                    The_Backup.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                    287, //backup gun
                    140, //cop hat
                    312, //blast helmet
                };
                this.OptionalGunIDs = new List<int>
                {

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class RudeBoy : AdvancedSynergyEntry
        {
            public RudeBoy()
            {
                this.NameKey = "Smoking Joe";
                this.MandatoryItemIDs = new List<int>
                {
                    CheatersDrawArm.ID,
                    
                };
                this.OptionalItemIDs = new List<int>
                {
                   500,
                   bad_attitude.ID,

                };
                this.OptionalGunIDs = new List<int>
                {
                    384,
                    125,

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class FlipADipDip : AdvancedSynergyEntry
        {
            public FlipADipDip()
            {
                this.NameKey = "Flip-A-Dip-Dip!";
                this.MandatoryItemIDs = new List<int>
                {
                    Fliplets.ID,

                };
                this.OptionalItemIDs = new List<int>
                {
                    115,
                    290,
                };
                this.OptionalGunIDs = new List<int>
                {
                    566,
                    149,

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class ProperKalibration : AdvancedSynergyEntry
        {
            public ProperKalibration()
            {
                this.NameKey = "Proper Kalibration";
                this.MandatoryItemIDs = new List<int>
                {
                    EndoThermic.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                    439,
                    135,
                    LichEyes,
                };
                this.OptionalGunIDs = new List<int>
                {
                    365,
                    331,

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class PerfectForm : AdvancedSynergyEntry
        {
            public PerfectForm()
            {
                this.NameKey = "Perfect Form";
                this.MandatoryItemIDs = new List<int>
                {
                    Phalanx.ID,
                };
                this.OptionalItemIDs = new List<int>
                {
                    148, //lies
                    LichEyes,
                };
                this.OptionalGunIDs = new List<int>
                {
                   380
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class UltraInstinct : AdvancedSynergyEntry
        {
            public UltraInstinct()
            {
                this.NameKey = "Autonomus Ultra Instinct";
                this.MandatoryItemIDs = new List<int>
                {
                    AutonomusDefenseDrive.ID,
                };
                this.OptionalItemIDs = new List<int>
                {
                    Malware.ID,
                    821, //scouter
                };
                this.OptionalGunIDs = new List<int>
                {
                    USB.ID,
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class GravitonSurge : AdvancedSynergyEntry
        {
            public GravitonSurge()
            {
                this.NameKey = "Graviton Surge";
                this.MandatoryItemIDs = new List<int>
                {
                    ParticleCannon.ID,
                };
                this.OptionalItemIDs = new List<int>
                {
                    155
                };
                this.OptionalGunIDs = new List<int>
                {
                    169,
                    597,
                    LichEyes,
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class ChainSmoker : AdvancedSynergyEntry
        {
            public ChainSmoker()
            {
                this.NameKey = "Chain Smoker";
                this.MandatoryItemIDs = new List<int>
                {
                    Cigaretta.ID,
                };
                this.OptionalItemIDs = new List<int>
                {
                    SmokeAmmolet.ID,
                    203,
                    LichEyes,
                };
                this.OptionalGunIDs = new List<int>
                {
                    
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        public class Pots_of_Luck : AdvancedSynergyEntry
        {
            public Pots_of_Luck()
            {
                this.NameKey = "Pot Smasher";
                this.MandatoryItemIDs = new List<int>
                {
                    Pot.ID,
                };
                this.OptionalItemIDs = new List<int>
                {
                    667,//hoverboots
                    572,// ocarina
                    8, //bow
                    LichEyes,
                };
                this.OptionalGunIDs = new List<int>
                {
                    Fourk.StandardID,

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }

        public class Daft_Hands : AdvancedSynergyEntry
        {
            public Daft_Hands()
            {
                this.NameKey = "Daft Hands";
                this.MandatoryItemIDs = new List<int>
                {
                    Deft_Hands.ID
                };
                this.OptionalItemIDs = new List<int>
                {
                   daft_helm.ID,
                   punk_helm.ID
                };
                this.OptionalGunIDs = new List<int>
                {
                   

                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {

                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }


        }
        //
    }
}
