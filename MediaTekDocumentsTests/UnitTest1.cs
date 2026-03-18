using Xunit;
using MediaTekDocuments.model;

namespace MediaTekDocumentsTests
{
    public class ModelTests
    {
        [Fact]
        public void TestGenre()
        {
            Genre genre = new Genre("G001", "Policier");
            Assert.Equal("G001", genre.Id);
            Assert.Equal("Policier", genre.Libelle);
        }

        [Fact]
        public void TestDocument()
        {
            Document doc = new Document("001", "Titre", "img.png", "G1", "Genre", "P1", "Public", "R1", "Rayon");
            Assert.Equal("001", doc.Id);
            Assert.Equal("Titre", doc.Titre);
        }
    }
}