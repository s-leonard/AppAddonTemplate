using System.IO;
using System.Text;
using System.Web;

namespace AddonTemplate.Web
{
	internal static class HttpRequestExtensions
	{
		public static string GetForwardedHostAddress(this HttpRequestBase httpRequest)
		{
			const string forwardedForHeader = "HTTP_X_FORWARDED_FOR";

			var forwardedFor = httpRequest.ServerVariables[forwardedForHeader];
			if (forwardedFor != null)
			{
				return forwardedFor;
			}

			return httpRequest.UserHostAddress;
		}

        public static string GetBody(this HttpRequestBase httpRequest)
        {
            string requestBody;
            httpRequest.InputStream.Position = 0;
            using (Stream receiveStream = httpRequest.InputStream)
            {
                using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    requestBody = readStream.ReadToEnd();
                }
            }
            return requestBody;
        }
	}
}
