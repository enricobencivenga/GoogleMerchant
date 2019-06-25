using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleMerchant.Service
{
    public class EntryService
    {
        public static void Run(string[] args)
        {
            var samples = new ShoppingContentSampleService();
            samples.startSamples(args);
        }
    }
}
