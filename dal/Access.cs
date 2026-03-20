using System;
using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Linq;
using System.Configuration;
using System.IO;

namespace MediaTekDocuments.dal
{
    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    public class Access
    {
        /// <summary>
        /// adresse de l'API
        /// </summary>
        private static readonly string uriApi = "http://localhost/rest_mediatekdocuments/src/";
        /// <summary>
        /// instance unique de la classe
        /// </summary>
        private static Access instance = null;
        /// <summary>
        /// instance de ApiRest pour envoyer des demandes vers l'api et recevoir la réponse
        /// </summary>
        private readonly ApiRest api = null;
        /// <summary>
        /// méthode HTTP pour select
        /// </summary>
        private const string GET = "GET";
        /// <summary>
        /// méthode HTTP pour insert
        /// </summary>
        private const string POST = "POST";
        /// <summary>
        /// méthode HTTP pour update
        
        private const string CHAMPS = "champs=";

        /// <summary>
        /// Méthode privée pour créer un singleton
        /// initialise l'accès à l'API
        /// </summary>

        /// <summary>
        /// Constructeur privé pour le pattern Singleton de la classe Access
        /// </summary>
        private Access()
        {
            try
            {
                string authenticationString = ConfigurationManager.AppSettings["ApiAuthentification"] ?? "admin:adminpwd";
                api = ApiRest.GetInstance(uriApi, authenticationString);
                
                LogToFile("SUCCÈS : Accès API initialisé.");
            }
            catch (Exception e)
            {
                LogToFile("ERREUR CRITIQUE CONSTRUCTEUR : " + e.Message);
                System.Windows.Forms.MessageBox.Show("Erreur Constructeur : " + e.Message);
            }
        }

        /// <summary>
        /// Interroge l'API pour vérifier les identifiants
        /// </summary>
        public Utilisateur Authentification(string login, string pwd)
        {
            try
            {
                string route = "index.php?table=utilisateur&id=" + login;
                
                List<Utilisateur> liste = TraitementRecup<Utilisateur>(GET, route, null);

                if (liste != null && liste.Count > 0)
                {
                    return liste[0];
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Accès refusé ou utilisateur inconnu.");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Erreur lors de l'appel API : " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Création et retour de l'instance unique de la classe
        /// </summary>
        /// <returns>instance unique de la classe</returns>
        public static Access GetInstance()
        {
            if(instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return TraitementRecup<Genre>(GET, "index.php?table=genre", null).Cast<Categorie>().ToList();
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return TraitementRecup<Rayon>(GET, "index.php?table=rayon", null).Cast<Categorie>().ToList();
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return TraitementRecup<Public>(GET, "index.php?table=public", null).Cast<Categorie>().ToList();
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return TraitementRecup<Livre>(GET, "index.php?table=livre", null);
        }

        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return TraitementRecup<Dvd>(GET, "index.php?table=dvd", null);
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return TraitementRecup<Revue>(GET, "index.php?table=revue", null);
        }


        /// <summary>
        /// Retourne un livre spécifique ou tous les livres si id est vide
        /// </summary>
        public List<Livre> GetLivre(string id)
        {
            return TraitementRecup<Livre>(GET, "index.php?table=livre&id=" + id, null) ?? new List<Livre>();
        }

        /// <summary>
        /// Retourne un DVD spécifique ou tous les DVD si id est vide
        /// </summary>
        public List<Dvd> GetDvd(string id)
        {
            return TraitementRecup<Dvd>(GET, "index.php?table=dvd&id=" + id, null) ?? new List<Dvd>();
        }

        /// <summary>
        /// Retourne une revue spécifique ou toutes les revues si id est vide
        /// </summary>
        public List<Revue> GetRevue(string id)
        {
            return TraitementRecup<Revue>(GET, "index.php?table=revue&id=" + id, null) ?? new List<Revue>();
        }

        /// <summary>
        /// Récupère les abonnements d'une revue
        /// </summary>
        public List<Abonnement> GetAbonnements(string idRevue)
        {
            return TraitementRecup<Abonnement>(GET, "index.php?table=abonnement&id=" + idRevue, null) ?? new List<Abonnement>();
        }

        /// <summary>
        /// Récupère les abonnements arrivant à échéance sous 30 jours
        /// </summary>
        public List<Abonnement> GetAbonnementsEcheance()
        {
            return TraitementRecup<Abonnement>(GET, "index.php?table=abonnements_echeance", null) ?? new List<Abonnement>();
        }

        // --- ALIAS POUR LES ACTIONS (DVD/REVUES) ---
        public bool CreerDvd(Dvd dvd) => CreerEntite("dvd", dvd);
        public bool ModifierDvd(Dvd dvd) => ModifierEntite("dvd", dvd);
        public bool SupprimerDvd(Dvd dvd) => SupprimerEntite("dvd", dvd);
        public bool CreerRevue(Revue revue) => CreerEntite("revue", revue);
        public bool ModifierRevue(Revue revue) => ModifierEntite("revue", revue);
        public bool SupprimerRevue(Revue revue) => SupprimerEntite("revue", revue);


        /// <summary>
        /// Retourne les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            String jsonIdDocument = convertToJson("id", idDocument);
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            return lesExemplaires;
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire">exemplaire à insérer</param>
        /// <returns>true si l'insertion a pu se faire (retour != null)</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try
            {
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(POST, "exemplaire", CHAMPS + jsonExemplaire);
                
                if (liste != null) {
                    LogToFile("SUCCÈS : Création exemplaire n°" + exemplaire.Numero);
                }
                
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogToFile("ERREUR CreerExemplaire : " + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Récupère toutes les étapes de suivi à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Suivi</returns>
        public List<Suivi> GetAllSuivi()
        {
            List<Suivi> lesSuivis = TraitementRecup<Suivi>(GET, "suivi", null);
            return lesSuivis;
        }

        /// <summary>
        /// Récupère les commandes d'un document (livre ou dvd)
        /// </summary>
        /// <param name="idDocument">id du document concerné</param>
        /// <returns>Liste d'objets CommandeDocument</returns>
        public List<CommandeDocument> GetCommandesDocument(string idDocument)
        {
            List<CommandeDocument> lesCommandes = TraitementRecup<CommandeDocument>(GET, "commandedocument/" + idDocument, null);
            return lesCommandes;
        }

        /// <summary>
        /// Envoi d'une demande de création d'une entité
        /// </summary>
        public bool CreerEntite<T>(String table, T objet)
        {
            String jsonObjet = JsonConvert.SerializeObject(objet, new CustomDateTimeConverter());
            try
            {
                List<T> liste = TraitementRecup<T>(POST, table, CHAMPS + jsonObjet);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogToFile("ERREUR CreerEntite sur table " + table + " : " + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Envoi d'une demande de modification d'une entité
        /// </summary>
        public bool ModifierEntite<T>(String table, T objet)
        {
            JObject json = JObject.FromObject(objet);
            String id = (String)json["Id"];
            String jsonObjet = JsonConvert.SerializeObject(objet, new CustomDateTimeConverter());
            try
            {
                List<T> liste = TraitementRecup<T>(POST, table + "/" + id, CHAMPS + jsonObjet);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogToFile("ERREUR ModifierEntite sur table " + table + " (ID:" + id + ") : " + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Envoi d'une demande de suppression d'une entité
        /// </summary>
        public bool SupprimerEntite<T>(String table, T objet)
        {
            JObject json = JObject.FromObject(objet);
            String id = (String)json["Id"];
            String jsonId = convertToJson("id", id);
            try
            {
                List<T> liste = TraitementRecup<T>(POST, "suppr" + table, CHAMPS + jsonId);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogToFile("ERREUR SupprimerEntite sur table " + table + " (ID:" + id + ") : " + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Traitement de la récupération du retour de l'api, avec conversion du json en liste pour les select (GET)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methode">verbe HTTP (GET, POST, PUT, DELETE)</param>
        /// <param name="message">information envoyée dans l'url</param>
        /// <param name="parametres">paramètres à envoyer dans le body, au format "chp1=val1&chp2=val2&..."</param>
        /// <returns>liste d'objets récupérés (ou liste vide)</returns>
        private List<T> TraitementRecup<T>(String methode, String message, String parametres)
        {
            List<T> liste = new List<T>();
            try
            {
                if (api == null)
                {
                    LogToFile("ERREUR : L'objet 'api' est NULL.");
                    return liste;
                }

                JObject retour = api.RecupDistant(methode, message, parametres);

                if (retour == null)
                {
                    LogToFile("ERREUR : RecupDistant a renvoyé NULL.");
                    return liste;
                }

                if (retour["code"] != null && retour["code"].ToString().Equals("200"))
                {
                    if (methode.Equals(GET) && retour["result"] != null)
                    {
                        String resultString = JsonConvert.SerializeObject(retour["result"]);
                        liste = JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());
                    }
                }
                else
                {
                    String code = retour["code"]?.ToString() ?? "Inconnu";
                    String msg = retour["message"]?.ToString() ?? "Pas de message";
                    LogToFile("ERREUR API : code=" + code + " msg=" + msg);
                }
            }
            catch (Exception e)
            {
                LogToFile("EXCEPTION CRITIQUE : Erreur lors de l'accès à l'API : " + e.Message);
            }
            return liste;
        }

        /// <summary>
        /// Convertit en json un couple nom/valeur
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="valeur"></param>
        /// <returns>couple au format json</returns>
        private static string convertToJson(Object nom, Object valeur)
        {
            Dictionary<Object, Object> dictionary = new Dictionary<Object, Object>();
            dictionary.Add(nom, valeur);
            return JsonConvert.SerializeObject(dictionary);
        }

        /// <summary>
        /// Modification du convertisseur Json pour gérer le format de date
        /// </summary>
        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
        }

        /// <summary>
        /// Modification du convertisseur Json pour prendre en compte les booléens
        /// classe trouvée sur le site :
        /// https://www.thecodebuzz.com/newtonsoft-jsonreaderexception-could-not-convert-string-to-boolean/
        /// </summary>
        private sealed class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }

            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

        /// <summary>
        /// Enregistre un message dans le fichier logs.txt avec horodatage
        /// </summary>
        /// <param name="message">Texte à consigner</param>
        private static void LogToFile(string message)
        {
            try
            {
               string logPath = "logs.txt";
              string logLine = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] - {message}{Environment.NewLine}";
        
            File.AppendAllText(logPath, logLine);
            }
            catch (Exception ex)
            {
               Console.WriteLine("Erreur écriture log : " + ex.Message);
            }
        }

    }
}
