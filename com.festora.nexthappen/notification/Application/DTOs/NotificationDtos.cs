namespace com.festora.nexthappen.notification.Application.DTOs;

public record SendVerificationCodeRequest(string Phone);

public record VerifyCodeRequest(string Phone, string Code);

public record NotificationResponse(bool Success, string Message);

public record VerifyCodeResponse(bool Verified, string Message);
