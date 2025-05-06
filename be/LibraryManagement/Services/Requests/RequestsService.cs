using Azure.Core;
using LibraryManagement.Data;
using LibraryManagement.Exceptions;
using LibraryManagement.Models.Dtos;
using LibraryManagement.Models.Dtos.Requests;
using LibraryManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagement.Services.Requests
{
    public class RequestsService : IRequestsService
    {
        private const int MaxRequestsPerMonth = 3;
        private const int MaxBooksPerRequest = 5;

        private ApplicationDbContext _dbContext;

        public RequestsService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RequestDto> GetMyRequestByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
        {
            BookBorrowingRequest? request = await _dbContext.BookBorrowingRequests
                .Include(r => r.Requestor)
                .Include(r => r.Approver)
                .Include(r => r.BookBorrowingRequestDetails)
                .ThenInclude(rd => rd.Book)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            if (request == null)
            {
                throw new NotFoundException("Borrowing request not found.");
            }

            if (request.RequestorId != userId)
            {
                throw new ForbiddenException("You do not have permission to view this request.");
            }

            return request.ToRequestDto();
        }

        public async Task<PagedResult<RequestDto>> GetMyRequestsAsync(int userId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            int skip = (pageNumber - 1) * pageSize;

            List<RequestDto> requests = await _dbContext.BookBorrowingRequests
                .Include(r => r.Requestor)
                .Include(r => r.Approver)
                .Include(r => r.BookBorrowingRequestDetails)
                .ThenInclude(rd => rd.Book)
                .Where(r => r.RequestorId == userId)
                .OrderByDescending(r => r.DateRequested)
                .Skip(skip)
                .Take(pageSize)
                .Select(r => r.ToRequestDto())
                .ToListAsync(cancellationToken);

            int totalRecordCount = await _dbContext.BookBorrowingRequests.Include(r => r.Requestor).Where(r => r.RequestorId == userId).CountAsync(cancellationToken);

            return new PagedResult<RequestDto>()
            {
                Results = requests,
                TotalRecordCount = totalRecordCount
            };
        }

        public async Task<RequestDto> CreateRequestAsync(int requestorId, CreateRequestDto createRequestDto, CancellationToken cancellationToken = default)
        {
            // Check if the requestor exists.
            User? requestor = await _dbContext.Users.FindAsync(requestorId, cancellationToken);
            if (requestor == null)
            {
                throw new NonExistentUserException("The requestor does not exist in the database.");
            }

            // Check if the monthly allowance has been reached.
            DateTime now = DateTime.UtcNow;
            DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
            DateTime startOfNextMonth = startOfMonth.AddMonths(1);

            int requestsThisMonth = await _dbContext.BookBorrowingRequests.Where(r => r.RequestorId == requestorId && r.DateRequested >= startOfMonth && r.DateRequested < startOfNextMonth).CountAsync(cancellationToken);
            if (requestsThisMonth >= MaxRequestsPerMonth)
            {
                throw new MonthlyLimitReachedException();
            }

            // Check if the list of book IDs is non-empty.
            if (createRequestDto.BookIds.IsNullOrEmpty())
            {
                throw new NoBooksProvidedException();
            }

            // Remove all duplicate book IDs and check if the list contains at MOST 5 unique IDs.
            HashSet<int> uniqueBookIds = new HashSet<int>(createRequestDto.BookIds);
            if (uniqueBookIds.Count > MaxBooksPerRequest)
            {
                throw new TooManyBooksException();
            }

            // Check if the provided book IDs exist.
            List<Book> foundBooks = await _dbContext.Books.Where(b => uniqueBookIds.Contains(b.Id)).ToListAsync(cancellationToken);

            List<int> foundBookIds = foundBooks.Select(b => b.Id).ToList();
            List<int> nonExistentBookIds = uniqueBookIds.Except(foundBookIds).ToList();
            if (nonExistentBookIds.Count != 0)
            {
                throw new NonExistentBookException($"Books with IDs {string.Join(", ", nonExistentBookIds)} do not exist.");
            }

            // Check if the requested books are available.
            List<int> nonAvailableBookIds = foundBooks.Where(b => b.Available < 1).Select(b => b.Id).ToList();
            if (nonAvailableBookIds.Count != 0)
            {
                throw new BookNotAvailableException($"Books with IDs {string.Join(", ", nonAvailableBookIds)} are not available.");
            }

            // Create the borrowing request.
            createRequestDto.BookIds = foundBookIds; // Removes duplicates.
            BookBorrowingRequest request = createRequestDto.ToBookBorrowingRequest(requestorId);
            await _dbContext.BookBorrowingRequests.AddAsync(request, cancellationToken);

            // Decrement the available number of each book by 1.
            foundBooks.ForEach(b => b.Available--);

            // Save changes to the DB.
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Read the newly created request from the DB and create a DTO.
            BookBorrowingRequest createdRequest = await _dbContext.BookBorrowingRequests
                .Include(r => r.Requestor)
                .Include(r => r.BookBorrowingRequestDetails)
                .ThenInclude(rd => rd.Book)
                .FirstAsync(r => r.Id == request.Id, cancellationToken);
            return createdRequest.ToRequestDto();
        }


        public async Task<AllowanceDto> GetMyAllowanceAsync(int userId, CancellationToken cancellationToken = default)
        {
            DateTime now = DateTime.UtcNow;
            DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
            DateTime startOfNextMonth = startOfMonth.AddMonths(1);

            int requestsThisMonth = await _dbContext.BookBorrowingRequests
                .Include(r => r.Requestor)
                .Where(r => r.DateRequested >= startOfMonth && r.DateRequested < startOfNextMonth && r.RequestorId == userId)
                .CountAsync(cancellationToken);

            return new AllowanceDto()
            {
                RequestsAvailable = MaxRequestsPerMonth - requestsThisMonth,
                RequestLimit = MaxRequestsPerMonth
            };
        }

        public async Task<RequestDto> GetRequestByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            BookBorrowingRequest? request = await _dbContext.BookBorrowingRequests
                .Include(r => r.Requestor)
                .Include(r => r.Approver)
                .Include(r => r.BookBorrowingRequestDetails)
                .ThenInclude(rd => rd.Book)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            if (request == null)
            {
                throw new NotFoundException("Borrowing request not found.");
            }

            return request.ToRequestDto();
        }

        public async Task<PagedResult<RequestDto>> GetRequestsAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            int skip = (pageNumber - 1) * pageSize;

            List<RequestDto> requests = await _dbContext.BookBorrowingRequests
                .Include(r => r.Requestor)
                .Include(r => r.Approver)
                .Include(r => r.BookBorrowingRequestDetails)
                .ThenInclude(rd => rd.Book)
                .OrderByDescending(r => r.DateRequested)
                .Skip(skip)
                .Take(pageSize)
                .Select(r => r.ToRequestDto())
                .ToListAsync(cancellationToken);

            int totalRecordCount = await _dbContext.BookBorrowingRequests.CountAsync(cancellationToken);

            return new PagedResult<RequestDto>()
            {
                Results = requests,
                TotalRecordCount = totalRecordCount
            };
        }

        public async Task ApproveRequestAsync(int id, int approverId, CancellationToken cancellationToken = default)
        {
            // Check if the approver exists.
            User? approver = await _dbContext.Users.FindAsync(approverId, cancellationToken);
            if (approver == null)
            {
                throw new NonExistentUserException("The approver does not exist in the database.");
            }

            // Check if the request exists.
            BookBorrowingRequest? request = await _dbContext.BookBorrowingRequests
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            if (request == null)
            {
                throw new NotFoundException("Borrowing request not found.");
            }

            // Check if the request is in the Waiting status.
            if (request.Status != Status.Waiting)
            {
                throw new RequestAlreadySettledException();
            }

            // Approve the request and save in DB.
            request.Status = Status.Approved;
            request.ApproverId = approverId;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RejectRequestAsync(int id, CancellationToken cancellationToken = default)
        {
            // Check if the request exists.
            BookBorrowingRequest? request = await _dbContext.BookBorrowingRequests
                .Include(r => r.Books)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            if (request == null)
            {
                throw new NotFoundException("Borrowing request not found.");
            }

            // Check if the request is in the Waiting status.
            if (request.Status != Status.Waiting)
            {
                throw new RequestAlreadySettledException();
            }

            // Approve the request.
            request.Status = Status.Rejected;

            // Increment the book available numbers by 1 for all books in the request.
            foreach (Book book in request.Books)
            {
                book.Available += 1;
            }

            // Save changes to the DB.
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}