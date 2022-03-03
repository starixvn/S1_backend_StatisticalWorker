using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StatisticalWorker.Service.Library
{
    public class Log2File
    {
        #region Field

        private readonly ILogger<Log2File> _logger;

        #endregion

        #region Constructors

        public Log2File(ILogger<Log2File> logger)
        {
            _logger = logger;
        }

        #endregion

        #region Public Methods

        public void Info(string InfoContent, string PrefixName)
        {
            LogContext.PushProperty("PrefixName", PrefixName);
            _logger.LogInformation(InfoContent);
        }

        public void Error(string ErrorContent, string PrefixName, Exception ex = null)
        {
            LogContext.PushProperty("PrefixName", PrefixName);
            if (ex != null)
            {
                _logger.LogError(ex, ErrorContent);
            }
            else
            {
                _logger.LogError(ErrorContent);
            }
        }

        #endregion

    }
}
