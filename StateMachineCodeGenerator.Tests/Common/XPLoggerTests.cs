using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateMachineCodeGenerator.Common;
using StateMachineCodeGenerator.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.Tests.Common
{
    [TestClass]
    public class XPLoggerTests
    {
        [TestMethod]
        public void InstantiationTest() {
            var target = XPLogger.GetInstance();
            Assert.AreEqual(XPLogger.Instance, target);
        }

        [TestMethod]
        public void Instantiation2Test() {
            var target1 = XPLogger.GetInstance();
            var target2 = XPLogger.GetInstance();
            Assert.AreEqual(target2, target1);
        }


        [TestMethod]
        public void Instantiation3Test() {
            var target1 = XPLogger.GetInstance();
            var target2 = XPLogger.GetInstance("New Instance");
            Assert.AreNotEqual(target2, target1);
            Assert.AreEqual(target2, XPLogger.Instance);
            XPLogger.ActiveLogger = target1;
            Assert.AreEqual(target1, XPLogger.Instance);
        }

        [TestMethod]
        public void AddErrorTest1() {
            var target = XPLogger.GetInstance();
            Assert.AreEqual(XPLogger.Instance, target);
            Assert.IsFalse(target.ActiveErrors.Any());
            var err = new ErrorLog("123", "Blabh blah", ErrorSeverity.Info);
            target.AddError(err);
            Assert.IsTrue(target.ActiveErrors.Any());
            Assert.AreEqual("123",target.ActiveErrors.First().Id.ToString());
        }

        [TestMethod]
        public void AddErrorTest2() {
            var target = XPLogger.GetInstance("AddErrorTest2");
            Assert.AreEqual(XPLogger.Instance, target);
            Assert.IsFalse(target.ActiveErrors.Any());
            var err = new ErrorLog("123", "Blabh blah", ErrorSeverity.Info);
            target.AddError(err);
            var err1 = new ErrorLog("234", "Blabh blah", ErrorSeverity.Warning);
            target.AddError(err1);
            Assert.AreEqual(2,target.ActiveErrors.Count);
            Assert.AreEqual("234", target.ActiveErrors.Skip(1).First().Id.ToString());
            Assert.IsTrue(target.ActiveErrors.Any(e => e.Id == "234"));
        }

        [TestMethod]
        public void ClearTest() {
            var target = XPLogger.GetInstance("ClearTest");
            Assert.AreEqual(XPLogger.Instance, target);
            Assert.IsFalse(target.ActiveErrors.Any());
            var err = new ErrorLog("123", "Blabh blah", ErrorSeverity.Info);
            target.AddError(err);
            var err1 = new ErrorLog("234", "Blabh blah", ErrorSeverity.Warning);
            target.AddError(err1);
            Assert.AreEqual(2, target.ActiveErrors.Count);
            target.ClearErrors();
            Assert.AreEqual(0, target.ActiveErrors.Count);
        }
    }
}
