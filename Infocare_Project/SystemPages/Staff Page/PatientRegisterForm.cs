﻿using Guna.UI2.WinForms;
using Infocare_Project.NewFolder;
using Infocare_Project_1.Classes;
using Infocare_Project_1.Object_Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Infocare_Project
{
    public partial class PatientRegisterForm : Form
    {
        private PlaceHolderHandler _placeHolderHandler;
        int houseNo;
        int zipCode;
        int zone;
        public PatientRegisterForm()
        {
            InitializeComponent();
            _placeHolderHandler = new PlaceHolderHandler();
        }

        private void PatientRegisterForm_Load(object sender, EventArgs e)
        {
            BdayDateTimePicker.MaxDate = DateTime.Today;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("Are you sure to cancel registration?", "Cancel registraion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)

            {
                this.Close();
            }
        }

        private void EnterButton_Click(object sender, EventArgs e)
        {
            string contactNumber = ContactNumberTxtbox.Text;

            if (contactNumber.Length > 0 && (contactNumber.Length != 11 || !contactNumber.StartsWith("09") || !contactNumber.All(char.IsDigit)))
            {
                MessageBox.Show("Invalid number. The contact number must start with '09' and be exactly 11 digits.", "Invalid Number", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!FirstnameTxtbox.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)) && !string.IsNullOrEmpty(FirstnameTxtbox.Text))
            {
                MessageBox.Show("First name must contain only letters and spaces.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (!MiddleNameTxtbox.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)) && !string.IsNullOrEmpty(MiddleNameTxtbox.Text))
            {
                MessageBox.Show("Middle name must contain only letters and spaces.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!LastNameTxtbox.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)) && !string.IsNullOrEmpty(LastNameTxtbox.Text))
            {
                MessageBox.Show("Last name must contain only letters and spaces.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (!InputValidator.ValidateAlphabetic(FirstnameTxtbox, "First name must contain only letters. ex. (Juan)") ||
                !InputValidator.ValidateAlphabetic(LastNameTxtbox, "Last name must contain only letters. ex. (Dela Cruz)") ||
                !InputValidator.ValidateAlphabetic(CityTxtbox, "City must contain only letters. ex. (Caloocan)"))
            {
                return;
            }

            if (!InputValidator.ValidateNumeric(ContactNumberTxtbox, "Contact number must contain only numbers. ex.(09777864220)") ||
                !InputValidator.ValidateNumeric(ZipCodeTxtbox, "Zip Code must contain only numbers. ex. (1400)") ||
                !InputValidator.ValidateNumeric(ZoneTxtbox, "Zone must contain only numbers. ex. (1)"))
            {
                return;
            }


            string[] validSuffixes = { "Jr.", "Sr.", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "Jr", "Sr", "N/A"};

            string enteredText = SuffixTxtbox.Text.Trim();

            if (!string.IsNullOrEmpty(enteredText) && !validSuffixes.Any(suffix => string.Equals(suffix, enteredText, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Please enter a valid suffix (e.g., Jr., Sr., I, II, III, IV, etc.).", "Invalid Suffix", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


           

            int ZipCode;
            int Zone;

            if (!int.TryParse(ZipCodeTxtbox.Text, out ZipCode))
            {
                MessageBox.Show("Please enter a valid number for Zip Code.");
                return;
            }

            if (!int.TryParse(ZoneTxtbox.Text, out Zone))
            {
                MessageBox.Show("Please enter a valid number for Zone.");
                return;
            }
            
            if (Database.IsUsernameExists(UsernameTxtbox.Text))
            {
                MessageBox.Show("The username is already in use. Please choose a different username.");
                return;
            }

            AddressModel address = new AddressModel(houseNo, StreetTxtbox.Text, BarangayTxtbox.Text, CityTxtbox.Text, zipCode, zone);

            PatientModel newPatient = new PatientModel
            {
                FirstName = FirstnameTxtbox.Text,
                LastName = LastNameTxtbox.Text,
                MiddleName = MiddleNameTxtbox.Text,
                ContactNumber = ContactNumberTxtbox.Text,
                UserName = UsernameTxtbox.Text,
                Address = address
            };


            try
            {
                
                Database.PatientRegFunc(newPatient);

                var patientInfoForm = new PatientBasicInformationForm(newPatient);


                patientInfoForm.Show();

                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            Guna2TextBox[] requiredTextBoxes = {
                FirstnameTxtbox, LastNameTxtbox, MiddleNameTxtbox, SuffixTxtbox, CityTxtbox,
                ContactNumberTxtbox, ZipCodeTxtbox, ZoneTxtbox, StreetTxtbox, BarangayTxtbox, UsernameTxtbox
            };

            if (!InputValidator.ValidateAllFieldsFilled(requiredTextBoxes, "Please fill out all fields."))
            {
                return;
            }
           
        }

        private void UsernameTxtbox_TextChanged(object sender, EventArgs e)
        {
            _placeHolderHandler.HandleTextBoxPlaceholder(UsernameTxtbox, UserNameLabel, "Email");
        }

        private void ContactNumberTxtbox_TextChanged(object sender, EventArgs e)
        {
            _placeHolderHandler.HandleTextBoxPlaceholder(ContactNumberTxtbox, ContactNumberLabel, "Contact Number");

            
        }


        private void FirstnameTxtbox_TextChanged(object sender, EventArgs e)
        {
            _placeHolderHandler.HandleTextBoxPlaceholder(FirstnameTxtbox, FirstNameLabel, "First name");
        }

        private void LastNameTxtbox_TextChanged(object sender, EventArgs e)
        {
            _placeHolderHandler.HandleTextBoxPlaceholder(LastNameTxtbox, LastNameLabel, "Last name");
        }

        private void HouseNoTxtbox_TextChanged(object sender, EventArgs e)
        {
            _placeHolderHandler.HandleTextBoxPlaceholder(HouseNoTxtbox, HouseLabel, "House No.");
        }

        private void ZipCodeTxtbox_TextChanged(object sender, EventArgs e)
        {
            _placeHolderHandler.HandleTextBoxPlaceholder(ZipCodeTxtbox, ZipCodeLabel, "Zip Code");
        }

        private void ZoneTxtbox_TextChanged(object sender, EventArgs e)
        {
            _placeHolderHandler.HandleTextBoxPlaceholder(ZoneTxtbox, ZoneLabel, "Zone");
        }

        private void MiddleNameTxtbox_TextChanged(object sender, EventArgs e)
        {
            _placeHolderHandler.HandleTextBoxPlaceholder(MiddleNameTxtbox, MiddleNameLabel, "Middle name");
        }

        private void SuffixTxtbox_TextChanged(object sender, EventArgs e)
        {
            _placeHolderHandler.HandleTextBoxPlaceholder(SuffixTxtbox, SuffixLabel, "Suffix");
        }

        private void StreetTxtbox_TextChanged(object sender, EventArgs e)
        {
            _placeHolderHandler.HandleTextBoxPlaceholder(StreetTxtbox, StreetLabel, "Street");
        }

        private void BarangayTxtbox_TextChanged(object sender, EventArgs e)
        {
            _placeHolderHandler.HandleTextBoxPlaceholder(BarangayTxtbox, BarangayLabel, "Barangay");
        }

        private void CityTxtbox_TextChanged(object sender, EventArgs e)
        {
            _placeHolderHandler.HandleTextBoxPlaceholder(CityTxtbox, CityLabel, "City");
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Control[] textBoxes = {
                                    UsernameTxtbox, FirstnameTxtbox, LastNameTxtbox, MiddleNameTxtbox, SuffixTxtbox, CityTxtbox,
                                    ContactNumberTxtbox, ZipCodeTxtbox, ZoneTxtbox, StreetTxtbox, BarangayTxtbox
                                  };

            if (textBoxes.All(tb => string.IsNullOrWhiteSpace(tb.Text)))
            {
                DialogResult confirm = MessageBox.Show("Are you sure you want to go back?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    this.Hide();
                }
            }
            else if (textBoxes.Any(tb => !string.IsNullOrWhiteSpace(tb.Text)))
            {
                DialogResult confirm = MessageBox.Show("Some fields are filled. Are you sure you want to go back? Unsaved changes may be lost.", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    StaffLogin patientLoginForm = new StaffLogin();
                    patientLoginForm.Show();
                    this.Hide();

                }
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }
    }
}