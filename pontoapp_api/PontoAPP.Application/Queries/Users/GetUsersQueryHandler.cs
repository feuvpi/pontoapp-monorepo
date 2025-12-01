using MediatR;
using PontoAPP.Application.DTOs.Users;
using PontoAPP.Domain.Entities.Identity;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.Repositories;

namespace PontoAPP.Application.Queries.Users;

public class GetUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersQuery, IEnumerable<UserResponse>>
{
    public async Task<IEnumerable<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<User> users;

        if (!string.IsNullOrEmpty(request.Role) && Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            users = await userRepository.GetByRoleAsync(role, cancellationToken);
        }
        else if (request.ActiveOnly == true)
        {
            users = await userRepository.GetActiveUsersAsync(cancellationToken);
        }
        else
        {
            users = await userRepository.GetAllAsync(cancellationToken);
        }

        return users.Select(MapToResponse);
    }

    private static UserResponse MapToResponse(User user)
    {
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