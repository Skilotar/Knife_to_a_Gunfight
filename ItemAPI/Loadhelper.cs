using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knives
{
	// Token: 0x02000247 RID: 583
	internal class LoadHelper
	{
		// Token: 0x06000B06 RID: 2822 RVA: 0x0008BF2C File Offset: 0x0008A12C
		public static UnityEngine.Object LoadAssetFromAnywhere(string path)
		{
			UnityEngine.Object @object = null;
			foreach (string path2 in LoadHelper.BundlePrereqs)
			{
				try
				{
					@object = ResourceManager.LoadAssetBundle(path2).LoadAsset(path);
				}
				catch
				{
				}
				bool flag = @object != null;
				if (flag)
				{
					break;
				}
			}
			bool flag2 = @object == null;
			if (flag2)
			{
			}
			return @object;
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x0008BFA4 File Offset: 0x0008A1A4
		public static T LoadAssetFromAnywhere<T>(string path) where T : UnityEngine.Object
		{
			T t = default(T);
			foreach (string path2 in LoadHelper.BundlePrereqs)
			{
				try
				{
					t = ResourceManager.LoadAssetBundle(path2).LoadAsset<T>(path);
				}
				catch
				{
				}
				bool flag = t != null;
				if (flag)
				{
					break;
				}
			}
			bool flag2 = t == null;
			if (flag2)
			{
			}
			return t;
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x0008C02C File Offset: 0x0008A22C
		public static List<T> Find<T>(string toFind) where T : UnityEngine.Object
		{
			List<T> list = new List<T>();
			foreach (string path in LoadHelper.BundlePrereqs)
			{
				try
				{
					foreach (string text in ResourceManager.LoadAssetBundle(path).GetAllAssetNames())
					{
						bool flag = text.ToLower().Contains(toFind);
						if (flag)
						{
							bool flag2 = ResourceManager.LoadAssetBundle(path).LoadAsset(text).GetType() == typeof(T) && !list.Contains(ResourceManager.LoadAssetBundle(path).LoadAsset<T>(text));
							if (flag2)
							{
								list.Add(ResourceManager.LoadAssetBundle(path).LoadAsset<T>(text));
							}
						}
					}
				}
				catch
				{
				}
			}
			return list;
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x0008C114 File Offset: 0x0008A314
		public static List<UnityEngine.Object> Find(string toFind)
		{
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			foreach (string path in LoadHelper.BundlePrereqs)
			{
				try
				{
					foreach (string text in ResourceManager.LoadAssetBundle(path).GetAllAssetNames())
					{
						bool flag = text.ToLower().Contains(toFind);
						if (flag)
						{
							bool flag2 = !list.Contains(ResourceManager.LoadAssetBundle(path).LoadAsset(text));
							if (flag2)
							{
								list.Add(ResourceManager.LoadAssetBundle(path).LoadAsset(text));
							}
						}
					}
				}
				catch
				{
				}
			}
			return list;
		}

		// Token: 0x0400077A RID: 1914
		private static string[] BundlePrereqs = new string[]
		{
			"brave_resources_001",
			"dungeon_scene_001",
			"encounters_base_001",
			"enemies_base_001",
			"flows_base_001",
			"foyer_001",
			"foyer_002",
			"foyer_003",
			"shared_auto_001",
			"shared_auto_002",
			"shared_base_001",
			"dungeons/base_bullethell",
			"dungeons/base_castle",
			"dungeons/base_catacombs",
			"dungeons/base_cathedral",
			"dungeons/base_forge",
			"dungeons/base_foyer",
			"dungeons/base_gungeon",
			"dungeons/base_mines",
			"dungeons/base_nakatomi",
			"dungeons/base_resourcefulrat",
			"dungeons/base_sewer",
			"dungeons/base_tutorial",
			"dungeons/finalscenario_bullet",
			"dungeons/finalscenario_convict",
			"dungeons/finalscenario_coop",
			"dungeons/finalscenario_guide",
			"dungeons/finalscenario_pilot",
			"dungeons/finalscenario_robot",
			"dungeons/finalscenario_soldier"
		};
	}
}
