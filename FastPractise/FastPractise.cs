using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony12;
using UnityEngine;
using UnityModManagerNet;

namespace FastPractise
{
	public class Settings : UnityModManager.ModSettings
	{
		public override void Save(UnityModManager.ModEntry modEntry)
		{
			UnityModManager.ModSettings.Save<Settings>(this, modEntry);
		}

		public bool enabled;

		public bool fastStudy;

		public bool fastBreak;

		public bool fastRead;

		public bool dangerousBreak;
	}
	
	public class Main
	{
		private static bool Load(UnityModManager.ModEntry modEntry)
		{
			HarmonyInstance.Create(modEntry.Info.Id).PatchAll(Assembly.GetExecutingAssembly());
			Main.settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
			modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
			modEntry.OnGUI = new Action<UnityModManager.ModEntry>(Main.OnGUI);
			modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(Main.OnSaveGUI);
			return true;
		}

		private static void OnGUI(UnityModManager.ModEntry modEntry)
		{
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			Main.settings.fastStudy = GUILayout.Toggle(Main.settings.fastStudy, "一键修习（修习至无法再修习）", Array.Empty<GUILayoutOption>());
			Main.settings.fastBreak = GUILayout.Toggle(Main.settings.fastBreak, "一键突破（直接根据资质和相关加成计算结果提升休息程度）", Array.Empty<GUILayoutOption>());
			Main.settings.fastRead = GUILayout.Toggle(Main.settings.fastRead, "一键读书（直接根据悟性耐心和相关加成计算结果改变书本状态）", Array.Empty<GUILayoutOption>());
			GUILayout.EndVertical();
		}

		private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
		{
			Main.settings.Save(modEntry);
		}

		public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
		{
			Main.enabled = value;
			return true;
		}

		public static bool enabled;

		public static Settings settings;

		[HarmonyPatch(typeof(BuildingWindow), "StudySkillUp")]
		private static class BuildingWindow_StudySkillUp_Hook
		{
			private static bool Prefix()
			{
				if (!Main.enabled || !Main.settings.fastStudy)
				{
					return true;
				}
				for (int i = 1; i <= 30; i++)
				{
					Main.BuildingWindow_StudySkillUp_Hook.NewStudySkillUp();
				}
				return false;
			}

