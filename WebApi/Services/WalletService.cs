using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;
using WebApi.Entities;

namespace WebApi.Services
{
    public interface IWalletService
    {
        void Create(double amount);
        double Get();
        void Update(double amountWallet);
    }


    public class WalletService : IWalletService
    {
        DataContext _context;

        public WalletService(DataContext context)
        {
            _context = context;
        }

        public void Create(double amount = 0)
        {
            var Wallet = new Wallet();
            Wallet.AmountWallet = amount;
            _context.Wallets.Add(Wallet);
            _context.SaveChanges();

        }
        public double Get()
        {
            return _context.Wallets.SingleOrDefault(w => w.Id == 1).AmountWallet;
        }

        public void Update(double amountWallet)
        {
            var wallet = _context.Wallets.FirstOrDefault(w => w.Id == 1);
            if (wallet == null)
            {
                Create();
                wallet = _context.Wallets.FirstOrDefault(w => w.Id == 1);
            }

            wallet.AmountWallet += amountWallet;

            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }
    }
}
