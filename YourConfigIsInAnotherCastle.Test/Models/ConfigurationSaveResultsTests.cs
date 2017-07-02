using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YourConfigIsInAnotherCastle.Models;
using YourConfigIsInAnotherCastle.Storage;
using DeepEqual;
using DeepEqual.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace YourConfigIsInAnotherCastle.Test.Models
{
    [TestClass]
    public class ConfigurationSaveResultsTests
    {

        [TestMethod]
        public void ConfigurationSaveResults_AddErrors_ErrorsAddedFailureState()
        {
            ConfigurationSaveResults results = new ConfigurationSaveResults();
            results.AddError("Test", DataStorageError.AlreadyExists);

            Assert.IsFalse(results.Successful, "Should fail due to error");
            Assert.AreEqual(results.Errors.Count, 1, "Should only have one errors added");
            results.Errors[0].WithDeepEqual(new ErrorMap() { Code = DataStorageError.AlreadyExists, Message = "Test" }).Assert();
        }

        [TestMethod]
        public void ConfigurationSaveResults_AddErrorsChained_ErrorsAddedFailureState()
        {
            ConfigurationSaveResults results = new ConfigurationSaveResults();
            results.AddError("Test1", DataStorageError.AlreadyExists).AddError("Test2", DataStorageError.RecordNotFound);

            Assert.IsFalse(results.Successful, "Should fail due to errors");
            Assert.AreEqual(results.Errors.Count, 2, "Should only have two errors added");
            results.Errors.OrderBy(r => r.Message).WithDeepEqual(new List<ErrorMap>() {
                new ErrorMap() { Code = DataStorageError.AlreadyExists, Message = "Test1" },
                new ErrorMap() { Code = DataStorageError.RecordNotFound, Message = "Test2" }
            }.OrderBy(r => r.Message)).Assert();

        }
        [TestMethod]
        public void ConfigurationSaveResults_NoErrors_SuccessfulState()
        {
            ConfigurationSaveResults results = new ConfigurationSaveResults();

            Assert.IsTrue(results.Successful, "Should pass due to no errorsw");
        }
    }
}
