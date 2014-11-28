
namespace SherpaDesk.Models
{
    public enum eErrorType
    {
        [Details("Error")]
        Error,

        [Details("Invalid input data")]
        InvalidInputData,

        [Details("Invalid output data")]
        InvalidOutputData,

        [Details("Failed Operation")]
        FailedOperation,

        [Details("Internal Error")]
        InternalError,

        [Details("Warning")]
        Warning,

        [Details("")]
        NoTitle,

        [Details("Message")]
        Message
    }
}
