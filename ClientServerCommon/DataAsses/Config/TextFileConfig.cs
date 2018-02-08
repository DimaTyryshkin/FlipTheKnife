using CodeWriter.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DataAsses.Config
{
    public class TextFileConfig : IConfig
    {
        private const string BASE_DIRECTORY = "config";

        private string m_applicationName;

        public string applicationName
        {
            get { return m_applicationName; }
        }

        public int build
        {
            get { return 0; }
        }

        public bool isDeveloperVersion
        {
            get { return false; }
        }

        public bool isOldVersion
        {
            get { return false; }
        }

        public string version
        {
            get { return "default"; }
        }

        public TextFileConfig(string applicationName)
        {
            m_applicationName = applicationName;
        }

        public string Get(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            key = Path.ChangeExtension(key, "json");
            
            string path = GetApplicationDirectory(key);

            //string p = Path.Combine(Environment.CurrentDirectory, path);
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else
            {
                throw new ArgumentException("key", "Key " + key + " not exists");
            }
        }

        string GetApplicationDirectory(string key)
        {
            var p = Path.Combine(BASE_DIRECTORY, applicationName);
            return Path.Combine(p, key);
        }
    }
}