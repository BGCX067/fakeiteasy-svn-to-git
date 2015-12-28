@echo off
call "Libraries\ILMerge\ilmerge.exe" /out:FakeItEasy.dll FakeItEasy/bin/Debug/FakeItEasy.dll FakeItEasy/bin/Debug/Castle.DynamicProxy2.dll FakeItEasy/bin/Debug/Castle.Core.dll
copy "FakeItEasy\bin\Debug\FakeItEasy.xml" FakeItEasy.xml