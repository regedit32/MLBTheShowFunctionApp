﻿using System;
using System.Collections.Specialized;
using System.Web;

namespace MLBTheShowSharp.Utils
{
    internal static class UriHelper
    {
        /// <summary>
        /// Updates the query string parameters to a url
        /// </summary>
        /// <param name="uri">The uri to update parameters</param> 
        /// <param name="query">The parameters to update to the url</param> 
        /// <returns>The url updated with the specified query</returns>
        public static Uri AddOrUpdateQuery(this Uri uri, NameValueCollection query)
        {
            var url = UpdateQuery(uri.AbsoluteUri, queryString =>
            {
                CopyProps(query, queryString);
            });

            return new Uri(url);
        }

        private static void CopyProps(NameValueCollection source, NameValueCollection destination)
        {
            foreach (string key in source)
            {
                destination[key] = source[key];
            }
        }

        private static string UpdateQuery(string url, Action<NameValueCollection> modifyQuery)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            modifyQuery(query);
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri.ToString();
        }
    }
}
