using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services
{
    public interface IErrorHandlerService
    {
        ErrorVM ReadError(int traceId);
        ServerException WriteLog(string message, Exception e, DateTime dateTime, string side, string where);
    }
    public class ErrorHandlerService : IErrorHandlerService
    {
        private readonly object lockObj = new object();
        private int TraceId = 0;

        public ErrorHandlerService()
        {
            Directory.CreateDirectory("Logs");
            Directory.CreateDirectory("Logs/Errors");
        }

        public ErrorVM ReadError(int traceId)
        {
            StreamReader sr = null;
            try
            {
                sr = new StreamReader("Logs/Errors/" + traceId.ToString() + ".txt");
                return new ErrorVM
                {
                    Message = sr.ReadLineAsync().GetAwaiter().GetResult(),
                    DateTime = new DateTime(long.Parse(sr.ReadLineAsync().GetAwaiter().GetResult())),
                    Side = sr.ReadLineAsync().GetAwaiter().GetResult(),
                    Where = sr.ReadLineAsync().GetAwaiter().GetResult(),
                    ErrorMessage = sr.ReadLineAsync().GetAwaiter().GetResult(),
                    InnerErrorMessage = sr.ReadLineAsync().GetAwaiter().GetResult(),
                    StackTrace = sr.ReadToEndAsync().GetAwaiter().GetResult()
                };
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }
        }

        public ServerException WriteLog(string message, Exception e, DateTime dateTime, string side, string where)
        {
            lock (lockObj)
            {
                string innerMessage = "";
                if (e.InnerException != null)
                {
                    innerMessage = e.InnerException.Message;
                }
                File.WriteAllTextAsync("Logs/Errors/" + TraceId.ToString() + ".txt",
                    message + "\n" + dateTime.Ticks.ToString() + "\n" + side + "\n" + where + "\n"
                    + e.Message + "\n" + innerMessage + "\n" + e.StackTrace).GetAwaiter().GetResult();
                return new ServerException(message, e.Message)
                {
                    TraceId = TraceId++,
                    Side = side
                };
            }
        }
    }
}
