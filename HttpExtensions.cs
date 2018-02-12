using System;
using System.Web;
using Xunit;

namespace Snippets
{
    public static class HttpExtensions
    {
        public static Uri AddQuery(this Uri uri, string name, string value)
        {
            var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

            httpValueCollection.Remove(name);
            httpValueCollection.Add(name, value);

            var ub = new UriBuilder(uri) {Query = httpValueCollection.ToString()};

            return ub.Uri;
        }

        [Fact]
        public static void Test()
        {
            var url = new Uri("http://localhost/rest/something/browse")
                .AddQuery("page", "0")
                .AddQuery("pageSize", "200").ToString();
            Assert.Equal("http://localhost/rest/something/browse?page=0&pageSize=200", url);
        }
    }


}
