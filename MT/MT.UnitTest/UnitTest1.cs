﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using MT.Common.ConfigHelper;
using System.Collections.Generic;

namespace MT.UnitTest
{

    public class TestModel
    {
        public TestModel() { }
        public int ID { get; set; }

        public string Name { get; set; }


    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestGetConfigurationValue()
        {
            string data = ConfigurationHelper.GetConfigurationValue<string>("test.json", "data", ConfigurationType.JSON);
            Assert.AreEqual(data, "test");  
            int TestModel_ID = ConfigurationHelper.GetConfigurationValue<int>("test.json", "TestModel:ID", ConfigurationType.JSON); 
            string TestModel_Name = ConfigurationHelper.GetConfigurationValue<string>("test.json", "TestModel:Name", ConfigurationType.JSON);
            TestModel TestModel = ConfigurationHelper.GetConfigurationValue<TestModel>("test.json", "TestModel", ConfigurationType.JSON); 
            Assert.AreEqual(TestModel_ID, 1);
            Assert.AreEqual(TestModel_Name, "张三");

            Assert.AreEqual(TestModel.ID, 1);
            Assert.AreEqual(TestModel.Name, "张三");

            data = ConfigurationHelper.GetConfigurationValue<string>("test.xml", "data", ConfigurationType.XML);
            Assert.AreEqual(data, "test"); 
            TestModel_ID = ConfigurationHelper.GetConfigurationValue<int>("test.xml", "TestModel:ID", ConfigurationType.XML); 
            TestModel_Name = ConfigurationHelper.GetConfigurationValue<string>("test.xml", "TestModel:Name", ConfigurationType.XML); 
        //    TestModel = ConfigurationHelper.GetConfigurationValue<TestModel>("test.xml", "TestModel", ConfigurationType.XML);
            Assert.AreEqual(TestModel_ID, 1);
            Assert.AreEqual(TestModel_Name, "张三");
            // Assert.AreEqual(TestModel, new TestModel { ID = 1, Name = "张三" });


            data = ConfigurationHelper.GetConfigurationValue<string>("test.ini", "data", ConfigurationType.INI);
            Assert.AreEqual(data, "test");
            TestModel_ID = ConfigurationHelper.GetConfigurationValue<int>("test.ini", "TestModel:ID", ConfigurationType.INI);
            TestModel_Name = ConfigurationHelper.GetConfigurationValue<string>("test.ini", "TestModel:Name", ConfigurationType.INI);
       //     TestModel = ConfigurationHelper.GetConfigurationValue<TestModel>("test.ini", "TestModel", ConfigurationType.INI);
            Assert.AreEqual(TestModel_ID, 1);
            Assert.AreEqual(TestModel_Name, "张三");
            // Assert.AreEqual(TestModel, new TestModel { ID = 1, Name = "张三" });

            List<KeyValuePair<string, string>> Data = new List<KeyValuePair<string, string>>();
            Data.Add(new KeyValuePair<string, string>("TestModel:ID","1"));
            Data.Add(new KeyValuePair<string, string>("TestModel:Name", "张三"));
            ConfigurationHelper.SetMemoryConfiguration(Data);
        //    TestModel = ConfigurationHelper.GetConfigurationValue<TestModel>(null, "TestModel", ConfigurationType.Memory);
            TestModel_ID = ConfigurationHelper.GetConfigurationValue<int>(null, "TestModel:ID", ConfigurationType.Memory);
            TestModel_Name = ConfigurationHelper.GetConfigurationValue<string>(null, "TestModel:Name", ConfigurationType.Memory);

            Assert.AreEqual(TestModel_ID, 1);
            Assert.AreEqual(TestModel_Name, "张三");
            // Assert.AreEqual(TestModel, new TestModel { ID = 1, Name = "张三" });

        }

        [TestMethod]
        public void TestGetConfiguration()
        {
            TestModel TestModel = ConfigurationHelper.GetConfiguration<TestModel>("testModel.json", ConfigurationType.JSON); 
            Assert.AreEqual(TestModel.ID, 1);
            Assert.AreEqual(TestModel.Name, "张三");

            TestModel = ConfigurationHelper.GetConfiguration<TestModel>("testModel.xml", ConfigurationType.XML);
            Assert.AreEqual(TestModel.ID, 1);
            Assert.AreEqual(TestModel.Name, "张三");


            TestModel = ConfigurationHelper.GetConfiguration<TestModel>("testModel.ini", ConfigurationType.INI);
            Assert.AreEqual(TestModel.ID, 1);
            Assert.AreEqual(TestModel.Name, "张三");

            List<KeyValuePair<string, string>> Data = new List<KeyValuePair<string, string>>();
            Data.Add(new KeyValuePair<string, string>("ID", "1"));
            Data.Add(new KeyValuePair<string, string>("Name", "张三"));
            ConfigurationHelper.SetMemoryConfiguration(Data);

            TestModel = ConfigurationHelper.GetConfiguration<TestModel>(null, ConfigurationType.Memory);
            Assert.AreEqual(TestModel.ID, 1);
            Assert.AreEqual(TestModel.Name, "张三");


        }
    }
}
