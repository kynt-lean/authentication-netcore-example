namespace BecamexIDC.Authentication.Options
{

    public class LDAPSettings
    {
        public string Account { set; get; }
        public string Password { set; get; }
        public string Domain { set; get; }
        public string Address { set; get; }
        public string Port { set; get; }
    }  
    public class MailSettings
    {
        public string Server { set; get; }
        public int Port { set; get; }
        public string SenderName { set; get; }
        public string SenderEmail { set; get; }
        public string Account { set; get; }
        public string Password { set; get; }
    }
}