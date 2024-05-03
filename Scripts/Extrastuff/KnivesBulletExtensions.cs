using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Knives { 
    internal static class KnivesBulletextensions
    {

		public static PlayerController ProjectilePlayerOwner(this Projectile bullet)
		{
			bool flag = bullet && bullet.Owner && bullet.Owner is PlayerController;
			PlayerController result;
			if (flag)
			{
				result = (bullet.Owner as PlayerController);
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
