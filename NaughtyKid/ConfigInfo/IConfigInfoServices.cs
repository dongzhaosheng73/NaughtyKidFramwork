using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaughtyKid.Model;

namespace NaughtyKid.ConfigInfo
{
    public interface IConfigInfoServices
    {
        string SettingPath { set; get; }

        IConfigInfo ReadXmlConfigInfos<T>() where T : IConfigInfo, new();

        bool WriteXmlConfigInfos(IConfigInfo obj);

        IConfigInfo ReadIniConfigInfos<T>() where T : IConfigInfo, new();

        bool WriteIniConfigInfos(IConfigInfo obj);

        bool WriteIniConfigInfos(string setting, string name, string value);
    }
}
