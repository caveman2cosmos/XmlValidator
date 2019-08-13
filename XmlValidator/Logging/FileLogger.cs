namespace XmlValidator.Logging
{
    public class FileLogger : ILogWriter
    {
        private readonly System.IO.StreamWriter _mLogFile;

        public FileLogger(string strFilePath)
        {
            if (!System.IO.File.Exists(strFilePath))
            {
                System.IO.File.Create(strFilePath).Close();
            }
            //Open the log file
            _mLogFile = new System.IO.StreamWriter(strFilePath, false) { AutoFlush = true };
        }

        public void WriteLine(string strText)
        {
            //Log
            _mLogFile.WriteLine(strText);
        }
    }
}
