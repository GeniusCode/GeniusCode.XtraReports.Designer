using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;

namespace GeniusCode.XtraReports.Designer.Prototypes
{
    public class DllLoader : IDynamicDllLoader
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private void LoadAssemblies(string path)
        {

            var assemblyFilePaths = Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories).ToList();
            _logger.Trace("{0} dlls to load from path {1}", assemblyFilePaths.Count, path);
            for (var index = 0; index < assemblyFilePaths.Count; index++)
            {
                var file = assemblyFilePaths[index];
                _logger.Trace("Loading dll {0} of {1} from {2}", index + 1, assemblyFilePaths.Count, file);
                LoadDll(file);
            }
        }

        //TODO: Unit Test this is required to avoid errors while loading on the fly
        private void LoadDll(string path)
        {
            var justTheFileName = Path.GetFileName(path);

            //DO NOT LOAD THIS DLL EVER!
            if (justTheFileName == "gcXtraReports.Core.dll") return;

/*            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var existingFiles = assemblies.Select(a => Path.GetFileName(a.Location)).ToList();

            if (!existingFiles.Contains(justTheFileName)) 
                Assembly.LoadFrom(path);*/


            Assembly.LoadFrom(path);
        }

        public void LoadDllsInDirectory(string path)
        {
            LoadAssemblies(path);
        }
    }
}