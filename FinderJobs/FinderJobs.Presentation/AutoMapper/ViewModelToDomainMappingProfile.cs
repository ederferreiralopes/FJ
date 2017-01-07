using AutoMapper;
using FinderJobs.Domain.Entities;
using FinderJobs.MVC.ViewModels;

namespace FinderJobs.MVC.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<UsuarioViewModel, Usuario>();
            Mapper.CreateMap<HabilidadeViewModel, Habilidade>();
            Mapper.CreateMap<VagaViewModel, Domain.Entities.Vaga>();
        }
    }
}