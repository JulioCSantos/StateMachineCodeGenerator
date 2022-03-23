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
        public static string XmlFilePath = null;
        public static string OutputFilesPath = null;

        public static void MainEntryPoint(string[] args)
        {
            // Handle arguments
            if (args.Any())
            {
                XmlFilePath = args[0];
                if (args.Length > 1) OutputFilesPath = args[1];
                //var models = GetStateMachineModelFromEAXMLFile(XmlFilePath);
                
            }
            else throw new Exception("Exported EA file (.xml) not provided.");

            if (string.IsNullOrEmpty(OutputFilesPath))
            {
                var filename = Path.GetFileNameWithoutExtension(XmlFilePath);
                OutputFilesPath = Path.GetDirectoryName(XmlFilePath) + "\\" + filename;
            }
            Directory.CreateDirectory(OutputFilesPath);

            var parsedXmlFile = new ParsedXmlFile(XmlFilePath);

            foreach (var diagram in parsedXmlFile.DiagramNodes)
            {
                parsedXmlFile.ActiveDiagramElem = diagram;
                var activeMainModel = new MainModel(parsedXmlFile.ActiveDiagramElem.Attribute("name").Value, parsedXmlFile.ActiveDiagramElem.GetId());
                var activeXML2ModelMapper = new XML2ModelMapper();
                activeXML2ModelMapper.Map(parsedXmlFile, activeMainModel);
                StateMachineGenerator.Generate(activeMainModel, OutputFilesPath);
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


        public static List<MainModel> MainModels { get; private set; }

        private static MainModel activeModel;
        public static MainModel ActiveModel
        {
            get { return activeModel ?? (activeModel = MainModels?.First()); }
            set { activeModel = value; }
        }

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

        public static Dictionary<TargetFileType, string> GetTargetFilesInSolution(string path)
        {
            // instantiate the Target files dictionary
            var filesDict = new Dictionary<TargetFileType, string>();
            filesDict.Add(TargetFileType.Solution, string.Empty);
            filesDict.Add(TargetFileType.StateMachineType, string.Empty);
            filesDict.Add(TargetFileType.StateMachineInterface, string.Empty);
            filesDict.Add(TargetFileType.SystemType, string.Empty);
            filesDict.Add(TargetFileType.SystemInterface, string.Empty);

            // If Solution not found return an empty instantiated dictionary
            if (string.IsNullOrEmpty(path)) return filesDict;
            var solutionFile = GetSolutionFile(path);
            if (string.IsNullOrEmpty(solutionFile)) return filesDict;


            string directoryName = Path.GetDirectoryName(solutionFile);
            List<string> filesList = ((IEnumerable<string>)Directory.GetFiles(directoryName, "*StateMachine.cs", SearchOption.AllDirectories)).ToList<string>();
            filesList.AddRange((IEnumerable<string>)Directory.GetFiles(directoryName, "*System.cs", SearchOption.AllDirectories));

            // Setup Target files
            filesDict[TargetFileType.Solution] = solutionFile;
            var solutionName = Path.GetFileName(solutionFile).Replace("All.sln", "");
            filesDict[TargetFileType.StateMachineType] = filesList.Where(tf => tf.EndsWith($"{solutionName}StateMachine.cs") && tf.Contains(@"Main\System")).FirstOrDefault();
            filesDict[TargetFileType.StateMachineInterface] = filesList.Where(tf => tf.EndsWith($"I{solutionName}StateMachine.cs") && tf.Contains(@"Main\Interfaces")).FirstOrDefault();
            filesDict[TargetFileType.SystemType] = filesList.Where(tf => tf.EndsWith($"{solutionName}System.cs") && tf.Contains(@"Main\System")).FirstOrDefault();
            filesDict[TargetFileType.SystemInterface] = filesList.Where(tf => tf.EndsWith($"I{solutionName}System.cs") && tf.Contains(@"Main\Interfaces")).FirstOrDefault();

            return filesDict;
        }

        public static string GetSolutionFile(string solutionPath)
        {
            // find solution file's directory
            if (File.Exists(solutionPath) && Path.GetExtension(solutionPath) == ".sln") return solutionPath;
            string[] filesPaths = Main.FindFileInCurrentOrParentFolder(solutionPath, "*.sln");
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
            XmlFilePath = xmlFilePath;
            var parsedXmlFile = new ParsedXmlFile(XmlFilePath);
            return parsedXmlFile;
        }
        #endregion Private Methods
    }

    public enum TargetFileType
    {
        unkown,
        Solution,
        StateMachineType,
        StateMachineInterface,
        SystemType,
        SystemInterface
    }
}
