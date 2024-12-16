using System;

namespace sReportsV2.DTOs.DTOs.Form.DataOut
{
    public interface IFormStatusDataOut
    {
        dynamic StatusValue { get; }
        DateTime CreatedDateTime { get; }
        string CreatedName { get; }
    }
}