			public static void NewStudySkillUp()
			{
				int num = DateFile.instance.MianActorID();
				int[] array = DateFile.instance.homeBuildingsDate[HomeSystem.instance.homeMapPartId][HomeSystem.instance.homeMapPlaceId][HomeSystem.instance.homeMapbuildingIndex];
				int value = Traverse.Create(BuildingWindow.instance).Field("skillUpUseTime").GetValue<int>();
				int value2 = Traverse.Create(BuildingWindow.instance).Field("studySkillId").GetValue<int>();
				if (value2 > 0)
				{
					int gongFaExperienceP = DateFile.instance.gongFaExperienceP;
					if (BuildingWindow.instance.studySkillTyp == 17)
					{
						int gongFaLevel = DateFile.instance.GetGongFaLevel(num, value2, 0);
						if (gongFaLevel >= 100)
						{
							return;
						}
						int num2 = Traverse.Create(BuildingWindow.instance).Method("GetMaxStudyNeedCost", new object[]
						{
							value2,
							gongFaLevel,
							HomeSystem.instance.homeMapPartId,
							HomeSystem.instance.homeMapPlaceId,
							HomeSystem.instance.homeMapbuildingIndex,
							66,
							0
						}).GetValue<int[]>()[0];
						bool flag = false;
						int[] array2 = new int[0];
						switch (int.Parse(DateFile.instance.gongFaDate[value2][2]))
						{
						case 1:
						case 2:
						case 3:
							array2 = DateFile.instance.GongFaPowerPart[0];
							break;
						case 4:
						case 5:
							array2 = DateFile.instance.GongFaPowerPart[1];
							break;
						case 6:
						case 7:
							array2 = DateFile.instance.GongFaPowerPart[2];
							break;
						case 8:
						case 9:
							array2 = DateFile.instance.GongFaPowerPart[3];
							break;
						}
						for (int i = 0; i < array2.Length; i++)
						{
							if (gongFaLevel == array2[i])
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							if (DateFile.instance.teachingOpening == 401)
							{
								Teaching.instance.RemoveTeachingWindow(8);
								YesOrNoWindow.instance.SetYesOrNoWindow(901, DateFile.instance.massageDate[7008][3].Split(new char[]
								{
									'|'
								})[0], DateFile.instance.massageDate[7008][4].Split(new char[]
								{
									'|'
								})[0], false, false);
								return;
							}
							YesOrNoWindow.instance.SetYesOrNoWindow(-1, DateFile.instance.massageDate[7008][3].Split(new char[]
							{
								'|'
							})[0], DateFile.instance.massageDate[7008][4].Split(new char[]
							{
								'|'
							})[0], false, true);
							return;
						}
						else
						{
							if (gongFaExperienceP < num2)
							{
								YesOrNoWindow.instance.SetYesOrNoWindow(-1, DateFile.instance.massageDate[7006][2].Split(new char[]
								{
									'|'
								})[0], DateFile.instance.massageDate[7006][2].Split(new char[]
								{
									'|'
								})[1], false, true);
								return;
							}
							if (DateFile.instance.dayTime < value)
							{
								YesOrNoWindow.instance.SetYesOrNoWindow(-1, DateFile.instance.massageDate[7006][4].Split(new char[]
								{
									'|'
								})[0], DateFile.instance.massageDate[7006][4].Split(new char[]
								{
									'|'
								})[1], false, true);
								return;
							}
							UIDate.instance.ChangeTime(false, value);
							DateFile.instance.ChangeGongfaExp(-num2, false);
							DateFile.instance.ChangeActorGongFa(num, value2, (UnityEngine.Random.Range(0, 100) < 20) ? 2 : 1, 0, 0, true);
							if (DateFile.instance.GetGongFaLevel(num, value2, 0) >= 100 && DateFile.instance.GetGongFaFLevel(num, value2, false) >= 10)
							{
								DateFile.instance.AddActorScore(304, int.Parse(DateFile.instance.gongFaDate[value2][2]) * 100);
							}
						}
					}
					else
					{
						int skillLevel = DateFile.instance.GetSkillLevel(value2);
						if (skillLevel >= 100)
						{
							return;
						}
						int num3 = Traverse.Create(BuildingWindow.instance).Method("GetMaxStudyNeedCost", new object[]
						{
							value2,
							skillLevel,
							HomeSystem.instance.homeMapPartId,
							HomeSystem.instance.homeMapPlaceId,
							HomeSystem.instance.homeMapbuildingIndex,
							66,
							0
						}).GetValue<int[]>()[0];
						bool flag2 = false;
						int[] array3 = new int[0];
						switch (int.Parse(DateFile.instance.skillDate[value2][2]))
						{
						case 1:
						case 2:
						case 3:
							array3 = DateFile.instance.GongFaPowerPart[0];
							break;
						case 4:
						case 5:
							array3 = DateFile.instance.GongFaPowerPart[1];
							break;
						case 6:
						case 7:
							array3 = DateFile.instance.GongFaPowerPart[2];
							break;
						case 8:
						case 9:
							array3 = DateFile.instance.GongFaPowerPart[3];
							break;
						}
						for (int j = 0; j < array3.Length; j++)
						{
							if (skillLevel == array3[j])
							{
								flag2 = true;
								break;
							}
						}
						if (flag2)
						{
							YesOrNoWindow.instance.SetYesOrNoWindow(-1, DateFile.instance.massageDate[7008][3].Split(new char[]
							{
								'|'
							})[1], DateFile.instance.massageDate[7008][4].Split(new char[]
							{
								'|'
							})[1], false, true);
							return;
						}
						if (gongFaExperienceP < num3)
						{
							YesOrNoWindow.instance.SetYesOrNoWindow(-1, DateFile.instance.massageDate[7006][3].Split(new char[]
							{
								'|'
							})[0], DateFile.instance.massageDate[7006][3].Split(new char[]
							{
								'|'
							})[1], false, true);
							return;
						}
						if (DateFile.instance.dayTime < value)
						{
							YesOrNoWindow.instance.SetYesOrNoWindow(-1, DateFile.instance.massageDate[7006][4].Split(new char[]
							{
								'|'
							})[0], DateFile.instance.massageDate[7006][4].Split(new char[]
							{
								'|'
							})[1], false, true);
							return;
						}
						UIDate.instance.ChangeTime(false, value);
						DateFile.instance.ChangeGongfaExp(-num3, false);
						DateFile.instance.ChangeMianSkill(value2, (UnityEngine.Random.Range(0, 100) < 20) ? 2 : 1, 0, true);
						if (DateFile.instance.GetSkillLevel(value2) >= 100 && DateFile.instance.GetSkillFLevel(value2) >= 10)
						{
							DateFile.instance.AddActorScore(204, int.Parse(DateFile.instance.skillDate[value2][2]) * 100);
						}
					}
					BuildingWindow.instance.UpdateStudySkillWindow();
					BuildingWindow.instance.UpdateLevelUPSkillWindow();
					BuildingWindow.instance.UpdateReadBookWindow();
				}
			}
		}

