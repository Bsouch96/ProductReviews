using Invoices.Helpers.Interface;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using ProductReviews.DomainModels;
using ProductReviews.Models;
using ProductReviews.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Invoices.Helpers.Concrete
{
    public class MemoryCacheAutomater : IMemoryCacheAutomater
    {
        private readonly IProductReviewsRepository _productReviewsRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IOptionsMonitor<MemoryCacheModel> _optionsMonitor;

        public MemoryCacheAutomater(IProductReviewsRepository productReviewsRepository, IMemoryCache memoryCache, IOptionsMonitor<MemoryCacheModel> optionsMonitor)
        {
            _productReviewsRepository = productReviewsRepository;
            _memoryCache = memoryCache;
        }

        public void AutomateCache()
        {
            RegisterCache(_optionsMonitor.CurrentValue.ProductReviews, null, EvictionReason.None, null);
        }

        private MemoryCacheEntryOptions GetMemoryCacheEntryOptions()
        {
            int cacheExpirationMinutes = 1;
            DateTime cacheExpirationTime = DateTime.Now.AddMinutes(cacheExpirationMinutes);
            CancellationChangeToken cacheExpirationToken = new CancellationChangeToken
            (
                new CancellationTokenSource(TimeSpan.FromMinutes(cacheExpirationMinutes + 0.01)).Token
            );

            return new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(cacheExpirationTime)
                .SetPriority(CacheItemPriority.NeverRemove)
                .AddExpirationToken(cacheExpirationToken)
                .RegisterPostEvictionCallback(callback: RegisterCache, state: this);
        }

        private async void RegisterCache(object key, object value, EvictionReason reason, object state)
        {
            List<ProductReviewModel> productReviewModels = await _productReviewsRepository.GetAllProductReviewsAsync();
            _memoryCache.Set(key, productReviewModels, GetMemoryCacheEntryOptions());
        }
    }
}
