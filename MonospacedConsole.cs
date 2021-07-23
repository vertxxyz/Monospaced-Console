using System;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;
using static Vertx.MonospacedConsoleSettings;
using Font = UnityEngine.Font;
using Object = UnityEngine.Object;

namespace Vertx
{
	[InitializeOnLoad]
	public static class MonospacedConsole
	{
		static MonospacedConsole()
		{
			#if UNITY_2021_2_OR_NEWER
			Debug.LogWarning("Unity 2021.2+ has built-in support for Monospaced Font in the console." +
			                 "\nSimply enable \"Use Monospace Font\" in the dropdown at the top right of the Console Window.\n" +
			                 "Vertx.Monospaced-Console package will attempt to remove itself.");
			UnityEditor.PackageManager.Client.Remove("com.vertx.monospaced-console");
			return;
			#endif
			
			EditorApplication.delayCall += () =>
			{
				EditorApplication.projectWindowItemOnGUI += Replacement;
				EditorApplication.hierarchyWindowItemOnGUI += Replacement;
			};
		}

		private const string forcedKey = "MSC_FORCED";

		private static void Replacement(int instanceId, Rect selectionRect) => Replacement();

		private static void Replacement(string guid, Rect selectionRect) => Replacement();

		public static void Replacement(bool force)
		{
			SessionState.SetBool(forcedKey, force);
			EditorApplication.projectWindowItemOnGUI += Replacement;
			EditorApplication.hierarchyWindowItemOnGUI += Replacement;
			EditorApplication.RepaintProjectWindow();
		}

		private static void Replacement()
		{
			EditorApplication.projectWindowItemOnGUI -= Replacement;
			EditorApplication.hierarchyWindowItemOnGUI -= Replacement;
			
			bool force = SessionState.GetBool(forcedKey, false);
			SessionState.EraseBool(forcedKey);

			if (force)
			{
				EditorApplication.delayCall += () =>
				{
					Type consoleWindowType = Type.GetType("UnityEditor.ConsoleWindow,UnityEditor");
					if (consoleWindowType == null)
						return;
					var consoleWindows = Resources.FindObjectsOfTypeAll(consoleWindowType);
					if (consoleWindows == null)
						return;
					foreach (Object consoleWindow in consoleWindows)
						((EditorWindow) consoleWindow).Repaint();
				};
			}
			
			Font font;
			switch ((MonospacedConsoleSettings.Font) EditorPrefs.GetInt(FontKey, 0))
			{
				case MonospacedConsoleSettings.Font.JetbrainsMono:
					font = AssetDatabase.LoadAssetAtPath<Font>("Packages/com.vertx.monospaced-console/JetbrainsMono-Regular.ttf");
					break;
				case MonospacedConsoleSettings.Font.Consola:
					font = EditorResources.Load<Font>("consola.ttf");
					break;
				default:
					return;
			}

			switch ((State) EditorPrefs.GetInt(StateKey, (int) DefaultState))
			{
				case State.LogsAndMessage:
					ReplaceFont("CN Message");
					ReplaceLogs();
					break;
				case State.MessageOnly:
					ReplaceFont("CN Message");
					if (force)
						ReplaceLogs(false);
					break;
				default:
					if (force)
					{
						ReplaceFont("CN Message", false);
						ReplaceLogs(false);
					}
					return;
			}

			void ReplaceLogs(bool replace = true)
			{
				ReplaceFont("CN EntryInfo", replace);
				ReplaceFont("CN EntryWarn", replace);
				ReplaceFont("CN EntryError", replace);
				ReplaceFont("CN EntryInfoSmall", replace);
				ReplaceFont("CN EntryWarnSmall", replace);
				ReplaceFont("CN EntryErrorSmall", replace);
			}

			void ReplaceFont(string guiStyle, bool replace = true)
			{
				GUIStyle style = guiStyle;
				style.font = replace ? font : null;
				style.fontSize = 12;
			}
		}
	}
}