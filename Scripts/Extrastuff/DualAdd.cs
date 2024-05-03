
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Knives
{
    class DualGunsManager
    {
        public static void AddDual()
        {
            DualWieldController doubleup = PickupObjectDatabase.GetById(51).gameObject.AddComponent<DualWieldController>();
            doubleup.PartnerGunID = Mozam.ID;

            DualWieldController doubleup2 = PickupObjectDatabase.GetById(93).gameObject.AddComponent<DualWieldController>();
            doubleup2.PartnerGunID = Mozam.ID;

            DualWieldController doubleup3 = PickupObjectDatabase.GetById(122).gameObject.AddComponent<DualWieldController>();
            doubleup3.PartnerGunID = Mozam.ID;

            DualWieldController doubleup4 = PickupObjectDatabase.GetById(329).gameObject.AddComponent<DualWieldController>();
            doubleup4.PartnerGunID = Mozam.ID;

            DualWieldController barrelbros = PickupObjectDatabase.GetById(7).gameObject.AddComponent<DualWieldController>();
            barrelbros.PartnerGunID = MonkeyBarrel.mbID;

            DualWieldController barrelbros2 = PickupObjectDatabase.GetById(MonkeyBarrel.mbID).gameObject.AddComponent<DualWieldController>();
            barrelbros2.PartnerGunID = PickupObjectDatabase.GetById(7).PickupObjectId;

            //DualWieldController SuperStars = PickupObjectDatabase.GetByEncounterName("LoneStar").gameObject.AddComponent<DualWieldController>();
            //SuperStars.PartnerGunID = PickupObjectDatabase.GetByEncounterName("StarBurst").PickupObjectId;


            //DualWieldController Gloves = PickupObjectDatabase.GetByEncounterName("Lefty").gameObject.AddComponent<DualWieldController>();
            //Gloves.PartnerGunID = PickupObjectDatabase.GetByEncounterName("Righty").PickupObjectId;
        }
    }
}