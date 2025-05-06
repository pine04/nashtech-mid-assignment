using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Security.Claims;
using LibraryManagement.Exceptions;
using LibraryManagement.Models.Dtos;
using LibraryManagement.Models.Dtos.Requests;
using LibraryManagement.Services.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("/api")]
    [Produces(MediaTypeNames.Application.Json)]
    public class RequestsController : ControllerBase
    {
        private IRequestsService _requestsService;

        public RequestsController(IRequestsService requestsService)
        {
            _requestsService = requestsService;
        }

        [HttpGet("my-requests")]
        [Authorize("NormalUser")]
        public async Task<ActionResult<PagedResult<RequestDto>>> GetMyRequestsAsync(
            [Range(1, int.MaxValue)] int pageNumber = 1,
            [Range(1, 100)] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                throw new InvalidSubClaimException("The sub claim in the access token is invalid.");
            }

            return await _requestsService.GetMyRequestsAsync(userId, pageNumber, pageSize, cancellationToken);
        }

        [HttpGet("my-requests/{id}")]
        [Authorize("NormalUser")]
        public async Task<ActionResult<RequestDto>> GetMyRequestByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                throw new InvalidSubClaimException("The sub claim in the access token is invalid.");
            }

            return await _requestsService.GetMyRequestByIdAsync(id, userId, cancellationToken);
        }

        [HttpPost("my-requests")]
        [Authorize("NormalUser")]
        public async Task<ActionResult<RequestDto>> CreateRequestAsync(CreateRequestDto createRequestDto, CancellationToken cancellationToken = default)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int requestorId))
            {
                throw new InvalidSubClaimException("The sub claim in the access token is invalid.");
            }

            RequestDto requestDto = await _requestsService.CreateRequestAsync(requestorId, createRequestDto, cancellationToken);
            return CreatedAtAction(nameof(GetMyRequestByIdAsync), new { id = requestDto.Id }, requestDto);
        }

        [HttpGet("my-allowance")]
        [Authorize("NormalUser")]
        public async Task<ActionResult<AllowanceDto>> GetMyAllowanceAsync(CancellationToken cancellationToken = default)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                throw new InvalidSubClaimException("The sub claim in the access token is invalid.");
            }

            return await _requestsService.GetMyAllowanceAsync(userId, cancellationToken);
        }

        [HttpGet("requests")]
        [Authorize("SuperUser")]
        public async Task<ActionResult<PagedResult<RequestDto>>> GetRequestsAsync(
            [Range(1, int.MaxValue)] int pageNumber = 1,
            [Range(1, 100)] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            return await _requestsService.GetRequestsAsync(pageNumber, pageSize, cancellationToken);
        }

        [HttpGet("requests/{id}")]
        [Authorize("SuperUser")]
        public async Task<ActionResult<RequestDto>> GetRequestByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _requestsService.GetRequestByIdAsync(id, cancellationToken);
        }

        [HttpPost("requests/{id}/approve")]
        [Authorize("SuperUser")]
        public async Task<ActionResult> ApproveRequestAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int approverId))
            {
                throw new InvalidSubClaimException("The sub claim in the access token is invalid.");
            }

            await _requestsService.ApproveRequestAsync(id, approverId, cancellationToken);
            return Ok();
        }

        [HttpPost("requests/{id}/reject")]
        [Authorize("SuperUser")]
        public async Task<ActionResult> RejectRequestAsync(int id, CancellationToken cancellationToken = default)
        {
            await _requestsService.RejectRequestAsync(id, cancellationToken);
            return Ok();
        }
    }
}