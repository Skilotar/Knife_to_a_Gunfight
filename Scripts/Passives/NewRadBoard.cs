using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class NewRadBoard : PassiveItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "Rad board";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/rad_board";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<NewRadBoard>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Sick Flips!";
            string longDesc = "A skate board that has had its fair share of use and of new coats of paint. Its usefulness in the gungeon is questionable, but you sure look cool jumping over bullets. \n\nGain a little coolness for rolling over bullets. Max of 3 per room." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.SPECIAL;

        }
        
        public override void Pickup(PlayerController player)
        {
            player.OnDodgedProjectile += this.OnDodgedProjectile;
            player.OnPreDodgeRoll += Player_OnPreDodgeRoll;
            player.OnEnteredCombat = (Action)Delegate.Combine(player.OnEnteredCombat, new Action(this.OnEnteredCombat));
            GameManager.Instance.RewardManager.SinglePlayerPickupIncrementModifier = 1.6f; // Sets Skater's default Roomdrop chance to be closer to co-op values to Offset her naturally high coolness stat.
            base.Pickup(player);
        }


        private void OnEnteredCombat()
        {
            CoolpointsThisCombat = 0;
            //ETGModConsole.Log(GameManager.Instance.RewardManager.SinglePlayerPickupIncrementModifier.ToString());
        }

        public float coolpoints = 0;
        public float CoolpointsThisCombat = 0; // max points per room
        public bool CurrentlyRolling = false;
        public bool Onebullet = false;
        private void Player_OnPreDodgeRoll(PlayerController obj)
        {
            obj.stats.RecalculateStats(obj, true);

            CurrentlyRolling = true;
            Onebullet = false;
        }

        private void OnDodgedProjectile(Projectile projectile)
        {
            if (this.Owner != null)
            {
                if(projectile.Owner != this.Owner)
                {
                    if (CurrentlyRolling == true && Onebullet == false)
                    {
                        if (CoolpointsThisCombat < 3f)
                        {

                            switch (CoolpointsThisCombat)
                            {
                                case 0:
                                    coolpoints += 0.125f;
                                    AkSoundEngine.PostEvent("Play_Skater_parry_001", base.gameObject);
                                    break;
                                case 1:
                                    coolpoints += 0.0625f;
                                    AkSoundEngine.PostEvent("Play_Skater_parry_002", base.gameObject);
                                    break;
                                case 2:
                                    coolpoints += 0.0625f;
                                    AkSoundEngine.PostEvent("Play_Skater_parry_003", base.gameObject);
                                    break;
                            }
                            CoolpointsThisCombat += 1f;

                            StartCoroutine(DoBamp(projectile));

                            Onebullet = true;
                        }
                        else
                        {
                            AkSoundEngine.PostEvent("Play_WPN_devolver_morph_01", base.gameObject);
                        }
                    }
                }
                
            }

        }

        private IEnumerator DoBamp(Projectile proj)
        {

            Vector2 vector = this.Owner.transform.position;
            GameObject Parry = SpawnManager.SpawnVFX(EasyVFXDatabase.SkaterP, vector, Quaternion.identity);
            Parry.GetComponent<tk2dBaseSprite>().scale *= 1.2f;
            Parry.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(this.Owner.transform.position + new Vector3(-1.5f, -.7f), tk2dBaseSprite.Anchor.LowerLeft);
            Parry.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
            BraveTime.SetTimeScaleMultiplier(0, base.gameObject);
            yield return new WaitForSecondsRealtime(.25f);
            BraveTime.SetTimeScaleMultiplier(1, base.gameObject);


        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnDodgedProjectile -= this.OnDodgedProjectile;
            player.OnPreDodgeRoll -= Player_OnPreDodgeRoll;
            player.OnEnteredCombat = (Action)Delegate.Remove(player.OnEnteredCombat, new Action(this.OnEnteredCombat));
            GameManager.Instance.RewardManager.SinglePlayerPickupIncrementModifier = 1.25f;
            
            return base.Drop(player);
        }

        public override void Update()
        {
            if(this.Owner != null && Time.timeScale != 0)
            {
                if (CurrentlyRolling == true)// roll was initiated
                {
                    if(this.Owner.IsDodgeRolling != true)//roll ended
                    {
                        
                        DoProcessGains(Owner);
                        CurrentlyRolling = false;
                        Onebullet = false;
                    }
                }

            }
            base.Update();
        }

        private void DoProcessGains(PlayerController user)
        {
            if (coolpoints <= 12 && CoolpointsThisCombat <= 3f) // max cap to gainable coolness 
            {
                if(Onebullet == true)
                {
                    AkSoundEngine.PostEvent("Play_WPN_radgun_noice_01", base.gameObject);
                }
                RemoveStat(PlayerStats.StatType.Coolness);
                AddStat(PlayerStats.StatType.Coolness, coolpoints);
                user.stats.RecalculateStats(user, true);
            }
           
        }
        private void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier modifier = new StatModifier();
            modifier.amount = amount;
            modifier.statToBoost = statType;
            modifier.modifyType = method;

            foreach (var m in passiveStatModifiers)
            {
                if (m.statToBoost == statType) return; //don't add duplicates
            }

            if (this.passiveStatModifiers == null)
                this.passiveStatModifiers = new StatModifier[] { modifier };
            else
                this.passiveStatModifiers = this.passiveStatModifiers.Concat(new StatModifier[] { modifier }).ToArray();
        }


        //Removes a stat
        private void RemoveStat(PlayerStats.StatType statType)
        {
            var newModifiers = new List<StatModifier>();
            for (int i = 0; i < passiveStatModifiers.Length; i++)
            {
                if (passiveStatModifiers[i].statToBoost != statType)
                    newModifiers.Add(passiveStatModifiers[i]);
            }
            this.passiveStatModifiers = newModifiers.ToArray();
        }
    }
}
