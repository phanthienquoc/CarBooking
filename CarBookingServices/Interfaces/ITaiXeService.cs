using CarBookingDomain.DTO.DonHang;
using CarBookingDomain.DTO.TaiXe;
using System.Threading.Tasks;

namespace CarBookingServices.Interfaces
{
    public interface ITaiXeService
    {
        bool LuuThongTinViTriTaiXe(ViTriTaiXeDTO viTriTaiXe);
        Task<TaiXeDTO> TimTaiXePhuHopTrongMessageQueueAsync(double latitudeKhachHang,
            double longtitudeKhachHang, double latitudeDiemDen,
            double longtitudeDiemDen);

        void XoaTaiXeKhoiMessageQueue(string soDienThoaiTaiXe);
    }
}
