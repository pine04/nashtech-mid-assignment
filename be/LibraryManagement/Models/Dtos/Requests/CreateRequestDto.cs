using LibraryManagement.Models.Entities;

namespace LibraryManagement.Models.Dtos.Requests
{
    public class CreateRequestDto
    {
        public required List<int> BookIds { get; set; }

        public BookBorrowingRequest ToBookBorrowingRequest(int requestorId)
        {
            List<BookBorrowingRequestDetail> details = BookIds.Select(id => new BookBorrowingRequestDetail()
            {
                BookId = id,
                Returned = false
            }).ToList();

            return new BookBorrowingRequest()
            {
                DateRequested = DateTime.UtcNow,
                Status = Status.Waiting,
                RequestorId = requestorId,
                BookBorrowingRequestDetails = details
            };
        }
    }
}