using System;
using System.Reflection;
using System.Text;
using TTMapSchemaGUID.Utility;

namespace TTMapSchemaGUID
{
    /// <summary>
    /// Class MapSchemaGUID
    /// </summary>
    public class MapSchemaGUID
    {
        string[] args;
        string appName;
        string appVersion;

        MapParam mapParam = new MapParam();


        public MapSchemaGUID(string[] args)
        {
            this.args = args;
            appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            appVersion = "v" + Assembly.GetEntryAssembly().GetName().Version.ToString().Substring(0,3);
        }

        /// <summary>
        /// Methode DoMap uses the class <see cref="DSMap"/> to get the mapped name of the giving Guid
        /// </summary>
        public void DoMap()
        {
            DisplayHeader();

            if (CheckParameter(args, mapParam))
            {
                DSMap dsMap = new DSMap(mapParam);
                try
                {
                    mapParam = dsMap.DoMapSchema();
                }
                catch (DSBindingException dsEx)
                {
                    DisplayErrorMsg(dsEx.Message);
                    mapParam = null;
                }
                catch (Exception ex)
                {
                    DisplayErrorMsg(ex.Message);
                    mapParam = null;
                }

                if (mapParam != null)
                {
                    DisplayMap(mapParam);
                }
            }
            else
                DisplayHelp();

        }
        /// <summary>
        /// Function DisplayMap display the mapping of the <see cref="MapParam"/> object
        /// </summary>
        /// <param name="mapParam"></param>
        private void DisplayMap(MapParam mapParam)
        {
            Console.WriteLine();
            Console.WriteLine("GUID: {0}", mapParam.GuId);
            Console.WriteLine("Anzeigename: {0}", mapParam.DisplayName);
#if DEBUG
            Console.ReadKey();
#endif
        }
        /// <summary>
        /// Methode CheckParameter checkt the arguments of the command line and put them in mapParam
        /// if everything is alright. CheckParameter use the Parameter Class <see cref="Parameter"/>
        /// </summary>
        /// <param name="args"></param>
        /// <param name="mapParam"></param>
        /// <returns></returns>
        private bool CheckParameter(string[] args, MapParam mapParam)
        {
            const string DOMAINCON = "dc";
            const string DOMAIN = "domain";
            const string GUID = "guid";
            const string HELP = "?";

            string errMsg = "";
            bool error = false;


            bool helpAsked = false;

            Parameter CommandLine = new Parameter(args);

            if (CommandLine[DOMAINCON] != null)
                mapParam.DomainCon = CommandLine[DOMAINCON];
            else
                errMsg += "Domain Controller nicht definiert!\n";

            if (CommandLine[DOMAIN] != null)
                mapParam.Domain = CommandLine[DOMAIN];
            else
                errMsg += "Domain nicht definiert!\n";

            if (CommandLine[GUID] != null)
                mapParam.GuId = CommandLine[GUID];
            else
                errMsg += "GuID nicht definiert!\n";

            helpAsked = CommandLine[HELP] != null;
                
            error = !string.IsNullOrEmpty(errMsg);

            if (helpAsked)
            {
                DisplayHelp();
            }
            else if(error)
            {
                DisplayParameterError(errMsg);
            }

            return !error;
        }

        /// <summary>
        /// Methode DisplayParameterError shows the error messages in errMsg, when CheckParameter failed.
        /// </summary>
        /// <param name="errMsg"></param>
        private void DisplayParameterError(string errMsg)
        {
            errMsg = "Unzulässige Parameteranzahl\n"; //+ errMsg;
            Console.WriteLine(errMsg);
        }
        /// <summary>
        /// Methode DisplayErrorMsg shows the error in errMsg, when catching Exception
        /// </summary>
        /// <param name="errMsg"></param>
        private void DisplayErrorMsg(string errMsg)
        {
            Console.WriteLine(errMsg);
        }
        /// <summary>
        /// Methode DisplayHelp shows the help text when /? found in the paramaters or <see cref="CheckParameter(string[], MapParam)"/> failed.
        /// </summary>
        private void DisplayHelp()
        {
            StringBuilder helpBuilder = new StringBuilder();

            helpBuilder.AppendLine();
            helpBuilder.AppendLine("Syntax:");
            helpBuilder.AppendLine("    {0} [/?] | /dc:[DomainController] /domain:[Domain] /guid:[GUID]");
            helpBuilder.AppendLine();
            helpBuilder.AppendLine("Wobei:");
            helpBuilder.AppendLine("    DomainController  Der Domaincontroller");
            helpBuilder.AppendLine("    Domain            Die Domaine");
            helpBuilder.AppendLine("    GUID              GUID von dem Schema-Objekt, dessen Anzeigename ermittelt werden soll.");
            helpBuilder.AppendLine();
            helpBuilder.AppendLine("    Optionen:");
            helpBuilder.AppendLine("       /?             Zeigt diese Hilfe an");
            helpBuilder.AppendLine();
            helpBuilder.AppendLine("Beispiel:");
            helpBuilder.AppendLine("    > TTMapSchemaGUID /dc:contoso-dc01.contoso.com /domain:contoso.com /guid:771727b1-31b8-4cdf-ae62-4fe39fadf89e");
            Console.WriteLine(helpBuilder.ToString(), appName);
#if DEBUG
            Console.ReadKey();
#endif
        }
        /// <summary>
        /// Methode DisplayHeader shows informations about this application
        /// </summary>
        private void DisplayHeader()
        {
            StringBuilder headerBuilder = new StringBuilder();
            headerBuilder.AppendLine();
            headerBuilder.AppendLine(string.Format("{0} {1} von TOP TECHNOLOGIES CONSULTING GmbH\n", appName, appVersion));
            headerBuilder.AppendLine("Ermittelt den Anzeigenamen der GUID eines Schema-Objekts aus dem Active Directory. ");
            headerBuilder.Append("Es wird keine ausdrückliche oder implizite Garantie irgendeiner Art und Weise übernommen; ");
            headerBuilder.Append("die Verwendung erfolgt auf eigene Verantwortung. Dieses Programm und dessen Quellcode ist Public Domain. ");
            headerBuilder.Append("Die aktuelle Version und/oder der Quellcode werden auf Anfrage bereitgestellt (info@toptechnologies.de).");
            Console.WriteLine(headerBuilder.ToString());
        }
    }
}
