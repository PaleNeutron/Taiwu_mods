using System;
using System.Text;
using Harmony12;
using UnityEngine.UI;

namespace GongFaBook
{
	[HarmonyPatch(typeof(WindowManage), "ShowGongFaMassage")]
	public static class WindowManage_ShowGongFaMassage_Patch
	{
		private static void Postfix(WindowManage __instance, int skillId, int skillTyp, int levelTyp, int actorId, Toggle toggle, ref Text ___informationMassage, ref string ___baseGongFaMassage)
		{
			if (Main.enabled && skillTyp != 0 && skillTyp == 1)
			{
				actorId = ((actorId != -1) ? actorId : ((!ActorMenu.instance.actorMenu.activeInHierarchy) ? DateFile.instance.MianActorID() : ActorMenu.instance.actorId));
				int num = (levelTyp != -1 && levelTyp != 0) ? 10 : ((skillId != 0) ? DateFile.instance.GetGongFaFLevel(actorId, skillId, false) : 0);
				int gongFaFTyp = DateFile.instance.GetGongFaFTyp(actorId, skillId);
				int num2 = int.Parse(DateFile.instance.gongFaDate[skillId][103]);
				if (num2 > 0)
				{
					int num3 = int.Parse(DateFile.instance.gongFaDate[skillId][104]);
					WindowManage_ShowGongFaMassage_Patch.str.Clear();
					WindowManage_ShowGongFaMassage_Patch.str.Append(___baseGongFaMassage);
					if (num < 5 || gongFaFTyp == 2)
					{
						WindowManage_ShowGongFaMassage_Patch.str.Append(DateFile.instance.SetColoer(20004, "  如果正练\n", false)).Append(__instance.SetMassageTitle(8007, 3, 11, 20010)).Append(__instance.Dit()).Append(DateFile.instance.SetColoer(20002, DateFile.instance.gongFaFPowerDate[num2][99] + ((DateFile.instance.gongFaFPowerDate[num2][98] == "") ? "" : DateFile.instance.massageDate[5001][4]) + DateFile.instance.gongFaFPowerDate[num2][98] + DateFile.instance.massageDate[5001][5], false)).Append("\n\n");
						WindowManage_ShowGongFaMassage_Patch.str.Append(DateFile.instance.SetColoer(20004, "  如果逆练\n", false)).Append(__instance.SetMassageTitle(8007, 3, 12, 20005)).Append(__instance.Dit()).Append(DateFile.instance.SetColoer(20002, DateFile.instance.gongFaFPowerDate[num3][99] + ((DateFile.instance.gongFaFPowerDate[num3][98] == "") ? "" : DateFile.instance.massageDate[5001][4]) + DateFile.instance.gongFaFPowerDate[num3][98] + DateFile.instance.massageDate[5001][5], false)).Append("\n\n");
					}
					else
					{
						bool flag = gongFaFTyp == 0;
						int key = flag ? num3 : num2;
						WindowManage_ShowGongFaMassage_Patch.str.Append(DateFile.instance.SetColoer(20004, "  如果" + (flag ? "逆" : "正") + "练\n", false)).Append(__instance.SetMassageTitle(8007, 3, flag ? 12 : 11, flag ? 20010 : 20005)).Append(__instance.Dit()).Append(DateFile.instance.SetColoer(20002, DateFile.instance.gongFaFPowerDate[key][99] + ((DateFile.instance.gongFaFPowerDate[key][98] == "") ? "" : DateFile.instance.massageDate[5001][4]) + DateFile.instance.gongFaFPowerDate[key][98] + DateFile.instance.massageDate[5001][5], false)).Append("\n\n");
					}
					Text text = ___informationMassage;
					string text2;
					___baseGongFaMassage = (text2 = WindowManage_ShowGongFaMassage_Patch.str.ToString());
					text.text = text2;
				}
			}
		}

		private static readonly StringBuilder str = new StringBuilder();
	}
}
