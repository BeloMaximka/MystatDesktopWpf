using MystatAPI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Domain
{
    public interface IHomeworkManager
    {
        void DownloadHomework(Homework homework);
    }
}
