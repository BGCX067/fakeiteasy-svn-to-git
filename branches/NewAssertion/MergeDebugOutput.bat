@echo off
call "Libraries\ILMerge\ilmerge.exe" /out:FakeItEasy.dll FakeItEasy/bin/Debug/FakeItEasy.dll FakeItEasy/bin/Debug/Castle.DynamicProxy2.dll FakeItEasy/bin/Debug/Castle.Core.dll
call "Libraries\ILMerge\ilmerge.exe" /out:FakeItEasy.Mef.dll FakeItEasy.Mef/bin/Debug/FakeItEasy.Mef.dll FakeItEasy.Mef/bin/Debug/System.ComponentModel.Composition.dll
copy "FakeItEasy\bin\Debug\FakeItEasy.xml" FakeItEasy.xml
copy "FakeItEasy.Mef\bin\Debug\FakeItEasy.Mef.xml" FakeItEasy.Mef.xml