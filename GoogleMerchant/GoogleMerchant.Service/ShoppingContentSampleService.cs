using System;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.ShoppingContent.v2;
using CommandLine;

namespace GoogleMerchant.Service
{
    internal class ShoppingContentSampleService : BaseContentService
    {
        private static readonly int MaxListPageSize = 50;


        internal override void runCalls()
        {         
            var datafeedsService = new DatafeedsService(service);
            var productsService = new ProductsService(service, MaxListPageSize);         

            ulong merchantId = config.MerchantId.Value;

            productsService.RunCalls(merchantId, config.WebsiteURL);
            datafeedsService.RunCalls(merchantId);             
        }
    }
}
