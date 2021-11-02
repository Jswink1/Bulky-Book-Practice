using Braintree;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBookPractice.Utility.BrainTree
{
    public class BrainTreeGate : IBrainTreeGate
    {
        public BrainTreeSettings Options { get; set; }
        private IBraintreeGateway BrainTreeGateway { get; set; }

        public BrainTreeGate(IOptions<BrainTreeSettings> options)
        {
            Options = options.Value;
        }

        public IBraintreeGateway CreateGateway()
        {
            return new BraintreeGateway(Options.Environment,
                                        Options.MerchantId,
                                        Options.PublicKey,
                                        Options.PrivateKey);
        }

        public IBraintreeGateway GetGateway()
        {
            if (BrainTreeGateway == null)
            {
                BrainTreeGateway = CreateGateway();
            }
            return BrainTreeGateway;
        }
    }
}
