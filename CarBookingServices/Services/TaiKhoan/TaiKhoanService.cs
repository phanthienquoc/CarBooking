using CarBookingDomain.Context;
using CarBookingDomain.DTO.TaiKhoan;
using CarBookingDomain.Entity;
using CarBookingServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CarBookingServices.Services
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly CarBookingDbContext _dbContext;

        public TaiKhoanService(CarBookingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ThongTinTaiKhoanDTO> CreateAsync(TaiKhoanDTO taiKhoanDto)
        {
            var taiXe = new TaiXe();
            var khachHang = new KhachHang();
            var taiKhoan = new TaiKhoan
            {
                SoDienThoai = taiKhoanDto.SoDienThoai,
                Password = taiKhoanDto.Password,
                LoaiTaiKhoan = taiKhoanDto.LoaiTaiKhoan
            };

            _dbContext.TaiKhoan.Add(taiKhoan);
            if (taiKhoan.LoaiTaiKhoan == 0)
            {
                taiXe.SoDienThoai = taiKhoanDto.SoDienThoai;
                taiXe.HoTen = taiKhoanDto.HoTen;
                taiXe.BienSoXe = taiKhoanDto.BienSoXe;
                _dbContext.TaiXe.Add(taiXe);
            }

            else
            {
                khachHang.SoDienThoai = taiKhoanDto.SoDienThoai;
                khachHang.HoTen = taiKhoanDto.HoTen;
                khachHang.LoaiKhachHang = 1;
                khachHang.TaiKhoan = taiKhoan;
                _dbContext.KhachHang.Add(khachHang);
            }

            await _dbContext.SaveChangesAsync();

            var thongTinTaiKhoan = new ThongTinTaiKhoanDTO
            {
                BienSoXe = taiXe.BienSoXe,
                HoTen = taiKhoanDto.HoTen,
                SoDienThoai = taiKhoan.SoDienThoai,
                LoaiKhachHang = khachHang.LoaiKhachHang == 0 ? (int?)null : khachHang.LoaiKhachHang,
                LoaiTaiKhoan = taiKhoan.LoaiTaiKhoan
            };
            return thongTinTaiKhoan;
        }

        public async Task<ThongTinTaiKhoanDTO> FindAsync(string soDienThoai, string password)
        {
            var taiKhoan = await _dbContext.TaiKhoan
                .FirstOrDefaultAsync(tk => tk.SoDienThoai == soDienThoai && tk.Password == password);

            if (taiKhoan == null)
                return null;

            var thongTinTaiKhoan = new ThongTinTaiKhoanDTO();

            // Tai xe
            if (taiKhoan.LoaiTaiKhoan == 0)
            {
                var taiXe = await _dbContext.TaiXe.FirstOrDefaultAsync(tx => tx.SoDienThoai == soDienThoai);
                if (taiXe != null)
                {
                    thongTinTaiKhoan.BienSoXe = taiXe.BienSoXe;
                    thongTinTaiKhoan.HoTen = taiXe.HoTen;
                    thongTinTaiKhoan.LoaiTaiKhoan = taiKhoan.LoaiTaiKhoan;
                    thongTinTaiKhoan.SoDienThoai = taiKhoan.SoDienThoai;
                }
                else
                {
                    return null;
                }
            }

            // Khach Hang
            else
            {
                var khachHang = await _dbContext.KhachHang.FirstOrDefaultAsync(kh => kh.SoDienThoai == soDienThoai);
                if (khachHang != null)
                {
                    thongTinTaiKhoan.HoTen = khachHang.HoTen;
                    thongTinTaiKhoan.LoaiTaiKhoan = taiKhoan.LoaiTaiKhoan;
                    thongTinTaiKhoan.SoDienThoai = taiKhoan.SoDienThoai;
                    thongTinTaiKhoan.LoaiKhachHang = khachHang.LoaiKhachHang;
                }
                else
                {
                    return null;
                }
            }

            return thongTinTaiKhoan;
        }
    }
}