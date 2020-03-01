using Harmony12;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace ZhuJianPlus
{
    public class Settings : UnityModManager.ModSettings
    {
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            UnityModManager.ModSettings.Save<Settings>(this, modEntry);
        }

        public bool notEnchant = false;

        public int extraEnchant = 0;

        //public int EnchantTimes = 10;

        public int maxEnchantTimes = 10;
    }
    public class Main
    {
        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            HarmonyInstance.Create(modEntry.Info.Id).PatchAll(Assembly.GetExecutingAssembly());
            Main.Logger = modEntry.Logger;
            Main.settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            modEntry.OnGUI = new Action<UnityModManager.ModEntry>(Main.OnGUI);
            modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
            modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(Main.OnSaveGUI);
            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginVertical("Box", Array.Empty<GUILayoutOption>());
            GUILayout.BeginHorizontal("Box", Array.Empty<GUILayoutOption>());
            GUILayout.Label("最大强效精制次数：", new GUILayoutOption[]
            {
                GUILayout.Width(120f)
            });
            int.TryParse(GUILayout.TextField(Main.settings.maxEnchantTimes.ToString(), 3, new GUILayoutOption[]
            {
                GUILayout.Width(30f)
            }), out Main.settings.maxEnchantTimes);
            GUILayout.EndHorizontal();
            GUILayout.Label("使用哪种强效精制", new GUILayoutOption[0]);
            GUILayout.BeginHorizontal("Box", Array.Empty<GUILayoutOption>());
            Main.settings.extraEnchant = GUILayout.SelectionGrid(Main.settings.extraEnchant, new string[]
            {
                "十二路鱼肠刺剑",
                "工布独一剑",
                "胜邪残剑",
                "巨阙千钧剑",
                "龙源七星剑法",
                "太阿无量剑",
                "纯钧剑气",
                "湛卢剑法"
            }, 8, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();

            //GUILayout.BeginHorizontal("Box", Array.Empty<GUILayoutOption>());
            //GUILayout.Label("精制次数：", new GUILayoutOption[]
            //{
            //    GUILayout.Width(120f)
            //});
            //int.TryParse(GUILayout.TextField(Main.settings.EnchantTimes.ToString(), 3, new GUILayoutOption[]
            //{
            //    GUILayout.Width(30f)
            //}), out Main.settings.EnchantTimes);
            //GUILayout.Label("最大强效精制次数：", new GUILayoutOption[]
            //{
            //    GUILayout.Width(120f)
            //});
            //int.TryParse(GUILayout.TextField(Main.settings.maxEnchantTimes.ToString(), 3, new GUILayoutOption[]
            //{
            //    GUILayout.Width(30f)
            //}), out Main.settings.maxEnchantTimes);
            //GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("精制!"))
            {
                Enchant();
            }
            //if (GUILayout.Button("洗白!"))
            //{
            //    Enchant();
            //}
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            bool flag = !value;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                Main.enabled = value;
                result = true;
            }
            return result;
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Main.settings.Save(modEntry);
        }

        private static Dictionary<int, int[]> echantTable = new Dictionary<int, int[]>
                            {
                                {
                                    1,
                                    new int[]
                                    {
                                        108
                                    }
},
                                {
                                    2,
                                    new int[]
                                    {
                                        109
                                    }
                                },
                                {
                                    3,
                                    new int[]
                                    {
                                        110
                                    }
                                },
                                {
                                    4,
                                    new int[]
                                    {
                                        103
                                    }
                                },
                                {
                                    5,
                                    new int[]
                                    {
                                        111
                                    }
                                },
                                {
                                    6,
                                    new int[]
                                    {
                                        112,
                                        113
                                    }
                                },
                                {
                                    7,
                                    new int[]
                                    {
                                        105,
                                        106
                                    }
                                },
                                {
                                    8,
                                    new int[]
                                    {
                                        101,
                                        102,
                                        107
                                    }
                                }
                            };

        public static bool enabled;

        public static Settings settings;

        public static UnityModManager.ModEntry.ModLogger Logger;

        //public static System.Random rd = new System.Random();

        public static bool Enchant()
        {
            bool flag = !Main.enabled;
            if (!flag)
            {
                if (true)
                {
                    Dictionary<int, int> itemExtraChangeTimesData = DateFile.instance.itemExtraChangeTimesData;
                    int usingWeaponId = int.Parse(DateFile.instance.GetActorDate(DateFile.instance.MianActorID(), 301, false));
                    bool flag2 = int.Parse(DateFile.instance.GetItemDate(usingWeaponId, 4, true)) == 4 && (!itemExtraChangeTimesData.ContainsKey(usingWeaponId) || itemExtraChangeTimesData[usingWeaponId] < Main.settings.maxEnchantTimes);
                    if (flag2)
                    {
                        foreach (int key in echantTable[Main.settings.extraEnchant + 1])
                        {
                            Dictionary<int, string> dictionary = DateFile.instance.changeEquipDate[key];
                            DateFile.instance.ChangItemDate(usingWeaponId, int.Parse(dictionary[2]), int.Parse(dictionary[3]) * 10, false);
                        }
                        bool flag3 = itemExtraChangeTimesData.ContainsKey(usingWeaponId);
                        if (flag3)
                        {
                            Dictionary<int, int> dictionary2 = itemExtraChangeTimesData;
                            int key2 = usingWeaponId;
                            int num = dictionary2[key2];
                            dictionary2[key2] = num + 1;
                        }
                        else
                        {
                            itemExtraChangeTimesData.Add(usingWeaponId, 1);
                        }
                    }
                }
            }
            return false;
        }

        public static bool UnEnchant()
        {
            return false;
        }
    }
}

