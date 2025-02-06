using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp6
{
    public partial class UserManager : Application
    {
        public static User currentUser { get; set; }

        public static bool IsAdmin()
        {
            // Проверка на админа
            return currentUser != null && currentUser.Role != null && currentUser.Role.RoleName == "Администратор";
        }
    }
}
