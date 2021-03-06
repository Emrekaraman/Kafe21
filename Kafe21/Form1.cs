using Kafe21.Data;
using Kafe21.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kafe21
{
    public partial class Form1 : Form
    {
        KafeVeri db = new KafeVeri();              
        public Form1()
        {
            InitializeComponent();
            MasalariYukle();
        }

       

        private void MasalariYukle()
        {
            ImageList imageList = new ImageList();
            imageList.Images.Add("bos", Resources.bos);
            imageList.Images.Add("dolu", Resources.dolu);
            imageList.ImageSize = new Size(70, 70);
            lvwMasalar.LargeImageList = imageList;
            for (int i = 0; i < db.MasaAdet; i++)
            {
                ListViewItem lvi = new ListViewItem("Masa " + i);
                bool doluMu = db.Siparisler.Any(x => x.MasaNo == i && x.Durum == SiparisDurum.Aktif);
                lvi.ImageKey = doluMu ? "dolu" : "bos";
                lvi.Tag = i;
                lvwMasalar.Items.Add(lvi);
            }
        }

    
        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            Urunler frmUrunler = new Urunler(db);
            frmUrunler.ShowDialog();
        }

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            var frmGecmisSiparisler = new GecmisSiparisler(db);
            frmGecmisSiparisler.ShowDialog();
        }

        private void lvwMasalar_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem lvi = lvwMasalar.SelectedItems[0];

            int masaNo = (int)lvi.Tag;
            Siparis siparis = db.Siparisler.FirstOrDefault(x => x.MasaNo == masaNo && x.Durum == SiparisDurum.Aktif);
            //henüz bu masa sipariş oluşturulmamışsa bu siparişi şimdi oluşturalım..
            if (siparis == null)
            {
                siparis = new Siparis() { MasaNo = masaNo };
                db.Siparisler.Add(siparis);
                db.SaveChanges();
                lvi.ImageKey = "dolu";
            }
            SiparisForm frmSiparis = new SiparisForm(db, siparis);
            frmSiparis.MasaTasindi += FrmSiparis_MasaTasindi;

            DialogResult dr = frmSiparis.ShowDialog();
            if (dr == DialogResult.OK)
            {
                lvi.ImageKey = "bos";
            }
        }

        private void FrmSiparis_MasaTasindi(object sender, MasaTasindiEventArgs e)
        {
            lvwMasalar.Items.Cast<ListViewItem>()
               .First(x => (int)x.Tag == e.EskiMasaNo).ImageKey = "bos";

            lvwMasalar.Items.Cast<ListViewItem>()
                .First(x => (int)x.Tag == e.YeniMasaNo).ImageKey = "dolu";

            //uzun hali
            //foreach (ListViewItem lvi in lvwMasalar.Items)
            //{
            //    if ((int)lvi.Tag == e.EskiMasaNo)
            //    {
            //        lvi.ImageKey = "bos";
            //    }
            //    else if ((int)lvi.Tag == e.YeniMasaNo)
            //    {
            //        lvi.ImageKey = "dolu";

            //    }

            //}
        }
    }
}
