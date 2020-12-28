using System;

namespace BecamexIDC.Authentication.Domain
{
    public class TokenCreator
    {
        public string token {set;get;}
        public DateTime expiration {set;get;}
        public TokenCreator(string token, DateTime expiration)
        {
            this.token = token;
            this.expiration = expiration;
        }
    }
}