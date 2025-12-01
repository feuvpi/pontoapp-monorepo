using MediatR;
using PontoAPP.Application.DTOs.Users;
using PontoAPP.Domain.Repositories;

namespace PontoAPP.Application.Queries.Users;

public class GetUserByIdQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
            return null;

        return new UserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email.Value,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            EmployeeCode = user.EmployeeCode,
            Department = user.Department,
            HiredAt = user.HiredAt,
            CreatedAt = user.CreatedAt
        };
    }
}