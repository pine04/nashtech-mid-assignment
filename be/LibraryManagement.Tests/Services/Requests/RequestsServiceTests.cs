using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Data;
using LibraryManagement.Exceptions;
using LibraryManagement.Models.Dtos.Requests;
using LibraryManagement.Models.Entities;
using LibraryManagement.Services.Requests;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LibraryManagement.Tests.Services.Requests
{
    public class RequestsServiceTests
    {
        private RequestsService _requestsService;
        private ApplicationDbContext _dbContext;
        private DbContextOptions<ApplicationDbContext> _dbContextOptions;

        [SetUp]
        public void Setup()
        {
            // Use an in-memory database for testing
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestRequestDatabase") // Give it a unique name
                .Options;

            // Create a new instance of ApplicationDbContext for each test
            _dbContext = new ApplicationDbContext(_dbContextOptions); // Pass null for config and seeder

            // Ensure the database is clean before each test.
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            _requestsService = new RequestsService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose the db context
            _dbContext.Dispose();
        }

        // Helper method to create a user
        private User CreateUser(int id, string firstName, string lastName, string email, Role role)
        {
            return new User
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Role = role,
                Password = ""
            };
        }

        // Helper method to create a book
        private Book CreateBook(int id, string title, string author, int quantity, int available)
        {
            return new Book
            {
                Id = id,
                Title = title,
                Author = author,
                Quantity = quantity,
                Available = available,
                Description = ""
            };
        }

        // Helper method to create a borrowing request
        private BookBorrowingRequest CreateBorrowingRequest(int id, int requestorId, Status status, DateTime dateRequested, int? approverId = null)
        {
            return new BookBorrowingRequest
            {
                Id = id,
                RequestorId = requestorId,
                Status = status,
                DateRequested = dateRequested,
                ApproverId = approverId
            };
        }

        // Helper method to create a borrowing request detail
        private BookBorrowingRequestDetail CreateBorrowingRequestDetail(int bookBorrowingRequestId, int bookId, bool returned)
        {
            return new BookBorrowingRequestDetail
            {
                BookBorrowingRequestId = bookBorrowingRequestId,
                BookId = bookId,
                Returned = returned
            };
        }

        [Test]
        public async Task GetMyRequestByIdAsync_ValidRequest_ReturnsRequestDto()
        {
            // Arrange
            User requestor = CreateUser(1, "John", "Doe", "john.doe@example.com", Role.NormalUser);
            User approver = CreateUser(2, "Jane", "Smith", "jane.smith@example.com", Role.SuperUser);
            Book book1 = CreateBook(101, "Book A", "Author 1", 3, 3);
            Book book2 = CreateBook(102, "Book B", "Author 2", 2, 2);

            BookBorrowingRequest request = CreateBorrowingRequest(1, requestor.Id, Status.Approved, DateTime.Now, approver.Id);

            BookBorrowingRequestDetail detail1 = CreateBorrowingRequestDetail(request.Id, book1.Id, false);
            BookBorrowingRequestDetail detail2 = CreateBorrowingRequestDetail(request.Id, book2.Id, false);

            request.Requestor = requestor;
            request.Approver = approver;
            request.BookBorrowingRequestDetails.Add(detail1);
            request.BookBorrowingRequestDetails.Add(detail2);

            _dbContext.Users.AddRange(requestor, approver);
            _dbContext.Books.AddRange(book1, book2);
            _dbContext.BookBorrowingRequests.Add(request);

            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _requestsService.GetMyRequestByIdAsync(request.Id, requestor.Id, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(request.Id));
            Assert.That(result.Requestor.FirstName, Is.EqualTo(requestor.FirstName));
            Assert.That(result.Approver?.FirstName, Is.EqualTo(approver.FirstName));
            Assert.That(result.Books.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetMyRequestByIdAsync_InvalidRequestId_ThrowsNotFoundException()
        {
            // Arrange (no requests in the database)

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _requestsService.GetMyRequestByIdAsync(999, 1, CancellationToken.None));
        }

        [Test]
        public void GetMyRequestByIdAsync_WrongUser_ThrowsForbiddenException()
        {
            // Arrange
            User requestor = CreateUser(1, "John", "Doe", "john.doe@example.com", Role.NormalUser);
            BookBorrowingRequest request = CreateBorrowingRequest(1, requestor.Id, Status.Waiting, DateTime.Now);
            request.Requestor = requestor;

            _dbContext.Users.Add(requestor);
            _dbContext.BookBorrowingRequests.Add(request);
            _dbContext.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<ForbiddenException>(async () =>
                await _requestsService.GetMyRequestByIdAsync(request.Id, 2, CancellationToken.None));
        }

        [Test]
        public async Task GetMyRequestsAsync_MultipleRequests_ReturnsPagedResult()
        {
            // Arrange
            User requestor = CreateUser(1, "John", "Doe", "john.doe@example.com", Role.NormalUser);
            _dbContext.Users.Add(requestor);

            //create 3 requests
            var requests = new List<BookBorrowingRequest>
            {
                CreateBorrowingRequest(1, requestor.Id, Status.Waiting, DateTime.Now.AddDays(-1)),
                CreateBorrowingRequest(2, requestor.Id, Status.Approved, DateTime.Now.AddDays(-2)),
                CreateBorrowingRequest(3, requestor.Id, Status.Rejected, DateTime.Now.AddDays(-3))
            };

            requests.ForEach(r => r.Requestor = requestor);
            _dbContext.BookBorrowingRequests.AddRange(requests);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _requestsService.GetMyRequestsAsync(requestor.Id, 1, 10, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.TotalRecordCount, Is.EqualTo(3));
            Assert.That(result.Results.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task GetMyRequestsAsync_NoRequests_ReturnsEmptyPagedResult()
        {
            // Arrange
            User requestor = CreateUser(1, "John", "Doe", "john.doe@example.com", Role.NormalUser);
            _dbContext.Users.Add(requestor);

            // Act
            var result = await _requestsService.GetMyRequestsAsync(requestor.Id, 1, 10, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.TotalRecordCount, Is.EqualTo(0));
            Assert.IsEmpty(result.Results);
        }

        [Test]
        public async Task CreateRequestAsync_ValidRequest_CreatesRequestAndDetails()
        {
            // Arrange
            User requestor = CreateUser(1, "John", "Doe", "john.doe@example.com", Role.NormalUser);
            Book book1 = CreateBook(101, "Book A", "Author 1", 3, 3);
            Book book2 = CreateBook(102, "Book B", "Author 2", 2, 2);

            _dbContext.Users.Add(requestor);
            _dbContext.Books.AddRange(book1, book2);
            await _dbContext.SaveChangesAsync();

            var createRequestDto = new CreateRequestDto
            {
                BookIds = new List<int> { book1.Id, book2.Id }
            };

            // Act
            await _requestsService.CreateRequestAsync(requestor.Id, createRequestDto, CancellationToken.None);

            // Assert
            // Verify that the request is created
            var request = await _dbContext.BookBorrowingRequests
                .Include(r => r.BookBorrowingRequestDetails)
                .FirstOrDefaultAsync(r => r.RequestorId == requestor.Id);

            Assert.IsNotNull(request);
            Assert.That(request.RequestorId, Is.EqualTo(requestor.Id));
            Assert.That(request.Status, Is.EqualTo(Status.Waiting));
            Assert.That(request.BookBorrowingRequestDetails.Count, Is.EqualTo(2));

            // Verify that the request details are created and linked to the correct books
            var detail1 = request.BookBorrowingRequestDetails.FirstOrDefault(d => d.BookId == book1.Id);
            var detail2 = request.BookBorrowingRequestDetails.FirstOrDefault(d => d.BookId == book2.Id);

            Assert.IsNotNull(detail1);
            Assert.IsNotNull(detail2);
            Assert.IsFalse(detail1.Returned);
            Assert.IsFalse(detail2.Returned);

            // Verify that the available quantity of the books is reduced
            var updatedBook1 = await _dbContext.Books.FindAsync(book1.Id);
            var updatedBook2 = await _dbContext.Books.FindAsync(book2.Id);

            Assert.That(updatedBook1?.Available, Is.EqualTo(2));
            Assert.That(updatedBook2?.Available, Is.EqualTo(1));
        }

        [Test]
        public void CreateRequestAsync_ExceedMaxBooksPerRequest_ThrowsMaxBooksPerRequestException()
        {
            // Arrange
            User requestor = CreateUser(1, "John", "Doe", "john.doe@example.com", Role.NormalUser);
            _dbContext.Users.Add(requestor);

            var createRequestDto = new CreateRequestDto
            {
                BookIds = new List<int> { 1, 2, 3, 4, 5, 6 } // Exceeds the limit of 5
            };

            // Act & Assert
            Assert.ThrowsAsync<TooManyBooksException>(async () =>
                await _requestsService.CreateRequestAsync(requestor.Id, createRequestDto, CancellationToken.None));
        }

        [Test]
        public void CreateRequestAsync_ExceedMaxRequestsPerMonth_ThrowsMaxRequestsPerMonthException()
        {
            // Arrange
            User requestor = CreateUser(1, "John", "Doe", "john.doe@example.com", Role.NormalUser);
            _dbContext.Users.Add(requestor);

            // Create 3 requests for the current month
            _dbContext.BookBorrowingRequests.AddRange(
                CreateBorrowingRequest(1, requestor.Id, Status.Waiting, DateTime.Now),
                CreateBorrowingRequest(2, requestor.Id, Status.Waiting, DateTime.Now),
                CreateBorrowingRequest(3, requestor.Id, Status.Waiting, DateTime.Now)
            );
            _dbContext.SaveChanges();

            var createRequestDto = new CreateRequestDto
            {
                BookIds = new List<int> { 1 }
            };

            // Act & Assert
            Assert.ThrowsAsync<MonthlyLimitReachedException>(async () =>
                await _requestsService.CreateRequestAsync(requestor.Id, createRequestDto, CancellationToken.None));
        }

        [Test]
        public void CreateRequestAsync_BookNotAvailable_ThrowsBookNotAvailableException()
        {
            // Arrange
            User requestor = CreateUser(1, "John", "Doe", "john.doe@example.com", Role.NormalUser);
            Book book1 = CreateBook(101, "Book A", "Author 1", 3, 0); // Available quantity is 0

            _dbContext.Users.Add(requestor);
            _dbContext.Books.Add(book1);
            _dbContext.SaveChanges();

            var createRequestDto = new CreateRequestDto
            {
                BookIds = new List<int> { book1.Id }
            };

            // Act & Assert
            Assert.ThrowsAsync<BookNotAvailableException>(async () =>
                await _requestsService.CreateRequestAsync(requestor.Id, createRequestDto, CancellationToken.None));
        }

        [Test]
        public async Task GetAllowanceAsync_UserWithNoRequests_ReturnsFullAllowance()
        {
            // Arrange
            User requestor = CreateUser(1, "John", "Doe", "john.doe@example.com", Role.NormalUser);
            _dbContext.Users.Add(requestor);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _requestsService.GetMyAllowanceAsync(requestor.Id, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.RequestLimit, Is.EqualTo(3)); // Assuming MaxRequestsPerMonth is 3
            Assert.That(result.RequestsAvailable, Is.EqualTo(3));
        }

        [Test]
        public async Task GetAllowanceAsync_UserWithSomeRequests_ReturnsRemainingAllowance()
        {
            // Arrange
            User requestor = CreateUser(1, "John", "Doe", "john.doe@example.com", Role.NormalUser);
            _dbContext.Users.Add(requestor);

            // Create 2 requests for the user
            _dbContext.BookBorrowingRequests.AddRange(
                CreateBorrowingRequest(1, requestor.Id, Status.Waiting, DateTime.Now),
                CreateBorrowingRequest(2, requestor.Id, Status.Approved, DateTime.Now)
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _requestsService.GetMyAllowanceAsync(requestor.Id, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.RequestLimit, Is.EqualTo(3));
            Assert.That(result.RequestsAvailable, Is.EqualTo(1));
        }

        [Test]
        public async Task ApproveRequestAsync_ValidRequest_ApprovesRequest()
        {
            // Arrange
            User requestor = CreateUser(1, "John", "Doe", "john.doe@example.com", Role.NormalUser);
            User approver = CreateUser(2, "Jane", "Smith", "jane.smith@example.com", Role.SuperUser);
            BookBorrowingRequest request = CreateBorrowingRequest(1, requestor.Id, Status.Waiting, DateTime.Now);
            request.Requestor = requestor;

            _dbContext.Users.AddRange(requestor, approver);
            _dbContext.BookBorrowingRequests.Add(request);
            await _dbContext.SaveChangesAsync();

            // Act
            await _requestsService.ApproveRequestAsync(request.Id, approver.Id, CancellationToken.None);

            // Assert
            var updatedRequest = await _dbContext.BookBorrowingRequests.FindAsync(request.Id);
            Assert.IsNotNull(updatedRequest);
            Assert.That(updatedRequest.Status, Is.EqualTo(Status.Approved));
            Assert.That(updatedRequest.ApproverId, Is.EqualTo(approver.Id));
        }

        [Test]
        public void ApproveRequestAsync_InvalidRequestId_ThrowsNotFoundException()
        {
            // Arrange (no requests)
            User approver = CreateUser(2, "Jane", "Smith", "jane.smith@example.com", Role.SuperUser);
            _dbContext.Users.Add(approver);
            _dbContext.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _requestsService.ApproveRequestAsync(999, approver.Id, CancellationToken.None));
        }

        [Test]
        public void ApproveRequestAsync_RequestNotWaiting_ThrowsRequestAlreadySettledException()
        {
            // Arrange
            User requestor = CreateUser(1, "John", "Doe", "john.doe@example.com", Role.NormalUser);
            User approver = CreateUser(2, "Jane", "Smith", "jane.smith@example.com", Role.SuperUser);
            BookBorrowingRequest request = CreateBorrowingRequest(1, requestor.Id, Status.Approved, DateTime.Now); // Status is already Approved
            request.Requestor = requestor;

            _dbContext.Users.AddRange(requestor, approver);
            _dbContext.BookBorrowingRequests.Add(request);
            _dbContext.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<RequestAlreadySettledException>(async () =>
                await _requestsService.ApproveRequestAsync(request.Id, approver.Id, CancellationToken.None));
        }

        [Test]
        public async Task RejectRequestAsync_ValidRequest_RejectsRequestAndIncrementsAvailableBooks()
        {
            // Arrange
            User requestor = CreateUser(1, "John", "Doe", "john.doe@example.com", Role.NormalUser);
            Book book1 = CreateBook(101, "Book A", "Author 1", 3, 1); // Available = 1
            Book book2 = CreateBook(102, "Book B", "Author 2", 2, 0); // Available = 0

            BookBorrowingRequest request = CreateBorrowingRequest(1, requestor.Id, Status.Waiting, DateTime.Now);

            BookBorrowingRequestDetail detail1 = CreateBorrowingRequestDetail(request.Id, book1.Id, false);
            BookBorrowingRequestDetail detail2 = CreateBorrowingRequestDetail(request.Id, book2.Id, false);

            request.Requestor = requestor;
            request.BookBorrowingRequestDetails.Add(detail1);
            request.BookBorrowingRequestDetails.Add(detail2);

            _dbContext.Users.Add(requestor);
            _dbContext.Books.AddRange(book1, book2);
            _dbContext.BookBorrowingRequests.Add(request);
            await _dbContext.SaveChangesAsync();

            // Act
            await _requestsService.RejectRequestAsync(request.Id, CancellationToken.None);

            // Assert
            var updatedRequest = await _dbContext.BookBorrowingRequests.FindAsync(request.Id);
            Assert.IsNotNull(updatedRequest);
            Assert.That(updatedRequest.Status, Is.EqualTo(Status.Rejected));

            var updatedBook1 = await _dbContext.Books.FindAsync(book1.Id);
            var updatedBook2 = await _dbContext.Books.FindAsync(book2.Id);
            Assert.That(updatedBook1?.Available, Is.EqualTo(2)); // Available incremented from 1 to 2
            Assert.That(updatedBook2?.Available, Is.EqualTo(1)); // Available incremented from 0 to 1
        }

        [Test]
        public void RejectRequestAsync_InvalidRequestId_ThrowsNotFoundException()
        {
            // Arrange (no requests)

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _requestsService.RejectRequestAsync(999, CancellationToken.None));
        }

        [Test]
        public void RejectRequestAsync_RequestNotWaiting_ThrowsRequestAlreadySettledException()
        {
            // Arrange
            User requestor = CreateUser(1, "John", "Doe", "john.doe@example.com", Role.NormalUser);
            BookBorrowingRequest request = CreateBorrowingRequest(1, requestor.Id, Status.Approved, DateTime.Now); // Status is Approved
            request.Requestor = requestor;

            _dbContext.Users.Add(requestor);
            _dbContext.BookBorrowingRequests.Add(request);
            _dbContext.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<RequestAlreadySettledException>(async () =>
                await _requestsService.RejectRequestAsync(request.Id, CancellationToken.None));
        }
    }
}