		[HarmonyPatch(typeof(BuildingWindow), "StartStudyGongFa")]
		private static class BuildingWindow_StartStudyGongFa_Hook
		{
			private static bool Prefix()
			{
				if (!Main.enabled || !Main.settings.fastBreak)
				{
					return true;
				}
				UIDate.instance.ChangeTime(false, 20);
				int mianActorId = DateFile.instance.mianActorId;
				float num = 1f;
				if (BuildingWindow.instance.studySkillTyp == 17)
				{
					if (DateFile.instance.GetActorFeature(mianActorId, false).Contains(5002))
					{
						num += 0.2f;
					}
					int num2 = int.Parse(DateFile.instance.gongFaDate[BuildingWindow.instance.levelUPSkillId][3]);
					if (num2 > 0)
					{
						int gangPartValue = DateFile.instance.GetGangPartValue(num2);
						if (gangPartValue >= 600)
						{
							num += 0.2f;
						}
						if (gangPartValue >= 800)
						{
							num += 0.2f;
						}
					}
					num += (float)DateFile.instance.addGongFaStudyValue / 100f;
				}
				else
				{
					if (DateFile.instance.GetActorFeature(mianActorId, false).Contains(5001))
					{
						num += 0.2f;
					}
					num += (float)DateFile.instance.addSkillStudyValue / 100f;
				}
				int num3 = Main.BuildingWindow_StartStudyGongFa_Hook.MyGetMaxStudyTurn();
				if (Main.settings.dangerousBreak)
				{
					num3 += 2;
					float largeSize = BattleSystem.instance.largeSize;
					BattleSystem.instance.SetRealDamage(true, 1, 15, 200, mianActorId, largeSize, false, 0);
				}
				int num4 = (int)((float)num3 * UnityEngine.Random.Range(num - 0.2f, num + 0.2f));
				DateFile.instance.ChangeGongfaExp(-BuildingWindow.instance.useValue, true);
				if (BuildingWindow.instance.studySkillTyp == 17)
				{
					GEvent.OnEvent(eEvents.GongfaBreakthroughBuffChange, Array.Empty<object>());
					DateFile.instance.ChangeActorGongFa(mianActorId, BuildingWindow.instance.levelUPSkillId, num4, 0, 0, false);
				}
				else
				{
					GEvent.OnEvent(eEvents.SkillBreakthroughBuffChange, Array.Empty<object>());
					DateFile.instance.ChangeMianSkill(BuildingWindow.instance.levelUPSkillId, num4, 0, false);
				}
				BuildingWindow.instance.UpdateStudySkillWindow();
				BuildingWindow.instance.UpdateLevelUPSkillWindow();
				BuildingWindow.instance.UpdateReadBookWindow();
				return false;
			}

