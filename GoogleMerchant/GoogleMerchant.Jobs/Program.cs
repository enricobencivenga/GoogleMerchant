using Google.Apis.ShoppingContent.v2;
using GoogleMerchant.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleMerchant.Jobs
{
    class Program
    {
        private static readonly int MaxListPageSize = 50;


        [STAThread]
        internal static void Main(string[] args)
        {
            EntryService.Run(args);
        }
    }
}
