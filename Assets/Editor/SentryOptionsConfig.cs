using Sentry.Unity;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(fileName = "SentryOptionsConfiguration", menuName = "Sentry/SentryOptionsConfiguration")]
    public class SentryOptionsConfig : SentryOptionsConfiguration
    {
        public override void Configure(SentryUnityOptions options)
        {
            string dsn = System.Environment.GetEnvironmentVariable("SENTRY_DSN");
            if (string.IsNullOrEmpty(dsn)) return;
            
            options.Dsn = dsn;
        }
    }
}