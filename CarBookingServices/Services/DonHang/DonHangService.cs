using AutoMapper;
using CarBookingDomain.Context;
using CarBookingDomain.DTO.DonHang;
using CarBookingDomain.DTO.TaiXe;
using CarBookingDomain.Entity;
using CarBookingDomain.ViewModels.DonHang;
using CarBookingServices.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CarBookingServices.Services
{
    public class DonHangService : IDonHangService
    {
        private readonly ITaiXeService _taiXeService;
        private readonly IMapper _iMapper;
        private readonly CarBookingDbContext _dbContext;

        public static TaoDonHangVM DonHangDangXuLy { get; set; }

        public DonHangService(ITaiXeService taiXeService, IMapper iMapper, CarBookingDbContext dbContext)
        {
            _dbContext = dbContext;
            _taiXeService = taiXeService;
            _iMapper = iMapper;
        }

        public ChiTietDonHangDTO TaoChiTietDonHang(TaiXeDTO taiXePhuTrach, TaoDonHangDTO donHang)
        {
            var chiTietDonHang = new ChiTietDonHangDTO
            {
                SoDienThoaiKhachHang = donHang.SoDienThoaiKhachHang,
                TenKhachHang = donHang.TenKhachHang,
                DiemDon = donHang.DiemDon,
                DiemDen = donHang.DiemDen,
                ThoiGianDon = donHang.ThoiGianDon.HasValue ? donHang.ThoiGianDon.Value : taiXePhuTrach.ThoiGianDonUocTinh,
                GioDatXe = donHang.GioDatXe,
                GiaTien = Math.Round(taiXePhuTrach.GiaTienUocTinh, 0),
                TenTaiXe = taiXePhuTrach.HoTen,
                SoDienThoaiTaiXe = taiXePhuTrach.SoDienThoai,
                TrangThai = 2 // trang thai = 2 la dang thuc hien
            };

            return chiTietDonHang;
        }

        public async Task<ChiTietDonHangDTO> ChuanBiDonHang(TaoDonHangVM donHang)
        {
            DonHangDangXuLy = donHang;
            var taiXePhuHop = await _taiXeService.TimTaiXePhuHopTrongMessageQueueAsync(donHang.LatitudeKhachHang, donHang.LongtitudeKhachHang,
                donHang.LatitudeDiemDen, donHang.LongtitudeDiemDen);
            if (taiXePhuHop == null)
            {
                return null;
            }

            var donHangDto = _iMapper.Map<TaoDonHangVM, TaoDonHangDTO>(donHang);
            var chiTietDonHangDto = TaoChiTietDonHang(taiXePhuHop, donHangDto);
            return chiTietDonHangDto;
        }

        public async Task LuuLichSuDonHang(ChiTietDonHangDTO chiTietDonHang)
        {
            var idKhachHang = _dbContext.KhachHang.Where(kh => kh.SoDienThoai == chiTietDonHang.SoDienThoaiKhachHang).FirstOrDefault().IdKhachHang;
            var idTaiXe = _dbContext.TaiXe.Where(tx => tx.SoDienThoai == chiTietDonHang.SoDienThoaiTaiXe).FirstOrDefault().IdTaiXe;

            var donHang = new DonHang
            {
                DiemDen = chiTietDonHang.DiemDen,
                DiemDon = chiTietDonHang.DiemDon,
                GioDatXe = chiTietDonHang.GioDatXe,
                IdKhachHang = idKhachHang,
                IdTaiXe = idTaiXe,
                GiaTien = chiTietDonHang.GiaTien,
                TrangThai = chiTietDonHang.TrangThai,
                ThoiGianDon = chiTietDonHang.ThoiGianDon
            };

            _dbContext.DonHang.Add(donHang);

            await _dbContext.SaveChangesAsync();
        }
    }
}
