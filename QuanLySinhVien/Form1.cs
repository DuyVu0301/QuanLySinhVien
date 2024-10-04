using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;

namespace QuanLySinhVien
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadFaculties();
        }
        private void LoadData()
        {
            using (var context = new QLSV())
            {
                dataGridView1.DataSource = context.Students.Include(s => s.Faculty).ToList();
            }
        }

        private void LoadFaculties()
        {
            using (var context = new QLSV())
            {
                cbxFaculty.DataSource = context.Faculties.ToList();
                cbxFaculty.DisplayMember = "FacultyName";
                cbxFaculty.ValueMember = "FacultyID";
            }
        }
     

     
   
  
        private void ResetFields()
        {
            txtMSSV.Clear();
            txtHoTen.Clear();
            txtDiemTB.Clear();
            cbxFaculty.SelectedIndex = -1;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (txtMSSV.Text.Length != 3)
            {
                MessageBox.Show("Mã số sinh viên phải có đúng 3 chữ số!");
                return;
            }

            // Kiểm tra nếu MSSV không phải là số
            if (!int.TryParse(txtMSSV.Text, out int studentId))
            {
                MessageBox.Show("Mã số sinh viên phải là số!");
                return;
            }

            using (var context = new QLSV())
            {
                // Kiểm tra xem MSSV đã tồn tại chưa
                var existingStudent = context.Students.Find(studentId);
                if (existingStudent != null)
                {
                    MessageBox.Show("Mã số sinh viên đã tồn tại!");
                    return;
                }

                // Tạo đối tượng sinh viên mới
                var student = new Student
                {
                    StudentID = studentId,
                    FullName = txtHoTen.Text,
                    FacultyID = (int)cbxFaculty.SelectedValue
                };

                // Kiểm tra và gán điểm trung bình
                if (float.TryParse(txtDiemTB.Text, out float averageScore))
                {
                    student.AverageScore = averageScore;
                }
                else
                {
                    MessageBox.Show("Điểm trung bình không hợp lệ!");
                    return; // Ngăn không cho tiếp tục
                }

                // Thêm sinh viên vào cơ sở dữ liệu
                context.Students.Add(student);
                context.SaveChanges();

                MessageBox.Show("Thêm mới dữ liệu thành công!");
                LoadData();
                ResetFields();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {

            if (!int.TryParse(txtMSSV.Text, out int studentId))
            {
                MessageBox.Show("Mã số sinh viên phải là số!");
                return;
            }

            using (var context = new QLSV())
            {
                var student = context.Students.Find(studentId);
                if (student != null)
                {
                    student.FullName = txtHoTen.Text;

                    if (float.TryParse(txtDiemTB.Text, out float averageScore))
                    {
                        student.AverageScore = averageScore;
                    }
                    else
                    {
                        MessageBox.Show("Điểm trung bình không hợp lệ!");
                        return; // Ngăn không cho tiếp tục
                    }

                    student.FacultyID = (int)cbxFaculty.SelectedValue;

                    context.SaveChanges();
                    MessageBox.Show("Cập nhật dữ liệu thành công!");
                    LoadData();
                    ResetFields();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy MSSV cần sửa!");
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtMSSV.Text, out int studentId))
            {
                MessageBox.Show("Mã số sinh viên phải là số!");
                return;
            }

            using (var context = new QLSV())
            {
                var student = context.Students.Find(studentId);
                if (student == null)
                {
                    MessageBox.Show("Không tìm thấy MSSV cần xóa!");
                    return;
                }

                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    context.Students.Remove(student);
                    context.SaveChanges();
                    MessageBox.Show("Xóa sinh viên thành công!");
                    LoadData();
                    ResetFields();
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridView1.Rows[e.RowIndex];
                txtMSSV.Text = row.Cells["StudentID"].Value.ToString();
                txtHoTen.Text = row.Cells["FullName"].Value.ToString();
                txtDiemTB.Text = row.Cells["AverageScore"].Value.ToString();
                cbxFaculty.SelectedValue = row.Cells["FacultyID"].Value;
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
            
        }

    }
}
