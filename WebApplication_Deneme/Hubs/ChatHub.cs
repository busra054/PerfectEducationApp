using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using WebApplication_Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using WebApplication_Domain.Entities;

namespace WebApplication_Deneme.Hubs
{
    public class ChatHub : Hub
    {
        // DbContext almanın ters yolu: DI scope yaratıp alıyoruz
        private readonly IServiceScopeFactory _scopeFactory;

        public ChatHub(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task SendMessage(int senderId, int receiverId, string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm");
            // 1) Her iki uçtaki dinleyicilere yayın
            await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", senderId, message, timestamp);
            await Clients.User(senderId.ToString()).SendAsync("ReceiveMessage", senderId, message, timestamp);

            // 2) Veritabanına kaydet
            using var scope = _scopeFactory.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            ctx.Messages.Add(new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                MessageText = message,
                SentDate = DateTime.Now
            });
            await ctx.SaveChangesAsync();
        }
    }
}
