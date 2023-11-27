using CarBookingDomain.Models.ViewModel.DonHang;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CarBookingServices.Services.SocketService
{
    public class WebSocketService : WebSocketBehavior
    {
        public static string ChiTietKhachHangDonHang { get; set; }
        public static bool TaiXeChapNhan { get; set; } = false;
        public static bool DonHangThatBai { get; set; } = false;

        public static void GuiChiTietDonHangChoTaiXe(ChiTietDonHangVM chiTietDonHang)
        {
            ChiTietKhachHangDonHang = JsonConvert.SerializeObject(chiTietDonHang);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.Data == "true")
            {
                TaiXeChapNhan = true;
            }
            Sessions.Broadcast(ChiTietKhachHangDonHang);
        }
  
        public void startSocketHost()
        {
            WebSocketServer wssv = new WebSocketServer("ws://127.0.0.1:7890");
            wssv.AddWebSocketService<WebSocketService>("/webSocket");
            wssv.Start();
        }
    }
}