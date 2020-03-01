using System;
using System.Reflection;
using Harmony12;
using UnityEngine;
using UnityModManagerNet;

namespace ReEducate
{
	public static class Main
	{
		public static bool Load(UnityModManager.ModEntry modEntry)
		{
			HarmonyInstance.Create(modEntry.Info.Id).PatchAll(Assembly.GetExecutingAssembly());
			Main.settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
			Main.Logger = modEntry.Logger;
			modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
			modEntry.OnGUI = new Action<UnityModManager.ModEntry>(Main.OnGUI);
			modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(Main.OnSaveGUI);
			return true;
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

		private static void OnGUI(UnityModManager.ModEntry modEntry)
		{
			if (DateFile.instance == null)
			{
				GUILayout.Label("存档未载入!", new GUILayoutOption[0]);
				return;
			}
			GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
			GUILayout.Label("选择重修范围", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			Main.settings.innergong = GUILayout.Toggle(Main.settings.innergong, "内功", new GUILayoutOption[0]);
			Main.settings.cuipo = GUILayout.Toggle(Main.settings.cuipo, "催破", new GUILayoutOption[0]);
			Main.settings.qingling = GUILayout.Toggle(Main.settings.qingling, "轻灵", new GUILayoutOption[0]);
			Main.settings.huti = GUILayout.Toggle(Main.settings.huti, "护体", new GUILayoutOption[0]);
			Main.settings.qiqiao = GUILayout.Toggle(Main.settings.qiqiao, "奇窍", new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
			if (GUILayout.Button("正练10重", new GUILayoutOption[]
			{
				GUILayout.Width(180f)
			}))
			{
				DayDayCook.Justice(DayDayCook.Getgong());
			}
			if (GUILayout.Button("逆练6重", new GUILayoutOption[]
			{
				GUILayout.Width(180f)
			}))
			{
				DayDayCook.Evil(DayDayCook.Getgong());
			}
			if (GUILayout.Button("冲解！！", new GUILayoutOption[]
			{
				GUILayout.Width(180f)
			}))
			{
				DayDayCook.Rush(DayDayCook.Getgong());
			}
			if (GUILayout.Button("完全遗忘", new GUILayoutOption[]
			{
				GUILayout.Width(180f)
			}))
			{
				DayDayCook.Remove(DayDayCook.Getgong());
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
			string s = GUILayout.TextField(Main.settings.heart.ToString(), 3, new GUILayoutOption[]
			{
				GUILayout.Width(100f)
			});
			if (GUI.changed)
			{
				if (!int.TryParse(s, out Main.settings.heart))
				{
					Main.settings.heart = 0;
				}
				if (Main.settings.heart > 99)
				{
					Main.settings.heart = 10;
				}
			}
			if (GUILayout.Button("自定逆练等级！！", new GUILayoutOption[]
			{
				GUILayout.Width(180f)
			}))
			{
				DayDayCook.Undefine(DayDayCook.Getgong(), Main.settings.heart);
			}
			GUILayout.EndHorizontal();
		}

		private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
		{
			Main.settings.Save(modEntry);
		}

		public static bool enabled;

		public static Settings settings;

		public static UnityModManager.ModEntry.ModLogger Logger;
	}
}
