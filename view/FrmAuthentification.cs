using MediaTekDocuments.model;
using MediaTekDocuments.controller;
       
        /// <summary>
        /// Événement du bouton Se Connecter
        /// </summary>
        private void btnSeConnecter_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text;
            string pwd = txtPassword.Text;

            Utilisateur utilisateur = controller.Authentification(login, pwd);

            if (utilisateur != null)
            {
                FrmMediatek frmMediatek = new FrmMediatek(utilisateur);
                frmMediatek.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Identifiants incorrects.", "Erreur d'authentification");
            }
        }