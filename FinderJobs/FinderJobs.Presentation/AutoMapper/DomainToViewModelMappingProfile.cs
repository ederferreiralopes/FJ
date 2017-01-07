using AutoMapper;
using FinderJobs.Domain.Entities;
using FinderJobs.MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinderJobs.MVC.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Usuario, UsuarioViewModel>();
            Mapper.CreateMap<Habilidade, HabilidadeViewModel>();
            Mapper.CreateMap<Domain.Entities.Vaga, VagaViewModel>();
        }
    }
}