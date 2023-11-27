using CarBookingDomain.DTO.TaiKhoan;
using System.Threading.Tasks;

namespace CarBookingServices.Interfaces
{
    public interface ITaiKhoanService
    {
        Task<ThongTinTaiKhoanDTO> CreateAsync(TaiKhoanDTO taiKhoan);
        Task<ThongTinTaiKhoanDTO> FindAsync(string soDienThoai, string password);
    }
}