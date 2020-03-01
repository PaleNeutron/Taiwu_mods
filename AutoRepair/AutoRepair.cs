using System;
using System.Collections.Generic;
using System.Reflection;
using GameData;
using Harmony12;
using UnityEngine;
using UnityModManagerNet;

namespace AutoRepair
{
	public static class Main
	{
		public static bool ExistMianActor()
		{
			return DateFile.instance != null && Characters.HasChar(DateFile.instance.MianActorID());
		}

		public static bool Load(UnityModManager.ModEntry modEntry)
		{
			HarmonyInstance harmonyInstance = HarmonyInstance.Create(modEntry.Info.Id);
			harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
			Main.settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
			Main.Logger = modEntry.Logger;
			modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
			modEntry.OnGUI = new Action<UnityModManager.ModEntry>(Main.OnGUI);
			modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(Main.OnSaveGUI);
			return true;
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

		private static void OnGUI(UnityModManager.ModEntry modEntry)
		{
			bool flag = !Main.ExistMianActor();
			if (flag)
			{
				GUILayout.Label("存档未载入!", new GUILayoutOption[0]);
			}
			else
			{
				Main.settings.open = GUILayout.Toggle(Main.settings.open, "开启自动修理业务", new GUILayoutOption[0]);
				bool open = Main.settings.open;
				if (open)
				{
					GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
					GUILayout.Label("选择修理部位", new GUIStyle
					{
						normal = 
						{
							textColor = new Color(0.999999f, 0.537255f, 0.537255f)
						}
					}, new GUILayoutOption[0]);
					GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
					Main.settings.weapon = GUILayout.Toggle(Main.settings.weapon, "武器", new GUILayoutOption[0]);
					Main.settings.hat = GUILayout.Toggle(Main.settings.hat, "头盔", new GUILayoutOption[0]);
					Main.settings.armor = GUILayout.Toggle(Main.settings.armor, "护甲", new GUILayoutOption[0]);
					Main.settings.shouse = GUILayout.Toggle(Main.settings.shouse, "鞋子", new GUILayoutOption[0]);
					Main.settings.pearl = GUILayout.Toggle(Main.settings.pearl, "其他", new GUILayoutOption[0]);
					GUILayout.EndHorizontal();
					Main.settings.familiy = GUILayout.Toggle(Main.settings.familiy, "也修理队友的装备", new GUILayoutOption[0]);
					GUILayout.EndVertical();
					GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
					GUILayout.Label("请选择会员卡种类：", new GUIStyle
					{
						normal = 
						{
							textColor = new Color(0.999999f, 0.537255f, 0.537255f)
						}
					}, new GUILayoutOption[0]);
					Main.settings.card = GUILayout.Toolbar(Main.settings.card, Main.settings.cardText, new GUILayoutOption[]
					{
						GUILayout.Width(400f)
					});
					GUILayout.EndVertical();
					bool flag2 = Main.settings.card == 0;
					if (flag2)
					{
						bool flag3 = !DateFile.instance.HaveLifeDate(10001, 79);
						if (flag3)
						{
							bool flag4 = GUILayout.Button("点击办理会员卡", new GUILayoutOption[]
							{
								GUILayout.Width(180f)
							});
							if (flag4)
							{
								bool flag5 = !DateFile.instance.actorLife.ContainsKey(10001);
								if (flag5)
								{
									DateFile.instance.actorLife.Add(10001, new Dictionary<int, List<int>>
									{
										{
											79,
											new List<int>
											{
												0,
												0,
												0,
												0
											}
										}
									});
								}
								else
								{
									DateFile.instance.actorLife[10001].Add(79, new List<int>
									{
										0,
										0,
										0,
										0
									});
								}
							}
						}
						else
						{
							GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
							GUILayout.Label("您的账户余额如下：", new GUIStyle
							{
								normal = 
								{
									textColor = new Color(0.999999f, 0.537255f, 0.537255f)
								}
							}, new GUILayoutOption[0]);
							GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
							GUILayout.Label(string.Format("木材:{0}", DateFile.instance.actorLife[10001][79][0]), new GUILayoutOption[]
							{
								GUILayout.Width(180f)
							});
							GUILayout.Label(string.Format("金石:{0}", DateFile.instance.actorLife[10001][79][1]), new GUILayoutOption[]
							{
								GUILayout.Width(180f)
							});
							GUILayout.Label(string.Format("织物:{0}", DateFile.instance.actorLife[10001][79][2]), new GUILayoutOption[]
							{
								GUILayout.Width(180f)
							});
							GUILayout.Label(string.Format("银两:{0}", DateFile.instance.actorLife[10001][79][3]), new GUILayoutOption[]
							{
								GUILayout.Width(180f)
							});
							GUILayout.EndHorizontal();
							GUILayout.EndVertical();
							GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
							GUILayout.Space(8f);
							GUILayout.Label("请选择充值目标（银两会在相应资源不足时支付维修费用，但会消耗50%额外手续费）：", new GUIStyle
							{
								normal = 
								{
									textColor = new Color(0.999999f, 0.537255f, 0.537255f)
								}
							}, new GUILayoutOption[0]);
							Main.settings.payment = GUILayout.Toolbar(Main.settings.payment, Main.settings.paymentText, new GUILayoutOption[]
							{
								GUILayout.Width(400f)
							});
							GUILayout.Space(15f);
							GUILayout.Label("请输入充值数量：", new GUIStyle
							{
								normal = 
								{
									textColor = new Color(0.999999f, 0.537255f, 0.537255f)
								}
							}, new GUILayoutOption[0]);
							string s = GUILayout.TextField(Main.settings.number.ToString(), 6, new GUILayoutOption[]
							{
								GUILayout.Width(100f)
							});
							bool changed = GUI.changed;
							if (changed)
							{
								bool flag6 = !int.TryParse(s, out Main.settings.number);
								if (flag6)
								{
									Main.settings.number = 0;
								}
							}
							GUILayout.EndVertical();
							GUILayout.BeginHorizontal("Box", new GUILayoutOption[]
							{
								GUILayout.Width(180f)
							});
							Main.settings.yes = GUILayout.Button("确认充值", new GUILayoutOption[]
							{
								GUILayout.Width(180f)
							});
							Main.settings.no = GUILayout.Button("注销会员", new GUILayoutOption[]
							{
								GUILayout.Width(180f)
							});
							string text = "";
							GUILayout.Label(text, new GUILayoutOption[0]);
							GUILayout.EndHorizontal();
							bool yes = Main.settings.yes;
							if (yes)
							{
								bool flag7 = Main.settings.number != 0;
								if (flag7)
								{
									bool flag8 = Autofix.Buy(Main.settings.payment, Main.settings.number);
									if (flag8)
									{
										text = "交易成功(^･ᴗ･^)";
									}
									else
									{
										text = "充值失败！充值额度不可超过所持资源上限！";
									}
									GUILayout.Label(text, new GUILayoutOption[0]);
									GUILayout.EndVertical();
								}
							}
							bool no = Main.settings.no;
							if (no)
							{
								int actorId = DateFile.instance.MianActorID();
								UIDate.instance.ChangeResource(actorId, 1, DateFile.instance.actorLife[10001][79][0], true);
								UIDate.instance.ChangeResource(actorId, 2, DateFile.instance.actorLife[10001][79][1], true);
								UIDate.instance.ChangeResource(actorId, 3, DateFile.instance.actorLife[10001][79][2], true);
								UIDate.instance.ChangeResource(actorId, 5, DateFile.instance.actorLife[10001][79][3], true);
								DateFile.instance.actorLife[10001].Remove(79);
							}
						}
					}
					else
					{
						GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
						GUILayout.Space(8f);
						GUILayout.Label("好懒，不想充值……我堂堂太吾传人难道还付不起修理费吗？！修就对啦！！！", new GUIStyle
						{
							normal = 
							{
								textColor = new Color(0.999999f, 0.537255f, 0.537255f)
							}
						}, new GUILayoutOption[0]);
						Main.settings.bymoney = GUILayout.Toggle(Main.settings.bymoney, "以银两支付修理费用（会消耗50%额外手续费）", new GUILayoutOption[0]);
						GUILayout.EndVertical();
					}
				}
			}
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
