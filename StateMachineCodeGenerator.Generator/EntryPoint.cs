using StateMachineCodeGeneratorSystem.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.Generator
{
    public class EntryPoint
    {
        /// <summary>
        /// The Filename and Path of the State Machine's Interface
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private string _eaXmlFilePath;
        // ReSharper disable once InconsistentNaming
        public string EaXmlFilePath {
            get { return _eaXmlFilePath; }
            set { _eaXmlFilePath = value; }
        }

        // This was included as part of a Quick Start Guid and may be removed
        // This starts a thread which calls "ApplicationTask" every 1000 mSec and is called by the State Machine.
        public Task StartRunningTheApplication(string eaXmlFilePath) {
            EaXmlFilePath = eaXmlFilePath;

            var taskGen = new Task(async () => await ApplicationTask());
            try {
                taskGen.Start();
                taskGen.Wait(); // this is a non-blocking (UI fully operational) command even though we won't proceed until 'ApplicationTask' finishes running.
            }
            catch (AggregateException ea) {
            }

            return taskGen;
        }
        public async Task<bool> ApplicationTask() {
            //var xmlFile = new FileInfo(@"C:\GenSys\GenSys projects\Shield T4\Models\Shield State Machine.xml");
            var xmlFile = new FileInfo(EaXmlFilePath);

            var CodeGenerator = new TemplatesGenerator(xmlFile);
            var filesGenerated = await CodeGenerator.GenerateFiles();

            return filesGenerated;
        }
    }
}
