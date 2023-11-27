using AutoMapper;
using CarBookingDomain.DTO.DonHang;
using CarBookingDomain.DTO.TaiXe;
using CarBookingDomain.Models.ViewModel.DonHang;
using CarBookingDomain.ViewModels.TaiXe;
using CarBookingServices.Interfaces;
using CarBookingServices.Services;
using CarBookingServices.Services.SocketService;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace CarBookingAPI.Controllers.TaiXe
{
    [ApiController]
    [Route("[controller]")]
    public class TaiXeController : Controller
    {
        private readonly IDonHangService _donHangService;
        private readonly ITaiXeService _taiXeService;
        private readonly IMapper _iMapper;
        public TaiXeController(ITaiXeService taiXeService, IMapper iMapper, IDonHangService donHangService)
        {
            _iMapper = iMapper;
            _taiXeService = taiXeService;
            _donHangService = donHangService;
        }

        [HttpPost]
        [Route("CapNhatViTri")]
        public IActionResult CapNhatViTri(ViTriTaiXeVM viTriTaiXeVM)
        {
            var viTriTaiXeDto = _iMapper.Map<ViTriTaiXeVM, ViTriTaiXeDTO>(viTriTaiXeVM);
            var luuThanhCong = _taiXeService.LuuThongTinViTriTaiXe(viTriTaiXeDto);
            if (luuThanhCong == true)
            {
                return Ok("Lưu vị trí thành công");
            }
            return BadRequest("Lưu vị trí không thành công");
        }

        [HttpPost]
        [Route("ChapNhanDonHang")]
        public IActionResult ChapNhanDonHang(string soDienThoaiTaiXe)
        {
            // xoa tai xe khoi message queue
            if (SocketServiceTaiXe.DonHangThatBai == false)
            {
                _taiXeService.XoaTaiXeKhoiMessageQueue(soDienThoaiTaiXe);
                return Ok("Đã chấp nhận đơn hàng");
            }
            return BadRequest("Đơn hàng đã hết thời gian chờ");
        }

        [HttpPost]
        [Route("TuChoiDonHang")]
        public async Task<IActionResult> TuChoiDonHang(string soDienThoaiTaiXe)
        {
            // xoa tai xe khoi message queue
            _taiXeService.XoaTaiXeKhoiMessageQueue(soDienThoaiTaiXe);

            // bat dau lai qua trinh tim tai xe
            var chiTietDonHangDto = await _donHangService.ChuanBiDonHang(DonHangService.DonHangDangXuLy);
            var chiTietDonHang = _iMapper.Map<ChiTietDonHangDTO, ChiTietDonHangVM>(chiTietDonHangDto);

            // broadcast thong tin don hang cho tai xe
            SocketServiceTaiXe.GuiChiTietDonHangChoTaiXe(chiTietDonHang);

            return Ok("Đã từ chối đơn hàng");
        }

        [HttpPost]
        [Route("TaiXeHuyDon")]

        public async Task<IActionResult> TaiXeHuyDon(ChiTietDonHangVM chiTietDonHang)
        {
            // trang thai = 0 nghia la don hang that bai
            chiTietDonHang.TrangThai = 0;
            var chiTietDonHangDto = _iMapper.Map<ChiTietDonHangVM, ChiTietDonHangDTO>(chiTietDonHang);
            await _donHangService.LuuLichSuDonHang(chiTietDonHangDto);

            return Ok("Đã huỷ đơn");
        }

        [HttpPost]
        [Route("TaiXeHoanThanhChuyenDi")]

        public async Task<IActionResult> TaiXeHoanThanhChuyenDi(ChiTietDonHangVM chiTietDonHang)
        {
            // trang thai = 0 nghia la don hang that bai
            chiTietDonHang.TrangThai = 1;
            var chiTietDonHangDto = _iMapper.Map<ChiTietDonHangVM, ChiTietDonHangDTO>(chiTietDonHang);
            await _donHangService.LuuLichSuDonHang(chiTietDonHangDto);

            return Ok("Chuyến đi đã hoàn thành");
        }
    }
}
