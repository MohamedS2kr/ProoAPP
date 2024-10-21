using Microsoft.AspNetCore.SignalR;
using Proo.Infrastructer.Data.Context;

namespace Proo.APIs.Hubs
{
    public class ChatHub :Hub
    {
        private readonly ApplicationDbContext _context; 
        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public async Task SendMessage(string conversationId, string senderId, string message)
        {
            // إنشاء رسالة جديدة
            #region Save in database
            //var newMessage = new Message
            //{
            //    ConversationId = int.Parse(conversationId),
            //    SenderId = senderId,
            //    Text = message,
            //    SentAt = DateTime.UtcNow
            //};

            // حفظ الرسالة في قاعدة البيانات
            //_context.Messages.Add(newMessage);
            //await _context.SaveChangesAsync(); 
            #endregion

            
            await Clients.Group(conversationId).SendAsync("ReceiveMessage", senderId, message);
        }

        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        
        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }
    }
}
