 
 
 
// Created by t4 template 'StateMachineModelBaseTemplate'
///////////////////////////////////////////////////////////
// Copyright © Corning Incorporated 2017
// File CPolarCamModelBase_gen.cs
// Project PolarCam
// Implementation of the Class CPolarCamModelBase
// Created on 2/27/2024 1:48:23 PM
///////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using Corning.GenSys.Logger;
using NorthStateSoftware.NorthStateFramework;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.CompilerServices;
using GenSysCommon;
using GenSysCommon.Interfaces;
using Corning.GenSys.MVVMCommon;

namespace PolarCam.Model
{
    // ReSharper disable once InconsistentNaming
    public class CPolarCamModelBase : INotifyPropertyChanged
    {
        private CPolarCamStateMachine stateMachine;
        public CPolarCamStateMachine StateMachine 
		{ 
			get { return stateMachine; }
			protected set {
				stateMachine = value;
                if (stateMachine != null)
                    this.StateMachine.StateChangesAtStateEvent += StateChangesAtState;
			}
		}
        private static readonly ILogger ms_iLogger = CLoggerFactory.CreateLog("CPolarCamModelBase");

        #region Constructors
        protected internal CPolarCamModelBase() { }
        #endregion Constructors

        #region ExceptionsToFaults
        public virtual void RegisterFaultEventMethods()
        {
            //var CameraPublisher = CCamera.GetInstance();
            //EventBroker.Instance.RegisterEvent(CameraPublisher, "CameraFaultEvent", eventId:CameraFaultEventId);
            EventBroker.Instance.RegisterEventMethods(CameraFaultEventId, this, nameof(CameraExceptionToCameraFault));

            //var SettingsPublisher = CSettings.GetInstance();
            //EventBroker.Instance.RegisterEvent(SettingsPublisher, "SettingsFaultEvent", eventId:SettingsFaultEventId);
            EventBroker.Instance.RegisterEventMethods(SettingsFaultEventId, this, nameof(SettingsExceptionToSettingsFault));

            //var UndefinedPublisher = CUndefined.GetInstance();
            //EventBroker.Instance.RegisterEvent(UndefinedPublisher, "UndefinedFaultEvent", eventId:UndefinedFaultEventId);
            EventBroker.Instance.RegisterEventMethods(UndefinedFaultEventId, this, nameof(UndefinedExceptionToUndefinedFault));

        }

        public virtual string CameraFaultEventId { get { return nameof(CameraFaultEventId); } } 
        public virtual void CameraExceptionToCameraFault(object sender, EventArgs args)
        {
            StateMachine.SendStateEvent(TriggerName.CameraFaultEvent);
        }

        public virtual string SettingsFaultEventId { get { return nameof(SettingsFaultEventId); } } 
        public virtual void SettingsExceptionToSettingsFault(object sender, EventArgs args)
        {
            StateMachine.SendStateEvent(TriggerName.SettingsFaultEvent);
        }

        public virtual string UndefinedFaultEventId { get { return nameof(UndefinedFaultEventId); } } 
        public virtual void UndefinedExceptionToUndefinedFault(object sender, EventArgs args)
        {
            StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
        }

        #endregion ExceptionsToFaults

        #region State indicators ("IsIn...")

        #region IsInAbortingState
        private bool isInAbortingState;
        public bool IsInAbortingState
        {
            get { return isInAbortingState; }
            internal set { SetProperty(ref isInAbortingState, value); }
        }
        #endregion IsInAbortingState

        #region IsInAcquisitionState
        private bool isInAcquisitionState;
        public bool IsInAcquisitionState
        {
            get { return isInAcquisitionState; }
            internal set { SetProperty(ref isInAcquisitionState, value); }
        }
        #endregion IsInAcquisitionState

        #region IsInCameraViewActiveState
        private bool isInCameraViewActiveState;
        public bool IsInCameraViewActiveState
        {
            get { return isInCameraViewActiveState; }
            internal set { SetProperty(ref isInCameraViewActiveState, value); }
        }
        #endregion IsInCameraViewActiveState

        #region IsInCameraViewIdleState
        private bool isInCameraViewIdleState;
        public bool IsInCameraViewIdleState
        {
            get { return isInCameraViewIdleState; }
            internal set { SetProperty(ref isInCameraViewIdleState, value); }
        }
        #endregion IsInCameraViewIdleState

