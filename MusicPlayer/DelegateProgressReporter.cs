using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{
    public class DelegateProgressReporter(Action<double> onReport) : IProgress<double>
    {
        public void Report(double value) => onReport(value);
    }
}
