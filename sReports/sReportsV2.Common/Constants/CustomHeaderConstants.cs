using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.Constants
{
    public class CustomHeaderConstant
    {
        public int DefaultHeaderCode { get; set; }
        public string Label { get; set;  }
    }

    public static class CustomHeaderConstants
    {
        public const string User = "User";
        public const string Version = "Version";
        public const string Language = "Language";
        public const string Patient = "Patient";
        public const string LastUpdate = "Last Update";
        public const string Unknown = "Unknown";
        public const string ProjectName = "Project Name";

        public static List<CustomHeaderConstant> GetCustomHeaderConstantList()
        {
            return new List<CustomHeaderConstant>(){
                new CustomHeaderConstant() { DefaultHeaderCode = 1, Label = User },
                new CustomHeaderConstant() { DefaultHeaderCode = 2, Label = Version },
                new CustomHeaderConstant() { DefaultHeaderCode = 3, Label = Language },
                new CustomHeaderConstant() { DefaultHeaderCode = 4, Label = Patient },
                new CustomHeaderConstant() { DefaultHeaderCode = 5, Label = LastUpdate },
                new CustomHeaderConstant() { DefaultHeaderCode = 6, Label = ProjectName },
            };

        }
    }
}