        #region IsInCameraViewState
        private bool isInCameraViewState;
        public bool IsInCameraViewState
        {
            get { return isInCameraViewState; }
            internal set { SetProperty(ref isInCameraViewState, value); }
        }
        #endregion IsInCameraViewState

        #region IsInFaultedState
        private bool isInFaultedState;
        public bool IsInFaultedState
        {
            get { return isInFaultedState; }
            internal set { SetProperty(ref isInFaultedState, value); }
        }
        #endregion IsInFaultedState

        #region IsInFaultParentState
        private bool isInFaultParentState;
        public bool IsInFaultParentState
        {
            get { return isInFaultParentState; }
            internal set { SetProperty(ref isInFaultParentState, value); }
        }
        #endregion IsInFaultParentState

        #region IsInIdleState
        private bool isInIdleState;
        public bool IsInIdleState
        {
            get { return isInIdleState; }
            internal set { SetProperty(ref isInIdleState, value); }
        }
        #endregion IsInIdleState

        #region IsInInitAfterSettingsState
        private bool isInInitAfterSettingsState;
        public bool IsInInitAfterSettingsState
        {
            get { return isInInitAfterSettingsState; }
            internal set { SetProperty(ref isInInitAfterSettingsState, value); }
        }
        #endregion IsInInitAfterSettingsState

        #region IsInInitState
        private bool isInInitState;
        public bool IsInInitState
        {
            get { return isInInitState; }
            internal set { SetProperty(ref isInInitState, value); }
        }
        #endregion IsInInitState

        #region IsInManualProcessImageState
        private bool isInManualProcessImageState;
        public bool IsInManualProcessImageState
        {
            get { return isInManualProcessImageState; }
            internal set { SetProperty(ref isInManualProcessImageState, value); }
        }
        #endregion IsInManualProcessImageState

        #region IsInMotionState
        private bool isInMotionState;
        public bool IsInMotionState
        {
            get { return isInMotionState; }
            internal set { SetProperty(ref isInMotionState, value); }
        }
        #endregion IsInMotionState

        #region IsInOpenSensorState
        private bool isInOpenSensorState;
        public bool IsInOpenSensorState
        {
            get { return isInOpenSensorState; }
            internal set { SetProperty(ref isInOpenSensorState, value); }
        }
        #endregion IsInOpenSensorState

        #region IsInOperatorRequestState
        private bool isInOperatorRequestState;
        public bool IsInOperatorRequestState
        {
            get { return isInOperatorRequestState; }
            internal set { SetProperty(ref isInOperatorRequestState, value); }
        }
        #endregion IsInOperatorRequestState

        #region IsInProcessingState
        private bool isInProcessingState;
        public bool IsInProcessingState
        {
            get { return isInProcessingState; }
            internal set { SetProperty(ref isInProcessingState, value); }
        }
        #endregion IsInProcessingState

        #region IsInReadTiffLayerState
        private bool isInReadTiffLayerState;
        public bool IsInReadTiffLayerState
        {
            get { return isInReadTiffLayerState; }
            internal set { SetProperty(ref isInReadTiffLayerState, value); }
        }
        #endregion IsInReadTiffLayerState

        #region IsInReadyState
        private bool isInReadyState;
        public bool IsInReadyState
        {
            get { return isInReadyState; }
            internal set { SetProperty(ref isInReadyState, value); }
        }
        #endregion IsInReadyState

        #region IsInReviewProcState
        private bool isInReviewProcState;
        public bool IsInReviewProcState
        {
            get { return isInReviewProcState; }
            internal set { SetProperty(ref isInReviewProcState, value); }
        }
        #endregion IsInReviewProcState

        #region IsInSetEnvironmentState
        private bool isInSetEnvironmentState;
        public bool IsInSetEnvironmentState
        {
            get { return isInSetEnvironmentState; }
            internal set { SetProperty(ref isInSetEnvironmentState, value); }
        }
        #endregion IsInSetEnvironmentState

        #region IsInSystemPoweredState
        private bool isInSystemPoweredState;
        public bool IsInSystemPoweredState
        {
            get { return isInSystemPoweredState; }
            internal set { SetProperty(ref isInSystemPoweredState, value); }
        }
        #endregion IsInSystemPoweredState


