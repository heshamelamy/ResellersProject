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

        public ResellerService(ResellerRepository RR)
        {
            this.resellersRepository = RR;
        }
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


        public ResellerViewModel MapToViewModel(Reseller reseller)
        {
            return Mapper.Map<Reseller,ResellerViewModel>(reseller);
        }

        public string GetResellerImage(Guid ResellerID)
        {
            return resellersRepository.GetResellerImage(ResellerID);
        }


        public Reseller GetById(Guid ResellerID)
        {
            return resellersRepository.GetById(ResellerID);
        }


        public void EditReseller(Reseller reseller)
        {
            resellersRepository.Update(reseller);
        }


        public void DeleteReseller(Reseller ToBeDeleted)
        {
            resellersRepository.Delete(ToBeDeleted);
        }


        public IEnumerable<Reseller> SearchForResellers(string Query)
        {
            return resellersRepository.SearchForResellers(Query);
        }


        public IEnumerable<Client> GetResellerClients(Guid ResellerID)
        {
            return resellersRepository.GetResellerClients(ResellerID);
        }


        public bool QuotaFull(Guid ResellerID)
        {
            return resellersRepository.QuotaFull(ResellerID);
        }

        public IEnumerable<Client> GetResellerDeletedClients(Guid resellerID)
        {
            return resellersRepository.GetResellerDeletedClients(resellerID);
        }

        public IEnumerable<Client> GetRecent10Clients(IEnumerable<Client> clients)
        {
            List <Client> myList = clients.ToList();
            List <Client> ordered = myList.OrderByDescending(m => m.CreationDate).ToList();
            return ordered.Take(5);
        }

        public Dictionary<string,int> GetChartData(Guid ResellerID)
        {
            int CurrentYear = DateTime.Now.Year;
            int CurrentMonth = DateTime.Now.Month;
            List<Client> AllResellerClients = GetResellerClients(ResellerID).ToList();
            var GroupedData = AllResellerClients.Where(m=>m.CreationDate.Month <= CurrentMonth && m.CreationDate.Year==CurrentYear).GroupBy(x => x.CreationDate.Month).ToList();
            Dictionary<string,int> chartdata = new Dictionary<string, int>();
            foreach (var item in GroupedData)
            {
                string month = item.FirstOrDefault().CreationDate.ToString("MMMM");
                chartdata[month] = item.Count();
            }
            return chartdata;
        }
    }
}
