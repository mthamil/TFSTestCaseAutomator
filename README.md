TFS Test Case Automator
=======================

TFS Test Case Automator is a desktop application that enables the association of Team Foundation Server Test Cases with automated tests.

By default, it comes with a plugin enabling association of test cases with xUnit.net tests, but plugins can be written for any test framework
by referencing [![this NuGet Package](https://img.shields.io/nuget/v/TestCaseAutomator.AutomationProviders.Interfaces.svg)](https://www.nuget.org/packages/TestCaseAutomator.AutomationProviders.Interfaces/) and implementing the included interfaces. In addition, manual entry of test case automation association 
details is available.

At a bare minimum, the Visual Studio shell should be installed on the target computer for the application to connect to a TFS server. This shell
comes with Visual Studio Test Professional, for example.

[![Build status](https://ci.appveyor.com/api/projects/status/trbslo0vug5xkur0)](https://ci.appveyor.com/project/mthamil/tfstestcaseautomator)