        #endregion State indicators ("IsIn...")

        #region States Actions
        
        /// <summary>
        /// AbortingState's Action
        /// </summary>
        public virtual async Task HaltAllMotionAsync() { await Task.CompletedTask; }

        /// <summary>
        /// AcquisitionState's Action
        /// </summary>
        public virtual async Task AcquireImagesAsync() { await Task.CompletedTask; }

        /// <summary>
        /// CameraViewActiveState's Action
        /// </summary>
        public virtual async Task EnableCamViewActiveFeatures() { await Task.CompletedTask; }

        /// <summary>
        /// CameraViewActiveState's Action
        /// </summary>
        public virtual async Task StartAcquisition() { await Task.CompletedTask; }

        /// <summary>
        /// CameraViewActiveState's Action
        /// </summary>        
        public virtual async Task DisableCamViewActiveFeatures() { await Task.CompletedTask; }

        /// <summary>
        /// CameraViewActiveState's Action
        /// </summary>        
        public virtual async Task StopAcquisition() { await Task.CompletedTask; }

        /// <summary>
        /// CameraViewIdleState's Action
        /// </summary>
        public virtual async Task EnableCamViewIdleFeatures() { await Task.CompletedTask; }

        /// <summary>
        /// CameraViewIdleState's Action
        /// </summary>        
        public virtual async Task DisableCamViewIdleFeatures() { await Task.CompletedTask; }

        /// <summary>
        /// CameraViewState's Action
        /// </summary>
        public virtual async Task EnableCameraViewFeatures() { await Task.CompletedTask; }

        /// <summary>
        /// CameraViewState's Action
        /// </summary>        
        public virtual async Task DisableCameraViewFeatures() { await Task.CompletedTask; }

        /// <summary>
        /// InitAfterSettingsState's Action
        /// </summary>
        public virtual async Task InitAfterSettingsAsync() { await Task.CompletedTask; }

        /// <summary>
        /// InitState's Action
        /// </summary>
        public virtual async Task InitAsync() { await Task.CompletedTask; }

        /// <summary>
        /// ManualProcessImageState's Action
        /// </summary>
        public virtual async Task ManualProcessImagesAsync() { await Task.CompletedTask; }

        /// <summary>
        /// MotionState's Action
        /// </summary>
        public virtual async Task StartNextMotionAsync() { await Task.CompletedTask; }

        /// <summary>
        /// OpenSensorState's Action
        /// </summary>
        public virtual async Task OpenAcqSensorAsync() { await Task.CompletedTask; }

        /// <summary>
        /// OperatorRequestState's Action
        /// </summary>
        public virtual async Task ShowNextOperatorPromptAsync() { await Task.CompletedTask; }

        /// <summary>
        /// ProcessingState's Action
        /// </summary>        
        public virtual async Task CloseAcqSensorAsync() { await Task.CompletedTask; }

        /// <summary>
        /// ReadTiffLayerState's Action
        /// </summary>
        public virtual async Task ReadLayeredTiffFileAsync() { await Task.CompletedTask; }

        /// <summary>
        /// ReviewProcState's Action
        /// </summary>
        public virtual async Task ReviewProcessedDataAsync() { await Task.CompletedTask; }


        #endregion States Actions

	    #region Transitions Actions
        /// <summary>
        /// IsBatchModeOnChoiceTOReviewProcTransition:
        /// This method is called when the 'IsBatchModeOnChoiceState' transitions to 'ReviewProcState'
        /// triggered by ''trigger is missing'' 
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>Nothing</returns>
        public virtual async Task DisplayBatchOn(NSFStateMachineContext oContext)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// SystemPoweredTOFaultParentTransitionBYSettingsFaultEvent:
        /// This method is called when the 'SystemPoweredState' transitions to 'FaultParentState'
        /// triggered by 'SettingsFaultEvent' 
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>Nothing</returns>
        public virtual async Task LatchSettingsFaultAlarm(NSFStateMachineContext oContext)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// SystemPoweredTOFaultParentTransitionBYCameraFaultEvent:
        /// This method is called when the 'SystemPoweredState' transitions to 'FaultParentState'
        /// triggered by 'CameraFaultEvent' 
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>Nothing</returns>
        public virtual async Task LatchCameraFaultAlarm(NSFStateMachineContext oContext)
        {
            await Task.CompletedTask;
        }


