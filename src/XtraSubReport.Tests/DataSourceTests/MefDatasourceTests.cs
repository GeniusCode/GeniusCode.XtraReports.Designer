﻿using System.Collections;
using System.Linq;
using NUnit.Framework;
using XtraSubreportEngine;

namespace XtraSubReport.Tests
{
    [TestFixture]
    public class MefDatasourceTests
    {
        DataSourceLocator locator;

        [SetUp]
        public void Init()
        {
            locator = new DataSourceLocator(string.Empty);
        }

        [Test]
        public void catalog_should_not_be_empty()
        {
            var exports = locator.GetDatasources(string.Empty).ToList();

            Assert.IsTrue(exports.Count > 0);
        }

        [Test]
        public void Northwind_Orders_should_be_exported()
        {
            var match = locator.GetDatasource(TestHelper.NorthwindDataSource);

            Assert.IsNotNull(match);
        }

        [Test]
        public void Northwind_Orders_should_contain_data()
        {
            var export = locator.GetDatasource(TestHelper.NorthwindDataSource);

            var exportInstance = export.Value;
            Assert.IsNotNull(exportInstance);

            var datasource = exportInstance.GetDataSource();
            Assert.IsNotNull(datasource);

            var collection = (datasource as IEnumerable).Cast<object>().ToList();
            Assert.AreNotEqual(0, collection.Count);
        }

        //[Test]
        //public void catalog_should_support_multiple_directories()
        //{
        //    //DesignTimeHelper.BuildDesignTimeDataSourceTreeItems(new MyReportBase());
        //    //var container = DataSourceProvider.Singleton._container;
        //}
    }
}
