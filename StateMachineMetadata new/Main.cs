using StateMachineMetadata.Extensions;
using StateMachineMetadata.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StateMachineMetadata
{
    public static class Main
    {

        #region Properties
        public static string CodeGeneratedLegacyDefaultLocation { get; } = @"\Main\Components\MainModel";
        public static string CodeGeneratedDefaultLocation { get; } = @"\Main\Model\MainModel";

        #region XmlFile
        private static string _xmlFile;
        public static string XmlFile
        {
            get { return _xmlFile; }
            set
            {
                if (_xmlFile == value) return;
                _xmlFile = value;
                PropertyChanged(nameof(XmlFile));
            }
        }
        #endregion XmlFile

        #region SolutionFile
        private static string _solutonFileName;

        public static string SolutionFileName
        {
            get { return _solutonFileName; }
            set
            {
                if (_solutonFileName == value) return;
                _solutonFileName = value;
                PropertyChanged(nameof(SolutionFileName));
            }
        }
        #endregion SolutionFile

        #region SolutionName
        private static string _solutionName;

        public static string SolutionName
        {
            get { return _solutionName; }
            set
            {
                if (_solutionName == value) return;
                _solutionName = value;
                PropertyChanged(nameof(SolutionName));
            }
        }
        #endregion SolutionName

        #region CodeGeneratedPath
        private static string _codeGeneratedPath;

        public static string CodeGeneratedPath
        {
            get { return _codeGeneratedPath; }
            set
            {
                if (_codeGeneratedPath == value) return;
                _codeGeneratedPath = value;
                if (_codeGeneratedPath.EndsWith(@"\") == false) _codeGeneratedPath += @"\";
                PropertyChanged(nameof(CodeGeneratedPath));
                //TargetFiles = GetTargetFilesInSolution(XmlFilePath, null, CodeGeneratedLocation);
            }
        }
        #endregion CodeGeneratedPath

        #region TargetPaths
        private static Dictionary<TargetPath, string> _targetPathsCache;
        public static Dictionary<TargetPath, string> TargetPaths
        {
            get
            {
                if (_targetPathsCache == null)
                {
                    _targetPathsCache = new Dictionary<TargetPath, string>();
                    InitializeTargetPaths(_targetPathsCache);
                }
                return _targetPathsCache;
            }
            set { _targetPathsCache = value; }
        }
        private static void InitializeTargetPaths(Dictionary<TargetPath, string> targetPaths)
        {
            if (targetPaths == null) targetPaths = new Dictionary<TargetPath, string>();
            targetPaths.Add(StateMachineMetadata.TargetPath.Solution, string.Empty);
            targetPaths.Add(StateMachineMetadata.TargetPath.CodeGeneratedPath, string.Empty);
            targetPaths.Add(StateMachineMetadata.TargetPath.StateMachineBaseFilePath, string.Empty);
            targetPaths.Add(StateMachineMetadata.TargetPath.StateMachineDerivedFilePath, string.Empty);
            targetPaths.Add(StateMachineMetadata.TargetPath.MainModelBaseFilePath, string.Empty);
            targetPaths.Add(StateMachineMetadata.TargetPath.MainModelDerivedFilePath, string.Empty);
        }
        #endregion TargetPaths
        #endregion Properties

        private static void PropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(XmlFile):
                    if (string.IsNullOrEmpty(XmlFile)) { TargetPaths = null; break; }
                    SolutionFileName = GetSolutionFile(XmlFile);
                    if (string.IsNullOrEmpty(SolutionFileName)) { TargetPaths = null; break; }
                    break;

                case nameof(SolutionFileName):
                    if (SolutionFileName == null) break;
                    TargetPaths[StateMachineMetadata.TargetPath.Solution] = SolutionFileName;
                    SolutionName = GetSolutionName(SolutionFileName);
                    CodeGeneratedPath = GetConventionalCodeGeneratedPath(SolutionFileName).FullName;
                    break;

                case nameof(MainModels):
                    PropertyChanged(nameof(ActiveModel));
                    break;
                case nameof(CodeGeneratedPath):
                case nameof(ActiveModel):
                    //TargetPaths = GetTargetFilesStartingOnInsideSolution(XmlFile, null, CodeGeneratedPath);
                    PointGeneratingTargetFilesToPath(TargetPaths, CodeGeneratedPath);
                    break;

            }
        }

        private static string GetSolutionName(string solutionFileName = null)
        {
            if (solutionFileName == null) solutionFileName = SolutionFileName; // default
            if (solutionFileName == null) return null;

            var solutionName = Path.GetFileName(solutionFileName).Replace("All.sln", "");
            if (solutionName.Contains("Oreo"))
                solutionName = solutionName.Replace("Oreo", "WEO") + "System"; //legacy name convention enforcement.
            return solutionName;
        }

        public static string OutputFilesPath = null;

        public static void MainEntryPoint(string[] args)
        {
            // Handle arguments
            if (args.Any())
            {
                XmlFile = args[0];
                if (args.Length > 1) OutputFilesPath = args[1];
                //var models = GetStateMachineModelFromEAXMLFile(XmlFilePath);
                
            }
            else throw new Exception("Exported EA file (.xml) not provided.");

            if (string.IsNullOrEmpty(OutputFilesPath))
            {
                var filename = Path.GetFileNameWithoutExtension(XmlFile);
                OutputFilesPath = Path.GetDirectoryName(XmlFile) + "\\" + filename;
            }
            Directory.CreateDirectory(OutputFilesPath);

            var parsedXmlFile = new ParsedXmlFile(XmlFile);

            foreach (var diagram in parsedXmlFile.DiagramNodes)
            {
                parsedXmlFile.ActiveDiagramElem = diagram;
                var activeMainModel = new MainModel(parsedXmlFile.ActiveDiagramElem.Attribute("name").Value, parsedXmlFile.ActiveDiagramElem.GetId());
                var activeXML2ModelMapper = new XML2ModelMapper();
                activeXML2ModelMapper.Map(parsedXmlFile, activeMainModel);
                //StateMachineGenerator.Generate(activeMainModel, OutputFilesPath);

            }
        }

        public static string GetNamespace(string filePath)
        {
            var fileNamespace = string.Empty;
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return fileNamespace;
            var systemFileLines = File.ReadAllLines(filePath);
            string namespaceLit = "namespace";
            var nameSpaceLine = systemFileLines.Where(l => l.Contains(namespaceLit)).FirstOrDefault();
            if (nameSpaceLine == null) return fileNamespace;
            var nameSpaceValueIndex = nameSpaceLine.IndexOf(namespaceLit) + namespaceLit.Length;
            fileNamespace = nameSpaceLine.Substring(nameSpaceValueIndex).Trim();
            return fileNamespace;
        }



        #region MainModels
        private static List<MainModel> _mainModels;

        public static List<MainModel> MainModels
        {
            get { return _mainModels; }
            private set
            {
                _mainModels = value;
                PropertyChanged(nameof(MainModels));
            }
        }
        #endregion MainModels


        private static MainModel activeModel;
        public static MainModel ActiveModel
        {
            get { return activeModel ?? (activeModel = MainModels?.First()); }
            set { activeModel = value; PropertyChanged(nameof(ActiveModel)); }
        }

        /// <summary>
        /// Main entry point to Parse an XML file exported from the State Machine's (Behavior) diagram create By Enterprise Architect application
        /// </summary>
        /// <param name="xmlFilePath">StateMachine's exported XML file path and name</param>
        /// <returns></returns>
        public static List<MainModel> GetStateMachineModelFromEAXMLFile(string xmlFilePath)
        {
            if (!System.IO.File.Exists(xmlFilePath)) return null;
            var parsedXmlFile = GetParsedEAXMLFile(xmlFilePath);
            if (parsedXmlFile == null) return null;
            var models = new List<MainModel>();

            foreach (var diagram in parsedXmlFile.DiagramNodes)
            {
                parsedXmlFile.ActiveDiagramElem = diagram;
                var activeMainModel = new MainModel(parsedXmlFile.ActiveDiagramElem.Attribute("name").Value, parsedXmlFile.ActiveDiagramElem.GetId());
                var activeXML2ModelMapper = new XML2ModelMapper();
                activeXML2ModelMapper.Map(parsedXmlFile, activeMainModel);
                models.Add(activeMainModel);
            }
            MainModels = models;
            return models;
        }


        public static DirectoryInfo GetConventionalCodeGeneratedPath(string insideSolutionPath)
        {
            // If Solution not found return an empty instantiated dictionary
            if (string.IsNullOrEmpty(insideSolutionPath)) return null;
            var solutionFile = GetSolutionFile(insideSolutionPath);
            if (string.IsNullOrEmpty(solutionFile)) return null;
            //var solutionFileInfo = new FileInfo(solutionFile);
            string codeGeneratePath = null;
            if (solutionFile.ToUpper().Contains("WEOALL")) // The specific solution name for the WEO project
                codeGeneratePath = Path.Combine(Path.GetDirectoryName(solutionFile) + CodeGeneratedLegacyDefaultLocation); // Used for WEO project
            else
                codeGeneratePath = Path.Combine(Path.GetDirectoryName(solutionFile) + CodeGeneratedDefaultLocation);

            var directoryInfo = new DirectoryInfo(codeGeneratePath);
            return directoryInfo;
        }

        public static Dictionary<TargetPath, string> GetTargetFiles(string solutionFile = null, string codeGeneratedPath = null, string diagramName = null)
        {
            if (solutionFile == null) solutionFile = SolutionFileName;
            if (codeGeneratedPath == null) codeGeneratedPath = CodeGeneratedPath;

            if (solutionFile == null || codeGeneratedPath == null) throw new ArgumentNullException();


            var targetPaths = new Dictionary<TargetPath, string>();
            InitializeTargetPaths(targetPaths);

            // Setup Target files
            TargetPaths[StateMachineMetadata.TargetPath.Solution] = solutionFile;
            //var solutionName = Path.GetFileName(solutionFile).Replace("All.sln", "");
            //if (solutionName.Contains("Oreo")) solutionName = solutionName.Replace("Oreo", "WEO") + "System"; //legacy name convention enforcement.
            
            //PointGeneratingTargetFilesToPath(targetPaths, codeGeneratedPath, solutionName);

            return TargetPaths;
        }

        public static void PointGeneratingTargetFilesToPath(Dictionary<TargetPath, string> targetFiles, string codeGeneratedPath, string solutionName = null)
        {
            if (targetFiles == null || targetFiles.Any() == false) return;
            if (string.IsNullOrEmpty(codeGeneratedPath)) codeGeneratedPath = CodeGeneratedPath;
            if (string.IsNullOrEmpty(codeGeneratedPath)) {
                foreach (var keyValuePair in targetFiles.Where(kv => kv.Key != TargetPath.Solution)) {
                        targetFiles[keyValuePair.Key] = null;
                        return;
                    }
            }
            if (solutionName == null) solutionName = ActiveModel?.Name;
            if (solutionName == null) return;
            targetFiles[TargetPath.CodeGeneratedPath] = codeGeneratedPath;

            if (solutionName.ToUpper().Contains("WEOSYSTEM"))
            {
                targetFiles[TargetPath.StateMachineBaseFilePath] = codeGeneratedPath + $"C{solutionName}StateMachineBase_gen.cs";
                targetFiles[TargetPath.StateMachineDerivedFilePath] = codeGeneratedPath + $"C{solutionName}StateMachine.cs";
                targetFiles[TargetPath.MainModelBaseFilePath] = codeGeneratedPath + $"C{solutionName}ModelBase_gen.cs";
                targetFiles[TargetPath.MainModelDerivedFilePath] = codeGeneratedPath + $"C{solutionName}Model.cs";
            }
            else
            {
                targetFiles[TargetPath.StateMachineBaseFilePath] = codeGeneratedPath + $"C{solutionName}MainStateMachineBase_gen.cs";
                targetFiles[TargetPath.StateMachineDerivedFilePath] = codeGeneratedPath + $"C{solutionName}MainStateMachine.cs";
                targetFiles[TargetPath.MainModelBaseFilePath] = codeGeneratedPath + $"C{solutionName}MainModelBase_gen.cs";
                targetFiles[TargetPath.MainModelDerivedFilePath] = codeGeneratedPath + $"C{solutionName}MainModel.cs";
            }
        }

        public static string GetSolutionFile(string insideSolutionPath)
        {
            // find solution file's directory
            if (File.Exists(insideSolutionPath) && Path.GetExtension(insideSolutionPath) == ".sln") return insideSolutionPath;
            string[] filesPaths = Main.FindFileInCurrentOrParentFolder(insideSolutionPath, "*.sln");
            if (filesPaths == null || filesPaths.Length == 0) return null;
            return filesPaths[0];
        }



        #region Private Methods
        private static string[] FindFileInCurrentOrParentFolder(string searchFolderPath, string fileSearchPattern)
        {
            // if this is a File path convert to actual directory
            if (File.Exists(searchFolderPath)) searchFolderPath = Path.GetDirectoryName(searchFolderPath);
            // If path is not a valid Folder then ignore
            if (!Directory.Exists(searchFolderPath) ) return null;

            string[] files = Directory.GetFiles(searchFolderPath, fileSearchPattern, SearchOption.TopDirectoryOnly);
            if (files.Length != 0) return files;
            DirectoryInfo parent = Directory.GetParent(searchFolderPath);
            if (parent.Root.FullName == parent.FullName)
                return (string[])null;
            return Main.FindFileInCurrentOrParentFolder(parent.FullName, fileSearchPattern);
        }

        private static ParsedXmlFile GetParsedEAXMLFile(string xmlFilePath)
        {
            if (string.IsNullOrEmpty(xmlFilePath)) return null;
            XmlFile = xmlFilePath;
            var parsedXmlFile = new ParsedXmlFile(XmlFile);
            return parsedXmlFile;
        } 


        #endregion Private Methods
    }

    public enum TargetPath
    {
        unkown,
        CodeGeneratedPath,
        Solution,
        StateMachineBaseFilePath,
        StateMachineDerivedFilePath,
        MainModelBaseFilePath,
        MainModelDerivedFilePath,
        NameSpace
    }
}
