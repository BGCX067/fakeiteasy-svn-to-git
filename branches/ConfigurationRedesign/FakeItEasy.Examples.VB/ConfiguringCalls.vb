Imports FakeItEasy
Imports FakeItEasy.VisualBasic

Public Class ConfiguringCalls
    Public Sub Configuring_a_sub_to_throw_an_exception()
        Dim widget = A.Fake(Of IWidget)()

        ThisCall.To(widget).Throws(New NotSupportedException()) : widget.Repair()
    End Sub

    Public Sub Configuring_a_function_to_return_a_value()
        Dim factory = A.Fake(Of IWidgetFactory)()

        Configure.Fake(factory).CallsTo(Function(x) x.Create()).Returns(A.Fake(Of IWidget)())
    End Sub
End Class
