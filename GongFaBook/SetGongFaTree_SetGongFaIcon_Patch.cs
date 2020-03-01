using System;
using System.Collections.Generic;
using Harmony12;
using UnityEngine;

namespace GongFaBook
{
	[HarmonyPatch(typeof(SetGongFaTree), "SetGongFaIcon")]
	public static class SetGongFaTree_SetGongFaIcon_Patch
	{
		private static bool Prefix(SetGongFaTree __instance, int typ, int gangId, int gangValue)
		{
			if (!Main.enabled)
			{
				return true;
			}
			int num = DateFile.instance.MianActorID();
			SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(__instance.gongFaImage, "gongFaImage", new int[]
			{
				typ
			});
			__instance.gongFaNameText.text = DateFile.instance.baseSkillDate[101 + typ][0];
			int num2 = (gangValue >= 0) ? Mathf.Max(gangValue / 100 - 2, 0) : -1;
			List<int> list = new List<int>();
			for (int i = 0; i < __instance.gongFaLevelText.Length; i++)
			{
				__instance.gongFaLevelText[i].text = DateFile.instance.massageDate[7003][3].Split(new char[]
				{
					'|'
				})[i];
			}
			for (int j = 0; j < DateFile.instance.allGongFaKey.Count; j++)
			{
				int num3 = DateFile.instance.allGongFaKey[j];
				if (int.Parse(DateFile.instance.gongFaDate[num3][1]) == typ && int.Parse(DateFile.instance.gongFaDate[num3][3]) == gangId)
				{
					list.Add(num3);
				}
			}
			for (int k = 0; k < __instance.gongFaIcons.Length; k++)
			{
				if (k < list.Count)
				{
					int num4 = list[k];
					__instance.gongFaImages[k].SetActive(true);
					__instance.gongFaImages[k].name = "GongFaImage," + num4;
					if (DateFile.instance.actorGongFas.ContainsKey(num) && DateFile.instance.actorGongFas[num].ContainsKey(num4))
					{
						SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(__instance.gongFaIcons[k], "gongFaSprites", new int[]
						{
							int.Parse(DateFile.instance.gongFaDate[num4][98])
						});
						__instance.gongFaNames[k].text = DateFile.instance.SetColoer((num2 < k) ? 20002 : 10003, DateFile.instance.gongFaDate[num4][0], false);
						if (DateFile.instance.GetGongFaLevel(num, num4, 0) >= 100 && DateFile.instance.GetGongFaFLevel(num, num4, false) >= 10)
						{
							__instance.gongFaStudyMassageText[k].text = DateFile.instance.SetColoer(20009, DateFile.instance.massageDate[7007][5].Split(new char[]
							{
								'|'
							})[3], false);
						}
						else
						{
							__instance.gongFaStudyMassageText[k].text = DateFile.instance.SetColoer(20008, DateFile.instance.massageDate[7007][5].Split(new char[]
							{
								'|'
							})[2], false);
						}
					}
					else
					{
						if (num2 < k && int.Parse(DateFile.instance.gongFaDate[num4][16]) == 1 && !Main.settings.showAll)
						{
							__instance.gongFaIcons[k].GetComponent<PointerEnter>().enabled = false;
							SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(__instance.gongFaIcons[k], "gongFaSprites", new int[]
							{
								int.Parse(DateFile.instance.gongFaDate[num4][98])
							});
							__instance.gongFaNames[k].text = DateFile.instance.SetColoer(10004, DateFile.instance.massageDate[7007][5].Split(new char[]
							{
								'|'
							})[0], false);
							__instance.gongFaStudyMassageText[k].text = DateFile.instance.SetColoer(10004, DateFile.instance.massageDate[7007][5].Split(new char[]
							{
								'|'
							})[1], false);
						}
						else
						{
							SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(__instance.gongFaIcons[k], "gongFaSprites", new int[]
							{
								int.Parse(DateFile.instance.gongFaDate[num4][98])
							});
							__instance.gongFaNames[k].text = DateFile.instance.SetColoer((num2 < k) ? 20002 : 10003, DateFile.instance.gongFaDate[num4][0], false);
							__instance.gongFaStudyMassageText[k].text = "";
						}
						__instance.gongFaIcons[k].color = ((num2 < k) ? new Color(0f, 0f, 0f) : new Color(1f, 1f, 1f));
					}
				}
				else
				{
					__instance.gongFaImages[k].SetActive(false);
				}
			}
			return false;
		}
	}
}
