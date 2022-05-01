using StateMachineMetadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StateMachineMetadata.Extensions;

namespace StateMachineCodeGeneratorSystem.Templates
{
    public class TemplatesGenerator
    {
        public FileInfo EAModelFile { get; set; }
        public TemplatesGenerator(FileInfo eaModelFile)
        {
            if (eaModelFile == null) throw new ArgumentNullException();
            if (!eaModelFile.Exists) throw new FileNotFoundException();
            EAModelFile = eaModelFile;
        }


        //public void GenerateIn(DirectoryInfo targetDirectory = null)
        //{
        //    // Parse the XML file
        //    if (Main.ActiveModel == null)
        //        Main.ActiveModel = Main.GetStateMachineModelFromEAXMLFile(EAModelFile.FullName).First();

        //    Dictionary<TargetPath, string> targetFiles;
        //    if (targetDirectory == null) targetFiles = Main.GetTargetFiles();
        //    else
        //    {
        //        targetFiles = Main.GetTargetFiles();
        //        targetFiles.Keys.ToList().ForEach(k => targetFiles[k] = targetDirectory.FullName);
        //    }

        //    GenerateFiles(targetFiles);
        //}

        private string _stateMachineName;
        public string StateMachineName
        {
            get { return _stateMachineName; }
            set { _stateMachineName = value; }
        }

        public string StateMachineBaseTypeName { get; private set; }
        public string StateMachineDerivedTypeName { get; private set; }
        public string StateMachineModelBaseTypeName { get; private set; }
        public string StateMachineModelDerivedTypeName { get; private set; }

        public async Task<bool> GenerateFiles(Dictionary<TargetPath, string> targetFiles = null)
        {
            // Find the targetted files based on passing path or EnterpriseArchitect's XML file
            if (targetFiles == null) 
                targetFiles = Main.TargetPaths;

            var targetDirectory = new DirectoryInfo(targetFiles[TargetPath.CodeGeneratedPath]);
            if (targetDirectory.Exists == false) targetDirectory.Create();

            var getNameOnly = new Func<string, string>(a => new FileInfo(a).Name.Replace("_gen.cs", "").Replace(".cs", ""));

            StateMachineBaseTypeName = getNameOnly(targetFiles[TargetPath.StateMachineBaseFilePath]);
            StateMachineDerivedTypeName = getNameOnly(targetFiles[TargetPath.StateMachineDerivedFilePath]);
            StateMachineModelBaseTypeName = getNameOnly(targetFiles[TargetPath.MainModelBaseFilePath]);
            StateMachineModelDerivedTypeName = getNameOnly(targetFiles[TargetPath.MainModelDerivedFilePath]);

            StateMachineName = StateMachineDerivedTypeName;

            var smBaseGenerationTask = await Task.Factory.StartNew(() => GenerateStateMachineBaseFile(new FileInfo(targetFiles[TargetPath.StateMachineBaseFilePath])));
            var smDervGenerationTask = await Task.Factory.StartNew(() => GenerateStateMachineDerivedFile(new FileInfo(targetFiles[TargetPath.StateMachineDerivedFilePath])));
            var smModelBaseGenerationTask = await Task.Factory.StartNew(() => GenerateStateMachineModelBaseFile(new FileInfo(targetFiles[TargetPath.MainModelBaseFilePath])));
            var smModelDervdGenerationTask = await Task.Factory.StartNew(() => GenerateStateMachineModelDerivedFile(new FileInfo(targetFiles[TargetPath.MainModelDerivedFilePath])));

            await Task.WhenAll(smBaseGenerationTask, smModelBaseGenerationTask, smDervGenerationTask, smModelDervdGenerationTask);
            if (smBaseGenerationTask.Result && smDervGenerationTask.Result && smModelBaseGenerationTask.Result &&
                smModelDervdGenerationTask.Result) {return true;}
            System.Diagnostics.Debug.Assert(smBaseGenerationTask.Result, "GenerateStateMachineBaseFile failed.");
            System.Diagnostics.Debug.Assert(smDervGenerationTask.Result, "GenerateStateMachineDerivedFile failed.");
            System.Diagnostics.Debug.Assert(smModelBaseGenerationTask.Result, "GenerateStateMachineModelBaseFile failed.");
            System.Diagnostics.Debug.Assert(smModelDervdGenerationTask.Result, "GenerateStateMachineModelDerivedFile failed.");
            return false;
        }

