using System;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Permissions;
using System.Windows.Forms;

namespace SiteTester
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class SiteTester
    {
        private FileStream fs;
        SiteTester(string pathToLog)
        {
            fs = new FileStream(pathToLog, FileMode.Create);
        }
        public void Log(string site, int status)
        {
            byte[] info = new System.Text.UTF8Encoding(true).GetBytes(site + "," + status + "\n");
            Console.Write(Encoding.Default.GetString(info).Replace(',', '\t'));
            fs.Write(info, 0, info.Length);
        }


        ~SiteTester()
        {
            fs.Close();
        }


        public void Browse(string address)
        {
            WebBrowser wb = new WebBrowser();
            wb.Navigate("http://www.macys.com");
            
    }
        public void Connect(string address)
        {
            HttpWebResponse myHttpWebResponse = null;
            try
            {
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(address);
                myHttpWebRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36";
                myHttpWebRequest.ContentType = @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                myHttpWebRequest.UseDefaultCredentials = true;
                myHttpWebRequest.AllowAutoRedirect = true;
                myHttpWebRequest.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                myHttpWebRequest.KeepAlive = true;
                myHttpWebRequest.Timeout = 5000;
                myHttpWebRequest.Method = "GET";
                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                this.Log(address, (int)myHttpWebResponse.StatusCode);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse resp = ex.Response as HttpWebResponse;
                    if (resp != null)
                        this.Log(address, (int)resp.StatusCode);
                }
                else
                    Console.WriteLine(ex.Message);
            }
            finally
            {
                if (myHttpWebResponse != null)
                    myHttpWebResponse.Close();
            }
        }
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("SiteTester.exe <input path to the file with websites> <output path and file to write results to>");
            }
            else
            {
                string inputSite = args[0];
          
                StreamReader fr = new StreamReader(inputSite);
                SiteTester st = new SiteTester(args[1].ToString());
                string site = string.Empty;
                try
                {
                    while ((site = fr.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(site) || site == " ")
                        {
                            continue;
                        }

                        site = "http://www." + site;

                        st.Connect(site);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    fr.Close();
                }
            }
        }
    }
}
