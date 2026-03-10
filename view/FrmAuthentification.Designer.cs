using System.ComponentModel;
using System.Windows.Forms;

namespace MediaTekDocuments.view
{
    partial class FrmAuthentification
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Déclaration des contrôles graphiques
        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnSeConnecter;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.Label lblPassword;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Méthode requise pour le support du concepteur - ne pas modifier
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container(); // Correction pour l'erreur CS0144
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnSeConnecter = new System.Windows.Forms.Button();
            this.lblLogin = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // lblLogin
            this.lblLogin.Text = "Identifiant :";
            this.lblLogin.Location = new System.Drawing.Point(30, 30);
            this.lblLogin.Size = new System.Drawing.Size(100, 20);

            // txtLogin
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Location = new System.Drawing.Point(30, 50);
            this.txtLogin.Size = new System.Drawing.Size(220, 20);

            // lblPassword
            this.lblPassword.Text = "Mot de passe :";
            this.lblPassword.Location = new System.Drawing.Point(30, 80);
            this.lblPassword.Size = new System.Drawing.Size(100, 20);

            // txtPassword
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*'; // Masquage des caractères
            this.txtPassword.Location = new System.Drawing.Point(30, 100);
            this.txtPassword.Size = new System.Drawing.Size(220, 20);

            // btnSeConnecter
            this.btnSeConnecter.Name = "btnSeConnecter";
            this.btnSeConnecter.Text = "Se connecter";
            this.btnSeConnecter.Location = new System.Drawing.Point(30, 140);
            this.btnSeConnecter.Size = new System.Drawing.Size(220, 30);
            this.btnSeConnecter.UseVisualStyleBackColor = true;
            this.btnSeConnecter.Click += new System.EventHandler(this.btnSeConnecter_Click);

            // FrmAuthentification
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 211);
            this.Controls.Add(this.lblLogin);
            this.Controls.Add(this.txtLogin);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnSeConnecter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FrmAuthentification";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connexion MediaTekDocuments";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}