			private static int MyGetMaxStudyTurn()
			{
				int actorId = DateFile.instance.MianActorID();
				int actorLevel;
				int needLevel;
				if (BuildingWindow.instance.studySkillTyp == 17)
				{
					int num = int.Parse(DateFile.instance.gongFaDate[BuildingWindow.instance.levelUPSkillId][1]);
					actorLevel = int.Parse(DateFile.instance.GetActorDate(actorId, 601 + num, true));
					needLevel = int.Parse(DateFile.instance.gongFaDate[BuildingWindow.instance.levelUPSkillId][63]) + Convert.ToInt32(float.Parse(DateFile.instance.gongFaDate[BuildingWindow.instance.levelUPSkillId][64]) * (float)DateFile.instance.GetGongFaLevel(actorId, BuildingWindow.instance.levelUPSkillId, 0));
				}
				else
				{
					int num2 = int.Parse(DateFile.instance.skillDate[BuildingWindow.instance.levelUPSkillId][3]);
					actorLevel = int.Parse(DateFile.instance.GetActorDate(actorId, 501 + num2, true));
					needLevel = int.Parse(DateFile.instance.skillDate[BuildingWindow.instance.levelUPSkillId][5]) + Convert.ToInt32(float.Parse(DateFile.instance.skillDate[BuildingWindow.instance.levelUPSkillId][6]) * (float)DateFile.instance.GetSkillLevel(BuildingWindow.instance.levelUPSkillId));
				}
				return Main.BuildingWindow_StartStudyGongFa_Hook.MyMaxStudyTurn(actorLevel, needLevel);
			}

			public static int MyMaxStudyTurn(int actorLevel, int needLevel)
			{
				int num = 0;
				if (actorLevel >= needLevel)
				{
					num = 20;
				}
				else if (actorLevel >= needLevel / 2)
				{
					num += Mathf.Min(15 * actorLevel / Mathf.Max(1, needLevel), 15);
				}
				else
				{
					num += Mathf.Min(10 * actorLevel / Mathf.Max(1, needLevel), 10);
				}
				return Mathf.Max(1, num);
			}
		}

