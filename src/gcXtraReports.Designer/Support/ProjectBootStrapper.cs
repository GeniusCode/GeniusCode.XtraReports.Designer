using System;
using System.IO;
using System.Reflection;
using GeniusCode.XtraReports.Designer.Prototypes;
using NLog;

namespace GeniusCode.XtraReports.Designer.Support
{
    //TODO: Refactory / Cleanup this class with tests

    public class ProjectBootStrapper
    {
        private const string PluginsFolderName = "Plugins_Root";

        public string ProjectPath
        {
            get { return _projectPath; }
        }

        private readonly Logger _logger;
        private readonly string _projectPath;
        private readonly string _reportsFolderName;
        private readonly string _datasourceFolderName;
        private readonly string _actionsFolderName;
        private readonly IFileAndDirectoryCloner _cloner;
        private readonly IDynamicDllLoader _loader;
        private readonly string _datasourceTargetFolderPath;
        private readonly string _actionsTargetFolderPath;

        public ProjectBootStrapper(string projectPath, string reportsFolderName, string datasourceFolderName, string actionsFolderName, IFileAndDirectoryCloner cloner, IDynamicDllLoader loader)
        {
            if (String.IsNullOrWhiteSpace(reportsFolderName)) throw new ArgumentNullException("reportsFolderName");
            if (String.IsNullOrWhiteSpace(datasourceFolderName)) throw new ArgumentNullException("datasourceFolderName");
            if (String.IsNullOrWhiteSpace(actionsFolderName)) throw new ArgumentNullException("actionsFolderName");
            if (String.IsNullOrWhiteSpace(projectPath)) throw new ArgumentNullException("projectPath");

            _logger = LogManager.GetCurrentClassLogger();



            _projectPath = projectPath;
            _reportsFolderName = reportsFolderName;
            _datasourceFolderName = datasourceFolderName;
            _actionsFolderName = actionsFolderName;
            _cloner = cloner;
            _loader = loader;

            var applicationRoot = Assembly.GetEntryAssembly() == null
                               ? Path.GetTempPath()
                               : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var pluginsRootPath = Path.Combine(applicationRoot, PluginsFolderName);

            //TODO: Unit test this!
            if (Directory.Exists(pluginsRootPath))
            {
                Directory.Delete(pluginsRootPath, true);
            }
            Directory.CreateDirectory(pluginsRootPath);

            _logger.Trace("Project path: {0}", _projectPath);
            _logger.Trace("Plugins path: {0}", pluginsRootPath);

            _datasourceTargetFolderPath = Path.Combine(pluginsRootPath, _datasourceFolderName);
            _actionsTargetFolderPath = Path.Combine(pluginsRootPath, _actionsFolderName);

        }


        private void CreatePath(string path)
        {

            if (!Directory.Exists(path))
            {
                _logger.Trace("creating directory at {0}", path);
                Directory.CreateDirectory(path);
            }
        }

        public void ExecuteProjectBootStrapperFile(string bootstrapperBat)
        {
            if (string.IsNullOrWhiteSpace(bootstrapperBat)) throw new ArgumentNullException("bootstrapperBat");

            var fullPath = Path.Combine(_projectPath, bootstrapperBat);

            if (!File.Exists(fullPath))
            {
                _logger.Trace("bootstrapper at {0} was not found", fullPath);
                return;
            }
            if (Path.GetExtension(fullPath).ToUpper() != ".BAT")
            {
                _logger.Trace("bootstrapper file at {0} had incorrect extension", fullPath);
                return;
            }

            var proc = new System.Diagnostics.Process
                           {
                               StartInfo =
                                   {
                                       FileName = fullPath,
                                       RedirectStandardError = false,
                                       RedirectStandardOutput = false,
                                       UseShellExecute = false,
                                       WorkingDirectory = _projectPath
                                   }
                           };
            proc.Start();
            proc.WaitForExit();

            /*var output = proc.StandardOutput.ReadToEnd();

            _logger.Trace("output from bootstrapper: {0}", output);*/

        }

        public void CopyProjectFiles()
        {

            var reportSourceFolderPath = Path.Combine(_projectPath, _reportsFolderName);
            var datasourceSourceFolderPath = Path.Combine(_projectPath, _datasourceFolderName);
            var actionsSourceFolderPath = Path.Combine(_projectPath, _actionsFolderName);

            // Create Folders in Project Root Path
            CreatePath(reportSourceFolderPath);
            CreatePath(datasourceSourceFolderPath);
            CreatePath(actionsSourceFolderPath);

            // Create Target Folders for Copying Datasources & Actions under our Application Path
            CreatePath(_datasourceTargetFolderPath);
            CreatePath(_actionsTargetFolderPath);

            CloneFiles(datasourceSourceFolderPath, _datasourceTargetFolderPath);
            CloneFiles(actionsSourceFolderPath, _actionsTargetFolderPath);
        }

        private void CloneFiles(string sourceFolderName, string targetPath)
        {
            _logger.Trace("Cloning files from {0} to {1}", sourceFolderName, targetPath);
            CreatePath(targetPath);
            _cloner.Clone(sourceFolderName, targetPath);
        }


        public void LoadProjectAssemblies()
        {
            _loader.LoadDllsInDirectory(_datasourceTargetFolderPath);
            _loader.LoadDllsInDirectory(_actionsTargetFolderPath);
        }

    }
}