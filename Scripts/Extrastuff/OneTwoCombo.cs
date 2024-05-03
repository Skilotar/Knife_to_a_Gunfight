using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;

namespace Knives
{
	public class OneTwoComboComponent : MonoBehaviour
	{

		public OneTwoComboComponent()
		{
			 
		}


		private void Start()
		{
			this.Enemy = base.GetComponent<AIActor>();
			orgPercent = this.Enemy.healthHaver.AllDamageMultiplier;
		}

		bool ComboUp = false;
		bool LorumIps = false;
		private void Update()
		{
			if (this.Enemy != null)
			{
				
				RelativeLabelAttacher label = Enemy.gameObject.GetOrAddComponent<RelativeLabelAttacher>();
				string formatComboText = "(0%)";
				label.labelValue = formatComboText;
				label.colour = UnityEngine.Color.red;
				label.offset = new Vector3(0f, Enemy.sprite.scale.y + 2, 0f);


				if (ComboTimer > 0)
				{
					if (!doingCo) StartCoroutine(UpdateLabel(label, false));

					Enemy.healthHaver.AllDamageMultiplier = orgPercent + ComboPercent;
					ComboTimer -= Time.deltaTime;
					ComboUp = true;
				}
				else
				{
					if(ComboUp == true && doingCo == false)
                    {
						if (!doingCo) StartCoroutine(UpdateLabel(label, true));
						Enemy.healthHaver.AllDamageMultiplier = orgPercent;
						ComboPercent = 0;
						ComboUp = false;
						LorumIps = true;
					}

				}

			}
		}

		
		public bool doingCo = false;

        private IEnumerator UpdateLabel(RelativeLabelAttacher label, bool ComboBroken)
        {
			doingCo = true;

            if(ComboBroken == false || Enemy.healthHaver.IsDead)
            {
				float DisplayPercent = ComboPercent * 100;
				Math.Truncate(DisplayPercent);
				string formatComboText = "(+" + DisplayPercent.ToString("00") + "%)";
				label.labelValue = formatComboText;
				label.colour = UnityEngine.Color.green;
				
				label.offset = new Vector3(0f, Enemy.sprite.scale.y + 2, 0f);
			}
            else
            {
				string formatComboText = "(0%)";
				label.labelValue = formatComboText;
				label.colour = UnityEngine.Color.red;
				label.offset = new Vector3(0f, Enemy.sprite.scale.y + 2, 0f);
			}

			if(LorumIps == true) 
            {
				string formatComboText = "(0%)";
				label.labelValue = formatComboText;
				label.colour = UnityEngine.Color.red;
				label.offset = new Vector3(0f, Enemy.sprite.scale.y + 2, 0f);
			}

			yield return new WaitForSeconds(.2f);

			UnityEngine.Object.Destroy(label);

			LorumIps = false;
			doingCo = false;
		}

        public float orgPercent;
		public float ComboTimer;
		public float ComboPercent;
		public AIActor Enemy;
		private GameObject extantOverheadder;
	}
}