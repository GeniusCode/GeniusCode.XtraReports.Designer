using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace GeniusCode.XtraReports.Designer.Support
{
    public class RegistryHelper : IRootPathAcquirer
    {
        private readonly string _rootKey;

        public RegistryHelper(string rootKey)
        {
            if (rootKey == null) throw new ArgumentNullException("rootKey");
            _rootKey = rootKey;
        }

        public string AcquireRootPath(string defaultValue)
        {
            if (!Registry.CurrentUser.GetSubKeyNames().Any(n => n == _rootKey))
                Registry.CurrentUser.CreateSubKey(_rootKey);

            RegistryKey myKey = Registry.CurrentUser.OpenSubKey(_rootKey, true);

            const string pathValueName = "ProjectRootPath";

            if (!myKey.GetValueNames().Any(n => n == pathValueName))
                myKey.SetValue(pathValueName, defaultValue, RegistryValueKind.String);

            return (string)myKey.GetValue(pathValueName);
        }
    }
}
