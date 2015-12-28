@echo off
call "Libraries\ILMerge\ilmerge.exe" /out:FakeItEasy.dll FakeItEasy/bin/Release/FakeItEasy.dll FakeItEasy/bin/Release/Castle.DynamicProxy2.dll FakeItEasy/bin/Release/Castle.Core.dll
copy "FakeItEasy\bin\Release\FakeItEasy.xml" FakeItEasy.xml