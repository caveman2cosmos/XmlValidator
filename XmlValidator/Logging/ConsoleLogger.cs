using System;

namespace XmlValidator.Logging
{
    class ConsoleLogger : ILogWriter
    {
        public void WriteLine(string strText)
        {
            Console.Out.WriteLine(strText);
        }
    }
}
