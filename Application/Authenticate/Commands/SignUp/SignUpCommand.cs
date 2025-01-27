using Application.Common.Requests;
using MediatR;

namespace Application.Authenticate.Commands.SignUp
{
    public class SignUpCommand : IRequest<RequestResult>
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public Guid RoleId { get; set; }
    }
}
