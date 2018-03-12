using System;
using System.Runtime.Serialization;

namespace ViewXMLCreator
{
    public class XmlElementsHierarchyError : Exception
    {
        public XmlElementsHierarchyError() : base("XmlElementsHierarchyError")
        {
        }

        public XmlElementsHierarchyError(string message) : base(message)
        {
        }

        public XmlElementsHierarchyError(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected XmlElementsHierarchyError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}