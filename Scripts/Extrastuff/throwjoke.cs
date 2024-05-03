using UnityEngine;


namespace Knives
{
    internal class throwjoke : MonoBehaviour
    {
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();


            this.player = (this.projectile.Owner as PlayerController);
            Projectile proj = this.projectile;

            this.projectile.sprite.spriteId = this.projectile.sprite.GetSpriteIdByName("Mozam_projectile_001");


        }
        

        private Projectile projectile;

        private PlayerController player;
    }
}
