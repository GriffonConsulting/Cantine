﻿namespace Application.Authenticate.Commands.SignIn
{
    public class SignInDto
    {
        public required string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
