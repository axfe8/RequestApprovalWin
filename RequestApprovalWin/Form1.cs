using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace RequestApprovalWin
{
    public partial class Form1 : Form
    {
        private readonly string _connString = ConfigurationManager.ConnectionStrings["BeachDb"]?.ConnectionString;

        public Form1()
        {
            InitializeComponent();

            // 1) Checkbox ve CommitEdit için
            dgvRequests.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvRequests.IsCurrentCellDirty)
                    dgvRequests.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            // 2) Onay sütunu değişince DB'ye yaz
            dgvRequests.CellValueChanged += dgvRequests_CellValueChanged;
            // 3) Resim byte[] → Image formatlama
            dgvRequests.CellFormatting += DgvRequests_CellFormatting;
            // 4) Buton
            btnLoadRequests.Click += btnLoadRequests_Click;

            dgvRequests.CellContentClick += DgvRequests_CellContentClick;

            dgvRequests.DataError += (_, args) =>
            {
                // biçimlendirme hatalarını yoksay
                args.ThrowException = false;
            };
            }

        private void btnLoadRequests_Click(object sender, EventArgs e)
        {
            // 1) Parametreleri oku
            var requester = txtRequesterName.Text.Trim();
            var filterDate = dtpRequestDate.Value.Date;

            // 2) Onay event’ini geçici kaldır
            dgvRequests.CellValueChanged -= dgvRequests_CellValueChanged;

            // 3) SP’den DataTable’ı doldur
            var table = new DataTable();
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("dbo.sp_GetRequestsWithGuests", conn) { CommandType = CommandType.StoredProcedure })
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@RequesterName", requester);
                cmd.Parameters.AddWithValue("@FilterDate", filterDate);
                conn.Open();
                da.Fill(table);
            }

            // 4) Kaynağı ata
            dgvRequests.DataSource = table;

            // 5) Bound byte[] “Picture” sütununu grid’den kaldır
            if (dgvRequests.Columns.Contains("Picture"))
                dgvRequests.Columns.Remove("Picture");

            // 6) Eskiden eklenmiş unbound sütunları da temizle
            if (dgvRequests.Columns.Contains("colUpload"))
                dgvRequests.Columns.Remove("colUpload");
            if (dgvRequests.Columns.Contains("colPicture"))
                dgvRequests.Columns.Remove("colPicture");

            // 7) “Resim Yükle” buton sütunu
            var btnCol = new DataGridViewButtonColumn()
            {
                Name = "colUpload",
                HeaderText = "Resim Yükle",
                Text = "Seç...",
                UseColumnTextForButtonValue = true
            };
            dgvRequests.Columns.Add(btnCol);

            // 8) Image sütunu (unbound)
            var imgCol = new DataGridViewImageColumn()
            {
                Name = "colPicture",
                HeaderText = "Resim",
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                ValueType = typeof(Image)
            };
            dgvRequests.Columns.Add(imgCol);

            // 7) CheckBox sütun ayarları
            if (dgvRequests.Columns["IsApproved"] is DataGridViewCheckBoxColumn cbOnay)
            {
                cbOnay.ValueType = typeof(bool);
                cbOnay.TrueValue = true;
                cbOnay.FalseValue = false;
                cbOnay.IndeterminateValue = DBNull.Value;
            }
            if (dgvRequests.Columns["IsOwner"] is DataGridViewCheckBoxColumn cbSahip)
            {
                cbSahip.ValueType = typeof(bool);
                cbSahip.TrueValue = true;
                cbSahip.FalseValue = false;
                cbSahip.IndeterminateValue = DBNull.Value;
            }

            // 8) Türkçe başlıklar
            dgvRequests.Columns["RequestID"].HeaderText = "TalepID";
            dgvRequests.Columns["RequestedDate"].HeaderText = "Talep Tarihi";
            dgvRequests.Columns["GuestID"].HeaderText = "MisafirID";
            dgvRequests.Columns["GuestName"].HeaderText = "İsim";
            dgvRequests.Columns["GuestPhone"].HeaderText = "Telefon";
            dgvRequests.Columns["GuestEmail"].HeaderText = "E-posta";   
            dgvRequests.Columns["GuestSocialMedia"].HeaderText = "Sosyal Medya";
            if (!dgvRequests.Columns.Contains("PickerNot"))
            {
                dgvRequests.Columns.Add("PickerNot", "Picker Not");
            }
            dgvRequests.Columns["IsOwner"].HeaderText = "Sahip";
            dgvRequests.Columns["IsApproved"].HeaderText = "Onayla";
            // btnCol zaten “Resim Yükle”, imgCol zaten “Resim” başlığında

            // 9) Sadece sahibi satırlarda onay kutusunu aktif tut
            foreach (DataGridViewRow row in dgvRequests.Rows)
            {
                if (row.IsNewRow) continue;
                bool isOwner = Convert.ToBoolean(row.Cells["IsOwner"].Value);
                var onayCell = (DataGridViewCheckBoxCell)row.Cells["IsApproved"];
                onayCell.ReadOnly = !isOwner;
                if (!isOwner) onayCell.Value = false;
            }

            // ► Buraya ekleyin:
            if (dgvRequests.Columns.Contains("colPicture"))
            {
                // Görsel hücreyi 200×200 px olarak gösterelim:
                dgvRequests.Columns["colPicture"].Width = 760;
                dgvRequests.RowTemplate.Height = 760;
                dgvRequests.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            }

            // 10) Event’i geri bağla
            dgvRequests.CellValueChanged += dgvRequests_CellValueChanged;
        }

        private void dgvRequests_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Satır veya kolon uygun değilse çık
            if (e.RowIndex < 0 || dgvRequests.Rows[e.RowIndex].IsNewRow) return;
            var row = dgvRequests.Rows[e.RowIndex];
            int requestId = Convert.ToInt32(row.Cells["RequestID"].Value);
            int guestId = Convert.ToInt32(row.Cells["GuestID"].Value);
            // 1) Eğer Not hücresi değiştiyse, DigitalPickerNot'u güncelle
            if (dgvRequests.Columns[e.ColumnIndex].Name == "PickerNote")
            {
                string newNote = row.Cells["PickerNote"].Value?.ToString().Trim();
                using var conn = new SqlConnection(_connString);
                using var cmd = new SqlCommand(
                    @"UPDATE dbo.ReservationRequestGuests
              SET DigitalPickerNot = @note
              WHERE RequestID = @rid AND GuestID = @gid", conn);
                cmd.Parameters.AddWithValue("@note", (object)newNote ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@rid", requestId);
                cmd.Parameters.AddWithValue("@gid", guestId);
                conn.Open();
                cmd.ExecuteNonQuery();
                // Burada isterseniz kullanıcıyı bilgilendiren bir log/message gösterebilirsiniz.
            }
            if (dgvRequests.Columns[e.ColumnIndex].Name != "IsApproved") return;
            bool isOwner = Convert.ToBoolean(row.Cells["IsOwner"].Value);
            bool isApproved = Convert.ToBoolean(row.Cells["IsApproved"].Value);

            // Sadece sahibi onaylayabilir
            if (!isOwner)
            {
                MessageBox.Show("Sadece talep sahibi onaylayabilir.", "Yetki Hatası",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                row.Cells["IsApproved"].Value = false;
                return;
            }

            // 2) ReservationRequests.Onayla sütununu güncelle
            try
            {
                using var conn = new SqlConnection(_connString);
                using var upd = new SqlCommand(
                    "UPDATE ReservationRequests SET Onayla = @ap WHERE RequestID = @rid",
                    conn);
                upd.Parameters.AddWithValue("@ap", isApproved);
                upd.Parameters.AddWithValue("@rid", requestId);
                conn.Open();
                upd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Onay bilgisi güncellenirken hata:\n{ex.Message}",
                                "Veritabanı Hatası",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                row.Cells["IsApproved"].Value = false;
                return;
            }

            // 3) TRUE’ya döndüyse sp_ApproveRequest’i çağır    
            if (isApproved)
            {
                // a)
                DateTime reqDate = Convert.ToDateTime(
                    row.Cells["RequestedDate"].Value);

                // b)
                var allRows = dgvRequests.Rows
                    .Cast<DataGridViewRow>()
                    .Where(r =>
                        !r.IsNewRow &&
                        Convert.ToInt32(r.Cells["RequestID"].Value) == requestId)
                    .ToList();
                int guestCount = allRows.Count;

                try
                {
                    using var conn = new SqlConnection(_connString);
                    using var cmd = new SqlCommand("dbo.sp_ApproveRequest", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@RequestID", requestId);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show(
                        $"Talep {requestId} onaylandı ve Reservation’a aktarıldı.",
                        "Başarılı",
                        MessageBoxButtons.OK, MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Rezervasyon aktarımı sırasında hata:\n{ex.Message}",
                        "Onaylama Hatası",
                        MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    // hem grid’deki, hem DB’deki Onayla’yı geri al
                    row.Cells["IsApproved"].Value = false;
                    using var conn2 = new SqlConnection(_connString);
                    using var corr = new SqlCommand(
                        "UPDATE ReservationRequests SET Onayla = 0 WHERE RequestID = @rid",
                        conn2);
                    corr.Parameters.AddWithValue("@rid", requestId);
                    conn2.Open();
                    corr.ExecuteNonQuery();
                }

                // c) Sahibin SMS’i
                var ownerRow = allRows.First(r =>
                    Convert.ToBoolean(r.Cells["IsOwner"].Value));
                string ownerPhone = ownerRow.Cells["GuestPhone"].Value?.ToString();
                if (!string.IsNullOrWhiteSpace(ownerPhone))
                    SmsHelper.SendReservationSms(ownerPhone, requestId, reqDate, guestCount);

                // d) Misafirlerin SMS’i
                foreach (var guestRow in allRows.Where(r =>
                         !Convert.ToBoolean(r.Cells["IsOwner"].Value)))
                {
                    string guestPhone = guestRow.Cells["GuestPhone"].Value?.ToString();
                    if (!string.IsNullOrWhiteSpace(guestPhone))
                        SmsHelper.SendReservationSms(guestPhone, requestId, reqDate, guestCount);
                }

                // 4) Güncel listeyi yeniden çek (hem not, hem picture gelir)
                btnLoadRequests.PerformClick();
            }
        }

        /// <summary>
        /// byte[] ham veriyi Image’a çevirip unbound colPicture hücresine atar.
        /// </summary>
        private void DgvRequests_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvRequests.Columns[e.ColumnIndex].Name != "colPicture") return;

            var drv = dgvRequests.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (drv == null) return;

            var raw = drv["Picture"] as byte[];
            if (raw?.Length > 0)
            {
                using var ms = new MemoryStream(raw);
                e.Value = Image.FromStream(ms);
            }
            else
            {
                e.Value = null;
            }

            e.FormattingApplied = true;
        }
        private void DgvRequests_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvRequests.Columns[e.ColumnIndex].Name != "colUpload")
                return;

            var row = dgvRequests.Rows[e.RowIndex];
            int requestId = Convert.ToInt32(row.Cells["RequestID"].Value);
            int guestId = Convert.ToInt32(row.Cells["GuestID"].Value);

            using var dlg = new OpenFileDialog
            {
                Filter = "Resim dosyaları|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Misafir resmi seçin"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            byte[] data = File.ReadAllBytes(dlg.FileName);

            // 1) DataTable’daki ham “Picture” sütununa yaz
            var table = (DataTable)dgvRequests.DataSource;
            table.Rows[e.RowIndex]["Picture"] = data;

            // 2) DB’ye kaydet
            using var conn = new SqlConnection(_connString);
            using var cmd = new SqlCommand(@"
        UPDATE ReservationRequestGuests
           SET Picture = @pic
         WHERE RequestID = @rid
           AND GuestID    = @gid", conn);
            cmd.Parameters.AddWithValue("@pic", data);
            cmd.Parameters.AddWithValue("@rid", requestId);
            cmd.Parameters.AddWithValue("@gid", guestId);
            conn.Open();
            cmd.ExecuteNonQuery();

            // 3) Sadece ilgili hücreyi yeniden çiz
            var cell = dgvRequests.Rows[e.RowIndex].Cells["colPicture"];
            dgvRequests.InvalidateCell(cell);

        }



    }
}
