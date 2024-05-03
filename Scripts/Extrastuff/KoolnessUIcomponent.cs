using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ETGGUI;
using SGUI;
using ItemAPI;
using System.Collections;
using System.Text.RegularExpressions;
using SaveAPI;

namespace Knives
{
	public class KoolnessUIComponent : MonoBehaviour
	{

		public KoolnessUIComponent()
		{
			Enabled = true;
		} 
        
        private void Start()
		{
			Player = base.GetComponent<PlayerController>();

            var iconTexture = ResourceExtractor.GetTextureFromResource("Knives/Resources/CoolMeter.png");
            icon = new SImage(iconTexture);
            float sizeMult = 1f;
            icon.Size = new Vector2(icon.Size.x * sizeMult, icon.Size.y * sizeMult); 
            icon.UpdateStyle();
            label = new SLabel(Player.stats.GetStatValue(PlayerStats.StatType.Coolness).ToString());
            labelChange = new SLabel();

            container = new SGroup() { Background = Color.clear, Size = new Vector2(600, icon.Size.y + 10) };
            layout = new SGroup() { Background = Color.clear, Size = container.Size, AutoLayoutVerticalStretch = false };
            layout.Children.Add(icon);
            layout.Children.Add(label);
            layout.Children.Add(labelChange);
            layout.AutoLayout = (SGroup g) => new Action<int, SElement>(g.AutoLayoutHorizontal);
            container.Children.Add(layout);

            container.AutoLayout = (SGroup g) => new Action<int, SElement>(g.AutoLayoutVertical);
            SGUIRoot.Main.Children.Add(container);

            
            
        }
		private void Update()
		{
            if (Enabled && !GameManager.Instance.IsPaused && !Key(GungeonActions.GungeonActionType.Map))
            {
                
                container.Visible = true;
                container.ContentSize = container.Size.WithY(0);
                container.UpdateStyle();
                container.Position.y = container.Root.Size.y / 1.20f - container.Size.y / 2 + 10;

                if(knownKoolness != Player.stats.GetStatValue(PlayerStats.StatType.Coolness))
                {
                    Alpha = 1f;
                    changeAppearTime = Time.time;
                    changeAmount = Player.stats.GetStatValue(PlayerStats.StatType.Coolness) - knownKoolness;
                    positive = changeAmount > 0;
                    knownKoolness = Player.stats.GetStatValue(PlayerStats.StatType.Coolness);
                   
                }

                label.Text = Player.stats.GetStatValue(PlayerStats.StatType.Coolness).ToString();
                label.Update();
                string strang;
                if (Alpha == 0)
                {
                    strang = "";
                }
                else
                {
                    string polarity = positive ? "+" : "";
                    strang = "(" + polarity + changeAmount.ToString("0.000").TrimEnd('0') + ")";
                }
                if (Alpha > 0)
                {
                    float timePassedSinceChange = Time.time - changeAppearTime;
                    Alpha = 1 - Mathf.InverseLerp(2f, 2f + 0.25f, timePassedSinceChange);
                }
                else
                {
                    changeAppearTime = 0;
                    Alpha = 0;
                }
                labelChange.Text = strang;
                labelChange.Foreground = labelChange.Foreground.WithAlpha(Alpha);
                labelChange.Foreground = positive ? Color.green : Color.red;
                labelChange.Update();
            }
            else
            {

                container.Visible = false;
            }

            if (Player.healthHaver.IsDead || Player == null || !Player.HasActiveItem(koolbucks.ID))
            {
                container.Visible = false;
                
            }

           
        }

        

        public float changeAppearTime;
        public bool positive;
        public float Alpha = 0;
        public double changeAmount;
        public float knownKoolness = 0;
        public bool Enabled = true;
        public PlayerController Player;
        public SGroup statsContainer;
        public SGroup container;
        public SGroup layout;
        public SImage icon;
        public SLabel label; //Actual Stat Number;
        public SLabel labelChange; // difference
        public bool Key(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel)
            {
                return BraveInput.GetInstanceForPlayer(Player.PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
            }
            else
            {
                return false;
            }

        }
    }
}
