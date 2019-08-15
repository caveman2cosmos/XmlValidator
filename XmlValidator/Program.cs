using XmlValidator.Logging;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;

namespace XmlValidator
{
    class Program
    {
        private static Log _log;
        private static int _fileCount;

        static int Main(string[] args)
        {
            Console.SetWindowSize(Math.Min(130, Console.LargestWindowWidth), Math.Min(35, Console.LargestWindowHeight));
            Console.Out.WriteLine($"Civ4 XML-Validator C2C Edition {Assembly.GetExecutingAssembly().GetName().Version} by Alberts2");
            Console.Out.WriteLine();

            var failed = false;
            var automated = args.Contains("-a", StringComparer.InvariantCultureIgnoreCase);
            if (Init())
            {
                try
                {
                    _log.LogWriteLine("Validating xml");
                    _fileCount = 0;
                    var directories = new[] { "..\\Assets\\Xml", "..\\Assets\\Modules" };
                    foreach (var dir in directories)
                    {
                        var path = Path.GetFullPath(dir);
                        if (Directory.Exists(path))
                        {
                            failed = !ValidateDirectory(path);
                            if (failed) break;
                        }
                    }

                    _log.LogWriteSeperator();

                    if (failed)
                    {
                        _log.LogWriteLine($"Validation failed!");
                    }
                    else
                    {
                        _log.LogWriteLine($"Validation of {_fileCount} files complete without error(s)!");
                    }

                }
                catch (Exception ex)
                {
                    _log.LogWriteLine(ex.ToString());
                    failed = true;
                }
            }

            if (!automated)
            {
                Console.Out.WriteLine();
                Console.Out.WriteLine("Press any key to exit....");

                while (true)
                {
                    Console.ReadKey(true);
                    break;
                } 
            }

            return failed ? -1 : 0;
        }

        private static bool Init()
        {
            try
            {
                _log = new Log(new ConsoleLogger(), new FileLogger("XmlValidator.log"));
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex);
                return false;
            }
            return true;
        }

        private static bool ValidateDirectory(string strPath)
        {
            foreach (var file in Directory.EnumerateFiles(strPath, "*.xml", SearchOption.AllDirectories).Where(f => !f.ToLowerInvariant().Contains("schema.xml")))
            {
                var failed = !ValidateFile(file);
                if(failed)
                    return false;
            }
            return true;
        }
        private static bool ValidateFile(string file)
        {
            _log.LogWriteLine($"Validating {file}");
            _fileCount++;
            var failed = false;
            try
            {
                using (var xmlTextReader = new XmlTextReader(file))
                {
                    using (var validatingReader = new XmlValidatingReader(xmlTextReader)
                    {
                        ValidationType = ValidationType.Auto
                    })
                    {
                        validatingReader.ValidationEventHandler += (o, e) => throw e.Exception;
                        while (validatingReader.Read()) {}
                    }
                }
            }
            catch (XmlSchemaException ex)
            {
                failed = true;
                _log.LogWriteLine($"{ex.LineNumber},{ex.LinePosition}: {ex.Message}");
            }
            catch (XmlException ex)
            {
                failed = true;
                _log.LogWriteLine($"{ex.LineNumber},{ex.LinePosition}: {ex.Message}");
            }
            catch (Exception ex)
            {
                failed = true;
                _log.LogWriteLine(ex.ToString());
            }

            return !failed;
        }
    }
}
