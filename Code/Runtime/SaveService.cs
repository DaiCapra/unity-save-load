using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Schema;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Runtime
{
    public class SaveService
    {
        private string _root;

        public SaveService()
        {
            Settings = new Settings();
        }

        public bool Initialized { get; set; }
        public Settings Settings { get; set; }

        public void Init(string pathRoot)
        {
            _root = pathRoot;
            Initialized = true;
        }

        public bool AutoSave<T>(string campaign, T data)
        {
            var path = $"{_root}/{campaign}";

            var autoSaves = GetSaves(campaign)
                .Where(t => t.ToLower().Contains(Settings.AutoSaveName.ToLower()))
                .ToList();


            var set = new HashSet<int>();
            foreach (var save in autoSaves)
            {
                if (string.IsNullOrEmpty(save))
                {
                    continue;
                }

                var p = Path.GetFileName(save)
                    .Replace(Settings.AutoSaveName, "")
                    .Replace(Settings.Extension, "")
                    .Replace('.', ' ')
                    .Replace(Settings.AutoSaveSeparator, ' ')
                    .Trim();

                if (int.TryParse(p, out int i))
                {
                    set.Add(i);
                }
            }

            int index = set.Count > 0 ? (set.Max() + 1) % Settings.AutoSaveCount : 0;
            var name = $"{Settings.AutoSaveName}{Settings.AutoSaveSeparator}{index}";
            return Save(campaign, name, data);
        }

        public bool Delete(string campaign)
        {
            var path = $"{_root}/{campaign}";
            return DeleteDirectory(path);
        }


        public bool Delete(string campaign, string name)
        {
            var path = $"{_root}/{campaign}/{name}";
            return DeleteDirectory(path);
        }

        public bool Load<T>(string campaign, string name, out T data)
        {
            data = default;

            if (!Initialized)
            {
                return false;
            }

            var path = GetSavePath(campaign, name);
            try
            {
                return ReadFromFile(path, out data);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public bool Save<T>(string campaign, string name, T data)
        {
            if (!Initialized)
            {
                return false;
            }

            var path = GetSavePath(campaign, name);
            try
            {
                WriteToFile(path, data);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }

            return true;
        }


        public bool WriteToFile<T>(string path, T data)
        {
            if (!Initialized)
            {
                return false;
            }

            var pathRelativeToRoot = GetPathRelativeToRoot(path);
            var pathTemp = $"{pathRelativeToRoot}-temp.{Settings.Extension}";
            var pathFinal = $"{pathRelativeToRoot}.{Settings.Extension}";

            string json = string.Empty;

            try
            {
                json = JsonConvert.SerializeObject(data, Settings.JsonSettings);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }

            try
            {
                if (!EnsureDirectory(pathTemp))
                {
                    return false;
                }

                if (!EnsureDirectory(pathFinal))
                {
                    return false;
                }

                if (File.Exists(pathTemp))
                {
                    File.Delete(pathTemp);
                }

                File.WriteAllText(pathTemp, json);

                if (!File.Exists(pathTemp))
                {
                    return false;
                }

                if (File.Exists(pathFinal))
                {
                    File.Delete(pathFinal);
                }

                File.Move(pathTemp, pathFinal);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public bool ReadFromFile<T>(string path, out T data)
        {
            data = default;

            if (!Initialized)
            {
                return false;
            }

            var pathRelativeToRoot = GetPathRelativeToRoot(path);
            var pathFinal = $"{pathRelativeToRoot}.{Settings.Extension}";
            string json = string.Empty;

            try
            {
                if (File.Exists(pathFinal))
                {
                    json = File.ReadAllText(pathFinal);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }

            if (string.IsNullOrEmpty(json))
            {
                return false;
            }

            try
            {
                data = JsonConvert.DeserializeObject<T>(json, Settings.JsonSettings);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }


        private static bool EnsureDirectory(string path)
        {
            try
            {
                path = Path.GetDirectoryName(path);
                if (path != null && !Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        private static bool DeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }

                return !Directory.Exists(path);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public List<string> GetCampaigns()
        {
            var campaigns = Directory.GetDirectories(_root).Where(t =>
            {
                var p = Path.GetFileName(t);
                return GetSaves(p).Any();
            }).ToList();

            return campaigns;
        }

        public List<string> GetSaves(string campaign)
        {
            campaign = $"{_root}/{campaign}/";
            if (!EnsureDirectory(campaign))
            {
                return new List<string>();
            }


            /*var saves = Directory
                .GetFiles(campaign, $"*.{Settings.Extension}", SearchOption.AllDirectories)
                .ToList();*/

            var set = new HashSet<string>();
            foreach (string file in Directory.EnumerateFiles(campaign, $"*.{Settings.Extension}",
                SearchOption.AllDirectories))
            {
                set.Add(file);
            }

            return set.ToList();
        }

        private string GetPathRelativeToRoot(string path)
        {
            return $"{_root}/{path}";
        }

        private string GetSavePath(string campaign, string name)
        {
            return $"/{campaign}/{name}/{name}";
        }
    }
}