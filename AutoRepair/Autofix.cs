using System;
using System.Collections.Generic;
using GameData;

namespace AutoRepair
{
	public static class Autofix
	{
		public static bool Buy(int index, int num)
		{
			int num2 = DateFile.instance.MianActorID();
			int num3 = (index == 3) ? (index + 2) : (index + 1);
			int num4 = DateFile.instance.ActorResource(num2)[num3];
			bool flag = num4 < num;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				UIDate.instance.ChangeResource(num2, num3, -num, true);
				List<int> list = DateFile.instance.actorLife[10001][79];
				list[index] += num;
				result = true;
			}
			return result;
		}

		public static int Charge(int id, int maxhp)
		{
			int num = int.Parse(DateFile.instance.GetItemDate(id, 901, true));
			bool flag = num >= maxhp;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int num2 = int.Parse(DateFile.instance.GetItemDate(id, 45, true)) * int.Parse(DateFile.instance.GetItemDate(id, 49, true)) / 375;
				int num3 = (maxhp - num) * num2 * 15 / maxhp;
				bool flag2 = num != 0;
				if (flag2)
				{
					result = num3;
				}
				else
				{
					result = num3 + num2 * 20;
				}
			}
			return result;
		}

		public static int Getpoint(int index)
		{
			bool flag = DateFile.instance.HaveLifeDate(10001, 79) && index < 4;
			int result;
			if (flag)
			{
				result = DateFile.instance.actorLife[10001][79][index];
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static void LazyBone(List<int> people, List<int> type)
		{
			foreach (int actorId in people)
			{
				foreach (int num in type)
				{
					int num2 = int.Parse(DateFile.instance.GetActorDate(actorId, 301 + num, true));
					bool flag = num2 > 0;
					if (flag)
					{
						string itemDate = DateFile.instance.GetItemDate(num2, 902, true);
						string itemDate2 = DateFile.instance.GetItemDate(num2, 4, true);
						string itemDate3 = DateFile.instance.GetItemDate(num2, 49, true);
						int num3 = int.Parse(DateFile.instance.GetItemDate(num2, 506, true));
						bool flag2 = itemDate != "0" && itemDate2 == "4" && itemDate3 != "0" && num3 < 4;
						if (flag2)
						{
							int num4 = Autofix.Charge(num2, int.Parse(itemDate));
							bool flag3 = num4 != 0;
							if (flag3)
							{
								int num5 = 0;
								switch (num3)
								{
								case 0:
									num5 = 2;
									break;
								case 1:
									num5 = 1;
									break;
								case 2:
									num5 = 0;
									break;
								case 3:
									num5 = 1;
									break;
								}
								bool flag4 = Main.settings.card == 0;
								if (flag4)
								{
									int num6 = Autofix.Getpoint(num5);
									bool flag5 = num6 >= num4;
									if (flag5)
									{
										Items.SetItemProperty(num2, 901, itemDate);
										DateFile.instance.actorLife[10001][79][num5] = num6 - num4;
									}
									else
									{
										bool flag6 = Autofix.Getpoint(3) >= num4 + num4 / 2;
										if (flag6)
										{
											Items.SetItemProperty(num2, 901, itemDate);
											DateFile.instance.actorLife[10001][79][3] = Autofix.Getpoint(3) - num4 - num4 / 2;
										}
									}
								}
								else
								{
									int num7 = DateFile.instance.MianActorID();
									bool bymoney = Main.settings.bymoney;
									if (bymoney)
									{
										num5 = 3;
										num4 += num4 / 2;
									}
									int num8 = (num5 == 3) ? (num5 + 2) : (num5 + 1);
									int num9 = DateFile.instance.ActorResource(num7)[num8];
									bool flag7 = num9 >= num4;
									if (flag7)
									{
										Items.SetItemProperty(num2, 901, itemDate);
										UIDate.instance.ChangeResource(num7, num8, -num4, false);
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