		[HarmonyPatch(typeof(BuildingWindow), "StartReadBook")]
		private static class BuildingWindow_StartReadBook_Hook
		{
			private static bool Prefix()
			{
				if (!Main.enabled || !Main.settings.fastRead)
				{
					return true;
				}
				for (int i = 0; i < 10; i++)
				{
					Main.BuildingWindow_StartReadBook_Hook.result[i] = false;
				}
				Main.BuildingWindow_StartReadBook_Hook.ok = false;
				int num = DateFile.instance.MianActorID();
				UIDate.instance.ChangeTime(false, 10);
				int num2 = int.Parse(DateFile.instance.GetItemDate(BuildingWindow.instance.readBookId, 32, true));
				int num3;
				int num4;
				if (BuildingWindow.instance.studySkillTyp == 17)
				{
					num3 = 601;
					num4 = int.Parse(DateFile.instance.gongFaDate[num2][1]);
					int num5 = int.Parse(DateFile.instance.gongFaDate[num2][2]);
					List<int> list = new List<int>(DateFile.instance.gongFaBookPages.Keys);
					for (int j = 0; j < list.Count; j++)
					{
						int num6 = list[j];
						if (num6 != num2 && int.Parse(DateFile.instance.gongFaDate[num6][1]) == num4 && int.Parse(DateFile.instance.gongFaDate[num6][2]) == num5 && DateFile.instance.GetGongFaFLevel(num, num6, false) >= 10)
						{
							Main.BuildingWindow_StartReadBook_Hook.haveSameLevel = true;
							break;
						}
					}
					int num7 = int.Parse(DateFile.instance.gongFaDate[BuildingWindow.instance.levelUPSkillId][3]);
					if (num7 > 0)
					{
						int gangPartValue = DateFile.instance.GetGangPartValue(num7);
						if (gangPartValue >= 300)
						{
							Main.BuildingWindow_StartReadBook_Hook.supportBonus += 50;
						}
						if (gangPartValue >= 400)
						{
							Main.BuildingWindow_StartReadBook_Hook.supportBonus += 50;
						}
						if (gangPartValue >= 500)
						{
							Main.BuildingWindow_StartReadBook_Hook.supportBonus += 50;
						}
						if (gangPartValue >= 700)
						{
							Main.BuildingWindow_StartReadBook_Hook.supportBonus += 50;
						}
					}
				}
				else
				{
					num3 = 501;
					num4 = int.Parse(DateFile.instance.skillDate[num2][3]);
				}
				int actorValue = DateFile.instance.GetActorValue(num, num3 + num4, true);
				int maxPatience = ReadBook.instance.GetMaxPatience();
				int intelligence = DateFile.instance.BaseAttr(num, 4, 0) / 2;
				int num8 = BuildingWindow.instance.GetNeedInt(actorValue, num2);
				Main.BuildingWindow_StartReadBook_Hook.needInt[1] = 10 * num8 / 100;
				Main.BuildingWindow_StartReadBook_Hook.needInt[2] = 30 * num8 / 100;
				Main.BuildingWindow_StartReadBook_Hook.needInt[3] = 30 * num8 / 100;
				Main.BuildingWindow_StartReadBook_Hook.needInt[4] = 40 * num8 / 100;
				Main.BuildingWindow_StartReadBook_Hook.needInt[5] = 50 * num8 / 100;
				int[] array = (BuildingWindow.instance.studySkillTyp == 17) ? (DateFile.instance.gongFaBookPages.ContainsKey(num2) ? DateFile.instance.gongFaBookPages[num2] : new int[10]) : (DateFile.instance.skillBookPages.ContainsKey(num2) ? DateFile.instance.skillBookPages[num2] : new int[10]);
				for (int k = 0; k < 10; k++)
				{
					if (array[k] == 1 || array[k] <= -100)
					{
						Main.BuildingWindow_StartReadBook_Hook.isRead[k] = true;
					}
					else
					{
						Main.BuildingWindow_StartReadBook_Hook.isRead[k] = false;
					}
				}
				int[] bookPage = DateFile.instance.GetBookPage(BuildingWindow.instance.readBookId);
				for (int l = 0; l < 10; l++)
				{
					if (bookPage[l] == 1)
					{
						Main.BuildingWindow_StartReadBook_Hook.isComplete[l] = true;
					}
					else
					{
						Main.BuildingWindow_StartReadBook_Hook.isComplete[l] = false;
					}
				}
				if (Main.BuildingWindow_StartReadBook_Hook.AllClear())
				{
					Main.BuildingWindow_StartReadBook_Hook.Exp(maxPatience, BuildingWindow.instance.readBookId);
				}
				else
				{
					Main.BuildingWindow_StartReadBook_Hook.dfs(0, maxPatience, intelligence);
				}
				Main.BuildingWindow_StartReadBook_Hook.UpdateResult(BuildingWindow.instance.readBookId, BuildingWindow.instance.studySkillTyp);
				if (int.Parse(DateFile.instance.GetItemDate(BuildingWindow.instance.readBookId, 901, true)) > 1)
				{
					DateFile.instance.ChangeItemHp(DateFile.instance.MianActorID(), BuildingWindow.instance.readBookId, -1, 0, true, 1);
				}
				else if (int.Parse(DateFile.instance.GetItemDate(BuildingWindow.instance.readBookId, 903, true)) > 0)
				{
					DateFile.instance.LoseItem(DateFile.instance.MianActorID(), BuildingWindow.instance.readBookId, 1, true, true, 1);
					BuildingWindow.instance.readBookId = 0;
				}
				BuildingWindow.instance.UpdateReadBookWindow();
				BuildingWindow.instance.UpdateStudySkillWindow();
				BuildingWindow.instance.UpdateLevelUPSkillWindow();
				return false;
			}

