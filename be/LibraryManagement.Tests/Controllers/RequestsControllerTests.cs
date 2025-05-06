using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Controllers;
using LibraryManagement.Exceptions;
using LibraryManagement.Models.Dtos;
using LibraryManagement.Models.Dtos.Requests;
using LibraryManagement.Services.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;

namespace LibraryManagement.Tests.Controllers
{
    public class RequestsControllerTests
    {
        private Mock<IRequestsService> _requestsServiceMock;
        private RequestsController _controller;
        private Mock<HttpContext> _httpContextMock;

        [SetUp]
        public void Setup()
        {
            _requestsServiceMock = new Mock<IRequestsService>();
            _httpContextMock = new Mock<HttpContext>();

            _controller = new RequestsController(_requestsServiceMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = _httpContextMock.Object,
                    ActionDescriptor = new ControllerActionDescriptor(),
                    RouteData = new RouteData(),
                }
            };
        }

        private void SetUpHttpContextWithUser(string userId)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _httpContextMock.Setup(c => c.User).Returns(principal);
        }

        [Test]
        public async Task GetMyRequestsAsync_ValidUserAndParameters_ReturnsOkResultWithPagedResult()
        {
            // Arrange
            int userId = 1;
            SetUpHttpContextWithUser(userId.ToString());

            var expectedResult = new PagedResult<RequestDto>
            {
                Results = new List<RequestDto>(), // Add some dummy data if needed for more detailed test
                TotalRecordCount = 0
            };

            _requestsServiceMock.Setup(s => s.GetMyRequestsAsync(userId, 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetMyRequestsAsync();

            // Assert
            Assert.IsInstanceOf<ActionResult<PagedResult<RequestDto>>>(result);
            Assert.IsNotNull(result.Value);
            Assert.That(result.Value, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GetMyRequestsAsync_InvalidUser_ThrowsInvalidSubClaimException()
        {
            // Arrange
            SetUpHttpContextWithUser("abc"); // Setup with an invalid user id.

            // Act & Assert
            Assert.ThrowsAsync<InvalidSubClaimException>(async () => await _controller.GetMyRequestsAsync());
        }


        [Test]
        public async Task GetMyRequestByIdAsync_ValidUserAndId_ReturnsOkResultWithRequestDto()
        {
            // Arrange
            int userId = 1;
            int requestId = 10;
            SetUpHttpContextWithUser(userId.ToString());

            var expectedRequest = new RequestDto { Id = requestId, Requestor = new RequestUserDto() { FirstName = "", LastName = "", Email = "" }, Approver = null, Books = new List<RequestBookDto>(), Status = "Waiting" }; //  Populate as necessary
            _requestsServiceMock.Setup(s => s.GetMyRequestByIdAsync(requestId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRequest);

            // Act
            var result = await _controller.GetMyRequestByIdAsync(requestId);

            // Assert
            Assert.IsInstanceOf<ActionResult<RequestDto>>(result);
            Assert.IsNotNull(result.Value);
            Assert.That(result.Value, Is.EqualTo(expectedRequest));
        }

        [Test]
        public void GetMyRequestByIdAsync_InvalidUser_ThrowsInvalidSubClaimException()
        {
            // Arrange
            int requestId = 10;
            SetUpHttpContextWithUser("abc"); // Invalid User

            // Act & Assert
            Assert.ThrowsAsync<InvalidSubClaimException>(async () => await _controller.GetMyRequestByIdAsync(requestId));
        }

        [Test]
        public async Task CreateRequestAsync_ValidUserAndDto_ReturnsCreatedAtActionResult()
        {
            // Arrange
            int userId = 1;
            SetUpHttpContextWithUser(userId.ToString());

            var createRequestDto = new CreateRequestDto { BookIds = new List<int> { 1, 2, 3 } };
            var createdRequestDto = new RequestDto { Id = 1, Requestor = new RequestUserDto() { FirstName = "", LastName = "", Email = "" }, Approver = null, Books = new List<RequestBookDto>(), Status = "Waiting" };

            _requestsServiceMock.Setup(s => s.CreateRequestAsync(userId, createRequestDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdRequestDto);

            // Act
            var result = await _controller.CreateRequestAsync(createRequestDto);

            // Assert
            Assert.IsInstanceOf<ActionResult<RequestDto>>(result);
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            var createdAtResult = result.Result as CreatedAtActionResult;
            Assert.AreEqual(createdRequestDto, createdAtResult.Value);
        }

        [Test]
        public void CreateRequestAsync_InvalidUser_ThrowsInvalidSubClaimException()
        {
            // Arrange
            SetUpHttpContextWithUser("abc");
            var createRequestDto = new CreateRequestDto { BookIds = new List<int>() };

            // Act & Assert
            Assert.ThrowsAsync<InvalidSubClaimException>(async () => await _controller.CreateRequestAsync(createRequestDto));
        }

        [Test]
        public async Task GetMyAllowanceAsync_ValidUser_ReturnsOkResultWithAllowanceDto()
        {
            // Arrange
            int userId = 1;
            SetUpHttpContextWithUser(userId.ToString());
            var expectedAllowance = new AllowanceDto { RequestsAvailable = 5, RequestLimit = 5 };

            _requestsServiceMock.Setup(s => s.GetMyAllowanceAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAllowance);

            // Act
            var result = await _controller.GetMyAllowanceAsync();

            // Assert
            Assert.IsInstanceOf<ActionResult<AllowanceDto>>(result);
            Assert.IsNotNull(result.Value);
            Assert.That(result.Value, Is.EqualTo(expectedAllowance));
        }

        [Test]
        public void GetMyAllowanceAsync_InvalidUser_ThrowsInvalidSubClaimException()
        {
            // Arrange
            SetUpHttpContextWithUser("abc");

            // Act & Assert
            Assert.ThrowsAsync<InvalidSubClaimException>(async () => await _controller.GetMyAllowanceAsync());
        }

        [Test]
        public async Task GetRequestsAsync_ValidParameters_ReturnsOkResultWithPagedResult()
        {
            // Arrange
            var expectedResult = new PagedResult<RequestDto>
            {
                Results = new List<RequestDto>(),
                TotalRecordCount = 0
            };
            _requestsServiceMock.Setup(s => s.GetRequestsAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetRequestsAsync();

            // Assert
            Assert.IsInstanceOf<ActionResult<PagedResult<RequestDto>>>(result);
            Assert.IsNotNull(result.Value);
            Assert.That(result.Value, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task GetRequestByIdAsync_ValidId_ReturnsOkResultWithRequestDto()
        {
            // Arrange
            int requestId = 10;
            var expectedRequest = new RequestDto { Id = 1, Requestor = new RequestUserDto() { FirstName = "", LastName = "", Email = "" }, Approver = null, Books = new List<RequestBookDto>(), Status = "Waiting" };
            _requestsServiceMock.Setup(s => s.GetRequestByIdAsync(requestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRequest);

            // Act
            var result = await _controller.GetRequestByIdAsync(requestId);

            // Assert
            Assert.IsInstanceOf<ActionResult<RequestDto>>(result);
            Assert.IsNotNull(result.Value);
            Assert.That(result.Value, Is.EqualTo(expectedRequest));
        }

        [Test]
        public async Task ApproveRequestAsync_ValidIdAndUser_ReturnsOkResult()
        {
            // Arrange
            int requestId = 10;
            int approverId = 2;
            SetUpHttpContextWithUser(approverId.ToString());

            _requestsServiceMock.Setup(s => s.ApproveRequestAsync(requestId, approverId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ApproveRequestAsync(requestId);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public void ApproveRequestAsync_InvalidUser_ThrowsInvalidSubClaimException()
        {
            // Arrange
            int requestId = 10;
            SetUpHttpContextWithUser("abc");  // Invalid user

            // Act & Assert
            Assert.ThrowsAsync<InvalidSubClaimException>(async () => await _controller.ApproveRequestAsync(requestId));
        }

        [Test]
        public async Task RejectRequestAsync_ValidId_ReturnsOkResult()
        {
            // Arrange
            int requestId = 10;
            _requestsServiceMock.Setup(s => s.RejectRequestAsync(requestId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RejectRequestAsync(requestId);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
        }
    }
}
