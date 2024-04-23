using System;
using System.Collections.Generic;

namespace DTO
{
    public class productoDTO
    {
        public int codigo { get; set; }
        public string nombre { get; set; }
        public int precio { get; set; }
        public int? tamanio { get; set; }
        public int categoria { get; set; }
        public string name_categoria { get; set; }
        public int precio_ing { get; set; }
        public List<TamanioPizzaDTO> tamanio_precio = new List<TamanioPizzaDTO>();
        public string ean { get; set; }
        //etapa1
        public string marca { get; set; }
        public string color { get; set; }

        public int? idmarca { get; set; }
        public int? idcolor { get; set; }
        public decimal? stock_min { get; set; }
        public decimal? stock_tot { get; set; }
        public int? idinventario { get; set; }
        public int? precio_costo { get; set; }
        public string centro { get; set; }
        public int idmedida { get; set; }
        public int? estado { get; set; }
        public int autorizacion  { get; set; }

    }

}
