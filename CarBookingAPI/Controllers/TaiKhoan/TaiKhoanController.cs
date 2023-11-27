using AutoMapper;
using CarBookingDomain.DTO.TaiKhoan;
using CarBookingDomain.Models.ViewModel.TaiKhoan;
using CarBookingDomain.ViewModels.TaiKhoan;
using CarBookingServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace CarBookingAPI.Controllers.TaiKhoan
{
    [ApiController]
    [Route("[controller]")]
    public class TaiKhoanController : ControllerBase
    {
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly IMapper _iMapper;
        public TaiKhoanController(ITaiKhoanService taiKhoanService, IMapper iMapper)
        {
            _iMapper = iMapper;
            _taiKhoanService = taiKhoanService;
        }

        [HttpPost]
        [Route("TaoTaiKhoan")]
        public async Task<IActionResult> TaoTaiKhoan(TaiKhoanVM taiKhoanVM)
        {
            if (taiKhoanVM.SoDienThoai == string.Empty && taiKhoanVM.Password == string.Empty)
            {
                return BadRequest("Thông tin tài khoản không hợp lệ");
            }
            var taiKhoanDto = _iMapper.Map<TaiKhoanVM, TaiKhoanDTO>(taiKhoanVM);
            var thongTinTaiKhoanDto = await _taiKhoanService.CreateAsync(taiKhoanDto);
            var thongTinTaiKhoan = _iMapper.Map<ThongTinTaiKhoanDTO, ThongTinTaiKhoanVM>(thongTinTaiKhoanDto);
            return Ok(thongTinTaiKhoan);
        }

        [HttpPost]
        [Route("DangNhap")]
        public async Task<IActionResult> DangNhap(ThongTinDangNhapVM taiKhoanDangNhap)
        {
            if (taiKhoanDangNhap.SoDienThoai == string.Empty || taiKhoanDangNhap.Password == string.Empty)
            {
                return BadRequest("Thông tin đăng nhập không hợp lệ");
            }

            var thongTinTaiKhoanDto = await _taiKhoanService.FindAsync(taiKhoanDangNhap.SoDienThoai, taiKhoanDangNhap.Password);
            if (thongTinTaiKhoanDto == null)
            {
                return BadRequest("Thông tin tài khoản không tồn tại!");
            }
            
            var thongTinTaiKhoan = _iMapper.Map<ThongTinTaiKhoanDTO, ThongTinTaiKhoanVM>(thongTinTaiKhoanDto);
            return Ok(thongTinTaiKhoan);
        }
    }
}
