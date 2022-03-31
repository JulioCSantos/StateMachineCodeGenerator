using StateMachineCodeGenerator.Common;
using StateMachineMetadata.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StateMachineMetadata
{
    //copied code from code behind.
    public class ViewModelCandidate : SetPropertyBase
    {

        /// <summary>
        /// The Filename and Path of the State Machine's Interface
        /// </summary>
        private string m_strEAXMLFilePath;
        public string EAXMLFilePath {
            get { return m_strEAXMLFilePath; }
            set { m_strEAXMLFilePath = value; }
        }

        private CSystem _cSystem;

        public CSystem CSystem {
            get { return _cSystem; }
            set { _cSystem = value; }
        }


        public string m_oEAXMLFileFilePathSetup_textBox { get; set; }

        public List<MainModel> m_lstEaModels { get; private set; } = new List<MainModel>();

        public List<string> m_oStateMachineName_comboBox => new List<string>();
        private string m_strStateMachineName = "";


        #region m_oStateMachineNamespaceName_textBox
        private string _m_oStateMachineNamespaceName_textBox;
        public string m_oStateMachineNamespaceName_textBox {
            get => _m_oStateMachineNamespaceName_textBox;
            set => SetProperty(ref _m_oStateMachineNamespaceName_textBox, value);
        }
        #endregion m_oStateMachineNamespaceName_textBox

        #region m_bIsReadyToRun
        private bool _m_bIsReadyToRun;
        public bool m_bIsReadyToRun {
            get => _m_bIsReadyToRun;
            set => SetProperty(ref _m_bIsReadyToRun, value);
        }
        #endregion m_bIsReadyToRun

        #region m_oStateMachineSolutionFilePathSetup_textBox
        private string _m_oStateMachineSolutionFilePathSetup_textBox;
        public string m_oStateMachineSolutionFilePathSetup_textBox {
            get => _m_oStateMachineSolutionFilePathSetup_textBox;
            set => SetProperty(ref _m_oStateMachineSolutionFilePathSetup_textBox, value);
        }
        #endregion m_oStateMachineSolutionFilePathSetup_textBox

        #region m_oStateMachineGeneratedCodeFolderPath_textBox
        private string _m_oStateMachineGeneratedCodeFolderPath_textBox;
        public string m_oStateMachineGeneratedCodeFolderPath_textBox {
            get => _m_oStateMachineGeneratedCodeFolderPath_textBox;
            set => SetProperty(ref _m_oStateMachineGeneratedCodeFolderPath_textBox, value);
        }
        #endregion m_oStateMachineGeneratedCodeFolderPath_textBox

        #region m_oStateMachineSourceFilePath_textBox
        private string _m_oStateMachineSourceFilePath_textBox;
        public string m_oStateMachineSourceFilePath_textBox {
            get => _m_oStateMachineSourceFilePath_textBox;
            set => SetProperty(ref _m_oStateMachineSourceFilePath_textBox, value);
        }
        #endregion m_oStateMachineSourceFilePath_textBox

        #region m_oStateMachineInterfaceFilePath_textBox
        private string _m_oStateMachineInterfaceFilePath_textBox;
        public string m_oStateMachineInterfaceFilePath_textBox {
            get => _m_oStateMachineInterfaceFilePath_textBox;
            set => SetProperty(ref _m_oStateMachineInterfaceFilePath_textBox, value);
        }
        #endregion m_oStateMachineInterfaceFilePath_textBox

        #region m_oStateMachineOwnerSourceFilePath_textBox
        private string _m_oStateMachineOwnerSourceFilePath_textBox;
        public string m_oStateMachineOwnerSourceFilePath_textBox {
            get => _m_oStateMachineOwnerSourceFilePath_textBox;
            set => SetProperty(ref _m_oStateMachineOwnerSourceFilePath_textBox, value);
        }
        #endregion m_oStateMachineOwnerSourceFilePath_textBox

        #region m_oStateMachineOwnerInterfaceFilePath_textBox
        private string _m_oStateMachineOwnerInterfaceFilePath_textBox;
        public string m_oStateMachineOwnerInterfaceFilePath_textBox {
            get => _m_oStateMachineOwnerInterfaceFilePath_textBox;
            set => SetProperty(ref _m_oStateMachineOwnerInterfaceFilePath_textBox, value);
        }
        #endregion m_oStateMachineOwnerInterfaceFilePath_textBox



        private void m_oEAXMLFileFilePathSetup_textBox_TextChanged(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(m_oEAXMLFileFilePathSetup_textBox)) return;
            //Cursor.Current = Cursors.WaitCursor;
            EA_XML_FileSetup(m_oEAXMLFileFilePathSetup_textBox);
            //Cursor.Current = Cursors.Default;
        }

        public void EA_XML_FileSetup(string EAXMLFilePath) {
            if (!System.IO.File.Exists(EAXMLFilePath)) { return;}

            UpdateStateMachineNamesComboBox(EAXMLFilePath);
            //m_oStateMachineCodeGeneratorSystem.EAXMLFilePath = EAXMLFilePath;
            CSystem.EAXMLFilePath = EAXMLFilePath;
            SetSolutionTargetFiles(EAXMLFilePath);
            //EnableStartButton(m_bIsReadyToRun);
            m_bIsReadyToRun = true;

        }

        private void UpdateStateMachineNamesComboBox(string EA_XML_FilePath) {
            //m_lstEaModels = StateMachineMetadata.Application.GetStateMachineModelFromEAXMLFile(m_oEaXMLFileFilePath_textBox.Text);
            m_lstEaModels = StateMachineMetadata.Main.GetStateMachineModelFromEAXMLFile(EA_XML_FilePath);
            //m_oStateMachineName_comboBox.BeginUpdate();
            //m_oStateMachineName_comboBox.Items.Clear();
            //m_lstEaModels.Select(m => m.ProjectName).ToList().ForEach(dn => m_oStateMachineName_comboBox.Items.Add(dn));
            m_lstEaModels.Select(m => m.ProjectName).ToList().ForEach(dn => m_oStateMachineName_comboBox.Add(dn));
            //m_oStateMachineName_comboBox.EndUpdate();
            if (m_oStateMachineName_comboBox.Count > 0) {
                m_strStateMachineName = (string)m_oStateMachineName_comboBox[0];
                //m_oStateMachineName_comboBox.SelectedIndex = 0;
                //m_oStateMachineName_comboBox.SelectedItem = m_strStateMachineName;
            }
        }

        public void SetSolutionTargetFiles(string solutionFile) {
            SetTextBoxesToTargetPaths(StateMachineMetadata.Main.TargetPaths);
            //m_oStateMachineNamespaceName_textBox.Text = StateMachineMetadata.Main.GetNamespace(StateMachineMetadata.Main.TargetPaths[StateMachineMetadata.TargetPath.MainModelBaseFilePath]);
            m_oStateMachineNamespaceName_textBox = StateMachineMetadata.Main.GetNamespace(StateMachineMetadata.Main.TargetPaths[StateMachineMetadata.TargetPath.MainModelBaseFilePath]);
        }



        private void SetTextBoxesToTargetPaths(Dictionary<StateMachineMetadata.TargetPath, string> targetPaths = null) {
            if (targetPaths == null) targetPaths = StateMachineMetadata.Main.TargetPaths;

            if (m_oStateMachineSolutionFilePathSetup_textBox != targetPaths[StateMachineMetadata.TargetPath.Solution])
                m_oStateMachineSolutionFilePathSetup_textBox = targetPaths[StateMachineMetadata.TargetPath.Solution];
            if (m_oStateMachineGeneratedCodeFolderPath_textBox != targetPaths[StateMachineMetadata.TargetPath.CodeGeneratedPath])
                m_oStateMachineGeneratedCodeFolderPath_textBox = targetPaths[StateMachineMetadata.TargetPath.CodeGeneratedPath];
            m_oStateMachineSourceFilePath_textBox = targetPaths[StateMachineMetadata.TargetPath.StateMachineBaseFilePath];
            m_oStateMachineInterfaceFilePath_textBox = targetPaths[StateMachineMetadata.TargetPath.StateMachineDerivedFilePath];
            m_oStateMachineOwnerSourceFilePath_textBox = targetPaths[StateMachineMetadata.TargetPath.MainModelBaseFilePath];
            m_oStateMachineOwnerInterfaceFilePath_textBox = targetPaths[StateMachineMetadata.TargetPath.MainModelDerivedFilePath];


            //if (m_oStateMachineSolutionFilePathSetup_textBox.Text != targetPaths[StateMachineMetadata.TargetPath.Solution])
            //    m_oStateMachineSolutionFilePathSetup_textBox.Text = targetPaths[StateMachineMetadata.TargetPath.Solution];
            //if (m_oStateMachineGeneratedCodeFolderPath_textBox.Text != targetPaths[StateMachineMetadata.TargetPath.CodeGeneratedPath])
            //    m_oStateMachineGeneratedCodeFolderPath_textBox.Text = targetPaths[StateMachineMetadata.TargetPath.CodeGeneratedPath];
            //m_oStateMachineSourceFilePath_textBox.Text = targetPaths[StateMachineMetadata.TargetPath.StateMachineBaseFilePath];
            //m_oStateMachineInterfaceFilePath_textBox.Text = targetPaths[StateMachineMetadata.TargetPath.StateMachineDerivedFilePath];
            //m_oStateMachineOwnerSourceFilePath_textBox.Text = targetPaths[StateMachineMetadata.TargetPath.MainModelBaseFilePath];
            //m_oStateMachineOwnerInterfaceFilePath_textBox.Text = targetPaths[StateMachineMetadata.TargetPath.MainModelDerivedFilePath];


        }


        //var xmlFile = new FileInfo(@"C:\GenSys\GenSys projects\Shield T4\Models\Shield State Machine.xml");
        //var xmlFile = new FileInfo(EAXMLFilePath);

        //CodeGenerator = new TemplatesGenerator(xmlFile);
        //var filesGenerated = await CodeGenerator.GenerateFiles();

        //return filesGenerated;

    }
}
