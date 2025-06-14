using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Knives
{
    class Temporal_Mantle : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Temporal Mantle";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Temporal_mantle";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Temporal_Mantle>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Divine Surge";
            string longDesc = "Automatically dodge 4 hits per floor.\n\n" +
                "An extension of the wearer's survival instinct, this mantle shimmers with a holy light." +
                "\n\n\n - Knife_to_a_Gunfight" +
                "\n Sprite in collab with Dallan";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            item.quality = PickupObject.ItemQuality.S;
            itemID = item.PickupObjectId;

            Temporal_Mantle.one = SpriteBuilder.SpriteFromResource(Temporal_Mantle.tatter, null);
            Temporal_Mantle.one.name = Temporal_Mantle.vfxName;
            UnityEngine.Object.DontDestroyOnLoad(Temporal_Mantle.one);
            FakePrefab.MarkAsFakePrefab(Temporal_Mantle.one);
            Temporal_Mantle.one.SetActive(false);
        }

        private static string tatter = "Knives/Resources/Tattered_mantle";
        private static GameObject one;
        private static string vfxName = "one";

        public static int itemID;

        public static int ID;
       
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            
            
            player.OnNewFloorLoaded += this.OnLoadedFloor;
            HealthHaver healthHaver = player.healthHaver;
            healthHaver.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Combine(healthHaver.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage));

            base.Pickup(player);
        }
        int numberofuses = 4;
       
        public override DebrisObject Drop(PlayerController player)
        {
            player.OnNewFloorLoaded -= this.OnLoadedFloor;
            HealthHaver healthHaver = player.healthHaver;
            healthHaver.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Remove(healthHaver.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage));
            
            return base.Drop(player);

        }

        private void ModifyIncomingDamage(HealthHaver arg1, HealthHaver.ModifyDamageEventArgs arg2)
        {
            if (numberofuses > 0)
            {
                
                arg2.ModifiedDamage = 0;
                RollToSafeSpot(arg1.GetComponentInParent<PlayerController>());
            }
        }

        private void RollToSafeSpot(PlayerController player)
        {
            Vector2 spot = CheckForSafeSpot(player);
            if (spot == null)
            {
                AkSoundEngine.PostEvent("Play_WPN_devolver_morph_01", base.gameObject);
                
            }
            else
            {
                numberofuses--;
                
                AkSoundEngine.PostEvent("Play_OBJ_compass_point_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_ENM_tutorialknight_appear_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_OBJ_compass_point_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_ENM_tutorialknight_appear_01", base.gameObject);
                Vector2 direc = spot - player.CenterPosition;
                player.FallingProhibited = true;
                player.ForceStartDodgeRoll(direc);
                player.FallingProhibited = false;
                player.healthHaver.TriggerInvulnerabilityPeriod(player.rollStats.GetModifiedTime(player));
                StartCoroutine(spawnsparkles(player));
                if (numberofuses == 0)
                {
                    AkSoundEngine.PostEvent("Play_BOSS_omegaBeam_fade_01", base.gameObject);
                    GameObject original = Temporal_Mantle.one;
                    tk2dSprite OhGeez = original.GetComponent<tk2dSprite>();
                    this.Owner.BloopItemAboveHead(OhGeez, "");
                }
            }
            
        }
        public void sparkle(PlayerController player)
        {
            GameObject Snap = SpawnManager.SpawnVFX(EasyVFXDatabase.Sparkle, (player.CenterPosition), Quaternion.identity);
            tk2dBaseSprite bang = Snap.GetComponent<tk2dBaseSprite>();

            Snap.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor((player.CenterPosition + new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1))), tk2dBaseSprite.Anchor.LowerCenter);
            Snap.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
            bang.HeightOffGround = 1f;
            bang.UpdateZDepth();

        }

        private IEnumerator spawnsparkles(PlayerController player)
        {
            sparkle(player);
            sparkle(player);
            yield return new WaitForSeconds(.1f);
            while (player.IsDodgeRolling)
            {
                yield return new WaitForSeconds(.1f);

                sparkle(player);

               yield return null;
            }
            sparkle(player);
            sparkle(player);
            sparkle(player);
            sparkle(player);

            SilencerInstance.DestroyBulletsInRange(player.CenterPosition, 1f, true, false);
        }




        private Vector2 CheckForSafeSpot(PlayerController player)
        {

            predictedPlayerPosis.Clear();
            offsets.Clear();
            predictedProjPosis.Clear();

            float dist = player.rollStats.GetModifiedDistance(player);
            //ETGModConsole.Log(dist);
            dist += 2;
            float time = player.rollStats.GetModifiedTime(player);
            //ETGModConsole.Log(time);
            List<Projectile> bullets = GetBullets(); // I am in pain
            for(int i = bullets.Count-1; i >= 0; i--)
            {
                Projectile projectile = bullets[i];
                predictedProjPosis.Add(BraveMathCollege.GetPredictedPosition(projectile.LastPosition, projectile.LastVelocity, time));
            }

            // build offsets based on current roll distance   // offsets based off distance and all degree intervals of the unit circle
            offsets.Add(1, new Vector2(0, dist)); // 90 degrees
            offsets.Add(2, new Vector2(dist, 0));
            offsets.Add(3, new Vector2(0, -dist));
            offsets.Add(4, new Vector2(-dist, 0));

            float special = (float)Math.Sqrt(2) / 2;
            offsets.Add(5, new Vector2(dist * special, dist * special)); // 45 degrees
            offsets.Add(6, new Vector2(-dist * special, dist * special));
            offsets.Add(7, new Vector2(-dist * special, -dist * special));
            offsets.Add(8, new Vector2(dist * special, -dist * special));

            special = (float)Math.Sqrt(3) / 2; 
            float special2 = (float)1 / 2;
            offsets.Add(9, new Vector2(dist * special, dist * special2));  // 30 degrees
            offsets.Add(10, new Vector2(-dist * special, dist * special2));
            offsets.Add(11, new Vector2(-dist * special, -dist * special2));
            offsets.Add(12, new Vector2(dist * special, -dist * special2));

            offsets.Add(13, new Vector2(dist * special2, dist * special)); //60 degees
            offsets.Add(14, new Vector2(-dist * special2, dist * special));
            offsets.Add(15, new Vector2(-dist * special2, -dist * special));
            offsets.Add(16, new Vector2(dist * special2, -dist * special));



            for (int i = 1; i <= offsets.Count; i++) // get predicted end positions for cardinal/ordinal roll directions
            {
                // predict position simple
                predictedPlayerPosis.Add(BraveMathCollege.GetPredictedPosition(player.CenterPosition, player.CenterPosition - (player.CenterPosition + offsets[i]), time));
                Vector2 quicksave = predictedPlayerPosis[i - 1];
                try
                {
                    RaycastHit info = new RaycastHit(); // check for walls and reevaluate
                    QueryTriggerInteraction yup = QueryTriggerInteraction.Ignore;
                    if (Physics.Raycast(player.CenterPosition, player.CenterPosition - (player.CenterPosition + offsets[i]), out info, dist, 1, yup))
                    {
                        predictedPlayerPosis[i - 1] = info.point;
                    }
                }
                catch
                {
                    predictedPlayerPosis[i - 1] = quicksave;
                }
                

            }

            //Compair and cull non good spots
            for(int i = predictedPlayerPosis.Count - 1; i >= 0; i--)
            {
                Vector2 spot = predictedPlayerPosis[i];
                if (spot == null) break;
                bool badspot = false;
                if (spot.GetCell().type == CellType.PIT || spot.GetCell().HasPitNeighbor(GameManager.Instance.Dungeon.data)) // check for pits 
                {
                    predictedPlayerPosis.Remove(spot);
                    break;
                }

                if (Vector2.Distance(spot.GetPositionOfNearestEnemy(true), spot) < 1.5f) // check for enemies near landing zone
                {
                    predictedPlayerPosis.Remove(spot);
                    break;
                }

                foreach (Vector2 proj in predictedProjPosis) // check projectile position
                {
                    if (Vector2.Distance(proj, spot) <= 2.5f)
                    {
                        badspot = true;
                    }
                }



                float spotangle = player.CenterPosition.CalculateVectorBetween(spot).ToAngle();
                float playerangle = player.CenterPosition.CalculateVectorBetween(player.unadjustedAimPoint).ToAngle();
                float angleDiff = playerangle - spotangle;

                angleDiff = Math.Abs(angleDiff);
                if(angleDiff >= 175 && angleDiff <= 185)
                {
                    badspot = true;
                    
                }

                if (badspot == true)
                {
                    predictedPlayerPosis.Remove(spot);
                    break;
                }
            }

            

            // Have all "good" locations
            BestRollSpot = predictedPlayerPosis[UnityEngine.Random.Range(0, predictedPlayerPosis.Count - 1)];
            return BestRollSpot;
        }

        public static Dictionary<int, Vector2> offsets = new Dictionary<int, Vector2>
        {
           
        };

        public Vector2 BestRollSpot;
        public List<Vector2> predictedProjPosis = new List<Vector2>();
        public List<Vector2> predictedPlayerPosis = new List<Vector2>();

        private static List<Projectile> GetBullets()
        {
            List<Projectile> list = new List<Projectile>();
            ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                bool flag = projectile && projectile.sprite && !projectile.ImmuneToBlanks && !projectile.ImmuneToSustainedBlanks;
                if (flag)
                {
                    bool flag2 = projectile.Owner != null;
                    if (flag2)
                    {
                        bool flag3 = projectile.isFakeBullet || projectile.Owner is AIActor || (projectile.Shooter != null && projectile.Shooter.aiActor != null) || projectile.ForcePlayerBlankable;
                        if (flag3)
                        {
                            list.Add(projectile);
                        }
                    }
                }
            }
            return list;
        }



        bool half_toggle = false;
        public void OnLoadedFloor(PlayerController player)
        {

            if (half_toggle)
            {
                numberofuses = 4;
                half_toggle = false;
            }
            else
            {
                half_toggle = true;
            }


        }

    }
}
