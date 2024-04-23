using ErpAdmin.INVENTARIO;
using ErpAdmin.PRODUCTO;
using ErpAdmin.USUARIOS;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace ErpClass
{
    /// <summary>
    /// Lógica de interacción para Home.xaml
    /// </summary>
    public partial class Menu : Page
    {
        Brush color = new BrushConverter().ConvertFromString("#E9E9E9") as Brush;
        public Menu()
        {
            InitializeComponent();
            cargarBtMenu();
            lb_usuario.Content = GlobalClass.user_nombre;
            lb_rol.Content = GlobalClass.user_rol_nombre;
        }
        private void mouseOverMenu_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                cargarPagina(sender);
            }
        }
        private void mouseOverSalir_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                CloseWindow();
            }
        }
        public void CloseWindow()
        {
            var parentWindow = Window.GetWindow(this);

            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }

        private List<MenuButton> getMenuUser()
        {
            int rol = GlobalClass.user_rol;
            List<MenuButton> bt_menu = new List<MenuButton>();
            switch (rol)
            {
                case 1:
                    bt_menu.Add(new MenuButton { idmenu = "venta", nombre = "VENTA" });
                    bt_menu.Add(new MenuButton { idmenu = "caja", nombre = "CAJA" });
                    bt_menu.Add(new MenuButton { idmenu = "inventario", nombre = "INVENTARIO" });
                    bt_menu.Add(new MenuButton { idmenu = "factura", nombre = "FACTURAS" });

                    break;
                case 2:
                    bt_menu.Add(new MenuButton { idmenu = "pedido", nombre = "PEDIDO" });
                    bt_menu.Add(new MenuButton { idmenu = "cambio", nombre = "CAMBIO" });
                    break;
                case 3:
                    bt_menu.Add(new MenuButton { idmenu = "reporteventa", nombre = "Home" });
                    bt_menu.Add(new MenuButton { idmenu = "rep_vent_dia", nombre = "Ventas del dia" });
                    bt_menu.Add(new MenuButton { idmenu = "pagecajadia", nombre = "Caja por fecha" });
                    bt_menu.Add(new MenuButton { idmenu = "inventario", nombre = "INVENTARIO" });
                    bt_menu.Add(new MenuButton { idmenu = "categoria", nombre = "PRO.CATEGORIA" });
                    bt_menu.Add(new MenuButton { idmenu = "bodega", nombre = "BODEGAS" });
                    bt_menu.Add(new MenuButton { idmenu = "mod_usuario", nombre = "USUARIOS" });
                    bt_menu.Add(new MenuButton { idmenu = "factura", nombre = "FACTURAS" });

                    break;
                case 4:
                    bt_menu.Add(new MenuButton { idmenu = "inventario", nombre = "INVENTARIO" });
                    break;
                case 5:
                    bt_menu.Add(new MenuButton { idmenu = "inventario", nombre = "INVENTARIO" });
                    bt_menu.Add(new MenuButton { idmenu = "categoria", nombre = "PRO.CATEGORIA" });
                    bt_menu.Add(new MenuButton { idmenu = "bodega", nombre = "BODEGAS" });
                   
                    break;
            }
            return bt_menu;
        }
        private void cargarBtMenu()
        {
            List<MenuButton> menuButtons = getMenuUser();
            foreach (var bt_menu in menuButtons)
            {
                TextBlock textBlock = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Padding = new Thickness(20),
                    Text = bt_menu.nombre,
                    FontSize = 15,
                    FontWeight = FontWeights.Bold,
                    Foreground = color
                };
                StackPanel stack = new StackPanel
                {
                    Tag = bt_menu.idmenu
                };

                stack.MouseDown += new MouseButtonEventHandler(mouseOverMenu_PreviewMouseDown);
                stack.Children.Add(textBlock);
                menu_.Children.Add(stack);

            }
            TextBlock text_salir = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding = new Thickness(20),
                Text = "SALIR",
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Foreground = color
            };
            StackPanel bt_salir = new StackPanel();

            bt_salir.MouseDown += new MouseButtonEventHandler(mouseOverSalir_PreviewMouseDown);
            bt_salir.Children.Add(text_salir);
            menu_.Children.Add(bt_salir);


            StackPanel btn_menu_firt = (StackPanel)menu_.Children[0];

            btn_menu_firt.Background = color;
            TextBlock text = btn_menu_firt.Children[0] as TextBlock;
            text.Foreground = Brushes.Black;
            selPagina(btn_menu_firt);
        }
        private void cargarPagina(object btn_menu_boton)
        {
            foreach (var bt_menu_obj in menu_.Children)
            {

                var bt_menu = (StackPanel)bt_menu_obj;
                if (bt_menu.Background == color)
                {
                    bt_menu.Background = Brushes.Transparent;
                    TextBlock te = bt_menu.Children[0] as TextBlock;
                    te.Foreground = color;
                }
              
            }
            var bt_menu_sel = (StackPanel)btn_menu_boton;
            bt_menu_sel.Background = color;

            var bt_menu_tex = (TextBlock)bt_menu_sel.Children[0];
            bt_menu_tex.Foreground = Brushes.Black;
            
            selPagina(btn_menu_boton);
        }
        private void selPagina(object btn_menu_sel)
        {
            string id_menu = ((StackPanel)btn_menu_sel).Tag.ToString();

            switch (id_menu)
            {
                case "pagecajadia":
                    cajadia cajadia = new cajadia();
                    menu.Navigate(cajadia);

                        break;
                case "caja":
                    Pagecaja ca = new Pagecaja();
                    menu.Navigate(ca);
                    break;
                case "venta":
                    Pagepedido pe = new Pagepedido();
                    menu.Navigate(pe);
                    break;
                case "pedido":
                    Pagenotaventa notaventa = new Pagenotaventa();
                    menu.Navigate(notaventa);
                    break;
                case "cambio":
                    modal_cambio cambio = new modal_cambio();
                    menu.Navigate(cambio);
                    break;
                case "inventario":
                    Pageproductos inv = new Pageproductos();
                    menu.Navigate(inv);
                    break;
                case "rep_vent_dia":
                    pageventadia vendia = new pageventadia();
                    menu.Navigate(vendia);
                    break;
                case "caja_fecha":
                    cajadia caja_fecha = new cajadia();
                    menu.Navigate(caja_fecha);
                    break;
                case "mod_usuario":
                    ModuloUser mod_user = new ModuloUser();
                    menu.Navigate(mod_user);
                    break;
                case "categoria":
                    ModuloCategoria categoria = new ModuloCategoria();
                    menu.Navigate(categoria);
                    break;
                case "bodega":
                    ModuloBodega bodega = new ModuloBodega();
                    menu.Navigate(bodega);
                    break;
                case "reporteventa":
                    reporteventa reporteventa = new reporteventa();
                    menu.Navigate(reporteventa);
                    break;
                case "reportebodega":
                    ModuloBodega reportebodega = new ModuloBodega();
                    menu.Navigate(reportebodega);
                    break;
                case "reporteflete":
                    ModuloBodega reporteflete = new ModuloBodega();
                    menu.Navigate(reporteflete);
                    break;
                case "factura":
                    pagefactura pagefactura = new pagefactura();
                    menu.Navigate(pagefactura);
                    break;
            }
        }
        private void menu_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            var ta = new ThicknessAnimation();
            ta.Duration = TimeSpan.FromSeconds(0.3);
            ta.DecelerationRatio = 0.7;
            ta.To = new Thickness(0, 0, 0, 0);
            if (e.NavigationMode == NavigationMode.New)
            {
                ta.From = new Thickness(500, 0, 0, 0);
            }
            else if (e.NavigationMode == NavigationMode.Back)
            {
                ta.From = new Thickness(0, 0, 500, 0);
            }
                (e.Content as Page).BeginAnimation(MarginProperty, ta);
        }
    }
    class MenuButton
    {
        public string idmenu { get; set; }
        public string nombre { get; set; }
    }
}
