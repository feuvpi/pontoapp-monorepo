using MediatR;
using PontoAPP.Application.DTOs.Users;

namespace PontoAPP.Application.Queries.Users;

public class GetUserByIdQuery : IRequest<UserResponse?>
{
    public Guid UserId { get; set; }
}