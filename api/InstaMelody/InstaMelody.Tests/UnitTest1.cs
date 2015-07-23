using System;
using System.Collections.Generic;
using InstaMelody.Data;
using InstaMelody.Model;
using InstaMelody.Business;
using InstaMelody.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InstaMelody.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod01()
        {
            var dal = new UserLoops();
            var test = dal.GetUserLoopPartById(4);
        }

    }
}
