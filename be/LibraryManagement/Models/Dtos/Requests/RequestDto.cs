using LibraryManagement.Models.Entities;

namespace LibraryManagement.Models.Dtos.Requests
{
    public class RequestDto
    {
        public int Id { get; set; }
        public DateTime DateRequested { get; set; }
        public required string Status { get; set; }
        public required RequestUserDto Requestor { get; set; }
        public required RequestUserDto? Approver { get; set; }
        public required List<RequestBookDto> Books { get; set; }
    }

    public class RequestUserDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
    }

    public class RequestBookDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public bool Returned { get; set; }
    }

    public static partial class BookBorrowingRequestExtensions
    {
        public static RequestDto ToRequestDto(this BookBorrowingRequest request)
        {
            if (request.Requestor == null)
            {
                throw new InvalidOperationException("Requestor must be included when building RequestDto.");
            }

            return new RequestDto()
            {
                Id = request.Id,
                DateRequested = request.DateRequested,
                Status = request.Status.ToString(),
                Requestor = request.Requestor.ToRequestUserDto(),
                Approver = request.Approver?.ToRequestUserDto(),
                Books = request.BookBorrowingRequestDetails.Select(rd => rd.ToRequestBookDto()).ToList()
            };
        }
    }

    public static partial class UserExtensions
    {
        public static RequestUserDto ToRequestUserDto(this User user)
        {
            return new RequestUserDto()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }
    }

    public static partial class BookBorrowingRequestDetailExtensions
    {
        public static RequestBookDto ToRequestBookDto(this BookBorrowingRequestDetail detail)
        {
            if (detail.Book == null)
            {
                throw new InvalidOperationException("Book must be included when building RequestBookDto.");
            }

            return new RequestBookDto()
            {
                Id = detail.Book.Id,
                Title = detail.Book.Title,
                Author = detail.Book.Author,
                Returned = detail.Returned
            };
        }
    }
}