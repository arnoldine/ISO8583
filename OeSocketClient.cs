// Decompiled with JetBrains decompiler
// Type: OASL_Listener_Mt.OeSocketClient
// Assembly: OASL_Listener_Mt, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26FE91B2-78D0-4A06-8BE1-48022BE25373
// Assembly location: C:\ADSL\Oasl_Listener_Mt\New folder\OASL_Listener_Mt.exe

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ISO8583_Test
{
  internal class OeSocketClient
  {
    private char DELIM = Convert.ToChar(1);
    private bool connected = false;
    private string hostname = "";
    private int port = 0;
    private string servername = "";
    private string database = "";
    private string username = "";
    private string password = "";
    private string procedurename = "";
    private string response = "";
    private const int portNum = 8088;
    private const string CLOGIN = "1";
    private const string CLOGOFF = "2";
    private const string CCALL = "3";
    private const string CQuery = "-1";
    private const string CResetGentle = "-2";
    private const string CResetForce = "-3";
    private const int PREFIXLEN = 8;
    private const string PREFIXFMT = "00000000";
    private const string hostname_default = "localhost";
    private const int port_default = 8088;
    private const string servername_default = "";
    private const string database_default = "BANKING";
    private const string username_default = "BANKING";
    private const string password_default = "SYSTEMADMIN123";
    private const string procedurename_default = "ASCII_TRANSACT";
    private TcpClient tcpClient;
    private NetworkStream networkStream;

    public string Response
    {
      get
      {
        return this.response;
      }
    }

    public string Hostname
    {
      get
      {
        return this.hostname;
      }
      set
      {
        this.hostname = value;
        if (!value.Equals(""))
          return;
        this.hostname = "localhost";
      }
    }

    public int Port
    {
      get
      {
        return this.port;
      }
      set
      {
        this.port = value;
        if (value != 0)
          return;
        this.port = 8088;
      }
    }

    public string Servername
    {
      get
      {
        return this.servername;
      }
      set
      {
        this.servername = value;
        if (!value.Equals(""))
          return;
        this.servername = "";
      }
    }

    public string Database
    {
      get
      {
        return this.database;
      }
      set
      {
        this.database = value;
        if (!value.Equals(""))
          return;
        this.database = "BANKING";
      }
    }

    public string Username
    {
      get
      {
        return this.username;
      }
      set
      {
        this.username = value;
        if (!value.Equals(""))
          return;
        this.username = "BANKING";
      }
    }

    public string Password
    {
      get
      {
        return this.password;
      }
      set
      {
        this.password = value;
        if (!value.Equals(""))
          return;
        this.password = "1234567";
      }
    }

    public string Procedurename
    {
      get
      {
        return this.procedurename;
      }
      set
      {
        this.procedurename = value;
        if (!value.Equals(""))
          return;
        this.procedurename = "ASCII_TRANSACT";
      }
    }

    public int loadProfile()
    {
      this.Hostname = ConfigOpt.GetOption("OEServerIp");
      this.Port = Convert.ToInt32(ConfigOpt.GetOption("OEServerPort").ToString());
      this.Database = ConfigOpt.GetOption("DatabaseName");
      this.Username = ConfigOpt.GetOption("UserName");
      this.Password = ConfigOpt.GetOption("UserPassword");
      this.Procedurename = ConfigOpt.GetOption("OiFunction");
      Console.WriteLine("Hostname = {0}", (object) this.Hostname);
      Console.WriteLine("Port = {0}", (object) this.Port);
      Console.WriteLine("OiFunction = {0}", (object) this.Procedurename);
      return 0;
    }

    public int resetProfile()
    {
      this.hostname = "LOCALHOST";
      this.port = 8088;
      this.servername = "";
      this.database = "BANKING";
      this.username = "BANKING";
      this.password = "1234567";
      this.procedurename = "ASCII_TRANSACT";
      return 0;
    }

    private bool connect()
    {
      if (this.connected)
        this.disconnect();
      this.loadProfile();
      this.tcpClient = new TcpClient();
      try
      {
        Console.WriteLine("{0} Connecting to Hostname = {1}",DateTime.Now, (object) this.Hostname);
        this.tcpClient.Connect(this.hostname, this.port);
        this.networkStream = this.tcpClient.GetStream();
        if (this.networkStream.CanWrite && this.networkStream.CanRead)
          this.connected = true;
        else if (!this.networkStream.CanRead)
        {
          Console.WriteLine("You can not write data to this stream");
          this.tcpClient.Close();
        }
        else if (!this.networkStream.CanWrite)
        {
          Console.WriteLine("You can not read data from this stream");
          this.tcpClient.Close();
        }
      }
      catch (SocketException ex)
      {
        Console.WriteLine("Server not available! - {0}", (object) ex.Message);
      }
      catch (IOException ex)
      {
        Console.WriteLine("Sever not available!");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      return this.connected;
    }

    public void disconnect()
    {
      this.networkStream.Close();
      this.tcpClient.Close();
    }

    public bool sendMessage(string messageIn, ref string messageOut)
    {
      string s = messageIn.Length.ToString("00000000") + messageIn;
      messageOut = "";
      if (!this.connected)
        this.connect();
      bool flag;
      try
      {
        byte[] bytes = Encoding.ASCII.GetBytes(s);
        this.networkStream.Write(bytes, 0, bytes.Length);
        byte[] numArray1 = new byte[8];
        int count1 = this.networkStream.Read(numArray1, 0, 8);
        string str1 = Encoding.ASCII.GetString(numArray1, 0, count1);
        int val1 = 0;
        if (!str1.Equals(""))
          val1 = Convert.ToInt32(str1);
        int count2 = Math.Min(val1, this.tcpClient.ReceiveBufferSize);
        byte[] numArray2 = new byte[count2];
        int num = 0;
        StringBuilder stringBuilder = new StringBuilder();
        while (num < val1)
        {
          int count3 = this.networkStream.Read(numArray2, 0, count2);
          num += count3;
          string str2 = Encoding.UTF8.GetString(numArray2, 0, count3);
          stringBuilder.Append(str2);
        }
        flag = stringBuilder.ToString(0, 1) == "0";
        messageOut = !flag ? "bad response" : stringBuilder.ToString(2, val1 - 3);
      }
      catch (Exception ex)
      {
        messageOut = "OpenInsight Communication Error " + ex.ToString();
        flag = false;
      }
      return flag;
    }

    public bool logon()
    {
      string messageIn = "1" + (object) this.DELIM + this.username + (object) this.DELIM + this.password + (object) this.DELIM + this.database + (object) this.DELIM + "2" + (object) this.DELIM + this.servername + (object) this.DELIM + this.procedurename + (object) this.DELIM;
      string messageOut = "";
      return this.sendMessage(messageIn, ref messageOut);
    }

    public bool logoff()
    {
      string messageIn = "2" + (object) this.DELIM;
      string messageOut = "";
      return this.sendMessage(messageIn, ref messageOut);
    }

    public bool runRequest(string request)
    {
        //    this.procedurename = procname;
      string base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(request));
      this.response = "";
      bool flag = this.connect();
      if (flag)
        flag = this.logon();
      if (flag)
      {
        string messageIn = "3" + (object) this.DELIM + request + (object) this.DELIM;
        string messageOut = "";
        flag = this.sendMessage(messageIn, ref messageOut);
        if (flag)
          this.response = messageOut;
      }
      this.logoff();
      this.disconnect();
      return flag;
    }
  }
}
