using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.Ini;

namespace USerialEditor
{
    public class ConfigManager
    {
        IniConfigurationProvider configurationProvider;
        string configPath;
        public ConfigManager(string configPath)
        {
            this.configPath = configPath;

        }


    }
}
