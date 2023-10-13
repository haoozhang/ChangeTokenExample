using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;

namespace ChangeTokenTest
{
    public class FileCache
    {
        private readonly IMemoryCache _cache;

        private readonly IFileProvider _fileProvider;

        public FileCache(IMemoryCache cache, IWebHostEnvironment env)
        {
            _cache = cache;
            _fileProvider = env.ContentRootFileProvider;
        }

        public async Task<string> GetFileContents(string fileName)
        {
            var filePath = _fileProvider.GetFileInfo(fileName).PhysicalPath;

            // try to obtain the file contents from the cache.
            if (_cache.TryGetValue(filePath, out string fileContent))
            {
                return "cache => " + fileContent;
            }

            // cache doesn't have the entry, so obtain from the file itself.
            fileContent = await GetFileContent(filePath);

            if (fileContent != null)
            {
                // obtain a change token from the file provider
                // whose callback is triggered when the file is modified.
                var changeToken = _fileProvider.Watch(fileName);

                // configure the cache entry options,
                // with five minute sliding expiration
                // use the change token to expire the file in the cache if modified.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .AddExpirationToken(changeToken);

                // Put the file contents into the cache.
                _cache.Set(filePath, fileContent, cacheEntryOptions);

                return "file=> " + fileContent;
            }

            return string.Empty;
        }

        public async static Task<string> GetFileContent(string filePath)
        {
            var runCount = 1;

            while (runCount < 4)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        using (var fileStreamReader = File.OpenText(filePath))
                        {
                            return await fileStreamReader.ReadToEndAsync();
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException();
                    }
                }
                catch (IOException ex)
                {
                    if (runCount == 3 || ex.HResult != -2147024864)
                    {
                        throw;
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, runCount)));
                        runCount++;
                    }
                }
            }

            return null;
        }
    }
}
