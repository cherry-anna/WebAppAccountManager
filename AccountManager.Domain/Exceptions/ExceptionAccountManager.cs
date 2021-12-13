using System;
using System.Globalization;

namespace AccountManager.Domain.Exceptions
{
    public class ExceptionAccountManager: Exception
    {
        public int StatusCode { get; set; }
        public ExceptionAccountManager() : base()
        {
        }
        public ExceptionAccountManager(string message) : base(message)
        {
        }
        public ExceptionAccountManager(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
        public ExceptionAccountManager(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
