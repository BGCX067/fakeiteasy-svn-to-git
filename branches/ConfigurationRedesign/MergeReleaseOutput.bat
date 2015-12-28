@echo off
call "Libraries\ILMerge\ilmerge.exe" /out:FakeItEasy.dll FakeItEasy/bin/Release/FakeItEasy.dll FakeItEasy/bin/Release/Castle.DynamicProxy2.dll FakeItEasy/bin/Release/Castle.Core.dll
call "Libraries\ILMerge\ilmerge.exe" /out:FakeItEasy.Mef.dll FakeItEasy.Mef/bin/Release/FakeItEasy.Mef.dll FakeItEasy.Mef/bin/Release/System.ComponentModel.Composition.dll
copy "FakeItEasy\bin\Release\FakeItEasy.xml" FakeItEasy.xml
copy "FakeItEasy.Mef\bin\Release\FakeItEasy.Mef.xml" FakeItEasy.Mef.xml