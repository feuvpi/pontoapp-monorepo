using MediatR;
using PontoAPP.Application.DTOs.Users;

namespace PontoAPP.Application.Queries.Users;

public class GetUsersQuery : IRequest<IEnumerable<UserResponse>>
{
    public bool? ActiveOnly { get; set; }
    public string? Role { get; set; }
}
