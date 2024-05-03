using System;
using System.Collections.Generic;
using System.Reflection;
using MonoMod.RuntimeDetour;

namespace Knives
{
	// Token: 0x02000100 RID: 256
	internal class UIHooks
	{
		// Token: 0x06000579 RID: 1401 RVA: 0x0004C2CB File Offset: 0x0004A4CB
		public static void Init()
		{
			UIHooks.GetCurrentHook = new Hook(typeof(GameUIItemController).GetMethod("UpdateItem", BindingFlags.Instance | BindingFlags.Public), typeof(UIHooks).GetMethod("UpdateCustomLabel"));
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x0004C304 File Offset: 0x0004A504
		public static void UpdateCustomLabel(Action<GameUIItemController, PlayerItem, List<PlayerItem>> orig, GameUIItemController self, PlayerItem current, List<PlayerItem> items)
		{
			Ui = self;
			CurrentSelectedActiveItem = current;
			orig(self, current, items);
			
		}

		public static GameUIItemController Ui;
		public static PlayerItem CurrentSelectedActiveItem;
		public static Hook GetCurrentHook;
	}
}
	