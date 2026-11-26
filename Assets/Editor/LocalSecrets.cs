using System;
using System.IO;
using UnityEngine;

namespace Editor
{
    public static class LocalSecrets
    {
        // Looks for a .env file in the project root (next to Assets/)
        private static readonly string EnvFilePath = Path.Combine(Application.dataPath, "..", ".env");

        public static string Get(string key)
        {
            string value = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrEmpty(value)) return value;

            if (!File.Exists(EnvFilePath)) return null;

            foreach (string line in File.ReadAllLines(EnvFilePath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                int separatorIndex = line.IndexOf('=');
                if (separatorIndex < 0) continue;

                string lineKey = line[..separatorIndex].Trim();
                string lineValue = line[(separatorIndex + 1)..].Trim();

                if (lineKey == key) return lineValue;
            }

            return null;
        }
    }
}