using System.Security.Claims;
using CarWash.Backend.Controllers;
using CarWash.Backend.DTOs.Auth;
using CarWash.Backend.DTOs.Booking;
using CarWash.Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarWash.Backend.Tests;

public class ControllerTests
{
    [Test]
    public async Task AuthController_Login_WhenServiceFails_ReturnsBadRequest()
    {
        var authService = new FakeAuthService
        {
            LoginResult = new AuthServiceResult<AuthResponse>
            {
                ErrorMessage = "Invalid email or password."
            }
        };

        var controller = new AuthController(authService);

        var result = await controller.Login(new LoginRequest
        {
            Email = "customer@example.com",
            Password = "wrong-password"
        });

        var badRequest = result.Result as BadRequestObjectResult;

        ClassicAssert.IsNotNull(badRequest);
        ClassicAssert.AreEqual(StatusCodes.Status400BadRequest, badRequest!.StatusCode);
        ClassicAssert.AreEqual("Invalid email or password.", badRequest.Value);
    }

    [Test]
    public async Task BookingsController_CreateBooking_WhenServiceReturnsNotFound_ReturnsNotFound()
    {
        var bookingService = new FakeBookingService
        {
            CreateBookingResult = new BookingServiceResult
            {
                IsNotFound = true,
                ErrorMessage = "Car not found."
            }
        };

        var controller = new BookingsController(bookingService);
        controller.ControllerContext = CreateControllerContextWithUserId(10);

        var result = await controller.CreateBooking(new CreateBookingRequest
        {
            CarId = 1,
            ServicePlanId = 2,
            BookingType = "WashNow",
            BookingDate = DateTime.UtcNow,
            Address = "Main Road"
        });

        var notFound = result.Result as NotFoundObjectResult;

        ClassicAssert.IsNotNull(notFound);
        ClassicAssert.AreEqual(StatusCodes.Status404NotFound, notFound!.StatusCode);
        ClassicAssert.AreEqual("Car not found.", notFound.Value);
    }

    [Test]
    public async Task WashersController_GetAssignedBookingById_WhenServiceReturnsNull_ReturnsNotFound()
    {
        var washerService = new FakeWasherService
        {
            AssignedBookingByIdResult = null
        };

        var controller = new WashersController(washerService);
        controller.ControllerContext = CreateControllerContextWithUserId(7);

        var result = await controller.GetAssignedBookingById(11);

        var notFound = result.Result as NotFoundObjectResult;

        ClassicAssert.IsNotNull(notFound);
        ClassicAssert.AreEqual(StatusCodes.Status404NotFound, notFound!.StatusCode);
        ClassicAssert.AreEqual("Assigned booking not found.", notFound.Value);
    }

    private static ControllerContext CreateControllerContextWithUserId(int userId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthType"))
            }
        };
    }
}
