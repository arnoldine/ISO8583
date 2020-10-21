// Decompiled with JetBrains decompiler
// Type: OASL_Listener_Mt.ConfigOpt
// Assembly: OASL_Listener_Mt, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26FE91B2-78D0-4A06-8BE1-48022BE25373
// Assembly location: C:\ADSL\Oasl_Listener_Mt\New folder\OASL_Listener_Mt.exe

using System;
using System.Data;
using System.IO;
using System.Xml;
namespace ISO8583_Test
{
  public class ConfigOpt
  {
    private static DataSet DSoptions;
    private static string mConfigFileName;

    public static string ConfigFileName
    {
      get
      {
        return ConfigOpt.mConfigFileName;
      }
    }

    public static void Initialize(string ConfigFile)
    {
      ConfigOpt.mConfigFileName = ConfigFile;
      ConfigOpt.DSoptions = new DataSet(nameof (ConfigOpt));
            if (File.Exists(ConfigFile))
            {
                int num = (int)ConfigOpt.DSoptions.ReadXml(ConfigFile);
            }
            else
                ConfigOpt.DSoptions.Tables.Add(new DataTable("ConfigValues")
                {
                    Columns = {
            {
              "OptionName",
              Type.GetType("System.String")
            },
            {
              "OptionValue",
              Type.GetType("System.String")
            }
          }
                });
    }

    public static void Store()
    {
      ConfigOpt.Store(ConfigOpt.mConfigFileName);
    }

    public static void Store(string ConfigFile)
    {
      ConfigOpt.mConfigFileName = ConfigFile;
      ConfigOpt.DSoptions.WriteXml(ConfigFile);
    }

    public static string GetOption(string OptionName)
    {
      DataView defaultView = ConfigOpt.DSoptions.Tables["ConfigValues"].DefaultView;
      defaultView.RowFilter = "OptionName='" + OptionName + "'";
      if (defaultView.Count > 0)
        return defaultView[0]["OptionValue"].ToString();
      return "";
    }

    public static void SetOption(string OptionName, string OptionValue)
    {
      DataView defaultView = ConfigOpt.DSoptions.Tables["ConfigValues"].DefaultView;
      defaultView.RowFilter = "OptionName='" + OptionName + "'";
      if (defaultView.Count > 0)
      {
        defaultView[0][nameof (OptionValue)] = (object) OptionValue;
      }
      else
      {
        DataRow row = ConfigOpt.DSoptions.Tables["ConfigValues"].NewRow();
        row[nameof (OptionName)] = (object) OptionName;
        row[nameof (OptionValue)] = (object) OptionValue;
        ConfigOpt.DSoptions.Tables["ConfigValues"].Rows.Add(row);
      }
    }
  }
}
