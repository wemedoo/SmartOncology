using sReportsV2.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.Extensions
{
    public static class TaskExtension
    {
        public static void LogTaskFailure(this Task task, string msg)
        {
            if (task.IsFaulted)
            {
                LogHelper.Error(msg);
            }
        }
    }
}
