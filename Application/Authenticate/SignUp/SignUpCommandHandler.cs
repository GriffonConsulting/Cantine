using Application.Common.Requests;
using Domain.Authorization;
using EntityFramework.Commands;
using EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Authentication.Commands.SignUp
{
    public class SignUpCommandHandler : IRequestHandler<SignUpCommand, RequestResult>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ClientCommands _userCommands;

        public SignUpCommandHandler(UserManager<IdentityUser> userManager, ClientCommands userCommands)
        {
            _userManager = userManager;
            _userCommands = userCommands;
        }

        public async Task<RequestResult> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            var user = new IdentityUser { UserName = request.Email, Email = request.Email };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return new RequestResult { StatusCodes = RequestStatusCodes.Status400BadRequest, Message = string.Join(";", result.Errors.Select(e => e.Code).ToArray()) };
            }
            await _userManager.AddToRoleAsync(user, nameof(UserRoles.User));
            var userId = await _userManager.GetUserIdAsync(user);
            var newUser = new Client
            {
                Id = Guid.Parse(userId),
                Email = request.Email,
                ClientRole = request.ClientRole,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
            };
            await _userCommands.AddAsync(newUser, cancellationToken);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, token);

            return new RequestResult { StatusCodes = RequestStatusCodes.Status200OK };
        }
    }
}
