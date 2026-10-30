using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ElearningAPI.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private static readonly Dictionary<string, string> _userConnections = new();
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections[userId] = Context.ConnectionId;
                _logger.LogInformation($"User {userId} connected with connection {Context.ConnectionId}");
                
                // Csatlakozás a felhasználó csatornájához
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
                
                // Értesítés a felhasználó csoportjának
                await Clients.Group($"user-{userId}").SendAsync("UserConnected", new
                {
                    UserId = userId,
                    ConnectionId = Context.ConnectionId,
                    Timestamp = DateTime.UtcNow
                });
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections.Remove(userId);
                _logger.LogInformation($"User {userId} disconnected");
                
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinCourseGroup(int courseId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"course-{courseId}");
                _logger.LogInformation($"User {userId} joined course group {courseId}");
            }
        }

        public async Task LeaveCourseGroup(int courseId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"course-{courseId}");
        }

        public async Task SendMessageToCourse(int courseId, string message)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = Context.User?.FindFirst(ClaimTypes.GivenName)?.Value + " " + 
                          Context.User?.FindFirst(ClaimTypes.Surname)?.Value;

            await Clients.Group($"course-{courseId}").SendAsync("ReceiveCourseMessage", new
            {
                CourseId = courseId,
                UserId = userId,
                UserName = userName,
                Message = message,
                Timestamp = DateTime.UtcNow
            });
        }

        public async Task SendNotificationToUser(string targetUserId, string message, string type = "info")
        {
            if (_userConnections.TryGetValue(targetUserId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotification", new
                {
                    Message = message,
                    Type = type,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // Statikus metódus a service rétegből való hozzáféréshez
        public static async Task SendNotificationToUserAsync(IHubContext<NotificationHub> hubContext, string userId, string message, string type = "info")
        {
            await hubContext.Clients.Group($"user-{userId}").SendAsync("ReceiveNotification", new
            {
                Message = message,
                Type = type,
                Timestamp = DateTime.UtcNow
            });
        }

        public static async Task SendCourseNotificationAsync(IHubContext<NotificationHub> hubContext, int courseId, string message, object? data = null)
        {
            await hubContext.Clients.Group($"course-{courseId}").SendAsync("ReceiveCourseNotification", new
            {
                CourseId = courseId,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    public class NotificationMessage
    {
        public string Type { get; set; } = "info"; // info, success, warning, error
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}