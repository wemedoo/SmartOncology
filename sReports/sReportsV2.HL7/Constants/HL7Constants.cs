namespace sReportsV2.HL7.Constants
{
    public static class HL7Constants
    {
        public const string HL7 = "HL7";
        public const string HL7_MESSAGES = "HL7 messages";
        public const string Inbound = "Inbound";
        public const string Outbound = "Outbound";
        public const string ADT_A01 = "ADT^A01";
        public const string ADT_A03 = "ADT^A03";
        public const string ADT_A04 = "ADT^A04";
        public const string ADT_A08 = "ADT^A08";
        public const string ADT_A28 = "ADT^A28";
        public const string ADT_A31 = "ADT^A31";
        public const string ORU_R01 = "ORU^R01";
        public const string ADMITTING_DOCTOR = "Admitting doctor";
        public const string ATTENDING_DOCTOR = "Attending doctor";
        public const string CONSULTING_DOCTOR = "Consulting doctor";
        public const string REFERRING_DOCTOR = "Referring doctor";
        public const char START_OF_BLOCK = (char)0x0B;
        public const char END_OF_BLOCK = (char)0x1C;
        public const char CARRIAGE_RETURN = (char)13;
        public const char FIELD_DELIMITER = '|';
        public const char COMPONENT_SEPARATOR = '^';
        public const string APPLICATION_ACCEPT_CODE = "AA";
        public const string APPLICATION_ERROR_CODE = "AE";
        public const string APPLICATION_REJECT_CODE = "AR";
        public const string VERSION_2_3_1 = "2.3.1";
        public const string HL7_DATE_TIME_FORMAT = "yyyyMMddHHmmss";
        public const string EMPTY_PARSED_STRING = "\"\"";
        public const string MLLP_PORT = "MLLP_PORT";
    }
}