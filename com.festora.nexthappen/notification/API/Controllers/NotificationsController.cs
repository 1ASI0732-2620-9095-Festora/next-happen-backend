using com.festora.nexthappen.notification.Application.DTOs;
using com.festora.nexthappen.notification.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace com.festora.nexthappen.notification.API.Controllers;

[ApiController]
[Route("api/v1/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly VerificationCodeService _verificationCodeService;

    public NotificationsController(VerificationCodeService verificationCodeService)
    {
        _verificationCodeService = verificationCodeService;
    }

    [HttpPost("send-verification-code")]
    public IActionResult SendVerificationCode([FromBody] SendVerificationCodeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Phone))
        {
            return BadRequest(new { Message = "Phone number is required." });
        }

        var result = _verificationCodeService.SendVerificationCode(request.Phone);

        if (!result.Success)
        {
            return BadRequest(new { Message = result.ErrorMessage });
        }

        return Ok(new NotificationResponse(true, "Verification code sent"));
    }

    [HttpPost("verify-code")]
    public IActionResult VerifyCode([FromBody] VerifyCodeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Phone) || string.IsNullOrWhiteSpace(request.Code))
        {
            return BadRequest(new { Message = "Phone number and code are required." });
        }

        var result = _verificationCodeService.VerifyCode(request.Phone, request.Code);

        if (!result.Success)
        {
            return BadRequest(new { Message = result.ErrorMessage });
        }

        return Ok(new VerifyCodeResponse(true, "Phone number verified"));
    }
}
