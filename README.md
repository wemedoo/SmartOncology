# sReports

# Setup SonarQube in local

1. Download SonarQube:
    Download the latest version of SonarQube from the official SonarQube site: https://www.sonarsource.com/products/sonarqube/downloads/success-download-community-edition/
	
2. Download Java 11 or Java 17 from the AdoptOpenJDK or Oracle website (if you don’t have it already)
	https://www.oracle.com/java/technologies/javase/jdk17-archive-downloads.html
	Download ‘Windows x64 Installer’ version

3. Set java Environment Variables
		1.	Open the Environment Variables settings
		2.	Add a new system variable named JAVA_HOME with the path to your Java installation (e.g., C:\Program Files\Java\jdk-11).
		3.	Update the Path variable to include %JAVA_HOME%\bin.
	We can follow instructions from youtube: https://www.youtube.com/watch?v=mg9jJr2_2Oo

4. Start SonarQube Server:
	Navigate to the SonarQube directory and start the server and Run StartSonar.bat in the bin/windows-x86-64 folder

5. Access SonarQube Dashboard:
	-	Open a browser and go to http://localhost:9000. The default login credentials are:
		o	Username: admin
		o	Password: admin
	-	Change the password after logging in for the first time for security purposes.
	
6. Install SonarScanner for .NET (if you don’t have it already)
	•	Install the .NET Core Global Tool: dotnet tool install --global dotnet-sonarscanner
	•	Verify Installation: dotnet-sonarscanner –version

7. Configure SonarScanner:
	Create a Project in SonarQube:
		•	Go to the Projects tab in SonarQube and click on Create new project.
		•	Enter a project key and display name, then save the project.
	Generate a SonarQube Token:
		•	Go to My Account > Security in SonarQube, and create a new token. Copy this token for later use.

8. Add SonarQube Analysis to Your .NET Core Project
	In the root directory of your .NET Core application, run the following commands to analyze your project:
	Begin Analysis:
	Replace PROJECT_KEY, SONARQUBE_URL, and SONAR_TOKEN with your specific values.
	dotnet-sonarscanner begin /k:"PROJECT_KEY" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="SONAR_TOKEN"
	Build the Project:
	This step is essential for the scanner to analyze the code.
	dotnet build
	End Analysis:
	Run this command to complete the analysis and send the results to SonarQube. 
	dotnet-sonarscanner end /d:sonar.login="SONAR_TOKEN"

	**note execute those commands in PowerShell

9. On the Sonar UI:
	1. Quality profile:
		1. Navigate to the Quality Profiles section.
		2. Choose the Restore option.
		3. Backup all custom profiles from "SonarQube Custom Profiles" folder.
		4. Set the imported profiles as Default.
	
	Administration:
		Go to Analysis Scope.
			A. File Exclusions:
				Add the following to File Exclusions:
				**/wwwroot/js/jquery-nestable/**
				**/wwwroot/js/libs/**
				**/*bootstrap*
				**/wwwroot/css/libs/**
				
			B. Code Coverage Exclusions:
				Add the following to Coverage Exclusions:
				**/**
				
			C. Duplicate Exclusions:
				Add the following to Duplicate Exclusions:
				**/*.cshtml
				**/*.js
				sReports/sReportsV2.Domain.Sql/Migrations/**
				sReports/sReportsV2.Domain.MongoDb/DatabaseMigrationScripts/MongoMigration/**
				sReports/sReportsV2.Domain.MongoDb/Entities/**
	
10. Review Analysis Results
	View Results on SonarQube Dashboard:
		•	Go back to your SonarQube dashboard and navigate to your project to see the analysis results.
		•	Here, you can review code quality metrics, issues, code smells, vulnerabilities, and other insights.
