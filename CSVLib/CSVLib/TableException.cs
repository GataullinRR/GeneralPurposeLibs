using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace CSVLib
{
    public class TableException : Exception
    {
        public enum ErrorCodes : int
        {
            NONE = 0,
            UNABLE_TO_SAVE_AS_CSV,
            UNABLE_TO_LOAD
        }

        public readonly ErrorCodes ErrorCode;

        public override string Message
        {
            get
            {
                return "BaseMessage: \"{0}\" Error: \"{1}\"".Format(base.Message as object, ErrorCode);
            }
        }

        public TableException(ErrorCodes error, string additionalMsg, Exception innerException)
            : base(additionalMsg, innerException)
        {
            ErrorCode = error;
        }

        public TableException(ErrorCodes error, string additionalMsg)
            : base(additionalMsg)
        {
            ErrorCode = error;
        }

        public TableException(ErrorCodes error, Exception innerException)
            : base("", innerException)
        {
            ErrorCode = error;
        }

        public TableException(ErrorCodes error) : base()
        {
            ErrorCode = error;
        }
    }
}
