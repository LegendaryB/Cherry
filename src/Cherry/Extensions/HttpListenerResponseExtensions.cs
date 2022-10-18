using System.Net;
using System.Text;
using System;

namespace Cherry.Extensions
{
    public static class HttpListenerResponseExtensions
    {
        /// <summary>
        /// Answers a request with the given status code and a response body.
        /// </summary>
        /// <param name="body">The response body as a string. UTF-8 encoding is used. If another encoding is needed the <seealso cref="AnswerWithStatusCodeAsync(HttpListenerResponse, byte[], HttpStatusCode)"/> method should be used.</param>
        /// <param name="statusCode">The status code which should be send back to the client.</param>
        /// <returns></returns>
        public static async Task AnswerWithStatusCodeAsync(
            this HttpListenerResponse res,
            string body,
            HttpStatusCode statusCode)
        {
            await AnswerWithStatusCodeAsync(
                res,
                Encoding.UTF8.GetBytes(body),
                statusCode);
        }

        /// <summary>
        /// Answers a request with the given status code and a response body.
        /// </summary>
        /// <param name="statusCode">The status code which should be send back to the client.</param>
        /// <returns></returns>
        public static async Task AnswerWithStatusCodeAsync(
            this HttpListenerResponse res, 
            byte[] body, 
            HttpStatusCode statusCode)
        {
            res.StatusCode = (int)statusCode;
            await res.OutputStream.WriteAsync(body);
            res.Close();
        }

        /// <summary>
        /// Answers a request with the given status code and NO body.
        /// </summary>
        /// <param name="statusCode">The status code which should be send back to the client.</param>
        public static void AnswerWithStatusCode(
            this HttpListenerResponse res, 
            HttpStatusCode statusCode)
        {
            res.StatusCode = (int)statusCode;
            res.Close();
        }
    }
}
