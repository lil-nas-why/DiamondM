using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    public partial class LogInPage : Page
    {
        private TradeDBEntities dbe;

        private string currentCaptcha;
        private int failedAttempts = 0;


        public LogInPage()
        {
            InitializeComponent();
            dbe = TradeDBEntities.GetContext();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {

            if (failedAttempts >= 3)
            {
                MessageBox.Show("Превышено количество попыток. Попробуйте позже.");
                return;
            }

            string login = LogInLogBox.Text;
            string password = LogInPassBox.Password;

            if (failedAttempts >= 1)
            {
                string captchaInput = CaptchaInput.Text;
                if (captchaInput != currentCaptcha)
                {
                    MessageBox.Show("Неверная CAPTCHA.");
                    GenerateAndDisplayCaptcha();
                    return;
                }
            }

            var user = dbe.User.FirstOrDefault(u => u.UserLogin == login && u.UserPassword == password);

            if (user != null)
            {
                UserManager.currentUser = user;
                FrameManager.MainFrame.Navigate(new ViewProductPage());
            }
            else
            {
                failedAttempts++;

                if (failedAttempts >= 1)
                {
                    GenerateAndDisplayCaptcha();
                }

                MessageBox.Show("Неверный логин или пароль.");
            }
        
    }

        private void GoToReg_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new RegPage());
        }


        private string GenerateCaptcha()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private BitmapImage GenerateCaptchaImage(string captchaText)
        {
            var bitmap = new RenderTargetBitmap(150, 50, 96, 96, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();

            using (var context = visual.RenderOpen())
            {
                var text = new FormattedText(
                    captchaText,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    24,
                    Brushes.Black);

                var random = new Random();
                for (int i = 0; i < 10; i++)
                {
                    context.DrawLine(
                        new Pen(Brushes.Gray, 1),
                        new Point(random.Next(150), random.Next(50)),
                        new Point(random.Next(150), random.Next(50)));
                }

                context.DrawText(text, new Point(10, 10));
            }

            bitmap.Render(visual);

            var bitmapImage = new BitmapImage();
            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        private void GenerateAndDisplayCaptcha()
        {
            ChBorder.Visibility = Visibility.Visible;
            currentCaptcha = GenerateCaptcha();
            CaptchaImage.Source = GenerateCaptchaImage(currentCaptcha);
            CaptchaPanel.Visibility = Visibility.Visible;
        }

        private void RefreshCaptcha_Click(object sender, RoutedEventArgs e)
        {
            GenerateAndDisplayCaptcha();
        }
    }
}
