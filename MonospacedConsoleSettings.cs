using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx
{
	public static class MonospacedConsoleSettings
	{
		public enum State
		{
			LogsAndMessage,
			MessageOnly,
			None
		}

		public const State DefaultState = State.MessageOnly;

		public enum Font
		{
			JetbrainsMono, //JetBrains will format as Jet Brains ):
			Consola
		}

		public const string
			StateKey = "MSC_STATE",
			FontKey = "MSC_FONT";

		[SettingsProvider]
		public static SettingsProvider GetSettingsProvider()
		{
			// First parameter is the path in the Settings window.
			// Second parameter is the scope of this setting: it only appears in the Settings window for the Project scope.
			var provider = new SettingsProvider("Preferences/Monospaced Console", SettingsScope.User)
			{
				label = "Monospaced Console",
				// activateHandler is called when the user clicks on the Settings item in the Settings window.
				activateHandler = (searchContext, rootElement) =>
				{
					State state = (State) EditorPrefs.GetInt(StateKey, (int) DefaultState);
					Font font = (Font) EditorPrefs.GetInt(FontKey, 0);

					var title = new Label
					{
						text = "Monospaced Console",
						style =
						{
							fontSize = 19,
							unityFontStyleAndWeight = FontStyle.Bold,
							marginLeft = 8,
							marginTop = 2
						}
					};
					rootElement.Add(title);

					var properties = new VisualElement
					{
						style =
						{
							flexDirection = FlexDirection.Column,
							marginLeft = 10,
							marginTop = 10
						}
					};
					rootElement.Add(properties);

					var stateDropdown = new EnumField(state)
					{
						label = "Monospaced"
					};
					stateDropdown.RegisterValueChangedCallback(evt =>
					{
						EditorPrefs.SetInt(StateKey, (int) (State) evt.newValue);
						MonospacedConsole.Replacement(true);
					});
					properties.Add(stateDropdown);
					
					var fontDropdown = new EnumField(font)
					{
						label = "Font"
					};
					fontDropdown.RegisterValueChangedCallback(evt =>
					{
						EditorPrefs.SetInt(FontKey, (int) (Font) evt.newValue);
						MonospacedConsole.Replacement(true);
					});
					properties.Add(fontDropdown);
				},

				// Populate the search keywords to enable smart search filtering and label highlighting:
				keywords = new HashSet<string>(new[] {"Console", "Monospaced"})
			};

			return provider;
		}
	}
}