Feature: Passing datasources at design time
	In order simplify report creation
	As a user
	I want a default root folder to be created for me automatically in my Documents

@mytag
Scenario: Passing Datasource Using Traversal
	Given The design runtime is ready
	And a datasource exists called DogTime
	And PersonReport exists with a subreport called DogReport in a detail report
	And PersonReport loads the DogTime datasource
	And a new report instance exists
	When A ReportActivatedBySubreportMessage occurs which contains the new report instance
	Then the new report instance's datasource should be the first dog of the first person from PersonReport's datasource

Scenario: Passing Datasource Using 2-Nested Subreports
	Given The design runtime is ready
	And a datasource exists called DogTime
	And ReportA exists with a subreport called ReportB in a detail report
	And ReportB exists with a subreport called ReportC in a detail report
	And ReportA loads the DogTime datasource
	And the user has activated subreport ReportB inside ReportA
	When the user activates subreport ReportC inside ReportB
	Then ReportC's datasource should be the first Toy of the first Dog of the first Person in DogTime

Scenario: Activating a Subreport without a Datasource
	Given The design runtime is ready
	And ReportA exists with a subreport called ReportB in a detail report
	When the user activates subreport ReportB inside ReportA without a datasource
	Then ReportB should open without a datasource