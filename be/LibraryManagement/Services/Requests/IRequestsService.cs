using LibraryManagement.Models.Dtos;
using LibraryManagement.Models.Dtos.Requests;

namespace LibraryManagement.Services.Requests
{
    public interface IRequestsService
    {
        public Task<PagedResult<RequestDto>> GetMyRequestsAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        public Task<RequestDto> GetMyRequestByIdAsync(int id, int userId, CancellationToken cancellationToken);
        public Task<RequestDto> CreateRequestAsync(int requestorId, CreateRequestDto createRequestDto, CancellationToken cancellationToken);
        public Task<AllowanceDto> GetMyAllowanceAsync(int userId, CancellationToken cancellationToken);
        public Task<PagedResult<RequestDto>> GetRequestsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        public Task<RequestDto> GetRequestByIdAsync(int id, CancellationToken cancellationToken);
        public Task ApproveRequestAsync(int id, int approverId, CancellationToken cancellationToken);
        public Task RejectRequestAsync(int id, CancellationToken cancellationToken);
    }
}