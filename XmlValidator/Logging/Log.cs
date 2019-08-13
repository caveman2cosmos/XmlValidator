using System;
using System.Collections.Generic;

namespace XmlValidator.Logging
{
    class Log
    {
        private readonly List<ILogWriter> _logWriters;

        public Log(params ILogWriter[] logWriters) => _logWriters = new List<ILogWriter>(logWriters);

        public void LogWriteSeperator() => LogWriteLine("-----------------------------------");

        public void LogWriteLine(string strText = "") => LogWriteLineInternal(strText);

        private void LogWriteLineInternal(string strText)
        {
            try
            {
                foreach (var logger in _logWriters)
                {
                    logger.WriteLine(strText);
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex);
            }
        }
    }
}
