﻿using StateMachineCodeGenerator.Common;
using StateMachineMetadata;
using StateMachineMetadata.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.ViewModels
{
    public class MainViewModel : SetPropertyBase
    {
        #region Commands
        public RelayCommand StartParsingCommand => new RelayCommand((o) => StartParsing());
        public RelayCommand OpenFileExplorerCommand => new RelayCommand(OpenFileExplorer);
        #endregion Commands

        #region TargetFilesDirectory
        private TargetFilesDirectory _targetFilesDirectory;
        public TargetFilesDirectory TargetFilesDirectory {
            get => _targetFilesDirectory ??= TargetFilesDirectory.Instance;
            set => SetProperty(ref _targetFilesDirectory, value);
        }
        #endregion TargetFilesDirectory

        #region constructor
        public MainViewModel() {
            //this.PropertyChanged += MainViewModel_PropertyChanged;
            this.TargetFilesDirectory.PropertyChanged += TargetFilesDirectory_PropertyChanged;
        }

        private void TargetFilesDirectory_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(TargetFilesDirectory.EaXmlFileName):
                    TargetFilesDirectory.EaModelsList = Main.GetStateMachineModelFromEAXMLFile(TargetFilesDirectory.EaXmlFileName);
                    TargetFilesDirectory.SelectedEaModel = TargetFilesDirectory.EaModelsList?.FirstOrDefault();
                    break;
            }
        }

        #endregion constructor

        #region commands
        public void OpenFileExplorer(object path) {
            if (string.IsNullOrEmpty(TargetFilesDirectory.EaXmlFileName) == false && string.IsNullOrEmpty(TargetFilesDirectory.TargetSolutionFileName)) {
                TargetFilesDirectory.TargetSolutionFileName = TargetFilesDirectory.TargetSolutionLiteral;
            }

            if (string.IsNullOrEmpty(TargetFilesDirectory.EaXmlFileName)) {
                TargetFilesDirectory.EaXmlFileName = TargetFilesDirectory.EaXmlFilePathLiteral;
            }

        }

        public void StartParsing() {
            //Main.MainEntryPoint(new string[] { EAXMLFileName, TargetSolutionFileName });
            //var models = Main.MainModels;
            //var codeGenerator = new StateMachineGenerator();
            //codeGenerator.Generate(models.FirstOrDefault(), TargetSolutionFileName);
        }
        #endregion commands

    }
}
