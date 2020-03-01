using System;
using System.Collections.ObjectModel;
using System.Reflection;
using Harmony12;
using UnityEngine;
using UnityModManagerNet;

namespace GongFaBook
{
	public static class Main
	{
		public static bool Load(UnityModManager.ModEntry modEntry)
		{
			Main.Logger = modEntry.Logger;
			Main.settings = UnityModManager.ModSettings.Load<Main.Settings>(modEntry);
			HarmonyInstance harmonyInstance = HarmonyInstance.Create(modEntry.Info.Id);
			harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
			MethodInfo method = typeof(ActorMenu).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);
			Patches patchInfo = harmonyInstance.GetPatchInfo(method);
			ReadOnlyCollection<Patch> readOnlyCollection = (patchInfo != null) ? patchInfo.Postfixes : null;
			if (readOnlyCollection == null || readOnlyCollection.Count == 0)
			{
				HarmonyMethod harmonyMethod = new HarmonyMethod(typeof(ActorMenu_Awake_Patch), "Postfix", new Type[]
				{
					typeof(ActorMenu)
				});
				harmonyInstance.Patch(method, null, harmonyMethod, null);
			}
			modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
			modEntry.OnGUI = new Action<UnityModManager.ModEntry>(Main.OnGUI);
			modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(Main.OnSaveGUI);
			return true;
		}

		private static void OnGUI(UnityModManager.ModEntry modEntry)
		{
			GUILayout.BeginVertical("Box", Array.Empty<GUILayoutOption>());
			Main.settings.showAll = GUILayout.Toggle(Main.settings.showAll, "是否显示所有不传之秘", Array.Empty<GUILayoutOption>());
			GUILayout.EndVertical();
		}

		public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
		{
			if (!value)
			{
				return false;
			}
			Main.enabled = value;
			return true;
		}

		private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
		{
			Main.settings.Save(modEntry);
		}

		public static bool enabled;

		public static UnityModManager.ModEntry.ModLogger Logger;

		public static Main.Settings settings;

		public class Settings : UnityModManager.ModSettings
		{
			public override void Save(UnityModManager.ModEntry modEntry)
			{
				UnityModManager.ModSettings.Save<Main.Settings>(this, modEntry);
			}

			public bool showAll = true;
		}
	}
}
