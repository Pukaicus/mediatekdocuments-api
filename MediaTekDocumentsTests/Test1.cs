using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocumentsTests
{
    [TestClass]
    public class Test1
    {
        [TestMethod]
        public void TestParutionDansAbonnement()
        {
            DateTime debut = new DateTime(2024, 1, 1);
            DateTime fin = new DateTime(2024, 12, 31);
            
            DateTime parutionOk = new DateTime(2024, 6, 1);
            Assert.IsTrue(Abonnement.ParutionDansAbonnement(debut, fin, parutionOk));

            DateTime parutionHors = new DateTime(2025, 1, 1);
            Assert.IsFalse(Abonnement.ParutionDansAbonnement(debut, fin, parutionHors));
        }
    }
}