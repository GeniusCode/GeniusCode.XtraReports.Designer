Feature: Making paths to external subreports relative
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@mytag
Scenario: Selecting report file in the same directory for subreport
	Given Infrastructure is initialized
	And reportA file exists in a folder
	And reportB file exists in the same folder as reportA
	And the container on ReportA contains the full path to ReportB
	When the report is saved
	Then the url on the container should be a relative path, and not absolute

Scenario: Selecting report file in a subdirectory for subreport
	Given Infrastructure is initialized
	And reportA file exists in a folder
	And reportB file exists in a subdirectory of reportA's path
	And the container on ReportA contains the full path to ReportB
	When the report is saved
	Then the url on the container should be a relative path, and not absolute

Scenario: Selecting report file out of directory structure for subreport
	Given Infrastructure is initialized 
	And reportA file exists in a folder
	And reportB file exists outside of reportA's directory structure
	And the container on ReportA contains the full path to ReportB
	When the report is saved
	Then an exception should be thrown
	And container's url should not be relative
