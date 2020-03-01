using System;
using UnityModManagerNet;

namespace ReEducate
{
	public class Settings : UnityModManager.ModSettings
	{
		public override void Save(UnityModManager.ModEntry modEntry)
		{
			UnityModManager.ModSettings.Save<Settings>(this, modEntry);
		}

		public bool innergong;

		public bool cuipo;

		public bool qingling;

		public bool huti;

		public bool qiqiao;

		public int heart;
	}
}