        public Task<bool> GenerateStateMachineBaseFile(FileInfo smFileInfo)
        {
            var result = false;
            try
            {
                var templ = new StateMachineBaseTemplate();
                var model = Main.ActiveModel;
                var fileName = smFileInfo.Name;
                var filePath = smFileInfo.DirectoryName + "\\" + fileName;

                var typeName = fileName.Replace(".cs", "").Replace("_gen", "");
                var projectName = model.DiagramName; ;
                templ.Session = new Dictionary<string, object>();
                templ.Session.Add(nameof(fileName), fileName);
                templ.Session.Add(nameof(typeName), typeName);
                templ.Session.Add(nameof(projectName), projectName);
                templ.Session.Add(nameof(StateMachineBaseTypeName), StateMachineBaseTypeName);
                templ.Session.Add(nameof(StateMachineDerivedTypeName), StateMachineDerivedTypeName);
                templ.Session.Add(nameof(StateMachineModelBaseTypeName), StateMachineModelBaseTypeName);
                templ.Session.Add(nameof(StateMachineModelDerivedTypeName), StateMachineModelDerivedTypeName);

                //Generate file
                var content = templ.TransformText();
                File.WriteAllText(filePath, content);
                result = true;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return Task.FromResult(result);
        }

        public Task<bool> GenerateStateMachineModelBaseFile(FileInfo smViewModelFileInfo)
        {
            bool result = false;
            try
            {
                var templ = new StateMachineModelBaseTemplate();
                var model = Main.ActiveModel;
                var fileName = smViewModelFileInfo.Name;
                var filePath = smViewModelFileInfo.DirectoryName + "\\" + fileName;
                var typeName = fileName.Replace(".cs", "").Replace("_gen","");

                var projectName = model.DiagramName; ;
                templ.Session = new Dictionary<string, object>();
                templ.Session.Add(nameof(fileName), fileName);
                templ.Session.Add(nameof(typeName), typeName);
                templ.Session.Add(nameof(projectName), projectName);
                templ.Session.Add(nameof(StateMachineBaseTypeName), StateMachineBaseTypeName);
                templ.Session.Add(nameof(StateMachineDerivedTypeName), StateMachineDerivedTypeName);
                templ.Session.Add(nameof(StateMachineModelBaseTypeName), StateMachineModelBaseTypeName);
                templ.Session.Add(nameof(StateMachineModelDerivedTypeName), StateMachineModelDerivedTypeName);

                //Generate file
                var content = templ.TransformText();
                File.WriteAllText(filePath, content);
                result = true;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return Task.FromResult(result);
        }

        public Task<bool> GenerateStateMachineDerivedFile(FileInfo smFileInfo)
        {
            bool result = false;
            try
            {
                var templ = new StateMachineDerivedTemplate();
                var model = Main.ActiveModel;
                var fileName = smFileInfo.Name;
                var filePath = smFileInfo.DirectoryName + "\\" + fileName;
                if (File.Exists(filePath)) return Task.FromResult(true);

                var typeName = fileName.Replace(".cs", "").Replace("_gen", "");
                var projectName = model.DiagramName; ;
                templ.Session = new Dictionary<string, object>();
                templ.Session.Add(nameof(fileName), fileName);
                templ.Session.Add(nameof(typeName), typeName);
                templ.Session.Add(nameof(projectName), projectName);
                templ.Session.Add(nameof(StateMachineBaseTypeName), StateMachineBaseTypeName);
                templ.Session.Add(nameof(StateMachineDerivedTypeName), StateMachineDerivedTypeName);
                templ.Session.Add(nameof(StateMachineModelBaseTypeName), StateMachineModelBaseTypeName);
                templ.Session.Add(nameof(StateMachineModelDerivedTypeName), StateMachineModelDerivedTypeName);

                //Generate file
                var content = templ.TransformText();
                File.WriteAllText(filePath, content);
                result = true;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return Task.FromResult(result);
        }

        public Task<bool> GenerateStateMachineModelDerivedFile(FileInfo smViewModelFileInfo)
        {
            bool result = false;
            try
            {
                var templ = new StateMachineModelDerivedTemplate();
                var model = Main.ActiveModel;
                var fileName = smViewModelFileInfo.Name;
                var filePath = smViewModelFileInfo.DirectoryName + "\\" + fileName;
                if (File.Exists(filePath)) return Task.FromResult(true);

                var typeName = fileName.Replace(".cs", "").Replace("_gen", "");
                var projectName = model.DiagramName; ;
                templ.Session = new Dictionary<string, object>();
                templ.Session.Add(nameof(fileName), fileName);
                templ.Session.Add(nameof(typeName), typeName);
                templ.Session.Add(nameof(projectName), projectName);
                templ.Session.Add(nameof(StateMachineBaseTypeName), StateMachineBaseTypeName);
                templ.Session.Add(nameof(StateMachineDerivedTypeName), StateMachineDerivedTypeName);
                templ.Session.Add(nameof(StateMachineModelBaseTypeName), StateMachineModelBaseTypeName);
                templ.Session.Add(nameof(StateMachineModelDerivedTypeName), StateMachineModelDerivedTypeName);

                //Generate file
                var content = templ.TransformText();
                File.WriteAllText(filePath, content);
                result = true;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return Task.FromResult(result);
        }
    }
}
