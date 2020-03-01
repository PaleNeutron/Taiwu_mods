using System;
using System.Collections.Generic;
using System.Text;
using Harmony12;
using UnityEngine.UI;

namespace GongFaBook
{
	[HarmonyPatch(typeof(WindowManage), "ShowItemMassage")]
	public static class WindowManage_ShowBookMassage_Patch
	{
		private static void Postfix(int itemId, ref string ___baseWeaponMassage, ref Text ___informationMassage, ref Text ___informationName)
		{
			if (!Main.enabled)
			{
				return;
			}
			if (int.Parse(DateFile.instance.GetItemDate(itemId, 31, true)).Equals(17))
			{
				WindowManage_ShowBookMassage_Patch.str.Clear();
				int num = ___baseWeaponMassage.IndexOf("所载心法");
				if (num > -1)
				{
					WindowManage_ShowBookMassage_Patch.str.Append(___baseWeaponMassage.Substring(0, num - 18));
				}
				else
				{
					WindowManage_ShowBookMassage_Patch.str.Append(___baseWeaponMassage);
				}
				string itemDate = DateFile.instance.GetItemDate(itemId, 99, true);
				if (int.Parse(DateFile.instance.GetItemDate(itemId, 35, true)).Equals(1))
				{
					___informationName.text = ___informationName.text.Insert(___informationName.text.IndexOf("》"), "·手抄");
					WindowManage_ShowBookMassage_Patch.str.Replace(itemDate, DateFile.instance.SetColoer(20010, itemDate, false));
				}
				else
				{
					WindowManage_ShowBookMassage_Patch.str.Replace(itemDate, DateFile.instance.SetColoer(20004, itemDate, false));
				}
				int key = int.Parse(DateFile.instance.GetItemDate(itemId, 32, false));
				Dictionary<int, string> dictionary;
				if (DateFile.instance.gongFaDate.TryGetValue(key, out dictionary))
				{
					int key2 = int.Parse(dictionary[103]);
					Dictionary<int, string> dictionary2;
					if (DateFile.instance.gongFaFPowerDate.TryGetValue(key2, out dictionary2))
					{
						WindowManage_ShowBookMassage_Patch.str.Append(DateFile.instance.SetColoer(10002, "【所载心法】\n", false));
						WindowManage_ShowBookMassage_Patch.str.Append(DateFile.instance.SetColoer(20004, "·正练:" + dictionary2[99] + "\n", false));
					}
					key2 = int.Parse(DateFile.instance.gongFaDate[key][104]);
					if (DateFile.instance.gongFaFPowerDate.TryGetValue(key2, out dictionary2))
					{
						WindowManage_ShowBookMassage_Patch.str.Append(DateFile.instance.SetColoer(20010, "·逆练:" + dictionary2[99] + "\n", false));
						string text = dictionary2[98];
						if (text.Length > 0)
						{
							WindowManage_ShowBookMassage_Patch.str.Append(DateFile.instance.SetColoer(20010, "·逆练Debuff: " + text + "\n", false));
						}
					}
				}
				___baseWeaponMassage = WindowManage_ShowBookMassage_Patch.str.ToString();
				___informationMassage.text = WindowManage_ShowBookMassage_Patch.str.ToString();
			}
		}

		private static readonly StringBuilder str = new StringBuilder();
	}
}