        /// <summary>
        /// ReviewProcTOReadTiffLayerTransitionBYLoadLayerTiffEvent:
        /// This method is called when the 'ReviewProcState' transitions to 'ReadTiffLayerState'
        /// triggered by 'LoadLayerTiffEvent' 
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>Nothing</returns>
        public virtual async Task ClearReviewedData(NSFStateMachineContext oContext)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// ReviewProcTOReviewProcTransitionBYReprocBatchEvent:
        /// This method is called when the 'ReviewProcState' transitions to 'ReviewProcState'
        /// triggered by 'ReprocBatchEvent' 
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>Nothing</returns>
        public virtual async Task UpdateProcDataCacheAsync(NSFStateMachineContext oContext)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// IdleTOManualProcessImageTransitionBYLoadSingleFileEvent:
        /// This method is called when the 'IdleState' transitions to 'ManualProcessImageState'
        /// triggered by 'LoadSingleFileEvent' 
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>Nothing</returns>
        public virtual async Task BatchModeOff(NSFStateMachineContext oContext)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// IdleTOManualProcessImageTransitionBYLoadBatchFileEvent:
        /// This method is called when the 'IdleState' transitions to 'ManualProcessImageState'
        /// triggered by 'LoadBatchFileEvent' 
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>Nothing</returns>
        public virtual async Task BatchModeOn(NSFStateMachineContext oContext)
        {
            await Task.CompletedTask;
        }

		#endregion Transitions Actions

	    #region Transitions Guards
        // ReSharper disable once InconsistentNaming
        private bool isBatchModeOn;
        /// <summary>
        /// A Guard function returning the State of 'IsBatchModeOn'.
        /// Defined at Transition IsBatchModeOnChoiceTOReviewProcTransition
        /// </summary>
        /// <returns>guard result</returns>
        // ReSharper disable once InconsistentNaming
        public virtual bool IsBatchModeOn
        {
            get {
                    ms_iLogger.Log(ELogLevel.Info, "'IsBatchModeOn' not initialized");
                    throw new NotImplementedException(); 
                }
                set { isBatchModeOn = value; }
    }

        // ReSharper disable once InconsistentNaming
        private bool systemPoweredTOInitTransitionBYInitEventIsAdminLoginGuard;
        /// <summary>
        /// A Guard function returning the State of 'SystemPoweredTOInitTransitionBYInitEventIsAdminLoginGuard'.
        /// Defined at Transition SystemPoweredTOInitTransitionBYInitEvent
        /// </summary>
        /// <returns>guard result</returns>
        // ReSharper disable once InconsistentNaming
        public virtual bool SystemPoweredTOInitTransitionBYInitEventIsAdminLoginGuard
        {
            get {
                    ms_iLogger.Log(ELogLevel.Info, "'SystemPoweredTOInitTransitionBYInitEventIsAdminLoginGuard' not initialized");
                    throw new NotImplementedException(); 
                }
                set { systemPoweredTOInitTransitionBYInitEventIsAdminLoginGuard = value; }
    }

        // ReSharper disable once InconsistentNaming
        private bool isCriticalAlarm;
        /// <summary>
        /// A Guard function returning the State of 'IsCriticalAlarm'.
        /// Defined at Transition IsCriticalAlarmChoiceTOFaultParentTransition
        /// </summary>
        /// <returns>guard result</returns>
        // ReSharper disable once InconsistentNaming
        public virtual bool IsCriticalAlarm
        {
            get {
                    ms_iLogger.Log(ELogLevel.Info, "'IsCriticalAlarm' not initialized");
                    throw new NotImplementedException(); 
                }
                set { isCriticalAlarm = value; }
    }

        // ReSharper disable once InconsistentNaming
        private bool isMoreImages;
        /// <summary>
        /// A Guard function returning the State of 'IsMoreImages'.
        /// Defined at Transition IsMoreImagesChoiceTOIsOperatorRequestChoiceTransition
        /// </summary>
        /// <returns>guard result</returns>
        // ReSharper disable once InconsistentNaming
        public virtual bool IsMoreImages
        {
            get {
                    ms_iLogger.Log(ELogLevel.Info, "'IsMoreImages' not initialized");
                    throw new NotImplementedException(); 
                }
                set { isMoreImages = value; }
    }

