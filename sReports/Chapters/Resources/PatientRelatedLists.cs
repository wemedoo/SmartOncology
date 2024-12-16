using System.Collections.Immutable;

namespace Chapters
{
    public static class PatientRelatedLists
    {
        public static readonly ImmutableList<string> BasicInfoList = ImmutableList.Create("Name", "Family", "Gender", "BirthDate", "MultipleBirth", "Language");

        public static readonly ImmutableList<string> AddressInfoList = ImmutableList.Create("City", "State", "PostalCode", "Country", "Street");

        public static readonly ImmutableList<string> ContactPersonList = ImmutableList.Create(
            "ContactStreet", "ContactCity", "ContactState", "ContactPostalCode", "ContactCountry", "ContactGender", "ContactName",
            "ContactFamily", "ContactTelecomCheckBox", "ContactPhoneUse", "ContactPhone", "ContactFaxUse", "ContactFax",
            "ContactEmailUse", "ContactEmail", "ContactPagerUse", "ContactPager", "ContactUrlUse", "ContactUrl", "ContactSmsUse",
            "ContactSms", "ContactOtherUse", "ContactOther", "Relationship"
        );

        public static readonly ImmutableList<string> TelecomValues = ImmutableList.Create(
            "TelecomCheckBox", "PhoneUse", "Phone", "FaxUse", "Fax", "EmailUse", "Email", "PagerUse", "Pager",
            "UrlUse", "Url", "SmsUse", "Sms", "OtherUse", "Other"
        );

        public static readonly ImmutableList<string> ContactTelecomValues = ImmutableList.Create(
            "ContactTelecomCheckBox", "ContactPhoneUse", "ContactPhone", "ContactFaxUse", "ContactFax",
            "ContactEmailUse", "ContactEmail", "ContactPagerUse", "ContactPager", "ContactUrlUse", "ContactUrl",
            "ContactSmsUse", "ContactSms", "ContactOtherUse", "ContactOther"
        );
    }
}
