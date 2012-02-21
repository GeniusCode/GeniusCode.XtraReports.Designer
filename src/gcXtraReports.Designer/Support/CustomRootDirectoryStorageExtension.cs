using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.UI;
using GeniusCode.XtraReports.Designer.Messaging;
using gcExtensions;

namespace GeniusCode.XtraReports.Designer.Support
{
    public class CustomRootDirectoryStorageExtension : ReportStorageExtension
    {
        private readonly string _rootDirectory;

        public CustomRootDirectoryStorageExtension(string rootDirectory)
        {
            _rootDirectory = rootDirectory;
        }

        protected override string RootDirectory
        {
            get { return _rootDirectory; }  
        }
    }
}
