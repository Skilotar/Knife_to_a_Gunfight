using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using ItemAPI;
using UnityEngine;

namespace Knives
{
    public class Radar_pendant : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Sonarlet";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Sonarlet";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Radar_pendant>();

            //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Ping!";
            string longDesc = "Scans the surrounding area revealing enemies and nearby rooms. A life support system used by rebel soldiers in the frontier war for field medics to find downed infantry."
                 +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            item.CanBeDropped = true;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalBlanksPerFloor, 1);
            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.C;
            item.AddToSubShop(ItemBuilder.ShopType.OldRed, .1f);
            Radar_pendant.healthBarPrefab = PickupObjectDatabase.GetById(821).GetComponent<RatchetScouterItem>().VFXHealthBar;
        }

        private void OnUsedBlank(PlayerController arg1, int arg2)
        {
            StartCoroutine(DoScan(arg1));
        }

        private IEnumerator DoScan(PlayerController arg1)
        {
            yield return new WaitForSeconds(.2f);
            AkSoundEngine.PostEvent("Play_Scan_ring", base.gameObject);
            AkSoundEngine.PostEvent("Play_Scan_ring", base.gameObject);
            AkSoundEngine.PostEvent("Play_Scan_ring", base.gameObject);
            Exploder.DoDistortionWave(arg1.sprite.WorldCenter, .3f, 0.25f, 50, 3f);
            RenderSettings.ambientLight = new Color(.89f, .48f, 0f);
            RoomHandler room = arg1.CurrentRoom;
            foreach(RoomHandler conroom in room.connectedRooms)
            {
                Minimap.Instance.RevealMinimapRoom(conroom, true, true, room == GameManager.Instance.PrimaryPlayer.CurrentRoom);
                foreach (RoomHandler conroom2 in conroom.connectedRooms)
                {
                    Minimap.Instance.RevealMinimapRoom(conroom2, true, true, room == GameManager.Instance.PrimaryPlayer.CurrentRoom);
                   
                    foreach (RoomHandler conroom3 in conroom2.connectedRooms)
                    {
                        Minimap.Instance.RevealMinimapRoom(conroom3, true, true, room == GameManager.Instance.PrimaryPlayer.CurrentRoom);
                    }
                }
            }
            if (!room.HasActiveEnemies(RoomHandler.ActiveEnemyType.All)) yield break;
            foreach (var enemy in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
            {
                enemy.AlwaysShowOffscreenArrow = true;
                enemy.SetOverrideOutlineColor(new Color(.89f, .48f, 0f));
                enemy.MovementSpeed *= .8f;
                enemy.healthHaver.AllDamageMultiplier *= 1.05f;
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Radar_pendant.healthBarPrefab);
                SimpleHealthBarController bar = gameObject.GetOrAddComponent<SimpleHealthBarController>();
                bar.Initialize(enemy.specRigidbody,enemy.healthHaver);
                AkSoundEngine.PostEvent("Play_Scan_hit", base.gameObject);
                yield return new WaitForSeconds(.05f);
            }
            
        }

        public override void Pickup(PlayerController player)
        {

            player.OnUsedBlank += this.OnUsedBlank;
            base.Pickup(player);
        }
        public override DebrisObject Drop(PlayerController player)
        {
            player.OnUsedBlank -= this.OnUsedBlank;
            return base.Drop(player);
        }
        public override void  OnDestroy()
        {
            Owner.OnUsedBlank -= this.OnUsedBlank;
            base.OnDestroy();
        }

        public static GameObject healthBarPrefab;
    }
}
