﻿using System;
using FFImageLoading.Work;
using System.IO;
using System.Threading.Tasks;
using FFImageLoading.Cache;
using System.Threading;

namespace FFImageLoading.Work.DataResolver
{
	public class UrlDataResolver : IDataResolver
	{

		protected TaskParameter Parameters { get; private set; }
		protected IDownloadCache DownloadCache { get; private set; }

		public UrlDataResolver(TaskParameter parameter, IDownloadCache downloadCache) {
			Parameters = parameter;
			DownloadCache = downloadCache;
		}
		
		public async Task<UIImageData> GetData(string identifier, CancellationToken token)
		{
			var downloadedData = await DownloadCache.GetAsync(identifier, token, Parameters.CacheDuration).ConfigureAwait(false);
			var bytes = downloadedData.Bytes;
			var path = downloadedData.CachedPath;
			var result = downloadedData.RetrievedFromDiskCache ? LoadingResult.DiskCache : LoadingResult.Internet;
			return new UIImageData() { Data = bytes, Result = result, ResultIdentifier = path };
		}

		public void Dispose() {
			Parameters = null;
			DownloadCache = null;
		}
		
	}
}