			private static void Exp(int patience, int readbookid)
			{
				int num = 1;
				while (patience > 1)
				{
					num++;
					patience /= 2;
				}
				int num2 = 30 - num;
				int num3 = int.Parse(DateFile.instance.GetItemDate(readbookid, 34, true)) * 100 / 100;
				int num4 = 0;
				for (int i = 0; i < 10; i++)
				{
					if (num2 >= 3)
					{
						num4 += num3 * 25 / 100;
						num2 -= 3;
					}
					else
					{
						num4 += num3 * 5 * (2 + num2) / 100;
						num2 = 0;
					}
				}
				DateFile.instance.gongFaExperienceP += num4 * num;
				DateFile.instance.gongFaExperienceP -= num3;
				for (int j = 0; j < 10; j++)
				{
					Main.BuildingWindow_StartReadBook_Hook.result[j] = true;
				}
			}

			private static bool AllClear()
			{
				for (int i = 0; i < 10; i++)
				{
					if (!Main.BuildingWindow_StartReadBook_Hook.isRead[i])
					{
						return false;
					}
				}
				return true;
			}

			private static void dfs(int index, int patience, int intelligence)
			{
				if (Main.BuildingWindow_StartReadBook_Hook.ok)
				{
					return;
				}
				if (index == 10 || patience <= 0)
				{
					for (int i = 0; i < 10; i++)
					{
						Main.BuildingWindow_StartReadBook_Hook.result[i] = (Main.BuildingWindow_StartReadBook_Hook.isRead[i] || Main.BuildingWindow_StartReadBook_Hook.result[i]);
					}
					if (Main.BuildingWindow_StartReadBook_Hook.result[9])
					{
						Main.BuildingWindow_StartReadBook_Hook.ok = true;
					}
					return;
				}
				if (Main.BuildingWindow_StartReadBook_Hook.isRead[index])
				{
					Main.BuildingWindow_StartReadBook_Hook.dfs(index + 1, patience, intelligence + 20);
				}
				if (Main.BuildingWindow_StartReadBook_Hook.isComplete[index] && Main.BuildingWindow_StartReadBook_Hook.haveSameLevel)
				{
					Main.BuildingWindow_StartReadBook_Hook.isRead[index] = true;
					Main.BuildingWindow_StartReadBook_Hook.dfs(index + 1, patience, intelligence + 20);
					Main.BuildingWindow_StartReadBook_Hook.isRead[index] = false;
				}
				int num;
				if (Main.BuildingWindow_StartReadBook_Hook.isComplete[index])
				{
					num = 10;
				}
				else
				{
					num = 5;
				}
				int num2;
				if (Main.BuildingWindow_StartReadBook_Hook.isComplete[index])
				{
					num2 = 30;
				}
				else
				{
					num2 = 5;
				}
				for (int j = 0; j <= 4; j++)
				{
					for (int k = 0; k <= j; k++)
					{
						for (int l = 0; l <= k; l++)
						{
							int num3 = Main.BuildingWindow_StartReadBook_Hook.needInt[j] + Main.BuildingWindow_StartReadBook_Hook.needInt[k] + Main.BuildingWindow_StartReadBook_Hook.needInt[l];
							if (num3 <= intelligence)
							{
								int num4 = num2 * Main.BuildingWindow_StartReadBook_Hook.supportBonus / 100;
								int num5 = num;
								int num6 = 0;
								int num7 = 0;
								int num8 = 0;
								int num9 = 0;
								if (j == 1)
								{
									num6++;
								}
								if (k == 1)
								{
									num6++;
								}
								if (l == 1)
								{
									num6++;
								}
								if (j == 2)
								{
									num7++;
								}
								if (k == 2)
								{
									num7++;
								}
								if (l == 2)
								{
									num7++;
								}
								if (j == 3)
								{
									num8++;
								}
								if (k == 3)
								{
									num8++;
								}
								if (l == 3)
								{
									num8++;
								}
								if (j == 4)
								{
									num9 += index;
								}
								if (k == 4)
								{
									num9 += index;
								}
								if (l == 4)
								{
									num9 += index;
								}
								num4 += num2 * num6;
								num5 += num * num8;
								num4 += num2 * num9 / 2;
								int num10 = Math.Max(10000 / num4 / num5 - 20 - num7 * 20, 0);
								Main.BuildingWindow_StartReadBook_Hook.isRead[index] = true;
								Main.BuildingWindow_StartReadBook_Hook.dfs(index + 1, patience - num10, intelligence - num3 + 20);
								Main.BuildingWindow_StartReadBook_Hook.isRead[index] = false;
								if (num10 == 0)
								{
									return;
								}
							}
						}
					}
				}
			}

