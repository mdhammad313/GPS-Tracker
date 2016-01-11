namespace Sms_Sender.Model
{
    public abstract class BaseResponseModel
    {
        public bool IsValid { get; set; }
        public string Error { get; set; }
    }
}