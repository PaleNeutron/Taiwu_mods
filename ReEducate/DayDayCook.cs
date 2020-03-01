using System;
using System.Collections.Generic;

namespace ReEducate
{
	public static class DayDayCook
	{
		public static List<int> Getgong()
		{
			List<int> list = new List<int>();
			if (Main.settings.innergong)
			{
				list.Add(0);
			}
			if (Main.settings.cuipo)
			{
				list.Add(1);
			}
			if (Main.settings.qingling)
			{
				list.Add(2);
			}
			if (Main.settings.huti)
			{
				list.Add(3);
			}
			if (Main.settings.qiqiao)
			{
				list.Add(4);
			}
			return list;
		}

		public static void Justice(List<int> gong)
		{
			if (gong.Count == 0)
			{
				return;
			}
			int mianActorId = DateFile.instance.mianActorId;
			foreach (int key in gong)
			{
				foreach (int num in DateFile.instance.GetActorEquipGongFa(mianActorId)[key])
				{
					if (DateFile.instance.gongFaDate[num][61] != "0" && DateFile.instance.GetGongFaFLevel(mianActorId, num, false) - DateFile.instance.GetGongFaFLevel(mianActorId, num, true) == 10)
					{
						DateFile.instance.actorGongFas[mianActorId][num][2] = 0;
					}
				}
			}
		}

		public static void Evil(List<int> gong)
		{
			if (gong.Count == 0)
			{
				return;
			}
			int mianActorId = DateFile.instance.mianActorId;
			foreach (int key in gong)
			{
				foreach (int num in DateFile.instance.GetActorEquipGongFa(mianActorId)[key])
				{
					if (DateFile.instance.gongFaDate[num][61] != "0" && DateFile.instance.GetGongFaFLevel(mianActorId, num, false) - DateFile.instance.GetGongFaFLevel(mianActorId, num, true) == 10)
					{
						DateFile.instance.actorGongFas[mianActorId][num][2] = 6;
					}
				}
			}
		}

		public static void Rush(List<int> gong)
		{
			if (gong.Count == 0)
			{
				return;
			}
			int mianActorId = DateFile.instance.mianActorId;
			foreach (int key in gong)
			{
				foreach (int num in DateFile.instance.GetActorEquipGongFa(mianActorId)[key])
				{
					if (DateFile.instance.gongFaDate[num][61] != "0" && DateFile.instance.GetGongFaFLevel(mianActorId, num, false) - DateFile.instance.GetGongFaFLevel(mianActorId, num, true) == 10)
					{
						DateFile.instance.actorGongFas[mianActorId][num][2] = 5;
					}
				}
			}
		}

		public static void Undefine(List<int> gong, int num)
		{
			int mianActorId = DateFile.instance.mianActorId;
			if (num > 10)
			{
				num = 10;
			}
			if (num < 0)
			{
				num = 0;
			}
			foreach (int key in gong)
			{
				foreach (int num2 in DateFile.instance.GetActorEquipGongFa(mianActorId)[key])
				{
					if (DateFile.instance.gongFaDate[num2][61] != "0" && DateFile.instance.GetGongFaFLevel(mianActorId, num2, false) - DateFile.instance.GetGongFaFLevel(mianActorId, num2, true) == 10)
					{
						DateFile.instance.actorGongFas[mianActorId][num2][2] = num;
					}
				}
			}
		}

		public static void Remove(List<int> gong)
		{
			int num = DateFile.instance.MianActorID();
			foreach (int key in gong)
			{
				foreach (int num2 in DateFile.instance.GetActorEquipGongFa(num)[key])
				{
					DateFile.instance.actorGongFas[num][num2][0] = 0;
					DateFile.instance.actorGongFas[num][num2][1] = 0;
					DateFile.instance.actorGongFas[num][num2][2] = 0;
					DateFile.instance.gongFaBookPages.Remove(num2);
					DateFile.instance.RemoveMainActorEquipGongFa(num2);
				}
			}
		}
	}
}
