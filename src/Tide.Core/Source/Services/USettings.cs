using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

namespace Tide.Core
{
    public delegate void settingChangedEvent();

    public class USettings : UComponent
    {
        private Dictionary<string, settingChangedEvent> onChangeCallbacks = new Dictionary<string, settingChangedEvent>();
        private Dictionary<string, FSetting> settings = new Dictionary<string, FSetting>();

        public USettings()
        {
            // defaults
            settings["volume"] = FSetting.Float(0.1f);
            settings["fullscreen"] = FSetting.Bool(false);
            settings["vsync"] = FSetting.Bool(true);

            LoadSettingsFromFile();
            EnforceSettingsRanges();
        }

        public FSetting this[string stat]
        {
            get
            {
                return settings[stat];
            }
            set
            {
                settings[stat] = value;
                if (onChangeCallbacks.ContainsKey(stat))
                {
                    onChangeCallbacks[stat].Invoke();
                }
                SaveSettingsToFile();
            }
        }

        private static string GetPersistentSettingsFile()
        {
            return Path.Combine(GetPersistentDataFolder(), "SETTINGS.xml");
        }

        private void SaveSettingsToFile()
        {
            XElement root = new XElement("document");

            foreach (var setting in settings)
            {
                XElement elem = new XElement(setting.Key)
                {
                    Value = setting.Value.GetValueString()
                };
                root.Add(elem);
            }

            try
            {
                XDocument doc = new XDocument(root);
                doc.Save(GetPersistentSettingsFile());
            }
            catch
            {
                return;
            }
        }

        protected void EnforceSettingsRanges()
        {
            this["volume"] = FSetting.Float(Math.Clamp(this["volume"].f, 0.0f, 1.0f));
        }

        protected void LoadSettingsFromFile()
        {
            XDocument doc = null;
            try
            {
                doc = XDocument.Load(GetPersistentSettingsFile());
            }
            catch (Exception e)
            {
                if (e is FileNotFoundException)
                {
                    SaveSettingsToFile();
                }
            }

            if (doc != null)
            {
                XElement settings_list = doc.Element("document");

                if (settings_list != null)
                {
                    foreach (var setting in settings_list.Elements())
                    {
                        if (SafeParse.ParseSetting(setting.Value, out FSetting newsetting))
                        {
                            this[setting.Name.ToString()] = newsetting;
                        }
                    }
                }
                else
                {
                    Debug.Print("Unable to load settings from document");
                }
            }
        }

        public static string GetPersistentDataFolder()
        {
            string dir = Directory.GetCurrentDirectory();

            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }

            return dir;
        }

        public void RemoveOnChangedCallback(string setting, settingChangedEvent evt)
        {
            onChangeCallbacks[setting] -= evt;
        }

        public void SetOnChangedCallback(string setting, settingChangedEvent evt)
        {
            if (!onChangeCallbacks.ContainsKey(setting))
            {
                onChangeCallbacks[setting] = evt;
            }
            else
            {
                onChangeCallbacks[setting] += evt;
            }
        }
    }
}