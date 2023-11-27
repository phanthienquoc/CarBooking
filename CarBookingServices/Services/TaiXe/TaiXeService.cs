using CarBookingDomain.Context;
using CarBookingDomain.DTO.TaiXe;
using CarBookingServices.Interfaces;
using GeoCoordinatePortable;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CarBookingServices.Services
{
    public class TaiXeService : ITaiXeService
    {
        private readonly CarBookingDbContext _dbContext;
        public static ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
        static AutoResetEvent messageEnqueuedEvent = new AutoResetEvent(false);

        public TaiXeService(CarBookingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool LuuThongTinViTriTaiXe(ViTriTaiXeDTO viTriTaiXe)
        {
            string viTriTaiXeJson = JsonConvert.SerializeObject(viTriTaiXe);
            return ThemMessageVaoCuoiQueue(viTriTaiXeJson) != 0 ? true : false;
        }

        static int ThemMessageVaoCuoiQueue(string message)
        {
            if (messageQueue.Count == 20)
            {
                messageQueue.Clear();
            }
            messageQueue.Enqueue(message);
            messageEnqueuedEvent.Set(); // Signal that a message has been enqueued

            return messageQueue.Count;
        }

        public async Task<TaiXeDTO> TimTaiXePhuHopTrongMessageQueueAsync(double latitudeKhachHang,
            double longtitudeKhachHang, double latitudeDiemDen,
            double longtitudeDiemDen)
        {
            if (messageQueue.Count == 0)
            {
                return null;
            }

            string soDienThoaiTaiXeDuocChon = string.Empty;
            var locationKhachHang = new GeoCoordinate(latitudeKhachHang, longtitudeKhachHang);
            var locationTaiXe = new GeoCoordinate();
            double KhoangCachTaiXeGanNhat = 0.0;
            double KhoangCachDenTaiXeHienTai;
            double TongChieuDaiQuangDuong = locationKhachHang.GetDistanceTo(new GeoCoordinate(latitudeDiemDen, longtitudeDiemDen));
            JObject json;
            foreach (var message in messageQueue)
            {
                json = JObject.Parse(message);
                locationTaiXe.Latitude = (double)json["LatitudeTaiXe"];
                locationTaiXe.Longitude = (double)json["LongtitudeTaiXe"];
                KhoangCachDenTaiXeHienTai = locationKhachHang.GetDistanceTo(locationTaiXe);
                if (KhoangCachTaiXeGanNhat == 0.0 || KhoangCachDenTaiXeHienTai < KhoangCachTaiXeGanNhat)
                {
                    KhoangCachTaiXeGanNhat = KhoangCachDenTaiXeHienTai;
                    soDienThoaiTaiXeDuocChon = (string)json["SoDienThoaiTaiXe"];
                }
            }

            var taiXe = await _dbContext.TaiXe
                .FirstOrDefaultAsync(tx => tx.SoDienThoai == soDienThoaiTaiXeDuocChon);

            if (taiXe == null)
            {
                return null;
            }

            var taiXeDto = new TaiXeDTO
            {
                SoDienThoai = taiXe.SoDienThoai,
                HoTen = taiXe.HoTen,
                BienSoXe = taiXe.BienSoXe,

                // Gia tien 6000 vnd 1 km, khoangCachTaiXeGanNhatTinhTheo met
                GiaTienUocTinh = (decimal)(TongChieuDaiQuangDuong * 6),

                // tinh trung binh 1km se di mat 2 phut
                ThoiGianDonUocTinh = DateTime.Now.AddMinutes(KhoangCachTaiXeGanNhat * 2)
            };

            return taiXeDto;
        }

        public void XoaTaiXeKhoiMessageQueue(string soDienThoaiTaiXe)
        {
            JObject json;
            string deletedMessage;
            foreach (var message in messageQueue)
            {
                json = JObject.Parse(message);
                if ((string)json["SoDienThoaiTaiXe"] == soDienThoaiTaiXe)
                {
                    messageQueue.TryDequeue(out deletedMessage);
                }
            }
        }
    }
}