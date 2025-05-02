namespace Jammer.ResponseMessage
{
    public class ResultMessage
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static ResultMessage SuccessResult(string message)
        {
            return new ResultMessage { Success = true, Message = message };
        }

        public static ResultMessage FailureResult(string message)
        {
            return new ResultMessage { Success = false, Message = message };
        }
        public static string DisplayMessage(string message,string error)
        {
            return $"message {message}  --  error {error}";
        }
        public static string DisplayError(string error)
        {
            return $"error {error}";
        }
        override
        public  string ToString()
        {
            return $"Success {this.Success}  --  Message {this.Message}";
        }
    }
}