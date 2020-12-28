namespace BecamexIDC.Authentication.Helpers
{
   public class OperationResult
    {
        public string Caption { set; get; }
        public string Message { set; get; }
        public bool Success { set; get; }
        public OperationResult() { }
        public OperationResult(string caption, string message, bool success)
        {
            this.Caption = caption;
            this.Message = message;
            this.Success = success;
        }
    }
}