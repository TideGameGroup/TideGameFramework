using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Tide.Core
{
    public delegate void settingChangedEvent();

    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
    public struct FSetting
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        public bool b;

        [System.Runtime.InteropServices.FieldOffset(0)]
        public int i;

        [System.Runtime.InteropServices.FieldOffset(0)]
        public double d;

        [System.Runtime.InteropServices.FieldOffset(0)]
        public float f;

        [System.Runtime.InteropServices.FieldOffset(sizeof(double))]
        public char heldType;

        public static FSetting Bool(bool v) { var s = new FSetting(); s.heldType = 'b'; s.b = v; return s; }
        public static FSetting Int(int v) { var s = new FSetting(); s.heldType = 'i'; s.i = v; return s; }
        public static FSetting Double(double v) { var s = new FSetting(); s.heldType = 'd'; s.d = v; return s; }
        public static FSetting Float(float v) { var s = new FSetting(); s.heldType = 'f'; s.f = v; return s; }

        public string GetValueString()
        {
            return heldType switch
            {
                'b' => "b" + b.ToString(),
                'f' => "f" + f.ToString(),
                'i' => "i" + i.ToString(),
                'd' => "d" + d.ToString(),
                _ => "",
            };
        }
    }

    public class USettings : UComponent
    {
        private Dictionary<string, FSetting> settings = new Dictionary<string, FSetting>();
        private Dictionary<string, settingChangedEvent> onChangeCallbacks = new Dictionary<string, settingChangedEvent>();

        public USettings()
        {
            // defaults
            settings["volume"] = FSetting.Float(0.1f);
            settings["fullscreen"] = FSetting.Bool(false);

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

        public void RemoveOnChangedCallback(string setting, settingChangedEvent evt)
        {
            onChangeCallbacks[setting] -= evt;
        }

        protected void EnforceSettingsRanges()
        {
            this["volume"] = FSetting.Float(Math.Clamp(this["volume"].f, 0.0f, 1.0f));
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
                        FSetting newsetting = new FSetting();
                        if (SafeParse.ParseSetting(setting.Value, ref newsetting))
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

        private static string GetPersistentSettingsFile()
        {
            return Path.Combine(GetPersistentDataFolder(), "SETTINGS.xml");
        }

        private void SaveSettingsToFile()
        {
            XElement root = new XElement("document");

            foreach (var setting in settings)
            {
                XElement elem = new XElement(setting.Key);
                elem.Value = setting.Value.GetValueString();
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
    }
}
