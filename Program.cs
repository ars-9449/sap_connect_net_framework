using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using SAP.Middleware.Connector;

namespace sap_connect_net_framework
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string VIMFM = "Z_ECME017_VIM_TRANSFER";
            ECCDestinationConfig cfg = null;
            RfcDestination dest = null;
            cfg = new ECCDestinationConfig();
            RfcDestinationManager.RegisterDestinationConfiguration(cfg);
            dest = RfcDestinationManager.GetDestination("ABAP_APP_SERVER");
            IRfcFunction VimFM = dest.Repository.CreateFunction(VIMFM); //получение функции

            // получаю файлы
            string XML = "F:\\conn\\InputFolder\\109673147SEVERSTALkgb031200186556.xml";
            string PDF = "F:\\conn\\InputFolder\\109673147SEVERSTALkgb031200186556.pdf";
            String pdfname = Path.GetFileName(PDF);
            string XLSX = "F:\\conn\\InputFolder\\109673147SEVERSTALkgb031200186556.xlsx";
            String xlsxname = Path.GetFileName(XLSX);

            byte[] xmlCont = File.ReadAllBytes(XML);
            VimFM.SetValue("IV_XML_X", xmlCont);

            byte[] pdfCont = File.ReadAllBytes(PDF);
            byte[] xlsxCont = File.ReadAllBytes(XLSX);
            IRfcTable files = VimFM.GetTable("IT_FILE");
            files.Append();
            files.SetValue("NAME", pdfname);
            files.SetValue("CONT_X", pdfCont);
            files.Append();
            files.SetValue("NAME", xlsxname);
            files.SetValue("CONT_X", xlsxCont);
            VimFM.Invoke(dest);
            foreach (IRfcParameter parameter in VimFM)
            {
                string sap_parameter = parameter.Metadata.Name;
                if (sap_parameter == "EV_ERROR")
                {
                    string errorFromSAP = parameter.GetString();
                    if (errorFromSAP == "X")
                    {
                        foreach (IRfcParameter parameter1 in VimFM)
                        {
                            string sap_parameter1 = parameter1.Metadata.Name;
                            if (sap_parameter1 == "ET_RETURN")
                            {
                                IRfcTable table = parameter1.GetTable();
                                try
                                {
                                    string errormessage = table.GetString(3);
                                    if (errormessage != null)
                                    {

                                    }
                                }
                                catch { }
                            }
                        }
                    }
                    else
                    {
                        //все хорошо
                    }
                }
            }
        }
        public class ECCDestinationConfig : IDestinationConfiguration
        {
            public bool ChangeEventsSupported()
            {
                return true;
            }

            public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged;


            public RfcConfigParameters GetParameters(string destionationName)
            {
                RfcConfigParameters parms = new RfcConfigParameters();

                //SAP Parameters  
                if (destionationName.Equals("ABAP_APP_SERVER"))
                {
                    string client = "001";
                    string system_number = "00";
                    string system_id = "NPL";
                    string language = "RU";
                    string ashost = "vhcalnplci";
                    string login = "NET_RFC";
                    string password = "1qAZ2wSX";
                    parms.Add(RfcConfigParameters.AppServerHost, ashost);
                    parms.Add(RfcConfigParameters.SystemNumber, system_number);
                    parms.Add(RfcConfigParameters.SystemID, system_id);
                    parms.Add(RfcConfigParameters.User, login);
                    parms.Add(RfcConfigParameters.Password, password);
                    parms.Add(RfcConfigParameters.Client, client);
                    parms.Add(RfcConfigParameters.Language, language);

                }

                return parms;
            }
        }

    }
}
