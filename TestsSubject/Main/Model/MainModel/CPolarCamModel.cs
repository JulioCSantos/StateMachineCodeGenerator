 
 
// Created by t4 template 'StateMachineModelDerivedTemplate'
///////////////////////////////////////////////////////////
// Copyright © Corning Incorporated 2017
// File CPolarCamModel.cs
// Project PolarCam
// Implementation of the Class CPolarCamModel
// Created on 9/5/2022 10:25:17 AM
///////////////////////////////////////////////////////////
using GenSysCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using Corning.GenSys.Logger;
using NorthStateSoftware.NorthStateFramework;
using System.Threading.Tasks;
using System.Linq;
using Corning.GenSys.Scanning;
using System.Runtime.CompilerServices;


namespace PolarCam.Model
{
    // ReSharper disable once InconsistentNaming
    public partial class CPolarCamModel : CPolarCamModelBase, IDisposable
    {

        #region properties

        #region EventIds
        public override string CameraFaultEventId { get { return nameof(EventBroker.CameraFaultEventId); } } 
        public override string SettingsFaultEventId { get { return nameof(EventBroker.SettingsFaultEventId); } } 
        public override string UndefinedFaultEventId { get { return nameof(EventBroker.UndefinedFaultEventId); } } 
        #endregion EventIds

        #endregion properties

        #region Constructors

        #region Singleton        
        private static readonly object SingletonLock = new object();
        private static CPolarCamModel mainModel;
        public static CPolarCamModel GetSingleton()
        {
                if (mainModel != null) return mainModel;
                lock (SingletonLock)
                {
                    return mainModel ?? (mainModel = new CPolarCamModel(CPolarCamStateMachineBase.StateMachineName));
                }
        }
        #endregion Singleton

        private CPolarCamModel(string strName)
        {
            base.StateMachine = new CPolarCamStateMachine(strName, this);
            base.RegisterFaultEventMethods();
            this.PropertyChanged += CPolarCamModel_PropertyChanged;
        }
        #endregion Constructors

        #region methods
            private void CPolarCamModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(IsInInitState):
                    break;
            }
        }        
        #endregion methods

        #region IDisposable
        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                if (StateMachine.TerminationStatus == NSFEventHandlerTerminationStatus.EventHandlerReady)
                    NSFEnvironment.terminate();
            }

            // Free any unmanaged objects here.

            disposed = true;
        }

        ~CPolarCamModel() { Dispose(false); }
        #endregion IDisposable

    }
}