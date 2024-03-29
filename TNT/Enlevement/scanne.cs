﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Symbol;
using TNT.Enlevement;
using System.IO;
using TNT.login;
using TNT.con_req;
using ComponentPro.Net;



namespace TNT
{
    public partial class scanne : Form
    {
        public static bool fermer = false;
        private String m_id_env = "";
        public String m_colis;
        private String m_dest = "";
        private String m_exp = "";
        private String m_user = "";
        private AP scanAPI=null;
        private EventHandler myReadNotifyHandler = null;
        private requete_signature sign;

        string repertoire_signature = ConfigurationManager.GetChemin<string>("repertoire_signature");

        private List<Enlevement.datastructure> m_envois = new List<Enlevement.datastructure>();
        private Enlevement.datastructure m_envoi;

        private List<Enlevement.datastructure> envois
        {
            get { return m_envois; }
            set { m_envois = value; }
        }
        public String exp
        {
            get { return m_exp; }
            set { m_exp = value; }
        }
        public String dest
        {
            get { return m_dest; }
            set { m_dest = value; }
        }



        public scanne()
        {
            InitializeComponent();
        }

        private void scanne_Load(object sender, EventArgs e)
        {
            scanAPI = new AP();
            scanAPI.InitReader();
            scanAPI.StartRead(false);
            this.myReadNotifyHandler = new EventHandler(myReader_ReadNotify);
            scanAPI.AttachReadNotify(myReadNotifyHandler);
            List<String> states = new List<String>();
            this.scan_manuel.Focus();

            //this.stateBox.DataSource = states;
            sign = new requete_signature();
            sign.Clear();
            sign.Location = zone_signature.Location;
            sign.Size = zone_signature.Size;

            this.zone_signature.Visible = false;
            //this.Controls.Add(sign);
            this.Controls.Add(sign);
        }

        public void next_Click(object sender, EventArgs e)
        {
            enreg_enlevmenet();
            initAll();
            this.scan_manuel.Focus();

        }

