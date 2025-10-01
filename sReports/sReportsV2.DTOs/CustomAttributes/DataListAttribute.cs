using System;

namespace sReportsV2.DTOs.CustomAttributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class DataListAttribute : Attribute
    {
    }
}
