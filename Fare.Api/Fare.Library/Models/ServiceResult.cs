namespace Fare.Library.Models
{
    public class ServiceResult
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorTrace { get; set; }
        public int? StatusCode { get; set; }
    }
    public class ServiceResult<TResult> : ServiceResult
    {
        public TResult Result { get; set; }
    }
}
