using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using Alexandria.ItemAPI;

namespace Knives
{
    public class OneGunBloodBroochMod : MonoBehaviour
    {

        public OneGunBloodBroochMod()
        {

        }


        private void Start()
        {
            this.gun = base.gameObject.GetComponent<Gun>();
            this.Boi = gun.CurrentOwner as PlayerController;

            Boi.OnDealtDamageContext += Boi_OnDealtDamageContext;
        }

        private void Boi_OnDealtDamageContext(PlayerController arg1, float arg2, bool arg3, HealthHaver arg4)
        {
            if(this.gun == Boi.CurrentGun)
            {
                Counter -= arg2;
            }
        }

        private void Update()
        {
            if (this.gun == Boi.CurrentGun)
            {
                if (Counter <= 0)
                {
                    Boi.healthHaver.ApplyHealing(.5f);
                    Counter = max;
                }
            }
        }


        public bool IsItThisGun(Gun gun)
        {
            bool yes = true;
            if (gun != this.gun)
            {
                yes = false;
            }

            return yes;
        }


        public float Counter = 800;
        public float max = 800;
        private Gun gun;
        private PlayerController Boi;
        


    }
}
