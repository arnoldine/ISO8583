using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Data;
using Microsoft.Owin.Hosting;
using System.Net;
using System.Threading;
using System.Data.SQLite;
using System.Data.SqlClient;
using Owin;
namespace ISO8583_Test
{
    // State object for reading client data asynchronously
   
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
        
    }
   

    public class Program
    {
        public static string clientnm;
        public static string URL = "https://127.0.0.1:443";
        public static string urlParameters = "?grant_type=password&username=AD00101&password=Seekr3t!!!";
        public static string L_CON_STR = "Data Source=localdb;Initial Catalog=trxlogs;User ID=Usapi;Password=*****"; //live code
        public static string T_CON_STR = "Data Source=.\\;Initial Catalog=trxlogs;Integrated Security=True";  //Test Code
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public static string data = null;
        public static string CON_STR = L_CON_STR;
        public static string apmode = null;
        public static int cport = 9500;
        //public AsynchronousSocketListener()
        //{
        //}
        public static DataTable localstorage = new DataTable("Transactions");
        public static string revaccount,account = null;
        public static string revamt,revtime = null;
        static int Main(string[] args)
        {
            var loginf = NLog.LogManager.GetCurrentClassLogger();
            apmode = "ISO LIVE";
            if (args.Length > 0)
            {
                if (args[0] == "/test")
                {
                    apmode = "ISO UAT";
                    URL = "https://192.168.70.17:443";
                   // CON_STR = T_CON_STR;
                    cport = 8500;
                }
                
                if (args[0] == "/http")
                {
                    apmode = "API001";
                    string ip = null;
                    try
                    {
                        ip = args[1];
                    }
                    catch
                    {
                        if (ip == null)
                        {
                          //  Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.WriteLine("ASL Quipu Connector !!!!");
                            Console.WriteLine("{0} : No Host specified using http://localhost:8100", DateTime.Now);
                            ip = "http://localhost:8100";

                        }
                    }
                    //----"http://197.159.142.173:21081"
                    using (WebApp.Start<Startup>(ip))

                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;

                        Console.WriteLine("{0} Optiplus Connector is running on :\n {1} ", DateTime.Now, ip);

                        Console.WriteLine("Press any key to quit.");

                        Console.ReadLine();


                    }
                }
            }
            
            loginf.Info($"Started: ");
          //savetrxnlogs("200120232", "1421000", "503000000", "150");
            createtable();
          //  loginf.Info($"DOME ATM CASH:: {getaccountnum("ASL0201")}");
          loginf.Info($"APPLICATION RUNNING IN::{apmode} MODE");
            loginf.Info($"ON PORT:: {cport}");
            loginf.Info($"CBS CWNET Running on:: {URL}");
          //  Console.WriteLine("Starting...{0}", DateTime.Now);
            //   Console.WriteLine("{0}", );

            StartListening();
            return 0;

            //  Console.ReadLine();
        }
        static void createtable()
        {
            //if (localstorage.Columns.Count > 1)
            //{
            //    return;
            //}
            //localstorage.Columns.Add("ExtRef", typeof(string));
            //localstorage.Columns.Add("RefId", typeof(string));
            //localstorage.Columns.Add("AccountNum", typeof(string));
            //localstorage.Columns.Add("Amount", typeof(string));
            //localstorage.Columns.Add("Stamp", typeof(string));
            // string CON_STR = "Data Source =" + "C:\\ISO_ATM" + @"\trxnlogs.db;version=3";
            //string CON_STR = @"Data Source=.\;Initial Catalog=trxlogs;Integrated Security=True";
           //
            
            /// SQLiteConnection mconn = new SQLiteConnection("Data Source=" + dbpath);
            //using (SqliConnection connection = new SqlConnection(CON_STR))

            using (SqlConnection connection = new SqlConnection(CON_STR))

            using (SqlCommand commnd = new SqlCommand("Select * from transactions where convert(date,stamp)=CONVERT(date,Getutcdate())", connection))
            {

                try
                {
                    connection.Open();
                    SqlDataAdapter adptbio = new SqlDataAdapter(commnd);
                    //SQLiteDataReader reader = commnd.ExecuteReader();
                    localstorage.Clear();
                    adptbio.Fill(localstorage);
                    Console.WriteLine($"{localstorage.Rows.Count}");
                    //while (reader.Read())
                    //{
                    //    //custids.Add(Convert.ToInt32(reader["clientid"]));
                    //    //fingerlist.Add(Fmd.DeserializeXml(Convert.ToString(reader["fingXml"])));
                    //    string act2debit = null;
                    //    act2debit = reader["Accountnum"].ToString();
                    //    return act2debit;
                    //}
                    // commnd.ExecuteNonQuery();
                    commnd.Dispose();
                    connection.Close();


                }
                catch (Exception km)
                {
                    connection.Close();
                    //  return "909";
                    //MessageBox.Show(km.Message);

                }
                //return "0000000000.0000";

            }
        }
        public static bool SaveToFile(DataTable[] NW, string path)
        {

            try
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    formatter.Serialize(stream, NW);
                    stream.Close();
                    System.IO.File.WriteAllBytes(path, stream.ToArray());
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static string getaccountnum(string atmcode)
        {
            Console.WriteLine($"ATM to Debit::{atmcode}");
            string CON_STRl = "Data Source =" + "C:\\ISO_ATM" + @"\trxnlogs.db;version=3";
            //string CON_STR = "Data Source =trxnlogs.db;version=3";

            // string CON_STR = "Server=192.168.70.11;Database=trxlogs;Integrated Security=True;MultipleActiveResultSets=true;Pooling=True;Max Pool Size=2500";
            // SQLiteConnection mconn = new SQLiteConnection("Data Source=" + dbpath);
            // using (SqlConnection connection = new SqlConnection(CON_STR))

            using (SQLiteConnection connection = new SQLiteConnection(CON_STRl))

            using (SQLiteCommand commnd = new SQLiteCommand("Select * from accountmaps where atmid='"
                + atmcode.Trim() + "'", connection))
            {

                try
                {
                    connection.Open();
                    // SQLiteDataAdapter adptbio = new SQLiteDataAdapter(commnd);
                    SQLiteDataReader reader = commnd.ExecuteReader();
                    // adptbio.Fill(clientsbiodata);
                    while (reader.Read())
                    {
                        //custids.Add(Convert.ToInt32(reader["clientid"]));
                        //fingerlist.Add(Fmd.DeserializeXml(Convert.ToString(reader["fingXml"])));
                        string act2debit = null;

                        act2debit = reader["Accountnum"].ToString();
                        Console.WriteLine($"::{act2debit}");
                        return act2debit;
                    }
                    // commnd.ExecuteNonQuery();
                    commnd.Dispose();
                    connection.Close();


                }
                catch (Exception km)
                {
                    connection.Close();
                    Console.WriteLine($"Error:: {km.Message}");
                    return "909";
                    //MessageBox.Show(km.Message);

                }
                return "85000001203936.1203";

            }
        }
            public static DataTable[] LoadFromFile(string path)
        {
            try
            {
                byte[] buffer = System.IO.File.ReadAllBytes(path);
                var stream = new System.IO.MemoryStream(buffer);
                System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (DataTable[])formatter.Deserialize(stream);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        static string getresponse(string inmsg)
        {
            string oeresponse = "";
            string tmpp = null; string accountnn = null;
            // Console.WriteLine("This message came in {0}", inmsg);
            createtable();
            var loginf = NLog.LogManager.GetCurrentClassLogger();
           // loginf.Info($"Received:{inmsg} ");
            f_ISO8583 iSO8583 = new f_ISO8583();
            string[] DE;
            string[] Resp;
            string without = inmsg.Substring(4, (inmsg.Length - 4));

          // Console.WriteLine("Got this now::{0} {1} ", (inmsg.Length - 4), (without));
            DE = iSO8583.Parse(without);
            Resp = DE;
            string MTI = without.Substring(0, 4);
          // Console.WriteLine("Without::"+without.Substring(4, 4));
            string reqid = new Random().Next(100000, 999999).ToString();
          // Resp[38] = reqid.Substring(0,6);
            //Resp[11]= reqid.Substring(0, 6);
            if (without.Substring(0, 4) == "0800")
            {
                loginf.Info($"Received:Echo... ");
                Resp[39] = "00";
             //  Resp[38] = null;
                // Console.WriteLine("Got it!");
                goto Loro;
            }
            loginf.Info($"Received:{inmsg} ");
            //string r41;
            //string r42 ;
            //string r43 ;
            //   Console.WriteLine(DE[3].ToString());
            int transtype = int.Parse(DE[3].Substring(0,2));
            // Console.WriteLine("Transcode::{0}", transtype);
            string typr="";
            accountnn = DE[102];
            switch (DE[3].Substring(0, 2))
            {
                case "38": /// Balance Enq
                //    Console.WriteLine("Got balance Enquiry");
                    string statem = getStatement(gettoken(), DE[102],"10");
                   // string stlen = statem.Length.ToString();
                    Resp[127] =  statem;
                    //string tyypo = null;
                    string fbalanceo = Balancecheck(gettoken(), DE[102],DE[49]);

                    string bklbal = fbalanceo;
                    // string fll = bkbal.Length.ToString();
                    Resp[39] = "00";
                    Resp[54] = bklbal;
                    break;
                

                case "40": // Funds Transfer
                    Decimal am= Convert.ToDecimal(DE[4]) / 100;
                    string trnsf = transfaccounts(gettoken(), DE[102], DE[103], DE[11], am.ToString(),DE[7]);
                    if ( trnsf == "00"){
                        Resp[39] = "00";
                        string balancer = Balancecheck(gettoken(), DE[102],DE[49]);
                        if (balancer == "14")
                        {
                            Resp[39] = "76";
                            goto Loro;

                        }
                        string tyy = null;
                        // r41 = DE[41].PadRight(16, '|');
                        // r42 = DE[42].PadRight(15, '|');
                        // r43 = DE[43].PadRight(40, '|');

                        //Resp[41] = r41;
                        //Resp[42] = r42;
                        //Resp[43] = r43;
                        
                        //  erred:
                        string bkkbalr = balancer;
                        // string flg = bkkbal.Length.ToString();
                        Resp[54] = bkkbalr;
                    }
                    Resp[39] = trnsf;
                    break;
                case "41":// NAME ENQ DR
                case "42":// NAME ENQ CR
                    string cnm = name_enquiry(gettoken(), DE[103]);
                   // Resp[103] = DE[71];
                    if (cnm != "14")
                    {
                    Resp[72] = cnm;
                        Console.WriteLine("..." + cnm);
                    Resp[39] = "00";
                    goto Loro;
                    }
                    Resp[39] = "76";
                    break;
                case "44": // GIP CREDIT INWD
                    decimal amtt = Convert.ToDecimal(DE[4]) / 100;
                    if (without.Substring(0, 4) == "0420")
                    {
                        
                        string rvs = Reversals(gettoken(), DE[11],amtt.ToString(),DE[7]);
                        if (rvs=="00")
                        {
                           // string typr = null;
                            string tkr = gettoken();
                            string balancer = Balancecheck(tkr, revaccount,DE[49]);
                            if (balancer == "14" || balancer == "01")
                            {
                                Resp[39] = "433";
                                goto Loro;
                              
                            }
                         
                            //  erred:
                            string bkkbalr = balancer;
                            // string flg = bkkbal.Length.ToString();
                            Resp[54] = bkkbalr;
                          
                            Resp[39] = "00";
                            goto Loro;
                           
                        }
                        if (rvs == "01")
                        {
                        Resp[39] = "00"; // should be 12
                        goto Loro;
                       
                        }
                        if (rvs == "07")
                        {
                            Resp[39] = "00"; // should be 13
                            goto Loro;
                        }
               
                    }
                    //if (DE[71]== "1000100967701" )
                    //{
                    //    Resp[39] = "39";
                    //    goto Loro;
                    //}
                    //if (DE[71] == "2000700242604")
                    //{
                    //    Resp[39] = "39";
                    //    goto Loro;
                    //}
                    string tres = transfaccounts(gettoken(), "85000001249936.1249", DE[103], DE[11], amtt.ToString(),DE[7]); // GIP Receiving|| DR GIP INWARDS CR CA
                    if ( tres== "00")
                    {
                        revaccount = DE[71];
                        //string typr = null;
                        string tkr = gettoken();
                        string balancer = Balancecheck(tkr, DE[71],DE[49]);
                        if (balancer == "14" || balancer == "01")
                        {
                            Resp[39] = "33";
                            goto Loro;
                         
                        }
                        // r41 = DE[41].PadRight(16, '|');
                        // r42 = DE[42].PadRight(15, '|');
                        // r43 = DE[43].PadRight(40, '|');

                        //Resp[41] = r41;
                        //Resp[42] = r42;
                        //Resp[43] = r43;

                        //  erred:
                        string bkkbalr = balancer;
                        // string flg = bkkbal.Length.ToString();
                        Resp[54] = bkkbalr;
                        Resp[72] = clientnm;
                        Resp[39] = "00";
                        goto Loro;
                     
                    }
                    if (tres=="01")
                    {
                        Resp[39] = "99";
                        goto Loro;
                    }
                    if (tres == "04")
                    {
                        Resp[39] = "76";
                    }
                    break;
                
                
                
                case "95":// GIP SPEEDY
                    //OeSocketClient oesocketclient2 = new OeSocketClient();

                    //////  Console.WriteLine("{0} {1}", DateTime.Now, sendit_som("233241112969", "We tried it"));
                    //////}
                    //if (oesocketclient2.runRequest("ISO8583_ATM|" + data.ToString()))
                    //    oeresponse = oesocketclient2.Response;
                    //return oeresponse;
                    //break;
                case "00":
                //  break;
                case "43":// GIP DEBIT 
                    Console.WriteLine("We have Message Type::" + without.Substring(0, 4));
                    Decimal amount = Convert.ToDecimal(DE[4]) / 100;
                    //if (without.Substring(0, 4)=="0200")
                    //{

                    //}
                    if (without.Substring(0, 4) == "0220" || without.Substring(0, 4) == "0200"|| without.Substring(0, 4) == "0210")
                    {
                        Console.WriteLine("Message Type::"+ without.Substring(0, 4));
                        //if (DE[3].Substring(0, 2) == "95")
                        //{
                        //    goto speedy;
                        //}
                        decimal transamt = Convert.ToDecimal(DE[4]) / 100;
                        Resp[61] = null;
                        Resp[125] = null;
                        Resp[39] = "00";
                        string ous = transfaccounts(gettoken(),DE[102], "85000001250936.1250", 
                            DE[11], transamt.ToString(), DE[7]); // GIP DR : DR CA CR GIP Out
                        //if (ous == "00")
                        //{
                        //    goto Loro;
                        //}
                        string rrBalancer = Balancecheck(gettoken(), DE[102], DE[49]);
                        //  erred:
                        string bkkbalr = rrBalancer;
                        Resp[54] = rrBalancer;
                        Resp[39] = ous;
                        goto Loro;
                    }

                speedy:

                    //}
                    //if (without.Substring(0, 4) == "0200")
                    //{

                    //}
                    if (without.Substring(0, 4) == "0422")
                    {
                       // Resp[39] = "00";

                       string rBalancer = Balancecheck(gettoken(),DE[102],DE[49]);
                                               //  erred:
                        string bkkbalr =rBalancer;
                        // string flg = bkkbal.Length.ToString();
                        Resp[61] = null;
                        Resp[125] = null;
                        Resp[54] = bkkbalr;
                        Resp[39] = "00";
                        goto Loro;
                    }
                    if (without.Substring(0, 4) == "0420" || without.Substring(0, 4) == "1420")
                    {
                        string revs = Reversals(gettoken(), DE[90].Substring(4, 6),(Convert.ToDecimal(DE[4]) / 100).ToString(), DE[7]);
                        if (revs == "00")
                        {
                          //  string typr = null;
                          if (without.Substring(0, 4) == "1420")
                            {
                                goto Loro;
                            }
                            string tkr = gettoken();
                            Console.WriteLine("The account Num:: {0}", revaccount);
                            string balancer = Balancecheck(tkr, revaccount,DE[49]);
                            if (balancer == "14" || balancer == "01")
                            {
                                Resp[39] = "133";
                                goto Loro;

                            }
                            // r41 = DE[41].PadRight(16, '|');
                            // r42 = DE[42].PadRight(15, '|');
                            // r43 = DE[43].PadRight(40, '|');

                            //Resp[41] = r41;
                            //Resp[42] = r42;
                            //Resp[43] = r43;
                            
                            //  erred:
                            string bkkbalr =balancer;
                            // string flg = bkkbal.Length.ToString();
                            Resp[54] = bkkbalr;
                            Resp[39] = "00";
                            goto Loro;

                        }
                        //if 
                        //Resp[39] = "12";
                        //goto Loro;
                        if (revs == "07")
                        {
                            Resp[39] = "00";
                            goto Loro;
                        }
                        if (revs == "01")
                        {
                            Resp[39] = "00";
                            goto Loro;
                        }
                        if (revs == "02")
                        {
                            Resp[39] = "51";
                            goto Loro;
                        }

                    }

                    //Console.WriteLine("Reference:: {0}", DE[37]);
                    //Console.WriteLine("Transcode:: {0}", DE[3].Substring(0,2));
                    string r = transfaccounts(gettoken(), 
                        accountnn,
                        getaccountnum(DE[41]),
                        DE[37],
                        (Convert.ToDecimal(DE[4]) / 100).ToString(),
                        "20200806");
                    if (r == "04")
                    {
                        Resp[39] = "56";
                        goto Loro;

                    }
                    if (r == "02")
                    {
                        Resp[39] = "51";
                        goto Loro;

                    }

                    // ------getresponse Statement too
                    string tyyp = null;
                    string fbalance = Balancecheck(gettoken(), accountnn,DE[49]);
                    Resp[72] = clientnm;
                    if (fbalance == "14" || fbalance == "01")
                    {
                        Resp[39] = "33";
                        goto Loro;
                    }
                    
                    string bkbal = fbalance;
                    // string fll = bkbal.Length.ToString();
                    Resp[39] = "00";
                    Resp[54] = bkbal;
                    //  Console.WriteLine("Balance::{0}", Resp[54]);
                    break;
                case "20":
                case "01":
                case "02":
                 
                    decimal amt = Convert.ToDecimal(DE[4]) / 100;
                    if (without.Substring(0, 4) == "0220")   // ECOM * remember to fix
                    {
                        if (DE[3].Substring(0, 2) == "95")
                        {
                            goto speedy;
                        }
                        decimal transamt = Convert.ToDecimal(DE[4]) / 100;
                        Resp[61] = null;
                        Resp[125] = null;
                      //  Resp[39] = "00";
                        string ous=transfaccounts(gettoken(), "85000001203936.1203", getaccountnum(DE[41]), DE[11], transamt.ToString(),"ATM WITHDRAWAL AT"+DE[41]);
                      //  string ous = transfaccounts(gettoken(), "85000001205936.1205", "2000200594501", DE[11], transamt.ToString(), DE[7]);
                        if (ous == "00")
                        {
                            Resp[39] = "00";
                            goto Loro;
                        }
                       Resp[39] = ous;
                        goto Loro;
                    }
                    //without.Substring(0, 4) == "0222"



                    if (without.Substring(0, 4) == "0420" || without.Substring(0, 4) == "0422")
                    {
                       // Console.WriteLine(without.Substring(0, 4));
                        string revs = Reversals(gettoken(), DE[90].Substring(4,6),amt.ToString(),DE[7]);
                        if ( revs== "00")
                        {
                            Resp[72] = null;
                           // string typr = null;
                            string tkr = gettoken();
                            Console.WriteLine("The account Num:: {0}", revaccount);
                            if (without.Substring(0, 4) == "0422")
                            {
                                Resp[39] = "00";
                                goto Loro;
                               
                            }
                            string balancer = Balancecheck(tkr, revaccount,DE[49]);
                            if (balancer == "14"||balancer=="01")
                            {
                                Resp[39] = "33";
                                goto Loro;
                              
                            }
                            // r41 = DE[41].PadRight(16, '|');
                            // r42 = DE[42].PadRight(15, '|');
                            // r43 = DE[43].PadRight(40, '|');

                            //Resp[41] = r41;
                            //Resp[42] = r42;
                            //Resp[43] = r43;
                            
                            //  erred:
                            string bkkbalr = balancer;
                            // string flg = bkkbal.Length.ToString();
                            Resp[54] = bkkbalr;
                            Resp[39] = "00";
                            goto Loro;
                          
                        }
                        //if 
                        //Resp[39] = "12";
                        //goto Loro;
                        //if (revs == "07")
                        //{
                        //    Resp[39] = "00";
                        //    goto Loro;
                        //}
                        //if (revs == "01")
                        //{
                        //    Resp[39] = "00";
                        //    goto Loro;
                        //}
                        if (revs == "02")
                        {
                            Resp[39] = "51";
                            goto Loro;
                        }
                        if (revs == "07")
                        {
                            Resp[39] = "33";
                            Resp[72] = "";
                            goto Loro;
                        }

                    }

                    //Console.WriteLine("Reference:: {0}", DE[37]);
                    //Console.WriteLine("Transcode:: {0}", DE[3].Substring(0,2));
                    string tr = transfaccounts(gettoken(), DE[102], getaccountnum(DE[41]), DE[11], amt.ToString(), DE[7]);
                    //string tr = cashwithdrawal(DE[102], 
                    //            DE[37].Substring(6,6)
                    //            ,amt.ToString(),DE[7]);             //transfaccounts(gettoken(), DE[102], DE[11], amt.ToString());
                    if (tr != "00")
                    {
                        Resp[39] = tr;
                        goto Loro;

                    }

                    // ------getresponse Statement too
                  //  string tyyp=null;
                    string fbbalance = Balancecheck(gettoken(), accountnn,DE[49]);
                    Resp[72] = clientnm; // Trouble code
                    if (fbbalance == "14"|| fbbalance=="01")
                    {
                        Resp[39] = "76";
                        goto Loro;
                    }
                    string bkkbal = fbbalance;
                    // string fll = bkbal.Length.ToString();
                    Resp[39] = "00";
                    Resp[54] =  bkkbal;
                  //  Console.WriteLine("Balance::{0}", Resp[54]);
                    break;
               case "30":
                    string typ=null;
                    string tk = gettoken();
                    string balance = Balancecheck(tk, DE[102],DE[49]);
                    if (balance == "14"||balance=="01")
                    {
                        Resp[39] = "76";
                            goto Loro;
                      
                    }
                    // r41 = DE[41].PadRight(16, '|');
                    // r42 = DE[42].PadRight(15, '|');
                    // r43 = DE[43].PadRight(40, '|');

                    //Resp[41] = r41;
                    //Resp[42] = r42;
                    //Resp[43] = r43;
                    
                  //  erred:
                    string bkkball = Balancecheck(tk,DE[102],DE[49]);
                    Resp[39] = "00";
                    // string flg = bkkbal.Length.ToString();
                    Resp[54] =  bkkball;
                   // Console.WriteLine("I made this ::{0}",Resp[54]); // Display 
                    break;
                default:
                    break;
            }
            Loro:
             int rmti = int.Parse(MTI) + 10;
            string srmti = rmti.ToString();
            
            string built= iSO8583.Build(Resp, srmti.PadLeft(4, '0'));
            string mlen = built.Length.ToString();
            loginf.Info($"Response:{mlen.PadLeft(4, '0') + built} ");
            // Console.WriteLine("{0}", built);
             return mlen.PadLeft(4,'0')+built;
            //return built;
       
        }
        
        static string creditaccount(string accountnum, string amount,string transactid,string tstamp)
        {
            return transfaccounts(gettoken(), "85000001247936.1247", accountnum, transactid, amount,tstamp);
        }
       static string gettoken()
        {
            // System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Uri u = new Uri(URL);
            ServicePointManager.ServerCertificateValidationCallback +=
         (sender, certificate, chain, sslPolicyErrors) => true;
            var client = new RestClient(URL+"/token");

            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "no auth");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            // ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            request.AddParameter("application/x-www-form-urlencoded", "grant_type=password&username=AD00101&password=Seekr3t!!!", ParameterType.RequestBody);
            try
            {
                IRestResponse response = client.Execute(request);

                var token = JsonConvert.DeserializeObject<tokenres>(response.Content);

                //    Console.WriteLine(token.access_token);
                if (token == null)
                {
                    return null;
                }
                return token.access_token.ToString();
            }
            catch
            {
                Console.WriteLine("Connection to CBS Cannot be established");
            }
            return "01";
            
        }
       static string Balancecheck(string token,string contract, string addit)
        {
            clientnm = string.Empty;
            if (token == null)
            {
                return "01";
            }
            ServicePointManager.ServerCertificateValidationCallback +=
        (sender, certificate, chain, sslPolicyErrors) => true;
            string bala = null;
            var client = new RestClient(URL+"/api/accounts/GetAccountBalance");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("content-length", "39");
            request.AddHeader("accept-encoding", "gzip, deflate");
          //  request.AddHeader("Host", "192.168.70.15:443");
            request.AddHeader("Postman-Token", "81717ea4-2db3-4778-8af1-8d78803b3f78,d3df76e4-0850-45d6-b004-4a9e9af4479c");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("User-Agent", "PostmanRuntime/7.11.0");
            request.AddHeader("Authorization", "Bearer "+token);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\n\t\"ContractNumber\":\""+contract+"\"\n\t\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
           // Console.WriteLine(response.Content.ToString());
            var balance = JsonConvert.DeserializeObject<respo>(response.Content);
            // var rootObject = JsonConvert.DeserializeObject<Datainfo>(balance.data);
         //   Console.WriteLine("Response from CBS:: {0}", balance.responsecode);
            if (balance.responsecode != "200")
            {
                return "14";
            }
            foreach(var bals in balance.data)
            {
               bala=bals.balance.ToString().Replace(",","");
                clientnm = bals.clientName.ToString();
            }
            //if (bala.Substring(bala.Length - 3,1) != ".")
            //{
            //    bala += ".00";
            //}
            if (bala.IndexOf('.') < 0)
            {
                bala += ".00";
            }
            string typ = null;
            string i = contract.Substring(0, 1);
            if (i == "1")
            {
                typ = "10"; //Change back to 20
            }
            else
            {
                typ = "20"; //Change back to 10
            }
            string sign = "C";
            if (Convert.ToSingle(bala) < 1)
            {
                sign = "D";
            }
            string jjbala = bala.Replace(".", "");
           string jbala = jjbala.Replace("$", "");
            string isobal = typ + "01" +addit + sign
                        + jbala.PadLeft(12, '0') +
                        typ + "02" +addit +
                        sign + jbala.PadLeft(12, '0');
            return isobal;
            // Console.WriteLine(balance.data.bala)
        }
       static string Reversals(string token, string exttrid, string amt,string tstamp)
        {
            createtable();
            Console.WriteLine("You sent this:: {0}", exttrid);
            DataRow[] foundrow = localstorage.Select("ExtRef='" + exttrid + "'");
            //Console.WriteLine("")
            String refid = null;
            if (foundrow.Length == 0)
            {
                return string.Format("01");
            }
            foreach (DataRow dr in foundrow)
            {
                refid = dr[1].ToString();
                Console.WriteLine("ReferenceId::{0}", refid);
                revaccount = dr[2].ToString();
                revamt = dr[3].ToString();
                revtime = dr[4].ToString();
                dr.Delete();
                localstorage.AcceptChanges();
                break;
            }
         
            Console.WriteLine("ReferenceId::{0}", refid);
             //externalid =
            var client = new RestClient(URL+"/api/transactions/CancelTransaction");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("content-length", "161");
            request.AddHeader("accept-encoding", "gzip, deflate");
         
            request.AddHeader("Postman-Token", "67b01720-e042-48b1-b16c-999068a98827,543635dd-7e8c-481a-90f0-9eafa9ace288");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("User-Agent", "PostmanRuntime/7.15.0");
            request.AddHeader("Authorization","Bearer "+token);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\"reference\":\""+refid+"\",\"externalReference\":\""+exttrid+"\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            var rst = JsonConvert.DeserializeObject<trobj>(response.Content);
            //if (rst.responsecode == "200")
            //{
            //    return string.Format("00");
            //}
            Console.WriteLine("We got this :: {0}", rst.responsecode);
            switch (rst.responsecode)
            {
                case "200":
                    return string.Format("00");
                case "207":
                    return string.Format("01");
                default:
                    return string.Format("-1");
            }
           // return string.Format("01");
        }
       static string getStatement(string token, string contract,string count)
        {
            if (token == null)
            {
                return "01";
            }
            ServicePointManager.ServerCertificateValidationCallback +=
        (sender, certificate, chain, sslPolicyErrors) => true;
            var client = new RestClient(URL+"/api/accounts/GetContractStatement");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("content-length", "162");
            request.AddHeader("accept-encoding", "gzip, deflate");
          //  request.AddHeader("Host", "192.168.70.15:443");
            request.AddHeader("Postman-Token", "47617873-b977-46cb-8fed-e12e23d164ce,3848c4d2-3b99-42bf-9b6b-a05deb8153ac");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("User-Agent", "PostmanRuntime/7.13.0");
            request.AddHeader("Authorization", "Bearer "+token);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\n\t\"ContractNumber\":\""+contract+"\",\n\t\"StatementRequestType\":\"1\",\n\t\"TransactionsCount\":\"9\",\n\t\"StartDate\":\""+DateTime.Now.ToString()+"\",\n\t\"EndDate\":\""+ DateTime.Now.ToString() + "\"\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
         
            string staters = null;
            stobj st = JsonConvert.DeserializeObject<stobj>(response.Content);
            if (st.responsecode != "200")
            {
                return "01";
            }
            int norecs = 0;
            foreach (Statement sth in st.data)
            {
               // string[] statm = null;
                string qy = sth.valueDate.ToString();
                //  Console.WriteLine(qy);

                string desc = sth.description;

                string amt = sth.amount;
                // statm[i] += 
                staters += qy.Substring(0, 10) + " "+desc.Substring(0, 8) +
                    " "+ sth.transactiontype + "R" +
                    " "+ amt.PadLeft(12, '0').Replace(".",""); //statm[i].Substring(0, 34);
                norecs += 1;
            }

            string ibal = (Balancecheck(token, contract,"288"));

            //string balstr = null;
            //if (ibal <1)
            //{
            //    balstr = "DR ";
            //}
            //else
            //{
            //    balstr = "CR ";
            //}


            string complet = "0"+norecs.ToString()+staters;// + "AVAILABLE BALANCE    " + ibal;//balstr+ibal.ToString().Replace(".","").PadLeft(12,'0');
            return complet;
        }
        static void savetrxnlogs(string externalReference, string referenceId, string exacct, string amount)
        {
            var loginf = NLog.LogManager.GetCurrentClassLogger();
            loginf.Info($"Logging Transactions:: ");
          
            using (SqlConnection connection = new SqlConnection(CON_STR))
            //using (SqlConnection connection = new SqlConnection(dbpath))
            using (SqlCommand commnd = new SqlCommand("atm_addtrxn", connection))
            {
                commnd.CommandType = CommandType.StoredProcedure;
                // commnd.Parameters.Add("@accountnum", SqlDbType.VarChar, 50);
                commnd.Parameters.AddWithValue("@Exter", externalReference);
                //commnd.Parameters["@accountnum"].Value = actno;
                //  commnd.Parameters.Add("@transcode", SqlDbType.VarChar, 20);
                commnd.Parameters.AddWithValue("@Regist", referenceId);
                //  commnd.Parameters["@transcode"].Value= proc;
                //commnd.Parameters.Add("@value", SqlDbType.Decimal);
                commnd.Parameters.AddWithValue("@AccountNum", exacct);
                //commnd.Parameters["@value"].Value = Convert.ToDecimal(valu);
                //commnd.Parameters.Add("@_userid", SqlDbType.VarChar, 20);
                commnd.Parameters.AddWithValue("@Amount", amount);
                //commnd.Parameters["@_userid"] .Value = user;
                ////commnd.Parameters.Add("@location", SqlDbType.VarChar, 256);
                //commnd.Parameters.AddWithValue("@location", locate);
                //commnd.Parameters.AddWithValue("fname", fullname);
                //    commnd.Parameters.AddWithValue("@_date", DateTime.Now);
                // commnd.Parameters["@location"].Value = locate;
                try
                {
                    connection.Open();
                    commnd.ExecuteNonQuery();
                    
                    return; // string.Format("00|Success");

                }
                catch (Exception km)
                {
                    Console.WriteLine($"Internal Server Error::{km.Message}");
                    // return string.Format("01|Fatal Error: {0}", km.Message);
                    loginf.Info($"Internal Server Error:: {km.Message}");
                }


            }
            //return string.Format("01|Couldn't submit transaction");

            //using (SQLiteCommand commnd = new SQLiteCommand("Insert into Transactions(ExtRef,RegId,AccountNum,Amount,Stamp) Values('"+externalReference+"','"
            //    +referenceId+"','"+exacct+"','"+amount+"',datetime('now'))", connection))
            //{

            //    try
            //    {
            //        connection.Open();
            //        // SQLiteDataAdapter adptbio = new SQLiteDataAdapter(commnd);
            //        commnd.ExecuteNonQuery();
            //        // adptbio.Fill(clientsbiodata);
            //        //while (reader.Read())
            //        //{
            //        //    //custids.Add(Convert.ToInt32(reader["clientid"]));
            //        //    //fingerlist.Add(Fmd.DeserializeXml(Convert.ToString(reader["fingXml"])));
            //        //    string act2debit = null;
            //        //    act2debit = reader["Accountnum"].ToString();
            //        //    return act2debit;
            //        //}
            //        // commnd.ExecuteNonQuery();
            //        commnd.Dispose();
            //        connection.Close();


            //    }
            //    catch (Exception km)
            //    {
            //        connection.Close();
            //        Console.WriteLine("Internal Error:");
            //        //MessageBox.Show(km.Message);

            //    }
            //   // return "0000000000.0000";

            //}
        }
      static string transfaccounts(string token,string frmcontract,string tocontract, string transid,string amount,string stamp) // Deposit
        {

            if (frmcontract.Length < 10)
            {
                return "76";
            }
            string exacct = frmcontract;
            if (frmcontract.IndexOf(".")!=-1)
            {
                exacct = tocontract;
            }
            if (token == null)
            {
                return "05";
            }
            ServicePointManager.ServerCertificateValidationCallback +=
        (sender, certificate, chain, sslPolicyErrors) => true;
            var client = new RestClient(URL+"/api/transactions/FundTransferIntraBank");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("content-length", "162");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("Postman-Token", "4e1afef7-dcfb-4613-9443-f0b774fb434c,52289e70-c6e4-4b0b-9bbb-3931d74c0600");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("User-Agent", "PostmanRuntime/7.13.0");
            request.AddHeader("Authorization", "Bearer "+token);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\"OrderingContractNumber\":\""+frmcontract+ "\",\r\n\"BeneficiaryContractNumber\":\""+tocontract+"\",\r\n\"Amount\":" + amount+",\"ExternalReference\":\""+transid+"\" , \"Naration\":\""+stamp+"\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            var trs = JsonConvert.DeserializeObject<trobj>(response.Content);
            switch (trs.responsecode)
            {
                case "200":
                    foreach (var ty in trs.data)
                    {
                        //var row = localstorage.NewRow();
                        ////row["ExtRef"] = ty.externalReference;
                        ////row["RefId"] = ty.referenceId;
                        //localstorage.Rows.Add(row);
                       
                       localstorage.Rows.Add(ty.externalReference, ty.referenceId,exacct,amount,DateTime.Now);
                       savetrxnlogs(ty.externalReference, ty.referenceId, exacct, amount);
                       // createtable();
                        Console.WriteLine("We got this Ext::{0} Ref::{1}::{2}", ty.externalReference, ty.referenceId, exacct);
                    }

                    string resp = "00";
                    return resp;
                   
                case "207":
                    return string.Format("01");
                case "205":
                    return string.Format("51");
                case "201":
                    return string.Format("05");
                case "204":
                    return string.Format("76");
                case "206":
                    return string.Format("51");
            }
            return string.Format("01");

        }
   //     static string reversal(string token, string transid)
       public static void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the 
            // host running the application.
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress,cport); //8500 IN TEST 9500 IN PRODUCTION
            Console.WriteLine($"Current ip :: {localEndPoint.Address}");
            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            var loginf = NLog.LogManager.GetCurrentClassLogger();

            // Bind the socket to the local endpoint and listen for incoming connections.
            //started:
            try
            {

                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    // Set the event to nonsignaled state.
                    //allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("{0} Started...", DateTime.Now);
                    Console.WriteLine("Waiting for a connection...");
                    Socket handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed.
                    //while (true)
                    //{
                    
                    loginf.Info($"New Connection from :{handler.RemoteEndPoint} ");

                    bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    // Wait until a connection is made before continuing.
                    // allDone.WaitOne();
                    string hav = getresponse(data);
                    byte[] msg = Encoding.ASCII.GetBytes(hav);//.Replace("|"," "));

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                  //  int bytesSent = handler.EndSend(ar);
                  //  Console.WriteLine("Sent {0} bytes to client.", bytesSent);
                    handler.Close();

                }

            }
            catch (Exception e)
            {
                //      //throw;
                loginf.Info($"Unknown Exception:: {e.Message}");
                //throw;

            }
            Console.WriteLine("\nSomething Happened...");
            //goto started;
            //Console.ReadKey();
        }

        static string cashwithdrawal(string accountnum, string transid,string amount,string cstamp)
        {
            return transfaccounts(gettoken(), accountnum, "85000001247936.1247", transid, amount,cstamp);
        }
        static string name_enquiry(string token, string contract)
        {
            if (token == null)
            {
                return "01";
            }
            ServicePointManager.ServerCertificateValidationCallback +=
        (sender, certificate, chain, sslPolicyErrors) => true;
            string fullname = null;
            var client = new RestClient(URL+"/api/accounts/GetAccountBalance");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("content-length", "39");
            request.AddHeader("accept-encoding", "gzip, deflate");
          
            request.AddHeader("Postman-Token", "81717ea4-2db3-4778-8af1-8d78803b3f78,d3df76e4-0850-45d6-b004-4a9e9af4479c");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("User-Agent", "PostmanRuntime/7.11.0");
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\n\t\"ContractNumber\":\"" + contract + "\"\n\t\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            // Console.WriteLine(response.Content.ToString());
            var balance = JsonConvert.DeserializeObject<respo>(response.Content);
            // var rootObject = JsonConvert.DeserializeObject<Datainfo>(balance.data);
            //   Console.WriteLine("Response from CBS:: {0}", balance.responsecode);
            if (balance.responsecode != "200")
            {
                return "14";
            }
            //foreach (var bals in balance.data)
            //{
            //    bala = bals.balance.ToString().Replace(",", "");
            //    clientnm = bals.clientName.ToString();
            //}
            foreach (var nam in balance.data)
            {
                fullname = (nam.clientName.ToString());

            }
            return fullname;
        }
    }
}
