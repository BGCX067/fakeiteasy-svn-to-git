Imports NUnit.Framework
Imports FakeItEasy.Tests
Imports FakeItEasy.VisualBasic

<TestFixture()> _
Public Class ThisCallTests

    <Test(), ExpectedException(GetType(ExpectationException))> _
    Public Overridable Sub AssertWasCalled_should_fail_when_call_has_not_been_made()

        Dim foo = A.Fake(Of IFoo)()

        ThisCall.To(foo).AssertWasCalled(Function(repeat) repeat > 0) : foo.Bar()
    End Sub

    <Test()> _
    Public Overridable Sub AssertWasCalled_should_succeed_when_call_has_been_made()
        Dim foo = A.Fake(Of IFoo)()

        foo.Bar()

        ThisCall.To(foo).AssertWasCalled(Function(repeat) repeat > 0) : foo.Bar()
    End Sub

End Class
