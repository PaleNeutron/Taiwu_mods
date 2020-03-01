using System;

namespace GongFaBook
{
	internal static class ActorMenu_Awake_Patch
	{
		public static void Postfix(ActorMenu __instance)
		{
			__instance.actorMenu.SetActive(false);
		}
	}
}
