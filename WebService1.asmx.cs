using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Configuration;
using System.Web.Hosting;

namespace WebApplication1
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        public static Trie.Trie dict = new Trie.Trie();
        private System.Diagnostics.PerformanceCounter ramCounter;
        private static string path = System.IO.Path.GetTempFileName();

        [WebMethod]
        public string DownloadWiki()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("titles");
            container.CreateIfNotExists();


            if (container.Exists())
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference("wikiDumpNew.txt");
                try
                {
                    using (var fileStream = File.OpenWrite(path))
                    {
                        blockBlob.DownloadToStream(fileStream);
                        Debug.WriteLine("Download Complete");
                        fileStream.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    Debug.WriteLine("Download Failed...");
                    return "FAIL: File was not downloaded";
                }
            }
            else
            {
                Debug.WriteLine("No Container Found");
                return "FAIL: No blob container found";

            }
            return "Success!";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string Search(string input)
        {
            List<String> result = dict.Search(input.ToLower());
            return new JavaScriptSerializer().Serialize(result.ToArray());
        }

        [WebMethod]
        public String BuildTree()
        {

            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string cur;
                    Double availableRam = ramCounter.NextValue();
                    while (((cur = sr.ReadLine()) != null) && (availableRam > 40))
                    {
                        dict.Add(cur.ToLower());
                        availableRam = ramCounter.NextValue();
                    }
                    Debug.WriteLine("Tree has maxed out its RAM capacity...");
                    return "Memory has been maxed out to its capacity";
                }
            }
            catch (IOException)
            {
                Debug.WriteLine("File could be used by another process");
                return "Failed...";
            }
            
        }
    }
}
