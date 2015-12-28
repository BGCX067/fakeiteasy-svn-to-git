using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.DebuggerVisualizers;
using System.Windows.Forms;
using FakeItEasy.Api;
using System.Diagnostics;
using FakeItEasy.Debugging;

[assembly: DebuggerVisualizer(
    typeof(FakeObjectVisualizer), 
    typeof(VisualizerObjectSource), 
    Target = typeof(FakeItEasy.Api.IFakedProxy), 
    Description = "Fake object visualizer")]

namespace FakeItEasy.Debugging
{
    public class FakeObjectVisualizer
        : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            var fakedObject = objectProvider.GetObject();
            var fakeObject = Fake.GetFakeObject(fakedObject);

            var message = new StringBuilder()
                .Append(fakedObject.GetType().Name)
                .AppendLine()
                .Append(string.Join(", ", fakeObject.Rules.Select(x => x.ToString()).ToArray()));

            MessageBox.Show(message.ToString());
        }
    }
}
