using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Text;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private const string MSG_INTROUVABLE = "Information non trouvée";
        private const string ETAT_AJOUTER = "Ajouter";
        private const string ETAT_ENREGISTRER = "Enregistrer";
        private const string ETAT_MODIFIER = "Modifier";
        private const string STR_MONTANT = "Montant";
        private const string STR_SUIVI = "Suivi";
        private const string STR_CONFIRMATION = "Confirmation";
        private const string STR_ENREGISTRER = "Enregistrer";
        private Livre dernierLivreSelectionne = null;
        private Utilisateur utilisateur;
        private const string STR_INFO = "Information";
        #region Commun
        private readonly FrmMediatekController controller;


        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        public FrmMediatek(Utilisateur utilisateur)
        {
            InitializeComponent();
            this.utilisateur = utilisateur;
            this.controller = new FrmMediatekController();
            InitCommandesLivres();
            InitCommandesDvd();
            InitCommandesRevues();
            btnLivresExemplairesSuppr.Click += (s, e) => SupprimerExemplaire();
            GestionAcces(utilisateur);
        }

        /// <summary>
        /// Initialisation complète de l'onglet des commandes de livres
        /// </summary>
        private void InitCommandesLivres()
        {
            tabCommandesLivres = new TabPage("Commandes de livres");
            tabOngletsApplication.TabPages.Add(tabCommandesLivres); 

            Label lblNum = new Label { Text = "Numéro livre :", Location = new System.Drawing.Point(20, 20), Size = new System.Drawing.Size(100, 20) };
            txbComLivresNum = new TextBox { Location = new System.Drawing.Point(130, 20), Width = 80 };
            Button btnRechercher = new Button { Text = "Chercher", Location = new System.Drawing.Point(220, 18) };
            
            lblComLivresInfos = new Label { Text = "Informations livre : ", Location = new System.Drawing.Point(320, 20), Size = new System.Drawing.Size(400, 20), Font = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Bold) };
            
            btnRechercher.Click += (s, e) => { ChargerCommandesLivre(txbComLivresNum.Text); };

            dgvComLivres = new DataGridView {
                Location = new System.Drawing.Point(20, 60),
                Size = new System.Drawing.Size(450, 300),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvComLivres.Columns.Add("Date", "Date");
            dgvComLivres.Columns.Add(STR_MONTANT, STR_MONTANT);
            dgvComLivres.Columns.Add("Quantite", "Quantité");
            dgvComLivres.Columns.Add(STR_SUIVI, STR_SUIVI);

            grpComLivresSaisie = new GroupBox { Text = "Gestion de la commande", Location = new System.Drawing.Point(500, 60), Size = new System.Drawing.Size(280, 300) };
            
            Label lblMontant = new Label { Text = "Montant :", Location = new System.Drawing.Point(10, 30), Width = 70 };
            txbComLivresMontant = new TextBox { Location = new System.Drawing.Point(90, 27), Width = 100 };
            Label lblQuantite = new Label { Text = "Quantité :", Location = new System.Drawing.Point(10, 70), Width = 70 };
            nudComLivresQuantite = new NumericUpDown { Location = new System.Drawing.Point(90, 67), Width = 60, Minimum = 1 };
            btnComLivresAjouter = new Button { Text = STR_ENREGISTRER, Location = new System.Drawing.Point(10, 110), Width = 100 };
            btnComLivresAjouter.Click += (s, e) => { EnregistrerNouvelleCommande(); };

            Label lblSuivi = new Label { Text = "Nouvel état :", Location = new System.Drawing.Point(10, 160), Width = 80 };
            cbxComLivresSuivi = new ComboBox { Location = new System.Drawing.Point(100, 157), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cbxComLivresSuivi.DataSource = controller.GetAllSuivi();
            cbxComLivresSuivi.DisplayMember = "Libelle";
            Button btnModifierSuivi = new Button { Text = "Changer l'état", Location = new System.Drawing.Point(100, 190), Width = 100 };
            btnModifierSuivi.Click += (s, e) => { ModifierSuiviCommande(); };

            Button btnSupprimerCom = new Button { Text = "Supprimer", Location = new System.Drawing.Point(10, 250), Width = 100, BackColor = System.Drawing.Color.LightCoral };
            btnSupprimerCom.Click += (s, e) => { SupprimerCommande(); };

            grpComLivresSaisie.Controls.Add(lblMontant);
            grpComLivresSaisie.Controls.Add(txbComLivresMontant);
            grpComLivresSaisie.Controls.Add(lblQuantite);
            grpComLivresSaisie.Controls.Add(nudComLivresQuantite);
            grpComLivresSaisie.Controls.Add(btnComLivresAjouter);
            grpComLivresSaisie.Controls.Add(lblSuivi);
            grpComLivresSaisie.Controls.Add(cbxComLivresSuivi);
            grpComLivresSaisie.Controls.Add(btnModifierSuivi);
            grpComLivresSaisie.Controls.Add(btnSupprimerCom);

            tabCommandesLivres.Controls.Add(lblNum);
            tabCommandesLivres.Controls.Add(txbComLivresNum);
            tabCommandesLivres.Controls.Add(btnRechercher);
            tabCommandesLivres.Controls.Add(lblComLivresInfos);
            tabCommandesLivres.Controls.Add(dgvComLivres);
            tabCommandesLivres.Controls.Add(grpComLivresSaisie);
        }

        /// <summary>
        /// Les commandes et les infos du livre
        /// </summary>
        private void ChargerCommandesLivre(string idLivre)
        {
            Livre leLivre = controller.GetLivre(idLivre);
            if (leLivre != null)
            {
                lblComLivresInfos.Text = $"Livre : {leLivre.Titre} - {leLivre.Auteur}";
                
                List<CommandeDocument> lesCommandes = controller.GetCommandesDocument(idLivre);
                
                dgvComLivres.DataSource = lesCommandes;
            }
            else
            {
                MessageBox.Show("Numéro de livre introuvable.");
                lblComLivresInfos.Text = "Informations livre : ";
                dgvComLivres.DataSource = null;
            }
        }

        /// <summary>
        /// Enregistre une nouvelle commande de livre
        /// </summary>
        private void EnregistrerNouvelleCommande()
        {
            string idDocument = txbComLivresNum.Text;
            double montant = double.Parse(txbComLivresMontant.Text);
            int nbExemplaires = (int)nudComLivresQuantite.Value;
            
            CommandeDocument uneCommande = new CommandeDocument(idDocument, DateTime.Now, montant, nbExemplaires, idDocument, "001", "En cours");
            if (controller.CreerCommandeDocument(uneCommande))
            {
                MessageBox.Show("Commande enregistrée avec succès.");
                ChargerCommandesLivre(idDocument);
            }
            else
            {
                MessageBox.Show("Erreur lors de l'enregistrement.");
            }
        }

        /// <summary>
        /// l'étape de suivi d'une commande en appliquant les règles de sécurité
        /// </summary>
        private void ModifierSuiviCommande()
        {
            if (dgvComLivres.CurrentRow == null)
            {
                MessageBox.Show("Veuillez sélectionner une commande dans la liste.");
                return;
            }

            CommandeDocument commandeSelectionnee = (CommandeDocument)dgvComLivres.CurrentRow.DataBoundItem;
            Suivi nouveauSuivi = (Suivi)cbxComLivresSuivi.SelectedItem;

            if (int.Parse(commandeSelectionnee.IdSuivi) >= 3 && nouveauSuivi.Id < 3)
            {
                MessageBox.Show("Sécurité : Une commande livrée ou réglée ne peut pas revenir à un état précédent.");
                return;
            }

            if (nouveauSuivi.Id == 4 && int.Parse(commandeSelectionnee.IdSuivi) < 3)
            {
                MessageBox.Show("Sécurité : Une commande doit être livrée avant de pouvoir être réglée.");
                return;
            }

            commandeSelectionnee.IdSuivi = nouveauSuivi.Id.ToString();
            commandeSelectionnee.LibelleSuivi = nouveauSuivi.Libelle;

            if (controller.ModifierCommandeDocument(commandeSelectionnee))
            {
                MessageBox.Show("L'étape de suivi a été modifiée avec succès.");
                ChargerCommandesLivre(txbComLivresNum.Text);
            }
        }

        /// <summary>
        /// Supprime une commande si elle n'est pas encore livrée
        /// </summary>
        private void SupprimerCommande()
        {
            if (dgvComLivres.CurrentRow == null) return;
            CommandeDocument commandeSelectionnee = (CommandeDocument)dgvComLivres.CurrentRow.DataBoundItem;

            if (int.Parse(commandeSelectionnee.IdSuivi) >= 3)
            {
                MessageBox.Show("Impossible de supprimer une commande déjà livrée ou réglée.");
                return;
            }

            if (MessageBox.Show("Supprimer la commande ?", STR_CONFIRMATION, MessageBoxButtons.YesNo) == DialogResult.Yes 
                && controller.SupprimerCommandeDocument(commandeSelectionnee))
            {
                MessageBox.Show("Commande supprimée.");
                ChargerCommandesLivre(txbComLivresNum.Text);
            }
        }

        /// <summary>
        /// Initialisation de l'onglet des commandes de DVD
        /// </summary>
        private void InitCommandesDvd()
        {
            tabCommandesDvd = new TabPage("Commandes de DVD");
            tabOngletsApplication.TabPages.Add(tabCommandesDvd);

            Label lblNum = new Label { Text = "Numéro DVD :", Location = new System.Drawing.Point(20, 20), Size = new System.Drawing.Size(100, 20) };
            txbComDvdNum = new TextBox { Location = new System.Drawing.Point(130, 20), Width = 80 };
            Button btnRechercher = new Button { Text = "Chercher", Location = new System.Drawing.Point(220, 18) };
            lblComDvdInfos = new Label { Text = "Informations DVD : ", Location = new System.Drawing.Point(320, 20), Size = new System.Drawing.Size(400, 20), Font = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Bold) };
            btnRechercher.Click += (s, e) => { ChargerCommandesDvd(txbComDvdNum.Text); };

            dgvComDvd = new DataGridView {
                Location = new System.Drawing.Point(20, 60),
                Size = new System.Drawing.Size(450, 300),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvComDvd.Columns.Add("Date", "Date");
            dgvComDvd.Columns.Add(STR_MONTANT, STR_MONTANT);
            dgvComDvd.Columns.Add("Quantite", "Quantité");
            dgvComDvd.Columns.Add(STR_SUIVI, STR_SUIVI);

            grpComDvdSaisie = new GroupBox { Text = "Gestion de la commande DVD", Location = new System.Drawing.Point(500, 60), Size = new System.Drawing.Size(280, 300) };
            
            Label lblMontant = new Label { Text = "Montant :", Location = new System.Drawing.Point(10, 30), Width = 70 };
            txbComDvdMontant = new TextBox { Location = new System.Drawing.Point(90, 27), Width = 100 };
            Label lblQuantite = new Label { Text = "Quantité :", Location = new System.Drawing.Point(10, 70), Width = 70 };
            nudComDvdQuantite = new NumericUpDown { Location = new System.Drawing.Point(90, 67), Width = 60, Minimum = 1 };
            
            btnComDvdAjouter = new Button { Text = STR_ENREGISTRER, Location = new System.Drawing.Point(10, 110), Width = 100 };
            btnComDvdAjouter.Click += (s, e) => { EnregistrerNouvelleCommandeDvd(); };

            Label lblSuivi = new Label { Text = "Nouvel état :", Location = new System.Drawing.Point(10, 160), Width = 80 };
            cbxComDvdSuivi = new ComboBox { Location = new System.Drawing.Point(100, 157), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cbxComDvdSuivi.DataSource = controller.GetAllSuivi();
            cbxComDvdSuivi.DisplayMember = "Libelle";
            
            Button btnModifier = new Button { Text = "Changer l'état", Location = new System.Drawing.Point(100, 190), Width = 100 };
            btnModifier.Click += (s, e) => { ModifierSuiviCommandeDvd(); };

            Button btnSupprimer = new Button { Text = "Supprimer", Location = new System.Drawing.Point(10, 250), Width = 100, BackColor = System.Drawing.Color.LightCoral };
            btnSupprimer.Click += (s, e) => { SupprimerCommandeDvd(); };

            grpComDvdSaisie.Controls.Add(lblMontant);
            grpComDvdSaisie.Controls.Add(txbComDvdMontant);
            grpComDvdSaisie.Controls.Add(lblQuantite);
            grpComDvdSaisie.Controls.Add(nudComDvdQuantite);
            grpComDvdSaisie.Controls.Add(btnComDvdAjouter);
            grpComDvdSaisie.Controls.Add(lblSuivi);
            grpComDvdSaisie.Controls.Add(cbxComDvdSuivi);
            grpComDvdSaisie.Controls.Add(btnModifier);
            grpComDvdSaisie.Controls.Add(btnSupprimer);

            tabCommandesDvd.Controls.Add(lblNum);
            tabCommandesDvd.Controls.Add(txbComDvdNum);
            tabCommandesDvd.Controls.Add(btnRechercher);
            tabCommandesDvd.Controls.Add(lblComDvdInfos);
            tabCommandesDvd.Controls.Add(dgvComDvd);
            tabCommandesDvd.Controls.Add(grpComDvdSaisie);
        }

        /// <summary>
        /// Les commandes et les infos du DVD
        /// </summary>
        private void ChargerCommandesDvd(string idDvd)
        {
            Dvd leDvd = controller.GetDvd(idDvd);
            if (leDvd != null)
            {
                lblComDvdInfos.Text = $"DVD : {leDvd.Titre} - {leDvd.Realisateur}";
                List<CommandeDocument> lesCommandes = controller.GetCommandesDocument(idDvd);
                dgvComDvd.DataSource = lesCommandes;
            }
            else
            {
                MessageBox.Show("Numéro de DVD introuvable.");
                dgvComDvd.DataSource = null;
            }
        }

        private void EnregistrerNouvelleCommandeDvd()
        {
            string idDoc = txbComDvdNum.Text;
            double montant = double.Parse(txbComDvdMontant.Text);
            int qte = (int)nudComDvdQuantite.Value;
            
            CommandeDocument com = new CommandeDocument(idDoc, DateTime.Now, montant, qte, idDoc, "001", "En cours");

            if (controller.CreerCommandeDocument(com))
            {
                MessageBox.Show("Commande DVD enregistrée.");
                ChargerCommandesDvd(idDoc);
            }
        }

        private void ModifierSuiviCommandeDvd()
        {
            if (dgvComDvd.CurrentRow == null) return;
            CommandeDocument com = (CommandeDocument)dgvComDvd.CurrentRow.DataBoundItem;
            Suivi nouveau = (Suivi)cbxComDvdSuivi.SelectedItem;

            if (int.Parse(com.IdSuivi) >= 3 && nouveau.Id < 3) {
                MessageBox.Show("Action impossible : Commande déjà livrée/réglée.");
                return;
            }
            if (nouveau.Id == 4 && int.Parse(com.IdSuivi) < 3) {
                MessageBox.Show("Action impossible : Livraison nécessaire avant règlement.");
                return;
            }

            com.IdSuivi = nouveau.Id.ToString();
            if (controller.ModifierCommandeDocument(com)) {
                ChargerCommandesDvd(txbComDvdNum.Text);
            }
        }

        private void SupprimerCommandeDvd()
        {
            if (dgvComDvd.CurrentRow == null) return;
            CommandeDocument com = (CommandeDocument)dgvComDvd.CurrentRow.DataBoundItem;

            if (int.Parse(com.IdSuivi) >= 3) {
                MessageBox.Show("Suppression interdite pour un DVD déjà livré.");
                return;
            }

            if (MessageBox.Show("Supprimer ?", STR_CONFIRMATION, MessageBoxButtons.YesNo) == DialogResult.Yes
                && controller.SupprimerCommandeDocument(com)) {
                ChargerCommandesDvd(txbComDvdNum.Text);
            }
        }

        /// <summary>
        /// Initialisation de l'onglet des commandes de revues
        /// </summary>
        private void InitCommandesRevues()
        {
            tabCommandesRevues = new TabPage("Commandes de revues");
            tabOngletsApplication.TabPages.Add(tabCommandesRevues);

            Label lblNum = new Label { Text = "Numéro revue :", Location = new System.Drawing.Point(20, 20), Size = new System.Drawing.Size(100, 20) };
            txbComRevuesNum = new TextBox { Location = new System.Drawing.Point(130, 20), Width = 80 };
            Button btnRechercher = new Button { Text = "Chercher", Location = new System.Drawing.Point(220, 18) };
            lblComRevuesInfos = new Label { Text = "Informations revue : ", Location = new System.Drawing.Point(320, 20), Size = new System.Drawing.Size(400, 20), Font = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Bold) };
            
            btnRechercher.Click += (s, e) => { ChargerCommandesRevue(txbComRevuesNum.Text); };

            dgvComRevues = new DataGridView {
                Location = new System.Drawing.Point(20, 60),
                Size = new System.Drawing.Size(450, 300),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvComRevues.Columns.Add("Date", "Date Commande");
            dgvComRevues.Columns.Add(STR_MONTANT, STR_MONTANT);
            dgvComRevues.Columns.Add("DateFin", "Fin Abonnement");

            grpComRevuesSaisie = new GroupBox { Text = "Gestion de l'abonnement", Location = new System.Drawing.Point(500, 60), Size = new System.Drawing.Size(280, 300) };
            
            Label lblDate = new Label { Text = "Date commande :", Location = new System.Drawing.Point(10, 30), Width = 100 };
            dtpComRevuesDate = new DateTimePicker { Location = new System.Drawing.Point(120, 27), Width = 130 };
            
            Label lblMontant = new Label { Text = "Montant :", Location = new System.Drawing.Point(10, 70), Width = 100 };
            txbComRevuesMontant = new TextBox { Location = new System.Drawing.Point(120, 67), Width = 80 };
            
            Label lblFin = new Label { Text = "Date de fin :", Location = new System.Drawing.Point(10, 110), Width = 100 };
            dtpComRevuesFin = new DateTimePicker { Location = new System.Drawing.Point(120, 107), Width = 130 };

            btnComRevuesAjouter = new Button { Text = STR_ENREGISTRER, Location = new System.Drawing.Point(10, 160), Width = 100 };
            btnComRevuesAjouter.Click += (s, e) => { EnregistrerAbonnement(); };

            Button btnSupprimer = new Button { Text = "Supprimer", Location = new System.Drawing.Point(10, 250), Width = 100, BackColor = System.Drawing.Color.LightCoral };
            btnSupprimer.Click += (s, e) => { SupprimerAbonnement(); };

            grpComRevuesSaisie.Controls.Add(lblDate);
            grpComRevuesSaisie.Controls.Add(dtpComRevuesDate);
            grpComRevuesSaisie.Controls.Add(lblMontant);
            grpComRevuesSaisie.Controls.Add(txbComRevuesMontant);
            grpComRevuesSaisie.Controls.Add(lblFin);
            grpComRevuesSaisie.Controls.Add(dtpComRevuesFin);
            grpComRevuesSaisie.Controls.Add(btnComRevuesAjouter);
            grpComRevuesSaisie.Controls.Add(btnSupprimer);

            tabCommandesRevues.Controls.Add(lblNum);
            tabCommandesRevues.Controls.Add(txbComRevuesNum);
            tabCommandesRevues.Controls.Add(btnRechercher);
            tabCommandesRevues.Controls.Add(lblComRevuesInfos);
            tabCommandesRevues.Controls.Add(dgvComRevues);
            tabCommandesRevues.Controls.Add(grpComRevuesSaisie);
        }

        /// <summary>
        /// Charge les abonnements d'une revue
        /// </summary>
        private void ChargerCommandesRevue(string idRevue)
        {
            Revue laRevue = controller.GetRevue(idRevue);
            if (laRevue != null)
            {
                lblComRevuesInfos.Text = $"Revue : {laRevue.Titre} - Editeur : {laRevue.Editeur}";
                
                List<Abonnement> lesAbonnements = controller.GetAbonnements(idRevue);
                
                dgvComRevues.DataSource = lesAbonnements;
            }
            else
            {
                MessageBox.Show("Numéro de revue introuvable.");
                lblComRevuesInfos.Text = "Informations revue : ";
                dgvComRevues.DataSource = null;
            }
        }

        /// <summary>
        /// Enregistre un nouvel abonnement ou un renouvellement
        /// </summary>
        private void EnregistrerAbonnement()
        {
            try 
            {
                string idRevue = txbComRevuesNum.Text;
                DateTime dateCommande = dtpComRevuesDate.Value;
                DateTime dateFin = dtpComRevuesFin.Value;
                double montant = double.Parse(txbComRevuesMontant.Text);

                if (dateFin <= dateCommande)
                {
                    MessageBox.Show("La date de fin doit être supérieure à la date de commande.");
                    return;
                }

                Abonnement nouvelAbo = new Abonnement("", dateCommande, montant, dateFin, idRevue);

                if (controller.CreerAbonnement(nouvelAbo))
                {
                    MessageBox.Show("Abonnement enregistré avec succès.");
                    ChargerCommandesRevue(idRevue);
                }
            }
            catch 
            {
                MessageBox.Show("Erreur : Vérifiez le format du montant.");
            }
        }

        /// <summary>
        /// Demande la suppression d'un abonnement après vérification de l'absence d'exemplaires rattachés
        /// </summary>
        private void SupprimerAbonnement()
        {
            if (dgvComRevues.CurrentRow == null) return;

            Abonnement abo = (Abonnement)dgvComRevues.CurrentRow.DataBoundItem;

            List<Exemplaire> listeExemplairesAbo = controller.GetExemplairesRevue(abo.IdIden);

            if (listeExemplairesAbo.Any(ex => ParutionDansAbonnement(abo.DateCommande, abo.DateFinAbonnement, ex.DateReception)))
            {
                MessageBox.Show("Suppression impossible : des exemplaires sont rattachés à cet abonnement.");
            }
            else if (MessageBox.Show("Supprimer ?", STR_CONFIRMATION, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                controller.SupprimerAbonnement(abo);
                ChargerCommandesRevue(txbComRevuesNum.Text);
            }
        }

        public static bool ParutionDansAbonnement(DateTime dateCommande, DateTime dateFinAbonnement, DateTime dateParution)
        {
             return (dateParution >= dateCommande && dateParution <= dateFinAbonnement);
        }

        /// <summary>
        /// Affiche une alerte si des abonnements de revues expirent dans moins de 30 jours
        /// </summary>
        private void AlerteFinAbonnements()
        {
            List<Revue> listeRevues = controller.GetAllRevues();
            DateTime dans30Jours = DateTime.Now.AddDays(30);

            var revuesCritiques = listeRevues
                .Where(r => r.DateFinAbonnement > DateTime.Now && r.DateFinAbonnement <= dans30Jours)
                .OrderBy(r => r.DateFinAbonnement)
                .ToList();

            if (revuesCritiques.Count > 0)
            {
                StringBuilder sb = new StringBuilder("Abonnements se terminant dans moins de 30 jours :\n\n");
                
                foreach (Revue revue in revuesCritiques)
                {
                    sb.AppendLine($"- {revue.Titre} : fin le {revue.DateFinAbonnement:dd/MM/yyyy}");
                }
                
                MessageBox.Show(sb.ToString(), "Alerte Abonnements", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Supprime l'exemplaire sélectionné après confirmation de l'utilisateur
        /// </summary>
        private void SupprimerExemplaire()
        {
            if (dgvLivresExemplaires.CurrentRow != null)
            {
                Exemplaire exemplaire = (Exemplaire)dgvLivresExemplaires.CurrentRow.DataBoundItem;

                if (MessageBox.Show($"Voulez-vous vraiment supprimer l'exemplaire n°{exemplaire.Numero} ?",
                    "Confirmation de suppression", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (controller.SupprimerExemplaire(exemplaire))
                    {
                        if (dernierLivreSelectionne != null)
                        {
                            RemplirLivresListe(controller.GetAllLivres()); 
                        }
                        MessageBox.Show("Exemplaire supprimé avec succès.", "Information");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la suppression.", "Erreur");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un exemplaire à supprimer.", "Information");
            }
        }

        /// <summary>
        /// Gère les accès selon le service de l'utilisateur connecté
        /// </summary>
        /// <param name="utilisateur">L'objet Utilisateur authentifié</param>
        private void GestionAcces(Utilisateur utilisateur)
        {
            if (utilisateur == null) 
            {
                MessageBox.Show("Erreur : Impossible de récupérer les infos de l'utilisateur.");
                return; 
            }

            MessageBox.Show("Le service reçu est : '" + utilisateur.Service + "'");

            if (utilisateur.Service == "Prêts")
            {
                tabCommandesLivres.Visible = false;
                tabCommandesDvd.Visible = false;
                tabCommandesRevues.Visible = false;
                
                btnLivresExemplairesSuppr.Enabled = false;
            }

            if (utilisateur.Service == "Administratif")
            {
                AlerteFinAbonnements();
            }

            if (utilisateur.Service == "Culture")
            {   
                MessageBox.Show("Accès refusé");
                // Application.Exit();
            }
        }




        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public static void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }
        #endregion

        #region Onglet Livres
        private List<Livre> lesLivres = new List<Livre>();

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show(MSG_INTROUVABLE); 
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    this.dernierLivreSelectionne = livre;
                    AfficheLivresInfos(livre);
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }
        #endregion

        #region Onglet Dvd

        private void btnDvdAjouter_Click(object sender, EventArgs e)
        {
            if (btnDvdAjouter.Text.Equals(ETAT_AJOUTER))
            {
                ViderChampsDvd();
                DeverrouillerChampsDvd();
                btnDvdAjouter.Text = ETAT_ENREGISTRER;
                btnDvdModifier.Enabled = false;
                btnDvdSupprimer.Enabled = false;
            }
            else
            {
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                Dvd dvd = new Dvd(txbDvdNumRecherche.Text, txbDvdTitre.Text, txbDvdImage.Text, int.Parse(txbDvdDuree.Text), txbDvdRealisateur.Text, txbDvdSynopsis.Text, genre.Id, genre.Libelle, lePublic.Id, lePublic.Libelle, rayon.Id, rayon.Libelle);
                
                if (controller.CreerDvd(dvd))
                {
                    MessageBox.Show("DVD ajouté.");
                    RemplirDvdListeComplete();
                    ResetInterfaceDvd();
                }
            }
        }

        private void btnDvdModifier_Click(object sender, EventArgs e)
        {
            if (btnDvdModifier.Text.Equals(ETAT_MODIFIER))
            {
                DeverrouillerChampsDvd();
                btnDvdModifier.Text = ETAT_ENREGISTRER;
                btnDvdAjouter.Enabled = false;
                btnDvdSupprimer.Enabled = false;
            }
            else
            {
                if (bdgDvdListe.Current != null)
            {
                    Dvd dvd = (Dvd)bdgDvdListe.Current;
                    dvd.Titre = txbDvdTitre.Text;
                    dvd.Realisateur = txbDvdRealisateur.Text;
                    dvd.Duree = int.Parse(txbDvdDuree.Text);
                    dvd.Synopsis = txbDvdSynopsis.Text;
                    dvd.Image = txbDvdImage.Text;

                    if (controller.ModifierDvd(dvd))
                    {
                        MessageBox.Show("DVD modifié.");
                        RemplirDvdListeComplete();
                        ResetInterfaceDvd();
                    }
                }
            }
        }

        #endregion


        #region Gestion des Livres (Ajout, Modification, Suppression)

        /// <summary>
        /// Gère l'ajout d'un livre (Préparation et Enregistrement ACID)
        /// </summary>
        private void btnLivresAjouter_Click(object sender, EventArgs e)
        {
            if (btnLivresAjouter.Text.Equals(ETAT_AJOUTER))
            {
                ViderChampsLivres();
                DeverrouillerChampsLivres(false); 
                btnLivresAjouter.Text = ETAT_ENREGISTRER;
                btnLivresModifier.Enabled = false;
                btnLivresSupprimer.Enabled = false;
            }
            else
            {
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                Livre nouveauLivre = new Livre(txbLivresNumRecherche.Text, txbLivresTitre.Text, txbLivresImage.Text, 
                    txbLivresIsbn.Text, txbLivresAuteur.Text, txbLivresCollection.Text, 
                    genre.Id, genre.Libelle, lePublic.Id, lePublic.Libelle, rayon.Id, rayon.Libelle);

                if (controller.CreerLivre(nouveauLivre))
                {
                    MessageBox.Show("Livre ajouté.");
                    RemplirLivresListeComplete();
                    ResetInterfaceLivres();
                }
            }
        }

        /// <summary>
        /// Gère la modification d'un livre (ID verrouillé selon consigne)
        /// </summary>
        private void btnLivresModifier_Click(object sender, EventArgs e)
        {
            if (btnLivresModifier.Text.Equals(ETAT_MODIFIER))
            {
                DeverrouillerChampsLivres(true); 
                btnLivresModifier.Text = ETAT_ENREGISTRER;
                btnLivresAjouter.Enabled = false;
                btnLivresSupprimer.Enabled = false;
            }
            else
            {
                Livre livre = (Livre)bdgDvdListe.Current;
                livre.Titre = txbLivresTitre.Text;
                livre.Auteur = txbLivresAuteur.Text;
                livre.Collection = txbLivresCollection.Text;
                livre.Image = txbLivresImage.Text;

                if (controller.ModifierLivre(livre))
                {
                    MessageBox.Show("Livre modifié.");
                    RemplirLivresListeComplete();
                    ResetInterfaceLivres();
                }
            }
        }

        /// <summary>
        /// Gère la suppression d'un livre avec vérification
        /// </summary>
        private void btnLivresSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentRow != null)
            {
                Livre livre = (Livre)bdgLivresListe.Current;
                if (MessageBox.Show("Supprimer " + livre.Titre + " ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (controller.SupprimerLivre(livre))
                    {
                        RemplirLivresListeComplete();
                        MessageBox.Show("Suppression réussie.");
                    }
                    else
                    {
                        MessageBox.Show("Suppression impossible : vérifiez les exemplaires ou commandes liés.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un livre.");
            }
        }

        // Méthodes outils pour l'interface
        private void ViderChampsLivres() 
        {
            txbLivresTitre.Text = ""; 
            txbLivresIsbn.Text = ""; 
            txbLivresAuteur.Text = ""; 
            txbLivresCollection.Text = ""; 
            txbLivresImage.Text = "";
        }

        private void DeverrouillerChampsLivres(bool modification) 
        {
            txbLivresTitre.ReadOnly = false; 
            txbLivresAuteur.ReadOnly = false;
            txbLivresCollection.ReadOnly = false; 
            txbLivresImage.ReadOnly = false;
            txbLivresIsbn.ReadOnly = modification; 
        }

        private void ResetInterfaceLivres() 
        {
            btnLivresAjouter.Text = "Ajouter"; 
            btnLivresModifier.Text = "Modifier";
            btnLivresAjouter.Enabled = true; 
            btnLivresModifier.Enabled = true; 
            btnLivresSupprimer.Enabled = true;
            txbLivresTitre.ReadOnly = true; 
            txbLivresIsbn.ReadOnly = true; 
            txbLivresAuteur.ReadOnly = true; 
            txbLivresCollection.ReadOnly = true;
            txbLivresImage.ReadOnly = true;
        }

        #endregion

        #region Gestion des Revues

        /// <summary>
        /// Gère l'ajout d'une revue
        /// </summary>
        private void btnRevuesAjouter_Click(object sender, EventArgs e)
        {
            if (btnLivresAjouter.Text.Equals(ETAT_AJOUTER))
            {
                ViderChampsRevues();
                DeverrouillerChampsRevues();
                btnLivresAjouter.Text = ETAT_ENREGISTRER;
                btnRevuesModifier.Enabled = false;
                btnRevuesSupprimer.Enabled = false;
            }
            else
            {
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                
                Revue revue = new Revue(txbRevuesNumRecherche.Text, txbRevuesTitre.Text, txbRevuesImage.Text, 
                    genre.Id, genre.Libelle, lePublic.Id, lePublic.Libelle, rayon.Id, rayon.Libelle, 
                    txbRevuesPeriodicite.Text, int.Parse(txbRevuesDateMiseADispo.Text), ""); 

                if (controller.CreerRevue(revue))
                {
                    MessageBox.Show("Revue ajoutée.");
                    RemplirRevuesListeComplete();
                    ResetInterfaceRevues();
                }
            }
        }

        /// <summary>
        /// Gère la suppression d'une revue
        /// </summary>
        private void btnRevuesSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentRow != null)
            {
                Revue revue = (Revue)bdgRevuesListe.Current;
                if (MessageBox.Show("Supprimer " + revue.Titre + " ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (controller.SupprimerRevue(revue))
                    {
                        RemplirRevuesListeComplete();
                        MessageBox.Show("Suppression réussie.");
                    }
                    else
                    {
                        MessageBox.Show("Suppression impossible : vérifiez les exemplaires rattachés.");
                    }
                }
            }
        }

        private void ViderChampsRevues() 
        {
            txbRevuesTitre.Text = ""; 
            txbRevuesPeriodicite.Text = ""; 
            txbRevuesDateMiseADispo.Text = ""; 
            txbRevuesImage.Text = "";
        }

        private void DeverrouillerChampsRevues() 
        {
            txbRevuesTitre.ReadOnly = false; 
            txbRevuesPeriodicite.ReadOnly = false; 
            txbRevuesDateMiseADispo.ReadOnly = false; 
            txbRevuesImage.ReadOnly = false;
        }

        private void ResetInterfaceRevues() 
        {
            btnRevuesAjouter.Text = "Ajouter"; 
            btnRevuesAjouter.Enabled = true;
            btnRevuesModifier.Enabled = true; 
            btnRevuesSupprimer.Enabled = true;
            txbRevuesTitre.ReadOnly = true; 
            txbRevuesPeriodicite.ReadOnly = true; 
            txbRevuesDateMiseADispo.ReadOnly = true;
            txbRevuesImage.ReadOnly = true;
        }

        #endregion

        private List<Dvd> lesDvd = new List<Dvd>();

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }


        #region Onglet Revues
        private List<Revue> lesRevues = new List<Revue>();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }
        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
                dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocuement = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesRevue(idDocuement);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateReception = dtpReceptionExemplaireDate.Value;
                    DateTime dateAchat = DateTime.Now;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;

                    Exemplaire exemplaire = new Exemplaire(numero, dateReception, photo, idEtat, idDocument, dateAchat);

                    if (controller.CreerExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }

        #endregion

private void ViderChampsDvd()
{
    txbDvdNumRecherche.Text = "";
    txbDvdTitre.Text = "";
    txbDvdRealisateur.Text = "";
    txbDvdDuree.Text = "";
    txbDvdSynopsis.Text = "";
    txbDvdImage.Text = "";
}

private void ResetInterfaceDvd()
{
    ViderChampsDvd();
    btnDvdAjouter.Text = ETAT_AJOUTER;
    btnDvdModifier.Text = ETAT_MODIFIER;
    btnDvdAjouter.Enabled = true;
    btnDvdModifier.Enabled = true;
    btnDvdSupprimer.Enabled = true;
}

private void DeverrouillerChampsDvd()
{
    txbDvdTitre.ReadOnly = false;
    txbDvdRealisateur.ReadOnly = false;
    txbDvdDuree.ReadOnly = false;
    txbDvdSynopsis.ReadOnly = false;
    txbDvdImage.ReadOnly = false;
}
    }
}

