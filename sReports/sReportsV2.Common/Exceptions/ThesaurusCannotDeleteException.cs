using System;

namespace sReportsV2.Common.Exceptions
{
    [Serializable()]
    public class ThesaurusCannotDeleteException : Exception
    {
        public ThesaurusCannotDeleteException()
        {
        }

        public ThesaurusCannotDeleteException(string message)
            : base(message)
        {
        }

        public ThesaurusCannotDeleteException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ThesaurusCannotDeleteException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
