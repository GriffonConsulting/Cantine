using Application.Authenticate.Commands.SignIn;
using Application.Authenticate.Commands.SignUp;
using Application.Common.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cantine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("SignUp", Name = "SignUp")]
        [ProducesResponseType(typeof(RequestResult<SignInDto>), RequestStatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RequestResult), RequestStatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RequestResult<SignInDto>), RequestStatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(SignUpCommand signUpCommand, CancellationToken cancellationToken)
        {
            var signUpResult = await _mediator.Send(signUpCommand, cancellationToken);
            if (signUpResult.StatusCodes != RequestStatusCodes.Status200OK)
                return BadRequest(signUpResult);
            var signInResult = await _mediator.Send(new SignInCommand { Email = signUpCommand.Email, Password = signUpCommand.Password }, cancellationToken);
            return new ObjectResult(signInResult)
            {
                StatusCode = signInResult.StatusCodes
            };
        }

        [HttpPost("SignIn", Name = "SignIn")]
        [ProducesResponseType(typeof(RequestResult<SignInDto>), RequestStatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RequestResult<SignInDto>), RequestStatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(SignInCommand signInCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(signInCommand, cancellationToken);
            return Ok(result);
        }
    }
}