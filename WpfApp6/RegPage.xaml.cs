using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp6
{
    public partial class RegPage : Page
    {
        private TradeDBEntities dbe;
        public RegPage()
        {
            InitializeComponent();
            dbe = TradeDBEntities.GetContext();
        }

        private void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new LogInPage());
        }

        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string selectedRole = (AddRoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                if (string.IsNullOrEmpty(selectedRole))
                {
                    MessageBox.Show("Please select a role.");
                    return;
                }

                int roleId = DetermineRoleId(selectedRole);

                Role role = dbe.Role.Find(roleId);
                if (role == null)
                {
                    MessageBox.Show("Выбранная роль не найдена в базе данных.");
                    return;
                }

                    User userReg = new User();
                {
                    userReg.UserLogin = LogBox.Text;
                    userReg.UserPassword = PassBox.Password;
                    userReg.Role = role;
                }

                UserManager.currentUser = userReg;
                dbe.User.Add(userReg);
                dbe.SaveChanges();
                FrameManager.MainFrame.Navigate(new ViewProductPage());
            
            }
            catch (Exception ex) {

                MessageBox.Show(ex.Message);
            }
        }

        private int DetermineRoleId(string roleName)
        {
            switch (roleName)
            {
                case "Администратор":
                    return 1;
                case "Менеджер":
                    return 2;
                case "Клиент":
                    return 3;
                default:
                    throw new InvalidOperationException("Invalid role selected.");
            }
        }
    }
}
