using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using Alexandria.ItemAPI;

namespace Knives
{
	public class OneGunMetronomeModifier : MonoBehaviour
	{

		public OneGunMetronomeModifier()
		{

		}


		private void Start()
		{
			this.gun = base.gameObject.GetComponent<Gun>();
			this.Boi = gun.CurrentOwner as PlayerController;
            Boi.OnKilledEnemy += Boi_OnKilledEnemy;
            Boi.OnReceivedDamage += Boi_OnReceivedDamage;
			

            doubleE = (PickupObjectDatabase.GetById(119).GetComponent<MetronomeItem>().doubleEighthNoteSprite);
            Boi.GunChanged += Boi_GunChanged;
            gun.OnDropped += this.ondrop;

            this.MetroModifier = new StatModifier();
            this.MetroModifier.statToBoost = PlayerStats.StatType.Damage;
            this.MetroModifier.modifyType = StatModifier.ModifyMethod.ADDITIVE;

            var gradient = new Gradient();

            // Blend color from red at 0% to blue at 100%
            var colors = new GradientColorKey[5];
            colors[0] = new GradientColorKey(Color.blue, 0.0f);
            colors[1] = new GradientColorKey(Color.green, .25f);
            colors[2] = new GradientColorKey(Color.yellow, .50f);
            colors[3] = new GradientColorKey(new Color(1, .69f, 0), .75f);
            colors[4] = new GradientColorKey(Color.red, 1f);

            // Blend alpha from opaque at 0% to transparent at 100%
            var alphas = new GradientAlphaKey[2];
            alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
            alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

            gradient.SetKeys(colors, alphas);

            colorGradient = gradient;
            colorGradient.mode = GradientMode.Blend;

        }

        public bool IsItThisGun(Gun gun)
        {
            bool yes = true;
            if(gun != this.gun)
            {
                yes = false;
            }

            return yes;
        }

        private void ondrop()
        {
            combonumber = 0;
           
            AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", base.gameObject);
            AkSoundEngine.SetRTPCValue("Pitch_Metronome", 0f);
            Boi.ownerlessStatModifiers.Remove(MetroModifier);
            Boi.stats.RecalculateStats(Boi, true);
        }

        private void Boi_GunChanged(Gun oldgun, Gun newgun, bool arg3)
        {
            if (IsItThisGun(oldgun))
            {
                if(combonumber > 0)
                {
                    combonumber = 0;
                    AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", base.gameObject);
                    AkSoundEngine.SetRTPCValue("Pitch_Metronome", 0f);
                    Boi.ownerlessStatModifiers.Remove(MetroModifier);
                    Boi.stats.RecalculateStats(Boi, true);
                }
               
            }
           
        }


        private void Boi_OnReceivedDamage(PlayerController obj)
        {
            if (IsItThisGun(obj.CurrentGun))
            {
                combonumber = 0;
                AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", base.gameObject);
                AkSoundEngine.SetRTPCValue("Pitch_Metronome", 0f);
                Boi.ownerlessStatModifiers.Remove(MetroModifier);
                Boi.stats.RecalculateStats(Boi, true);
            }
        }

        private void Boi_OnKilledEnemy(PlayerController obj)
        {
            if (IsItThisGun(obj.CurrentGun))
            {
                if (combonumber < 100)
                {
                    combonumber = combonumber + 1;
                    DoMetroUp();
                }
                else
                {
                    combonumber = 100;
                }
            }
		}

        private void DoMetroUp()
        {
            AkSoundEngine.SetRTPCValue("Pitch_Metronome", combonumber);
            AkSoundEngine.PostEvent("Play_OBJ_metronome_jingle_01", base.gameObject);
           
            float num = Mathf.InverseLerp(1f, 100, combonumber);

            Color tintColor = colorGradient.Evaluate(num);
           
            this.Boi.BloopItemAboveHead(this.doubleE, string.Empty, tintColor, false);

            Boi.ownerlessStatModifiers.Remove(MetroModifier);
            this.MetroModifier.amount = (.01f * combonumber);
            Boi.ownerlessStatModifiers.Add(MetroModifier);
            Boi.stats.RecalculateStats(Boi, true);
        }


        public StatModifier MetroModifier = null;
        public tk2dSprite doubleE;
        private int combonumber = 0;
		public PlayerSpecialStates state;
		private Gun gun;
		private PlayerController Boi;
        public Gradient colorGradient;


    }
}
