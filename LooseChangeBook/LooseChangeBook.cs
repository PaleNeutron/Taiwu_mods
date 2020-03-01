using GameData;
using Harmony12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace LooseChangeBook
{
    public class Settings : UnityModManager.ModSettings
    {
        public override void Save(UnityModManager.ModEntry modEntry) => Save(this, modEntry);
        // public bool avoid_battle = true;
        // public int level = 9;
        // public int jingchun = 1;
    }

    public static class Main
    {
        internal static bool enabled;
        internal static Settings settings;
        internal static UnityModManager.ModEntry.ModLogger Logger;

        private static Version gameVersion;
        internal static Version GameVersion
        {
            get
            {
                gameVersion = gameVersion ?? new Version(DateFile.instance.gameVersion.Replace("Beta V", "").Replace("[Test]", ""));
                return gameVersion;
            }
        }

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            Logger = modEntry.Logger;
            try
            {
                settings = Settings.Load<Settings>(modEntry);
                var harmony = HarmonyInstance.Create(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
      
                modEntry.OnToggle = OnToggle;
                modEntry.OnGUI = OnGUI;
                modEntry.OnSaveGUI = OnSaveGUI;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                var inner = ex.InnerException;
                while (inner != null)
                {
                    Logger.Log(inner.ToString());
                    inner = inner.InnerException;
                }
                Debug.LogException(ex);
                return false;
            }
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;
            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("插件功能:");
            GUILayout.Label("在交换书籍时, 只考虑门派支持度而不需要太吾对特定功法的修习许可");

            // settings.avoid_battle = GUILayout.Toggle(settings.avoid_battle, "自动放走");

            // GUILayout.Label("以下两个条件为或的关系");
            // GUILayout.BeginHorizontal("Box");
            // GUILayout.Label("不放走品级大于:");
            // int.TryParse(GUILayout.TextField(settings.level.ToString(), 1, GUILayout.Width(30)), out settings.level);
            // GUILayout.Label("品的敌人");
            // GUILayout.EndHorizontal();

            // GUILayout.BeginHorizontal("Box");
            // GUILayout.Label("不放走精纯值大于:");
            // int.TryParse(GUILayout.TextField(settings.jingchun.ToString(), 2, GUILayout.Width(30)), out settings.jingchun);
            // GUILayout.Label("的敌人");
            // GUILayout.EndHorizontal();
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry) => settings.Save(modEntry);
    }

    /// <summary>
    /// 整个替换DateFile.GetActorBook函数
    /// <see cref="DateFile.GetActorBook"/>
    /// </summary>
    [HarmonyPatch(typeof(DateFile), "GetActorBook")]
    internal static class DateFile_GetActorBook_Patch
    {
        public static bool Prefix(DateFile __instance, int actorId, bool needFavor, bool needGongFa, out List<int> __state)
        {
            __state = new List<int>();
            if (DateFile.instance.actorBookDate.ContainsKey(actorId))
            {
                int num = DateFile.instance.GetActorFavor(false, DateFile.instance.MianActorID(), actorId) / 6000 - 1;
                // 获得对话人的门派
                int gangId = DateFile.instance.GetActorGangId(actorId);
                // 获得对应门派的支持度
                int partValue = DateFile.instance.GetGangPartValue(gangId);
#if DEBUG
                Main.Logger.Log($"门派: {DateFile.instance.GetGangName(gangId)},支持度{partValue}");
#endif
                for (int i = 0; i < DateFile.instance.actorBookDate[actorId].Count; i++)
                {
                    int bookId = DateFile.instance.actorBookDate[actorId][i];
                    if (!needFavor || num >= int.Parse(DateFile.instance.GetItemDate(bookId, 8)))
                    {
                        if (needGongFa)
                        {
                            // 获得书籍等级
                            int bookLevel = int.Parse(DateFile.instance.GetItemDate(bookId, 8));
#if DEBUG
                            Main.Logger.Log($"书籍{DateFile.instance.GetItemDate(bookId, 0).Replace("\n", "")},品级{10 - bookLevel}");
#endif
                            if (partValue / 100 < bookLevel + 1)
                            {
                                continue;
                            }
                        }
                        __state.Add(bookId);
                    }
                }
                return false;
            }
            return false;
        }

        static void Postfix(ref List<int> __result, List<int> __state)
        {
                __result = __state;
        }
    }
}