        // ReSharper disable once InconsistentNaming
        private bool isOperatorRequest;
        /// <summary>
        /// A Guard function returning the State of 'IsOperatorRequest'.
        /// Defined at Transition IsOperatorRequestChoiceTOOperatorRequestTransition
        /// </summary>
        /// <returns>guard result</returns>
        // ReSharper disable once InconsistentNaming
        public virtual bool IsOperatorRequest
        {
            get {
                    ms_iLogger.Log(ELogLevel.Info, "'IsOperatorRequest' not initialized");
                    throw new NotImplementedException(); 
                }
                set { isOperatorRequest = value; }
    }

        // ReSharper disable once InconsistentNaming
        private bool isMotion;
        /// <summary>
        /// A Guard function returning the State of 'IsMotion'.
        /// Defined at Transition IsMotionChoiceTOMotionTransition
        /// </summary>
        /// <returns>guard result</returns>
        // ReSharper disable once InconsistentNaming
        public virtual bool IsMotion
        {
            get {
                    ms_iLogger.Log(ELogLevel.Info, "'IsMotion' not initialized");
                    throw new NotImplementedException(); 
                }
                set { isMotion = value; }
    }

        // ReSharper disable once InconsistentNaming
        private bool isEnvSetting;
        /// <summary>
        /// A Guard function returning the State of 'IsEnvSetting'.
        /// Defined at Transition IsEnvSettingChoiceTOSetEnvironmentTransition
        /// </summary>
        /// <returns>guard result</returns>
        // ReSharper disable once InconsistentNaming
        public virtual bool IsEnvSetting
        {
            get {
                    ms_iLogger.Log(ELogLevel.Info, "'IsEnvSetting' not initialized");
                    throw new NotImplementedException(); 
                }
                set { isEnvSetting = value; }
    }

		#endregion Transitions Guards

