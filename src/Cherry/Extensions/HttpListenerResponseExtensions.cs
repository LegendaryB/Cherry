﻿using System.Net;
using System.Text;
using System;
using System.Net.Mime;

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
            var bytes = Encoding.UTF8.GetBytes(body);

            res.ContentEncoding = Encoding.UTF8;

            await AnswerWithStatusCodeAsync(
                res,
                bytes,
                statusCode,
                MediaTypeNames.Text.Plain);
        }

        /// <summary>
        /// Answers a request with the given status code and a response body.
        /// </summary>
        /// <param name="statusCode">The status code which should be send back to the client.</param>
        /// <returns></returns>
        public static async Task AnswerWithStatusCodeAsync(
            this HttpListenerResponse res, 
            byte[] body, 
            HttpStatusCode statusCode,
            string contentType = MediaTypeNames.Application.Octet)
        {
            res.ContentType = contentType;
            res.ContentLength64 = body.Length;
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
