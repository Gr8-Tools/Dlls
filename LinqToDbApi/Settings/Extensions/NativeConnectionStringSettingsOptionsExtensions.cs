using System.Collections.Generic;
using LinqToDbApi.Settings.Utils.ConnectionStringOptions;

namespace LinqToDbApi.Settings.Extensions
{
    public static class NativeConnectionStringSettingsOptionsExtensions
    {
        public static IEnumerable<NativeConnectionStringSettingsOptions> UpdateOptions(this IEnumerable<NativeConnectionStringSettingsOptions> options, string? server, int? port)
        {
            foreach (var option in options)
                option.Update(server, port);

            return options;
        }
    }
}