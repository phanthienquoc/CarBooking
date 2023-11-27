using AutoMapper;
using CarBookingDomain.DTO.DonHang;
using CarBookingDomain.DTO.TaiKhoan;
using CarBookingDomain.DTO.TaiXe;
using CarBookingDomain.Models.ViewModel.DonHang;
using CarBookingDomain.Models.ViewModel.TaiKhoan;
using CarBookingDomain.ViewModels.DonHang;
using CarBookingDomain.ViewModels.TaiXe;

namespace CarBookingDomain.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TaiKhoanVM, TaiKhoanDTO>();
            CreateMap<ThongTinTaiKhoanDTO, ThongTinTaiKhoanVM>();
            CreateMap<ViTriTaiXeVM, ViTriTaiXeDTO>();
            CreateMap<TaoDonHangVM, TaoDonHangDTO>();
            CreateMap<ChiTietDonHangDTO, ChiTietDonHangVM>();
            CreateMap<ChiTietDonHangVM, ChiTietDonHangDTO>();
        }
    }
}