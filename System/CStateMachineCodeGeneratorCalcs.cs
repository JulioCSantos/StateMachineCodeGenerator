///////////////////////////////////////////////////////////
//  Copyright © Corning Incorporated  2016
//  CStateMachineCodeGeneratorCalcs.cs
//  Project StateMachineCodeGeneratorSystem
//  Implementation of the Class CStateMachineCodeGeneratorCalcs
//  Created on:      November 23, 2016 5:36:03 AM
///////////////////////////////////////////////////////////

using System;

using Corning.GenSys.Logger;
using Corning.GenSys.SettingAttribute;

using StateMachineCodeGenerator.Interfaces;

namespace StateMachineCodeGenerator.StateMachineCodeGeneratorSystem
{
    internal class CStateMachineCodeGeneratorCalcs
    {
        #region Enums
        #endregion Enums

        #region Fields
        private static IStateMachineCodeGeneratorSystem m_iStateMachineCodeGeneratorSystem;

        [ASetting(Name = "SampleData1", Description = "", Units = "???", ReadAccess = EAccess.Admin | EAccess.Engineer)]
        private float m_fSampleData1 = float.NaN;

        [ASetting(Name = "SampleData2", Description = "", Units = "???", ReadAccess = EAccess.Admin | EAccess.Engineer, InstanceNameOverride = "Overrides.CalcOverride")]
        private float m_fSampleData2 = float.NaN;

        #endregion Fields

        #region Properties
        #endregion Properties

        #region Events
        #endregion Events

        #region Constructors
        public CStateMachineCodeGeneratorCalcs(float fSampleData1, float fSampleData2, IStateMachineCodeGeneratorSystem iStateMachineCodeGeneratorSystem)
        {
            m_fSampleData1 = fSampleData1;
            m_fSampleData2 = fSampleData2;
            m_iStateMachineCodeGeneratorSystem = iStateMachineCodeGeneratorSystem;
        }

        public CStateMachineCodeGeneratorCalcs(IStateMachineCodeGeneratorSystem iStateMachineCodeGeneratorSystem)
        {
            m_iStateMachineCodeGeneratorSystem = iStateMachineCodeGeneratorSystem;
        }

        ~CStateMachineCodeGeneratorCalcs()
        {
        }
        #endregion Constructors

        #region Methods
        public void CalcStateMachineCodeGeneratorData(ref IStateMachineCodeGeneratorData iStateMachineCodeGeneratorData, ILogger iLogger)
        // Acronym used (violating Coding Standard) purposefully to shorten calculation lines for better clarity
        {
            try
            {
                //TODO: Perform calculation on iStateMachineCodeGeneratorData
            }
            catch (Exception ex)
            {
                iLogger.LogException(ELogLevel.Error, "Calculation Error!", ex);
            }
        }
        #endregion Methods

        #region InnerClasses
        #endregion InnerClasses
    }
}
