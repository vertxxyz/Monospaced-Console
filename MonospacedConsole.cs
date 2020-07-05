using UnityEditor;
using UnityEngine;

namespace Vertx
{
	[InitializeOnLoad]
	public static class MonospacedConsole
	{
		static MonospacedConsole()
		{
			EditorApplication.delayCall += () =>
			{
				EditorApplication.projectWindowItemOnGUI += Replacement;
				EditorApplication.hierarchyWindowItemOnGUI += Replacement;
			};
		}

		private static void Replacement(int instanceId, Rect selectionRect) => Replacement();

		private static void Replacement(string guid, Rect selectionRect) => Replacement();

		static void Replacement()
		{
			EditorApplication.projectWindowItemOnGUI -= Replacement;
			EditorApplication.hierarchyWindowItemOnGUI -= Replacement;
			Font font = AssetDatabase.LoadAssetAtPath<Font>("Packages/com.vertx.monospaced-console/JetbrainsMono-Regular.ttf");

			ReplaceFont("CN Message");

			void ReplaceFont(string guiStyle)
			{
				GUIStyle box = guiStyle;
				box.font = font;
			}
		}
	}
}