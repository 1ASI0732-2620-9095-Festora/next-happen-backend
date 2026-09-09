using com.festora.nexthappen.iam.Application.DTOs;
using com.festora.nexthappen.iam.Domain.Entities;
using com.festora.nexthappen.iam.Domain.Repositories;
using com.festora.nexthappen.iam.Domain.Services;

namespace com.festora.nexthappen.iam.Application.UseCases;

public class RegisterUser
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUser(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task HandleAsync(RegisterRequest request)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing != null)
            throw new Exception("El correo ya está registrado.");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = request.Role
        };

        await _userRepository.AddAsync(user);
    }
}
