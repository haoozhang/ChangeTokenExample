﻿using Microsoft.Extensions.Primitives;

namespace ChangeTokenExample
{
    public static class IConfigurationExtensions
    {
        /// <summary>
        /// Perform an action when configuration changes. 
        /// Note this requires config sources to be added with `reloadOnChange` enabled
        /// </summary>
        /// <param name="config">Configuration to watch for changes</param>
        /// <param name="action">Action to perform when <paramref name="config"/> is changed</param>
        public static void OnChange(this IConfiguration config, Action action)
        {
            // IConfiguration's change detection is based on FileSystemWatcher, which will fire multiple change
            // events for each change - Microsoft's code is buggy in that it doesn't bother to debounce/dedupe
            // https://github.com/aspnet/AspNetCore/issues/2542
            var debouncer = new Debouncer(TimeSpan.FromSeconds(1));

            ChangeToken.OnChange<object>(config.GetReloadToken, _ => debouncer.Debouce(action), null);
        }
    }
}
