using Sentry.Unity;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(fileName = "SentryCliOptionsConfiguration", menuName = "Sentry/SentryCliOptionsConfiguration")]
    public class CustomSentryCliOptions : SentryCliOptionsConfiguration
    {
        public override void Configure(SentryCliOptions cliOptions)
        {
            cliOptions.Auth = LocalSecrets.Get("SENTRY_AUTH_TOKEN");
        }
    }
}