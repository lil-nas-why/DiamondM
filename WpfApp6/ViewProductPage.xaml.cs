using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    public partial class ViewProductPage : Page
    {

        private ObservableCollection<Product> products;
        private ObservableCollection<Manufacturers> manufacturers;

        public ViewProductPage()
        {
            InitializeComponent();
            this.Loaded += ProductPage_Loaded;
            
        }

        private void UpdateDisplayedCount()
        {
            var displayedCount = (ProductLV.ItemsSource as IEnumerable<Product>)?.Count() ?? 0;
            var totalCount = products.Count;
            DisplayedCountText = $"Позиций: {displayedCount} из {totalCount}";
        }

        public string DisplayedCountText
        {
            get { return (string)GetValue(DisplayedCountTextProperty); }
            set { SetValue(DisplayedCountTextProperty, value); }
        }

        public static readonly DependencyProperty DisplayedCountTextProperty =
            DependencyProperty.Register("DisplayedCountText", typeof(string), typeof(ViewProductPage), new PropertyMetadata("0 из 0"));


        private void ProductPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProducts();

            bool isAdmin = UserManager.IsAdmin();

            // Видимость кнопок
            DeleteProduct.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            AddProduct.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadProducts()
        {
            products = new ObservableCollection<Product>(TradeDBEntities.GetContext().Product.ToList());
            ProductLV.ItemsSource = products;

            manufacturers = new ObservableCollection<Manufacturers>(TradeDBEntities.GetContext().Manufacturers.ToList());

            // Добавляем элемент "Все производители" в начало списка
            manufacturers.Insert(0, new Manufacturers { Name = "Все производители" });

            // Привязываем список производителей к ComboBox
            ManufacturerFilterComboBox.ItemsSource = manufacturers;
            ManufacturerFilterComboBox.DisplayMemberPath = "Name"; // Отображаем название производителя
            ManufacturerFilterComboBox.SelectedIndex = 0; 
        }

        private void UpdateSource()
        {
            if(ProductLV == null|| SearchBar == null || ManufacturerFilterComboBox == null || SortComboBox == null)
            {
                return;
            }

            var source = TradeDBEntities.GetContext().Product.ToList();
            if (!String.IsNullOrWhiteSpace(SearchBar.Text))
            {
                source = source.Where(p =>
                p.ProductName.ToLower().Contains(SearchBar.Text) ||
                p.Description.ToLower().Contains(SearchBar.Text) ||
                p.Manufacturers.Name.ToLower().Contains(SearchBar.Text) ||
                p.ProductCost.ToString().Contains(SearchBar.Text) ||
                p.ProductCurrentDiscount.ToString().Contains(SearchBar.Text)).ToList();
            }


            var selectedManufacturer = ManufacturerFilterComboBox.SelectedItem as Manufacturers;
            if (selectedManufacturer != null && selectedManufacturer.Name != "Все производители")
            {
                source = source.Where(p => p.Manufacturers.Name == selectedManufacturer.Name).ToList();
            }

            if (SortComboBox.SelectedIndex == 0) // По возрастанию
            {
                source = source.OrderBy(p => p.ProductCost).ToList();
            }
            else if (SortComboBox.SelectedIndex == 1) // По убыванию
            {
                source = source.OrderByDescending(p => p.ProductCost).ToList();
            }


            ProductLV.ItemsSource = new ObservableCollection<Product>(source);
            UpdateDisplayedCount();
        }

        private void ManufacturerFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           UpdateSource();
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductLV.SelectedItem as Product;
            if (selectedProduct != null)
            {
                if (selectedProduct.QuantityWarehouse > 0)
                {
                    MessageBox.Show("Товар присутствует на складе и не может быть удален.");
                    return;
                }

                TradeDBEntities.GetContext().Product.Remove(selectedProduct);
                TradeDBEntities.GetContext().SaveChanges();
                LoadProducts();
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Навигация на страницу добавления товара
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new LogInPage());
        }

        private void ProductLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SearchBar_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            UpdateSource();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSource();
        }
    }
}
