using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HubManPractices.Models;
using HubManPractices.Repository;
using HubManPractices.Repository.Repositories;
using AutoMapper;
using HubManPractices.Service.ViewModels;

namespace HubManPractices.Service
{
    public class ResellerService : IResellerService
    {
        private readonly IResellerRepository resellersRepository;

        private readonly ApplicationEntities ApplicationEntity;
        public ResellerService(ApplicationEntities Ae, IResellerRepository RR)
        {
            this.resellersRepository = RR;
            this.ApplicationEntity = Ae;
        }

        public IEnumerable<Reseller> GetResellers()
        {
            return resellersRepository.GetMany(r => r.IsDeleted == false);
        }
        public void SaveReseller()
        {
            ApplicationEntity.Commit();
        }

        public IEnumerable<ResellerViewModel> MapToViewModel(IEnumerable<Reseller> resellers)
        {
           return Mapper.Map<IEnumerable<Reseller>, IEnumerable<ResellerViewModel>>(resellers);
        }

        public IEnumerable<Reseller> GetUserReseller(string UserID)
        {
            return resellersRepository.GetUserReseller(UserID);
        }

        public void CreateReseller(Reseller r)
        {
            resellersRepository.Add(r);
        }
    }
}
