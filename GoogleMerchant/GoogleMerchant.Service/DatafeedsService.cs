﻿using Google.Apis.ShoppingContent.v2;
using System;
using System.Collections.Generic;
using System.Net;
using Google.Apis.ShoppingContent.v2;
using Google.Apis.ShoppingContent.v2.Data;
using GoogleMerchant.Service.Utilities;

namespace GoogleMerchant.Service
{
    /// <summary>
    /// A sample consumer that runs multiple requests against the Datafeeds
    /// service in the Content API for Shopping.
    /// <para>These include:
    /// <list type="bullet">
    /// <item>
    /// <description>Datafeeds.list</description>
    /// </item>
    /// <item>
    /// <description>Datafeeds.insert</description>
    /// </item>
    /// <item>
    /// <description>Datafeeds.patch</description>
    /// </item>
    /// <item>
    /// <description>Datafeeds.delete</description>
    /// </item>
    /// </list></para>
    /// </summary>
    public class DatafeedsService
    {
        private ShoppingContentService service;
        ShoppingUtilities shoppingUtilities = new ShoppingUtilities();
        // Currently, we may receive a 401 Unauthorized error if a datafeed is not yet
        // available soon after creating it, so retry if we see one while making a modification
        // to or deleting a datafeed. The specific HTTP error we receive may be subject to change.
        IEnumerable<HttpStatusCode> retryCodes = new HttpStatusCode[] { HttpStatusCode.Unauthorized };

        /// <summary>Initializes a new instance of the <see cref="DatafeedsSample"/> class.</summary>
        /// <param name="service">Content service object on which to run the requests.</param>
        public DatafeedsService(ShoppingContentService service)
        {
            this.service = service;
        }

        /// <summary>Runs multiple requests against the Content API for Shopping.</summary>
        internal void RunCalls(ulong merchantId)
        {
            // Datafeeds
            GetAllDatafeeds(merchantId);
            Datafeed newDatafeed = InsertDatafeed(merchantId);
            UpdateDatafeed(merchantId, (ulong)newDatafeed.Id);
            DeleteDatafeed(merchantId, (ulong)newDatafeed.Id);
        }

        /// <summary>Gets and prints all datafeeds for the given merchant ID.</summary>
        /// <returns>The last page of retrieved accounts.</returns>
        private DatafeedsListResponse GetAllDatafeeds(ulong merchantId)
        {
            Console.WriteLine("=================================================================");
            Console.WriteLine("Listing all Datafeeds");
            Console.WriteLine("=================================================================");

            // Retrieve account list in pages and display data as we receive it.
            DatafeedsListResponse datafeedsResponse = null;

            DatafeedsResource.ListRequest accountRequest = service.Datafeeds.List(merchantId);
            datafeedsResponse = accountRequest.Execute();

            if (datafeedsResponse.Resources != null && datafeedsResponse.Resources.Count != 0)
            {
                foreach (var datafeed in datafeedsResponse.Resources)
                {
                    Console.WriteLine(
                        "Datafeed with ID \"{0}\" and name \"{1}\" was found.",
                        datafeed.Id,
                        datafeed.Name);
                }
            }
            else
            {
                Console.WriteLine("No accounts found.");
            }

            Console.WriteLine();

            // Return the last page of accounts.
            return datafeedsResponse;
        }

        /// <summary>
        /// Updates a datafeed using the Datafeeds.patch method.
        /// </summary>
        private void UpdateDatafeed(ulong merchantId, ulong datafeedId)
        {
            Console.WriteLine("=================================================================");
            Console.WriteLine(String.Format("Updating datafeed {0}", datafeedId));
            Console.WriteLine("=================================================================");

            Datafeed datafeed = new Datafeed();
            datafeed.FetchSchedule = new DatafeedFetchSchedule();
            datafeed.FetchSchedule.Hour = 7;

            var request = service.Datafeeds.Patch(datafeed, merchantId, datafeedId);
            Datafeed response = shoppingUtilities.ExecuteWithRetries(request, retryCodes);
            Console.WriteLine(
                "Datafeed updated with ID \"{0}\" and name \"{1}\".",
                response.Id,
                response.Name);
            Console.WriteLine();
        }

        /// <summary>
        /// Adds a datafeed to the specified account.
        /// </summary>
        /// <returns>The datafeed that was inserter</returns>
        private Datafeed InsertDatafeed(ulong merchantId)
        {
            Console.WriteLine("=================================================================");
            Console.WriteLine("Inserting a datafeed");
            Console.WriteLine("=================================================================");
            Datafeed datafeed = GenerateDatafeed();

            Datafeed response = service.Datafeeds.Insert(datafeed, merchantId).Execute();
            Console.WriteLine(
                "Datafeed inserted with ID \"{0}\" and name \"{1}\".",
                response.Id,
                response.Name);
            Console.WriteLine();
            return response;
        }

        /// <summary>
        /// Removes a datafeed from the specified account.
        /// </summary>
        private void DeleteDatafeed(ulong merchantId, ulong datafeedId)
        {
            Console.WriteLine("=================================================================");
            Console.WriteLine(String.Format("Deleting datafeed {0}", datafeedId));
            Console.WriteLine("=================================================================");

            var request = service.Datafeeds.Delete(merchantId, datafeedId);
            shoppingUtilities.ExecuteWithRetries(request, retryCodes);

            Console.WriteLine("Datafeed with ID \"{0}\" was deleted.", datafeedId);
            Console.WriteLine();
        }

        internal Datafeed GenerateDatafeed()
        {
            String name = String.Format("datafeed{0}", shoppingUtilities.GetUniqueId());
            Datafeed datafeed = new Datafeed();
            datafeed.Name = name;
            datafeed.ContentType = "products";
            datafeed.AttributeLanguage = "en";
            datafeed.ContentLanguage = "EN";
            datafeed.IntendedDestinations = new List<String>();
            datafeed.IntendedDestinations.Add("Shopping");
            datafeed.FileName = name;
            datafeed.TargetCountry = "US";
            datafeed.FetchSchedule = new DatafeedFetchSchedule();
            datafeed.FetchSchedule.Weekday = "monday";
            datafeed.FetchSchedule.Hour = 6;
            datafeed.FetchSchedule.TimeZone = "America/Los_Angeles";
            datafeed.FetchSchedule.FetchUrl = "http://feeds.my-shop.com/" + name;
            datafeed.Format = new DatafeedFormat();
            datafeed.Format.FileEncoding = "utf-8";
            datafeed.Format.ColumnDelimiter = "tab";
            datafeed.Format.QuotingMode = "value quoting";

            return datafeed;
        }
    }
}

