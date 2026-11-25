using Sentry.Unity;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(fileName = "SentryCliOptionsConfiguration", menuName = "Sentry/SentryCliOptionsConfiguration")]
    public class CustomSentryCliOptions : SentryCliOptionsConfiguration
    {
        public override void Configure(SentryCliOptions cliOptions)
        {
            cliOptions.Auth = System.Environment.GetEnvironmentVariable("SENTRY_AUTH_TOKEN");
        }
    }
}