using System;

namespace CarBookingDomain.Models.ViewModel.DonHang
{
    public class ChiTietDonHangVM
    {
        public string SoDienThoaiKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string DiemDon { get; set; }
        public string DiemDen { get; set; }
        public DateTime ThoiGianDon { get; set; }
        public DateTime GioDatXe { get; set; }
        public decimal GiaTien { get; set; }
        public string TenTaiXe { get; set; }
        public string SoDienThoaiTaiXe { get; set; }
        public string BienSoXe { get; set; }
        public int TrangThai { get; set; }
    }
}