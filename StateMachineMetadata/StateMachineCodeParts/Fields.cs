using StateMachineMetadata.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineMetadata.StateMachineCodeParts
{
    public class Fields : IToCSharp
    {
        private MainModel members { get; set; }

        private IEnumerable<string> fields
        {
            get
            {
                var hdrFields = new string[] {
@"        #region Fields",
$"        private static ILogger ms_iLogger = CLoggerFactory.CreateLog(\"{members.StateMachineTypeName}\");",
@"        //private Semaphore m_semaphoreStateChange = new Semaphore();",
@"        private AutoResetEvent m_autoreseteventStateChange = new AutoResetEvent(false);",
@"        private object m_objLock = new object();",
 "        private string m_strLastOperatorPrompt = \"\";",
@"",
@"        private EState? m_estatePrevious;",
@"        private EState? m_estateCurrent;",
@"        private string m_strSystemState;",
$"        private I{members.SystemTypeName} m_i{members.SystemTypeName};",
@"",
@"        private bool m_bSimulationMode = false;",
$"        private Dictionary<{members.SystemTypeName}EventsEnum, NSFEvent> m_dictEventByEnum = new Dictionary<Proj3SystemEventsEnum, NSFEvent>();",
@"        private Dictionary<NSFState, string> m_odictOperatorPromptForState;",
@"",
@"        private bool m_bInitComplete = false;",
@"        private bool m_bInitAfterSettingsComplete = false;",
@"",
@"        #region State Machine Fields",
@""     }.ToList();
                
                var tlrFields = new string[] {
@"",
@"        #endregion State Machine Fields",
@"",
@"        #endregion Fields"
        };
                hdrFields.AddRange((new StateMachineFields()).ToCSharp(members));
                hdrFields.AddRange(tlrFields);
                return hdrFields;
            }
        }
        public IEnumerable<string> ToCSharp(MainModel model)
        {
            members = model;
            return fields;
        }
    }
}
