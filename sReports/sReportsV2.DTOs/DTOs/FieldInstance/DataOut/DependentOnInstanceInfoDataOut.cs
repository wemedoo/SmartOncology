namespace sReportsV2.DTOs.Field.DataOut
{
    public class DependentOnInstanceInfoDataOut : DependentOnInfoDataOut
    {
        public string ChildFieldInstanceRepetitionId { get; set; }
        public string ChildFieldSetInstanceRepetitionId { get; set; }
        public bool IsChildDependentFieldSetRepetitive { get; set; }
        public string ParentFieldInstanceCssSelector { get; set; }
        public string ChildFieldInstanceCssSelector { get; set; }

        public DependentOnInstanceInfoDataOut(DependentOnInfoDataOut dependentOnInfoDataOut) : base(dependentOnInfoDataOut) { }
    }
}