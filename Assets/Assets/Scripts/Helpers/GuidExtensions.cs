using System;
using System.IO;

namespace Assets.Scripts.Helpers
{
    public static class GuidExtensions
    {
        public static Guid TryParseGuid(this string id) =>
            Guid.TryParse(id, out Guid guid)
                ? guid
                : throw new InvalidDataException($"Could not parse GUID '{id}'.");
    }
}