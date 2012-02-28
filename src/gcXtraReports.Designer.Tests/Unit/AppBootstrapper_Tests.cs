using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using GeniusCode.XtraReports.Designer.Support;
using NUnit.Framework;

namespace XtraSubReports.Winforms.Tests.Unit
{
    


    [TestFixture]
    public class AppBootstrapper_Tests
    {

        public class TestAppBootStrapper : AppBootStrapper
        {
            public TestAppBootStrapper(string defaultPath) : base(new TestPathAcquirer(), defaultPath)
            {
            }
        }

        public class TestPathAcquirer : IRootPathAcquirer
        {
            public string AcquireRootPath(string defaultValue)
            {
                //this is totally the wrong behavior, but the assumption
                //with these tests is that the value for the root path will not be changed
                return defaultValue;
            }
        }


        [Test]
        public void Should_auto_create_folder()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.Exists(tempPath).Should().BeFalse("Test is in invalid state, cannot proceed");
            var bootStrapper = new TestAppBootStrapper(tempPath);
            bootStrapper.CreateRootPathIfNeeded();
            Directory.Exists(tempPath).Should().BeTrue("AppBootStrapper did not create a temp path as expected");
        }

        [Test]
        public void Should_detect_when_no_projects_exist()
        {
            //Given that the path already exists;
            var tempPath = GetNewEmptyPathThatExists();
            var bs = new TestAppBootStrapper(tempPath);
            bs.DetectProjectMode().Should().Be(AppProjectsStructureMode.None);
        }

        [Test]
        public void Should_detect_when_single_projects_exist()
        {
            //Given that the path already exists;
            var tempPath = GetNewEmptyPathThatExists();
            // make project folder

            var projectPath = Path.Combine(tempPath, "MyProject");
            Directory.CreateDirectory(projectPath);
            var bs = new TestAppBootStrapper(tempPath);
            bs.DetectProjectMode().Should().Be(AppProjectsStructureMode.Single);
            bs.SetProjectNameToSingle();

            bs.GetProjectBootstrapper("Reports", "Datasources", "Actions").ProjectPath.Should().Be(projectPath);
        }


        [Test]
        public void Should_detect_when_multiple_projects_exist_with_correct_count_when_three()
        {
            var tempPath = GetPathWithThreeProjects();
            var bs = new TestAppBootStrapper(tempPath);

            bs.DetectProjectMode().Should().Be(AppProjectsStructureMode.MultipleUnchosen);
            bs.GetProjects().Count().Should().Be(3);
        }


        [Test]
        public void Should_detect_when_multiple_projects_exist_with_correct_count_when_three_and_chosen()
        {
            var tempPath = GetPathWithThreeProjects();
            var bs = new TestAppBootStrapper(tempPath);

            bs.SetProjectName("Project1");
            bs.DetectProjectMode().Should().Be(AppProjectsStructureMode.MultipleChosen);
        }

        [Test]
        public void Should_detect_when_multiple_projects_exist_with_correct_count_when_three_and_chosen_but_doesnt_exist()
        {
            var tempPath = GetPathWithThreeProjects();
            var bs = new TestAppBootStrapper(tempPath);
            Exception exception = null;

            try
            {
                bs.SetProjectName("Project45");
            }
            catch (Exception ex)
            {
                exception = ex;
            }


            exception.Should().NotBeNull();
        }

        private static string GetPathWithThreeProjects()
        {
            var tempPath = GetNewEmptyPathThatExists();
            Directory.CreateDirectory(Path.Combine(tempPath, "Project1"));
            Directory.CreateDirectory(Path.Combine(tempPath, "Project2"));
            Directory.CreateDirectory(Path.Combine(tempPath, "Project3"));
            return tempPath;
        }

        [Test]
        public void Should_detect_when_multiple_projects_exist_with_correct_count_when_one()
        {
            var tempPath = GetNewEmptyPathThatExists();
            Directory.CreateDirectory(Path.Combine(tempPath, "Project1"));
            var bs = new TestAppBootStrapper(tempPath);

            bs.DetectProjectMode().Should().Be(AppProjectsStructureMode.Single);
            bs.GetProjects().Count().Should().Be(1);
        }


        private string GetNewEmptyPathThatDoesntExists()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            return tempPath;
        }

        private static string GetNewEmptyPathThatExists()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);
            return tempPath;
        }
    }
}
