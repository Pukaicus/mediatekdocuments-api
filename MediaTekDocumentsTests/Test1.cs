using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.view; 
using System;

namespace MediaTekDocumentsTests
{
    [TestClass]
    public class AbonnementTests
    {
        [TestMethod]
        public void TestParutionDansAbonnement()
        {
            DateTime dateCommande = new DateTime(2024, 01, 01);
            DateTime dateFin = new DateTime(2024, 12, 31);

            DateTime dateParutionOk = new DateTime(2024, 06, 01);
            Assert.IsTrue(FrmMediatek.ParutionDansAbonnement(dateCommande, dateFin, dateParutionOk));

            DateTime dateParutionHors = new DateTime(2025, 01, 15);
            Assert.IsFalse(FrmMediatek.ParutionDansAbonnement(dateCommande, dateFin, dateParutionHors));
        }
    }
}