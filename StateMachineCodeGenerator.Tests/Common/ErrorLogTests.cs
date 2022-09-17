using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateMachineCodeGenerator.Common;

namespace StateMachineCodeGenerator.Tests.Common
{
    [TestClass]
    public class ErrorLogTests
    {
        [TestMethod]
        public void InitializationTest() {
            var id = new ErrorId("123");
            var msg = "blah";
            var target = new ErrorLog(id, msg, ErrorSeverity.Error);
            Assert.AreEqual(id, target.Id);
            Assert.AreEqual(msg, target.Message);
            Assert.AreEqual(nameof(InitializationTest), target.CallingMemberName);
            Assert.AreEqual(target.Severity, target.Severity);
            Assert.AreEqual(target, ErrorLog.Errors[id]);
            Assert.AreEqual(target, ErrorLog.Errors["123"]);
        }

        [TestMethod]
        public void EditedErrorTest() {
            var id = new ErrorId("123");
            var target = new ErrorLog(id, "blah {0}", ErrorSeverity.Error);
            var editedError = ErrorLog.GetEditedErrorLog(id, new object[] { "XXX" });
            Assert.IsTrue(editedError.Message.EndsWith("XXX"));
        }
    }
}
