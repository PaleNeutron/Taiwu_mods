using System;
using UnityModManagerNet;

namespace AutoRepair
{
	public class Settings : UnityModManager.ModSettings
	{
		public override void Save(UnityModManager.ModEntry modEntry)
		{
			UnityModManager.ModSettings.Save<Settings>(this, modEntry);
		}

		public bool open = false;

		public bool familiy = false;

		public bool weapon = false;

		public bool hat = false;

		public bool armor = false;

		public bool shouse = false;

		public bool pearl = false;

		public bool bymoney = false;

		public int number = 0;

		public string[] paymentText = new string[]
		{
			"木材",
			"金石",
			"织物",
			"银两"
		};

		public int payment = 0;

		public string[] cardText = new string[]
		{
			"琥珀会员",
			"赤血会员"
		};

		public int card = 0;

		public bool yes = false;

		public bool no = false;
	}
}
