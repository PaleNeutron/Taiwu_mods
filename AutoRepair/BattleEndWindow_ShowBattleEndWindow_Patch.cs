using System;
using System.Collections.Generic;
using Harmony12;

namespace AutoRepair
{
	[HarmonyPatch(typeof(BattleEndWindow), "ShowBattleEndWindow")]
	public static class BattleEndWindow_ShowBattleEndWindow_Patch
	{
		private static void Postfix()
		{
			bool flag = !Main.enabled || !Main.settings.open;
			if (!flag)
			{
				bool flag2 = Main.settings.card == 0 && Autofix.Getpoint(0) == 0 && Autofix.Getpoint(1) == 0 && Autofix.Getpoint(2) == 0 && Autofix.Getpoint(3) == 0;
				if (!flag2)
				{
					List<int> list = new List<int>();
					bool weapon = Main.settings.weapon;
					if (weapon)
					{
						list = new List<int>
						{
							0,
							1,
							2
						};
					}
					bool hat = Main.settings.hat;
					if (hat)
					{
						list.Add(3);
					}
					bool armor = Main.settings.armor;
					if (armor)
					{
						list.Add(5);
					}
					bool shouse = Main.settings.shouse;
					if (shouse)
					{
						list.Add(6);
					}
					bool pearl = Main.settings.pearl;
					if (pearl)
					{
						list.AddRange(new List<int>
						{
							4,
							7,
							8,
							9,
							10
						});
					}
					bool flag3 = list.Count == 0;
					if (!flag3)
					{
						List<int> list2 = new List<int>
						{
							DateFile.instance.MianActorID()
						};
						bool familiy = Main.settings.familiy;
						if (familiy)
						{
							list2.AddRange(DateFile.instance.GetFamily(false, false));
						}
						Autofix.LazyBone(list2, list);
					}
				}
			}
		}
	}
}
