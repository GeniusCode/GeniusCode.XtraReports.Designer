using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using GeniusCode.XtraReports.Designer.Support;
using Microsoft.Win32;
using NUnit.Framework;

namespace GeniusCode.XtraReports.Designer.Tests.Unit
{
    [TestFixture]
    public class RegistryHelper_tests
    {
        [Test]
        public void Should_create_new_registry_key()
        {
            const string subKey = "SOFTWARE\\gcXtraReports.Designer";
            try
            {
                Registry.CurrentUser.DeleteSubKey(subKey);
            }
            catch
            {
            }


            var helper = new RegistryHelper(subKey);
            var value = helper.AcquireRootPath("Hello, world");

            value.Should().Be("Hello, world");

            var key = Registry.CurrentUser.OpenSubKey(subKey); 
            key.Should().NotBeNull();
            key.GetValue("ProjectRootPath").Should().Be("Hello, world");
        }

        [Test]
        public void Should_use_existing_registry_key()
        {
            const string subKey = "SOFTWARE\\gcXtraReports.Designer";

            var helper = new RegistryHelper(subKey);
            var value = helper.AcquireRootPath("Hello, world");

            var value2 = helper.AcquireRootPath("second time around");

            value.Should().Be(value2);

        }
    }
}