        #region States changes handling
        public virtual async Task AtAbortingState()
        {
            //StateMachine.SendStateEvent(TriggerName.MotionCompleteEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtAcquisitionState()
        {
            //StateMachine.SendStateEvent(TriggerName.ImagesAcquiredEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtCameraViewActiveState()
        {
            //StateMachine.SendStateEvent(TriggerName.CameraFaultEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtCameraViewIdleState()
        {
            //StateMachine.SendStateEvent(TriggerName.StartAcquisitionEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtCameraViewState()
        {
            //StateMachine.SendStateEvent(TriggerName.LeaveCameraViewEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtFaultParentState()
        {
            //StateMachine.SendStateEvent(TriggerName.ClearFaultsEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtIdleState()
        {
            //StateMachine.SendStateEvent(TriggerName.LoadSingleFileEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtInitAfterSettingsState()
        {
            //StateMachine.SendStateEvent(TriggerName.InitAfterSettingsCompleteEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtInitState()
        {
            //StateMachine.SendStateEvent(TriggerName.InitCompleteEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtManualProcessImageState()
        {
            //StateMachine.SendStateEvent(TriggerName.AbortEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtMotionState()
        {
            //StateMachine.SendStateEvent(TriggerName.AbortEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtOpenSensorState()
        {
            //StateMachine.SendStateEvent(TriggerName.OpenSensorCompleteEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtOperatorRequestState()
        {
            //StateMachine.SendStateEvent(TriggerName.StartEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtProcessingState()
        {
            //StateMachine.SendStateEvent(TriggerName.AbortEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtReadTiffLayerState()
        {
            //StateMachine.SendStateEvent(TriggerName.ReadLayerTiffCompleteEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtReadyState()
        {
            //StateMachine.SendStateEvent(TriggerName.AbortEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtReviewProcState()
        {
            //StateMachine.SendStateEvent(TriggerName.StartEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtSetEnvironmentState()
        {
            //StateMachine.SendStateEvent(TriggerName.SetEnvCompleteEvent);
			await Task.CompletedTask;
        }

        public virtual async Task AtSystemPoweredState()
        {
            //StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
			await Task.CompletedTask;
        }


        #endregion States changed

        #region State changes handling
        public async void StateChangesAtState(object sender, StateChangesEventArgs eventArgs)
        {
            if (eventArgs.HandledStateName == null) return;
            EState eState;
            Enum.TryParse(eventArgs.HandledStateName, out eState);
            if (eState == EState.none) System.Diagnostics.Debugger.Break();

            switch (eState)
            {

                case EState.Aborting:
                    try { await AtAbortingState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.Acquisition:
                    try { await AtAcquisitionState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.CameraViewActive:
                    try { await AtCameraViewActiveState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.CameraViewIdle:
                    try { await AtCameraViewIdleState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.CameraView:
                    try { await AtCameraViewState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.FaultParent:
                    try { await AtFaultParentState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.Idle:
                    try { await AtIdleState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.InitAfterSettings:
                    try { await AtInitAfterSettingsState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.Init:
                    try { await AtInitState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.ManualProcessImage:
                    try { await AtManualProcessImageState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.Motion:
                    try { await AtMotionState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.OpenSensor:
                    try { await AtOpenSensorState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.OperatorRequest:
                    try { await AtOperatorRequestState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.Processing:
                    try { await AtProcessingState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.ReadTiffLayer:
                    try { await AtReadTiffLayerState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.Ready:
                    try { await AtReadyState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.ReviewProc:
                    try { await AtReviewProcState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.SetEnvironment:
                    try { await AtSetEnvironmentState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

                case EState.SystemPowered:
                    try { await AtSystemPoweredState(); }
                    catch (Exception e)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                        StateMachine.SendStateEvent(TriggerName.UndefinedFaultEvent);
                    }
                    break;

            }
        } 
        #endregion State changes handling


		#region CancellationTokenSources

        #region MainFaultCancellationTokenSource
        private CancellationTokenSource _mainFaultCancellationTokenSource;
        public CancellationTokenSource MainFaultCancellationTokenSource
        {
            get { return _mainFaultCancellationTokenSource ?? (MainFaultCancellationTokenSource = new CancellationTokenSource()); }
            set 
            { 
                _mainFaultCancellationTokenSource?.Dispose();
                SetProperty(ref _mainFaultCancellationTokenSource, value); 
            }
        }
        #endregion MainFaultCancellationTokenSource

        #region CameraFaultCancellationTokenSource
        private CancellationTokenSource _cameraFaultCancellationTokenSource;
        public CancellationTokenSource CameraFaultCancellationTokenSource
        {
            get { return _cameraFaultCancellationTokenSource ?? (CameraFaultCancellationTokenSource = new CancellationTokenSource()); }
            set 
            { 
                _cameraFaultCancellationTokenSource?.Dispose();
                SetProperty(ref _cameraFaultCancellationTokenSource, value); 
            }
        }
        #endregion CameraFaultCancellationTokenSource

        #region SettingsFaultCancellationTokenSource
        private CancellationTokenSource _settingsFaultCancellationTokenSource;
        public CancellationTokenSource SettingsFaultCancellationTokenSource
        {
            get { return _settingsFaultCancellationTokenSource ?? (SettingsFaultCancellationTokenSource = new CancellationTokenSource()); }
            set 
            { 
                _settingsFaultCancellationTokenSource?.Dispose();
                SetProperty(ref _settingsFaultCancellationTokenSource, value); 
            }
        }
        #endregion SettingsFaultCancellationTokenSource

        #region UndefinedFaultCancellationTokenSource
        private CancellationTokenSource _undefinedFaultCancellationTokenSource;
        public CancellationTokenSource UndefinedFaultCancellationTokenSource
        {
            get { return _undefinedFaultCancellationTokenSource ?? (UndefinedFaultCancellationTokenSource = new CancellationTokenSource()); }
            set 
            { 
                _undefinedFaultCancellationTokenSource?.Dispose();
                SetProperty(ref _undefinedFaultCancellationTokenSource, value); 
            }
        }
        #endregion UndefinedFaultCancellationTokenSource

		#endregion CancellationTokenSources


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual bool SetProperty<T>(ref T prevValue, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(prevValue, value)) return false;
            prevValue = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }
        #endregion INotifyPropertyChanged
    }
}