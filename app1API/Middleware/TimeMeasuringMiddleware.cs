using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace app1API.Middleware
{
    public class TimeMeasuringMiddleware: IMiddleware
    {
        private Stopwatch _stopwatch;
        private readonly ILogger<TimeMeasuringMiddleware> _logger;

        public TimeMeasuringMiddleware(ILogger<TimeMeasuringMiddleware> logger)
        {
            _stopwatch = new Stopwatch();
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _stopwatch.Start();
            await next.Invoke(context);
            _stopwatch.Stop();

            long elaspedMilliseconds = _stopwatch.ElapsedMilliseconds;

            if (elaspedMilliseconds / 1000 > 4)
            {
                var message = $"Request {context.Request.Method} at {context.Request.Path} took {elaspedMilliseconds} ms";

                _logger.LogInformation(message);
            }
        }
    }
}
