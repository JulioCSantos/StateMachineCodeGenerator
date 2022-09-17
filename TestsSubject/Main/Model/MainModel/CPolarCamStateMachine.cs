 
 
// Created by t4 template 'StateMachineDerivedTemplate'
///////////////////////////////////////////////////////////
// Copyright © Corning Incorporated 2017
// File CPolarCamStateMachine.cs
// Project PolarCam
// Implementation of the Class CPolarCamStateMachine
// Created on 9/5/2022 10:25:17 AM
///////////////////////////////////////////////////////////

// Quick Start Guide for those using the GenSys Easy for the first time:
// First time users will primarily be interested in three states: Ready, Idle, and Running. The Idle and Running states are sub-states of the Ready State.
// The Ready state is entered after initialization and then the State Machine proceeds to the Idle State.
// The Idle state is intended to be a "waiting" state with nothing significant happening.
// The Running state is intended to be the state where your task is accomplished.


// Here are the rules for the Ready, Idle and Running States
// The method "ReadyStateEntryActions"   is called after initializtion has completed.
// The method "ReadyStateExitActions"    is called after if a Fault occurs.
// The method "IdleStateEntryActions"    is called after either "ReadyStateEntryActions" or "RunningStateExitActions" is called.
// The method "IdleStateExitActions"     is called when the Start button is pressed.
// The method "RunningStateEntryActions" is called when the Start button is pressed but after "IdleStateExitActions" is called.
// The method "RunningStateExitActions"  is called when the Abort button is pressed.


// To start developing an Application utilizing a Start and Abort button:
// Place Initialization Code in "ReadyStateEntryActions". A call into the sytem may be needed.
// Place code to call into system to perform your task in "RunningStateEntryActions".
// If a System thread was kicked off to start a task, then place a call in "RunningStateExitActions" to call into the system to stop the thread.

// To start developing an application which doesn't require user interaction and just performs a task, place code to call into the System in "IdleStateEntryActions" and
// uncomment "m_iShield.EnableStartButton = false; "  in "IdleStateEntryActions".
// This default template has code to show how to use the Start and Abort button in conjunction with calling into the System, which uses the ThreadHandler.
// When Start is pressed a message will be display every second until one of two conditions are met:
// 20 Messages have been displayed or
// The Abort button is pressed.

// It is expected that this and the System code will be heavily modified.
// Note that the State Machine uses System to do all of the real work.
// The desired pattern is the State Machine not implementing any Application code.

// The method CreateOperatorPrompts() creates GUI prompts based on the state of the State Machine.
// As the State Machine state changes, so does the GUI prompt.
// The following two statement are included in the code as part of the Quick Start Guide and may be tailored for your application.
// m_odictOperatorPromptForState.Add(oIdleState, "Hit Start to Start Running");
// m_odictOperatorPromptForState.Add(oRunningState, "Hit Abort to Stop Running");

using System;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using Corning.GenSys.Logger;
using NorthStateSoftware.NorthStateFramework;
using System.Threading.Tasks;
using System.Linq;
using Corning.GenSys.Scanning;

namespace PolarCam.Model
{
    // ReSharper disable once InconsistentNaming
    public class CPolarCamStateMachine : CPolarCamStateMachineBase
    {
        internal CPolarCamStateMachine(string strName, CPolarCamModel mainModel) 
            : base(strName, mainModel) { }

        public override void BeforeCreateStateMachine()
        {
            //OperatorPromptForStateDict.Add(EState.Idle,"get the ball rolling"); //example
        }

        public override void AfterCreateStateMachine()
        {
        }
    }
}