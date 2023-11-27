using AutoMapper;
using CarBookingDomain.DTO.DonHang;
using CarBookingDomain.Models.ViewModel.DonHang;
using CarBookingDomain.ViewModels.DonHang;
using CarBookingServices.Interfaces;
using CarBookingServices.Services;
using CarBookingServices.Services.SocketService;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CarBookingAPI.Controllers.DonHang
{
    [ApiController]
    [Route("[controller]")]
    public class DonHangController : ControllerBase
    {

        private readonly IDonHangService _donHangService;
        private readonly IMapper _iMapper;
        public DonHangController(IMapper iMapper, IDonHangService donHangService)
        {
            _iMapper = iMapper;
            _donHangService = donHangService;
        }

        [HttpPost]
        [Route("TaoDonHang")]
        [SwaggerOperation(Summary = "Loại khách hàng = 0 là khách hàng thường, loại khách hàng = 1 là khách hàng vip." +
            "Trạng thái đơn hàng. 0 là thất bại, 1 là thành công, 2 là đang thực hiện." +
            "Loại tài khoản = 0 là tài xế, loại tài khoản = 1 là khách hàng.")]
        public async Task<IActionResult> TaoDonHangAsync(TaoDonHangVM donHang)
        {
            SocketServiceTaiXe.DonHangThatBai = false;
            SocketServiceTaiXe.TaiXeChapNhan = false;
            var timeout = TimeSpan.FromSeconds(10);
            var cancellationToken = new CancellationTokenSource(timeout);

            try
            {
                var chiTietDonHangDto = await _donHangService.ChuanBiDonHang(donHang);
                var chiTietDonHang = _iMapper.Map<ChiTietDonHangDTO, ChiTietDonHangVM>(chiTietDonHangDto);

                // broadcast thong tin don hang cho tai xe
                SocketServiceTaiXe.GuiChiTietDonHangChoTaiXe(chiTietDonHang);

                // cho tai xe chap nhan don hang
                while (SocketServiceTaiXe.TaiXeChapNhan == false)
                {
                    Thread.Sleep(8000);
                    cancellationToken.Token.ThrowIfCancellationRequested();
                }
                return Ok(chiTietDonHang);
            }

            catch (OperationCanceledException)
            {
                SocketServiceTaiXe.DonHangThatBai = true;
                DonHangService.DonHangDangXuLy = null;

                return BadRequest("Tất cả tài xế đều đang bận");
            }
        }
    }
}
