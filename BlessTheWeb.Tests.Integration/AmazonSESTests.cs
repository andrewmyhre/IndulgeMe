using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlessTheWeb.Core;
using NUnit.Framework;

namespace BlessTheWeb.Tests.Integration
{
    [TestFixture]
    public class AmazonSESTests
    {
        [Test]
        public void AmazonSES_CanSendATestEmail()
        {
            //var emailer = new AmazonSesIndulgenceEmailer();

            //emailer.Send(TestIndulgence(), "indulgence.pdf");
        }

        private Indulgence TestIndulgence()
        {
            return new Indulgence()
                       {
                           Id = "indulgences/1",
                           Name = "andrew",
                           CharityName = "test charity",
                           DateConfessed = DateTime.Now,
                           AmountDonated = 102,
                           DonorEmailAddress = "andrew.myhre@gmail.com"
                       };
        }
    }
}
