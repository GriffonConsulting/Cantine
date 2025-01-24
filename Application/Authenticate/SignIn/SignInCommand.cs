using Application.Common.Requests;
using Domain.Authorization;
using MediatR;

namespace Application.Authentication.Commands.SignIn;

public class SignInCommand : IRequest<RequestResult<SignInDto>>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
