namespace Fare.Library.Models
{
    public class ServiceResult
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorTrace { get; set; }
        public int? StatusCode { get; set; }

        public ServiceResult()
        {
            ErrorMessage = string.Empty;
            ErrorTrace = string.Empty;
        }
    }
    public class ServiceResult<TResult> : ServiceResult
    {
        public TResult Result { get; set; }
    }
}