        public int enreg_enlevmenet()
        {

            requete_enlevement req_enlev =new requete_enlevement();
            if (this.scannedData.Text.ToString()!="" || scan_manuel.Text.ToString()!="")
            {
                string scan;
                if (this.scannedData.Text.ToString() != "")
                {
                    scan = this.scannedData.Text.ToString();
                }
                else 
                {
                    scan = scan_manuel.Text.ToString();
                }
                
                DataSet ds=req_enlev.verfier_scanne(scan);
                if (ds.Tables[0].Rows.Count == 0)
                {


                    int exp = int.Parse(traitement_enlevement.expediteur.ToString());
                    int dest = int.Parse(traitement_enlevement.destinataire.ToString());

                    int util_exp = int.Parse(traitement_enlevement.util_exp.ToString());
                    int util_dest = int.Parse(traitement_enlevement.util_dest.ToString());
                    int util = int.Parse(traitement_authentification.id_util.ToString());

                    int cpt = req_enlev.count_pers("id_enlev", "enlevement");
                    string cpt_sign = util.ToString() +"-"+ cpt.ToString();

                    if (!System.IO.Directory.Exists("My Documents\\signature\\enlevement"))
                    {
                        System.IO.Directory.CreateDirectory("My Documents\\signature\\enlevement");
                    }
                    string chemin = "\\My Documents\\signature\\enlevement\\ES" +  cpt_sign.ToString() + "-"+scan.ToString() + ".png";
                    string chemin_serv = repertoire_signature+"enlevement/ES" + cpt_sign.ToString() + "-" + scan.ToString() + ".png";
                    requete_signature.image.Save(chemin, System.Drawing.Imaging.ImageFormat.Png);

                    DateTime dt = DateTime.Now;
                    string Format = "yyyy-MM-dd H:mm:ss";
                    string dat = dt.ToString(Format);
                    string nb = txb_package.Text.ToString();
                    if (nb == "") { nb = "0"; }
                    try
                    {
                        int nbr = int.Parse(nb);
                        //if(nbr!=){
                        string requete = "insert into enlevement (exp,dest,id_util,code_colis,date_enlev,general_desc,nb_package,util_exp,util_dest,signature_enlev,observ_enlev) values (" + exp + "," + dest + "," + util + ",'" + scan.ToString() + "','" + dat.ToString() + "','" + txb_gn_desc.Text.ToString() + "'," + nbr + "," + util_exp + "," + util_dest + ",'"+chemin_serv+"','" + txb_com.Text.ToString() + "')";
                        int rep = Requete.ExecuteUpdate(requete);

                        if (rep == 1)
                        {
                            MessageBox.Show("L'insertion est réussite");
                            return 1;
                        }
                        else
                        {
                            MessageBox.Show("Echec d'insertion");
                            return -1;
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Erreur de saisie de donnée");
                        return -1;
                    }
                }
                else {

                    MessageBox.Show("Ce colis est deja scanner !!!");
                    return -1;
                }
                
            }
            else 
            {
                MessageBox.Show("Aucun scan est effectue  !!");
                return -1;
            }
            this.scanAPI.StopRead();
        }
        
        private void initAll()
        {
            this.scan_manuel.Text = "";
            this.scan_manuel.Text = "";
            this.scannedData.Text = "";
            this.txb_gn_desc.Text = "";
            this.txb_package.Text = "";
            this.txb_com.Text = "";
            this.zone_signature.Refresh();
            this.scan_manuel.Focus();
        }


        private void myReader_ReadNotify(object Sender, EventArgs e)
        {
            // Get ReaderData
            Symbol.Barcode.ReaderData TheReaderData = scanAPI.Reader.GetNextReaderData();

            switch (TheReaderData.Result)
            {
                case Symbol.Results.SUCCESS:

                    // Handle the data from this read & submit the next read.

                    this.scannedData.Text = TheReaderData.Text;
                    scanAPI.StartRead(false);
                    //this.stateBox.Focus();
                    break;

                case Symbol.Results.E_SCN_READTIMEOUT:


                    scanAPI.StartRead(false);
                    break;

                case Symbol.Results.CANCELED:

                    break;

                case Symbol.Results.E_SCN_DEVICEFAILURE:

                    scanAPI.StopRead();
                    scanAPI.StartRead(false);
                    break;

                default:

                    string sMsg = "Read Failed\n"
                        + "Result = "
                        + (TheReaderData.Result).ToString();

                    if (TheReaderData.Result == Symbol.Results.E_SCN_READINCOMPATIBLE)
                    {
                        // If the failure is E_SCN_READINCOMPATIBLE, exit the application.
                        MessageBox.Show("AppExitMsg", "Failure");
                        this.Close();
                        return;
                    }

                    break;
            }
        }



        private void pBHome_Click_1(object sender, EventArgs e)
        {
            scanAPI.StopRead();
            this.Close();
        }


        public void pBUpdate_Click_1(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int resu=enreg_enlevmenet();
            if (resu == 1)
            {
                traitement_enlevement tr_enlev = new traitement_enlevement();
                tr_enlev.exporte_fichier();
                tr_enlev.upload();
                initAll();
            }
            this.scan_manuel.Focus();
            Cursor.Current = Cursors.Default;
        }


        private void pBQuitter_Click(object sender, EventArgs e)
        {
            scanAPI.StopRead();
            this.Close();
            TNT.Menu mn = new Menu();
            mn.Show();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            requete_signature.image.Save("\\My Documents\\MaSignature.png", System.Drawing.Imaging.ImageFormat.Png);
            requete_signature.enregistrement_image("\\My Documents\\MaSignature.png");
        }

        private void Effacer_Click(object sender, EventArgs e)
        {
            requete_signature signature = new requete_signature();
            signature.Clear();
            signature.Refresh();
            signature.Invalidate();
        }
    }
}