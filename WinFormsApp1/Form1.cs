﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using MailKit.Net.Imap;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        System.Threading.Thread time;
        public Form1()
        {
            
            InitializeComponent();
            
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }

        public void DoThisAllTheTime()
        {
            try
                {
                using (var client = new ImapClient())
                {

                    client.Connect("imap.yandex.ru", 993, true);

                    client.Authenticate("admin@bobolala.xyz", "ecstipxneiopwyvx");
                    Console.WriteLine(DateTime.Now);
                    while (true)
                    {
                        Console.WriteLine(DateTime.Now);
                        string fbcode;
                        string json = "";
                        HttpWebRequest webRequest;
                        // Get the first personal namespace and list the toplevel folders under it.
                        var personal = client.GetFolder(client.PersonalNamespaces[0]);
                        foreach (var folder in personal.GetSubfolders(false))
                        {
                            Console.WriteLine("Dang tien hanh truy cap vao folder --> {0}", folder.Name);
                            if (folder.Name == "Trash")
                            {
                                folder.Open(FolderAccess.ReadWrite);

                                for (int i = 0; i < folder.Count; i++)
                                {
                                    var message = folder.GetMessage(i);
                                    Console.WriteLine("Dang tien hanh truy cap vao folder {0} --> {1}/{2}", folder.Name,i ,folder.Count);
                                    folder.Store(i, new StoreFlagsRequest(StoreAction.Add, MessageFlags.Deleted) { Silent = true });
                                }
                                folder.Expunge();
                            }
                            
                            else if (folder.Name == "INBOX" || folder.Name == "Spam")
                            {
                                folder.Open(FolderAccess.ReadWrite);
                                Console.WriteLine("Dang tien hanh truy cap vao folder {0}", folder.Name);
                                for (int i = 0; i < folder.Count; i++)
                                {
                                    var message = folder.GetMessage(i);
                                    if (message.TextBody.Contains("FB-"))
                                    {
                                        string regexcodeFB = @"(FB-\d+)";
                                        Regex rg = new Regex(regexcodeFB);
                                        MatchCollection matchedAuthors = rg.Matches(message.TextBody);
                                        // Print all matched authors  
                                        for (int count = 0; count < matchedAuthors.Count; count++)
                                        {
                                            fbcode = matchedAuthors[count].Value;
                                            string code = fbcode.Substring(3);

                                            Product product = new Product();
                                            product.email = message.To.ToString();
                                            product.code = code;
                                            product.domain = "FB";

                                            json = JsonConvert.SerializeObject(product);

                                            webRequest = (HttpWebRequest)WebRequest.Create("https://api9.autofarmer.net/public-api/v1/add-mail");

                                            webRequest.Method = "POST";
                                            webRequest.ContentType = "application/json";

                                            byte[] byteArray = Encoding.UTF8.GetBytes(json);
                                            webRequest.ContentLength = byteArray.Length;
                                            using (Stream requestStream = webRequest.GetRequestStream())
                                            {
                                                requestStream.Write(byteArray, 0, byteArray.Length);
                                            }

                                            // Get the response.
                                            using (WebResponse response = webRequest.GetResponse())
                                            {
                                                using (Stream responseStream = response.GetResponseStream())
                                                {
                                                    StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                                                    string Json = rdr.ReadToEnd(); // response from server
                                                    Console.WriteLine(json);
                                                }
                                            }
                                        }
                                            
                                    }
                                    else if(message.TextBody.Contains("Bạn có thể được yêu cầu nhập mã xác nhận sau")|| message.TextBody.Contains("You may be asked to enter this confirmation code"))
                                    {
                                        string regexcodeFB = @"(: \d+)(.)";
                                        Regex rg = new Regex(regexcodeFB);
                                        MatchCollection matchedAuthors = rg.Matches(message.TextBody);
                                        // Print all matched authors  

                                        if(matchedAuthors.Count !=0)
                                        {
                                            for (int count = 0; count < matchedAuthors.Count; count++)
                                            {
                                                fbcode = matchedAuthors[count].Value.ToString();
                                                string code = getBetween(fbcode, " ", ".");

                                                Product product = new Product();
                                                product.email = message.To.ToString();
                                                product.code = code;
                                                product.domain = "FB";

                                                json = JsonConvert.SerializeObject(product);

                                                webRequest = (HttpWebRequest)WebRequest.Create("https://api9.autofarmer.net/public-api/v1/add-mail");

                                                webRequest.Method = "POST";
                                                webRequest.ContentType = "application/json";

                                                byte[] byteArray = Encoding.UTF8.GetBytes(json);
                                                webRequest.ContentLength = byteArray.Length;
                                                using (Stream requestStream = webRequest.GetRequestStream())
                                                {
                                                    requestStream.Write(byteArray, 0, byteArray.Length);
                                                }

                                                // Get the response.
                                                using (WebResponse response = webRequest.GetResponse())
                                                {
                                                    using (Stream responseStream = response.GetResponseStream())
                                                    {
                                                        StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                                                        string Json = rdr.ReadToEnd(); // response from server
                                                        Console.WriteLine(json);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            fbcode = message.Subject;
                                            string code = fbcode.Substring(0, 6);

                                            Product product = new Product();
                                            product.email = message.To.ToString();
                                            product.code = code;
                                            product.domain = "FB";

                                            json = JsonConvert.SerializeObject(product);

                                            webRequest = (HttpWebRequest)WebRequest.Create("https://api9.autofarmer.net/public-api/v1/add-mail");

                                            webRequest.Method = "POST";
                                            webRequest.ContentType = "application/json";

                                            byte[] byteArray = Encoding.UTF8.GetBytes(json);
                                            webRequest.ContentLength = byteArray.Length;
                                            using (Stream requestStream = webRequest.GetRequestStream())
                                            {
                                                requestStream.Write(byteArray, 0, byteArray.Length);
                                            }

                                            // Get the response.
                                            using (WebResponse response = webRequest.GetResponse())
                                            {
                                                using (Stream responseStream = response.GetResponseStream())
                                                {
                                                    StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                                                    string Json = rdr.ReadToEnd(); // response from server
                                                    Console.WriteLine(json);
                                                }
                                            }
                                        }

                                        
                                            
                                    }
                                    folder.Store(i, new StoreFlagsRequest(StoreAction.Add, MessageFlags.Deleted) { Silent = true });

                                }
                                folder.Expunge();
                            }
                        }
                        // The Inbox folder is always available on all IMAP servers...
                        var inbox = client.Inbox;
                        inbox.Open(FolderAccess.ReadWrite);

                        Console.WriteLine("Total messages: {0}", inbox.Count);
                        Console.WriteLine("Recent messages: {0}", inbox.Recent);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                DoThisAllTheTime();
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form form = (Form)sender;
            form.ShowInTaskbar = false;
            form.Opacity = 0;
            time = new System.Threading.Thread(DoThisAllTheTime);
            time.Start();
        }
    }
}