			private static void UpdateResult(int readbookid, int studyskilltyp)
			{
				int num = DateFile.instance.MianActorID();
				int num2 = int.Parse(DateFile.instance.GetItemDate(readbookid, 32, true));
				int[] bookPage = DateFile.instance.GetBookPage(readbookid);
				for (int i = 0; i < 10; i++)
				{
					if (Main.BuildingWindow_StartReadBook_Hook.result[i])
					{
						int num3 = int.Parse(DateFile.instance.GetItemDate(readbookid, 34, true)) * 100 / 100;
						if (studyskilltyp == 17)
						{
							if (!DateFile.instance.gongFaBookPages.ContainsKey(num2))
							{
								DateFile.instance.gongFaBookPages.Add(num2, new int[10]);
							}
							int num4 = DateFile.instance.gongFaBookPages[num2][i];
							if (num4 != 1 && num4 > -100)
							{
								int num5 = int.Parse(DateFile.instance.gongFaDate[num2][2]);
								int num6 = int.Parse(DateFile.instance.GetItemDate(readbookid, 35, true));
								DateFile.instance.ChangeActorGongFa(num, num2, 0, 0, num6, true);
								if (num6 != 0)
								{
									DateFile.instance.ChangeMianQi(num, 50 * num5, 5);
								}
								DateFile.instance.gongFaBookPages[num2][i] = 1;
								DateFile.instance.AddActorScore(303, num5 * 100);
								if (DateFile.instance.GetGongFaLevel(num, num2, 0) >= 100 && DateFile.instance.GetGongFaFLevel(num, num2, false) >= 10)
								{
									DateFile.instance.AddActorScore(304, num5 * 100);
								}
								if (bookPage[i] == 0)
								{
									DateFile.instance.AddActorScore(305, num5 * 100);
								}
							}
							else
							{
								num3 = num3 * 10 / 100;
							}
							DateFile.instance.gongFaExperienceP += num3;
						}
						else
						{
							if (!DateFile.instance.skillBookPages.ContainsKey(num2))
							{
								DateFile.instance.skillBookPages.Add(num2, new int[10]);
							}
							int num7 = DateFile.instance.skillBookPages[num2][i];
							if (num7 != 1 && num7 > -100)
							{
								int num8 = int.Parse(DateFile.instance.skillDate[num2][2]);
								if (!DateFile.instance.actorSkills.ContainsKey(num2))
								{
									DateFile.instance.ChangeMianSkill(num2, 0, 0, true);
								}
								DateFile.instance.skillBookPages[num2][i] = 1;
								DateFile.instance.AddActorScore(203, num8 * 100);
								if (DateFile.instance.GetSkillLevel(num2) >= 100 && DateFile.instance.GetSkillFLevel(num2) >= 10)
								{
									DateFile.instance.AddActorScore(204, num8 * 100);
								}
								if (bookPage[i] == 0)
								{
									DateFile.instance.AddActorScore(205, num8 * 100);
								}
							}
							else
							{
								num3 = num3 * 10 / 100;
							}
							DateFile.instance.gongFaExperienceP += num3;
						}
					}
				}
			}

			private static int[] needInt = new int[6];

			private static bool[] isComplete = new bool[10];

			private static bool[] isRead = new bool[10];

			private static bool[] result = new bool[10];

			private static int supportBonus = 100;

			private static bool haveSameLevel = false;

			private static bool ok = false;
		}
	}
}
