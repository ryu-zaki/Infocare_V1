﻿using Infocare_Project;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace Infocare_Project_1
{
    public partial class StaffDashboard : Form
    {
        private string LoggedInUsername;
        private string FirstName;
        private string LastName;

        public StaffDashboard(string usrnm, string firstName, string lastName)
        {
            InitializeComponent();

            LoggedInUsername = usrnm;
            FirstName = firstName;
            LastName = lastName;

            NameLabel.Text = $"{firstName}!";
        }

        private void PatientDashboard_Load(object sender, EventArgs e)
        {
            LoadSpecializations();
            AppointmentDatePicker.MinDate = DateTime.Today;
        }

        private void pd_BookAppointment_Click(object sender, EventArgs e)
        {
            SelectPatientPanel.Visible = true;
            BookAppPanel.Visible = true;
            SpecPanel.Visible = false;
            pd_DoctorPanel.Visible = false;
            BookingPanel.Visible = false;
            ViewAppointmentPanel.Visible = false;

            Database db = new Database();

            List<string> patientNames = db.GetPatientNames();

            PatientComboBox.Items.Clear();
            PatientComboBox.Items.Add("Select");

            foreach (var patientName in patientNames)
            {
                PatientComboBox.Items.Add(patientName);
            }

            PatientComboBox.SelectedIndex = 0;
        }


        private void pd_ViewAppointment_Click(object sender, EventArgs e)
        {
            ViewAppointmentPanel.Visible = true;
            AppointmentDataGridViewList.Visible = true;

            SelectPatientPanel.Visible = false;
            BookAppPanel.Visible = false;
            SpecPanel.Visible = false;
            pd_DoctorPanel.Visible = false;
            BookingPanel.Visible = false;

            ShowAppointmentList();
        }

        private void pd_EditInfo_Click(object sender, EventArgs e)
        {
        }

        private void pd_SpecBtn_Click(object sender, EventArgs e)
        {
            Database db = new Database();

            if (pd_SpecBox.SelectedItem == null || pd_SpecBox.SelectedItem.ToString() == "Select")
            {
                MessageBox.Show("Please select a valid doctor's specialization.");
                return;
            }

            string selectedSpecialization = pd_SpecBox.SelectedItem.ToString();

            DialogResult result = MessageBox.Show($"You selected '{selectedSpecialization}' as the specialization. Would you like to proceed?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                SelectPatientPanel.Visible = false;
                SpecPanel.Visible = false;
                pd_DoctorPanel.Visible = true;
                BookAppPanel.Visible = true;

                List<string> doctorNames = db.GetDoctorNames(selectedSpecialization);

                pd_DocBox.Items.Clear();
                pd_DocBox.Items.Add("Select");

                foreach (var doctorName in doctorNames)
                {
                    pd_DocBox.Items.Add(doctorName);
                }

                pd_DocBox.SelectedIndex = 0;
            }
            else if (result == DialogResult.No)
            {
                SpecPanel.Visible = true;
                BookAppPanel.Visible = true;
            }

            LoadConsFee();
        }

        private void pd_DocBtn_Click(object sender, EventArgs e)
        {
            if (pd_DocBox.SelectedItem == null || pd_DocBox.SelectedItem.ToString() == "Select")
            {
                MessageBox.Show("Please select a doctor.");
                return;
            }

            string selectedDoctor = pd_DocBox.SelectedItem.ToString();
            Database db = new Database();

            var timeSlots = db.GetDoctorAvailableTimes(selectedDoctor);

            SelectPatientPanel.Visible = false;
            SpecPanel.Visible = false;
            pd_DoctorPanel.Visible = false;
            BookAppPanel.Visible = true;
            BookingPanel.Visible = true;

            if (timeSlots.Count > 0)
            {
                TimeCombobox.Items.Clear();
                TimeCombobox.Items.Add("Select a Time Slot");

                foreach (var timeSlot in timeSlots)
                {
                    TimeCombobox.Items.Add(timeSlot);
                }

                TimeCombobox.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No time slots found for this doctor.");
            }


        }


        private void pd_DocBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (pd_DocBox.SelectedItem != null && pd_DocBox.SelectedItem.ToString() != "Select")
            {
                string selectedDoctor = pd_DocBox.SelectedItem.ToString();
                Database db = new Database();

                string availability = db.GetDoctorAvailability(selectedDoctor);

                if (!string.IsNullOrEmpty(availability))
                {
                    List<DayOfWeek> availableDays = ParseDayAvailability(availability);

                    ConfigureMonthCalendar(AppointmentDatePicker, availableDays);
                }
                else
                {
                    MessageBox.Show("No date availability data found for the selected doctor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private List<DayOfWeek> ParseDayAvailability(string availability)
        {
            List<DayOfWeek> days = new List<DayOfWeek>();
            string[] dayNames = availability.Split(new[] { '-', ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string day in dayNames)
            {
                if (Enum.TryParse(day.Trim(), true, out DayOfWeek dayOfWeek))
                {
                    days.Add(dayOfWeek);
                }
            }

            return days;
        }

        private void ConfigureMonthCalendar(MonthCalendar calendar, List<DayOfWeek> availableDays)
        {
            DateTime today = DateTime.Today;
            DateTime minDate = today.AddDays(4); // Allow selection starting 4 days from today

            calendar.MinDate = minDate;
            calendar.MaxSelectionCount = 1;

            calendar.DateChanged += (s, e) =>
            {
                if (e.Start < minDate || !availableDays.Contains(e.Start.DayOfWeek))
                {
                    MessageBox.Show("The selected date is unavailable. Please select a valid day.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    calendar.SetDate(minDate);
                }
            };
        }
        private void EnterPatientButton_Click(object sender, EventArgs e)
        {
            string selectedpatient = PatientComboBox.SelectedItem.ToString();

            if (PatientComboBox.SelectedItem == null || PatientComboBox.SelectedItem.ToString() == "Select")
            {
                MessageBox.Show("Please select a valid patient.");
                return;
            }

            DialogResult result = MessageBox.Show($"You selected '{selectedpatient}' as the patient. Would you like to proceed?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                SelectPatientPanel.Visible = false;
                SpecPanel.Visible = true;
                BookAppPanel.Visible = true;
                pd_DoctorPanel.Visible = false;
                BookingPanel.Visible = false;

                LoadSpecializations();
            }
            else if (result == DialogResult.No)
            {
                SelectPatientPanel.Visible = true;
                BookAppPanel.Visible = true;
            }
        }

        private void LoadSpecializations()
        {
            try
            {
                Database db = new Database();

                var Specialties = db.GetSpecialization();

                pd_SpecBox.Items.Clear();

                pd_SpecBox.Items.Add("Select");

                // Add doctor names to the ComboBox
                pd_SpecBox.Items.AddRange(Specialties.ToArray());
                pd_SpecBox.SelectedIndex = 0; // Set default selection
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadConsFee()
        {
            try
            {
                Database db = new Database();

                string selectedSpecialization = pd_SpecBox.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(selectedSpecialization) || selectedSpecialization == "Select")
                {
                    MessageBox.Show("Please select a valid specialization.");
                    return;
                }

                decimal? consultationFee = db.GetConsultationFee(selectedSpecialization);

                if (consultationFee.HasValue)
                {
                    ConsFeeLbl.Text = $"{consultationFee}";
                }
                else
                {
                    ConsFeeLbl.Text = "Consultation Fee: Not Available";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading consultation fee: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfirmBookBtn_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to confirm this booking?",
                "Confirm Booking",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    Database db = new Database();

                    string selectedPatient = PatientComboBox.SelectedItem?.ToString() ?? string.Empty;
                    string selectedDoctor = pd_DocBox.SelectedItem?.ToString() ?? string.Empty;
                    string selectedTimeSlot = TimeCombobox.SelectedItem?.ToString() ?? string.Empty;
                    DateTime appointmentDate = AppointmentDatePicker.SelectionStart;
                    string specialization = pd_SpecBox.SelectedItem?.ToString() ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(selectedPatient) ||
                        string.IsNullOrWhiteSpace(selectedDoctor) ||
                        string.IsNullOrWhiteSpace(selectedTimeSlot) ||
                        string.IsNullOrWhiteSpace(specialization))
                    {
                        MessageBox.Show("Please ensure all fields are filled out correctly.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (selectedTimeSlot == "Select a Time Slot")
                    {
                        MessageBox.Show("Please select a valid time slot.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (db.IsPatientAppointmentPendingOrAccepted(selectedPatient))
                    {
                        MessageBox.Show("This patient already has a pending or accepted appointment. They cannot book another appointment until the current one is completed.", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (db.IsDoctorOccupied(selectedDoctor, appointmentDate))
                    {
                        MessageBox.Show("This doctor is already occupied for the selected date. Please choose another doctor or date.", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string feeText = ConsFeeLbl.Text.Trim();
                    if (!decimal.TryParse(feeText, out decimal consultationFee))
                    {
                        MessageBox.Show("Invalid consultation fee.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    bool appointmentSaved = db.SaveAppointment(selectedPatient, specialization, selectedDoctor, selectedTimeSlot, appointmentDate, consultationFee);

                    if (appointmentSaved)
                    {
                        MessageBox.Show("Appointment saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to save appointment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowAppointmentList()
        {

            Database db = new Database();
            try
            {
                DataTable AppointmentData = db.AppointmentList();
                if (AppointmentData.Rows.Count > 0)
                {
                    AppointmentDataGridViewList.DataSource = AppointmentData;
                }
                else
                {
                    MessageBox.Show("No Appointment History data found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Appointment History data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Staff_ExitButton_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("Are you sure you want to close? Unsaved changes will be lost.", "Please Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)

            {
                this.Close();
            }
        }

        private void Staff_MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pd_logoutlabel_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("Are you sure you want to Log Out?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                StaffLogin patientLoginForm = new StaffLogin();
                patientLoginForm.Show();
                this.Hide();
            }
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {

        }
        private void pd_HomeButton_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("Are you sure you want to Log Out?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                StaffLogin patientLoginForm = new StaffLogin();
                patientLoginForm.Show();
                this.Hide();
            }
        }
    }
}
