using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DTO
{
    public class DetalleDTO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int cantidad;
        public int cantidad_disp;
        public int Total;
        public string ing;
        public string cantidad_formato { get; set; }
        public int categoria { get; set; }
        public int code_pedido { get; set; }
        public int code_detalle { get; set; }
        public int tamanio;
        public int subtotal { get; set; }
        public Visibility visibility { get; set; }
        public Visibility BtnVisible { get; set; }
        
        public string observacion { get; set; }
        private DateTime? fecha;
        public int codigo { get; set; }
        public int precio { get; set; }
        public int? precio_desc { get; set; }
        public int? desc_total { get; set; }
        public string ean { get; set; }
        public decimal? stock { get; set; }
        public int? idinventario { get; set; }

        public string producto { get; set; }
        public string marca { get; set; }
        public string color { get; set; }
        public string centro { get; set; }
        public int autorizacion { get; set; }
        public int idmedida { get; set; }
        public int CantidadDispo
        {
            get
            {
                return cantidad_disp;
            }

            set
            {
                cantidad_disp = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("CantidadDispo");
            }
        }

        public int Cantidad
        {
            get
            {
                return cantidad;
            }

            set
            {
                cantidad = value;
                Cantidad_formato = getCant();
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Cantidad");
            }
        }
        public Visibility ver 
        {
            get
            {
                return idmedida == 3 ? Visibility.Visible : Visibility.Collapsed;
            }  
        }
        public Visibility verBtn
        {
            get
            {
                return idmedida == 3 ?  Visibility.Collapsed: Visibility.Visible;
            }
        }
        public string Cantidad_formato
        {
            get
            {
                return cantidad_formato;
            }
            set
            {
                cantidad_formato = value;
                OnPropertyChanged("Cantidad_formato");
            }
        }
        public string stock_formato
        {
            get
            {
                return getStock();
            }
        }
        public int total
        {
            get
            {
                return Total;
            }

            set
            {
                Total = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("total");
            }
        }

        public DateTime? Fecha
        {
            get
            {
                return fecha;
            }

            set
            {
                fecha = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Fecha");
            }
        }
        public int Tamanio
        {
            get
            {
                return tamanio;
            }

            set
            {
                tamanio = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Tamanio");
            }
        }
        public string Ing
        {
            get
            {
                return ing;
            }

            set
            {
                ing = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Ing");
            }
        }
        public int? Precio_desc
        {
            get
            {
                return precio_desc;
            }

            set
            {
                precio_desc = value;
                int desc = 0;
                if (precio_desc != null)
                {
                    desc = precio_desc.Value;
                }
                if (idmedida == 3)
                {
                    total = (precio - desc) * cantidad;
                }
                else
                {
                     
                    total =  subtotal- desc;
                }
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Precio_desc");
            }
        }
        public string getStock()
        {
            string res = "";
            if (idmedida==1)
            {
                res = ((decimal)stock / 1000) + " KG";
            }
            else
            {
                res = stock + " UN";
            }
            return res;
        }
        public string getCant()
        {
            string res = "";
            if (idmedida == 1)
            {
                res = ((decimal)cantidad / 1000) + " KG";
            }
            else
            {
                res = cantidad + " UN";
            }
            return res;
        }
        public List<AddIngDTO> ingrediente = new List<AddIngDTO>();
        public Visibility Visible
        {
            get
            {
                return BtnVisible;
            }

            set
            {
                BtnVisible = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Visible");
            }
        }
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
