﻿namespace Shared.ApiSchema.Auth;

public class LoginModel
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}