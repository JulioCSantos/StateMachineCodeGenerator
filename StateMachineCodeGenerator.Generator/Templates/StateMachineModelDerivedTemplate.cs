﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace StateMachineCodeGenerator.Templates
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using StateMachineMetadata.Model;
    using StateMachineMetadata.Extensions;
    using StateMachineCodeGeneratorSystem.Templates;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class StateMachineModelDerivedTemplate : StateMachineModelDerivedTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write(" \r\n");
            this.Write(" \r\n");
            this.Write("// Created by t4 template \'");
            
            #line 9 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetType().Name));
            
            #line default
            #line hidden
            this.Write("\'\r\n");
            this.Write("///////////////////////////////////////////////////////////\r\n// Copyright © Corni" +
                    "ng Incorporated 2017\r\n// File ");
            
            #line 8 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\Parts/CopyrightHeader.t4"
            this.Write(this.ToStringHelper.ToStringWithCulture(Session["fileName"].ToString()));
            
            #line default
            #line hidden
            this.Write("\r\n// Project ");
            
            #line 9 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\Parts/CopyrightHeader.t4"
            this.Write(this.ToStringHelper.ToStringWithCulture(Session["projectName"].ToString()));
            
            #line default
            #line hidden
            this.Write("\r\n// Implementation of the Class ");
            
            #line 10 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\Parts/CopyrightHeader.t4"
            this.Write(this.ToStringHelper.ToStringWithCulture(Session["typeName"].ToString()));
            
            #line default
            #line hidden
            this.Write("\r\n// Created on ");
            
            #line 11 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\Parts/CopyrightHeader.t4"
            this.Write(this.ToStringHelper.ToStringWithCulture(DateTime.Now.ToLocalTime().ToString()));
            
            #line default
            #line hidden
            this.Write("\r\n///////////////////////////////////////////////////////////");
            this.Write("\r\n");
            
            #line 11 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
 var model = StateMachineMetadata.Main.ActiveModel; 
            
            #line default
            #line hidden
            this.Write(@"using GenSysCommon.Interfaces;
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


namespace ");
            
            #line 25 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Session["NameSpace"].ToString()));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    // ReSharper disable once InconsistentNaming\r\n    public partial class ");
            
            #line 28 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Session["typeName"].ToString()));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 28 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Session["StateMachineModelBaseTypeName"].ToString()));
            
            #line default
            #line hidden
            this.Write(", IDisposable\r\n    {\r\n\r\n        #region properties\r\n\r\n        #region EventIds\r\n");
            
            #line 34 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
   foreach (var faultTriggerName in model.NSF.GetFaultTriggerNames())  { 
            
            #line default
            #line hidden
            
            #line 35 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
   var faultName = faultTriggerName.Replace("Fault", "").ToValidCSharpName(); 
            
            #line default
            #line hidden
            this.Write("        public override string ");
            
            #line 36 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(faultName));
            
            #line default
            #line hidden
            this.Write("FaultEventId { get { return nameof(EventBroker.");
            
            #line 36 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(faultName));
            
            #line default
            #line hidden
            this.Write("FaultEventId); } } \r\n");
            
            #line 37 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("        #endregion EventIds\r\n\r\n        #endregion properties\r\n\r\n        #region C" +
                    "onstructors\r\n\r\n        #region Singleton        \r\n        private static readonl" +
                    "y object SingletonLock = new object();\r\n        private static ");
            
            #line 46 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Session["StateMachineModelDerivedTypeName"].ToString()));
            
            #line default
            #line hidden
            this.Write(" mainModel;\r\n        public static ");
            
            #line 47 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Session["StateMachineModelDerivedTypeName"].ToString()));
            
            #line default
            #line hidden
            this.Write(" GetSingleton()\r\n        {\r\n                if (mainModel != null) return mainMod" +
                    "el;\r\n                lock (SingletonLock)\r\n                {\r\n                  " +
                    "  return mainModel ?? (mainModel = new ");
            
            #line 52 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Session["StateMachineModelDerivedTypeName"].ToString()));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 52 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Session["StateMachineBaseTypeName"].ToString()));
            
            #line default
            #line hidden
            this.Write(".StateMachineName));\r\n                }\r\n        }\r\n        #endregion Singleton\r" +
                    "\n\r\n        private ");
            
            #line 57 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Session["typeName"].ToString()));
            
            #line default
            #line hidden
            this.Write("(string strName)\r\n        {\r\n            base.StateMachine = new ");
            
            #line 59 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Session["StateMachineDerivedTypeName"].ToString()));
            
            #line default
            #line hidden
            this.Write("(strName, this);\r\n            base.RegisterFaultEventMethods();\r\n            this" +
                    ".PropertyChanged += ");
            
            #line 61 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Session["StateMachineModelDerivedTypeName"].ToString()));
            
            #line default
            #line hidden
            this.Write("_PropertyChanged;\r\n        }\r\n        #endregion Constructors\r\n\r\n        #region " +
                    "methods\r\n            private void ");
            
            #line 66 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Session["StateMachineModelDerivedTypeName"].ToString()));
            
            #line default
            #line hidden
            this.Write(@"_PropertyChanged(object sender, PropertyChangedEventArgs e) {
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

        ~");
            
            #line 101 "C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineCodeGenerator.Generator\Templates\StateMachineModelDerivedTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Session["StateMachineModelDerivedTypeName"].ToString()));
            
            #line default
            #line hidden
            this.Write("() { Dispose(false); }\r\n        #endregion IDisposable\r\n\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public class StateMachineModelDerivedTemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
